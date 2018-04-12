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

            data = Data.Null;

            return result;
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
