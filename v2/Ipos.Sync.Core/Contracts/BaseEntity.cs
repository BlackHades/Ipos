using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Contracts
{
    public abstract class BaseEntity
    {
    }

    public abstract class BaseEntity<T> : BaseEntity, IEntity<T>
    {
        [Key]
        public virtual T Id { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
        public Boolean IsDeleted { get; set; }
        [Timestamp]
        public Byte[] RowVersion { get; set; }
    }
}
