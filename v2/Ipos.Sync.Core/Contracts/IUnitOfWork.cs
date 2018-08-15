using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Contracts
{
    public interface IUnitOfWork<T> : IDisposable
    {
        int SaveChanges();
        void Dispose(bool disposing);
        IRepository<TEntity, T> Repository<TEntity>() where TEntity : BaseEntity;
        void BeginTransaction();
        int Commit();
        void Rollback();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> CommitAsync();
    }
}
