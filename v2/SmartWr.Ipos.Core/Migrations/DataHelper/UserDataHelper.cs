using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Design.PluralizationServices;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SmartWr.Ipos.Core.Context;
using SmartWr.Ipos.Core.Context.Services;
using SmartWr.Ipos.Core.Logic.Helper;
using SmartWr.Ipos.Core.Models;
using SmartWr.Ipos.Core.Settings;
using SmartWr.Ipos.Domain.ViewModels;
using SmartWr.WebFramework.Library.Infrastructure.Factory;
using SmartWr.WebFramework.Library.Infrastructure.Identity;

namespace SmartWr.Ipos.Core.Migrations.DataHelper
{
    public class UserDataHelper
    {
        public static ApplicationIdentityUser CreateAdminUser(IPosDbContext context)
        {
            var applicationUserManager = IdentityFactory.CreateUserManager(context);

            string username = "admin@ipos.com";
            string password = "micr0s0ft_";

            ApplicationIdentityUser user = applicationUserManager.FindByNameAsync(username).Result;

            if (user != null)
                return user;

            user = new ApplicationIdentityUser
            {
                UserName = username,
                Email = username
            };


            if (IposConfig.UseMembership)
                IposMembershipService.CreateUserAccount(new AppUserViewModel { UserName = username, Password = password });


            applicationUserManager.CreateAsync(user, password).Wait();
            applicationUserManager.SetLockoutEnabled(user.Id, false);
            applicationUserManager.Update(user);

            var isInRole = applicationUserManager.IsInRoleAsync(user.Id, IposRoleHelper.ADMIN);
            if (user != null && !isInRole.Result)
                applicationUserManager.AddToRoleAsync(user.Id, IposRoleHelper.ADMIN).Wait();
            return user;
        }

        public static void CreateRoles(IPosDbContext context)
        {
            String[] roleNames = new String[]
            {
                IposRoleHelper.ADMIN,
                IposRoleHelper.STAFF,
                IposRoleHelper.SUPPORT,
                IposRoleHelper.SUPERADMIN,
            };

            var applicationRoleManager = IdentityFactory.CreateRoleManager(context);

            Array.ForEach(roleNames, (string roleName) =>
            {
                var roleResult = applicationRoleManager.RoleExistsAsync(roleName);
                if (!roleResult.Result)
                    applicationRoleManager.CreateAsync(new ApplicationIdentityRole { Name = roleName }).Wait();
            });
        }

        public static void ConfigureIposMemberShip(IPosDbContext context)
        {
            var appMembershipSettings = new List<aspnet_SchemaVersions>
            {
                   new aspnet_SchemaVersions{ Feature="membership", CompatibleSchemaVersion="1", IsCurrentVersion=true},
                   new aspnet_SchemaVersions{ Feature="health monitoring", CompatibleSchemaVersion="1", IsCurrentVersion=true},
                   new aspnet_SchemaVersions{ Feature="personalization", CompatibleSchemaVersion="1", IsCurrentVersion=true},
                   new aspnet_SchemaVersions{ Feature="common", CompatibleSchemaVersion="1", IsCurrentVersion=true},
                   new aspnet_SchemaVersions{ Feature="profile", CompatibleSchemaVersion="1", IsCurrentVersion=true},
                   new aspnet_SchemaVersions{ Feature="role manager", CompatibleSchemaVersion="1", IsCurrentVersion=true}
           };

            context.Set<aspnet_SchemaVersions>().AddOrUpdate(p => p.Feature, appMembershipSettings.ToArray());
            context.SaveChanges();
        }

        static readonly Random Rand = new Random();

        public static void CreateTestData(IPosDbContext context, ApplicationIdentityUser user)
        {
            //old db
            var connetionString = ConfigurationManager.ConnectionStrings["OldIPosDbContext"];

            var categories = ImportExistingData<Category>(connetionString.ConnectionString).Result;
            categories.ToList().ForEach((e) =>
            {
                e.CreatedBy_Id = user.Id;
            });
            context.Set<Category>().AddOrUpdate(p => p.CategoryUId, categories.ToArray());
            context.SaveChanges();

            var adminUser = IposMembershipService.GetUserId("admin@ipos.com");
            var products = ImportExistingData<Product>(connetionString.ConnectionString).Result;
            products.ToList().ForEach((e) =>
            {
                e.CreatedBy_Id = user.Id;
                e.Category_UId = Rand.Next(1, categories.Count());
                e.Insert_UId = adminUser;
            });
            context.Set<Product>().AddOrUpdate(p => p.ProductUId, products.ToArray());
            context.SaveChanges();
        }

        private async static Task<IQueryable<T>> ImportExistingData<T>(string connectionString, bool includeInheritedProperties = false, int totalItems = 0, bool pluralizeTableName = false) where T : class,new()
        {
            var pluralizer = PluralizationService.CreateService(CultureInfo.CurrentCulture);
            var response = new List<T>();

            if (string.IsNullOrEmpty(connectionString))
                return response
                    .AsQueryable();

            var connection = new SqlConnection(connectionString);

            var total = totalItems > 0 ? string.Format("TOP ({0})", totalItems) : "";

            var tsql = string.Format("SELECT {1} * FROM [dbo].[{0}] order by [EntryDate]", pluralizeTableName ? pluralizer.Pluralize(typeof(T).Name) : typeof(T).Name, total);

            var sqlcmd = connection.CreateCommand();
            sqlcmd.CommandText = tsql;
            sqlcmd.CommandTimeout = 4500;

            await connection.OpenAsync();
            var dataReader = await sqlcmd.ExecuteReaderAsync();

            if (!dataReader.HasRows)
                return response
                           .AsQueryable();

            while (dataReader.Read())
            {
                var row = new Dictionary<string, object>();

                for (var i = 0; i < dataReader.FieldCount; i++)
                {
                    var type = dataReader.GetFieldType(i);
                    row.Add(dataReader.GetName(i), TypeValueGetter(() =>
                    {
                        var val = dataReader.GetValue(i);
                        return val == DBNull.Value ? null : val;
                    }));
                }

                var instance = Activator.CreateInstance<T>();

                var instanceProperties = includeInheritedProperties ? instance.GetType().GetProperties() :
                    instance.GetType().
                    GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

                foreach (var column in row.Keys)
                {
                    foreach (var properties in instanceProperties)
                    {
                        if (column == properties.Name)
                            properties.SetValue(instance, row[column]);
                    }
                }
                response.Add(instance);
            }

            connection.Close();
            return response.AsQueryable();
        }

        private static object TypeValueGetter(Func<object> del)
        {
            return del.Invoke();
        }
    }
}