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
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.Reporting.WebForms;
using SmartWr.Core.ViewModels;
using SmartWr.Ipos.Core.Context.Services;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.Models;
using SmartWr.Ipos.Core.Settings;
using SmartWr.Ipos.Core.Utilities;
using SmartWr.Ipos.Core.ViewModels;
using SmartWr.WebFramework.Library.Infrastructure.Logging;

namespace SmartWr.Ipos.Core.Controllers.ApiControllers
{
    [Authorize]
    [RoutePrefix("api/StockApi")]
    public class StockApiController : BaseApiController
    {
        private readonly ProductService _prodSvc;
        private readonly WasteService _spoilSvc;
        private readonly AuditTrailService _auditSvc;
        private readonly ILogger _logger;
        private readonly OrderDetailService _ordDetSvc;

        public StockApiController(ProductService prodSvc, ILogger logger, CategoryService categorySvc,
            WasteService spoilSvc, AuditTrailService auditSvc, OrderDetailService ordDetSvc)
        {
            _logger = logger;
            _prodSvc = prodSvc;
            _spoilSvc = spoilSvc;
            _ordDetSvc = ordDetSvc;
            _auditSvc = auditSvc;
        }

        [Route("GetProductHistory"), HttpPost]
        public HttpResponseMessage GetItemHistory(ProductSalesHistoryRequestViewModel request)
        {

            var response = new ApiResultViewModel<List<OrderDetailViewModel>>();

            try
            {
                var productHistory = _ordDetSvc.GetOrderDetailHistory(request.pageIndex, request.itemsOnPage, request.id);

                var totalProductHistory = new List<OrderDetailViewModel>();
                productHistory.ForEach(history =>
                {
                    totalProductHistory.Add(Mapper.Map<OrderDetailViewModel>(history));

                });

                response.result = totalProductHistory;

                if (totalProductHistory.Count > 0)
                    response.additionalResult = totalProductHistory.FirstOrDefault().TotalCnt;

                return Request.CreateResponse(response);
            }
            catch (Exception e)
            {
                _logger.Log(e);
                response.errorMessage = "An error occurred while working, Please try again or contact support.";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }


        [HttpGet]
        [Route("GetCategoryValues")]
        public HttpResponseMessage CategoryFetch()
        {
            var response = new ApiResultViewModel<List<GetCategoryDto>>();

            try
            {
                var dtoResult = _prodSvc.GetCategories();
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

        [Route("GetProductList")]
        public HttpResponseMessage ProductList(ApiRequestViewModel request)
        {
            var response = new ApiResultViewModel<List<ProductViewModel>>();

            try
            {
                var products = _prodSvc.GetPagedProducts(request.pageIndex, request.itemsOnPage, request.q as string);

                var allProducts = new List<ProductViewModel>();

                products.ForEach(product =>
                {
                    allProducts.Add(Mapper.Map<ProductViewModel>(product));
                });

                response.result = allProducts;

                if (products.Count > 0)
                    response.additionalResult = products.FirstOrDefault().TotalCount;

                return Request.CreateResponse(response);
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
                return Request.CreateResponse(response);
            }
        }

        [Route("CreateProduct")]
        [HttpPost]
        public HttpResponseMessage CreateNewProd(ProductViewModel prodVM, bool isDiscountable = false)
        {
            var response = new ApiResultViewModel<ProductViewModel>();

            try
            {
                if (prodVM.CanExpire && string.IsNullOrEmpty(prodVM.ExpiryDate))
                {
                    response.errorStatus = true;
                    response.errorMessage = "Expiry Date is required.";
                    return Request.CreateResponse(response);
                }

                var identityUserId = User.Identity.GetUserId<int>();

                var membershipUserId = IposConfig.UseMembership ? (Guid?)IposMembershipService.GetUserId(User.Identity.Name) : null;
                var newProd = new Product
                {
                    Name = prodVM.Name.Trim(),
                    Description = prodVM.Description,
                    CostPrice = prodVM.CostPrice ?? 0,
                    Price = prodVM.SellPrice ?? 0,
                    Quantity = prodVM.Quantity ?? 0,
                    Category_UId = prodVM.Category,
                    ReorderLevel = prodVM.ReorderLevel ?? 0,
                    Notes = prodVM.Notes,
                    IsDiscountable = prodVM.IsDiscountable,
                    Barcode = prodVM.Barcode,
                    CreatedBy_Id = identityUserId,
                    Insert_UId = membershipUserId
                };

                SetExpiryDate(prodVM, newProd);

                if (newProd.CanExpire && DateTime.Today > newProd.ExpiryDate)
                {
                    response.errorStatus = true;
                    response.errorMessage = "Expiry Date must be a date beyond today.";
                    return Request.CreateResponse(response);
                }

                if (_prodSvc.ProductBarcodeExists(newProd.Barcode))
                {
                    response.errorStatus = true;
                    response.errorMessage = "Barcode already exists for another product.";
                    return Request.CreateResponse(response);
                }

                _prodSvc.NewProduct(newProd);

                if (newProd.HasErrors)
                {
                    response.errorStatus = newProd.HasErrors;
                    response.errorMessage = newProd.ValidationErrors.FirstOrDefault() != null ?
                        newProd.ValidationErrors.FirstOrDefault().ErrorMessage : String.Empty;
                }
                else
                {
                    response.errorStatus = false;
                    response.errorMessage = "Product was saved successfully";
                }

                var eventDescription = String.Format("{0} item was created ", newProd.Name);
                _auditSvc.LogEvent(eventDescription, AuditType.NEW_PRODUCT, membershipUserId, identityUserId);
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

                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
            return Request.CreateResponse(response);
        }

        [Route("EditProduct")]
        [HttpPost]
        public HttpResponseMessage EditProduct(ProductViewModel prodVM)
        {
            var response = new ApiResultViewModel<ProductViewModel>();

            try
            {
                var product = _prodSvc.GetProductById(prodVM.Id);

                if (product == null || product.IsDeleted)
                {
                    response.errorStatus = true;
                    response.errorMessage = ("Sorry this product can not be found, Please contact your administrator");
                    Request.CreateResponse(response);
                }
                var fmrQty = product.Quantity;
                var fmrName = product.Name;
                var membershipId = IposConfig.UseMembership ? (Guid?)IposMembershipService.GetUserId(User.Identity.Name) : null;
                var identityUserId = User.Identity.GetUserId<int>();

                product.ModifiedOnUtc = DateTime.Now;
                product.ModifiedBy_Id = identityUserId;
                product.Name = prodVM.Name ?? product.Name;
                product.Description = String.IsNullOrEmpty(prodVM.Description) ? product.Description : prodVM.Description;
                product.CostPrice = prodVM.CostPrice;
                product.Price = prodVM.SellPrice;
                product.Quantity = prodVM.Quantity;
                product.Category_UId = prodVM.Category;
                product.ReorderLevel = prodVM.ReorderLevel;
                product.Notes = String.IsNullOrEmpty(prodVM.Notes) ? product.Notes : prodVM.Notes;
                product.Barcode = prodVM.Barcode;
                product.IsDiscountable = prodVM.IsDiscountable;

                SetExpiryDate(prodVM, product);

                _prodSvc.Update(product);

                response.message = "Item was successfully edited.";

                var eventDescription = String.Format("{0} Item with {1} quantity was edited to {2} with {3} quantity.", fmrName, fmrQty, product.Name, product.Quantity);
                _auditSvc.LogEvent(eventDescription, AuditType.EDIT_PRODUCT, membershipId, identityUserId);
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
                return Request.CreateResponse(response);
            }
            return Request.CreateResponse(response);
        }


        [Route("Downloadstocks")]
        [HttpGet]
        public async Task<HttpResponseMessage> ExportProduct()
        {
            try
            {
                var productVms = new List<ProductViewModel>();

                _prodSvc.GetPagedProducts(0, int.MaxValue, null).ForEach(item =>
                {
                    item.CostPrice = item.CostPrice ?? 0;
                    item.ReorderLevel = item.ReorderLevel ?? 0;
                    productVms.Add(Mapper.Map<ProductViewModel>(item));
                });

                var relativeUrl = HostingEnvironment.ApplicationPhysicalPath + @"bin\rdlc\products.rdlc";

                if (!File.Exists(relativeUrl))
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { errorStatus = true, errorMessage = "report file not found." });

                var binaryResult = await Extension.GenerateReport(relativeUrl, ReportType.PDF, v =>
                {
                    v.DataSources.Add(new ReportDataSource("ProductDataSet", productVms));
                });

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(new MemoryStream(binaryResult))
                };

                var fileName = String.Format("Stock List.{0}", Extension.GetReportExtension(ReportType.PDF));

                var mime = MimeMapping.GetMimeMapping(fileName);

                response.Content.Headers.ContentType = new MediaTypeHeaderValue(mime);

                response.Content.Headers.ContentLength = binaryResult.Length;

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
                return Request.CreateResponse(new { errorMessage = ex.Message, errorStatus = true });
#else
                 return Request.CreateResponse(new { errorMessage = "Error occured, please contact admin.",errorStatus = true});
#endif
            }
        }

        [HttpGet, Route("GetProdItems")]
        public HttpResponseMessage GetProducts(Guid? Id)
        {
            if (Id == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Incomplete Request");

            var response = new ApiResultViewModel<ProductViewModel>();

            var dbProduct = _prodSvc.GetProductByUId(Id.Value);

            if (dbProduct == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product not found");
            response.result = Mapper.Map<ProductViewModel>(dbProduct);

            return Request.CreateResponse(response);

        }

        [HttpPost, Route("SpoilProduct")]
        public HttpResponseMessage CreateWasteStock(SpoilViewModel spoilVM)
        {
            var response = new ApiResultViewModel<dynamic>();

            if (spoilVM == null || spoilVM.Id == 0)
            {
                response.errorStatus = true;
                response.errorMessage = "Invalid request.";
                return Request.CreateResponse(response);
            }

            try
            {
                using (var uow = _spoilSvc.UnitOfWork)
                {
                    uow.BeginTransaction();
                    var product = _prodSvc.GetProductById(spoilVM.Id);
                    if (product == null)
                    {
                        response.errorStatus = true;
                        response.errorMessage = "Product was not found.";
                        return Request.CreateResponse(response);
                    }

                    if (product.Quantity < 0 || product.Quantity - spoilVM.Quantity < 0)
                    {
                        response.errorStatus = true;
                        response.errorMessage = "Cannot report waste for a negative product.";
                        return Request.CreateResponse(response);
                    }
                    var newSpoil = new Spoil();
                    var membershipId = IposConfig.UseMembership ? (Guid?)IposMembershipService.GetUserId(User.Identity.Name) : null;

                    var identityUserId = User.Identity.GetUserId<int>();
                    newSpoil.Title = product.Name;
                    newSpoil.Description = spoilVM.Description;
                    newSpoil.Quantity = spoilVM.Quantity;
                    newSpoil.Product_Id = product.ProductId;
                    newSpoil.CreatedBy_Id = identityUserId;
                    newSpoil.User_Id = membershipId;

                    _spoilSvc.NewWaste(newSpoil);

                    if (newSpoil.HasErrors)
                    {
                        response.errorStatus = newSpoil.HasErrors;
                        response.errorMessage = newSpoil.ValidationErrors.FirstOrDefault() != null ?
                            newSpoil.ValidationErrors.FirstOrDefault().ErrorMessage : String.Empty;
                    }
                    else
                    {
                        product.Quantity -= newSpoil.Quantity;
                        _prodSvc.Update(product);
                        var eventDescription = String.Format("{0} quantity of {1} was entered as a waste.", newSpoil.Quantity, product.Name);
                        _auditSvc.LogEvent(eventDescription, AuditType.NEW_WASTE, membershipId, identityUserId);
                        uow.Commit();
                        response.result = new { product.Quantity, Id = product.ProductId };
                        response.message = "Waste has now been reported.";
                    }
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







        [HttpPost, Route("ProductQuantity")]
        public HttpResponseMessage CreateQuantityOfStock(QuantityViewModel quantityVM)
        {
            var response = new ApiResultViewModel<QuantityViewModel>();

            if (quantityVM == null || quantityVM.Id == 0)
            {
                response.errorStatus = true;
                response.errorMessage = "Invalid request.";
                return Request.CreateResponse(response);
            }

            try
            {
                using (var uow = _prodSvc.UnitOfWork)
                {
                    uow.BeginTransaction();
                    var product = _prodSvc.GetProductById(quantityVM.Id);
                    if (product == null)
                    {
                        response.errorStatus = true;
                        response.errorMessage = "Product was not found.";
                        return Request.CreateResponse(response);
                    }

                    if (quantityVM.Quantity == null)
                    {
                        response.errorStatus = true;
                        response.errorMessage = "Please enter a value for the quantity";
                        return Request.CreateResponse(response);
                    }

                    var membershipId = IposConfig.UseMembership ? (Guid?)IposMembershipService.GetUserId(User.Identity.Name) : null;

                    var identityUserId = User.Identity.GetUserId<int>();



                    product.Quantity += quantityVM.Quantity;


                    product.ProductId = product.ProductId;
                    product.CreatedBy_Id = identityUserId;
                    //newQuantity.ModifiedBy_Id = membershipId(int);

                    // newSpoil.User_Id = membershipId;

                    _prodSvc.Update(product);

                    if (product.HasErrors)
                    {
                        response.errorStatus = product.HasErrors;
                        response.errorMessage = product.ValidationErrors.FirstOrDefault() != null ?
                           product.ValidationErrors.FirstOrDefault().ErrorMessage : String.Empty;
                    }
                    else
                    {
                        quantityVM.Quantity = product.Quantity;

                        var eventDescription = String.Format("{0} quantity of {1} was updated.", product.Quantity, product.Name);
                        _auditSvc.LogEvent(eventDescription, AuditType.NEW_PRODUCT, membershipId, identityUserId);
                        uow.Commit();
                        response.message = "Quantity has now been updated.";
                        response.result = quantityVM;
                    }

                    // return Request.CreateResponse(response);
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



        [HttpPost, Route("DeleteProduct")]
        public HttpResponseMessage StockDelete(Guid?[] request)
        {
            var response = new ApiResultViewModel<Guid?[]>();
            if (request == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Incomplete Request.");

            try
            {
                using (var uow = _prodSvc.UnitOfWork)
                {
                    uow.BeginTransaction();
                    Array.ForEach(request, iterate =>
                    {
                        var product = _prodSvc.GetProductByUId(iterate.Value);

                        if (product != null)
                        {
                            product.IsDeleted = true;
                            _prodSvc.Update(product);
                        }
                    });

                    uow.Commit();
                    response.result = request;
                    response.message = "Products has been deleted.";
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


        private void SetExpiryDate(ProductViewModel productVm, Product product)
        {
            if (productVm.CanExpire)
            {
                DateTime dt;
                var dtfi = DateTimeFormatInfo.InvariantInfo;

                if (DateTime.TryParseExact(productVm.ExpiryDate, "dd/MM/yyyy", dtfi, DateTimeStyles.None, out dt))
                {
                    product.CanExpire = true;
                    product.ExpiryDate = dt;
                }
            }
        }
    }
}