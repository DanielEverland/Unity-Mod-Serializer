using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.EntryWriters
{
    public class MaterialEntryWriter : UnityEngineObjectEntryWriter<Material>
    {
        protected override string GetExtension(Material obj)
        {
            return "material";
        }
        protected override string GetFileName(Material obj)
        {
            return string.Format("{0}/Materials/{1}", StaticObjects.FOLDER_NAME, obj.name);
        }
    }
}
