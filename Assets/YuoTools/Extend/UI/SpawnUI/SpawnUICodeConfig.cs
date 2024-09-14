using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YuoTools.UI
{
    public partial class SpawnUICodeConfig
    {
        public const string UIComponentTag = "C_";
        public const string GeneralUITag = "G_";
        public const string ChildUITag = "D_";
        public const string VariantChildUITag = "DV_";
        public const string VariantChildComponentTag = "CV_";

        public static List<string> AllTag = new List<string>()
        {
            UIComponentTag,
            GeneralUITag,
            ChildUITag,
            VariantChildUITag,
            VariantChildComponentTag,
        };

        [LabelText("会被检索到的组件类型")] public List<Type> SpawnType = new()
        {
            typeof(Button),
            typeof(Image),
            typeof(RawImage),
            typeof(Text),
            typeof(TextMeshProUGUI),
            typeof(TMP_InputField),
            typeof(TMP_Dropdown),
            typeof(Toggle),
            typeof(ToggleGroup),
            typeof(Dropdown),
            typeof(InputField),
            typeof(Slider),
            typeof(ScrollRect),
            typeof(EventTrigger),
            typeof(Scrollbar),
            typeof(LayoutGroup),
            typeof(ContentSizeFitter),
            typeof(ParticleSystem),
            typeof(CanvasGroup),
            typeof(Canvas),
        };

        [LabelText("相斥组件")] public Dictionary<Type, Type> RemoveType = new()
        {
            { typeof(Button), typeof(Image) },
            { typeof(Dropdown), typeof(Image) },
            { typeof(InputField), typeof(Image) },
            { typeof(Toggle), typeof(Image) },
            { typeof(ToggleGroup), typeof(Image) },
            { typeof(Slider), typeof(Image) },
            { typeof(ScrollRect), typeof(Image) },
            { typeof(Scrollbar), typeof(Image) },
            { typeof(ParticleSystem), typeof(Image) },
            { typeof(TMP_InputField), typeof(Image) },
            { typeof(TMP_Dropdown), typeof(Image) }
        };

        [LabelText("组件的默认命名空间")] public List<string> ComponentNameSpace = new()
        {
            "using UnityEngine;",
            "using System.Collections;",
            "using System.Collections.Generic;",
            "using UnityEngine.UI;",
            "using YuoTools.Main.Ecs;",
            "using Sirenix.OdinInspector;",
        };

        [LabelText("需要额外添加命名空间的组件")] public Dictionary<Type, string> ComponentAddNameSpace = new()
        {
            { typeof(TextMeshProUGUI), "using TMPro;" },
            { typeof(TMP_InputField), "using TMPro;" },
            { typeof(TMP_Dropdown), "using TMPro;" },
            { typeof(EventTrigger), "using UnityEngine.EventSystems;" },
        };

        [LabelText("System的默认命名空间")] public List<string> SystemNameSpace = new()
        {
            "using DG.Tweening;",
            "using YuoTools.Extend.Helper;",
            "using YuoTools.Main.Ecs;",
        };
    }
}