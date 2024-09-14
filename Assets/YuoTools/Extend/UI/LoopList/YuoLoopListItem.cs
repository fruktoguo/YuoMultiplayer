using Sirenix.OdinInspector;
using UnityEngine;

namespace YuoTools.UI
{
    public class YuoLoopListItem : MonoBehaviour
    {
        public Rect rect;

        public RectTransform rectTransform;

        [ReadOnly]
        public int Index;

        public virtual void SetData(int index)
        {
            Index = index;
        }
    }
}