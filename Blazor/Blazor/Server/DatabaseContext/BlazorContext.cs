using Blazor.Server.Configuration;
using Blazor.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Server.DatabaseContext
{
    public class BlazorContext : DbContext
    {
        private readonly BlazorConfiguration _config;
        public string conn;
        public BlazorContext(BlazorConfiguration config)
        {
            _config = config;
            Database.EnsureCreated();
        }
        public DbSet<Wallet> wallets { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_config.POSTGRES_CONNECTION_STRING);
        }
    }
}
