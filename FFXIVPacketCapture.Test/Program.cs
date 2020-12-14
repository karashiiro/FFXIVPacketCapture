using System;

namespace FFXIVPacketCapture.Test
{
    public static class Program
    {
        public static void Main()
        {
            var scanner = new FFXIVPacketScanner();
            scanner.OnIpcMessageReceived += (connection, message) =>
            {
                Console.WriteLine(connection);
                Console.WriteLine(message.Header);
                Console.WriteLine(string.Join(" ", message.Data));
            };
            scanner.Start();
        }
    }
}
