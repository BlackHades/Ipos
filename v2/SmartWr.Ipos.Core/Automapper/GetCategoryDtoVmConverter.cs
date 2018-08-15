using AutoMapper;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Utilities;
using SmartWr.Ipos.Core.ViewModels;

namespace SmartWr.Ipos.Core.Automapper
{
    public class GetCategoryDtoVmConverter : ITypeConverter<GetCategoryDto, CategoryViewModel>
    {
        public CategoryViewModel Convert(ResolutionContext context)
        {
            var source = context.SourceValue as GetCategoryDto;
            var vm = new CategoryViewModel
            {
                Id = source.Id,
                CategoryUId = source.CategoryUId,
                Description = source.Description.Shorten(30),
                Name = source.Name,
                ProductCount = source.ProductCount
            };
            return vm;
        }
    }
}
