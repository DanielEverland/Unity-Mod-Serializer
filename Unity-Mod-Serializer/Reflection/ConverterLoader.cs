using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Reflection
{
    public static class ConverterLoader
    {
        [LoadTypes]
        public static void Poll(Type type)
        {
            if (typeof(BaseConverter).IsAssignableFrom(type))
            {
                if (type.GetCustomAttributes(false).Any(x => x is IgnoreAttribute))
                    return;

                if (type.IsAbstract)
                    return;

                Mods.Serializer.AddConverter((BaseConverter)Activator.CreateInstance(type));
            }
        }
    }
}
