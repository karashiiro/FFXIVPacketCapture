using System.Runtime.InteropServices;

namespace FFXIVPacketCapture.Packets
{
    [StructLayout(LayoutKind.Sequential)]
    public class PacketHeader
    {
        public ulong Magic1 { get; }

        public ulong Magic2 { get; }

        /// <summary>
        /// Represents the number of milliseconds since epoch that the packet was sent.
        /// </summary>
        public ulong Timestamp { get; }

        /// <summary>
        /// The size of the packet header and its payload.
        /// </summary>
        public uint Size { get; }

        /// <summary>
        /// The type of this connection.
        /// </summary>
        public ConnectionType ConnectionType { get; }

        /// <summary>
        /// The number of packet segments that follow.
        /// </summary>
        public ushort SegmentCount { get; }

        public byte Unknown20 { get; }

        /// <summary>
        /// Indicates if the data segments of this packet are compressed.
        /// </summary>
        public bool IsCompressed { get; }

        public uint Unknown24 { get; }

        public override string ToString()
            => $"Magic1:{Magic1};" +
               $"Magic2:{Magic2};" +
               $"Timestamp:{Timestamp};" +
               $"Size:{Size};" +
               $"ConnectionType:{ConnectionType};" +
               $"SegmentCount:{SegmentCount};" +
               $"Unknown20:{Unknown20};" +
               $"IsCompressed:{IsCompressed};" +
               $"Unknown24:{Unknown24};";
    }
}