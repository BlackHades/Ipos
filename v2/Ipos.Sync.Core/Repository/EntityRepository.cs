using Ipos.Sync.Core.Contracts;
using Ipos.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Repository
{
    public class EntityRepository<TEntity, T> : IRepository<TEntity, T> where TEntity : BaseEntity<T>
    {
        private readonly IContext _context;
        private IDbSet<TEntity> _dbEntitySet;
        private bool _disposed;

        public EntityRepository(IContext context)
        {
            _context = context;
            _dbEntitySet = _context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> SqlQuery(String sql, params object[] parameters)
        {
            return _context.SqlQuery<TEntity>(sql, parameters);
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            return _context.ExecuteSqlCommand(sql, doNotEnsureTransaction, timeout, parameters);
        }

        public virtual IQueryable<TEntity> Table
        {
            get
            {
                return this.Entities;
            }
        }

        protected virtual IDbSet<TEntity> Entities
        {
            get
            {
                if (_dbEntitySet == null)
                {
                    _dbEntitySet = _context.Set<TEntity>();
                }

                return _dbEntitySet;
            }
        }

        public List<TEntity> GetAll()
        {
            return _dbEntitySet.ToList();
        }

        public IEnumerable<TEntity> GetAll(int pageIndex, int pageSize)
        {
            return GetAll(pageIndex, pageSize, x => x.Id);
        }

        public IEnumerable<TEntity> GetAll(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector, OrderBy orderBy = OrderBy.Ascending)
        {
            return GetAll(pageIndex, pageSize, keySelector, null, orderBy);
        }

        public IEnumerable<TEntity> GetAll(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector, Expression<Func<TEntity, bool>> predicate, OrderBy orderBy, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = FilterQuery(keySelector, predicate, orderBy, includeProperties);
            entities = entities.Skip(pageIndex).Take(pageSize);
            return entities.AsEnumerable();
        }

        public List<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            return entities.ToList();
        }

        public TEntity GetSingle(T id)
        {
            return _dbEntitySet.FirstOrDefault(t => t.Id.Equals(id));
        }

        public TEntity GetSingleIncluding(T id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            return entities.FirstOrDefault(x => x.Id.Equals(id));
        }

        public List<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbEntitySet.Where(predicate).ToList();
        }

        public void Insert(TEntity entity)
        {
            _context.SetAsAdded(entity);
        }

        public void Update(TEntity entity)
        {
            _context.SetAsModified(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.SetAsDeleted(entity);
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return _dbEntitySet.ToListAsync();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(int pageIndex, int pageSize)
        {
            return GetAllAsync(pageIndex, pageSize, x => x.Id);
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector, OrderBy orderBy = OrderBy.Ascending)
        {
            return GetAllAsync(pageIndex, pageSize, keySelector, null, orderBy);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(int pageIndex, int pageSize, Expression<Func<TEntity, T>> keySelector,
            Expression<Func<TEntity, bool>> predicate, OrderBy orderBy, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = FilterQuery(keySelector, predicate, orderBy, includeProperties);
            return await Task.FromResult<IEnumerable<TEntity>>(entities.AsEnumerable());
            //var total = await entities.CountAsync();// entities.CountAsync() is different than pageSize
            //entities = entities.Paginate(pageIndex, pageSize);
            //var list = await entities.ToListAsync();
            //return list.ToPaginatedList(pageIndex, pageSize, total);
        }

        public Task<List<TEntity>> GetAllIncludingAsync(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            return entities.ToListAsync();
        }

        public Task<TEntity> GetSingleAsync(T id)
        {
            return _dbEntitySet.FirstOrDefaultAsync(t => t.Id.Equals(id));
        }

        public Task<TEntity> GetSingleIncludingAsync(T id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            return entities.FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        public Task<List<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbEntitySet.Where(predicate).ToListAsync();
        }

        private IQueryable<TEntity> FilterQuery(Expression<Func<TEntity, T>> keySelector, Expression<Func<TEntity, bool>> predicate, OrderBy orderBy,
            Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            entities = (predicate != null) ? entities.Where(predicate) : entities;
            entities = (orderBy == OrderBy.Ascending)
                ? entities.OrderBy(keySelector)
                : entities.OrderByDescending(keySelector);
            return entities;
        }

        private IQueryable<TEntity> IncludeProperties(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> entities = _dbEntitySet;
            foreach (var includeProperty in includeProperties)
            {
                entities = entities.Include(includeProperty);
            }
            return entities;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
