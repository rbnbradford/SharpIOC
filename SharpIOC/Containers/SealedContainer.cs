using System;
using System.Collections.Generic;
using SharpIOC.Containers.Interfaces;

namespace SharpIOC.Containers
{
    public class SealedContainer : IContainerSealed
    {
        private readonly Dictionary<int, Func<object>> _services;

        internal SealedContainer(Dictionary<int, Func<object>> services)
        {
            _services = services;
        }

        public T Get<T>()
        {
            var hash = typeof(T).GetHashCode();
            return _services.ContainsKey(hash)
                ? (T) _services[hash]()
                : throw new UndefinedComponentException("Typed Service", typeof(T).Name);
        }

        public T Get<T>(string name)
        {
            var hash = name.GetHashCode();
            return _services.ContainsKey(hash)
                ? (T) _services[hash]()
                : throw new UndefinedComponentException("Named Service", name);
        }
    }
}
