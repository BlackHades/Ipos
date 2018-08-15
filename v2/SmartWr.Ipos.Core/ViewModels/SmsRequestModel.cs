namespace SmartWr.Ipos.Core.ViewModels
{
    public class SmsRequestModel
    {
        public string Message { get; set; }

        public string[] Receipients { get; set; }
        public bool Tocustomers { get; set; }

    }
}