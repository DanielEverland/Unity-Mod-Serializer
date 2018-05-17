using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Reflection
{
    public class ModelLoader
    {
        [LoadTypes]
        private static void Poll(Type type)
        {
            if(typeof(IModel).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
            {
                Serializer.AddModel((IModel)Activator.CreateInstance(type));
            }
        }
    }
}
