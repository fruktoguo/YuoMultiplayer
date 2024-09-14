using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Unity.EditorCoroutines.Editor;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;

#else
using UnityEngine.Experimental.UIElements;
#endif

// 注意：由于这个类是基于反射的，所以它有点"黑科技"

namespace Toolbox.Editor
{
    /// <summary>
    /// 工具箱编辑器工具栏是一个扩展到Unity's经典场景工具栏的工具栏，提供新的功能。
    /// </summary>
    [InitializeOnLoad]
    public static class ToolboxEditorToolbar
    {
        static ToolboxEditorToolbar()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(Initialize());
        }

        // 定义一些基础类型和属性
        private static readonly Type ContainerType = typeof(IMGUIContainer);
        private static readonly Type ToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly Type GUIViewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GUIView");
#if UNITY_2020_1_OR_NEWER
        private static readonly Type BackendType =
            typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.IWindowBackend");

        private static readonly PropertyInfo GUIBackend = GUIViewType.GetProperty("windowBackend",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly PropertyInfo VisualTree = BackendType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
        private static readonly PropertyInfo visualTree = guiViewType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        private static readonly FieldInfo OnGuiHandler = ContainerType.GetField("m_OnGUIHandler",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        // 工具栏对象
        private static Object toolbar;

        // 初始化函数
        private static IEnumerator Initialize()
        {
            // 当工具栏为空时
            while (ToolboxEditorToolbar.toolbar == null)
            {
                var toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
                if (toolbars == null || toolbars.Length == 0)
                {
                    // 如果没有找到工具栏，则返回null并继续
                    yield return null;
                    continue;
                }
                else
                {
                    // 找到工具栏，并赋值给内部变量
                    ToolboxEditorToolbar.toolbar = toolbars[0];
                }
            }

            // 使用不同Unity版本进行特定初始化
#if UNITY_2021_1_OR_NEWER
            var rootField = ToolboxEditorToolbar.toolbar.GetType()
                .GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            if (rootField != null)
            {
                var root = rootField.GetValue(ToolboxEditorToolbar.toolbar) as VisualElement;


                var visualElementLeft = root.Q("ToolbarZoneLeftAlign");

                var element = new VisualElement()
                {
                    style =
                    {
                        flexGrow = 1,
                        flexDirection = FlexDirection.Row,
                    }
                };

                var containerLeft = new IMGUIContainer();
                containerLeft.name = "额外左Toolbar";
                containerLeft.style.flexGrow = 1;
                containerLeft.onGUIHandler += OnGuiLeft;

                element.Add(containerLeft);
                visualElementLeft.Add(element);

               var visualElementRight = root.Q("ToolbarZoneRightAlign");

                var element2 = new VisualElement()
                {
                    style =
                    {
                        flexGrow = 1, 
                        flexDirection = FlexDirection.Row,
                    }
                };
                
                var containerRight = new IMGUIContainer();
                containerRight.name = "额外右Toolbar";
                containerRight.style.flexGrow = 1;
                containerRight.onGUIHandler += OnGuiRight;
                
                element2.Add(containerRight);
                visualElementRight.Add(element2);
            }

#else
#if UNITY_2020_1_OR_NEWER
            var backend = guiBackend.GetValue(toolbar);
            var elements = visualTree.GetValue(backend, null) as VisualElement;
#else
            var elements = visualTree.GetValue(toolbar, null) as VisualElement;
#endif

#if UNITY_2019_1_OR_NEWER
            var container = elements[0];
#else
            var container = elements[0] as IMGUIContainer;
#endif
            var handler = onGuiHandler.GetValue(container) as Action;
            handler -= OnGui;
            handler += OnGui;
            onGuiHandler.SetValue(container, handler);
#endif
        }

        // OnGui函数，用于处理工具栏的GUI事件
        private static void OnGuiLeft()
        {
            // 如果工具栏不允许或者没有对应的GUI事件，则直接返回
            if (!IsToolbarAllowed || OnToolbarGuiLeft == null)
            {
                return;
            }

#if UNITY_2021_1_OR_NEWER
            using (new GUILayout.HorizontalScope())
            {
                OnToolbarGuiLeft.Invoke();
            }
#else
            var screenWidth = EditorGUIUtility.currentViewWidth;
            var toolbarRect = new Rect(0, 0, screenWidth, Style.rowHeight);
            //calculations known from UnityCsReference
            toolbarRect.xMin += FromToolsOffset;
            toolbarRect.xMax = (screenWidth - FromStripOffset) / 2;
            toolbarRect.xMin += Style.spacing;
            toolbarRect.xMax -= Style.spacing;
            toolbarRect.yMin += Style.topPadding;
            toolbarRect.yMax -= Style.botPadding;

            if (toolbarRect.width <= 0)
            {
                return;
            }

            using (new GUILayout.AreaScope(toolbarRect))
            {
                using (new GUILayout.HorizontalScope())
                {
                    OnToolbarGui?.Invoke();
                }
            }
#endif
        }

        private static void OnGuiRight()
        {
            // 如果工具栏不允许或者没有对应的GUI事件，则直接返回
            if (!IsToolbarAllowed || OnToolbarGuiRight == null)
            {
                return;
            }

#if UNITY_2021_1_OR_NEWER
            using (new GUILayout.HorizontalScope())
            {
                OnToolbarGuiRight.Invoke();
            }
#else
            var screenWidth = EditorGUIUtility.currentViewWidth;
            var toolbarRect = new Rect(0, 0, screenWidth, Style.rowHeight);
            //calculations known from UnityCsReference
            toolbarRect.xMin += FromToolsOffset;
            toolbarRect.xMax = (screenWidth - FromStripOffset) / 2;
            toolbarRect.xMin += Style.spacing;
            toolbarRect.xMax -= Style.spacing;
            toolbarRect.yMin += Style.topPadding;
            toolbarRect.yMax -= Style.botPadding;

            if (toolbarRect.width <= 0)
            {
                return;
            }

            using (new GUILayout.AreaScope(toolbarRect))
            {
                using (new GUILayout.HorizontalScope())
                {
                    OnToolbarGui?.Invoke();
                }
            }
#endif
        }

        // 判断是否允许工具栏
        public static bool IsToolbarAllowed { get; set; } = true;

        // 工具栏的配置参数
        public static float FromToolsOffset { get; set; } = 400.0f;
        public static float FromStripOffset { get; set; } = 150.0f;

        // 工具栏的GUI事件
        public static event Action OnToolbarGuiLeft;
        public static event Action OnToolbarGuiRight;

        // 工具栏的样式定义
        private static class Style
        {
            // 行高，间距，顶部和底部的填充
            internal static readonly float RowHeight = 30.0f;
            internal static readonly float Spacing = 15.0f;
            internal static readonly float TopPadding = 5.0f;
            internal static readonly float BotPadding = 3.0f;
        }
    }
}