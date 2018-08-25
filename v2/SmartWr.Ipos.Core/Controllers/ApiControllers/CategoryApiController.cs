using AutoMapper;
using Microsoft.AspNet.Identity;
using Nop.Core.Caching;
using SmartWr.Core.ViewModels;
using SmartWr.Ipos.Core.Context.Services;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.Models;
using SmartWr.Ipos.Core.Settings;
using SmartWr.Ipos.Core.ViewModels;
using SmartWr.WebFramework.Library.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartWr.Ipos.Core.Controllers.ApiControllers
{
    [Authorize]
    [RoutePrefix("api/CategoryApi")]
    public class CategoryApiController : BaseApiController
    {
        private readonly CategoryService _catSvc;
        private readonly AuditTrailService _auditSvc;
        private readonly ILogger _logger;
        private readonly ICacheManager _cacheManager;
        private readonly string Key = "ipos_exisiting_categories";

        public CategoryApiController(CategoryService catSvc, ILogger logger, AuditTrailService auditSvc, ICacheManager cacheManager)
        {
            _catSvc = catSvc;
            _logger = logger;
            _auditSvc = auditSvc;
            _cacheManager = cacheManager;
        }

        [HttpPost, Route("CreateItemCategory")]
        public HttpResponseMessage CreateNewCategory(CategoryViewModel catVm)
        {
            var response = new ApiResultViewModel<CategoryViewModel>();

            try
            {
                var identityUserId = User.Identity.GetUserId<int>();

                var newCategoryItem = new Category()
                {
                    Name = catVm.Name.Trim(),
                    Description = catVm.Description,
                    CreatedBy_Id = identityUserId,
                    ParentCatId = catVm.ParentCatId
                };

                _catSvc.NewCategory(newCategoryItem);

                if (newCategoryItem.HasErrors)
                {
                    response.errorStatus = newCategoryItem.HasErrors;
                    response.errorMessage = newCategoryItem.ValidationErrors.FirstOrDefault() != null ?
                        newCategoryItem.ValidationErrors.FirstOrDefault().ErrorMessage : string.Empty;
                }
                else
                {

                    var membershipUserId = IposConfig.UseMembership ? (Guid?)IposMembershipService.GetUserId(User.Identity.Name) : null;

                    response.message = "Category was saved successfully";
                    var eventDescription = String.Format("{0} Category was created.", newCategoryItem.Name);
                    _cacheManager.Remove(Key);
                    _auditSvc.LogEvent(eventDescription, AuditType.NEW_CATEGORY, membershipUserId, identityUserId);
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

        [HttpPost, Route("GetSearchedCategory")]
        public HttpResponseMessage SearchItemCategory(ApiRequestViewModel vm)
         {
            ApiResultViewModel<List<CategoryViewModel>> response = new ApiResultViewModel<List<CategoryViewModel>>();
            try
            {
                var pageIndex = ((vm.pageIndex - 1) * vm.itemsOnPage);

                var categoryList = _catSvc.GetSearchCategory(vm.q as string, pageIndex, vm.itemsOnPage);

                var categoryVmList = categoryList.Select(Mapper.Map<GetCategoryDto, CategoryViewModel>).ToList();

                response.result = categoryVmList;

                if (categoryList.Count > 0)
                    response.additionalResult = categoryList.FirstOrDefault().TotalCount;
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

        [HttpGet, Route("GetCatItems")]
        public HttpResponseMessage CategoryItemToView(int? id)
        {
            if (id == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Incomplete Request");

            var response = new ApiResultViewModel<CategoryViewModel>();

            var dbCategory = _catSvc.GetCategoryById(id.Value);

            if (dbCategory == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Category not found or previously deleted. Please contact admin.");

            try
            {
                var catego = new CategoryViewModel()
                {
                    CategoryUId = dbCategory.CategoryUId,
                    Name = dbCategory.Name,
                    Description = dbCategory.Description,
                    Id = dbCategory.Id,
                };

                response.result = catego;
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

        [HttpPost, Route("EditItemCategory")]
        public HttpResponseMessage EditCategory(CategoryViewModel catVm)
        {
            var response = new ApiResultViewModel<CategoryViewModel>();

            try
            {
                var identityUserId = User.Identity.GetUserId<int>();
                var membershipUserId = IposConfig.UseMembership ? (Guid?)IposMembershipService.GetUserId(User.Identity.Name) : null;

                var category = _catSvc.GetCategoryById(catVm.CategoryUId);

                if (category == null || category.IsDeleted)
                {
                    response.errorStatus = true;
                    response.errorMessage = "Sorry please this category could not be found or permanently deleted. Please contact administrator";
                }
                else
                {
                    var previousName = category.Name;
                    category.ModifiedOnUtc = DateTime.Now;
                    category.ModifiedBy_Id = identityUserId;
                    category.Name = catVm.Name ?? category.Name;
                    category.Description = String.IsNullOrEmpty(catVm.Description) ? category.Description : catVm.Description;
                    _catSvc.Update(category);

                    var eventDescription = String.Format("{0} Category was edited to {1}.", previousName, category.Name);
                    _auditSvc.LogEvent(eventDescription, AuditType.EDIT_PRODUCT, membershipUserId, identityUserId);
                    _cacheManager.Remove(Key);

                    response.message = "Category edit was successful.";
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

        [HttpPost, Route("DeleteItemCategory")]
        public HttpResponseMessage DeleteCategory(int[] itemsToDelete)
        {
            var response = new ApiResultViewModel<int[]>();

            if (itemsToDelete == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Incomplete Request");

            try
            {
                using (var uow = _catSvc.UnitOfWork)
                {
                    uow.BeginTransaction();

                    Array.ForEach(itemsToDelete, iterate =>
                    {
                        var categoryitem = _catSvc.GetCategoryById(iterate);

                        if (categoryitem == null) return;

                        categoryitem.IsDeleted = true;
                        _catSvc.Update(categoryitem);
                    });
                    uow.Commit();
                    response.result = itemsToDelete;
                    _cacheManager.Remove(Key);
                    response.message = "Category has been deleted.";
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

        [HttpGet, Route("CheckExistingCategory")]
        public Boolean CheckExistingCategory(string name)
        {
            var existingCategories = _cacheManager.Get<IEnumerable<string>>(Key);

            if (existingCategories == null)
            {
                existingCategories = _catSvc.GetCategories().Select(p => p.Name.ToLower());
                _cacheManager.Set(Key, existingCategories, AppKeys.DefaultCacheTime);
            }

            return !existingCategories.Any(p => p.Equals(name.Trim()));
        }
    }
}