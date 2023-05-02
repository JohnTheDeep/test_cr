using Blazor.Server.Services;
using Blazor.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blazor.Server.Controllers
{
    [ApiController]
    [Route("api/Wallets")]
    public class WalletController : ControllerBase
    {
        private readonly ILogger<WalletController> _logger;
        private readonly DatabaseManager _databaseManager;
        public WalletController(ILogger<WalletController> logger, DatabaseManager databaseManager)
        {
            _logger = logger;
            _databaseManager = databaseManager;
        }

        [HttpGet]
        public async Task<IDictionary<int, Wallet>> Get()
        {
            return await _databaseManager.GetWallets();
        }
    }
}