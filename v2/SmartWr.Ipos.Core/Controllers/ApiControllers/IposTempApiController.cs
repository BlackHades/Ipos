using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using SmartWr.Core.ViewModels;
using SmartWr.Ipos.Core.Context.Services;
using SmartWr.Ipos.Core.Dtos;

namespace SmartWr.Ipos.Core.Controllers.ApiControllers
{
    [RoutePrefix("api/IposTemp")]
    public class IposTempApiController : BaseApiController
    {
        //    [Authorize]
        //[RoutePrefix("api/StockTest")]
        //public class ProductApiController : BaseApiController
        //{
        private readonly ProductService _prodSvc;

        public IposTempApiController(ProductService prodSvc)
        {
            _prodSvc = prodSvc;
        }

        [Route("GetCategories")]
        public HttpResponseMessage CategoryFetch()
        {
            ApiResultViewModel<List<GetCategoryDto>> response = new ApiResultViewModel<List<GetCategoryDto>>
            {
            };

            try
            {
                var dtoResult = _prodSvc.GetCategories();
                response.result = dtoResult;
            }
            catch (Exception ex)
            {
                
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