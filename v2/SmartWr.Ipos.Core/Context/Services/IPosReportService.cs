using System;
using System.Collections.Generic;
using System.Linq;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Models;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Interfaces.Data;
using SmartWr.WebFramework.Library.MiddleServices.Services;

namespace SmartWr.Ipos.Core.Context.Services
{
    public class IPosReportService : Service<Order>
    {
        public IPosReportService(IUnitOfWork iuow)
            : base(iuow)
        {
        }

        public OrderDto GetDashboardAggregateData(String today)
        {
            var dto = new OrderDto();
            try
            {

                dto = this.UnitOfWork.Repository<OrderDto>().SqlQuery("Exec [dbo].[Sp_GetDashboardAggregateData] @p0", today).FirstOrDefault();
            }
            catch (Exception ex)
            {
                dto.ValidationErrors.Add(new ValidationError("", ex.Message));
            }

            return dto;
        }
        public List<RecentItemBoughtDto> GetLastItemsBought()
        {
            return this.UnitOfWork.Repository<RecentItemBoughtDto>().SqlQuery("EXEC [dbo].[Sp_GetRecentBoughtItems]").ToList();
        }

        public List<FaultyProductsDto> GetFaultyItems()
        {
            return this.UnitOfWork.Repository<FaultyProductsDto>().SqlQuery("EXEC [dbo].[Sp_GetFaultyProducts]").ToList();
        }
        public List<RestockDto> GetRestockItems()
        {
            return this.UnitOfWork.Repository<RestockDto>().SqlQuery("EXEC [dbo].[Sp_GetRestockItems]").ToList();
        }
        public List<RecentItemDetailsDto> GetRecentItemsDetails(Guid OrderUid)
        {
            return this.UnitOfWork.Repository<RecentItemDetailsDto>().SqlQuery("EXEC [dbo].[Sp_GetOrderDetails] @p0", OrderUid).ToList();
        }

        public List<RestockDto> Ignoreval(int Pid)
        {
            return this.UnitOfWork.Repository<RestockDto>().SqlQuery("EXEC [dbo].[Sp_IgnoreItem] @p0", Pid).ToList();
        }
            
        public List<SalesRecordDto> GetSalesHistory(int page, int pageCount, string user = null, DateTime? startDate = null
            , DateTime? endDate = null, int? status = null, string transactionId = null, Int32? stockId = null)
        {
            return this.UnitOfWork.Repository<SalesRecordDto>()
                .SqlQuery("EXEC [dbo].[Sp_GetSalesHistory] @p0,@p1,@p2,@p3,@p4,@p5,@p6, @p7", page, pageCount, startDate, endDate
                , user, status, transactionId, stockId).ToList();
        }

        public List<OrderDetailDto> GetOrderedProducts(Guid orderUId)
        {
            return UnitOfWork.Repository<OrderDetailDto>().SqlQuery("EXEC [dbo].[Sp_GetOrderedProducts] @p0", orderUId).ToList();
        }

        public Order GetOrderByUId(Guid id)
        {
            return FirstOrDefault(p => p.OrderUId == id);
        }
    }
}