using SharpIOC.Containers.Interfaces;

namespace SharpIOC.ContainerConfiguration
{
    public interface IContainerConfiguration
    {
        void DefineContainer(IContainerUnderConfiguration container);
    }
}
