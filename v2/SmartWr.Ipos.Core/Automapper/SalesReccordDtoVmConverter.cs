using System;
using AutoMapper;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.ViewModels;

namespace SmartWr.Ipos.Core.Automapper
{
    public class SalesReccordDtoVmConverter : ITypeConverter<SalesRecordDto, SalesRecordViewModel>
    {
        public SalesRecordViewModel Convert(ResolutionContext context)
        {
            var dbSalesRecord = context.SourceValue as SalesRecordDto;
            var salesRecordVm = new SalesRecordViewModel
            {
                CreatedDate = dbSalesRecord.CreatedDate,
                CustomerName = dbSalesRecord.StaffName,
                OrderUId = dbSalesRecord.Order_UId,
                TotalItemsBought = dbSalesRecord.TotalItemsBought,
                PaymentMethod = dbSalesRecord.PaymentMethod == null ? "NA" : Enum.GetName(typeof(PaymentMethod), dbSalesRecord.PaymentMethod),
                Status = dbSalesRecord.Status == null ? "NA" : Enum.GetName(typeof(OrderStatus), dbSalesRecord.Status),
                Total = dbSalesRecord.Total.HasValue ? (Decimal?)dbSalesRecord.Total.Value : 0,
                Discount = dbSalesRecord.Discount,
                Profit = dbSalesRecord.Profit
            };


            return salesRecordVm;
        }
    }
}