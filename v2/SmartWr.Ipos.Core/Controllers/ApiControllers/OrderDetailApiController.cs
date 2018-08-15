using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Nop.Core.Caching;
using SmartWr.Core.ViewModels;
using SmartWr.Ipos.Core.Context.Services;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.Models;
using SmartWr.Ipos.Core.Settings;
using SmartWr.Ipos.Core.ViewModels;
using SmartWr.WebFramework.Library.Infrastructure.Logging;

namespace SmartWr.Ipos.Core.Controllers.ApiControllers
{
    [RoutePrefix("api/OrderDetailapi")]
    public class OrderDetailApiController : BaseApiController
    {
        private const string PostKey = "ipos_pending_post";
        private readonly IPosReportService _orderSvc;
        private readonly OrderDetailService _orderDetailSvc;
        private readonly ProductService _productSvc;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;

        public OrderDetailApiController(ICacheManager cacheManager, ProductService productSvc,
            IPosReportService orderSvc, OrderDetailService orderDetailSvc, ILogger logger)
        {
            _cacheManager = cacheManager;
            _orderSvc = orderSvc;
            _orderDetailSvc = orderDetailSvc;
            _productSvc = productSvc;
            _logger = logger;
        }

        [Route("GetPendingPost"), HttpGet]
        public HttpResponseMessage PendingPost()
        {
            var response = new ApiResultViewModel<List<PostedProduct>>();

            var pendingPost = _cacheManager.Get<List<PostedProduct>>(PostKey);

            response.result = pendingPost;
            return Request.CreateResponse(response);
        }

