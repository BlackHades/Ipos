using AutoMapper;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.ViewModels;
using System;

namespace SmartWr.Ipos.Core.Automapper
{
    public class OrderDetailDtoVmConverter : ITypeConverter<OrderDetailDto, OrderDetailViewModel>
    {
        public OrderDetailViewModel Convert(ResolutionContext context)
        {
            var orderDetail = context.SourceValue as OrderDetailDto;

            return new OrderDetailViewModel
            {
                CostPrice = orderDetail.CostPrice ?? 0,
                Discount = orderDetail.Discount.HasValue ? Math.Round(orderDetail.Discount.Value, 2) : 0,
                Total = (Decimal)(orderDetail.Total.HasValue ? orderDetail.Total : 0),
                ProductId = orderDetail.ProductId,
                ProductName = orderDetail.ProductName,
                Quantity = orderDetail.Quantity ?? 0,
                OrderUId = orderDetail.OrderUId,
                Id = orderDetail.OrderDetailId,
                SellPrice = (Decimal)(orderDetail.Price.HasValue ? orderDetail.Price : 0),
                ProductDescription = orderDetail.Description,
                UnitCost = orderDetail.UnitCost.HasValue ? (Decimal)orderDetail.UnitCost.Value : 0
            };
        }
    }
}