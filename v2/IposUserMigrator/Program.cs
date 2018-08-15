using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using SmartWr.Ipos.Core.Context;
using SmartWr.WebFramework.Library.Infrastructure.Factory;
using SmartWr.WebFramework.Library.Infrastructure.Identity;
using SmartWr.WebFramework.Library.MiddleServices.DataAccess;

namespace IposUserMigrator
{
    public class Program
    {
        private RoleManager<ApplicationIdentityRole, int> RoleManager { get; set; }
        private UserManager<ApplicationIdentityUser, int> UserManager { get; set; }

        private Program(ApplicationDbContext context)
        {
            UserManager = IdentityFactory.CreateUserManager(context);
            RoleManager = IdentityFactory.CreateRoleManager(context);
        }
        private ApplicationIdentityUser CreateUser(string userName, string password)
        {
            var user = UserManager.FindByNameAsync(userName).Result;

            var identityResult = new IdentityResult();

            if (user == null)
            {
                user = new ApplicationIdentityUser { UserName = userName, Email = userName };
                identityResult = UserManager.CreateAsync(user, password).Result;

                if (identityResult.Succeeded)
                    identityResult = UserManager.SetLockoutEnabledAsync(user.Id, false).Result;
            }

            UserManager.GeneratePasswordResetToken(user.Id);
            return user;
        }

        private IdentityResult AddToRole(string roleName, ApplicationIdentityUser user)
        {
            var identityResult = new IdentityResult();

            var roleResult = RoleManager.RoleExistsAsync(roleName);
            if (!roleResult.Result)
                identityResult = RoleManager.CreateAsync(new ApplicationIdentityRole { Name = roleName }).Result;
            var isInRole = UserManager.IsInRoleAsync(user.Id, roleName);
            if (user != null && !isInRole.Result)
                identityResult = UserManager.AddToRoleAsync(user.Id, roleName).Result;

            return identityResult;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Ipos v2 user account migrator... \n");

            List<UserRoleViewModel> oldUsers;

            using (var oldIposDbCtxt = new DbContext("OldIPosDbContext"))
            {
                oldUsers = oldIposDbCtxt.Database.SqlQuery<UserRoleViewModel>(@"Select userName,RoleName from [dbo].[aspnet_Users] u,
                                                                    [dbo].[aspnet_roles] r,
                                                                    [dbo].[aspnet_UsersInRoles] ur
                                                                     Where u.UserId = ur.UserId
                                                                     and ur.RoleId = r.RoleId
                                                                     and UserName != 'admin'").ToList();
            }

            if (oldUsers.Count > 0)
            {
                using (var db = new IPosDbContext())
                {
                    var p = new Program(db);

                    foreach (var oldUser in oldUsers)
                    {
                        var newUser = p.CreateUser(oldUser.UserName, "Micr0s0ft_");
                        if (newUser != null)
                            p.AddToRole(oldUser.RoleName, newUser);
                        Console.WriteLine("Successfully migrated User: {0} with Role: {1} to Ipos V2.", oldUser.UserName, oldUser.RoleName);
                    }
                }
                Console.WriteLine("User migration was successful.");
            }
            else
            {
                Console.WriteLine("There was no user to migrate.");
            }
                Console.ReadLine();
        }
    }
}