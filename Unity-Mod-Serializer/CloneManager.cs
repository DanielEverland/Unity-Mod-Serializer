using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS
{
    /// <summary>
    /// Sometimes we have to instantiate objects in-editor in order to
    /// access its properties. MeshFilter.mesh for instance, will throw
    /// an error if you try to access it on a prefab.
    /// </summary>
    [ExecuteInEditMode]
    public class CloneManager : MonoBehaviour
    {
        private static CloneManager _instance;

        private List<Object> _clones = new List<Object>();

        private void Update()
        {
            foreach (Object obj in _clones)
            {
                DestroyImmediate(obj);
            }

            DestroyImmediate(gameObject);
        }
        private T CreateClone<T>(T obj) where T : Object
        {
            T clone = Object.Instantiate(obj);
            _clones.Add(clone);

            clone.name = obj.name;
            clone.hideFlags = obj.hideFlags;

            return clone;
        }
        public static T GetClone<T>(T obj) where T : Object
        {
            return GetInstance().CreateClone(obj);
        }
        private static CloneManager GetInstance()
        {
            if(_instance == null)
            {
                _instance = CreateNewInstance();
            }

            return _instance;
        }
        private static CloneManager CreateNewInstance()
        {
            if (_instance != null)
                throw new System.InvalidOperationException();

            GameObject container = new GameObject();
            container.name = "CloneManager";

            return container.AddComponent<CloneManager>();
        }
    }
}
