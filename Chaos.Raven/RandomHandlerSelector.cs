using Castle.MicroKernel;
using System;

namespace Chaos.Raven
{
    internal class RandomHandlerSelector<T> : IHandlerSelector
        where T : class
    {
        public bool HasOpinionAbout(string key, Type service)
        {
            return typeof(T).FullName.Equals(service.FullName);
        }

        public IHandler SelectHandler(string key, Type service, IHandler[] handlers)
        {
            var random = new Random((int)DateTime.UtcNow.Ticks);
            return handlers[random.Next(0, handlers.Length - 1)];
        }
    }
}
