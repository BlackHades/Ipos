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
    public class ProductService : Service<Product>
    {
        public ProductService(IUnitOfWork uow)
            : base(uow)
        {

        }
        public List<GetCategoryDto> GetCategories()
        {
            return this.UnitOfWork.Repository<GetCategoryDto>().SqlQuery("EXEC [dbo].[Sp_GetCategories]").ToList();
        }

        public bool ProductBarcodeExists(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return false;

            var product = this.UnitOfWork.Repository<Product>().SqlQuery("Select * from product where barcode = @p0", barcode).FirstOrDefault();
            return product != null;
        }

        public void NewProduct(Product prod)
        {
            if (prod == null)
            {
                throw new ArgumentNullException("Product isn't valid.");
            }

            if (String.IsNullOrEmpty(prod.Name))
            {
                prod.ValidationErrors.Add(new ValidationError("Name", "Product name is required."));
            }

            if (prod.Quantity.HasValue && prod.Quantity.Value < 0)
            {
                prod.ValidationErrors.Add(new ValidationError("Quantity", "Product quantity must be greater than 0."));
            }

            prod.ProductUId = Guid.NewGuid();
            prod.EntryDate = DateTime.Now;


            if (String.IsNullOrEmpty(prod.Description))
            {
                prod.ValidationErrors.Add(new ValidationError("Description", "Product description is required."));
            }

            if (prod.CostPrice.HasValue && prod.CostPrice <= 0)
            {
                prod.ValidationErrors.Add(new ValidationError("Cost Price", "Product cost price must be greater than 0."));
            }

            if (prod.Price.HasValue && prod.Price <= 0)
            {
                prod.ValidationErrors.Add(new ValidationError("Selling Price", "Product selling price must be greater than 0."));
            }

            if (prod.Category_UId == null)
            {
                prod.ValidationErrors.Add(new ValidationError("Category", "The product category is required"));
            }

            this.Add(prod);
        }

        public Product GetProductByUId(Guid uid)
        {
            return this.FirstOrDefault(p => p.ProductUId == uid);
        }

        public Product GetProductById(int id)
        {
            return this.FirstOrDefault(p => p.ProductId == id);
        }

        public List<ProductDto> GetPagedProducts(int page, int pageIndex, string q)
        {
            return UnitOfWork.Repository<ProductDto>().SqlQuery("EXEC [dbo].[Sp_SearchProducts] @p0,@p1,@p2", page, pageIndex, q).ToList();
        }
    }
}