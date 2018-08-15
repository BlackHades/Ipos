using Autofac;
using Ipos.Sync.Core.Context;
using Ipos.Sync.Core.Contracts;
using Ipos.Sync.Core.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.DISetup
{
    public class IposDIConfig
    {
        public static void Setup(IContainer container)
        {
            var builder = new ContainerBuilder();
            var context = new IposSyncContext();
            var uof = new UnitOfWork<Guid>(context);
            builder.RegisterInstance(uof).As<IUnitOfWork<Guid>>();
        }
    }
}
