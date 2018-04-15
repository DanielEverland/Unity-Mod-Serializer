using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Reflection
{
    /// <summary>
    /// Must be used on static members
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class ReferenceTypesAttribute : Attribute
    {
        public bool IsValid(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return FieldValid(member as FieldInfo);
                case MemberTypes.Method:
                    return MethodValid(member as MethodInfo);
                case MemberTypes.Property:
                    return PropertyValid(member as PropertyInfo);
                default:
                    return false;
            }
        }
        private bool PropertyValid(PropertyInfo property)
        {
            if (property == null)
                return false;
            
            if (property.GetMethod == null)
                return false;

            return MethodValid(property.GetMethod);
        }
        private bool FieldValid(FieldInfo field)
        {
            if (field == null)
                return false;

            if (!field.IsStatic)
                return false;

            if (!typeof(IEnumerable<Type>).IsAssignableFrom(field.FieldType))
                return false;

            return true;
        }
        private bool MethodValid(MethodInfo method)
        {
            if (method == null)
                return false;

            if (!method.IsStatic)
                return false;

            if (!typeof(IEnumerable<Type>).IsAssignableFrom(method.ReturnType))
                return false;

            return true;
        }
        public void AddToManager(MemberInfo member)
        {
            if (!IsValid(member))
                throw new ArgumentException("Member isn't valid");

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    AddAsField(member as FieldInfo);
                    break;
                case MemberTypes.Method:
                    AddAsMethod(member as MethodInfo);
                    break;
                case MemberTypes.Property:
                    AddAsProperty(member as PropertyInfo);
                    break;
            }
        }
        private void AddAsProperty(PropertyInfo property)
        {
            AddAsMethod(property.GetMethod);
        }
        private void AddAsField(FieldInfo field)
        {
            foreach (Type type in (IEnumerable<Type>)field.GetValue(null))
            {
                ReferenceManager.AddType(type);
            }
        }
        private void AddAsMethod(MethodInfo method)
        {
            foreach(Type type in (IEnumerable<Type>)method.Invoke(null, null))
            {
                ReferenceManager.AddType(type);
            }
        }
    }
}
