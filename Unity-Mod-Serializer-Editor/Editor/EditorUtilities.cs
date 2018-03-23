using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace UMS.Editor
{
    public static class EditorUtilities
    {
        public const int MENU_ITEM_PRIORITY = 100;
        public const string MENU_ITEM_ROOT = "Modding";
        public const string MENU_SERIALIZATION = "Serialization";

        public static string GetID(object obj)
        {
            if(obj is IIDObject idObject)
            {
                return idObject.ID;
            }
            else if(obj is Object unityObject)
            {
                return unityObject.GetInstanceID().ToString();
            }
            else
            {
                throw new System.ArgumentException();
            }            
        }
        public static bool CanGetID(object obj)
        {
            return obj is IIDObject || obj is Object;
        }
        public static List<ModPackage> GetAllPackages()
        {
            return new List<ModPackage>(AssetDatabase.FindAssets("t:ModPackage").Select(x =>
            {
                return AssetDatabase.LoadAssetAtPath<ModPackage>(AssetDatabase.GUIDToAssetPath(x));
            }));
        }
    }
}
