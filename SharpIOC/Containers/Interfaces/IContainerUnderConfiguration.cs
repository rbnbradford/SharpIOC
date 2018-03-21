using System;

namespace SharpIOC.Containers.Interfaces
{
    public interface IContainerUnderConfiguration
    {
        IContainerUnderConfiguration RegisterPrivate<T>(
            Func<IContainerInsideRegistration, T> creationFunc,
            bool singleton = true);

        IContainerUnderConfiguration RegisterPublic<T>(
            Func<IContainerInsideRegistration, T> creationFunc,
            bool singleton = true);

        IContainerUnderConfiguration RegisterPrivate<T>(
            string name,
            Func<IContainerInsideRegistration, T> creationFunc,
            bool singleton = true);

        IContainerUnderConfiguration RegisterPublic<T>(
            string name,
            Func<IContainerInsideRegistration, T> creationFunc,
            bool singleton = true);

        IContainerUnderConfiguration SetParameter(string name, object parameter);
        IContainerUnderConfiguration SetParameterFromEnv(string name, string environmentVariableName);
        IContainerSealed Seal();
    }
}
