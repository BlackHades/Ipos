using Ipos.Sync.Core.ApiClient.Handler;
using Ipos.Sync.Core.Models;
using Ipos.Sync.Core.Models.Enums;
using Ipos.Sync.StoreDataProviders.Dto;
using IposAnalytics.Logic.DataContracts;
using IposAnalytics.Logic.DataContracts.Enums;
using IposAnalytics.Logic.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.ApiClient
{
    public class SyncApiClient
    {
        private readonly HttpClient client;
        private readonly String _apiBaseAddress;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _jsonSettings;

        private readonly String txnApiEndpoint = "api/txnapi/PostBatchTxn";
        private readonly String spoilApiEndpoint = "api/syncspoilapi/PostBulkSpoil";
        public SyncApiClient(String apiBaseAddress)
        {
            CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler();
            client = HttpClientFactory.Create(customDelegatingHandler);
            _apiBaseAddress = apiBaseAddress;

            _logger = new LoggerConfiguration()
               .WriteTo.ColoredConsole()
               .WriteTo.RollingFile(@"C:\Iposlogs\syncapi\log-{Date}.txt")
               .CreateLogger();

            _jsonSettings = new JsonSerializerSettings();
            _jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public async Task<IEnumerable<TransactionDto>> SendBulkSales(IEnumerable<TransactionDto> bulkSales)
        {
            try
            {
                List<TxnData> txnDataList = new List<TxnData>();

                foreach (var sale in bulkSales)
                {
                    var txnData = CreateTxnPayload(sale);
                    txnDataList.Add(txnData);
                }

                var response = await HttpPostJson<List<TxnData>>(_apiBaseAddress + txnApiEndpoint, txnDataList);

                await SyncResponse(response, bulkSales.ToArray());
            }
            catch (Exception)
            {
                _logger.Information("Error occured while sending transaction sales to the cloud ");
            }

            return bulkSales;
        }

        private async Task<HttpResponseMessage> HttpPostJson<TData>(String url, TData payload)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);
            return response;
        }

        [Obsolete]
        public async Task<TransactionDto> SendSingleSales(TransactionDto txn)
        {
            try
            {
                TxnData txnData = CreateTxnPayload(txn);

                var response = await HttpPostJson<TxnData>(
                    _apiBaseAddress + txnApiEndpoint, txnData);

                await SyncResponse(response, new TransactionDto[] { txn });
                return txn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SyncResponse(HttpResponseMessage response,
            params TransactionDto[] txnList)
        {
            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResult<List<TxnData>>>(responseString, _jsonSettings);

                _logger.Information($"({apiResponse.Result.Count}) items was processed by ipos analytic API.");
                _logger.Information("HTTP Status: {0}, Reason {1}.", response.StatusCode, response.ReasonPhrase);

                if (apiResponse != null)
                {
                    Array.ForEach(txnList, txn =>
                    {
                        if (apiResponse.Result != null || !apiResponse.Result.Any())
                        {
                            if (apiResponse.HasError == false)
                            {
                                var txnDataResponse = apiResponse.Result.FirstOrDefault(f => f.TxnNo == txn.Id);

                                if (txnDataResponse == null)
                                    return;

                                var item = txnDataResponse.TxnLines.FirstOrDefault();
                                txn.IsSyncReady = false;
                                txn.SyncDate = DateTime.UtcNow;
                                txn.SyncStatus = (Int32)SyncStatus.COMPLETED;
                                txn.SyncRefNo = item.LineItemRefNo;
                                txn.SyncRefCreatedOn = txnDataResponse.SyncRefCreatedDate;
                                txn.SyncRefModifiedOn = txnDataResponse.SyncRefModifiedDate;
                                txn.ReasonSyncFailed = null;// apiResponse
                            }
                            else
                            {
                                txn.IsSyncReady = true;
                                txn.SyncDate = DateTime.UtcNow;
                                txn.SyncStatus = txn.SyncFailedCount > 5 ? (Int32)SyncStatus.FAILED : (Int32)SyncStatus.PENDING;
                                txn.SyncFailedCount = txn.SyncFailedCount++;

                                if (apiResponse.ErrorList != null && apiResponse.ErrorList.Any())
                                {
                                    StringBuilder errorListBuilder = new StringBuilder();

                                    apiResponse.ErrorList.ForEach(e =>
                                    {
                                        errorListBuilder.AppendFormat("{0};", e);
                                    });

                                    String errorMsg = errorListBuilder.ToString();
                                    txn.ReasonSyncFailed = String.IsNullOrEmpty(errorMsg) ?
                                    "No reason for sync failure" : errorMsg;
                                }
                            }
                        }
                        else
                        {
                            _logger.Information("Txn API endpoint returned an empty response.");
                        }
                    });
                }
            }
            else
            {
                Array.ForEach(txnList, txn =>
                {
                    txn.SyncDate = DateTime.UtcNow;
                    txn.SyncStatus = txn.SyncFailedCount > 5 ? (Int32)SyncStatus.FAILED : (Int32)SyncStatus.PENDING;
                    txn.SyncFailedCount = 1 + txn.SyncFailedCount;
                    txn.ReasonSyncFailed = String.Format("Failed to call the API. HTTP Status: {0}, Reason {1}", response.StatusCode, response.ReasonPhrase);
                    _logger.Information("Failed to call the API. HTTP Status: {0}, Reason {1}", response.StatusCode, response.ReasonPhrase);
                });
            }
        }

        private static TxnData CreateTxnPayload(TransactionDto txn)
        {
            TxnData txnData = (TxnData)txn;

            txnData.TxnLines.Add(new TxnLineItem
            {
                TxnNo = txn.Id,
                LineItemNo = txn.StockItemNo,
                LineItemInfo = txn.StockDetails,
                LineItemCode = txn.StockItemCode ?? txn.Barcode,
                LineType = (Int32)LineItemTypes.STOCK,
                LineItemUnitSold = txn.StockUnitAmount,
                LineItemUnitCost = txn.StockCostAmount,
                LineItemUnitLeft = txn.StockUnitLeft,
                LineItemUnits = txn.StockUnitPurchased,
                LineItemUnitTax = txn.TaxAmount,
                LineItemUnitDiscount = txn.StockDiscountAmount,
                LineItemCategoryNo = txn.StockCategoryRefNo,
                LineItemCategoryInfo = txn.StockCategoryLine,
                LineItemReorderUnit = txn.StockReorderUnit,
                RefCreatedDate = txn.RefCreatedDate ?? txn.CreatedOnUtc,
                RefModifiedDate = txn.RefModifiedDate
            });

            return txnData;
        }

        public async Task<List<SpoilDto>> SendSpoil(List<SpoilDto> spoilList)
        {
            try
            {
                List<SpoilData> spoilDataList = new List<SpoilData>();

                foreach (var spoil in spoilList)
                {
                    var spoilData = (SpoilData)spoil;

                    spoilData.SpoilLines.Add(new SpoilLineItem
                    {
                        ReportedBy = spoil.ReportedBy,
                        StockName = spoil.StockDetails,
                        StockRefNo = spoil.StockRefNo,
                        Cost = spoil.Cost,
                        StockDetails = spoil.SpoilDetails,
                        StockUnit = spoil.StockUnit,
                        StockUnitLeft = spoil.StockUnitLeft,
                        TxnNo = spoil.Id,
                        RefCreatedDate = spoil.RefCreatedDate ?? spoil.CreatedOnUtc,
                        RefModifiedDate = spoil.RefModifiedDate
                    });


                    spoilDataList.Add(spoilData);
                }

                var response = await client.PostAsJsonAsync(_apiBaseAddress + spoilApiEndpoint, spoilDataList);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    _logger.Information(responseString);
                    _logger.Information("HTTP Status: {0}, Reason {1}. Press ENTER to exit", response.StatusCode, response.ReasonPhrase);

                    var apiResponse = JsonConvert.DeserializeObject<ApiResult<List<SpoilData>>>(responseString, _jsonSettings);

                    if (apiResponse != null)
                    {
                        foreach (var spoil in spoilList)
                        {
                            if (apiResponse.Result != null || !apiResponse.Result.Any())
                            {
                                var txnDataResponse = apiResponse.Result.FirstOrDefault(f => f.TxnNo == spoil.Id);

                                if (txnDataResponse == null)
                                    continue;

                                var item = txnDataResponse.SpoilLines.FirstOrDefault();
                                spoil.IsSyncReady = false;
                                spoil.SyncDate = DateTime.UtcNow;
                                spoil.SyncStatus = (Int32)SyncStatus.COMPLETED;
                                spoil.SyncRefNo = item.LineItemRefNo;
                                spoil.SyncRefCreatedOn = txnDataResponse.SyncRefCreatedDate;
                                spoil.SyncRefModifiedOn = txnDataResponse.SyncRefModifiedDate;
                                spoil.ReasonSyncFailed = null;
                            }
                            else
                            {
                                spoil.IsSyncReady = true;
                                spoil.SyncDate = DateTime.UtcNow;
                                spoil.SyncStatus = spoil.SyncFailedCount > 5 ? (Int32)SyncStatus.FAILED : (Int32)SyncStatus.PENDING;
                                spoil.SyncFailedCount = spoil.SyncFailedCount++;

                                if (apiResponse.ErrorList != null && apiResponse.ErrorList.Any())
                                {
                                    StringBuilder errorListBuilder = new StringBuilder();

                                    apiResponse.ErrorList.ForEach(e =>
                                    {
                                        errorListBuilder.AppendFormat("{0};", e);
                                    });

                                    String errorMsg = errorListBuilder.ToString();
                                    spoil.ReasonSyncFailed = String.IsNullOrEmpty(errorMsg) ? "No reason for sync failure" : errorMsg;
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.Information("Spoil API endpoint returned an empty response.");
                    }
                }
                else
                {
                    spoilList.ForEach(spoil =>
                    {
                        spoil.SyncDate = DateTime.UtcNow;
                        spoil.SyncStatus = spoil.SyncFailedCount > 5 ? (Int32)SyncStatus.FAILED : (Int32)SyncStatus.PENDING;
                        spoil.SyncFailedCount = 1 + spoil.SyncFailedCount;
                        spoil.ReasonSyncFailed = String.Format("Failed to call the API. HTTP Status: {0}, Reason {1}", response.StatusCode, response.ReasonPhrase);
                        _logger.Information("Failed to call the API. HTTP Status: {0}, Reason {1}", response.StatusCode, response.ReasonPhrase);
                    });
                }

                return spoilList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}