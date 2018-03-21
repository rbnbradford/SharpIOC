namespace SharpIOC.Containers.Interfaces
{
    public interface IContainerSealed
    {
        T Get<T>();
        T Get<T>(string name);
    }
}