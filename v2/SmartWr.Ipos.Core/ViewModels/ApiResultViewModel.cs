using System;

namespace SmartWr.Core.ViewModels
{
    public class ApiResultViewModel<T>
    {
        public bool errorStatus { get; set; }
        public string errorMessage { get; set; }
        public string message { get; set; }
        public T result { get; set; }
        public dynamic additionalResult { get; set; }
    }
}