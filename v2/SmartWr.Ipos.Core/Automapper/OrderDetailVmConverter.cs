using AutoMapper;
using SmartWr.Ipos.Core.Models;
using SmartWr.Ipos.Core.ViewModels;

namespace SmartWr.Ipos.Core.Automapper
{
    public class OrderDetailVmConverter : ITypeConverter<OrderDetail, OrderDetailViewModel>
    {
        public OrderDetailViewModel Convert(ResolutionContext context)
        {
            var orderdetail = context.SourceValue as OrderDetail;
            return new OrderDetailViewModel
            {
                Total = orderdetail.Price ?? 0,
                Discount = orderdetail.Discount ?? 0,
                Quantity = orderdetail.Quantiy ?? 0,               
                Id = orderdetail.OrderDetailUId,
                ProductId = orderdetail.Product_Id ?? 0
                
            };
        }
    }
}