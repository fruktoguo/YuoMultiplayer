using System;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;
using Unity.Netcode;
using UnityEngine;
using YuoSteam;
using YuoTools.Extend.Helper;

public partial class YuoNetworkManager
{
    /// <summary>
    /// Steam大厅管理器，负责创建、加入、退出Steam大厅及处理相关事件
    /// </summary>
    public class SteamLobbyModel
    {
        private readonly YuoNetworkManager yuoNetworkManager; // 游戏网络管理器实例
        private readonly FacepunchTransport transport; // 网络传输层实例

        /// <summary>
        /// 当前活动的Steam大厅
        /// </summary>
        public Lobby? CurrentLobby { get; private set; }

        /// <summary>
        /// SteamLobbyModel构造函数，初始化Steam大厅管理器
        /// </summary>
        /// <param name="manager">游戏网络管理器实例</param>
        /// <param name="transport">网络传输层实例</param>
        public SteamLobbyModel(YuoNetworkManager manager, FacepunchTransport transport)
        {
            this.yuoNetworkManager = manager;
            this.transport = transport;
        }

        #region 事件注册与取消

        /// <summary>
        /// 注册Steam Matchmaking和Friends相关的事件
        /// </summary>
        public void RegisterEvents()
        {
            // 注册Steam Matchmaking相关的事件
            SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyDataChanged += OnLobbyDataChanged;
            SteamMatchmaking.OnLobbyMemberDataChanged += OnLobbyMemberDataChanged;
            SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;

            // 注册Steam Friends相关的事件
            SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
        }

        /// <summary>
        /// 取消注册所有Steam Matchmaking和Friends相关的事件
        /// </summary>
        public void UnregisterEvents()
        {
            // 取消注册Steam Matchmaking相关的事件
            SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyDataChanged -= OnLobbyDataChanged;
            SteamMatchmaking.OnLobbyMemberDataChanged -= OnLobbyMemberDataChanged;
            SteamMatchmaking.OnLobbyInvite -= OnLobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreated;

            // 取消注册Steam Friends相关的事件
            SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;
        }

        #endregion

        #region Steam回调处理

        /// <summary>
        /// Steam大厅创建完成的回调处理
        /// </summary>
        /// <param name="result">创建结果</param>
        /// <param name="lobby">创建的大厅对象</param>
        private void OnLobbyCreated(Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                Debug.LogError("创建房间失败");
                return;
            }

            // 设置大厅为公开并可加入
            lobby.SetPublic();
            lobby.SetJoinable(true);
            // 设置游戏服务器为大厅所有者
            lobby.SetGameServer(lobby.Owner.Id);

            // 更新当前大厅
            CurrentLobby = lobby;

            Debug.Log("房间创建成功");
        }

        /// <summary>
        /// 成功进入Steam大厅的回调处理
        /// </summary>
        /// <param name="lobby">进入的大厅对象</param>
        private void OnLobbyEntered(Lobby lobby)
        {
            Debug.Log($"进入房间: {lobby.Id}");

            // 更新当前大厅
            CurrentLobby = lobby;

            // 如果当前是主机，则无需继续处理
            if (NetworkManager.Singleton.IsHost)
            {
                return;
            }

            // 尝试连接到主机
            if (CurrentLobby.HasValue)
            {
                yuoNetworkManager.StartClient(CurrentLobby.Value.Owner.Id);
            }
            else
            {
                Debug.LogError("当前房间为空");
            }
        }

        /// <summary>
        /// 有成员加入Steam大厅的回调处理
        /// </summary>
        /// <param name="lobby">大厅对象</param>
        /// <param name="friend">加入的成员</param>
        private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
        {
            Debug.Log($"{friend.Name} 加入了房间");
        }

        /// <summary>
        /// 有成员离开Steam大厅的回调处理
        /// </summary>
        /// <param name="lobby">大厅对象</param>
        /// <param name="friend">离开的成员</param>
        private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
        {
            Debug.Log($"{friend.Name} 离开了房间");
        }

        /// <summary>
        /// Steam大厅数据变化的回调处理
        /// </summary>
        /// <param name="lobby">大厅对象</param>
        private void OnLobbyDataChanged(Lobby lobby)
        {
            Debug.Log($"房间数据已更改: {lobby.Id}");
        }

        /// <summary>
        /// Steam大厅成员数据变化的回调处理
        /// </summary>
        /// <param name="lobby">大厅对象</param>
        /// <param name="friend">成员</param>
        private void OnLobbyMemberDataChanged(Lobby lobby, Friend friend)
        {
            Debug.Log($"房间成员数据已更改: {friend.Name}");
        }

