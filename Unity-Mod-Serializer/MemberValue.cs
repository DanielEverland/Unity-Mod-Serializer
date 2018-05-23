using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf;

namespace UMS
{
    // We have to wrap out proto data in this class, otherwise we encounter a myriad of exceptions
    // A better alternative would be to serialize component data as a Dictionary[int, object],
    // with the DynamicType attribute property set to true. Unfortunately that results in two issues

    // The first is that the object *must* be a contract, so we can't serialize primitives directly.
    // This is easily "fixable" by creating a contract for each type, although that's definitely a hack.

    // The second issue is that we get an "Unknown sub-type" exception. That has me stumped. This is,
    // obviously, usually because we haven't used the [ProtoInclude] attribute on inherited types. Thing
    // is, though, that none of the types have *any* inhertiance at all. Perhaps a bug in protobuf?
    [ProtoContract]
    public class MemberValue
    {
        [ProtoMember(1)]
        public int MemberID;
        [ProtoMember(2)]
        public System.Type Type;
        [ProtoMember(3)]
        public byte[] Data;
    }
}
