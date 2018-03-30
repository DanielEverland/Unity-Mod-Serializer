using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace UMS.Editor
{
    public static class EditorSession
    {
        public static void Load()
        {
            ObjectContainer.Initialize();

            if (Settings.SimulateBuildLoading)
            {
                SimulateBuildModeLoading();
            }
            else //This is default behaviour
            {
                LoadInEditor();
            }
        }
        private static void LoadInEditor()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:modpackage"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ModPackage package = AssetDatabase.LoadAssetAtPath<ModPackage>(path);

                foreach (ModPackage.ObjectEntry entry in package.ObjectEntries)
                {
                    string id = IDManager.GetID(entry.Object);

                    ObjectContainer.SetObject(id, entry.Key, entry.Object);
                }

                Debug.Log("Loaded " + package.name);
            }
        }
        private static void SimulateBuildModeLoading()
        {
            Debug.LogWarning("SIMULATING BUILT GAME MOD DESERIALIZAION");

            //First we serialize all mods to a temporary directory
            string directory = Path.GetTempPath() + Guid.NewGuid().ToString();
            Directory.CreateDirectory(directory);

            Debug.Log("Serializing to temp dir: " + directory);

            foreach (string guid in AssetDatabase.FindAssets("t:modpackage"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ModPackage package = AssetDatabase.LoadAssetAtPath<ModPackage>(path);

                if (package.IncludeInBuilds)
                {
                    package.Save(directory);
                }                
            }

            Debug.Log("Deserializing temp data");

            foreach (string file in Directory.GetFiles(directory))
            {
                if (Path.GetExtension(file) == ".mod")
                {
                    Mods.Load(file);
                }
            }
        }
    }
}
