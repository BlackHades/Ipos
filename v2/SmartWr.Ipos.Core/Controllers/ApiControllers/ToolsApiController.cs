using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.Identity;
using SmartWr.Core.ViewModels;
using SmartWr.Ipos.Core.Context.Services;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.Messaging;
using SmartWr.Ipos.Core.Settings;
using SmartWr.Ipos.Core.Utilities;
using SmartWr.Ipos.Core.ViewModels;
using SmartWr.WebFramework.Library.Infrastructure.Logging;

namespace SmartWr.Ipos.Core.Controllers.ApiControllers
{
    [RoutePrefix("api/toolsapi")]
    public class ToolsApiController : BaseApiController
    {
        private readonly ILogger _logger;
        private readonly WasteService _wasteSvc;
        private readonly ProductService _prodSvc;
        private readonly AuditTrailService _auditSvc;
        private readonly SmsService _smsSvc;
        private readonly CustomerService _userMgr;
        private readonly IEmailService _mailSvc;

        public ToolsApiController(ILogger logger, WasteService spoilSvc, IEmailService mailSvc,
            ProductService prodSvc, AuditTrailService auditSvc, CustomerService userMgr, SmsService smsSvc)
        {
            _wasteSvc = spoilSvc;
            _logger = logger;
            _prodSvc = prodSvc;
            _auditSvc = auditSvc;
            _userMgr = userMgr;
            _mailSvc = mailSvc;
            _smsSvc = smsSvc;
        }

        [Route("GetWastedItems"), HttpPost]
        public HttpResponseMessage GetWaste(ApiRequestViewModel request)
        {
            var response = new ApiResultViewModel<List<SpoilViewModel>>();

            try
            {
                var wasteDtos = _wasteSvc.GetPagedWastedItems(request.pageIndex, request.itemsOnPage, request.q as string);

                var items = new List<SpoilViewModel>();

                wasteDtos.ForEach(wasteRecord =>
                {
                    items.Add(Mapper.Map<SpoilViewModel>(wasteRecord));
                });

                response.result = items;

                if (wasteDtos.Count > 0)
                    response.additionalResult = wasteDtos.FirstOrDefault().TotalCount;

                return Request.CreateResponse(response);
            }
            catch (Exception e)
            {
                _logger.Log(e);
#if DEBUG
                response.errorMessage = e.Message;
                response.errorStatus = true;
#else
                        response.errorMessage = "An error occurred while working, Please try again or contact support.";
                        response.errorStatus = true;
#endif
                return Request.CreateResponse(response);
            }
        }

        [Route("EditwasteItem"), HttpPost]
        public HttpResponseMessage EditWasteItem(SpoilViewModel spoilVm)
        {
            var response = new ApiResultViewModel<dynamic>();

            if (spoilVm == null || Guid.Empty == spoilVm.SpoilId || spoilVm.Quantity <= 0)
            {
                response.errorStatus = true;
                response.errorMessage = "Invalid request. Please confirm and try again.";
            }

            try
            {
                using (var uow = _wasteSvc.UnitOfWork)
                {
                    var spoil = _wasteSvc.GetWastedById(spoilVm.SpoilId);

                    var product = _prodSvc.GetProductById(spoil.Product_Id.Value);
                    if (product == null)
                    {
                        response.errorStatus = true;
                        response.errorMessage = "Product was not found.";
                        return Request.CreateResponse(response);
                    }
                    var oldQty = spoil.Quantity;

                    if (product.Quantity < 0 || product.Quantity + (oldQty - spoilVm.Quantity) < 0)
                    {
                        response.errorStatus = true;
                        response.errorMessage = "Cannot report waste for a negative product.";
                        return Request.CreateResponse(response);
                    }

                    var membershipId = IposConfig.UseMembership ? (Guid?)IposMembershipService.GetUserId(User.Identity.Name) : null;
                    var identityUserId = User.Identity.GetUserId<int>();
                    spoil.Quantity = spoilVm.Quantity;
                    product.Quantity += (oldQty - spoilVm.Quantity);

                    _prodSvc.Update(product);
                    _wasteSvc.Update(spoil);

                    var eventDescription = String.Format("Waste {0} item was edited.", product.Name);

                    _auditSvc.LogEvent(eventDescription, AuditType.EDIT_WASTE, membershipId, identityUserId);
                    uow.SaveChanges();
                    response.message = "Waste item has been updated";
                    response.result = new { productName = product.Name, spoil.Quantity, spoil.SpoilId, EntryDate = spoil.EntryDate.Value.ToString("dd/MM/yyyy") };
                }
            }
            catch (Exception e)
            {
                _logger.Log(e);
#if DEBUG
                response.errorMessage = e.Message;
                response.errorStatus = true;
#else
                        response.errorMessage = "An error occurred while working, Please try again or contact support.";
                        response.errorStatus = true;
#endif
                return Request.CreateResponse(response);
            }
            return Request.CreateResponse(response);
        }

