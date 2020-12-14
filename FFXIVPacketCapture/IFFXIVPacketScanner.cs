using System.Collections.Generic;
using FFXIVPacketCapture.Packets;

namespace FFXIVPacketCapture
{
    public delegate void PacketReceived(string connection, FFXIVPacket packet);
    public delegate void IpcMessageReceived(string connection, FFXIVIpcMessage message);

    public interface IFFXIVPacketScanner
    {
        /// <summary>
        /// Fires when a game packet is received.
        /// </summary>
        public PacketReceived OnPacketReceived { get; set; }

        /// <summary>
        /// Fires when a game packet with inter-process communication data is received.
        /// </summary>
        public IpcMessageReceived OnIpcMessageReceived { get; set; }

        /// <summary>
        /// Start packet capture on the loaded device, saving the results to a file if a filename is provided.
        /// </summary>
        public void Start(string captureFileName = null);

        /// <summary>
        /// Stop packet capture on the loaded device.
        /// </summary>
        public void Stop();

        /// <summary>
        /// Return the MAC addresses of all available network devices.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetDevices();

        /// <summary>
        /// Loads the network device with the specified MAC address.
        /// </summary>
        /// <param name="macAddress">The MAC address of the device to load.</param>
        public void SetDevice(string macAddress);
    }
}