# FFXIVPacketCapture
A packet scanner for FFXIV.

## Installation
`Install-Package FFXIVPacketCapture` or other methods as described [here](https://www.nuget.org/packages/FFXIVPacketCapture/).

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
