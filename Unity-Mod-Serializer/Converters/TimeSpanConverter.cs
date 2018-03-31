using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters
{
    public sealed class TimeSpanConverter : DateConverter<TimeSpan>
    {
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            var timeSpan = (TimeSpan)instance;
            serialized = new Data(timeSpan.ToString());
            return Result.Success;
        }
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if(data.IsString == false)
            {
                return Result.Fail("Date deserialization requires a string, not " + data.Type);
            }

            TimeSpan result;
            if (TimeSpan.TryParse(data.AsString, out result))
            {
                instance = result;
                return Result.Success;
            }

            return Result.Fail("Unable to parse " + data.AsString + " into a TimeSpan");
        }
    }
}
