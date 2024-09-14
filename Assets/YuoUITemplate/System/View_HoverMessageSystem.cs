using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_HoverMessageComponent
    {
        public ObjectPool<View_HoverMessageItemComponent> messagePool;
        public float hoverTime = 5;

        public async void ShowMessage(string message)
        {
            var messageItem = messagePool.Get();
            messageItem.TextMeshProUGUI_Text.text = message;
            messageItem.rectTransform.ForceRebuildLayout();
            messageItem.rectTransform.SetAsLastSibling();
            messageItem.MainCanvasGroup.alpha = 1;
            //获取当前划动列表的百分比
            var percent = ScrollRect_MessagePanel.verticalNormalizedPosition;

            await YuoWait.WaitTimeAsync(hoverTime);
            await messageItem.CloseWaitAnima();
            messagePool.Release(messageItem);
        }
    }

    public class ViewHoverMessageCreateSystem : YuoSystem<View_HoverMessageComponent>, IUICreate
    {
        public override string Group => "UI/HoverMessage";

        protected override void Run(View_HoverMessageComponent view)
        {
            view.FindAll();
            view.messagePool =
                new ObjectPool<View_HoverMessageItemComponent>(CreateFunc, actionOnRelease: ActionOnRelease,actionOnGet: ActionOnGet);

            View_HoverMessageItemComponent CreateFunc() => view.AddChildAndInstantiate(view.Child_HoverMessageItem);

            void ActionOnRelease(View_HoverMessageItemComponent messageItem)
            {
                messageItem.rectTransform.Hide();
            }
            
            void ActionOnGet(View_HoverMessageItemComponent messageItem)
            {
                messageItem.rectTransform.Show();
                messageItem.RunSystem<IUIOpen>();
            }
        }
    }

    public class ViewHoverMessageOpenSystem : YuoSystem<View_HoverMessageComponent>, IUpdate
    {
        private int a = 1;

        protected override void Run(View_HoverMessageComponent view)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                view.ShowMessage(a++.ToString());
            }
        }
    }
}