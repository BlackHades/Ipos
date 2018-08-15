using System;
using AutoMapper;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.ViewModels;

namespace SmartWr.Ipos.Core.Automapper
{
    public class OrderDetailDtoSalesVmConverter : ITypeConverter<OrderDetailDto, SalesRecordViewModel>
    {

        public SalesRecordViewModel Convert(ResolutionContext context)
        {
            var source = context.SourceValue as OrderDetailDto;

            return new SalesRecordViewModel
            {
                CustomerName = source.CustomerName,
                Total = source.TotalPrice,
                CreatedDate = source.CreatedDate,
                Status = source.Status == null ? "NA" : Enum.GetName(typeof(OrderStatus), source.Status),
                PaymentMethod = source.PaymentMethod == null ? "NA" : Enum.GetName(typeof(PaymentMethod), source.PaymentMethod),
                OrderUId = source.OrderUId,
                Discount = source.Discount ?? 0,
                Discounted = source.IsDiscounted,
                Remark = source.Remark
            };
        }
    }
}