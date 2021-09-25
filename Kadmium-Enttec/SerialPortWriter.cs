using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Kadmium_Enttec
{
	public class SerialPortWriter : ISerialPortWriter
	{
		private SerialPort Port { get; set; }

		public Task CloseAsync()
		{
			return Task.Run(() =>
			{
				Port?.Close();
			});
		}

		public async ValueTask DisposeAsync()
		{
			if (Port != null)
			{
				await CloseAsync();
				Port.Dispose();
			}
		}

		public Task OpenAsync(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
		{
			if (Port != null)
			{
				throw new InvalidOperationException("There is already a port open");
			}
			return Task.Run(() =>
			{
				Port = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
				Port.Open();
			});
		}

		public Task WriteAsync(byte[] data)
		{
			return Task.Run(() =>
			{
				Port.Write(data, 0, data.Length);
			});
		}

		public string PortName => Port?.PortName;

		public bool IsOpen => Port?.IsOpen ?? false;
	}
}