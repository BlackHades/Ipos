using System;
using SmartWr.Ipos.Core.Enums;

namespace SmartWr.Ipos.Core.ViewModels
{
    public class ApiRequestViewModel
    {
        public dynamic q { get; set; }
        public int pageIndex { get; set; }
        public int itemsOnPage { get; set; }

        public ApiRequestViewModel()
        {
            pageIndex = 1;
            itemsOnPage = 50;
        }
    }

    public class SalesHistoryApiRequestModel : ApiRequestViewModel
    {
        public string startDate { get; set; }
        public string user { get; set; }
        public string transactionId { get; set; }
        public string endDate { get; set; }
        public int? status { get; set; }
        public Int32? Stock { get; set; }
    }

    public class ExportSalesReportRequestModel : SalesHistoryApiRequestModel
    {
        public ReportType reportType { get; set; }
    }

    public class PostRequestViewModel : ApiRequestViewModel
    {
        public string entryDate { get; set; }
        public string remarks { get; set; }
        public int PaymentMethod { get; set; }
    }

    public class RecallRequestViewModel : ApiRequestViewModel
    {
        public Guid itemId { get; set; }
        public string comment { get; set; }
        public int quantity { get; set; }

        public Decimal? price { get; set; }
    }



    public class AuditSearchRequestViewModel : ApiRequestViewModel
    {
        public string user { get; set; }
        public int? type { get; set; }
    }

    public class ProductSalesHistoryRequestViewModel : ApiRequestViewModel
    {
        public int id { get; set; }
    }


}