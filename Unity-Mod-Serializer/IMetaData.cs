using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace UMS
{
    [ProtoContract]
    [ProtoInclude(100, typeof(TypeMetaData))]
    public interface IMetaData
    {
    }
}
