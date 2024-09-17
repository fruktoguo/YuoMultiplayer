using System.Threading.Tasks;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using YuoTools;

/// <summary>
/// 游戏网络管理器，负责协调Steam大厅和网络连接管理
/// </summary>
public partial class YuoNetworkManager : SingletonMono<YuoNetworkManager>
{
    /// <summary>
    /// 网络传输层组件，用于处理网络通信
    /// </summary>
    [SerializeField] private FacepunchTransport transport;

    /// <summary>
    /// Steam大厅管理器实例
    /// </summary>
    private SteamLobbyModel steamLobby;

    /// <summary>
    /// 网络连接管理器实例
    /// </summary>
    private NetworkConnectionModel networkConnection;

    /// <summary>
    /// 初始化游戏网络管理器
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        // 初始化网络传输层组件
        InitializeTransport();

        // 初始化子管理器：Steam大厅和网络连接
        InitializeManagers();
    }

    /// <summary>
    /// 初始化FacepunchTransport组件
    /// </summary>
    private void InitializeTransport()
    {
        if (transport == null)
        {
            transport = GetComponent<FacepunchTransport>();
            if (transport == null)
            {
                Debug.LogError("FacepunchTransport组件未找到！");
            }
        }
    }

    /// <summary>
    /// 初始化SteamLobbyModel和NetworkConnectionModel管理器
    /// </summary>
    private void InitializeManagers()
    {
        steamLobby = new SteamLobbyModel(this, transport);
        networkConnection = new NetworkConnectionModel(this, transport);
    }

    /// <summary>
    /// 注册Steam大厅事件
    /// </summary>
    private void Start()
    {
        steamLobby.RegisterEvents();
    }

    /// <summary>
    /// 取消注册Steam大厅事件及网络回调
    /// </summary>
    private void OnDestroy()
    {
        steamLobby.UnregisterEvents();
        networkConnection.UnregisterNetworkCallbacks();
    }

    /// <summary>
    /// 应用程序退出时断开所有连接并清理资源
    /// </summary>
    private void OnApplicationQuit()
    {
        Disconnect();
    }

    /// <summary>
    /// 异步启动主机并创建Steam大厅
    /// </summary>
    /// <param name="maxPlayers">大厅的最大玩家数量</param>
    /// <returns>创建的Steam大厅，若失败则返回null</returns>
    public async Task<Lobby?> StartHostAsync(int maxPlayers)
    {
        return await networkConnection.StartHost(maxPlayers);
    }

    /// <summary>
    /// 启动主机，并创建Steam大厅
    /// </summary>
    /// <param name="maxPlayers">大厅的最大玩家数量</param>
    public void StartHost(int maxPlayers)
    {
        networkConnection.StartHost(maxPlayers);
    }

    /// <summary>
    /// 启动客户端并连接到指定的SteamId
    /// </summary>
    /// <param name="targetSteamId">目标主机的SteamId</param>
    public void StartClient(SteamId targetSteamId)
    {
        networkConnection.StartClient(targetSteamId);
    }

    /// <summary>
    /// 查找符合条件的Steam大厅
    /// </summary>
    /// <returns>符合条件的Steam大厅数组</returns>
    public async Task<Lobby[]> FindLobbies()
    {
        return await steamLobby.FindLobbies();
    }

    /// <summary>
    /// 断开所有网络连接并清理资源
    /// </summary>
    public void Disconnect()
    {
        networkConnection.Disconnect();
        steamLobby.LeaveLobby();
    }

    /// <summary>
    /// 获取SteamLobbyModel实例
    /// </summary>
    /// <returns>当前的SteamLobbyModel实例</returns>
    public SteamLobbyModel GetSteamLobbyManager()
    {
        return steamLobby;
    }

    /// <summary>
    /// 获取NetworkConnectionModel实例
    /// </summary>
    /// <returns>当前的NetworkConnectionModel实例</returns>
    public NetworkConnectionModel GetNetworkConnectionManager()
    {
        return networkConnection;
    }
}