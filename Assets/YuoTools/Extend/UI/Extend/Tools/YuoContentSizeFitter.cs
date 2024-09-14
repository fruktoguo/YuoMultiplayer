using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YuoTools
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class YuoContentSizeFitter : UIBehaviour, ILayoutSelfController
    {
        [System.NonSerialized] private RectTransform _mRect;

        [SerializeField] private RectTransform targetRect;

        private RectTransform rectTransform
        {
            get
            {
                if (_mRect == null)
                    _mRect = GetComponent<RectTransform>();
                return _mRect;
            }
        }

        // field is never assigned warning
#pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;
#pragma warning restore 649

        private YuoContentSizeFitter()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        public Vector2 minSize;
        public Vector2 maxSize;

        [FoldoutGroup("Padding")] public float width, height;

        private void HandleSelfFittingAlongAxis(int axis)
        {
            var a = (RectTransform.Axis)axis;
            var preferredSize = targetRect.GetPreferredSize();
            preferredSize.x += width;
            preferredSize.y += height;
            preferredSize.x = preferredSize.x.RClamp(minSize.x, maxSize.x < 0.001f ? float.MaxValue : maxSize.x);
            preferredSize.y = preferredSize.y.RClamp(minSize.y, maxSize.y < 0.001f ? float.MaxValue : maxSize.y);
            rectTransform.SetSizeWithCurrentAnchors(a,
                a == RectTransform.Axis.Horizontal ? preferredSize.x : preferredSize.y);
        }

        public void SetLayoutHorizontal()
        {
            m_Tracker.Clear();
            HandleSelfFittingAlongAxis(0);
            Debug.Log("SetLayoutHorizontal");
        }

        public void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
            Debug.Log("SetLayoutVertical");
        }

        private void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            Debug.Log("SetDirty");
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
    }
}