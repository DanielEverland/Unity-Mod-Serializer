﻿using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UMS.MemberBlockers
{
    /// <summary>
    /// Member blockers can be used to block members from being serialized
    /// on types which you do not have direct control over, i.e. Unity types
    /// 
    /// They must be used on static fields that return IEnumerable<string>
    /// 
    /// We use the full name of the member, which is the class in which it
    /// is declared, then a dot, and then the member name. I.e. to block the
    /// member called mesh in MeshFilter (since we need to grab sharedMesh),
    /// add a member blocker called "MeshFilter.mesh" - not that this is
    /// case sensitive
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MemberBlockerAttribute : Attribute
    {
        static MemberBlockerAttribute()
        {
            _blockedMembers = new HashSet<string>();
        }

        private static HashSet<string> _blockedMembers;

        public static bool IsBlocked(MemberInfo member)
        {
            return IsBlocked(string.Format("{0}.{1}", member.DeclaringType.Name, member.Name));
        }
        public static bool IsBlocked(string fullName)
        {
            return _blockedMembers.Contains(fullName);
        }
        public static void AddBlockers(IEnumerable<string> enumerable)
        {
            foreach (string member in enumerable)
            {
                AddBlocker(member);
            }
        }
        public static void AddBlocker(string member)
        {
            if (!_blockedMembers.Contains(member))
                _blockedMembers.Add(member);
        }
        public static Result IsValid(FieldInfo info)
        {
            if (!info.IsStatic)
                return Result.Fail("Field must be static!");

            if (!(typeof(IEnumerable<string>).IsAssignableFrom(info.FieldType)))
                return Result.Fail("Field type must be castable to IEnumerable<string>");

            return Result.Success;
        }
    }
}
