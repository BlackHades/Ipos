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
    public class CategoryService : Service<Category>
    {
        public CategoryService(IUnitOfWork uow)
            : base(uow)
        {
        }

        public List<GetCategoryDto> GetAllCategories()
        {
            return UnitOfWork.Repository<GetCategoryDto>().SqlQuery("EXEC [dbo].[Sp_GetCategories]").ToList();
        }
        public bool categoryExists(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                return false;

            var newcategory = this.UnitOfWork.Repository<Category>().SqlQuery("Select * from Category where Name = @p0", categoryName).FirstOrDefault();
            return newcategory != null;

            //return FirstOrDefault(x => 
            //x.Name.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase)) != null;

        }

        public void NewCategory(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException("Category has a null parameter");
            }

            else if (String.IsNullOrEmpty(category.Description))
            {
                category.ValidationErrors.Add(new ValidationError("Description", "Category description is required"));
            }

            else if (String.IsNullOrEmpty(category.Name))
            {
                category.ValidationErrors.Add(new ValidationError("Name", "Category field is required."));
            }

            var categoryExist = categoryExists(category.Name);

             if (categoryExist)
            {
                category.ValidationErrors.Add(new ValidationError("Name", "Category already exists."));
            }
            else { 

            category.EntryDate = DateTime.Now;

            Add(category);
            }
        }

        public Category GetCategoryById(int id)
        {
            return FirstOrDefault(p => p.CategoryUId == id);
        }

        public List<GetCategoryDto> GetSearchCategory(string keyword, Int32 pageIndex, Int32 pageSize)
        {
            return UnitOfWork.Repository<GetCategoryDto>().SqlQuery("Sp_SearchCategories @p0, @p1, @p2", pageIndex, pageSize, keyword).ToList();
        }

        public IEnumerable<Category> GetCategories()
        {
            return GetAll()
                .Where(b => b.IsDeleted == false);
        }
    }
}