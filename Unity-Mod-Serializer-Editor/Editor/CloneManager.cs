using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.Editor
{
    public static class CloneManager
    {
        static CloneManager()
        {
            _clones = new List<GameObject>();
        }

        private static List<GameObject> _clones = new List<GameObject>();

        public static void Clear()
        {
            foreach (GameObject clone in _clones)
            {
                GameObject.DestroyImmediate(clone);
            }
        }
        public static GameObject GetClone(GameObject obj)
        {
            GameObject clone = GameObject.Instantiate(obj);

            _clones.Add(clone);

            return clone;
        }
    }
}
