using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.EntryWriters
{
    /// <summary>
    /// Premade entry writer which makes serializing objects easier
    /// </summary>
    public abstract class DefaultEntryWriter<T> : EntryWriter
    {
        public override Type WriterType => typeof(T);

        protected abstract string GetFileName(T obj);
        protected abstract string GetExtension(T obj);

        public override Manifest.Entry Write(object obj)
        {
            string id = IDManager.GetID(obj);
            IEnumerable<string> keys = Manifest.Instance.GetKeys(id);
            string path = string.Format("{0}.{1}", GetFileName((T)obj), GetExtension((T)obj));

            return new Manifest.Entry(id, path, keys);
        }
    }
}
