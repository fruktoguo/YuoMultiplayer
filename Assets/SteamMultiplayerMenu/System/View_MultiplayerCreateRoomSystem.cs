using System;
using DG.Tweening;
using Steamworks;
using UnityEngine;
using YuoSteam;
using YuoTools.Extend.Helper;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class View_MultiplayerCreateRoomComponent
    {
        void ShowInfo(string message)
        {
            View_HoverMessageComponent.GetView().ShowMessage(message);
        }

        public async void CreateRoom()
        {
            try
            {
                // GameNetworkManager.Instance.StartHost(4);
                // return;
                // 显示加载中消息
                ShowInfo("正在创建房间...");

                // 获取最大玩家数并进行验证
                var maxPlayerText = TMP_InputField_MaxPlayer.text;
                if (!int.TryParse(maxPlayerText, out int maxPlayer) || maxPlayer < 1 || maxPlayer > 32)
                {
                    ShowInfo("最大玩家数必须在1到32之间。");
                    maxPlayer = 4;
                    TMP_InputField_MaxPlayer.SetTextWithoutNotify(maxPlayer.ToString());
                }

                // 获取房间名称并进行验证
                var roomName = TMP_InputField_RoomName.text.Trim();
                if (string.IsNullOrEmpty(roomName))
                {
                    ShowInfo("房间名称不能为空。");
                    return;
                }

                // 设置是否使用密码
                var usePassword = Toggle_UsePassword.isOn;
                var password = TMP_InputField_Password.text.Trim();
                if (usePassword)
                {
                    if (string.IsNullOrEmpty(password))
                    {
                        ShowInfo("密码不能为空。");
                        return;
                    }
                }

                // 创建房间
                var lobbyQuery = await YuoNetworkManager.Instance.StartHostAsync(maxPlayer);
                if (!lobbyQuery.HasValue)
                {
                    ShowInfo("创建房间失败，请重试。");
                    return;
                }

                var lobby = lobbyQuery.Value;

                // 设置房间名称
                lobby.SetLobbyName(roomName);

                if (usePassword)
                {
                    // 使用 SetLobbyData 设置密码
                    lobby.SetLobbyPassword(password);
                }

                // 设置房间公开或私有
                var publicGame = Toggle_Public.isOn;
                if (publicGame)
                {
                    lobby.SetPublic();
                }
                else
                {
                    lobby.SetPrivate();
                }

                // 显示成功消息
                ShowInfo("房间创建成功！");
                View_HoverMessageComponent.GetView().hoverTime = 99999999;
                View_HoverMessageComponent.GetView().ShowMessage($"房间ID: {lobby.Id}，房间名称: {roomName} 房间拥有者: {lobby.Owner.Id}");
                View_HoverMessageComponent.GetView().hoverTime = 5;

                this.CloseView();
            }
            catch (Exception ex)
            {
                // 记录异常并显示错误消息
                Debug.LogError($"创建房间时发生错误: {ex.Message}");
                ShowInfo("创建房间时发生错误，请重试。");
            }
        }
    }

    public class ViewMultiplayerCreateRoomCreateSystem : YuoSystem<View_MultiplayerCreateRoomComponent>, IUICreate
    {
        public override string Group => "UI/MultiplayerCreateRoom";

        protected override void Run(View_MultiplayerCreateRoomComponent view)
        {
            view.FindAll();
            //关闭窗口的事件注册,名字不同请自行更
            view.Button_Close.SetUIClose(view.ViewName);
            // view.Button_Mask.SetUIClose(view.ViewName);

            view.TMP_InputField_MaxPlayer.onValueChanged.AddListener(x =>
            {
                if (x.ToInt() is { } num)
                {
                    if (!num.InRange(1, 32))
                    {
                        num.Clamp(1, 32);
                        view.TMP_InputField_MaxPlayer.SetTextWithoutNotify(num.ToString());
                    }
                }
            });

            view.Toggle_UsePassword.onValueChanged.AddListener(x =>
            {
                view.TMP_InputField_Password.gameObject.SetActive(x);
            });

            view.Button_Create.SetBtnClick(view.CreateRoom);
        }
    }

    public class ViewMultiplayerCreateRoomOpenSystem : YuoSystem<View_MultiplayerCreateRoomComponent>, IUIOpen
    {
        public override string Group => "UI/MultiplayerCreateRoom";

        protected override async void Run(View_MultiplayerCreateRoomComponent view)
        {
            if (string.IsNullOrEmpty(view.TMP_InputField_RoomName.text))
            {
                // 获取当前Steam用户的名字
                string steamUserName = SteamClient.Name; // 使用SteamFriends获取用户的名字
                var steamID = SteamClient.SteamId;
                view.TMP_InputField_RoomName.SetTextWithoutNotify($"{steamUserName}的游戏");
                var image = await SteamFriends.GetLargeAvatarAsync(steamID);
                if (image.HasValue)
                {
                    view.RawImage_Avatar.texture = image.Value.ToTexture2D();
                }
            }
        }
    }

    public class ViewMultiplayerCreateRoomCloseSystem : YuoSystem<View_MultiplayerCreateRoomComponent>, IUIClose
    {
        public override string Group => "UI/MultiplayerCreateRoom";

        protected override void Run(View_MultiplayerCreateRoomComponent view)
        {
        }
    }

    public class
        ViewMultiplayerCreateRoomOpenAnimaSystem : YuoSystem<View_MultiplayerCreateRoomComponent, UIAnimaComponent>,
        IUIOpen
    {
        public override string Group => "UI/MultiplayerCreateRoom";

        protected override void Run(View_MultiplayerCreateRoomComponent view, UIAnimaComponent anima)
        {
            view.Button_Mask.image.SetColorA(0);

            view.Button_Mask.image.DOFade(0.6f, anima.AnimaDuration);
        }
    }

    public class
        ViewMultiplayerCreateRoomCloseAnimaSystem : YuoSystem<View_MultiplayerCreateRoomComponent, UIAnimaComponent>,
        IUIClose
    {
        public override string Group => "UI/MultiplayerCreateRoom";

        protected override void Run(View_MultiplayerCreateRoomComponent view, UIAnimaComponent anima)
        {
            view.Button_Mask.image.DOFade(0f, anima.AnimaDuration);
        }
    }
}