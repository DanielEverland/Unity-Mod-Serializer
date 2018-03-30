using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace UMS.Editor
{
    public static class EditorSession
    {
        public static void Load()
        {
            ObjectContainer.Initialize();

            foreach (string guid in AssetDatabase.FindAssets("t:modpackage"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ModPackage package = AssetDatabase.LoadAssetAtPath<ModPackage>(path);

                foreach (ModPackage.ObjectEntry entry in package.ObjectEntries)
                {
                    string id = IDManager.GetID(entry.Object);

                    ObjectContainer.SetObject(id, entry.Key, entry.Object);
                }

                UnityEngine.Debug.Log("Loaded " + package.name);
            }
        }
    }
}
