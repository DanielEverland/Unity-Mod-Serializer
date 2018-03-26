using System;
using System.Collections.Generic;
using UMS.AOT;
using UMS.Converters;
using UMS.Reflection;

namespace UMS.EntryWriters
{
    /// <summary>
    /// This class allows creators to register
    /// </summary>
    public partial class EntryWriterRegistrar
    {
        static EntryWriterRegistrar()
        {
            Converters = new List<EntryWriter>();

            foreach (var field in typeof(EntryWriterRegistrar).GetDeclaredFields())
            {
                if (field.Name.StartsWith("Register_"))
                {
                    Type type = field.FieldType;

                    if (!typeof(EntryWriter).IsAssignableFrom(type))
                    {
                        UnityEngine.Debug.LogWarning("Tried to register type that doesn't derive from EntryWriter " + type);
                        continue;
                    }

                    EntryWriter writer = (EntryWriter)Activator.CreateInstance(type);

                    if (!Converters.Contains(writer))
                    {
                        Converters.Add(writer);
                    }
                }
            }
        }

        public static List<EntryWriter> Converters;
    }
}