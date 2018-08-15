using AutoMapper;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.ViewModels;

namespace SmartWr.Ipos.Core.Automapper
{
    public class OrderDetailSalesHistoryDtoConverter : ITypeConverter<OrderDetailSalesHistoryDto, OrderDetailViewModel>
    {
        public OrderDetailViewModel Convert(ResolutionContext context)
        {
            var source = context.SourceValue as OrderDetailSalesHistoryDto;
            return new OrderDetailViewModel
            {
                OrderDetailUId = source.OrderDetailUId,
                EntryDate = source.CreatedDate,
                Quantity = source.Quantity ?? 0,
                SellPrice = source.Price ?? 0,
                Total = source.TotalPrice ?? 0,
                Discount = source.Discount ?? 0,
                TotalCnt = source.Total ?? 0

            };

        }
    }
}
