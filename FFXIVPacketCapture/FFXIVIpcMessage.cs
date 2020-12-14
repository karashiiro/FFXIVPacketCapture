namespace FFXIVPacketCapture
{
    public class FFXIVIpcMessage
    {
        public IpcHeader Header { get; }
        public byte[] Data { get; }

        public FFXIVIpcMessage(IpcHeader header, byte[] data)
        {
            Header = header;
            Data = data;
        }
    }
}