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

namespace Chaos.Raven
{
    public class ActionStore : IDisposable
    {
        private readonly IWindsorContainer container;

        public ActionStore(string actionsFolder)       
        {
            container = new WindsorContainer();

            container.Register(Classes.FromAssemblyInThisApplication()
                                      .BasedOn<BaseAction>()
                                      .OrBasedOn(typeof(BaseVerificationAction))
                                      .WithServiceBase()
                                      .LifestyleTransient());

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(actionsFolder))
                                      .BasedOn<BaseAction>()
                                      .OrBasedOn(typeof(BaseVerificationAction))
                                      .WithServiceBase()
                                      .LifestyleTransient());

            container.Kernel.AddHandlerSelector(new RandomHandlerSelector<BaseAction>());
            container.Kernel.AddHandlerSelector(new RandomHandlerSelector<BaseVerificationAction>());
        }

        public BaseAction GetRandomChaosAction()
        {
            return container.Resolve<BaseAction>();
        }

        public BaseVerificationAction GetRandomVerificationAction()
        {
            return container.Resolve<BaseVerificationAction>();
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}
