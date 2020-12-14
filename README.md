# FFXIVPacketCapture
A packet scanner for FFXIV.

## Usage
```csharp
var scanner = new FFXIVPacketScanner();
scanner.OnIpcMessageReceived += (connection, message) =>
{
    Console.WriteLine(connection);
    Console.WriteLine(message.Header);
    Console.WriteLine(string.Join(" ", message.Data));
};
scanner.Start();
```
