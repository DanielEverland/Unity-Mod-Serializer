using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMS.Converters;

namespace UMS
{
    public static class Serializer
    {
        static Serializer()
        {
            _directConverters = new Dictionary<System.Type, IDirectConverter>();
            _converters = new List<IBaseConverter>();
        }

        private static Dictionary<System.Type, IDirectConverter> _directConverters;
        private static List<IBaseConverter> _converters;

        public static Result Serialize(object value, out Data data)
        {
            Result result = Result.Success;

            System.Type objType = value.GetType();
            IBaseConverter converter = GetConverter(objType);

            result += converter.Serialize(value, out data);

            return result;
        }
        private static IBaseConverter GetConverter(System.Type type)
        {
            IBaseConverter converter = null;

            if (_directConverters.ContainsKey(type))
            {
                converter = _directConverters[type];
            }
            else
            {
                converter = TypeInheritanceTree.GetClosestType(_converters, type, x => x.ModelType);
            }

            if(converter == null)
            {
                throw new System.NotImplementedException("Couldn't find converter for " + type);
            }

            return converter;
        }
        public static void AddConverter(IBaseConverter converter)
        {
            if(converter is IDirectConverter directConverter)
            {
                _directConverters.Add(directConverter.ModelType, directConverter);
            }
            else
            {
                _converters.Add(converter);
            }
        }
    }
}
