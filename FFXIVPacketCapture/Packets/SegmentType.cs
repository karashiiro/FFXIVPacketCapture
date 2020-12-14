namespace FFXIVPacketCapture.Packets
{
    public enum SegmentType : ushort
    {
        SessionInit = 1,
        Ipc = 3,
        KeepAlive = 7,
        //Response = 8,
        EncryptionInit = 9,
    }
}