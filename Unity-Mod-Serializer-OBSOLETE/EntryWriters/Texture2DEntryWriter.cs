using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.EntryWriters
{
    public class Texture2DEntryWriter : UnityEngineObjectEntryWriter<Texture2D>
    {
        protected override string GetExtension(Texture2D obj)
        {
            return "png";
        }
        protected override string GetFileName(Texture2D obj)
        {
            return string.Format("{0}/Textures/{1}", StaticObjects.FOLDER_NAME, obj.name);
        }
    }
}
