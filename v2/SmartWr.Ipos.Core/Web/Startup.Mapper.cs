using AutoMapper;
using SmartWr.Ipos.Core.Automapper;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Models;
using SmartWr.Ipos.Core.ViewModels;
using SmartWr.Ipos.Domain.ViewModels;
using SmartWr.WebFramework.Library.MiddleServices.Models.Auth;

namespace SmartWr.Ipos.Core.Web
{
    public partial class Startup
    {
        public static void ConfigureMapper(IMapperConfiguration config)
        {
            config.CreateMap<Product, ProductViewModel>()
                  .ConvertUsing<ProductToVmConverter>();

            config.CreateMap<ProductDto, ProductViewModel>()
                  .ConvertUsing<ProductDtoVmConverter>();

            config.CreateMap<SalesRecordDto, SalesRecordViewModel>()
                 .ConvertUsing<SalesReccordDtoVmConverter>();

            config.CreateMap<OrderDetailDto, SalesRecordViewModel>()
                 .ConvertUsing<OrderDetailDtoSalesVmConverter>();

            config.CreateMap<OrderDetailDto, OrderDetailViewModel>()
                 .ConvertUsing<OrderDetailDtoVmConverter>();

            config.CreateMap<GetCategoryDto, CategoryViewModel>()
                .ConvertUsing<GetCategoryDtoVmConverter>();


            config.CreateMap<OrderDetail, OrderDetailViewModel>()
                 .ConvertUsing<OrderDetailVmConverter>();

            config.CreateMap<FaultyProductsDto, SpoilViewModel>()
                 .ConvertUsing<FaultyProductDtoSpoilVmConverter>();

            config.CreateMap<AuditDto, AuditViewModel>()
                .ConvertUsing<AuditDtoVmConverter>();

            config.CreateMap<AccountDto, AppUserViewModel>()
                .ConvertUsing<AccountDtoToAppUserVmConverter>();

            config.CreateMap<AppUser, AppUserViewModel>()
                .ConvertUsing<AppUserToAppUserVmConverter>();

            config.CreateMap<OrderDetailSalesHistoryDto, OrderDetailViewModel>()
                .ConvertUsing<OrderDetailSalesHistoryDtoConverter>();
        }
    }
}