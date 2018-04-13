using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class CharacterConverter : DirectConverter<char>
    {
        public override Result Serialize(char obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result Deserialize(Data data, ref char obj)
        {
            if (!data.IsString)
                return Result.Error("Type mismatch. Expected String", data);

            obj = System.Convert.ToChar(data.AsString);
            return Result.Success;
        }
    }
}
