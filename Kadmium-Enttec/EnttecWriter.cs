using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Kadmium-Enttec.Test")]
namespace Kadmium_Enttec
{
	public class EnttecWriter : IEnttecWriter, IAsyncDisposable
	{
		private const int DMX_PRO_MIN_PAYLOAD_SIZE = 24;
		private const int DMX_PRO_MAX_PAYLOAD_SIZE = 512;
		private const byte DMX_PRO_MESSAGE_START = 0x7E;
		private const byte DMX_PRO_MESSAGE_END = 0xE7;
		private const byte DMX_PRO_SEND_PACKET = 6;
		private const byte DMX_COMMAND_BYTE = 0;

		private const int BAUD_RATE = 115200;
		private const Parity PARITY = Parity.None;
		private const int DATA_BITS = 8;
		private const StopBits STOP_BITS = StopBits.One;

		private const int METADATA_LENGTH = 6;
		private const int MAX_PACKET_SIZE = METADATA_LENGTH + DMX_PRO_MAX_PAYLOAD_SIZE;

		private ISerialPortWriter Writer { get; set; }
		public string Name => Writer?.PortName;

		public EnttecWriter() : this(new SerialPortWriter())
		{
		}

		internal EnttecWriter(ISerialPortWriter writer)
		{
			Writer = writer;
		}

		public Task OpenAsync(string portName)
		{
			return Writer.OpenAsync(portName, BAUD_RATE, PARITY, DATA_BITS, STOP_BITS);
		}

		public Task CloseAsync()
		{
			return Writer.CloseAsync();
		}

		private Span<byte> GetPacket(Span<byte> payload)
		{
			var packetLength = payload.Length + METADATA_LENGTH;
			Span<byte> packet = new Span<byte>(new byte[packetLength]);
			packet[0] = DMX_PRO_MESSAGE_START;
			packet[1] = DMX_PRO_SEND_PACKET;
			BinaryPrimitives.WriteUInt16BigEndian(packet[2..4], (UInt16)payload.Length);
			packet[4] = DMX_COMMAND_BYTE;
			payload.CopyTo(packet[5..^1]);
			packet[^1] = DMX_PRO_MESSAGE_END;
			return packet;
		}

		public Task WriteAsync(byte[] payload)
		{
			if (payload.Length < DMX_PRO_MIN_PAYLOAD_SIZE || payload.Length > DMX_PRO_MAX_PAYLOAD_SIZE)
			{
				throw new ArgumentOutOfRangeException($"DMX Data must be at least {DMX_PRO_MIN_PAYLOAD_SIZE} and no more than {DMX_PRO_MAX_PAYLOAD_SIZE} bytes");
			}

			if (!Writer.IsOpen)
			{
				throw new InvalidOperationException("The port has not been opened");
			}

			var packet = GetPacket(payload);
			return Writer.WriteAsync(packet.ToArray());
		}

		public async ValueTask DisposeAsync()
		{
			if (Writer != null)
			{
				await Writer.DisposeAsync();
			}
		}
	}
}
