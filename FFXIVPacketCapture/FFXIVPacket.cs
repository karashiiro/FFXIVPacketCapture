using System.Collections.Generic;

namespace FFXIVPacketCapture
{
    public class FFXIVPacket
    {
        public PacketHeader PacketHeader { get; }
        public IEnumerable<Segment> Segments { get; }

        public FFXIVPacket(PacketHeader header, IEnumerable<Segment> segments)
        {
            PacketHeader = header;
            Segments = segments;
        }
    }
}