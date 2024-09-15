using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Steamworks.Data;
using Netcode.Transports.Facepunch;
using Steamworks;
using YuoTools;

/// <summary>
/// 管理网络连接，包括启动主机、客户端及处理网络回调
/// 该类负责处理与网络相关的所有操作，包括主机和客户端的启动、连接和断开连接。
/// </summary>
public class NetworkConnectionManager
{
    private readonly GameNetworkManager gameNetworkManager; // 游戏网络管理器实例
    private readonly FacepunchTransport transport; // 网络传输层实例

    /// <summary>
    /// 构造函数，初始化网络连接管理器
    /// </summary>
    /// <param name="manager">游戏网络管理器</param>
    /// <param name="transport">网络传输层</param>
    public NetworkConnectionManager(GameNetworkManager manager, FacepunchTransport transport)
    {
        this.gameNetworkManager = manager ?? throw new ArgumentNullException(nameof(manager), "游戏网络管理器不能为空");
        this.transport = transport ?? throw new ArgumentNullException(nameof(transport), "网络传输层不能为空");
    }

    /// <summary>
    /// 注册网络相关的回调
    /// </summary>
    /// <param name="isHost">是否为主机</param>
    public void RegisterNetworkCallbacks(bool isHost)
    {
        if (NetworkManager.Singleton == null)
        {
            "错误：NetworkManager.Singleton 未初始化！".LogError();
            return;
        }

        if (isHost)
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted; // 注册服务器启动回调
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected; // 注册客户端连接回调
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect; // 注册客户端断开连接回调
        }
    }

    /// <summary>
    /// 取消注册网络相关的回调
    /// </summary>
    public void UnregisterNetworkCallbacks()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.OnServerStarted -= OnServerStarted; // 取消服务器启动回调
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected; // 取消客户端连接回调
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect; // 取消客户端断开连接回调
    }

    /// <summary>
    /// 启动主机并创建Steam大厅
    /// </summary>
    /// <param name="maxPlayers">大厅的最大玩家数量</param>
    public async Task<Lobby?> StartHost(int maxPlayers)
    {
        if (NetworkManager.Singleton == null)
        {
            "错误：NetworkManager.Singleton 未初始化！".LogError();
            return null;
        }

        RegisterNetworkCallbacks(isHost: true); // 注册主机相关的回调

        // 启动主机模式
        if (NetworkManager.Singleton.StartHost())
        {
            "主机已成功启动".Log();
            try
            {
                // 创建Steam大厅，设置最大玩家数量
                var steamLobbyManager = gameNetworkManager.GetSteamLobbyManager();
                if (steamLobbyManager != null)
                {
                    var lobby = await steamLobbyManager.CreateLobby(maxPlayers)!; // 创建大厅
                    if (lobby.HasValue)
                    {
                        $"Steam大厅已创建，最大玩家数量: {maxPlayers}".Log();
                    }
                    else
                    {
                        "错误：无法创建Steam大厅".LogError();
                    }

                    return lobby;
                }
                else
                {
                    "错误：SteamLobbyManager 未找到！".LogError();
                }
            }
            catch (Exception ex)
            {
                $"创建大厅异常: {ex.Message}".LogError(); // 记录异常信息
            }
        }
        else
        {
            "错误：主机启动失败".LogError();
        }

        return null;
    }


    /// <summary>
    /// 启动客户端并连接到指定的SteamId
    /// </summary>
    /// <param name="targetSteamId">目标主机的SteamId</param>
    public void StartClient(SteamId targetSteamId)
    {
        if (NetworkManager.Singleton == null)
        {
            "错误：NetworkManager.Singleton 未初始化！".LogError();
            return;
        }

        RegisterNetworkCallbacks(isHost: false); // 注册客户端相关的回调

        // 设置传输层的目标SteamId
        if (transport != null)
        {
            transport.targetSteamId = targetSteamId; // 设置目标SteamId
            $"目标SteamId已设置: {targetSteamId}".Log();
        }
        else
        {
            "错误：FacepunchTransport 未初始化！".LogError();
            return;
        }

        // 启动客户端模式
        if (NetworkManager.Singleton.StartClient())
        {
            "客户端已成功启动".Log();
        }
        else
        {
            "错误：客户端启动失败".LogError();
        }
    }

    /// <summary>
    /// 断开连接并清理资源
    /// </summary>
    public void Disconnect()
    {
        // 如果NetworkManager存在，进行清理
        if (NetworkManager.Singleton != null)
        {
            UnregisterNetworkCallbacks(); // 取消注册回调

            // 关闭网络管理器，强制关闭所有连接
            NetworkManager.Singleton.Shutdown(true);
            "已成功断开连接".Log();
        }
    }

    #region Network Callbacks

    private void OnServerStarted()
    {
        "服务器已成功启动".Log();
        // 可以在此添加服务器启动后的逻辑，例如广播服务器信息
    }

    private void OnClientConnected(ulong clientId)
    {
        $"客户端已连接，客户端ID: {clientId}".Log();
        // 可以在此添加客户端连接后的逻辑，例如初始化玩家信息
    }

    private void OnClientDisconnect(ulong clientId)
    {
        $"客户端已断开连接，客户端ID: {clientId}".Log();
        // 可以在此添加客户端断开连接后的逻辑，例如清理玩家资源
    }

    #endregion
}