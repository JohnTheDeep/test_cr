using Blazor.Server.Configuration;
using Blazor.Server.Helper;
using Blazor.Shared.Models;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System.Collections.Concurrent;

namespace Blazor.Server.Services
{
    public class NodeService
    {
        private readonly BlazorConfiguration _config;
        public readonly WalletBuffer _buffer;
        public NodeService(BlazorConfiguration config, WalletBuffer buffer)
        {
            _config = config;
            _buffer = buffer;
        }
        public async Task<Dictionary<int, Wallet>> GetBalances(Dictionary<int, Wallet> Wallets)
        {
            try
            {
                var web3 = new Web3(_config.INFURA_API_KEY);
                var temp = new List<IRpcRequestResponseBatchItem>();
                int walletsCount = Wallets.Count;
                var batchSize = walletsCount / Environment.ProcessorCount;
                Wallets = DistinctHelper.GetDistinctValuesAsync(_buffer.GetValues(), Wallets);
                if (Wallets.Count > 0)
                {
                    Parallel.ForEach(Partitioner.Create(0, walletsCount, batchSize),
                        range =>
                        {
                            var batchBalanceRequest = new RpcRequestResponseBatch();

                            for (int i = range.Item1; i < range.Item2; i += batchSize)
                            {
                                var batchAddresses = Wallets.Skip(i).Take(Math.Min(batchSize, Wallets.Count - i));

                                foreach (var wallet in batchAddresses)
                                {
                                    var bathcItem = new RpcRequestResponseBatchItem<EthGetBalance, HexBigInteger>
                                        (web3.Eth.GetBalance as EthGetBalance, web3.Eth.GetBalance.BuildRequest(wallet.Value.Address, BlockParameter.CreateLatest(), wallet.Value.Id));
                                    batchBalanceRequest.BatchItems.Add(bathcItem);
                                }
                            }
                            var responses = web3.Client.SendBatchRequestAsync(batchBalanceRequest).Result;
                            lock (temp)
                                temp.AddRange(responses.BatchItems);
                        });

                    foreach (var item in temp)
                        Wallets[(int)item.RpcRequestMessage.Id].Balance = Web3.Convert.FromWei(item?.RawResponse as HexBigInteger);

                    foreach (var wallet in Wallets)
                        _buffer.Add(wallet.Value);
                }
                return _buffer
                    .GetValues()
                    .OrderByDescending(el => el.Value.Balance)
                    .ToDictionary(el => el.Key, el => el.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ": " + ex);
            }
            return _buffer.GetValues();
        }
    }
}
