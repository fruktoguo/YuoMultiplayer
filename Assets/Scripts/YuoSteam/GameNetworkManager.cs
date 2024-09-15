using System.Threading.Tasks;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using YuoTools;

/// <summary>
/// 游戏网络管理器，负责协调Steam大厅和网络连接管理
/// </summary>
public class GameNetworkManager : SingletonMono<GameNetworkManager>
{
    [SerializeField] private FacepunchTransport transport;

    private SteamLobbyManager steamLobbyManager;
    private NetworkConnectionManager networkConnectionManager;

    public override void Awake()
    {
        base.Awake();
        // 初始化组件
        if (transport == null)
        {
            transport = GetComponent<FacepunchTransport>();
            if (transport == null)
            {
                "FacepunchTransport组件未找到！".LogError();
            }
        }

        // 初始化子管理器
        steamLobbyManager = new SteamLobbyManager(this, transport);
        networkConnectionManager = new NetworkConnectionManager(this, transport);
    }

    private void Start()
    {
        // 注册Steam大厅事件
        steamLobbyManager.RegisterEvents();
    }

    private void OnDestroy()
    {
        // 取消注册Steam大厅事件
        steamLobbyManager.UnregisterEvents();
        networkConnectionManager.UnregisterNetworkCallbacks();
    }

    private void OnApplicationQuit()
    {
        // 断开连接并清理资源
        Disconnect();
    }

    /// <summary>
    /// 开始主机并创建大厅
    /// </summary>
    public Task<Lobby?> StartHostAsync(int maxPlayers)
    {
        return networkConnectionManager.StartHost(maxPlayers);
    }

    public async void StartHost(int maxPlayers)
    {
        await networkConnectionManager.StartHost(maxPlayers);
    }

    /// <summary>
    /// 开始客户端并连接到指定的SteamId
    /// </summary>
    /// <param name="targetSteamId">目标主机的SteamId</param>
    public void StartClient(SteamId targetSteamId)
    {
        networkConnectionManager.StartClient(targetSteamId);
    }

    public Task<Lobby[]> FindLobbies()
    {
        return steamLobbyManager.FindLobbies();
    }

    /// <summary>
    /// 断开连接并清理资源
    /// </summary>
    public void Disconnect()
    {
        networkConnectionManager.Disconnect();
        steamLobbyManager.LeaveLobby();
    }

    /// <summary>
    /// 获取SteamLobbyManager
    /// </summary>
    public SteamLobbyManager GetSteamLobbyManager()
    {
        return steamLobbyManager;
    }

    public NetworkConnectionManager GetNetworkConnectionManager()
    {
        return networkConnectionManager;
    }
}