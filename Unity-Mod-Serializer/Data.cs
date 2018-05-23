using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf;

namespace UMS
{
    [ProtoContract]
    public class Data
    {
        [ProtoMember(1)]
        private int memberID;
        [ProtoMember(2)]
        private System.Type type;
        [ProtoMember(3)]
        private byte[] data;
    }
}
