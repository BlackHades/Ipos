using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.Identity;
using SmartWr.Core.ViewModels;
using SmartWr.Ipos.Core.Context.Services;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.Logic.Helper;
using SmartWr.Ipos.Core.Settings;
using SmartWr.Ipos.Core.ViewModels;
using SmartWr.Ipos.Domain.ViewModels;
using SmartWr.WebFramework.Library.Infrastructure.Logging;
using SmartWr.WebFramework.Library.MiddleServices.Interfaces.Auth;
using SmartWr.WebFramework.Library.MiddleServices.Models.Auth;

namespace SmartWr.Ipos.Core.Controllers.ApiControllers
{
    [Authorize(Roles = IposRoleHelper.ADMIN + "," + IposRoleHelper.SUPERADMIN)]
    [RoutePrefix("api/AuthApi")]
    public class AuthApiController : BaseApiController
    {
        private readonly IApplicationUserManager _appUserMgr;
        private readonly IApplicationRoleManager _appRoleMgr;
        private readonly ILogger _logger;
        private readonly AuditTrailService _auditSvc;

        public AuthApiController(IApplicationUserManager appUserMgr, IApplicationRoleManager appRoleMgr,
           AuditTrailService auditSvc, ILogger logger)
        {
            _appUserMgr = appUserMgr;
            _appRoleMgr = appRoleMgr;
            _logger = logger;
            _auditSvc = auditSvc;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<HttpResponseMessage> Login(LoginViewModel vm)
        {
            var response = new ApiResultViewModel<bool>()
            {
                errorStatus = false
            };

            try
            {
                if (string.IsNullOrEmpty(vm.Username) || string.IsNullOrEmpty(vm.Password))
                {
                    response.errorMessage = "Username or Password cannot be empty.";
                    response.errorStatus = true;
                }

                if (!response.errorStatus)
                {
                    var userAcct = await _appUserMgr.FindAsync(vm.Username, vm.Password);

                    if (userAcct != null)
                    {
                        if (userAcct.LockoutEnabled)
                        {
                            response.errorMessage = "Sorry, account locked. Please contact your Admin.";
                            response.errorStatus = true;
                        }

                        _appUserMgr.SignIn(userAcct, false, false);
                        response.result = true;
                    }

                    else
                    {
                        response.errorMessage = "Username or Password is invalid, try again.";
                        response.errorStatus = true;
                    }
                }
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

        public bool RemoveUserFromRole(AppUser user)
        {
            ApplicationIdentityResult result;
            var roles = user.Roles;
            try
            {
                foreach (var item in roles)
                {
                    var role = _appRoleMgr.FindByIdAsync(item.RoleId).Result;

                    if (role.Name.Equals(IposRoleHelper.SUPERADMIN))
                        return false;

                    result = _appUserMgr.RemoveFromRoleAsync(user.Id, role.Name).Result;

                    if (!result.Succeeded)
                        return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Route("CreateUserAccount"), HttpPost]
        public HttpResponseMessage CreateUserAccount(AppUserViewModel accountModel)
        {
            var response = new ApiResultViewModel<AppUserViewModel>();

            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Email = accountModel.Email,
                    UserName = accountModel.UserName,
                    PhoneNumber = accountModel.PhoneNumber,
                    FirstName = accountModel.FirstName,
                    LastName = accountModel.LastName,
                    LockoutEnabled = !accountModel.Status
                };

                try
                {
                    if (IposConfig.UseMembership)
                    {
                        var membershipUser = IposMembershipService.CreateUserAccount(accountModel);

                        if (membershipUser.HasError)
                        {
                            response.errorMessage = membershipUser.ErrorMessage;
                            response.errorStatus = true;
                            return Request.CreateResponse(response);
                        }
                    }

                    var userRegisterResponse = _appUserMgr.Create(user, accountModel.Password);

                    if (userRegisterResponse.Succeeded)
                    {
                        AddToMutipleRoles(accountModel, user.Id);

                        var eventDescription = String.Format("{0} account was created.", accountModel.UserName);

                        var membershipUserId = IposConfig.UseMembership ? (Guid?)IposMembershipService.GetUserId(User.Identity.Name) : null;

                        _auditSvc.LogEvent(eventDescription, AuditType.NEW_ACCOUNT, membershipUserId, User.Identity.GetUserId<int>());

                        response.message = "New User account has been created.";
                        return Request.CreateResponse(response);
                    }
                    else
                        response.errorMessage = userRegisterResponse.Errors.FirstOrDefault();
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
            }
            else
            {
                response.errorStatus = true;
                response.errorMessage = "Cannot create user account with Incomplete fields.";
            }

            return Request.CreateResponse(response);
        }

        private void AddToMutipleRoles(AppUserViewModel accountModel, int userId)
        {
            if (accountModel.Role != null)
            {
                Array.ForEach(accountModel.Role, roleId =>
                {
                    var role = _appRoleMgr.GetRoles().FirstOrDefault(p => p.Id == roleId);

                    if (role != null)
                        IposRoleHelper.AddUserToRole(role.Name, userId);
                });
            }
        }

        private void RemovePreviousRoles(AppUser userAcct)
        {
            Array.ForEach(userAcct.Roles.ToArray(), item =>
              {
                  var appRole = _appRoleMgr.FindByIdAsync(item.RoleId).Result;
                  _appUserMgr.RemoveFromRoleAsync(userAcct.Id, appRole.Name).Wait();
              });
        }

        [Route("GetAllUsers"), HttpPost]
        public HttpResponseMessage GetUsers(ApiRequestViewModel request)
        {
            var response = new ApiResultViewModel<List<AppUserViewModel>>();

            var users = _auditSvc.UnitOfWork.Repository<AccountDto>()
                .SqlQuery("exec [dbo].[Getusers] @p0, @p1, @p2", request.pageIndex, request.itemsOnPage, request.q as string)
                .ToList();

            var userlistResult = new List<AppUserViewModel>();

            users.ForEach(user =>
            {
                var userVm = Mapper.Map<AppUserViewModel>(user);
                userlistResult.Add(userVm);
            });

            response.result = userlistResult;

            response.additionalResult = users.Count > 0 ? users.First().TotalCount : 0;

            return Request.CreateResponse(response);
        }

        [HttpPost, Route("EditUserAccount")]
        public HttpResponseMessage EditUserAccount(AppUserViewModel accountModel)
        {
            var response = new ApiResultViewModel<AppUserViewModel>();

            if (accountModel.Id != 0)
            {
                var account = _appUserMgr.FindById(accountModel.Id);

                if (account == null)
                {
                    response.errorMessage = "Account does not exist.";
                    return Request.CreateResponse(response);
                }

                try
                {
                    account.FirstName = accountModel.FirstName;
                    account.LastName = accountModel.LastName;
                    account.Email = accountModel.Email;
                    account.PhoneNumber = accountModel.PhoneNumber;
                    account.LockoutEnabled = !accountModel.Status;

                    var result = _appUserMgr.Update(account);

                    if (result.Succeeded)
                    {
                        RemovePreviousRoles(account);
                        AddToMutipleRoles(accountModel, account.Id);
                        _appUserMgr.Update(account);

                        var eventDescription = String.Format("{0} account was edited.", account.UserName);

                        var membershipUserId = IposConfig.UseMembership ? (Guid?)IposMembershipService.GetUserId(User.Identity.Name) : null;

                        _auditSvc.LogEvent(eventDescription, AuditType.NEW_ACCOUNT, membershipUserId, User.Identity.GetUserId<int>());

                        response.message = "Account details has been updated.";
                    }
                    else response.errorMessage = result.Errors.FirstOrDefault();
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
            }
            return Request.CreateResponse(response);
        }

        [HttpPost, Route("ChangeUserPassword")]
        public async Task<HttpResponseMessage> ChangingUserPassword(AppUserViewModel accountModel)
        {
            var response = new ApiResultViewModel<bool>();

           var changePwdResult = IposMembershipService.ChangeUserAccountPassword(accountModel.UserName,  String.Empty, accountModel.NewPassword);

            if (!changePwdResult)
            {
                response.errorStatus = true;
                response.errorMessage = string.Format("Password change was unsuccessful ");

                return Request.CreateResponse(response);
            }

            if (ModelState.IsValid && accountModel.Id != 0)
            {
                var token = await _appUserMgr.GeneratePasswordResetTokenAsync(accountModel.Id);

                var result = await _appUserMgr.ResetPasswordAsync(accountModel.Id, token, accountModel.NewPassword);

                if (result.Succeeded)
                {
                    response.message = "Password change was successful.";
                    response.result = true;
                }
                else
                {
                    response.errorStatus = true;
                    response.errorMessage = string.Format("Password change was unsuccessful : {0}.", result.Errors.FirstOrDefault());
                }
            }
            else
            {
                response.errorStatus = true;
                response.errorMessage = "Invalid request. Please confirm and try again.";
            }

            return Request.CreateResponse(response);
        }
        [HttpGet, Route("GetuserAccount")]
        public HttpResponseMessage GetuserAccount(int? accountId)
        {
            var response = new ApiResultViewModel<AppUserViewModel>();

            if (accountId == null)
            {
                response.errorMessage = "Incomplete request.";
                return Request.CreateResponse(response);
            }
            var account = _appUserMgr.FindById(accountId.Value);
            if (account == null)
            {
                response.errorStatus = true;
                response.errorMessage = "Account does not exist.";
                return Request.CreateResponse(response);
            }
            var userAccountVm = Mapper.Map<AppUserViewModel>(account);

            if (account.Roles.Any())
            {
                userAccountVm.Role = account.Roles.Select(item => item.RoleId).ToArray();
            }

            response.result = userAccountVm;
            return Request.CreateResponse(response);
        }

        [HttpPost, Route("DeleteUserAccount")]
        public HttpResponseMessage DeleteUserAccount(int[] acctToDeleteIds)
        {
            var response = new ApiResultViewModel<List<int>>();

            if (acctToDeleteIds == null)
            {
                response.errorStatus = true;
                response.errorMessage = "Incomplete delete request.";
                return Request.CreateResponse(response);
            }

            try
            {
                var deletedacctId = new List<int>();

                foreach (var iterator in acctToDeleteIds)
                {
                    var account = _appUserMgr.FindById(iterator);

                    if (account == null)
                    {
                        response.errorStatus = true;
                        response.errorMessage = "One or more selected user account does not exist.";
                        return Request.CreateResponse(response);
                    }

                    if (IposConfig.UseMembership)
                    {
                        var deleteMember = IposMembershipService.DeleteAccount(account.UserName);

                        if (!deleteMember)
                        {
                            response.errorStatus = true;
                            response.errorMessage = "One or more user account could not be deleted.";
                            continue;
                        }
                    }

                    if (account.Roles.Count > 0)
                        RemoveUserFromRole(account);

                    var result = _appUserMgr.DeleteAsync(account.Id).Result;

                    if (result.Succeeded)
                        deletedacctId.Add(iterator);
                }

                if (deletedacctId.Count == acctToDeleteIds.Length)
                    response.message = "Selected user accounts has been deleted.";

                else
                {
                    response.errorMessage = "Sorry! An error orccured while working.";
                    response.errorStatus = true;
                }

                response.result = deletedacctId;
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

        //[HttpGet]
        //public HttpResponseMessage GenerateRandomPassword()
        //{
        //    var response = new ApiResultViewModel<string> { };
        //    IposMembershipService
        //    return Request
        //}
    }
}