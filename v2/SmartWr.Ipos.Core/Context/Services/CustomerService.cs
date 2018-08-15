using System;
using System.Collections.Generic;
using System.Linq;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Models;
using SmartWr.Ipos.Core.Utilities;
using SmartWr.WebFramework.Library.MiddleServices.Interfaces.Data;
using SmartWr.WebFramework.Library.MiddleServices.Services;

namespace SmartWr.Ipos.Core.Context.Services
{
    public class CustomerService : Service<Customer>
    {
        public CustomerService(IUnitOfWork uow)
            : base(uow)
        {
        }

        public List<CustomerDto> GetCustomers(int page, int itemsPerPage)
        {
            return UnitOfWork.Repository<CustomerDto>().SqlQuery("EXEC [dbo].[GetCustomers] @p0,@p1", page, itemsPerPage).ToList();
        }

        public IEnumerable<String> CustomersEmail()
        {
            return GetCustomers(1, int.MaxValue)
                .Where( e=> e.Email.ValidEmail())
                .Select(p => p.Email);
        }
    }
}