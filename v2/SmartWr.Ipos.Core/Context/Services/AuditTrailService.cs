using System;
using System.Collections.Generic;
using System.Linq;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.Models;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Interfaces.Data;
using SmartWr.WebFramework.Library.MiddleServices.Services;

namespace SmartWr.Ipos.Core.Context.Services
{
    public class AuditTrailService : Service<Audit>
    {
        public AuditTrailService(IUnitOfWork uow)
            : base(uow)
        {
        }

        public bool LogEvent(string description, AuditType type, Guid? membershipUserId, int userId)
        {
            var adt = new Audit
            {
                User_Id = membershipUserId,
                AuditId = Guid.NewGuid(),
                CreatedBy_Id = userId,
                Description = description,
                EntryDate = DateTime.Now,
                AuditType = (int)type
            };

            var status = Create(adt);
            return !status.HasErrors;
        }

        private Audit Create(Audit adt)
        {
            if (adt == null)
                throw new ArgumentNullException("Audit parameter is null");

            if (Guid.Empty == adt.User_Id || adt.CreatedBy_Id == 0)
                adt.ValidationErrors.Add(new ValidationError("User_Id", "Invalid user"));

            if (adt.Description.Equals(string.Empty))
                adt.ValidationErrors.Add(new ValidationError("Description", "Description is empty"));

            Add(adt);
            return adt;
        }

        public List<AuditDto> GetPagedAudits(int pageIndex, int itemsOnPage, string user, int? status = null)
        {
            return UnitOfWork.Repository<AuditDto>().SqlQuery("EXEC [dbo].[GetAuditLog] @p0, @p1, @p2, @p3", pageIndex, itemsOnPage, user, status).ToList();
        }
    }
}