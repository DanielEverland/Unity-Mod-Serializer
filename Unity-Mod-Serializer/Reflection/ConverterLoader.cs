using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Converters;

namespace UMS.Reflection
{
    public static class ConverterLoader
    {
        [LoadTypes]
        public static void Poll(Type type)
        {
            if (typeof(BaseConverter).IsAssignableFrom(type))
            {
                if (type.IsAbstract)
                    return;

                Mods.Serializer.AddConverter((BaseConverter)Activator.CreateInstance(type));
            }

            if (typeof(IBinaryConverter).IsAssignableFrom(type))
            {
                if (type.IsAbstract)
                    return;

                Mods.Serializer.BinarySerializer.AddConverter((IBinaryConverter)Activator.CreateInstance(type));
            }
        }
    }
}
