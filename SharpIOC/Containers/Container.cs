using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using SharpIOC.ContainerConfiguration;
using SharpIOC.Containers.Interfaces;

namespace SharpIOC.Containers
{
    public class Container : IContainerUnderConfiguration, IContainerInsideRegistration
    {
        private readonly Dictionary<int, Func<object>> _services;
        private readonly Dictionary<int, Func<object>> _privateServices;
        private readonly Dictionary<int, object> _parameters;

        private Container()
        {
            _services = new Dictionary<int, Func<object>>();
            _privateServices = new Dictionary<int, Func<object>>();
            _parameters = new Dictionary<int, object>();
        }

        public static IContainerUnderConfiguration Empty()
        {
            return new Container();
        }

        private Container(IContainerConfiguration containerConfiguration) : this()
        {
            containerConfiguration.DefineContainer(this);
        }

        public static IContainerSealed DefinedBy(IContainerConfiguration containerConfiguration)
        {
            return new Container(containerConfiguration).Seal();
        }

        public static IContainerSealed DefinedBy<T>() where T : IContainerConfiguration, new()
        {
            var containerDefinition = Activator.CreateInstance(typeof(T)) as IContainerConfiguration;
            return new Container(containerDefinition).Seal();
        }

        public static IContainerSealed DefinedBy(string containerDefinitionClassName, string assemblyName = null)
        {
            if (assemblyName == null) assemblyName = Assembly.GetCallingAssembly().GetName().Name;

            var type = Type.GetType($"{assemblyName}.{containerDefinitionClassName}, {assemblyName}", true);
            var containerDefinition = Activator.CreateInstance(type) as IContainerConfiguration;
            return new Container(containerDefinition).Seal();
        }

        public IContainerUnderConfiguration RegisterPrivate<T>(
            Func<IContainerInsideRegistration, T> creationFunc,
            bool singleton = true)
        {
            return _Register(typeof(T).GetHashCode(), creationFunc, false, singleton);
        }

        public IContainerUnderConfiguration RegisterPublic<T>(
            Func<IContainerInsideRegistration, T> creationFunc,
            bool singleton = true)
        {
            return _Register(typeof(T).GetHashCode(), creationFunc, true, singleton);
        }

        private IContainerUnderConfiguration _Register<T>(
            int hash,
            Func<IContainerInsideRegistration, T> creationFunc,
            bool @public,
            bool singleton = true)
        {
            object Func() => creationFunc(this);
            var collection = @public ? _services : _privateServices;
            collection[hash] = singleton ? Singletonify(Func) : Func;
            return this;
        }

        public IContainerUnderConfiguration RegisterPrivate<T>(
            string name,
            Func<IContainerInsideRegistration, T> creationFunc,
            bool singleton = true)
        {
            return _Register(name.GetHashCode(), creationFunc, false, singleton);
        }

        public IContainerUnderConfiguration RegisterPublic<T>(
            string name,
            Func<IContainerInsideRegistration, T> creationFunc,
            bool singleton = true)
        {
            return _Register(name.GetHashCode(), creationFunc, true, singleton);
        }

        public IContainerUnderConfiguration SetParameter(string name, object parameter)
        {
            _parameters[name.GetHashCode()] = parameter;
            return this;
        }

        public IContainerUnderConfiguration SetParameterFromEnv(string name, string environmentVariableName)
        {
            return SetParameter(name, Environment.GetEnvironmentVariable(environmentVariableName));
        }

        public IContainerSealed Seal() => new SealedContainer(_services);

        public dynamic GetPrivate<T>()
        {
            return _privateServices.ContainsKey(typeof(T).GetHashCode())
                ? (T) _privateServices[typeof(T).GetHashCode()]()
                : throw new UndefinedComponentException("Private Typed Service", typeof(T).Name);
        }

        public dynamic GetPrivate(string name)
        {
            return _privateServices.ContainsKey(name.GetHashCode())
                ? _privateServices[name.GetHashCode()]()
                : throw new UndefinedComponentException("Private Named Service", name);
        }

        public dynamic Parameter(string parameterName)
        {
            var hash = parameterName.GetHashCode();
            return _parameters.ContainsKey(hash)
                ? _parameters[hash]
                : throw new UndefinedComponentException("Parameter", parameterName);
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

        private static Func<object> Singletonify(Func<object> creationFunc)
        {
            var wrapper = new SingletonWrapper(creationFunc);
            return () => wrapper.GetInstance();
        }
    }
}
