using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace UMS.Editor
{
    public class ModBrowserTreeView : TreeView
    {
        //We don't care about serializing the state
        public ModBrowserTreeView() : base(new TreeViewState()) { }

        public ModFile.Entry ActiveObject;

        private Dictionary<Data, int> _ids;
        
        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(-1, -1, "Root");
            _ids = new Dictionary<Data, int>();

            AddToItem(root, ActiveObject.Key, ActiveObject.Data);

            SetupDepthsFromParentsAndChildren(root);

            return root;
        }
        private void AddToItem(TreeViewItem parent, string key, Data data)
        {
            TreeViewItem item = new TreeViewItem(GetID(data));
            string dataString = DataToDisplayName(data);

            if (key == null)
            {
                item.displayName = dataString;
            }
            else
            {
                item.displayName = string.Format(@"""{0}"": {1}", key, dataString);
            }

            parent.AddChild(item);

            CallChildren(item, data);
        }
        private void CallChildren(TreeViewItem parent, Data data)
        {
            if (data.IsDictionary)
            {
                foreach (KeyValuePair<string, Data> keyvaluePair in data.Dictionary)
                {
                    AddToItem(parent, keyvaluePair.Key, keyvaluePair.Value);
                }
            }
            else if (data.IsList)
            {
                foreach (Data dataInstance in data.List)
                {
                    AddToItem(parent, null, dataInstance);
                }
            }
        }
        private int GetID(Data data)
        {
            if (!_ids.ContainsKey(data))
            {
                HashSet<int> existingIDs = new HashSet<int>(_ids.Values);
                int id = 0;

                while (existingIDs.Contains(id) || id == 0)
                {
                    id = Utility.GetRandomInt();
                }

                _ids.Add(data, id);
            }

            return _ids[data];
        }
        private string DataToDisplayName(Data data)
        {
            if (data.IsDictionary)
            {
                return "Dictionary";
            }
            else if (data.IsList)
            {
                return "List";
            }

            return data.ToString();
        }
    }
}
