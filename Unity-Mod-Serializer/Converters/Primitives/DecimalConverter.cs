using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class DecimalConverter : DirectConverter<decimal>
    {
        public override Result DoSerialize(decimal obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref decimal obj)
        {
            if (!data.IsDecimal)
                return Result.Error("Type mismatch. Expected Decimal", data);

            obj = data.Decimal;
            return Result.Success;
        }
    }
}
