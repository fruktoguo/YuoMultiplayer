using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace YuoTools.UI
{
    [CreateAssetMenu(fileName = "UIAutoLoadConfig", menuName = "YuoTools/UIAutoLoadConfig", order = 0)]
    public class UIAutoLoadConfig : ScriptableObject
    {
        [SerializeField] private List<GameObject> autoLoadList;
        [ReadOnly] [SerializeField] private List<GameObject> linkDirList;

        public List<GameObject> AutoLoadList
        {
            get
            {
                var list = new List<GameObject>(autoLoadList);
                list.AddRange(linkDirList);
                return list;
            }
        }

#if UNITY_EDITOR
        [FolderPath] [SerializeField] [OnValueChanged(nameof(OnLinkDirChange))]
        public List<string> linkDir = new();

        void OnLinkDirChange()
        {
            linkDirList.Clear();
            if (linkDir.Count == 0) return;
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:GameObject", linkDir.ToArray());

            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                GameObject gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (gameObject != null)
                {
                    if (!linkDirList.Contains(gameObject))
                    {
                        linkDirList.Add(gameObject);
                    }
                }
            }
        }

        [Button]
        public void Refresh()
        {
            OnLinkDirChange();
        }

        private void OnValidate()
        {
            OnLinkDirChange();
        }
#endif
    }
}