        /// <summary>
        /// 收到Steam大厅邀请的回调处理
        /// </summary>
        /// <param name="friend">发起邀请的好友</param>
        /// <param name="lobby">被邀请的大厅</param>
        private async void OnLobbyInvite(Friend friend, Lobby lobby)
        {
            Debug.Log($"{friend.Name} 邀请你加入房间");
            // 可选择自动加入大厅
            try
            {
                var joinResult = await lobby.Join();
                if (joinResult != RoomEnter.Success)
                {
                    Debug.LogError("加入房间失败");
                }
                else
                {
                    Debug.Log("加入房间成功");
                    CurrentLobby = lobby;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"加入房间异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 在Steam大厅内创建游戏的回调处理
        /// </summary>
        /// <param name="lobby">大厅对象</param>
        /// <param name="ip">游戏服务器IP</param>
        /// <param name="port">游戏服务器端口</param>
        /// <param name="steamId">创建游戏的用户SteamId</param>
        private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId steamId)
        {
            Debug.Log($"房间内创建了游戏: {lobby.Id}，由用户: {steamId} 创建");
            // 可在此添加连接到游戏服务器的逻辑
        }

        /// <summary>
        /// 处理Steam Friends发起的大厅加入请求
        /// </summary>
        /// <param name="lobby">要加入的大厅</param>
        /// <param name="steamId">发起邀请的用户SteamId</param>
        private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
        {
            try
            {
                var joinResult = await lobby.Join();
                if (joinResult != RoomEnter.Success)
                {
                    Debug.LogError("加入房间失败");
                }
                else
                {
                    Debug.Log("加入房间成功");
                    CurrentLobby = lobby;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"加入房间异常: {ex.Message}");
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 创建一个新的Steam大厅
        /// </summary>
        /// <param name="maxPlayers">大厅的最大玩家数量</param>
        /// <returns>创建的Steam大厅，若失败则返回null</returns>
        public async Task<Lobby?> CreateLobby(int maxPlayers)
        {
            try
            {
                var lobby = await SteamMatchmakingHelper.CreateLobbyAsync(maxPlayers);
                if (lobby.HasValue)
                {
                    // 设置大厅的过滤器数据
                    lobby.Value.SetData(nameof(LobbyTestFilter), LobbyTestFilter);
                    return lobby;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"创建大厅异常: {ex.Message}");
            }

            Debug.LogError("创建大厅失败");
            return null;
        }

        /// <summary>
        /// 断开当前Steam大厅
        /// </summary>
        public void LeaveLobby()
        {
            if (CurrentLobby.HasValue)
            {
                try
                {
                    CurrentLobby.Value.Leave();
                    Debug.Log("已离开大厅");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"离开大厅异常: {ex.Message}");
                }

                CurrentLobby = null;
            }
        }

        /// <summary>
        /// Steam大厅的测试过滤器
        /// </summary>
        public const string LobbyTestFilter = "YuoHira";

        /// <summary>
        /// 根据过滤器获取Steam大厅列表
        /// </summary>
        /// <returns>符合条件的Steam大厅数组，如果没有则返回空数组</returns>
        private async Task<Lobby[]> GetLobbiesFilter()
        {
            var lobbyQuery = SteamMatchmakingHelper.LobbyList;

            // 因为是测试ID480，所以会有很多其他开发人员的大厅，进行过滤
            lobbyQuery.WithKeyValue(nameof(LobbyTestFilter), LobbyTestFilter);

            return await lobbyQuery.RequestAsync() ?? Array.Empty<Lobby>(); // 如果没有找到大厅，返回空数组
        }

        /// <summary>
        /// 查找符合条件的Steam大厅
        /// </summary>
        /// <returns>符合条件的Steam大厅数组，若查找失败则返回null</returns>
        public async Task<Lobby[]> FindLobbies()
        {
            try
            {
                var lobbyList = await GetLobbiesFilter();
                if (lobbyList == null || lobbyList.Length == 0)
                {
                    Debug.Log("未找到符合条件的大厅");
                    return null;
                }

                foreach (var lobby in lobbyList)
                {
                    // 获取大厅信息
                    var lobbyMaxPlayers = lobby.MaxMembers;
                    var lobbyHasPassword = lobby.GetData("password") == "true";
                    var lobbyInfo = lobby.GetData("info");

                    lobby.Refresh();
                    ReflexHelper.LogAll(lobby.Owner);

                    Debug.Log($"找到大厅: {lobby.Id}, 拥有者: {lobby.Owner.Id}, 最大人数: {lobbyMaxPlayers}, 是否有密码: {lobbyHasPassword}, 附加信息: {lobbyInfo}");
                }

                return lobbyList;
            }
            catch (Exception ex)
            {
                Debug.LogError($"查找大厅异常: {ex.Message}");
            }

            return null;
        }

        #endregion
    }
}