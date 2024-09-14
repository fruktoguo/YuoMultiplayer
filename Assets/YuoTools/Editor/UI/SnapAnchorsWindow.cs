using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.UI
{
    public class SnapAnchorsWindow : EditorWindow
    {
        const string HighlightColor = "#0ef05d"; // 高亮颜色

        public enum AnchorMode
        {
            Border, // 边缘模式
            Point, // 点模式
        }

        List<RectTransform> objects; // 选中的UI元素列表

        AnchorMode mode = AnchorMode.Border; // 当前锚点模式
        bool parentPosition; // 是否使用父对象空间
        Vector2 point = new Vector2(0.5f, 0.5f); // 对齐点

        // 纹理资源
        Texture2D allBorderPic,
            verticalBorderPic,
            horizontalBorderPic,
            matchParentPic,
            pointPic,
            verticalPointPic,
            horizontalPointPic;

        // GUI样式
        GUIStyle setPivotStyle, selectPointStyle;

        [MenuItem("Tools/YuoTools/对齐锚点", false, 60)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(SnapAnchorsWindow), false, "对齐锚点");
        }

        void OnEnable()
        {
            minSize = new Vector2(240, 340); // 窗口最小尺寸
            
            Selection.selectionChanged += this.Repaint; // 选中对象变化时重绘窗口

            // 加载纹理资源
            allBorderPic = Resources.Load<Texture2D>("snap_all_edges");
            pointPic = Resources.Load<Texture2D>("snap_all_direction_point");
            horizontalPointPic = Resources.Load<Texture2D>("snap_horizontal_point");
            verticalPointPic = Resources.Load<Texture2D>("snap_vertical_point");
            horizontalBorderPic = Resources.Load<Texture2D>("snap_horizontal_edges");
            verticalBorderPic = Resources.Load<Texture2D>("snap_vertical_edges");
            matchParentPic = Resources.Load<Texture2D>("snap_to_parent");
        }

        void OnGUI()
        {
            #region 初始化样式

            if (selectPointStyle == null)
            {
                selectPointStyle = new GUIStyle(EditorStyles.helpBox);
                selectPointStyle.margin = new RectOffset(0, 0, 0, 0);
                selectPointStyle.richText = true;
                selectPointStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (setPivotStyle == null)
            {
                setPivotStyle = new GUIStyle(EditorStyles.miniButton);
                setPivotStyle.richText = true;
                setPivotStyle.alignment = TextAnchor.MiddleCenter;
            }

            #endregion

            // 获取选中的RectTransform对象
            objects = Selection.gameObjects
                .Where((o) => o.transform is RectTransform)
                .Select((o) => o.transform as RectTransform)
                .ToList();

            EditorGUILayout.Space();
            DrawModeSelection(); // 绘制模式选择按钮
            EditorGUILayout.Space();

            bool active = objects.Count > 0; // 是否有选中的对象
            if (!(active))
            {
                EditorGUI.BeginDisabledGroup(true); // 禁用UI控件
            }

            if (objects.Count > 0)
            {
                // 显示选中对象的名称或数量
                string txt = (objects.Count == 1) ? objects[0].name : string.Format("{0} 个UI元素", objects.Count);
                EditorGUILayout.LabelField(txt, EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                // 显示警告信息
                GUIStyle warn = GUI.skin.GetStyle("WarningOverlay");
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.TextArea("没有选中UI元素。", warn);
                GUILayout.Space(5);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            // 根据当前模式绘制不同的UI控件
            switch (mode)
            {
                case AnchorMode.Border:
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    // 对齐所有边缘按钮
                    if (GUILayout.Button(new GUIContent(allBorderPic, "对齐所有边缘"), GUILayout.Width(120),
                            GUILayout.Height(120)))
                        SnapBorder(left: true, right: true, top: true, bottom: true);

                    // 对齐上下边缘按钮
                    if (GUILayout.Button(new GUIContent(verticalBorderPic, "对齐上下边缘"),
                            GUILayout.Width(60), GUILayout.Height(120)))
                        SnapBorder(left: false, right: false, top: true, bottom: true);

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    // 对齐左右边缘按钮
                    if (GUILayout.Button(new GUIContent(horizontalBorderPic, "对齐左右边缘"),
                            GUILayout.Width(120), GUILayout.Height(60)))
                        SnapBorder(left: true, right: true, top: false, bottom: false);

                    // 调整到父对象大小并对齐边缘按钮
                    if (GUILayout.Button(
                            new GUIContent(matchParentPic,
                                "调整到父对象大小并对齐边缘。"),
                            GUILayout.Width(60), GUILayout.Height(60)))
                    {
                        MatchParent();
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    if (!(active))
                        EditorGUI.EndDisabledGroup(); // 结束禁用UI控件
                }
                    break;

                case AnchorMode.Point:

                    DrawPointButtons(); // 绘制对齐点按钮

                    if (!(active))
                        EditorGUI.EndDisabledGroup(); // 结束禁用UI控件

                    GUILayout.Space(-12); // 向上移动一点，因为有空白。
                    parentPosition = EditorGUILayout.ToggleLeft("使用父对象空间", parentPosition); // 使用父对象空间选项

                    point = EditorGUILayout.Vector2Field("对齐点", point); // 对齐点输入框

                    // 设置枢轴按钮
                    string btnText = string.Format("将枢轴设置为 <color={0}>({1:f}, {2:f})</color>", HighlightColor,
                        point.x, point.y);
                    if (GUILayout.Button(btnText, setPivotStyle))
                    {
                        Undo.RecordObjects(objects.Select(o => o as UnityEngine.Object).ToArray(), "设置枢轴");
                        foreach (var obj in objects)
                        {
                            obj.pivot = point;
                        }
                    }

                    break;

                default:
                    break;
            }

            EditorGUILayout.Space();
        }

        // 调整到父对象大小并对齐边缘
        private void MatchParent()
        {
            Undo.RecordObjects(objects.ToArray(), "调整到父对象" + DateTime.Now.ToFileTime());
            foreach (RectTransform obj in objects)
            {
                obj.anchorMin = Vector2.zero;
                obj.anchorMax = Vector2.one;
                obj.anchoredPosition = Vector2.zero;
                obj.sizeDelta = Vector2.zero;
            }
        }

        // 设置对齐点
        private void SetPoint(float x, float y)
        {
            point = new Vector2(x, y);
        }

        // 获取枢轴偏移量
        Vector2 GetPivotOffset(RectTransform obj)
        {
            if (mode == AnchorMode.Border)
                return Vector2.zero;

            Vector2 result;

            if (parentPosition && mode == AnchorMode.Point)
            {
                RectTransform parentTransform = obj.parent as RectTransform;
                Rect parent = (parentTransform != null)
                    ? ToScreenRect(parentTransform, true)
                    : new Rect(0, 0, Screen.width, Screen.height);

                Rect rect = ToScreenRect(obj, true);
                Vector2 p = point;

                result = new Vector2(p.x * parent.width, p.y * parent.height);
                result += parent.position;
                result -= rect.position;
                result = new Vector2(result.x / rect.width, result.y / rect.height) - obj.pivot;
            }
            else
            {
                result = point - obj.pivot;
            }

            return result;
        }

        // 绘制对齐点按钮
        void DrawPointButtons()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // 对齐所有方向按钮
            if (GUILayout.Button(new GUIContent(pointPic, "对齐所有方向"), GUILayout.Width(120),
                    GUILayout.Height(100)))
                SnapPoint(horizontal: true, vertical: true);

            // 垂直对齐按钮
            if (GUILayout.Button(new GUIContent(verticalPointPic, "垂直对齐"), GUILayout.Width(60),
                    GUILayout.Height(100)))
                SnapPoint(horizontal: false, vertical: true);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // 水平对齐按钮
            if (GUILayout.Button(new GUIContent(horizontalPointPic, "水平对齐"),
                    GUILayout.Width(120), GUILayout.Height(60)))
                SnapPoint(horizontal: true, vertical: false);

            EditorGUILayout.BeginVertical();
            var style = selectPointStyle;
            EditorGUILayout.BeginHorizontal();
            DrawSelectionPoint("┌", style, 0f, 1f); // 左上角对齐点
            DrawSelectionPoint("┬", style, 0.5f, 1f); // 上中对齐点
            DrawSelectionPoint("┐", style, 1f, 1f); // 右上角对齐点
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            DrawSelectionPoint("├", style, 0f, 0.5f); // 左中对齐点
            DrawSelectionPoint("┼", style, 0.5f, 0.5f); // 中心对齐点
            DrawSelectionPoint("┤", style, 1f, 0.5f); // 右中对齐点
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            DrawSelectionPoint("└", style, 0f, 0f); // 左下角对齐点
            DrawSelectionPoint("┴", style, 0.5f, 0f); // 下中对齐点
            DrawSelectionPoint("┘", style, 1f, 0f); // 右下角对齐点
            EditorGUILayout.EndHorizontal();

            if (this.objects.Count == 1)
            {
                var p = this.objects[0].pivot;
                string content = "[ 枢轴 ]";
                content = HighlightTextIfMatchCoordinate("[ 枢轴 ]", p.x, p.y);
                if (GUILayout.Button(content, style, GUILayout.Width(60), GUILayout.Height(16)))
                {
                    SetPoint(p.x, p.y);
                }
            }
            else
            {
                GUILayout.Label("");
            }

            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        // 绘制对齐点选择按钮
        private void DrawSelectionPoint(string content, GUIStyle style, float x, float y)
        {
            const float size = 20;
            content = HighlightTextIfMatchCoordinate(content, x, y);

            if (GUILayout.Button(content, style, GUILayout.Width(size), GUILayout.Height(size)))
            {
                SetPoint(x, y);
            }
        }

        // 高亮显示匹配的对齐点
        private string HighlightTextIfMatchCoordinate(string content, float x, float y)
        {
            if (point.x == x && point.y == y)
            {
                content = string.Format("<color={0}>{1}</color>", HighlightColor, content);
            }

            return content;
        }

        // 绘制模式选择按钮
        void DrawModeSelection()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Toggle((mode == AnchorMode.Border), "边缘", EditorStyles.miniButtonLeft)
                && (mode != AnchorMode.Border))
            {
                mode = AnchorMode.Border;
            }

            if (GUILayout.Toggle((mode == AnchorMode.Point), "点", EditorStyles.miniButtonRight)
                && (mode != AnchorMode.Point))
            {
                mode = AnchorMode.Point;
            }

            EditorGUILayout.EndHorizontal();
        }

        // 对齐边缘
        void SnapBorder(bool left, bool right, bool top, bool bottom)
        {
            Undo.SetCurrentGroupName("对齐边缘" + DateTime.Now.ToFileTime());
            int group = Undo.GetCurrentGroup();

            foreach (var obj in objects)
            {
                SnapBorder(obj, left, right, top, bottom);
            }

            Undo.CollapseUndoOperations(group);
        }

        // 对齐边缘的具体实现
        internal static void SnapBorder(RectTransform obj, bool left, bool right, bool top, bool bottom)
        {
            Undo.RecordObject(obj.transform, "对齐锚点边缘");

            Quaternion parentRotation = obj.parent.rotation;
            Quaternion objLocalRotation = obj.localRotation;
            Vector3 objLocalScale = obj.localScale;
            obj.parent.rotation = Quaternion.identity;
            obj.localRotation = Quaternion.identity;
            obj.localScale = Vector3.one;

            RectTransform parentTransform = obj.parent as RectTransform;
            Rect parent = (parentTransform != null)
                ? ToScreenRect(parentTransform)
                : new Rect(0, 0, Screen.width, Screen.height);

            Rect rect = ToScreenRect(obj);

            float sx = CalculateSize(obj.sizeDelta.x, left, right);
            float sy = CalculateSize(obj.sizeDelta.y, top, bottom);
            float x = CalculateAncherPos(obj.pivot.x, sx, left, right,
                obj.anchoredPosition.x); // 计算锚点位置X
            float y = CalculateAncherPos(obj.pivot.y, sy, top, bottom,
                obj.anchoredPosition.y); // 计算锚点位置Y

            if (left || bottom)
            {
                float xMin =
                    CalculateMinAnchor(left, rect.xMin, parent.xMin, parent.size.x,
                        obj.anchorMin.x); // 计算最小锚点X
                float yMin =
                    1 - CalculateMaxAnchor(top, rect.yMax, parent.yMax, parent.size.y,
                        1 - obj.anchorMin.y); // 计算最小锚点Y
                obj.anchorMin = new Vector2(xMin, yMin);
            }

            if (right || top)
            {
                float xMax =
                    CalculateMaxAnchor(right, rect.xMax, parent.xMax, parent.size.x,
                        obj.anchorMax.x); // 计算最大锚点X
                float yMax =
                    1 - CalculateMinAnchor(bottom, rect.yMin, parent.yMin, parent.size.y,
                        1 - obj.anchorMax.y); // 计算最大锚点Y
                obj.anchorMax = new Vector2(xMax, yMax);
            }

            obj.anchoredPosition = new Vector2(x, y);
            obj.sizeDelta = new Vector3(sx, sy);

            obj.parent.rotation = parentRotation;
            obj.localRotation = objLocalRotation;
            obj.localScale = objLocalScale;
        }

        // 对齐点
        void SnapPoint(bool horizontal, bool vertical)
        {
            Undo.SetCurrentGroupName("对齐点" + DateTime.Now.ToFileTime());
            int group = Undo.GetCurrentGroup();

            foreach (var obj in objects)
            {
                Vector2 pivotOffset = GetPivotOffset(obj);
                SnapPoint(obj, pivotOffset, horizontal, vertical);
            }

            Undo.CollapseUndoOperations(group);
        }

        // 对齐点的具体实现
        void SnapPoint(RectTransform obj, Vector2 pivotOffset, bool horizontal, bool vertical)
        {
            Undo.RecordObject(obj.transform, "对齐锚点点");

            Vector2 pivot = obj.pivot + pivotOffset;

            Quaternion parentRotation = obj.parent.rotation;
            Quaternion objLocalRotation = obj.localRotation;
            Vector3 objLocalScale = obj.localScale;
            obj.parent.rotation = Quaternion.identity;
            obj.localRotation = Quaternion.identity;
            obj.localScale = Vector3.one;

            RectTransform parentTransform = obj.parent as RectTransform;
            Rect parent = (parentTransform != null)
                ? ToScreenRect(parentTransform, true)
                : new Rect(0, 0, Screen.width, Screen.height);

            Rect rect = ToScreenRect(obj, true);

            Debug.Log((rect, parent));

            Vector2 pos = new Vector2(pivot.x * rect.width, pivot.y * rect.height);
            pos += rect.position;
            pos -= parent.position;
            pos.x /= parent.width;
            pos.y /= parent.height;

            Vector2 diff = obj.anchoredPosition
                           + new Vector2(pivotOffset.x * rect.width, pivotOffset.y * rect.height);

            if (horizontal && vertical)
            {
                obj.anchorMin = pos;
                obj.anchorMax = pos;
                // obj.sizeDelta = rect.size;
                obj.anchoredPosition -= diff;
            }
            else if (horizontal)
            {
                obj.anchorMin = new Vector2(pos.x, obj.anchorMin.y);
                obj.anchorMax = new Vector2(pos.x, obj.anchorMax.y);
                // obj.sizeDelta = new Vector2(rect.size.x, obj.sizeDelta.y);
                obj.anchoredPosition -= new Vector2(diff.x, 0);
            }
            else if (vertical)
            {
                obj.anchorMin = new Vector2(obj.anchorMin.x, pos.y);
                obj.anchorMax = new Vector2(obj.anchorMax.x, pos.y);
                // obj.sizeDelta = new Vector2(obj.sizeDelta.x, rect.size.y);
                obj.anchoredPosition -= new Vector2(0, diff.y);
            }

            obj.parent.rotation = parentRotation;
            obj.localRotation = objLocalRotation;
            obj.localScale = objLocalScale;
        }

        // 计算最小锚点
        // 计算最小锚点的方法，如果需要计算，则返回内位置与外位置之差除以外尺寸，否则返回备用值
        static float CalculateMinAnchor(bool calculate, float innerPos, float outerPos, float outerSize, float fallback)
        {
            return (calculate) ? (innerPos - outerPos) / outerSize : fallback;
        }

        // 计算最大锚点
        // 计算最大锚点的方法，如果需要计算，则返回1减去外位置与内位置之差除以外尺寸，否则返回备用值
        static float CalculateMaxAnchor(bool calculate, float innerPos, float outerPos, float outerSize, float fallback)
        {
            return (calculate) ? 1 - ((outerPos - innerPos) / outerSize) : fallback;
        }

        // 计算尺寸
        // 根据前后标志计算尺寸，如果前后都有标志，则返回0；如果只有一个标志，则返回尺寸的一半；否则返回原尺寸
        static float CalculateSize(float size, bool front, bool back)
        {
            if (front && back)
            {
                return 0;
            }
            else if (front || back)
            {
                return 0.5f * size;
            }
            else
            {
                return size;
            }
        }

        // 计算锚点位置
        // 根据枢轴、尺寸、前后标志和备用值计算锚点位置，如果前后都没有标志，则返回备用值；如果尺寸为0，则返回0；否则返回尺寸的一半减去枢轴乘以尺寸
        static float CalculateAncherPos(float pivot, float size, bool front, bool back, float fallback)
        {
            if (!(front) && !(back))
                return fallback;

            if (size == 0)
                return 0;

            return 0.5f * size - pivot * size;
        }

        // 转换为屏幕矩形
        // 将RectTransform转换为屏幕矩形的方法，可以选择是否从底部开始，是否使用本地变换，以及指定画布
        public static Rect ToScreenRect(RectTransform self, bool startAtBottom = false, Canvas canvas = null,
            bool localTransform = false)
        {
            Vector3[] corners = new Vector3[4]; // 存储四个角的数组
            Vector3[] screenCorners = new Vector3[2]; // 存储屏幕角的数组

            // 获取四个角的坐标，根据localTransform选择获取本地或世界坐标
            if (localTransform)
            {
                self.GetLocalCorners(corners);
            }
            else
            {
                self.GetWorldCorners(corners);
            }

            // 根据是否从底部开始选择索引
            int idx1 = (startAtBottom) ? 0 : 1;
            int idx2 = (startAtBottom) ? 2 : 3;

            // 如果画布不为空且渲染模式为ScreenSpaceCamera或WorldSpace，则使用画布的相机转换世界坐标到屏幕坐标
            if (canvas != null && (canvas.renderMode == RenderMode.ScreenSpaceCamera ||
                                   canvas.renderMode == RenderMode.WorldSpace))
            {
                screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[idx1]);
                screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[idx2]);
            }
            else
            {
                // 否则使用默认相机转换世界坐标到屏幕坐标
                screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[idx1]);
                screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[idx2]);
            }

            // 如果不是从底部开始，则调整y坐标
            if (!(startAtBottom))
            {
                screenCorners[0].y = Screen.height - screenCorners[0].y;
                screenCorners[1].y = Screen.height - screenCorners[1].y;
            }

            // 返回屏幕矩形
            return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
        }
    }
}