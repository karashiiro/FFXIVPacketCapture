using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Npcap;

namespace FFXIVPacketCapture
{
    public class FFXIVPacketScanner : IFFXIVPacketScanner
    {
        private const string Filter = "tcp portrange 54992-54994 or tcp portrange 55006-55007 or tcp portrange 55021-55040 or tcp portrange 55296-55551";
        private const ulong Magic1 = 16304822851840528978;
        private const ulong Magic2 = 8486076352731294335;

        private readonly Mode _mode;
        private ICaptureDevice _device;

        public PacketReceived OnPacketReceived { get; set; }
        public IpcMessageReceived OnIpcMessageReceived { get; set; }

        /// <summary>
        /// Creates a new packet scanner on the first network device available.
        /// </summary>
        public FFXIVPacketScanner()
        {
            var devices = CaptureDeviceList.Instance;
            if (devices.Count < 1)
            {
                throw new Exception("No valid network capture devices were found on this machine.");
            }

            _device = devices[0];
            _mode = Mode.Live;
        }

        /// <summary>
        /// Creates a new packet scanner on the specified capture file.
        /// </summary>
        /// <param name="captureFileName">The pcap capture file.</param>
        public FFXIVPacketScanner(string captureFileName)
        {
            _device = new CaptureFileReaderDevice(captureFileName);
            _mode = Mode.File;
        }

        public void Start(string captureFileName = null)
        {
            _device.OnPacketArrival += DeviceOnPacketArrival;
            if (!string.IsNullOrEmpty(captureFileName))
            {
                var captureWriter = new CaptureFileWriterDevice(captureFileName);
                _device.OnPacketArrival += (sender, args) => captureWriter.Write(args.Packet);
            }

            if (_mode == Mode.File)
                _device.Open();
            else if (_device is NpcapDevice nPCap)
                nPCap.Open(OpenFlags.DataTransferUdp | OpenFlags.NoCaptureLocal | OpenFlags.MaxResponsiveness, 1000);
            else
                _device.Open(DeviceMode.Promiscuous, 1000);

            _device.Filter = Filter;
            _device.StartCapture();
        }

        public void Stop()
        {
            _device.StopCapture();
            _device.Close();
        }

        public IEnumerable<string> GetDevices()
            => CaptureDeviceList.Instance.Select(d => d.MacAddress.ToString());

        public void SetDevice(string macAddress) =>
            _device = CaptureDeviceList.Instance.First(device => device.MacAddress.ToString() == macAddress);

        private unsafe void DeviceOnPacketArrival(object sender, CaptureEventArgs e)
        {
            var fullPacket = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            var packet = fullPacket.Extract<TcpPacket>();
            if (!packet.Push) return; // Only TCP PSH has actual data; ACK does not

            var ipv4Info = fullPacket.Extract<IPv4Packet>();
            var connection = new StringBuilder(ipv4Info.SourceAddress.ToString())
                .Append(':')
                .Append(packet.SourcePort.ToString())
                .Append("=>")
                .Append(ipv4Info.DestinationAddress)
                .Append(':')
                .Append(packet.DestinationPort.ToString())
                .ToString();

            // All FFXIV packets begin with a packet header...
            fixed (void* payload = &packet.PayloadData[0])
            {
                var packetHeader = Marshal.PtrToStructure<PacketHeader>(new IntPtr(payload));
                if (!IsMagical(packetHeader)) return;

                // ...possibly with a compressed payload,
                using var remainder = new UnmanagedMemoryStream(
                    (byte*)payload + 40,
                    packet.PayloadData.Length - 40,
                    packet.PayloadData.Length * 2,
                    FileAccess.ReadWrite);

                if (packetHeader.IsCompressed)
                {
                    using var deflateStream = new DeflateStream(remainder, CompressionMode.Decompress);
                    deflateStream.CopyTo(remainder);
                }

                // ...followed by one or more segment headers,
                var segmentPtr = remainder.PositionPointer;
                var segments = new List<Segment>();
                for (var i = 0; i < packetHeader.SegmentCount; i++)
                {
                    var segmentHeader = Marshal.PtrToStructure<SegmentHeader>(new IntPtr(segmentPtr));

                    // ...potentially with an IPC header and body.
                    IpcHeader ipcHeader = null;
                    var ipcData = new byte[0];
                    if (segmentHeader.Type == SegmentType.Ipc)
                    {
                        ipcHeader = Marshal.PtrToStructure<IpcHeader>(new IntPtr(segmentPtr + 16));
                        ipcData = new byte[segmentHeader.Size];
                        using var ipcDataStreamUnmanaged = new UnmanagedMemoryStream(segmentPtr + 32, segmentHeader.Size);
                        using var ipcDataStream = new MemoryStream(ipcData, writable:true);
                        ipcDataStreamUnmanaged.CopyTo(ipcDataStream);

                        OnIpcMessageReceived?.Invoke(connection, new FFXIVIpcMessage(ipcHeader, ipcData));

                        segmentPtr += 16 + segmentHeader.Size;
                    }

                    segments.Add(new Segment(segmentHeader, ipcHeader, ipcData));
                }

                OnPacketReceived?.Invoke(connection, new FFXIVPacket(packetHeader, segments));
            }
        }

        private static bool IsMagical(PacketHeader header)
            => header.Magic1 == Magic1 && header.Magic2 == Magic2 ||
               header.Magic1 + header.Magic2 == 0;

        private enum Mode
        {
            Live,
            File,
        }
    }
}
