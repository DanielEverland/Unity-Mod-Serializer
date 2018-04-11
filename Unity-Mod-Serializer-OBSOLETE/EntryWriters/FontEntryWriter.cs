using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.EntryWriters
{
    public class FontEntryWriter : UnityEngineObjectEntryWriter<Font>
    {
        protected override string GetExtension(Font obj)
        {
            return "font";
        }
        protected override string GetFileName(Font obj)
        {
            return string.Format("{0}/Fonts/{1}", StaticObjects.FOLDER_NAME, obj.name);
        }
    }
}
