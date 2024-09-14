using System.Collections.Generic;
using ET;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_TipWindowComponent
    {
        public List<Button> buttons = new();
        public List<TextMeshProUGUI> texts = new();

        (Button btn, TextMeshProUGUI btnText) CreateButton()
        {
            var go = Object.Instantiate(Button_Option.gameObject, Button_Option.transform.parent);
            var btn = go.GetComponent<Button>();
            var btnText = go.GetComponentInChildren<TextMeshProUGUI>();
            buttons.Add(btn);
            texts.Add(btnText);
            return (btn, btnText);
        }

        public async ETTask<int> ShowMulTip(string tip, params string[] options)
        {
            this.OpenView();
            var task = ETTask<int>.Create();
            TextMeshProUGUI_Content.text = tip;

            bool selected = false;

            for (var i = 0; i < options.Length; i++)
            {
                TextMeshProUGUI btnText;
                Button btn;

                if (i < buttons.Count)
                {
                    btn = buttons[i];
                    btnText = texts[i];
                }
                else
                {
                    (btn, btnText) = CreateButton();
                }

                btnText.text = options[i];
                var index = i;
                btn.SetBtnClick(() =>
                {
                    selected = true;
                    task.SetResult(index);
                    this.CloseView();
                });
            }
            
            for (var i = 0; i < buttons.Count; i++)
            {
                buttons[i].gameObject.SetActive(i < options.Length);
            }

            while (!selected)
            {
                await YuoWait.WaitFrameAsync();
            }

            if (TryGetComponent<UIAnimaComponent>(out var anima))
            {
                await YuoWait.WaitTimeAsync(anima.AnimaDuration);
            }

            return task.GetResult();
        }

        public async ETTask<bool> ShowTip(string tip, string yes = "确定", string no = "取消")
        {
            var result = await ShowMulTip(tip, yes, no);
            return result == 0;
        }

        public async ETTask ShowOneTip(string tip, string yes = "确定")
        {
            await ShowMulTip(tip, yes);
        }
    }

    public class ViewTipWindowCreateSystem : YuoSystem<View_TipWindowComponent>, IUICreate
    {
        public override string Group => "UI/TipWindow";

        protected override void Run(View_TipWindowComponent view)
        {
            view.FindAll();
        }
    }
}