using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SmartWr.Ipos.Core.ViewModels
{
    public class MessageRequestModel
    {
        public string[] Receipients { get; set; }

        [AllowHtml]
        [Required]
        public string Message { get; set; }

        public bool Tocustomers { get; set; }

        [Required]
        public string Subject { get; set; }
    }
}