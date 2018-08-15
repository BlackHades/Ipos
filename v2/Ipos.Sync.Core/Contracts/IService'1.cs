using Ipos.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Contracts
{
    public interface IService<TEntity, T> : IService<T> where TEntity : BaseEntity
    {
        List<TEntity> GetAll();
        IEnumerable<TEntity> GetAll(int pageIndex, int pageSize);
        IEnumerable<TEntity> GetAll(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector, OrderBy orderBy = OrderBy.Ascending);
        IEnumerable<TEntity> GetAll(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector, Expression<Func<TEntity, bool>> predicate, OrderBy orderBy, params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity GetById(T id);
        TEntity GetById(T id, Expression<Func<TEntity, T>> keySelector);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(int pageIndex, int pageSize);
        Task<IEnumerable<TEntity>> GetAllAsync(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector, OrderBy orderBy = OrderBy.Ascending);
        Task<IEnumerable<TEntity>> GetAllAsync(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector, Expression<Func<TEntity, bool>> predicate, OrderBy orderBy, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetByIdAsync(T id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
