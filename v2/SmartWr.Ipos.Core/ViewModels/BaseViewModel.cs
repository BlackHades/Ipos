using System;

namespace SmartWr.Ipos.Core.ViewModels
{
    public class BaseViewModel
    {
        public Int32 Id { get; set; }
        public Boolean HasError { get; set; }
        public String ErrorMessage { get; set; }
    }
}
