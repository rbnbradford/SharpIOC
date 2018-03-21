using System;

namespace SharpIOC.Containers
{
    internal class SingletonWrapper
    {
        private object _cache;
        private readonly Func<object> _creationFunc;
        public SingletonWrapper(Func<object> creatioFunc) => _creationFunc = creatioFunc;
        public object GetInstance() => _cache ?? (_cache = _creationFunc());
    }
}
