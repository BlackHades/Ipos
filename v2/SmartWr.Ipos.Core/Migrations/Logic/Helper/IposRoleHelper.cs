
using System;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using SmartWr.WebFramework.Library.MiddleServices.Interfaces.Auth;
using SmartWr.WebFramework.Library.MiddleServices.Models.Auth;

namespace SmartWr.Ipos.Core.Logic.Helper
{
    public static class IposRoleHelper
    {
        public const string ADMIN = "ADMIN";
        public const string SUPERADMIN = "SUPERADMIN";
        public const string SUPPORT = "SUPPORT";
        public const string STAFF = "STAFF";

        public static void AddUserToRole(String roleName, Int32 userId)
        {
            var _userMgr = HttpContext.Current.GetOwinContext().Get<IApplicationUserManager>();
            var _roleMgr = HttpContext.Current.GetOwinContext().Get<IApplicationRoleManager>();

            if (!_roleMgr.RoleExistsAsync(roleName).Result)
            {
                var role = new ApplicationRole { Name = roleName };
                _roleMgr.Create(role);
            }

            _userMgr.AddUserToRoles(userId, new String[] { roleName }).Wait();
        }
    }
}