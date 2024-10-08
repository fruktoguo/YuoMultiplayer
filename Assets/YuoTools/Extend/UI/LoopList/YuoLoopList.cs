using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace YuoTools.UI
{
    public class YuoLoopList : MonoBehaviour, IGenerateCode
    {
        /// <summary>
        /// 单元格尺寸（宽，高）
        /// </summary>
        [FoldoutGroup("单元格")] [LabelText("大小")]
        public Vector2 cellSize = Vector2.one * 100;

        /// <summary>
        /// 单元格尺寸缩放
        /// </summary>
        [FoldoutGroup("单元格")] [LabelText("缩放")]
        public Vector2 cellSizeScale = Vector2.one;

        /// <summary>
        /// 单元格间隙（水平，垂直）
        /// </summary>
        [FoldoutGroup("单元格")] [LabelText("间距")]
        public Vector2 spacingSize;

        /// <summary>
        /// 列数
        /// </summary>
        [FoldoutGroup("单元格")] [LabelText("横向")] [HorizontalGroup("单元格/1")]
        public bool horizontal = true;

        [FoldoutGroup("单元格")] [LabelText("列/行数")] [HorizontalGroup("单元格/1")]
        public int columnCount = 4;

        /// <summary>
        /// 单元格渲染器prefab
        /// </summary>
        [FoldoutGroup("单元格")] [LabelText("单元格预设体")]
        public YuoLoopListItem renderGo;

        [FoldoutGroup("单元格/外边距")] public float top, bottom, left, right;

        /// <summary>
        /// 渲染格子数
        /// </summary>
        private int _mRendererCount;

        [LabelText("额外渲染格子行列数")] public int extraRenderingCount;

        /// <summary>
        /// 父节点蒙版尺寸
        /// </summary>
        [HideInInspector] public Vector2 mMaskSize;

        /// <summary>
        /// 蒙版矩形
        /// </summary>
        [SerializeField] private Rect _mRectMask;

        public ScrollRect mScrollRect;

        /// <summary>
        /// 转换器
        /// </summary>
        public RectTransform mRectTransformContainer;

        /// <summary>
        /// 渲染脚本集合
        /// </summary>
        [SerializeField] private List<YuoLoopListItem> _mListItems;

        /// <summary>
        /// 数据提供者
        /// </summary>
        [SerializeField] private List<Vector2Int> _mDataProviders;

        public List<Rect> rectData = new();

        /// <summary>
        /// 初始化渲染脚本
        /// </summary>
        void Init()
        {
            //获得蒙版尺寸
            mMaskSize = mScrollRect.GetComponent<RectTransform>().sizeDelta;

            _mRectMask.size = mMaskSize;

            //通过蒙版尺寸和格子尺寸计算需要的单元格个数
            _mRendererCount = columnCount * (Mathf.CeilToInt(mMaskSize.y / BlockSizeY) + 1 + extraRenderingCount * 2);

            _mListItems = new List<YuoLoopListItem>();

            //生成单元格
            for (int i = 0; i < _mRendererCount; ++i)
            {
                GameObject child = Instantiate(renderGo.gameObject, mRectTransformContainer, true);

                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = Vector3.one;
                child.layer = gameObject.layer;
                child.name = "Item_" + i;
                ((RectTransform)child.transform).sizeDelta = cellSize * cellSizeScale;
                YuoLoopListItem item = child.GetComponent<YuoLoopListItem>();

                if (item == null)
                {
                    item = child.AddComponent<YuoLoopListItem>();
                }

                item.SetData(i);

                _mListItems.Add(item);

                child.SetActive(true);
                var rectTransform = child.transform as RectTransform;
                item.rectTransform = rectTransform;
                if (rectTransform != null)
                {
                    rectTransform.pivot = Vector2.up;
                    rectTransform.anchorMin = Vector2.up;
                    rectTransform.anchorMax = Vector2.up;
                }

                if (i >= _mDataProviders.Count)
                {
                    item.Hide();
                }

                UpdateChildTransformPos(item, i);
            }

            mHasInitialized = true;
        }


        /// <summary>
        /// 更新各个渲染格子的位置
        /// </summary>
        /// <param name="child"></param>
        /// <param name="index"></param>
        private void UpdateChildTransformPos(YuoLoopListItem child, int index)
        {
            if (index >= _mDataProviders.Count) return;

            int row = index / columnCount;
            int column = index % columnCount;

            if (!horizontal) (row, column) = (column, row);

            Temp.V2.Set(column * BlockSizeX + left, -BlockSizeY * row - top);

            child.rectTransform.anchoredPosition3D = Vector3.zero;
            child.rectTransform.anchoredPosition = Temp.V2;
            child.rect = rectData[index];
        }

        Dictionary<int, Rect> _inOverlaps = new();

        void UpdateRender()
        {
            _mRectMask.y = -mMaskSize.y - mRectTransformContainer.anchoredPosition.y;
            _inOverlaps.Clear();

            int min = (int)(rectData.Count * (1 - mScrollRect.verticalNormalizedPosition) - _mRendererCount);
            min.Clamp();
            int max = (int)(rectData.Count * (1 - mScrollRect.verticalNormalizedPosition) + _mRendererCount);
            max.Clamp(rectData.Count);

            var newMask = _mRectMask;
            if (extraRenderingCount > 0)
            {
                Temp.V2.Set(BlockSizeX * extraRenderingCount * 2, BlockSizeY * extraRenderingCount * 2);
                newMask.size += Temp.V2;
                Temp.V2.Set(BlockSizeX * extraRenderingCount, BlockSizeY * extraRenderingCount);
                newMask.position -= Temp.V2;
            }

            //获取可能会刷新的格子
            for (int i = min; i < max; i++)
            {
                if (rectData[i].Overlaps(newMask))
                {
                    _inOverlaps.Add(i, rectData[i]);
                }
            }

            WaitItems.Clear();

            foreach (var item in _mListItems)
            {
                //如果格子不在矩形内，则放到等待列表中
                if (!item.rect.Overlaps(_mRectMask))
                {
                    WaitItems.Add(item);
                }
            }

            // (WaitItems.Count, _inOverlaps.Count).Log();
            foreach (var dR in _inOverlaps)
            {
                if (GetItem(dR.Value) == null)
                {
                    YuoLoopListItem item = GetWaitItem();
                    if (item != null)
                    {
                        item.rect = dR.Value;

                        var index = dR.Key;


                        if (_mDataProviders != null)
                        {
                            if (index >= _mDataProviders.Count)
                            {
                                item.Hide();
                            }
                            else
                            {
                                item.SetData(index);
                                item.Show();
                                UpdateChildTransformPos(item, index);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"没有可用的单元格{WaitItems.Count}");
                    }
                }
                else
                {
                    //Debug.Log("检测到不需要刷新的格子");
                }
            }
        }

        public void UpdateData()
        {
            foreach (var item in _mListItems)
            {
                if (item.rect.Overlaps(_mRectMask))
                {
                    item.SetData(item.Index);
                }
            }
        }

        public List<YuoLoopListItem> WaitItems = new();

        YuoLoopListItem GetWaitItem()
        {
            if (WaitItems.Count > 0)
            {
                YuoLoopListItem item = WaitItems[0];
                WaitItems.RemoveAt(0);
                return item;
            }

            return null;
        }

        YuoLoopListItem GetItem(Rect rect)
        {
            foreach (var item in _mListItems)
            {
                if (item.rect == rect)
                {
                    return item;
                }
            }

            return null;
        }

        public void SetData(List<Vector2Int> data)
        {
            _mDataProviders = data;

            SetAllRect();

            if (mMaskSize == Vector2.zero) Init();

            UpdateRender();
            UpdateItemData();
        }

        public void UpdateItemData()
        {
            foreach (var item in _mListItems)
            {
                item.SetData(item.Index);
            }
        }

        void SetAllRect()
        {
            rectData.Clear();
            for (int i = 0; i < _mDataProviders.Count; i++)
            {
                int row = i / columnCount;
                int column = i % columnCount;
                Rect dRect = new Rect(column * BlockSizeX,
                    -row * BlockSizeY - cellSize.y * cellSizeScale.y, cellSize.x * cellSizeScale.x,
                    cellSize.y * cellSizeScale.y);
                rectData.Add(dRect);
            }

            //计算容器大小
            if (horizontal)
            {
                var rowCount = Mathf.CeilToInt(_mDataProviders.Count / (float)columnCount);
                mRectTransformContainer.sizeDelta =
                    new Vector2(left + right + cellSize.x * cellSizeScale.x * columnCount +
                                spacingSize.x * (columnCount - 1),
                        top + bottom + rowCount * cellSize.y * cellSizeScale.y + spacingSize.y * (rowCount - 1));
            }
            else
            {
                mRectTransformContainer.sizeDelta = new Vector2(
                    left + right + Mathf.CeilToInt(_mDataProviders.Count / (float)columnCount) * BlockSizeY,
                    top + bottom + mRectTransformContainer.sizeDelta.y);
            }

            //重置滚动条
            mScrollRect.verticalNormalizedPosition = 1;
        }

        private Vector2 lastPos = new(-9999999, -999999);

        protected bool mHasInitialized;

        private void Update()
        {
            if (mHasInitialized)
            {
                //如果没有运动.几帧后会停止刷新
                if (!lastPos.x.ApEqual(mRectTransformContainer.anchoredPosition.x) ||
                    !lastPos.y.ApEqual(mRectTransformContainer.anchoredPosition.y))
                {
                    UpdateRender();
                    lastPos = Vector2.Lerp(lastPos, mRectTransformContainer.anchoredPosition, 0.9f);
                }
            }
        }

        /// <summary>
        /// 获得格子块尺寸
        /// </summary>
        /// <returns></returns>
        private float BlockSizeY =>
            cellSize.y * cellSizeScale.y + spacingSize.y;

        private float BlockSizeX =>
            cellSize.x * cellSizeScale.x + spacingSize.x;
    }
}