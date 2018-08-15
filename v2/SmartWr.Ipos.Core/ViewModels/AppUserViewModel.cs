using System.ComponentModel.DataAnnotations;
using SmartWr.Ipos.Core.ViewModels;

namespace SmartWr.Ipos.Domain.ViewModels
{
    public class AppUserViewModel : BaseViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
        public bool Status { get; set; }
        [Required]
        public int [] Role { get; set; }

        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

    }
}