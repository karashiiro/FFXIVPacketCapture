using System.Runtime.InteropServices;

namespace FFXIVPacketCapture
{
    [StructLayout(LayoutKind.Sequential)]
    public class IpcHeader
    {
        public ushort Reserved { get; }
        public ushort Type { get; }
        public ushort Padding { get; }
        public ushort ServerId { get; }
        public uint Timestamp { get; }
        public uint Padding1 { get; }

        public override string ToString()
            => $"Reserved:{Reserved};" +
               $"Type:{Type};" +
               $"Padding:{Padding};" +
               $"ServerId:{ServerId};" +
               $"Timestamp:{Timestamp};" +
               $"Padding1:{Padding1};";
    }
}