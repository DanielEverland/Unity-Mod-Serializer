using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.EntryWriters
{
    public class PhysicMaterialEntryWriter : UnityEngineObjectEntryWriter<PhysicMaterial>
    {
        protected override string GetExtension(PhysicMaterial obj)
        {
            return "physicMaterial";
        }
        protected override string GetFileName(PhysicMaterial obj)
        {
            return string.Format("{0}/Materials/{1}", StaticObjects.FOLDER_NAME, obj.name);
        }
    }
}
