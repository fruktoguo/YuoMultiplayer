using DG.Tweening;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_MultiplayerMenuComponent
    {
        public async void OpenCreateRoomMenu()
        {
            await this.CloseWaitAnima();
            View_MultiplayerCreateRoomComponent.GetView().OpenView();
        }

        public async void OpenJoinRoomMenu()
        {
            await this.CloseWaitAnima();
            View_MultiplayerJoinRoomComponent.GetView().OpenView();
        }
    }

    public class ViewMultiplayerMenuCreateSystem : YuoSystem<View_MultiplayerMenuComponent>, IUICreate
    {
        public override string Group => "UI/MultiplayerMenu";

        protected override void Run(View_MultiplayerMenuComponent view)
        {
            view.FindAll();
            //关闭窗口的事件注册,名字不同请自行更
            view.Button_Close.SetUIClose(view.ViewName);
            view.Button_Mask.SetUIClose(view.ViewName);

            view.Button_Create.SetBtnClick(view.OpenCreateRoomMenu);
            view.Button_Join.SetBtnClick(view.OpenJoinRoomMenu);
        }
    }

    public class ViewMultiplayerMenuOpenSystem : YuoSystem<View_MultiplayerMenuComponent>, IUIOpen
    {
        public override string Group => "UI/MultiplayerMenu";

        protected override void Run(View_MultiplayerMenuComponent view)
        {
        }
    }

    public class ViewMultiplayerMenuCloseSystem : YuoSystem<View_MultiplayerMenuComponent>, IUIClose
    {
        public override string Group => "UI/MultiplayerMenu";

        protected override void Run(View_MultiplayerMenuComponent view)
        {
        }
    }
}