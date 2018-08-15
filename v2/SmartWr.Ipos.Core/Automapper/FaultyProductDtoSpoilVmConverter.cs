using AutoMapper;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.ViewModels;

namespace SmartWr.Ipos.Core.Automapper
{
    public class FaultyProductDtoSpoilVmConverter : ITypeConverter<FaultyProductsDto, SpoilViewModel>
    {
        public SpoilViewModel Convert(ResolutionContext context)
        {
            var data = context.SourceValue as FaultyProductsDto;
            return new SpoilViewModel
            {
                Description = data.Description,
                SpoilId = data.SpoilId,
                Quantity = data.Quantity,
                Title = data.Name,
                EntryDate = data.EntDate,
                ProductName = data.ProductName
            };
        }
    }
}