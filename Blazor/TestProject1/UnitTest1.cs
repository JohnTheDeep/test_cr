using Blazor.Server.DatabaseContext;
using Blazor.Shared.Models;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace TestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        public async Task GetBalances(List<Wallet> Wallets, string key)
        {
            Stopwatch watch = new();
            watch.Start();
            try
            {
                var web3 = new Web3(key);
                var batchRequest = new RpcRequestResponseBatch();

                foreach (var item in Wallets)
                {
                    var bathcItem = new RpcRequestResponseBatchItem<EthGetBalance, HexBigInteger>
                        ((EthGetBalance)web3.Eth.GetBalance, web3.Eth.GetBalance.BuildRequest(item.Address, BlockParameter.CreateLatest(), item.Id));
                    batchRequest.BatchItems.Add(bathcItem);
                }

                //await web3.Client.SendBatchRequestAsync(batchRequest);
                //foreach (var item in batchRequest.BatchItems)
                //{
                //    Wallets.FirstOrDefault(el => el.Id == (int)item.RpcRequestMessage.Id).Balance =
                //        Web3.Convert.FromWei((HexBigInteger)item.RawResponse);
                //}
                //foreach (var item in response.BatchItems)
                //{
                //    Wallets[(int)item.RpcRequestMessage.Id].Balance =
                //        Web3.Convert.FromWei((HexBigInteger)item.RawResponse);
                //    //Wallets[(int)item.RpcRequestMessage.Id].Balance =
                //    //    (decimal)Int64.Parse(item.RawResponse.ToString(), System.Globalization.NumberStyles.HexNumber);
                //}
                //Parallel.ForEach(Partitioner.Create(0, Wallets.Count(), chunkSize), range =>
                //{
                //    for (int i = range.Item1; i < range.Item2; i++)
                //    {
                //        GetBalance(Wallets[i], web3).Wait();
                //    }
                //});

                //var temp = new List<IRpcRequestResponseBatchItem>();
                //var tempval = Wallets.ToList();
                //var batchSize = 250; // количество адресов в одном Batch запросе

                //for (int i = 0; i < Environment.ProcessorCount; i++)
                //{
                //    var startIndex = i * (Wallets.Length / Environment.ProcessorCount);
                //    var endIndex = (i == Environment.ProcessorCount - 1) ? Wallets.Length : (i + 1) * (Wallets.Length / Environment.ProcessorCount);
                //    var threadAddresses = tempval.GetRange(startIndex, endIndex - startIndex);
                //    for (int j = 0; j < threadAddresses.Count; j += batchSize)
                //    {
                //        var batchAddresses = threadAddresses.GetRange(j, Math.Min(batchSize, threadAddresses.Count - j));
                //        var batchBalanceRequest = new RpcRequestResponseBatch();

                //        foreach (var wallet in batchAddresses)
                //        {
                //            var bathcItem = new RpcRequestResponseBatchItem<EthGetBalance, HexBigInteger>
                //                ((EthGetBalance)web3.Eth.GetBalance, web3.Eth.GetBalance.BuildRequest(wallet.Address, BlockParameter.CreateLatest(), wallet.Id));
                //            batchBalanceRequest.BatchItems.Add(bathcItem);
                //        }
                //        var responses = await web3.Client.SendBatchRequestAsync(batchBalanceRequest);
                //        temp.AddRange(responses.BatchItems);
                //    }
                //}



                //var temp = new List<IRpcRequestResponseBatchItem>();
                //var batchSize = 1000;
                //var walletsList = Wallets.ToList();
                //int processorsCount = Environment.ProcessorCount;
                //int walletsCount = walletsList.Count;
                //int walletsPerProcessor = walletsCount / processorsCount;

                //for (int i = 0; i < processorsCount; i++)
                //{
                //    int startIndex = i * walletsPerProcessor;
                //    int endIndex = (i == processorsCount - 1) ? walletsCount : (i + 1) * walletsPerProcessor;
                //    var threadAddresses = walletsList.GetRange(startIndex, endIndex - startIndex);

                //    for (int j = 0; j < threadAddresses.Count; j += batchSize)
                //    {
                //        var batchAddresses = threadAddresses.GetRange(j, Math.Min(batchSize, threadAddresses.Count - j));
                //        var batchBalanceRequest = new RpcRequestResponseBatch();

                //        foreach (var wallet in batchAddresses)
                //        {
                //            var bathcItem = new RpcRequestResponseBatchItem<EthGetBalance, HexBigInteger>
                //                (web3.Eth.GetBalance as EthGetBalance, web3.Eth.GetBalance.BuildRequest(wallet.Address, BlockParameter.CreateLatest(), wallet.Id));
                //            batchBalanceRequest.BatchItems.Add(bathcItem);
                //        }
                //        var responses = await web3.Client.SendBatchRequestAsync(batchBalanceRequest);
                //        temp.AddRange(responses.BatchItems);
                //    }
                //}

                var temp = new List<IRpcRequestResponseBatchItem>();

                var walletsList = Wallets.ToList();
                var batchSize = walletsList.Count / Environment.ProcessorCount;
                Parallel.ForEach(Partitioner.Create(0, walletsList.Count, batchSize),
                    range =>
                    {
                        var batchBalanceRequest = new RpcRequestResponseBatch();

                        for (int i = range.Item1; i < range.Item2; i += batchSize)
                        {
                            var batchAddresses = walletsList.GetRange(i, Math.Min(batchSize, walletsList.Count - i));

                            foreach (var wallet in batchAddresses)
                            {
                                var bathcItem = new RpcRequestResponseBatchItem<EthGetBalance, HexBigInteger>
                                    (web3.Eth.GetBalance as EthGetBalance, web3.Eth.GetBalance.BuildRequest(wallet.Address, BlockParameter.CreateLatest(), wallet.Id));
                                batchBalanceRequest.BatchItems.Add(bathcItem);
                            }
                        }
                        Task.Run(async () =>
                        {
                            var responses = await web3.Client.SendBatchRequestAsync(batchBalanceRequest);
                            lock (temp)
                            {
                                temp.AddRange(responses.BatchItems);

                            }
                        });
                        foreach (var item in temp)
                        {
                            walletsList[(int)item.RpcRequestMessage.Id].Balance =
                                Web3.Convert.FromWei(item.RawResponse as HexBigInteger);
                        }
                    });


                watch.Stop();
                Console.WriteLine($"???????????????????????????????????????\n{watch.Elapsed}?????????????/");
                await Console.Out.WriteLineAsync();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message + ": " + ex);
            }

        }
        [Test]
        public void Test1()
        {
            BlazorContext manager = new Blazor.Server.DatabaseContext.BlazorContext("Username=postgres;Password=root;Host=127.0.0.1;Port=5432;Database=postgres;");

            var list = manager.wallets.ToList();
            GetBalances(list, "https://mainnet.infura.io/v3/77c3e5699eda40b196d62c81368e32b0").Wait();

        }
    }
}