using Ipos.Sync.Core.Contracts;
using Ipos.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Contracts
{
    public interface IRepository<TEntity, T> : IDisposable where TEntity : BaseEntity
    {
        IEnumerable<TEntity> SqlQuery(String sql, params object[] parameters);
        int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters);
        IQueryable<TEntity> Table { get; }
        List<TEntity> GetAll();
        IEnumerable<TEntity> GetAll(int pageIndex, int pageSize);
        IEnumerable<TEntity> GetAll(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector, OrderBy orderBy = OrderBy.Ascending);
        IEnumerable<TEntity> GetAll(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector, Expression<Func<TEntity, bool>> predicate, OrderBy orderBy, params Expression<Func<TEntity, object>>[] includeProperties);
        List<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity GetSingle(T id);
        TEntity GetSingleIncluding(T id, params Expression<Func<TEntity, object>>[] includeProperties);
        List<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(int pageIndex, int pageSize);
        Task<IEnumerable<TEntity>> GetAllAsync(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector, OrderBy orderBy = OrderBy.Ascending);
        Task<IEnumerable<TEntity>> GetAllAsync(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector, Expression<Func<TEntity, bool>> predicate, OrderBy orderBy, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<List<TEntity>> GetAllIncludingAsync(params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetSingleAsync(T id);
        Task<TEntity> GetSingleIncludingAsync(T id, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<List<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
