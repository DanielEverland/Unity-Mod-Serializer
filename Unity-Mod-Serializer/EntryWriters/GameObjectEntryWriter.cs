using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.EntryWriters
{
    public class GameObjectEntryWriter : UnityEngineObjectEntryWriter<GameObject>
    {
        public static string GetGameObjectFolder(GameObject obj)
        {
            return string.Format("GameObjects/{0}", GetGameObjectPath(obj));
        }
        private static string GetGameObjectPath(GameObject obj)
        {
            if (obj == null)
                return string.Empty;

            Transform parent = obj.transform.parent;

            if(parent != null)
            {
                return string.Format("{0}/{1}", GetGameObjectPath(obj.transform.parent.gameObject), obj.name);
            }
            else
            {
                return obj.name;
            }
        }

        protected override string GetFileName(GameObject obj)
        {
            return string.Format("{0}/{1}", GetGameObjectFolder(obj), obj.name);
        }
        protected override string GetExtension(GameObject obj)
        {
            return "gameObject";
        }
    }
}