        [Route("GetAuditLogList"), HttpPost]
        public HttpResponseMessage GetAuditList(AuditSearchRequestViewModel request)
        {
            var response = new ApiResultViewModel<List<AuditViewModel>>();

            try
            {
                var auditItems = _auditSvc.GetPagedAudits(request.pageIndex, request.itemsOnPage, request.user, request.type);

                var items = new List<AuditViewModel>();

                auditItems.ForEach(product =>
                {
                    items.Add(Mapper.Map<AuditViewModel>(product));
                });

                response.result = items;

                if (auditItems.Count > 0)
                    response.additionalResult = auditItems.FirstOrDefault().TotalCount;

                return Request.CreateResponse(response);
            }
            catch (Exception e)
            {
                _logger.Log(e);
#if DEBUG
                response.errorMessage = e.Message;
                response.errorStatus = true;
#else
                        response.errorMessage = "An error occurred while working, Please try again or contact support.";
                        response.errorStatus = true;
#endif
                return Request.CreateResponse(response);
            }
        }

        [HttpPost, Route("SendEmail")]
        public HttpResponseMessage SendEmailMessage(MessageRequestModel messageVm)
        {
            var response = new ApiResultViewModel<List<dynamic>>();

            if (ModelState.IsValid)
            {
                try
                {
                    var sb = new List<string> ();

                    var addresses = messageVm.Receipients ?? new string[] { };
                    sb.AddRange(addresses.Where(email => email.ValidEmail()));

                    if (messageVm.Tocustomers)
                    {
                        var allCustomers = _userMgr.CustomersEmail().ToList();

                        allCustomers.ForEach(c =>
                        {
                            if (!sb.Contains(c))
                                sb.Add(c);
                        });
                    }

                    if (sb.Count <= 0)
                    {
                        response.errorMessage = "No receipient email address was found. Please confirm and try again.";
                        response.errorStatus = true;
                        return Request.CreateResponse(response);
                    }

                    Task.Factory.StartNew(() =>
                    {
                        foreach (var item in sb)
                            _mailSvc.SendMail(item, IposConfig.AutomatedFromEmail, messageVm.Subject, messageVm.Message, IposConfig.AppName);
                    });

                    response.message = "Message has been sent successfully.";
                    return Request.CreateResponse(response);
                }
                catch (Exception e)
                {
                    _logger.Log(e);
#if DEBUG
                    response.errorMessage = e.Message;
                    response.errorStatus = true;
#else
                        response.errorMessage = "An error occurred while working, Please try again or contact support.";
                        response.errorStatus = true;
#endif
                    return Request.CreateResponse(response);
                }
            }

            response.errorMessage = "Incomplete message request. Please confirm try again.";
            response.errorStatus = true;
            return Request.CreateResponse(response);
        }

        [HttpPost, Route("SendSms")]
        public async Task<HttpResponseMessage> SendSmsMessage(SmsRequestModel messageVm)
        {
            var response = new ApiResultViewModel<List<dynamic>>();

            if (ModelState.IsValid)
            {
                var httpcontext = HttpContext.Current.Request;

                if (httpcontext.Files.Count > 0)
                {
                    var file = httpcontext.Files[0].InputStream;
                }

                var request = new HttpRequestMessage(HttpMethod.Post, IposConfig.SmsHost);

                var p = await _smsSvc.Send(request);
            }

            return Request.CreateResponse();
        }
    }
}