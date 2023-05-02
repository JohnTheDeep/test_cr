using Blazor.Shared.Models;

namespace Blazor.Server.Services
{
    public class WalletBuffer
    {

        private readonly Dictionary<int, Wallet> _buffer;
        public WalletBuffer()
        {
            _buffer = new Dictionary<int, Wallet>();
        }
        public Dictionary<int, Wallet> GetValues() => _buffer;
        public int Count() => _buffer.Count;
        public void ClearBuffer() => _buffer.Clear();
        public void Add(Wallet wallet)
        {
            _buffer.TryAdd(wallet.Id, wallet);
        }
    }
}
