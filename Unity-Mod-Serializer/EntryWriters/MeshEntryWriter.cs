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
        public MeshEntryWriter Register_MeshEntryWriter;
    }
    public class MeshEntryWriter : UnityEngineObjectEntryWriter<Mesh>
    {
        protected override string GetExtension(Mesh obj)
        {
            return "mesh";
        }
        protected override string GetFileName(Mesh obj)
        {
            return string.Format("{0}/Meshes/{1}", StaticObjects.FOLDER_NAME, obj.name);
        }
    }
}
