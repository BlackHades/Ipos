using AutoMapper;
using SmartWr.Ipos.Core.Models;
using SmartWr.Ipos.Core.ViewModels;

namespace SmartWr.Ipos.Core.Automapper
{
    public class ProductToVmConverter : ITypeConverter<Product, ProductViewModel>
    {
        public ProductViewModel Convert(ResolutionContext context)
        {
            var source = context.SourceValue as Product;
            var vm = new ProductViewModel
            {
                Id = source.ProductId,
                ProductUId = source.ProductUId,
                Name = source.Name,
                Description = source.Description,
                Notes = source.Notes,
                Barcode = source.Barcode,
                CostPrice = source.CostPrice,
                SellPrice = source.Price,
                ReorderLevel = source.ReorderLevel,
                IsDiscountable = source.IsDiscountable,
                CanExpire = source.CanExpire,
                Quantity = source.Quantity,
                //Converting a datetime from the database to only on the screen
                ExpiryDate =source.ExpiryDate.HasValue ? source.ExpiryDate.Value.ToString("dd/MM/yyyy"):"",
                Category = source.Category_UId
                
            };

            return vm;
        }
    }
}