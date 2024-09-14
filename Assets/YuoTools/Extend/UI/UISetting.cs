using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using YuoTools.Extend.Helper;
#endif

namespace YuoTools.UI
{
    using DG.Tweening;

    public class UISetting : MonoBehaviour
    {
        [HideInInspector] [HorizontalGroup] [Header("默认显示状态")]
        public bool Active = true;

        [HorizontalGroup("2")] [LabelText("模块UI")]
        public bool ModuleUI;

        [ShowIf(nameof(HasAnimator))] public float AnimatorLength = 0.5f;

        bool HasAnimator => UIAnimator != null;

        private Animator animator;

        public Animator UIAnimator
        {
            get
            {
                if (!animator) animator = GetComponent<Animator>();
                return animator;
            }
        }

#if UNITY_EDITOR

        [HorizontalGroup("2")]
        [Button("生成UI代码")]
        public void SpawnCode()
        {
            if (gameObject.transform is not RectTransform)
            {
                Debug.LogError("目标窗口没有RectTransform");
                return;
            }
            SpawnUICode.BasePath = useCustomDirectory ? customDirectory : SpawnUICode.DefaultBasePath;
            SpawnUICode.SpawnCode(gameObject);
            if (useCustomDirectory && customDirectory == "YuoUITemplate")
            {
                FileHelper.CheckOrCreateDirectoryPath("YuoUITemplate/Resources/Prefabs/UI");
            }
        }

        [HorizontalGroup("2")]
        [Button("打开Component脚本")]
        public void SelectSystemScript()
        {
            var fileName = $"View_{gameObject.name}Component";
            var result = AssetDatabase.FindAssets(fileName);
            if (result.Length > 0)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(result[0]));
                AssetDatabase.OpenAsset(asset);
            }
            else
            {
                Debug.LogError($"{fileName} 没有找到");
            }
        }

        [HorizontalGroup("2")]
        [Button("打开System脚本")]
        public void OpenSystemScript()
        {
            var fileName = $"View_{gameObject.name}System";
            var result = AssetDatabase.FindAssets(fileName);
            if (result.Length > 0)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(result[0]));
                AssetDatabase.OpenAsset(asset);
            }
            else
            {
                Debug.LogError($"{fileName} 没有找到");
            }
        }

        [SerializeField] [HorizontalGroup("customDirectory")] [LabelText("自定义生成目录")] [LabelWidth(200)]
        bool useCustomDirectory;

        [ShowIf("useCustomDirectory")] [HorizontalGroup("customDirectory")] [LabelText("自定义目录")] [SerializeField]
        string customDirectory = "YuoUITemplate";

        [ShowInInspector] public bool OpenTools { get; set; }


        [ShowIf("OpenTools")] [FoldoutGroup("Raycast")]
        public List<MaskableGraphic> maskableGraphics = new();

        [ShowIf("OpenTools")]
        [FoldoutGroup("Raycast")]
        [Button(ButtonHeight = 30, Name = "获取所有开启了Raycast的物体")]
        public void FindAllRaycast()
        {
            maskableGraphics.Clear();
            foreach (var item in transform.GetComponentsInChildren<MaskableGraphic>())
            {
                if (item.raycastTarget)
                {
                    maskableGraphics.Add(item);
                }
            }
        }

        [ShowIf("OpenTools")]
        [FoldoutGroup("Raycast")]
        [Button(ButtonHeight = 30, Name = "清除剩余Raycast")]
        public void CloseRaycast()
        {
            foreach (var item in maskableGraphics)
            {
                item.raycastTarget = false;
            }
        }

#endif

        public async void Init()
        {
            if (!enabled) return;
            var o = gameObject;
            Active = o.activeSelf;
            if (Active)
                await UIManagerComponent.Get.OpenAsync(o.name, o);
            else
            {
                var uiComponent = await UIManagerComponent.Get.AddWindow(o.name, gameObject);
                if (uiComponent == null) $"创建View失败_{o.name}".LogError();
            }
        }
        
        public bool HasAnima()
        {
            return gameObject.TryGetComponent<DOTweenAnimation>(out var _) ||
                   gameObject.TryGetComponent<Animator>(out _);
        }

        public enum UISate
        {
            Hide = 0,
            Show = 1,
            ShowAnima = 2,
            HideAnima = 3,
        }
    }
}