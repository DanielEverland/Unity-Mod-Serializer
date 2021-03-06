﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf;
using System.Reflection;

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
        public MemberValue() { }
        public MemberValue(int memberID, System.Type type, byte[] data)
        {
            MemberID = memberID;
            TypeName = type.FullName;
            Data = data;
        }
        
        [ProtoMember(1)]
        public int MemberID;
        [ProtoMember(2)]
        public string TypeName;
        [ProtoMember(3)]
        public byte[] Data;

#if DEBUG
        [ProtoMember(4)]
        internal DebugData DebugInfo;
        
        [ProtoContract]
        internal class DebugData
        {
            public DebugData() { }
            public DebugData(MemberInfo memberInfo)
            {
                info.Add("Member Name", memberInfo.Name);
                info.Add("Declared Member Name", memberInfo.DeclaringType.FullName);
                info.Add("Declared Member Assembly", memberInfo.DeclaringType.Assembly.FullName);
            }

            [ProtoMember(1)]
            private Dictionary<string, string> info = new Dictionary<string, string>();

            public override string ToString()
            {
                return string.Join("\n", info.Select(x => $"{x.Key}: {x.Value}"));
            }
        }
#endif
    }
}
