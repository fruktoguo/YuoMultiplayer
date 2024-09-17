using DG.Tweening;
using Sirenix.OdinInspector;
using Steamworks.Data;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_RoomItemComponent
    {
        public Lobby? lobby;

        public void Join()
        {
            if (lobby.HasValue && CanJoin())
            {
                ReflexHelper.LogAll(lobby.Value.Owner);
                YuoNetworkManager.Instance.StartClient(lobby.Value.Owner.Id.Log());
            }
        }

        public bool CanJoin()
        {
            return true;
        }

        [Button]
        public void Refresh()
        {
            if (lobby.HasValue && lobby.Value.Owner.Id == 0)
            {
                lobby.Value.Refresh();
                $"房间:{lobby.Value.Id} 所有者:{lobby.Value.Owner.Id}".Log();
            }
        }
    }

    public class ViewRoomItemCreateSystem : YuoSystem<View_RoomItemComponent>, IUICreate
    {
        public override string Group => "UI/RoomItem";

        protected override void Run(View_RoomItemComponent view)
        {
            view.FindAll();
            
            view.Button_Join.SetBtnClick(view.Join);
        }
    }

    public class ViewRoomItemOpenSystem : YuoSystem<View_RoomItemComponent>, IUIOpen
    {
        public override string Group => "UI/RoomItem";

        protected override void Run(View_RoomItemComponent view)
        {
        }
    }

    public class ViewRoomItemCloseSystem : YuoSystem<View_RoomItemComponent>, IUIClose
    {
        public override string Group => "UI/RoomItem";

        protected override void Run(View_RoomItemComponent view)
        {
        }
    }
    
    public class ViewRoomItemUpdateSystem : YuoSystem<View_RoomItemComponent,UIActiveComponent>, IUpdate
    {
        protected override void Run(View_RoomItemComponent view, UIActiveComponent active)
        {
            view.Refresh();
        }
    }
}