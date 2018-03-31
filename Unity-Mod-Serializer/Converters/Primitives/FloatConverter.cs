using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters.Primitives
{
    public sealed class FloatConverter : DirectConverter<float>
    {
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            // Casting from float to double introduces floating point jitter,
            // ie, 0.1 becomes 0.100000001490116. Casting to decimal as an
            // intermediate step removes the jitter. Not sure why.
            if (instance.GetType() == typeof(float) &&
                // Decimal can't store
                // float.MinValue/float.MaxValue/float.PositiveInfinity/float.NegativeInfinity/float.NaN
                // - an exception gets thrown in that scenario.
                (float)instance != float.MinValue &&
                (float)instance != float.MaxValue &&
                !float.IsInfinity((float)instance) &&
                !float.IsNaN((float)instance)
                )
            {
                serialized = new Data((double)(decimal)(float)instance);
                return Result.Success;
            }

            serialized = new Data((double)Convert.ChangeType(instance, typeof(double)));
            return Result.Success;
        }
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (!data.IsDouble)
                return Result.Fail("Expected type double");

            instance = Convert.ChangeType(data.AsDouble, typeof(float));
            return Result.Success;
        }
    }
}
