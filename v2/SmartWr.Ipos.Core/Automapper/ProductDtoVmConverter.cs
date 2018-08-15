using System;
using AutoMapper;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.ViewModels;

namespace SmartWr.Ipos.Core.Automapper
{
    public class ProductDtoVmConverter : ITypeConverter<ProductDto, ProductViewModel>
    {
        public ProductViewModel Convert(ResolutionContext context)
        {
            var source = context.SourceValue as ProductDto;
            var vm = new ProductViewModel
            {
                Id = source.ProductId,
                ProductUId = source.ProductUId,
                Name = String.IsNullOrEmpty(source.Name) ? "[NA]" : source.Name,
                Description = String.IsNullOrEmpty(source.Description) ? "[NA]" : source.Description,
                Notes = String.IsNullOrEmpty(source.Notes) ? "[NA]" : source.Notes,
                Barcode = String.IsNullOrEmpty(source.Barcode) ? "[NA]" : source.Barcode,
                CostPrice = source.CostPrice,
                SellPrice = source.Price,
                ReorderLevel = source.ReorderLevel,
                IsDiscountable = source.IsDiscountable,
                CanExpire = source.CanExpire ?? false,
                Quantity = source.Quantity,
                //Converting a datetime from the database to only date on the screen
                ExpiryDate = source.ExpiryDate.HasValue ? source.ExpiryDate.Value.ToString("dd/MM/yyyy") : "",
                Category = source.Category_Id
               
            };

            return vm;
        }
    }
}