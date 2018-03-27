using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.EntryWriters
{
    public partial class EntryWriterRegistrar
    {
        public UnityEngineObjectEntryWriter<UnityEngine.Object> Register_UnityEngineObjectEntryWriter;
    }
    public class UnityEngineObjectEntryWriter<T> : DefaultEntryWriter<T> where T : UnityEngine.Object
    {
        protected override string GetExtension(T obj)
        {
            return obj.GetType().FullName.ToCamelCase();
        }

        protected override string GetFileName(T obj)
        {
            return string.Format("Undefined Objects/{0}", obj.name);
        }
    }
}
