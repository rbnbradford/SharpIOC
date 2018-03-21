namespace SharpIOC.Containers.Interfaces
{
    public interface IContainerInsideRegistration
    {
        dynamic GetPrivate<T>();
        dynamic GetPrivate(string name);
        dynamic Parameter(string parameterName);
        T Get<T>();
        T Get<T>(string name);
    }
}
