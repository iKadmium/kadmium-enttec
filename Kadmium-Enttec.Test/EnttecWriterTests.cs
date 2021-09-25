using System;
using Xunit;
using Moq;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Linq;

namespace Kadmium_Enttec.Test
{
	public class UnitTest1
	{
		[Fact]
		public async Task Given_PayloadIsTooSmall_When_WriteAsyncIsCalled_Then_AnErrorIsThrown()
		{
			var payload = new byte[20];

			var serialPortMock = Mock.Of<ISerialPortWriter>(x =>
				x.IsOpen == true
			);
			EnttecWriter writer = new EnttecWriter(serialPortMock);
			await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => writer.WriteAsync(payload));
		}

		[Fact]
		public async Task Given_PayloadIsTooBig_When_WriteAsyncIsCalled_Then_AnErrorIsThrown()
		{
			var payload = new byte[513];

			var serialPortMock = Mock.Of<ISerialPortWriter>(x =>
				x.IsOpen == true
			);
			EnttecWriter writer = new EnttecWriter(serialPortMock);
			await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => writer.WriteAsync(payload));
		}

		[Fact]
		public async Task Given_PortIsNotOpen_When_WriteAsyncIsCalled_Then_AnErrorIsThrown()
		{
			var payload = new byte[200];

			var serialPortMock = Mock.Of<ISerialPortWriter>(x =>
				x.IsOpen == false
			);
			EnttecWriter writer = new EnttecWriter(serialPortMock);
			await Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteAsync(payload));
		}

		[Fact]
		public async Task Given_PortIsOpen_When_WriteAsyncIsCalled_Then_ThePayloadIsWritten()
		{
			var payload = Enumerable.Range(0, 300).Select(x => (byte)(x & 255)).ToArray();
			byte[] actual = null;

			var serialPortMock = Mock.Of<ISerialPortWriter>(x =>
				x.IsOpen == true
				&& x.WriteAsync(It.IsAny<byte[]>()) == Task.CompletedTask
			);

			Mock.Get(serialPortMock)
				.Setup(x => x.WriteAsync(It.IsAny<byte[]>()))
				.Callback<byte[]>(packet => actual = packet);

			EnttecWriter writer = new EnttecWriter(serialPortMock);
			await writer.WriteAsync(payload);

			var actualPayload = actual[5..305].ToArray();
			Assert.Equal(actualPayload, payload);
		}

		[Theory]
		[InlineData(300, 1, 44)]
		[InlineData(200, 0, 200)]
		public async Task Given_PortIsOpen_When_WriteAsyncIsCalled_Then_TheLengthIsCorrect(ushort size, byte msb, byte lsb)
		{
			var payload = Enumerable.Repeat((byte)64, size).ToArray();
			byte[] actual = null;

			var serialPortMock = Mock.Of<ISerialPortWriter>(x =>
				x.IsOpen == true
				&& x.WriteAsync(It.IsAny<byte[]>()) == Task.CompletedTask
			);

			Mock.Get(serialPortMock)
				.Setup(x => x.WriteAsync(It.IsAny<byte[]>()))
				.Callback<byte[]>(packet => actual = packet);

			EnttecWriter writer = new EnttecWriter(serialPortMock);
			await writer.WriteAsync(payload);

			var actualMsb = actual[2];
			var actualLsb = actual[3];
			Assert.Equal(msb, actualMsb);
			Assert.Equal(lsb, actualLsb);
		}
	}
}
