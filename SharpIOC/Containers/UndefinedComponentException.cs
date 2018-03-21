using System;

namespace SharpIOC.Containers
{
    internal class UndefinedComponentException : Exception
    {
        public UndefinedComponentException()
        {
        }

        public UndefinedComponentException(string type, string name) : base(PrepareMessage(type, name))
        {
        }

        private static string PrepareMessage(string type, string name)
        {
            return $"no '{type}': '{name}' is registered";
        }
    }
}
