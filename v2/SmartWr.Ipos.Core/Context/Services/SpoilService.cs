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
    public class WasteService : Service<Spoil>
    {
        public WasteService(IUnitOfWork uow)
            : base(uow)
        {
        } 

        public void NewWaste(Spoil spoil)
        {
            if (spoil == null)
                throw new ArgumentNullException("Product parameter is null");


            if (spoil.Quantity.HasValue && spoil.Quantity.Value <= 0)
                spoil.ValidationErrors.Add(new ValidationError("Quantity", "The quantity of spoilt item must be greater than 0."));


            if (String.IsNullOrEmpty(spoil.Description))
                spoil.ValidationErrors.Add(new ValidationError("Description", "The remark of the spoilt is required"));

            spoil.SpoilId = Guid.NewGuid();
            spoil.EntryDate = DateTime.Now;
            Add(spoil);
        }

        public Spoil GetWastedById(Guid id)
        {
            return this.FirstOrDefault(p => p.SpoilId == id);
        }

        public List<FaultyProductsDto> GetPagedWastedItems(int page, int items, string user)
        {
            return UnitOfWork.Repository<FaultyProductsDto>().SqlQuery("EXEC [dbo].[GetWastedItems] @p0, @p1, @p2", page, items,user).ToList();
        }
    }
}