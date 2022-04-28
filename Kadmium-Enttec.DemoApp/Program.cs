using Kadmium_Enttec;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
IEnttecWriter writer = new EnttecWriter();
await writer.OpenAsync("COM3");
byte[] payload = new byte[500];
payload[0] = 255;
payload[1] = 255;

while (true)
{
	await writer.WriteAsync(payload);
	await Task.Delay(1000 / 40);
}