using System;

namespace SharpIOC.Containers
{
    public static class FluentApiExtension
    {
        public static T Then<T>(
            this T obj,
            Action<T> action
        )
        {
            action(obj);
            return obj;
        }
    }
}