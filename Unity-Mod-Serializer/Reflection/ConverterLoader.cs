using System;
using UMS.Converters;

namespace UMS.Reflection
{
    public static class ConverterLoader
    {
        [LoadTypes]
        public static void Poll(Type type)
        {
            if (typeof(IBaseConverter).IsAssignableFrom(type))
            {
                if (type.IsAbstract)
                    return;

                Serializer.AddConverter((IBaseConverter)Activator.CreateInstance(type));
            }
        }
    }
}
