using System.Linq;
using ET;
using UnityEngine;
using YuoTools.Extend;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    using System.Collections.Generic;

    public partial class UIManagerComponent
    {
        public async ETTask<T> OpenAsync<T>(T component) where T : UIComponent
        {
            // $"Open View {component}".Log();

            if (component == null) return null;

            if (!component.ModuleUI) TopView = component;

            //不能重复打开
            if (!openItems.Contains(component))
            {
                component.IsOpen = true;

                openItems.Add(component);

                component.AddComponent<UIActiveComponent>();

                RunSystemAndChild<IUIOpen>(component);

                if (component.TryGetComponent<UIAnimaComponent>(out var anima))
                {
                    await anima.Open();
                    anima.RunSystem<IUIOpenAnima>();
                }
            }

            return component;
        }

        public async void Open<T>(T component) where T : UIComponent
        {
            await OpenAsync(component);
        }

        public async ETTask CloseAsync<T>(T component) where T : UIComponent
        {
            if (component == null)
            {
                $"找不到窗口{typeof(T).FullName}".LogError();
                return;
            }

            if (component.rectTransform.gameObject.activeSelf) RunSystemAndChild<IUIClose>(component);

            if (component.ModuleUI && moduleUiItems.Contains(component)) moduleUiItems.Remove(component);

            if (openItems.Contains(component))
            {
                openItems.Remove(component);
            }

            component.IsOpen = false;

            component.Entity.RemoveComponent<UIActiveComponent>();

            if (!component.ModuleUI)
            {
                TopView = openItems.LastOrDefault(item => !item.ModuleUI);
            }

            if (component.TryGetComponent<UIAnimaComponent>(out var anima))
            {
                await anima.Close();
                YuoWorld.RunSystem<IUICloseAnima>(anima);
            }

            if (component.AutoHide) component.rectTransform.gameObject.Hide();
        }

        public async void Close<T>(T component) where T : UIComponent
        {
            await CloseAsync(component);
        }

        #region Open

        public async ETTask<UIComponent> OpenAsync(string winName, GameObject go = null)
        {
            UIComponent component = GetUIView(winName) ?? await AddWindow(winName, go);

            Open(component);

            return component;
        }

        public async void Open(string winName, GameObject go = null) => await OpenAsync(winName, go);

        public async void Open<T>() where T : UIComponent => await OpenAsync<T>();

        public async ETTask<T> OpenAsync<T>() where T : UIComponent
        {
            if (uiItemsType.ContainsKey(typeof(T)))
                return await OpenAsync(uiItemsType[typeof(T)].ViewName) as T;
            else
            {
                Debug.LogError($"UIManagerComponent Open error,not find {typeof(T)}");
                return null;
            }
        }

        #endregion

        #region Close

        public void Close<T>() where T : UIComponent
        {
            if (uiItemsType.ContainsKey(typeof(T)))
                Close(uiItemsType[typeof(T)].ViewName);
            else
            {
                Debug.LogError($"UIManagerComponent Close<T> error,not find {typeof(T)}");
            }
        }

        public void Close(string winName)
        {
            UIComponent component = GetUIView(winName);
            Close(component);
        }

        public void CloseAll(bool closeModule = false)
        {
            var openItemsCopy = openItems.ToArray();
            foreach (var item in openItemsCopy)
            {
                if (!closeModule && item.ModuleUI) continue;
                Close(item.ViewName);
            }
        }

        #endregion

        public static void ResetWindowLayer(UIComponent component)
        {
            if (component.ModuleUI)
            {
                if (!Get.moduleUiItems.Contains(component))
                {
                    Get.moduleUiItems.Add(component);
                    component.AddComponent<TopViewComponent>();
                }

                component.rectTransform.SetAsLastSibling();
            }
            else
            {
                component.SetWindowLayer(Get.Transform.childCount - 1 - Get.moduleUiItems.Count);
            }
        }

        public async ETTask<UIComponent> AddWindow(string winName, GameObject go = null)
        {
            var type = YuoWorld.Instance.GetComponentType($"View_{winName}Component");
            if (type == null)
            {
                Debug.LogError($"找不到对应类型---View_{winName}Component");
                return null;
            }

            //生成窗口
            var component =
                YuoWorld.Main.GetComponent<UIManagerComponent>().Entity
                    .AddChild(type, IDGenerate.GetID(winName)) as UIComponent;
            if (component == null) return null;

            component.AddComponent<UIAutoExitComponent>();

            if (go == null) go = await Create(winName);

            //初始化窗口
            component.rectTransform = go.transform as RectTransform;

            //如果有动画组件就挂载动画组件
            if (go.TryGetComponent<UISetting>(out var uiSetting))
            {
                if (uiSetting.HasAnima())
                    component.Entity.AddComponent<UIAnimaComponent>().From(uiSetting);
                component.ModuleUI = uiSetting.ModuleUI;
                go.SetActive(uiSetting.Active);
                Object.Destroy(uiSetting);
            }

            component.ViewName = winName;

            uiItems.Add(winName, component);
            uiItemsType.Add(type, component);

            if (BaseIndex == -1) BaseIndex = go.transform.GetSiblingIndex();

            component.Entity.EntityName = "View_" + component.ViewName;

            //调用这个窗口的初始化系统
            YuoWorld.RunSystem<IUICreate>(component.Entity);

            //模块UI永远在最上面
            if (component.ModuleUI)
            {
                component.Entity.AddComponent<TopViewComponent>();
                moduleUiItems.Add(component);
            }

            return component;
        }

        public void Remove(string winName)
        {
            var win = GetUIView(winName);
            if (win != null)
            {
                uiItems.Remove(winName);
                uiItemsType.Remove(win.Type);
                win.Entity.Dispose();
            }
        }

        public bool IsOpen(string winName)
        {
            return openItems.Contains(GetUIView(winName));
        }

        public UIComponent GetUIView(string winName)
        {
            return uiItems.GetValueOrDefault(winName);
        }

        public T GetUIView<T>() where T : UIComponent
        {
            if (uiItemsType.ContainsKey(typeof(T)))
                return uiItemsType[typeof(T)] as T;
            else
            {
                // Debug.LogError($"没有找到对应的窗口---{typeof(T)}");
                return null;
            }
        }

        public async ETTask<UIComponent> GetOrAddView(string winName)
        {
            return uiItems.ContainsKey(winName) ? uiItems[winName] : await AddWindow(winName);
        }

        private async ETTask<GameObject> Create(string winName)
        {
            return (await YuoWorld.Main.GetBaseComponent<AssetsLoadComponent>().LoadPrefabAsync(LoadPath + winName))
                .Instantiate(Transform);
        }

        private UIComponent _topView;

        public UIComponent TopView
        {
            get => _topView;
            private set
            {
                if (_topView != value)
                {
                    if (_topView != null)
                    {
                        _topView.GetComponent<TopViewComponent>().DestroyComponent();
                        // $"{_topView.Entity}不在顶部了".Log();
                    }

                    if (value != null)
                    {
                        value.Entity.AddComponent<TopViewComponent>();
                        // $"{value.Entity}在顶部了".Log();

                        RunSystemAndChild<IUIActive>(value);
                    }
                    else
                    {
                        // "没有窗口在顶部".Log();
                    }

                    _topView = value;
                }
            }
        }

        public int WindowCount => uiItems.Count + BaseIndex;

        public static Vector2 CanvasSize;

        public void RunSystemAndChild<T>(UIComponent component) where T : ISystemTag
        {
            component.Entity.RunSystem<T>();
            YuoWorld.RunSystem<T>(component.Entity.GetAllChildren());
        }

        public List<UIComponent> GetOpenItems() => openItems;
    }

    public partial class UIComponent
    {
        public void SetWindowLayer(int windowLayer)
        {
            rectTransform.SetSiblingIndex(windowLayer);
        }

        public T AddChildAndInstantiate<T>(T template) where T : UIComponent, new() =>
            AddChildAndInstantiate(template, Entity);

        public T AddChildAndInstantiate<T>(T template, YuoEntity parent) where T : UIComponent, new()
        {
            var go = Object.Instantiate(template.rectTransform, template.rectTransform.parent);
            var child = parent.AddChild<T>();
            child.rectTransform = go.transform as RectTransform;
            child.RunSystem<IUICreate>();
            go.Show();
            return child;
        }
    }

    public static class YuoUIExtensions
    {
        public static Vector2 GetLossyScale(this RectTransform rectTransform)
        {
            var canvasScale = UIManagerComponent.Get?.Canvas.transform.lossyScale ?? Vector3.one;
            var lossyScale = rectTransform.lossyScale;
            return new Vector2(lossyScale.x / canvasScale.x, lossyScale.y / canvasScale.y);
        }
    }

    public class UIManagerAwakeSystem : YuoSystem<UIManagerComponent>, IAwake
    {
        public override string Group => SystemGroupConst.MainUI;

        protected override void Run(UIManagerComponent component)
        {
            if (component.Transform == null)
            {
                component.DestroyComponent();
                return;
            }
            
            //初始化所有挂载在根节点的UI
            var uiSettings = component.Transform.GetComponentsInChildren<UISetting>(true);
            foreach (var uiSetting in uiSettings)
            {
                uiSetting.Init();
            }
            
            //初始化自动加载的UI
            var uiAutoLoadConfig = Resources.Load<UIAutoLoadConfig>("UIAutoLoadConfig");
            if (uiAutoLoadConfig != null)
            {
                foreach (var uiWindow in uiAutoLoadConfig.AutoLoadList)
                {
                    var uiObject = uiWindow.Instantiate(component.Transform);
                    uiObject.name = uiWindow.name;
                    var uiSetting = uiObject.GetComponent<UISetting>();
                    if (uiSetting != null)
                    {
                        uiSetting.Init();
                    }
                }
            }
        }
    }

    public class UIItemOpenSystem : YuoSystemOfBase<UIComponent, UIActiveComponent>, IUIOpen
    {
        public override string Group => SystemGroupConst.MainUI;

        protected override void Run(UIComponent component, UIActiveComponent _)
        {
            // component.SetWindowLayer(YuoWorld.Main.GetComponent<UIManagerComponent>().WindowCount);
            UIManagerComponent.ResetWindowLayer(component);
            // $"{component.ViewName} open {component.AutoShow}".Log();
            if (component.AutoShow) component.rectTransform.Show();
        }
    }

    public class UIItemSelectSystem : YuoSystemOfBase<UIComponent>, IStart, IUICreate
    {
        public override string Group => SystemGroupConst.MainUI;

        protected override void Run(UIComponent baseComponent)
        {
            if (baseComponent.rectTransform != null)
            {
                baseComponent.AddComponent<EntitySelectComponent>().SelectGameObject =
                    baseComponent.rectTransform.gameObject;
            }
            else
            {
                Debug.LogError($"UIItemSelectSystem error,not find rectTransform {baseComponent.ViewName}");
            }
        }
    }

    public class UICanvasSizeResetSystem : YuoSystem<UIManagerComponent>, IAwake, IUIAdaption
    {
        public override string Group => SystemGroupConst.MainUI;

        protected override void Run(UIManagerComponent component)
        {
            // var canvas = component.Transform.GetComponent<CanvasScaler>();
            // ReflexHelper.LogAll(canvas);
        }
    }

    public class UIAutoExitComponent : YuoComponent
    {
        public override string Name => "场景切换时自动销毁";
    }

    /// <summary>
    /// 切换场景时清理UI
    /// </summary>
    public class UIAutoExitSystem : YuoSystem<UIAutoExitComponent>, ISceneExit
    {
        public override string Group => SystemGroupConst.MainUI;

        protected override void Run(UIAutoExitComponent component)
        {
            component.Entity.Destroy();
        }
    }

    public class UIDestroySystem : YuoSystemOfBase<UIComponent>, IDestroy
    {
        public override string Group => SystemGroupConst.MainUI;

        protected override void Run(UIComponent baseComponent)
        {
            if (baseComponent.rectTransform != null)
            {
                Object.Destroy(baseComponent.rectTransform.gameObject);
            }
        }
    }

    public class UIUpdateSystem : YuoSystem<UIManagerComponent>, IUpdate
    {
        public override string Group => SystemGroupConst.MainUI;

        protected override void Run(UIManagerComponent manager)
        {
            foreach (var uiComponent in manager.GetOpenItems())
            {
                manager.RunSystemAndChild<IUIUpdate>(uiComponent);
            }
        }
    }
}