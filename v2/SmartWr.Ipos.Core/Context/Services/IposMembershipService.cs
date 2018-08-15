using System;
using System.Web.Security;
using SmartWr.Ipos.Domain.ViewModels;

namespace SmartWr.Ipos.Core.Context.Services
{
    public static class IposMembershipService
    {
        public static AppUserViewModel CreateUserAccount(AppUserViewModel user)
        {
            MembershipCreateStatus status;
            Membership.CreateUser(user.UserName, user.Password, null, user.Question ?? "IPOS", user.Answer ?? "IPOS", true, out status);

            switch (status)
            {
                case MembershipCreateStatus.DuplicateEmail:
                    user.HasError = true;
                    user.ErrorMessage = "Duplicate Email";
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    user.HasError = true;
                    user.ErrorMessage = "Duplicate UserName";
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    user.HasError = true;
                    user.ErrorMessage = "Duplicate Email";
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    user.HasError = true;
                    user.ErrorMessage = "Invalid Email";
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    user.HasError = true;
                    user.ErrorMessage = "Invalid Password";
                    break;
                case MembershipCreateStatus.InvalidUserName:
                    user.HasError = true;
                    user.ErrorMessage = "Invalid Username";
                    break;
                case MembershipCreateStatus.UserRejected:
                    user.HasError = true;
                    user.ErrorMessage = "User Rejected";
                    break;
                case MembershipCreateStatus.Success:
                    break;
                default:
                    user.HasError = true;
                    user.ErrorMessage = "This user account could not be created.";
                    break;
            }
            return user;
        }

        public static bool DeleteAccount(string userName)
        {
            return Membership.DeleteUser(userName, true);
        }

        public static bool ChangeUserAccountPassword(string userName, string oldPwd, string newpassword, String question = "IPOS", String questionAnswer = "IPOS")
        {
            var useracct = Membership.GetUser(userName);
            return useracct != null && useracct.ChangePasswordQuestionAndAnswer(newpassword, question, questionAnswer); 
        }

        public static Guid GetUserId(string userName)
        {
            var membershipUser = Membership.GetUser(userName);
            if (membershipUser == null) return Guid.Empty;
            var providerUserKey = membershipUser.ProviderUserKey;
            if (providerUserKey != null)
                return (Guid)providerUserKey;
            return Guid.Empty;
        }
    }
}