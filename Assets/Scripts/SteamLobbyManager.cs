using System;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;
using Unity.Netcode;
using YuoTools;

/// <summary>
/// 管理Steam大厅的创建、加入、退出及相关事件
/// </summary>
public class SteamLobbyManager
{
    private readonly GameNetworkManager gameNetworkManager;
    private readonly FacepunchTransport transport;
    private Lobby? currentLobby;

    public SteamLobbyManager(GameNetworkManager manager, FacepunchTransport transport)
    {
        this.gameNetworkManager = manager;
        this.transport = transport;
    }

    #region Event Registration

    public void RegisterEvents()
    {
        // 注册SteamMatchmaking相关的事件
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyDataChanged += OnLobbyDataChanged;
        SteamMatchmaking.OnLobbyMemberDataChanged += OnLobbyMemberDataChanged;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;

        // 注册SteamFriends相关的事件
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
    }

    public void UnregisterEvents()
    {
        // 取消注册SteamMatchmaking相关的事件
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyDataChanged -= OnLobbyDataChanged;
        SteamMatchmaking.OnLobbyMemberDataChanged -= OnLobbyMemberDataChanged;
        SteamMatchmaking.OnLobbyInvite -= OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreated;

        // 取消注册SteamFriends相关的事件
        SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;
    }

    #endregion

    #region Steam Callbacks

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK)
        {
            "创建房间失败".LogError();
            return;
        }

        // 设置大厅为公开并可加入
        lobby.SetPublic();
        lobby.SetJoinable(true);
        // 设置游戏服务器为大厅所有者
        lobby.SetGameServer(lobby.Owner.Id);

        // 更新当前大厅
        currentLobby = lobby;

        "房间创建成功".Log();
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        $"进入房间: {lobby.Id}".Log();

        // 更新当前大厅
        currentLobby = lobby;

        // 如果是主机，不需要再执行后续逻辑
        if (NetworkManager.Singleton.IsHost)
        {
            return;
        }

        // 尝试连接到主机
        if (currentLobby.HasValue)
        {
            gameNetworkManager.StartClient(currentLobby.Value.Owner.Id);
        }
        else
        {
            "当前房间为空".LogError();
        }
    }

    private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        $"{friend.Name} 加入了房间".Log();
    }

    private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        $"{friend.Name} 离开了房间".Log();
    }

    private void OnLobbyDataChanged(Lobby lobby)
    {
        $"房间数据已更改: {lobby.Id}".Log();
    }

    private void OnLobbyMemberDataChanged(Lobby lobby, Friend friend)
    {
        $"房间成员数据已更改: {friend.Name}".Log();
    }

    private async void OnLobbyInvite(Friend friend, Lobby lobby)
    {
        $"{friend.Name} 邀请你加入房间".Log();
        // 可选择自动加入大厅
        try
        {
            var joinResult = await lobby.Join();
            if (joinResult != RoomEnter.Success)
            {
                "加入房间失败".LogError();
            }
            else
            {
                "加入房间成功".Log();
                currentLobby = lobby;
            }
        }
        catch (Exception ex)
        {
            $"加入房间异常: {ex.Message}".LogError();
        }
    }

    private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId steamId)
    {
        $"房间内创建了游戏: {lobby.Id}，由用户: {steamId} 创建".Log();
        // 可在此添加连接到游戏服务器的逻辑
    }

    private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        try
        {
            var joinResult = await lobby.Join();
            if (joinResult != RoomEnter.Success)
            {
                "加入房间失败".LogError();
            }
            else
            {
                "加入房间成功".Log();
                currentLobby = lobby;
            }
        }
        catch (Exception ex)
        {
            $"加入房间异常: {ex.Message}".LogError();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// 创建一个新的Steam大厅
    /// </summary>
    /// <param name="maxPlayers">大厅的最大玩家数量</param>
    public async void CreateLobby(int maxPlayers)
    {
        try
        {
            var lobby = await SteamMatchmaking.CreateLobbyAsync(maxPlayers);
            if (!lobby.HasValue)
            {
                "创建大厅失败".LogError();
                return;
            }

            // 设置自定义数据字段
            lobby.Value.SetData(nameof(LobbyTestFilter), LobbyTestFilter);
            "创建大厅成功并设置过滤器".Log();
        }
        catch (Exception ex)
        {
            $"创建大厅异常: {ex.Message}".LogError();
        }
    }

    /// <summary>
    /// 断开当前大厅
    /// </summary>
    public void LeaveLobby()
    {
        if (currentLobby.HasValue)
        {
            try
            {
                currentLobby.Value.Leave();
                "已离开大厅".Log();
            }
            catch (Exception ex)
            {
                $"离开大厅异常: {ex.Message}".LogError();
            }

            currentLobby = null;
        }
    }

    public const string LobbyTestFilter = "YuoHira";

    async Task<Lobby[]> GetLobbiesFilter()
    {
        var lobbyQuery = SteamMatchmaking.LobbyList;

        //因为是测试id480,所以会有很多其他开发人员的大厅,过滤一下
        lobbyQuery.WithKeyValue(nameof(LobbyTestFilter), LobbyTestFilter);

        return await lobbyQuery.RequestAsync() ?? Array.Empty<Lobby>(); // 如果没有找到大厅，返回空数组
    }

    /// <summary>
    /// 查找符合条件的Steam大厅
    /// </summary>
    public async void FindLobbies()
    {
        try
        {
            var lobbyList = await GetLobbiesFilter();
            if (lobbyList == null || lobbyList.Length == 0)
            {
                "未找到符合条件的大厅".Log();
                return;
            }

            foreach (var lobby in lobbyList)
            {
                // 获取大厅信息
                var lobbyMaxPlayers = lobby.MaxMembers;
                var lobbyHasPassword = lobby.GetData("password") == "true";
                var lobbyInfo = lobby.GetData("info");

                $"找到大厅: {lobby.Id}, 最大人数: {lobbyMaxPlayers}, 是否有密码: {lobbyHasPassword}, 附加信息: {lobbyInfo}".Log();
            }
        }
        catch (Exception ex)
        {
            $"查找大厅异常: {ex.Message}".LogError();
        }
    }

    #endregion
}