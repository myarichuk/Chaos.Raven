using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Chaos.Raven.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Troschuetz.Random.Distributions.Continuous;

namespace Chaos.Raven
{
    public class ActionStore : IDisposable
    {
        private readonly IWindsorContainer container;
        private volatile bool isDisposed;
        public ActionStore(string actionsFolder)
        {
            isDisposed = false;
            container = new WindsorContainer();

            container.Register(Classes.FromAssemblyInThisApplication()
                                      .BasedOn<ChaosAction>()
                                      .OrBasedOn(typeof(VerificationAction))
                                      .WithServiceBase()
                                      .LifestyleTransient());

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(actionsFolder))
                                      .BasedOn<ChaosAction>()
                                      .OrBasedOn(typeof(VerificationAction))
                                      .WithServiceBase()
                                      .LifestyleTransient());

            container.Kernel.AddHandlerSelector(new RandomHandlerSelector<ChaosAction>());
            container.Kernel.AddHandlerSelector(new RandomHandlerSelector<VerificationAction>());
        }

        public ChaosAction GetRandomChaosAction()
        {
            if (isDisposed)
                throw new ObjectDisposedException("ActionStore");
            return container.Resolve<ChaosAction>();
        }

        public VerificationAction GetRandomVerificationAction()
        {
            if (isDisposed)
                throw new ObjectDisposedException("ActionStore");
            return container.Resolve<VerificationAction>();
        }

        public void Dispose()
        {
            isDisposed = true;
            container.Dispose();
        }
    }
}
