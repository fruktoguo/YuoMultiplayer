using DG.Tweening;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_LobbyComponent
    {
    }

    public class ViewLobbyCreateSystem : YuoSystem<View_LobbyComponent>, IUICreate
    {
        public override string Group => "UI/Lobby";

        protected override void Run(View_LobbyComponent view)
        {
            view.FindAll();
            //关闭窗口的事件注册,名字不同请自行更
            view.Button_Close.SetUIClose(view.ViewName);
            view.Button_Mask.SetUIClose(view.ViewName);
        }
    }

    public class ViewLobbyOpenSystem : YuoSystem<View_LobbyComponent>, IUIOpen
    {
        public override string Group => "UI/Lobby";

        protected override void Run(View_LobbyComponent view)
        {
        }
    }

    public class ViewLobbyCloseSystem : YuoSystem<View_LobbyComponent>, IUIClose
    {
        public override string Group => "UI/Lobby";

        protected override void Run(View_LobbyComponent view)
        {
        }
    }

    public class ViewLobbyOpenAnimaSystem : YuoSystem<View_LobbyComponent, UIAnimaComponent>, IUIOpen
    {
        public override string Group => "UI/Lobby";

        protected override void Run(View_LobbyComponent view, UIAnimaComponent anima)
        {
            view.Button_Mask.image.SetColorA(0);

            view.Button_Mask.image.DOFade(0.6f, anima.AnimaDuration);
        }
    }

    public class ViewLobbyCloseAnimaSystem : YuoSystem<View_LobbyComponent, UIAnimaComponent>, IUIClose
    {
        public override string Group => "UI/Lobby";

        protected override void Run(View_LobbyComponent view, UIAnimaComponent anima)
        {
            view.Button_Mask.image.DOFade(0f, anima.AnimaDuration);
        }
    }
}