using System;
using System.ComponentModel.DataAnnotations;

namespace SmartWr.Ipos.Core.ViewModels
{
    public class ResetPasswordViewModel : BaseViewModel
    {
        //[Required(ErrorMessage="Please Enter your Password")]
        [DataType(DataType.Password)]
        public String NewPassword { get; set; }

        //[Required(ErrorMessage = "Please Confirm your Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage="New Password and Confirm Password do not match")]
        public String ConfirmPassword { get; set; }

        public String ReturnToken { get; set; }
    }
}
