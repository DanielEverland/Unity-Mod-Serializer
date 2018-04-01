using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.EntryWriters
{
    public class ShaderEntryWriter : UnityEngineObjectEntryWriter<Shader>
    {
        protected override string GetExtension(Shader obj)
        {
            return "shader";
        }
        protected override string GetFileName(Shader obj)
        {
            return string.Format("{0}/Materials/{1}", StaticObjects.FOLDER_NAME, obj.name);
        }
    }
}
