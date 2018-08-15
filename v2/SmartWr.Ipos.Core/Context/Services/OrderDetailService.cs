using System;
using System.Collections.Generic;
using System.Linq;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Models;
using SmartWr.WebFramework.Library.MiddleServices.Interfaces.Data;
using SmartWr.WebFramework.Library.MiddleServices.Services;

namespace SmartWr.Ipos.Core.Context.Services
{
    public class OrderDetailService : Service<OrderDetail>
    {
        public OrderDetailService(IUnitOfWork uow)
            : base(uow)
        {
        }

        public OrderDetail GetOrderDetailByUId(Guid id)
        {
            return FirstOrDefault(p =>  p.OrderDetailUId == id);
        }

        public List<OrderDetailSalesHistoryDto> GetOrderDetailHistory ( int pageIndex,int itemPerPage, int id)
        {
            return UnitOfWork.Repository<OrderDetailSalesHistoryDto>().SqlQuery("EXEC [dbo].[Sp_GetProductHistory] @p0,@p1,@p2", pageIndex, itemPerPage, id).ToList();   
        }
    }
}