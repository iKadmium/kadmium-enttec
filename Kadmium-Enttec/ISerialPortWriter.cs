using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Kadmium_Enttec
{
	public interface ISerialPortWriter : IAsyncDisposable
	{
		Task WriteAsync(byte[] data);
		Task OpenAsync(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits);
		Task CloseAsync();
		string PortName { get; }
		bool IsOpen { get; }
	}
}