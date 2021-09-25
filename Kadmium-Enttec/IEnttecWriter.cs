using System;
using System.Threading.Tasks;

namespace Kadmium_Enttec
{
	public interface IEnttecWriter : IAsyncDisposable
	{
		Task WriteAsync(byte[] data);
		Task OpenAsync(string portName);
		Task CloseAsync();

	}
}