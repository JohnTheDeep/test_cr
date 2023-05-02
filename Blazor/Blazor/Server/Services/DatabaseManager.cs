using Blazor.Server.DatabaseContext;
using Blazor.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Server.Services
{
    public class DatabaseManager
    {
        private readonly BlazorContext _context;
        private readonly NodeService _nodeService;
        public DatabaseManager(BlazorContext context, NodeService nodeService)
        {
            _context = context;
            _nodeService = nodeService;
        }
        public async Task<IDictionary<int, Wallet>> GetWallets()
        {
            var values = await _context.wallets.ToDictionaryAsync(el => el.Id, el => el);
            return await _nodeService.GetBalances(values);
        }
    }
}
