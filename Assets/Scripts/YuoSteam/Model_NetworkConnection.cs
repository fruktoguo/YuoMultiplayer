using System;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

namespace YuoSteam
{
    public partial class YuoNetworkManager
    {
        /// <summary>
        /// 网络连接管理器，负责处理主机和客户端的网络连接及相关回调
        /// </summary>
        public class NetworkConnectionModel : NetworkModelBase
        {
            /// <summary>
            /// 构造函数，初始化网络连接管理器
            /// </summary>
            /// <param name="manager">游戏网络管理器实例</param>
            /// <param name="transport">网络传输层实例</param>
            public NetworkConnectionModel(YuoNetworkManager manager, SteamTransport transport)
                : base(manager, transport)
            {
            }
            /// <summary>
            /// 注册网络相关的回调事件
            /// </summary>
            /// <param name="isHost">是否为主机</param>
            public void RegisterNetworkCallbacks(bool isHost)
            {
                if (NetworkManager.Singleton == null)
                {
                    Debug.LogError("错误：NetworkManager.Singleton 未初始化！");
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
            /// 取消注册所有网络相关的回调事件
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
            /// 异步启动主机并创建Steam大厅
            /// </summary>
            /// <param name="maxPlayers">大厅的最大玩家数量</param>
            /// <returns>创建的Steam大厅，若失败则返回null</returns>
            public async Task<Lobby?> StartHost(int maxPlayers)
            {
                if (NetworkManager.Singleton == null)
                {
                    Debug.LogError("错误：NetworkManager.Singleton 未初始化！");
                    return null;
                }

                RegisterNetworkCallbacks(isHost: true); // 注册主机相关的回调

                // 启动主机模式
                if (NetworkManager.Singleton.StartHost())
                {
                    Debug.Log("主机已成功启动");
                    try
                    {
                        // 创建Steam大厅，设置最大玩家数量
                        var steamLobbyManager = yuoNetworkManager.GetSteamLobbyManager();
                        if (steamLobbyManager != null)
                        {
                            var lobby = await steamLobbyManager.CreateLobby(maxPlayers); // 创建大厅
                            if (lobby.HasValue)
                            {
                                Debug.Log($"Steam大厅已创建，最大玩家数量: {maxPlayers}");
                            }
                            else
                            {
                                Debug.LogError("错误：无法创建Steam大厅");
                            }

                            return lobby;
                        }
                        else
                        {
                            Debug.LogError("错误：SteamLobbyManager 未找到！");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"创建大厅异常: {ex.Message}"); // 记录异常信息
                    }
                }
                else
                {
                    Debug.LogError("错误：主机启动失败");
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
                    Debug.LogError("错误：NetworkManager.Singleton 未初始化！");
                    return;
                }

                RegisterNetworkCallbacks(isHost: false); // 注册客户端相关的回调

                // 设置传输层的目标SteamId
                if (transport != null)
                {
                    transport.targetSteamId = targetSteamId; // 设置目标SteamId
                    Debug.Log($"目标SteamId已设置: {targetSteamId}");
                }
                else
                {
                    Debug.LogError("错误：FacepunchTransport 未初始化！");
                    return;
                }

                // 启动客户端模式
                if (NetworkManager.Singleton.StartClient())
                {
                    Debug.Log("客户端已成功启动");
                }
                else
                {
                    Debug.LogError("错误：客户端启动失败");
                }
            }

            /// <summary>
            /// 断开所有网络连接并清理资源
            /// </summary>
            public void Disconnect()
            {
                if (NetworkManager.Singleton != null)
                {
                    UnregisterNetworkCallbacks(); // 取消注册回调

                    // 关闭网络管理器，强制关闭所有连接
                    NetworkManager.Singleton.Shutdown(true);
                    Debug.Log("已成功断开连接");
                }
            }

            #region 网络回调处理

            /// <summary>
            /// 服务器启动后的回调处理
            /// </summary>
            private void OnServerStarted()
            {
                Debug.Log("服务器已成功启动");
                // 可以在此添加服务器启动后的逻辑，例如广播服务器信息
            }

            /// <summary>
            /// 客户端连接成功后的回调处理
            /// </summary>
            /// <param name="clientId">连接的客户端ID</param>
            private void OnClientConnected(ulong clientId)
            {
                Debug.Log($"客户端已连接，客户端ID: {clientId}");
                // 可以在此添加客户端连接后的逻辑，例如初始化玩家信息
            }

            /// <summary>
            /// 客户端断开连接后的回调处理
            /// </summary>
            /// <param name="clientId">断开连接的客户端ID</param>
            private void OnClientDisconnect(ulong clientId)
            {
                Debug.Log($"客户端已断开连接，客户端ID: {clientId}");
                // 可以在此添加客户端断开连接后的逻辑，例如清理玩家资源
            }

            #endregion
        }
    }
}