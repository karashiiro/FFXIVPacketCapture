using System.Runtime.InteropServices;

namespace FFXIVPacketCapture.Packets
{
    [StructLayout(LayoutKind.Sequential)]
    public class SegmentHeader
    {
        /// <summary>
        /// The size of the segment header and its data.
        /// </summary>
        public uint Size { get; }

        /// <summary>
        /// The session ID this segment describes.
        /// </summary>
        public uint SourceActor { get; }

        /// <summary>
        /// The session ID this packet is being delivered to.
        /// </summary>
        public uint TargetActor { get; }

        /// <summary>
        /// The segment type. (1, 2, 3, 7, 8, 9, 10)
        /// </summary>
        public SegmentType Type { get; }

        public ushort Padding { get; }

        public override string ToString()
            => $"Size:{Size};" +
               $"SourceActor:{SourceActor};" +
               $"TargetActor:{TargetActor};" +
               $"Type:{Type};" +
               $"Padding:{Padding};";
    }
}