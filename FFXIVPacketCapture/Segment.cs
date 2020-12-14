namespace FFXIVPacketCapture
{
    public class Segment
    {
        public SegmentHeader Header { get; }
        public IpcHeader IpcHeader { get; }
        public byte[] IpcData { get; }

        public Segment(SegmentHeader header, IpcHeader ipcHeader, byte[] ipcData)
        {
            Header = header;
            IpcHeader = ipcHeader;
            IpcData = ipcData;
        }
    }
}