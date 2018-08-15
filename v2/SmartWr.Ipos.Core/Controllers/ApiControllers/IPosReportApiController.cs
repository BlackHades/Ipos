using AutoMapper;
using Microsoft.Reporting.WebForms;
using SmartWr.Core.ViewModels;
using SmartWr.Ipos.Core.Context.Services;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.Logic.Helper;
using SmartWr.Ipos.Core.Utilities;
using SmartWr.Ipos.Core.ViewModels;
using SmartWr.WebFramework.Library.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace SmartWr.Ipos.Core.Controllers.ApiControllers
{
    [RoutePrefix("api/IPosReportApi")]
    public class IPosReportApiController : BaseApiController
    {
        private readonly ILogger _logger;
        private readonly IPosReportService _reportSvc;

        public IPosReportApiController(IPosReportService reportSvc, ILogger logger)
        {
            _logger = logger;
            _reportSvc = reportSvc;
        }

        [HttpPost, Route("DownloadSalesReport")]
        public async Task<HttpResponseMessage> ExportSales(ExportSalesReportRequestModel request)
        {
            try
            {
                //Select all matched records once
                request.pageIndex = 0;
                request.itemsOnPage = int.MaxValue;

                var dtoResult = GetPagedSales(request);

                AddOrderStatus(dtoResult);

                var relativeUrl = HostingEnvironment.ApplicationPhysicalPath + @"bin\rdlc\salesreport.rdlc";

                if (!File.Exists(relativeUrl))
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "report file not found.");

                var filebytes = await Extension.GenerateReport(relativeUrl, request.reportType, localReport =>
                {
                    localReport.SetParameters(new ReportParameter("EntryDate", request.startDate));
                    localReport.SetParameters(new ReportParameter("EndDate", request.endDate));
                    localReport.DataSources.Add(new ReportDataSource("SalesRecordDto", dtoResult));
                });

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(new MemoryStream(filebytes))
                };

                var fileName = String.Format("Transaction Report.{0}", Extension.GetReportExtension(request.reportType));

                var mime = MimeMapping.GetMimeMapping(fileName);

                response.Content.Headers.ContentType = new MediaTypeHeaderValue(mime);

                response.Content.Headers.ContentLength = filebytes.Length;

                var cd = new ContentDispositionHeaderValue("attachment") { FileName = fileName };
                response.Content.Headers.ContentDisposition = cd;

                HttpContext.Current.Response.SetCookie(new HttpCookie("fileDownload", "true")
                {
                    Path = "/"
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
#if DEBUG
                return Request.CreateResponse(ex);
#else
                return Request.CreateResponse(new { errorMessage = "Error occured, please contact admin.", errorStatus = true });
#endif
            }
        }

        private void AddOrderStatus(List<SalesRecordDto> records)
        {
            if (records == null || records.Count <= 0)
                return;

            try
            {
                for (int i = 0; i < records.Count; i++)
                {
                    var item = records[i];
                    item.OrderStatus = ((OrderStatus)item.Status).ToString();
                }
            }
            catch (Exception)
            {
            }
        }


        [Route("GetDashboardSales")]
        public HttpResponseMessage GetTransactionSales()
        {
            var response = new ApiResultViewModel<OrderDto>();

            try
            {
                var todayDate = DateTime.Now.ToShortDateString();
                var dtoResult = _reportSvc.GetDashboardAggregateData(todayDate);

                if (!dtoResult.HasErrors)
                {
                    response.result = dtoResult;
                }
                else
                {
                    response.errorMessage = dtoResult.ValidationErrors[0].ErrorMessage;
                    response.errorStatus = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
#if DEBUG
                response.errorMessage = ex.Message;
                response.errorStatus = true;
#else
                response.errorMessage = "Error occured, please contact admin.";
                response.errorStatus = true;
#endif
            }

            return Request.CreateResponse(response);
        }

        [Route("GetDashboardTopSales")]
        public HttpResponseMessage GetRecentSales()
        {
            var response = new ApiResultViewModel<List<RecentItemBoughtDto>>();

            try
            {
                var dtoResult = _reportSvc.GetLastItemsBought();
                response.result = dtoResult;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
#if DEBUG
                response.errorMessage = ex.Message;
                response.errorStatus = true;
#else
                response.errorMessage = "Error occured, please contact admin.";
                response.errorStatus = true;
#endif
            }

            return Request.CreateResponse(response);
        }

        [Route("GetDashboardFaultyProducts")]
        public HttpResponseMessage GetFaultyProducts()
        {
            var response = new ApiResultViewModel<List<FaultyProductsDto>>();

            try
            {
                var dtoResult = _reportSvc.GetFaultyItems();
                response.result = dtoResult;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
#if DEBUG
                response.errorMessage = ex.Message;
                response.errorStatus = true;
#else
                response.errorMessage = "Error occured, please contact admin.";
                response.errorStatus = true;
#endif
            }

            return Request.CreateResponse(response);
        }

        [Route("GetDashboardRestockItems")]
        public HttpResponseMessage GetRestockItems()
        {
            var response = new ApiResultViewModel<List<RestockDto>>();

            try
            {
                var dtoResult = _reportSvc.GetRestockItems().Select(p =>
                    new RestockDto
                    {
                        Description = p.Description.Shorten(30),
                        Id = p.Id,
                        ProductId = p.ProductId,
                        ProductUId = p.ProductUId,
                        ReorderLevel = p.ReorderLevel,
                        Quantity = p.Quantity,
                        EntryDate = p.EntryDate,
                        Name = p.Name
                    });
                response.result = dtoResult.ToList();
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
#if DEBUG
                response.errorMessage = ex.Message;
                response.errorStatus = true;
#else
                response.errorMessage = "Error occured, please contact admin.";
                response.errorStatus = true;
#endif
            }

            return Request.CreateResponse(response);
        }

        [HttpGet]
        [Route("GetDashBoardRecentDetailsItems")]
        public HttpResponseMessage GetRecentItemDetails(string ordUid)
        {
            var response = new ApiResultViewModel<List<RecentItemDetailsDto>>();

            try
            {
                var dtoResult = _reportSvc.GetRecentItemsDetails(Guid.Parse(ordUid));
                response.result = dtoResult;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
#if DEBUG
                response.errorMessage = ex.Message;
                response.errorStatus = true;
#else
                response.errorMessage = "Error occured, please contact admin.";
                response.errorStatus = true;
#endif
            }

            return Request.CreateResponse(response);
        }

        [HttpGet]
        [Route("GetDashBoardIgnoreItem")]
        public HttpResponseMessage DoIgnoreValue(string prdUid)
        {
            var response = new ApiResultViewModel<List<RestockDto>>();

            try
            {
                var dtoResult = _reportSvc.Ignoreval(int.Parse(prdUid));
                response.result = dtoResult;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
#if DEBUG
                response.errorMessage = ex.Message;
                response.errorStatus = true;
#else
                response.errorMessage = "Error occured, please contact admin.";
                response.errorStatus = true;
#endif
            }

            return Request.CreateResponse(response);
        }

        [Authorize(Roles = IposRoleHelper.ADMIN + "," + IposRoleHelper.SUPERADMIN)]
        [HttpPost, Route("SalesFor")]
        public HttpResponseMessage GetSalesTransaction(SalesHistoryApiRequestModel request)
        {
            var response = new ApiResultViewModel<List<SalesRecordViewModel>>
            {
                result = new List<SalesRecordViewModel>()
            };

            try
            {
                var dtoResult = GetPagedSales(request);

                dtoResult.ForEach(item => response.result.Add(Mapper.Map<SalesRecordViewModel>(item)));

                if (dtoResult.Count() > 0)
                    response.additionalResult = new { pageCount = dtoResult.FirstOrDefault().TotalCount, SumTotal = dtoResult.Sum(t => t.Total) };
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
#if DEBUG
                response.errorMessage = ex.Message;
                response.errorStatus = true;
#else
                response.errorMessage = "Error occured, please contact admin.";
                response.errorStatus = true;
#endif
            }
            return Request.CreateResponse(response);
        }

        private List<SalesRecordDto> GetPagedSales(SalesHistoryApiRequestModel request)
        {
            var startDate = ParseDate(request.startDate);
            var endDate = ParseDate(request.endDate);

            request.startDate = startDate.ToString("ddd, dd MMM, yyyy", CultureInfo.InvariantCulture);
            request.endDate = endDate.ToString("ddd, dd MMM, yyyy", CultureInfo.InvariantCulture);

            var dtoResult = _reportSvc.GetSalesHistory(request.pageIndex, request.itemsOnPage, request.user
                , startDate, endDate, request.status, request.transactionId
                , request.Stock);
            return dtoResult;
        }

        private DateTime ParseDate(string date)
        {
            DateTime startDate = DateTime.Now;
            if (string.IsNullOrEmpty(date)) return startDate;
            try
            {
                DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
            }
            catch
            {
                // ignored
            }

            return startDate;
        }

        [Authorize(Roles = IposRoleHelper.ADMIN + "," + IposRoleHelper.SUPERADMIN)]
        [Route("GetTransactionDetails"), HttpGet]
        public HttpResponseMessage TransactionDetails(Guid? transactionId)
        {
            if (transactionId == null)
                return Request
                    .CreateErrorResponse(HttpStatusCode.BadRequest, "Incomplete request, Please confirm and try again.");

            var response = new ApiResultViewModel<List<OrderDetailViewModel>> { result = new List<OrderDetailViewModel>() };

            try
            {
                IEnumerable<OrderDetailDto> orderDetail = _reportSvc.GetOrderedProducts(transactionId.Value);

                var totalItems = orderDetail.Count();
                if (totalItems > 0)
                {
                    double totalDiscount = 0;
                    var orderedItems = new List<OrderDetailViewModel>();

                    foreach (var item in orderDetail)
                    {
                        totalDiscount += item.Discount ?? 0;
                        orderedItems.Add(Mapper.Map<OrderDetailViewModel>(item));
                    }
                    response.result = orderedItems;

                    var salesDetails = Mapper.Map<SalesRecordViewModel>(orderDetail.FirstOrDefault());
                    salesDetails.Discount = Math.Round(totalDiscount, 2);
                    salesDetails.Total = Math.Round(orderedItems.Sum(t => t.Total), 2);
                    salesDetails.SubTotal = Math.Round(orderedItems.Sum(t => t.UnitCost), 2);

                    //Uncoment here for discount %
                    //salesDetails.Discount = totalDiscount / totalItems;
                    response.additionalResult = salesDetails;

                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);

#if DEBUG
                response.errorMessage = ex.Message;
                response.errorStatus = true;
#else
                response.errorMessage = "Error occured, please contact admin.";
                response.errorStatus = true;
#endif
            }
            return Request.CreateResponse(response);
        }
    }
}