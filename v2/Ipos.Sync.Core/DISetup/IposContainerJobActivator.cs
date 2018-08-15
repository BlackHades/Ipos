using Autofac;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.DISetup
{
    public class IposContainerJobActivator : JobActivator
    {
        private IContainer _container;

        public IposContainerJobActivator(IContainer container)
        {
            _container = container;
           // IposDIConfig.Setup(container);
        }

        public override object ActivateJob(Type type)
        {
            return _container.Resolve(type);
        }

    }
}
