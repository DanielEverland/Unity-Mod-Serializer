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

            return new Manifest.Entry()
            {
                id = id,
                key = Manifest.Instance.GetKey(id),
                path = string.Format("{0}.{1}", GetFileName((T)obj), GetExtension((T)obj)),
            };
        }
    }
}
