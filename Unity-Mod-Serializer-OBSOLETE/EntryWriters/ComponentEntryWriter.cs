using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.EntryWriters
{
    public class ComponentEntryWriter : UnityEngineObjectEntryWriter<Component>
    {
        protected override string GetFileName(Component component)
        {
            return string.Format("{0}/{1}", GameObjectEntryWriter.GetGameObjectFolder(component.gameObject), component.name);
        }
        protected override string GetExtension(Component component)
        {
            return component.GetType().Name.ToCamelCase();
        }
    }
}