        [Route("TenderCurrentPost"), HttpPost]
        public HttpResponseMessage TenderOrders(PostRequestViewModel request)
        {
            DateTime entrytDate;
            var pendingPost = _cacheManager.Get<List<PostedProduct>>(PostKey);

            var response = new ApiResultViewModel<string>();

            if (!pendingPost.Any())
            {
                response.errorMessage = "Transaction could not be completed!. Please refresh your page and try again.";
                response.errorStatus = true;
                return Request.CreateResponse(response);
            }

            if (
                !DateTime.TryParseExact(request.entryDate, "dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo,
                    DateTimeStyles.None, out entrytDate))
            {
                response.errorMessage = "Transaction date is invalid.";
                response.errorStatus = true;
                return Request.CreateResponse(response);
            }

            AddCurrentTime(ref entrytDate);

            if (entrytDate > DateTime.Now)
            {
                response.errorMessage = "Transaction date cannot be in the future.";
                response.errorStatus = true;
                return Request.CreateResponse(response);
            }

            try
            {

                using (var uow = _orderSvc.UnitOfWork)
                {
                    uow.BeginTransaction();
                    var currentUserId = User.Identity.GetUserId<int>();
                    var order = new Order
                    {
                        EntryDate = entrytDate,
                        Remark = request.remarks,
                        OrderUId = Guid.NewGuid(),
                        Total = 0,
                        OrderStatus = (int)OrderStatus.POST,
                        CreatedBy_Id = currentUserId
                    };
                    if (IposConfig.UseMembership)
                        order.User_Id = IposMembershipService.GetUserId(User.Identity.Name);

                    _orderSvc.Add(order);

                    foreach (var pt in pendingPost)
                    {
                        AddMilliSecconds(ref entrytDate);
                        if (pt.Quantity <= 0)
                            continue;

                        var product = _productSvc.GetProductById(pt.Id);

                        if (product == null)
                            continue;

                        //entrytDate = entrytDate.AddMilliseconds(DateTime.Now.Millisecond);

                        var ordDt = new OrderDetail
                        {
                            EntryDate = entrytDate,
                            OrderDetailUId = Guid.NewGuid(),
                            Remarks = pt.Remarks ?? product.Description,
                            Quantiy = pt.Quantity,
                            Price = (product.Price ?? 0) * pt.Quantity,
                            Order_UId = order.OrderUId,
                            Product_Id = product.ProductId,
                            CostPrice = product.CostPrice ?? (product.Price ?? 0),
                            CreatedBy_Id = currentUserId,
                            Discount = 0
                        };

                        order.Total += ordDt.Price;
                        product.Quantity -= pt.Quantity;
                        _orderDetailSvc.Add(ordDt);
                        _productSvc.Update(product);
                    }
                    _orderSvc.Update(order);

                    uow.Commit();

                    _cacheManager.Remove(PostKey);
                    response.message = "Pending transactions was commited successfully.";
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

        private void AddCurrentTime(ref DateTime date)
        {
            var current = DateTime.Now;
            date = date.AddHours(current.Hour)
                    .AddMinutes(current.Minute)
                    .AddSeconds(current.Second)
                    .AddMilliseconds(current.Millisecond);
        }

        private void AddMilliSecconds(ref DateTime date)
        {
            var current = DateTime.Now;
            var timespan = DateTime.Now.Subtract(date);
            date = date.AddMilliseconds(timespan.Milliseconds);
        }

        [HttpPost, Route("PostTransaction")]
        public HttpResponseMessage PostTransaction(PostedTransactionViewModel request)
        {
            var response = new ApiResultViewModel<string>();

            if (request == null)
            {
                response.errorMessage = "Transaction not found.";
                response.errorStatus = true;
                return Request.CreateResponse(response);
            }

            if (!request.Products.Any())
            {
                response.errorMessage = "Posted Item is Invalid. Please try again.";
                response.errorStatus = true;
                return Request.CreateResponse(response);
            }

            var pendingPost = _cacheManager.IsSet(PostKey)
                ? _cacheManager.Get<List<PostedProduct>>(PostKey)
                : new List<PostedProduct>();

            foreach (var item in request.Products)
            {
                var prod = pendingPost.Find(p => p.Id == item.Id);

                if (prod != null)
                {
                    prod.Quantity += request.Quantity;
                    prod.Remarks = request.Remarks ?? prod.Remarks;
                    prod.UnitPrice = prod.UnitPrice;
                }

                else
                    pendingPost.Add(new PostedProduct
                    {
                        CreatedDate = DateTime.Now.ToString("dd/MM/yyyy"),
                        Id = item.Id,
                        Name = item.Name,
                        Quantity = item.Quantity <= 0 ? request.Quantity : item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Remarks = item.Remarks ?? request.Remarks
                    });

                _cacheManager.Set(PostKey, pendingPost, AppKeys.DefaultCacheTime);
            }

            response.message = "Transaction successfully posted.";
            return Request.CreateResponse(response);
        }

        [HttpPost, Route("DeletePostItems")]
        public HttpResponseMessage RemoveItem(int[] productIds)
        {
            var response = new ApiResultViewModel<int[]>();

            var pendingPost = _cacheManager.Get<List<PostedProduct>>(PostKey);

            if (pendingPost.Any() && productIds.Any())
            {
                foreach (var id in productIds)
                    pendingPost.RemoveAll(p => p.Id == id);

                _cacheManager.Set(PostKey, pendingPost, AppKeys.DefaultCacheTime);
                response.result = productIds;
            }

            response.message = "Items has been deleted.";
            return Request.CreateResponse(response);
        }

        [Route("RecallOrderItem"), HttpPost]
        public HttpResponseMessage RecallOrder(RecallRequestViewModel request)
        {
            var response = new ApiResultViewModel<OrderDetailViewModel>();

            try
            {
                if (request == null || Guid.Empty.Equals(request.itemId))
                {
                    response.errorStatus = true;
                    response.errorMessage = "Invalid request. Please confirm and try again.";
                    return Request.CreateResponse(response);
                }

                if (request.quantity <= 0 & request.price <= 0)
                {
                    response.errorStatus = true;
                    response.errorMessage = "Please enter a valid quantity and price to complete recall action.";
                    return Request.CreateResponse(response);
                }

                var orderDetail = _orderDetailSvc.GetOrderDetailByUId(request.itemId);

                if (orderDetail == null)
                {
                    response.errorStatus = true;
                    response.errorMessage = "Transaction record not found.";
                    return Request.CreateResponse(response);
                }

                if (request.quantity > orderDetail.Quantiy)
                {
                    response.errorStatus = true;
                    response.errorMessage = String.Format("Stock quantity supplied cannot exceed {0}.", orderDetail.Quantiy);
                    return Request.CreateResponse(response);
                }

                if (request.price > orderDetail.Price)
                {
                    response.errorStatus = true;
                    response.errorMessage = String.Format("Stock price supplied cannot exceed N {0}.", orderDetail.Price);
                    return Request.CreateResponse(response);
                }

                //var order = _orderSvc.GetOrderByUId(orderDetail.Order_UId);

                //if ((orderDetail == null || orderDetail.IsDeleted) || (order == null || order.IsDeleted))
                //{
                //    response.errorStatus = true;
                //    response.errorMessage = "Order not found.";
                //    return Request.CreateResponse(response);
                //}

                //if (orderDetail.Quantiy < request.quantity)
                //{
                //    response.errorStatus = true;
                //    response.errorMessage = "Quantity exceeds Order item quantity.";
                //    return Request.CreateResponse(response);
                //}

                //RecallOrderItem(request, orderDetail, order);

                var stockRecalled = _productSvc.GetProductById(orderDetail.Product_Id.Value);

                OrderDetail newOrderDt = new OrderDetail();
                newOrderDt.OrderDetailUId = Guid.NewGuid();

                Order newOrder = new Order();
                newOrder.OrderUId = Guid.NewGuid();
                newOrder.EntryDate = DateTime.Now;
                newOrder.OrderStatus = (Int32)OrderStatus.RECALL;
                newOrder.Total = (request.quantity * -request.price);
                newOrder.User_Id = IposMembershipService.GetUserId(User.Identity.Name);
                newOrder.IsDiscounted = request.price < orderDetail.Price ? true : false;
                newOrder.CreatedBy_Id = User.Identity.GetUserId<Int32>();
                newOrder.PaymentMethod = (Int32)PaymentMethod.CASH;

                newOrderDt.EntryDate = newOrder.EntryDate;
                newOrderDt.Order_UId = newOrder.OrderUId;
                newOrderDt.Price = -request.price;
                newOrderDt.Product_Id = orderDetail.Product_Id;
                newOrderDt.CostPrice = orderDetail.CostPrice;
                newOrderDt.CreatedBy_Id = User.Identity.GetUserId<int>();
                newOrderDt.Discount = 0; //(Double?)(orderDetail.Price - request.price);
                newOrderDt.Quantiy = orderDetail.Quantiy;

                stockRecalled.Quantity += request.quantity;

                newOrder.Remark = String.Format("Recall of {0} price was update to {1} and quantity to {2} reason being {0}"
                    , stockRecalled.Name + " " + stockRecalled.Description, request.price, stockRecalled.Quantity, request.comment);

                using (var uow = _orderDetailSvc.UnitOfWork)
                {
                    _productSvc.Update(stockRecalled);
                    _orderSvc.Add(newOrder);
                    _orderDetailSvc.Add(newOrderDt);
                    uow.SaveChanges();
                }

                //response.result = Mapper.Map<OrderDetailViewModel>(orderDetail);
                response.message = "Recall action on stock item was successful.";
            }
            catch (Exception e)
            {
                _logger.Log(e);
#if DEBUG
                response.errorMessage = e.Message;
                response.errorStatus = true;
#else
                response.errorMessage = "Error occured, please contact admin.";
                response.errorStatus = true;
#endif
            }
            return Request.CreateResponse(response);
        }

        private void RecallOrderItem(RecallRequestViewModel request, OrderDetail orderDetail, Order order)
        {
            var orderItemprice = orderDetail.Price ?? 0;

            var singleItemPrice = orderItemprice / orderDetail.Quantiy.Value;

            orderDetail.Quantiy -= request.quantity;

            var newOrderItemPrice = orderDetail.Quantiy * singleItemPrice;

            var priceDiff = orderDetail.Price - newOrderItemPrice;

            orderDetail.Price = newOrderItemPrice;

            var discountedPriceToRemove = orderDetail.Discount > 0
                ? ApplyDiscountAmount(orderDetail.Discount.Value, priceDiff.Value)
                : priceDiff;
            order.Total -= discountedPriceToRemove;
            order.ModifiedOnUtc = DateTime.Now;

            order.Remark = "One or more item was recalled.";
            order.OrderStatus = (int)OrderStatus.RECALL;

            var currentUserId = User.Identity.GetUserId<int>();
            order.ModifiedBy_Id = currentUserId;
            orderDetail.ModifiedBy_Id = currentUserId;
            order.ModifiedOnUtc = DateTime.Now;
            orderDetail.ModifiedOnUtc = DateTime.Now;
            if (orderDetail.Quantiy <= 0) orderDetail.Discount = 0;
        }

        private Decimal ReverseDiscountPercent(double percent, decimal currentPrice)
        {
            double _100percent = 100;

            if (currentPrice <= 0)
                return 0;

            if (percent > _100percent)
                return currentPrice;

            percent = _100percent - percent;
            currentPrice *= 100;
            return currentPrice / (decimal)percent;
        }

        private static Decimal ApplyDiscountPercent(double discountPercent, decimal currentPrice)
        {
            const double _100percent = 100;

            if (currentPrice <= 0)
                return 0;

            if (discountPercent > _100percent)
                return currentPrice;

            var amt = discountPercent / _100percent;

            currentPrice -= (decimal)amt * currentPrice;
            return currentPrice;
        }

        private static Decimal ApplyDiscountAmount(double discountAmount, decimal currentPrice)
        {
            var decimalDiscountPrice = (decimal)discountAmount;
            if (currentPrice <= 0)
                return 0;

            if (decimalDiscountPrice > currentPrice)
                return currentPrice;

            currentPrice -= decimalDiscountPrice;
            return currentPrice;
        }
    }
}