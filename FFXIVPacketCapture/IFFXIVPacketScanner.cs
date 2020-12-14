using System.Collections.Generic;

namespace FFXIVPacketCapture
{
    public delegate void PacketReceived(string connection, FFXIVPacket packet);
    public delegate void IpcMessageReceived(string connection, FFXIVIpcMessage message);

    public interface IFFXIVPacketScanner
    {
        public PacketReceived OnPacketReceived { get; set; }
        public IpcMessageReceived OnIpcMessageReceived { get; set; }

        public void Start();
        public void Stop();

        public IEnumerable<string> GetDevices();
        public void SetDevice(string macAddress);
    }
}