using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace YuoSteam
{
    using SocketConnection = Connection;

    /// <summary>
    /// Steam传输层类，用于处理基于Steam网络的网络通信
    /// </summary>
    public class SteamTransport : NetworkTransport, IConnectionManager, ISocketManager
    {
        /// <summary>
        /// Steam连接管理器
        /// </summary>
        private ConnectionManager connectionManager;

        /// <summary>
        /// Steam套接字管理器
        /// </summary>
        private SocketManager socketManager;

        /// <summary>
        /// 已连接客户端的字典，键为客户端ID，值为Client对象
        /// </summary>
        private Dictionary<ulong, Client> connectedClients;

        [Space] [Tooltip("你的游戏的Steam App ID。技术上不允许使用480，但Valve并不会对此采取行动，所以用于测试目的是可以的。")] [SerializeField]
        private uint steamAppId = 480;

        [Tooltip("作为客户端加入时目标用户的Steam ID。")] [SerializeField]
        public ulong targetSteamId;

        [Header("信息")] [ReadOnly] [Tooltip("在播放模式下，这里将显示你的Steam ID。")] [SerializeField]
        private ulong userSteamId;

        /// <summary>
        /// 获取当前的日志级别
        /// </summary>
        private LogLevel LogLevel => NetworkManager.Singleton.LogLevel;

        /// <summary>
        /// 表示一个连接的客户端
        /// </summary>
        private class Client
        {
            public SteamId steamId;
            public SocketConnection connection;
        }

        #region MonoBehaviour Messages

        /// <summary>
        /// 在对象被实例化时调用，用于初始化Steam客户端
        /// </summary>
        private void Awake()
        {
            try
            {
                SteamClient.Init(steamAppId, false);
            }
            catch (Exception e)
            {
                if (LogLevel <= LogLevel.Error)
                    Debug.LogError($"[{nameof(SteamTransport)}] - 初始化Steam客户端时捕获到异常: {e}");
            }
            finally
            {
                StartCoroutine(InitSteamworks());
            }
        }

        /// <summary>
        /// 每帧调用，用于运行Steam回调
        /// </summary>
        private void Update()
        {
            SteamClient.RunCallbacks();
        }

        /// <summary>
        /// 在对象被销毁时调用，用于关闭Steam客户端
        /// </summary>
        private void OnDestroy()
        {
            SteamClient.Shutdown();
        }

        #endregion

        #region NetworkTransport Overrides

        /// <summary>
        /// 获取服务器客户端ID
        /// </summary>
        public override ulong ServerClientId => 0;

        /// <summary>
        /// 断开本地客户端连接
        /// </summary>
        public override void DisconnectLocalClient()
        {
            connectionManager?.Connection.Close();

            if (LogLevel <= LogLevel.Developer)
                Debug.Log($"[{nameof(SteamTransport)}] - 断开本地客户端连接。");
        }

        /// <summary>
        /// 断开远程客户端连接
        /// </summary>
        /// <param name="clientId">要断开连接的客户端ID</param>
        public override void DisconnectRemoteClient(ulong clientId)
        {
            if (connectedClients.TryGetValue(clientId, out Client user))
            {
                // 在关闭连接前刷新所有待处理的消息
                user.connection.Flush();
                user.connection.Close();
                connectedClients.Remove(clientId);

                if (LogLevel <= LogLevel.Developer)
                    Debug.Log($"[{nameof(SteamTransport)}] - 断开ID为 {clientId} 的远程客户端连接。");
            }
            else if (LogLevel <= LogLevel.Normal)
                Debug.LogWarning($"[{nameof(SteamTransport)}] - 断开ID为 {clientId} 的远程客户端连接失败，客户端未连接。");
        }

        /// <summary>
        /// 获取当前往返时间（RTT）
        /// </summary>
        /// <param name="clientId">客户端ID</param>
        /// <returns>往返时间</returns>
        public override unsafe ulong GetCurrentRtt(ulong clientId)
        {
            return 0;
        }

        /// <summary>
        /// 初始化传输层
        /// </summary>
        /// <param name="networkManager">网络管理器实例</param>
        public override void Initialize(NetworkManager networkManager = null)
        {
            connectedClients = new Dictionary<ulong, Client>();
        }

        /// <summary>
        /// 将NetworkDelivery枚举转换为SendType枚举
        /// </summary>
        /// <param name="delivery">NetworkDelivery枚举值</param>
        /// <returns>对应的SendType枚举值</returns>
        private SendType NetworkDeliveryToSendType(NetworkDelivery delivery)
        {
            return delivery switch
            {
                NetworkDelivery.Reliable => SendType.Reliable,
                NetworkDelivery.ReliableFragmentedSequenced => SendType.Reliable,
                NetworkDelivery.ReliableSequenced => SendType.Reliable,
                NetworkDelivery.Unreliable => SendType.Unreliable,
                NetworkDelivery.UnreliableSequenced => SendType.Unreliable,
                _ => SendType.Reliable
            };
        }

        /// <summary>
        /// 关闭传输层
        /// </summary>
        public override void Shutdown()
        {
            try
            {
                if (LogLevel <= LogLevel.Developer)
                    Debug.Log($"[{nameof(SteamTransport)}] - 正在关闭。");

                connectionManager?.Close();
                socketManager?.Close();
            }
            catch (Exception e)
            {
                if (LogLevel <= LogLevel.Error)
                    Debug.LogError($"[{nameof(SteamTransport)}] - 关闭时捕获到异常: {e}");
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="clientId">目标客户端ID</param>
        /// <param name="data">要发送的数据</param>
        /// <param name="delivery">传输方式</param>
        public override void Send(ulong clientId, ArraySegment<byte> data, NetworkDelivery delivery)
        {
            var sendType = NetworkDeliveryToSendType(delivery);

            if (clientId == ServerClientId)
                connectionManager.Connection.SendMessage(data.Array, data.Offset, data.Count, sendType);
            else if (connectedClients.TryGetValue(clientId, out Client user))
                user.connection.SendMessage(data.Array, data.Offset, data.Count, sendType);
            else if (LogLevel <= LogLevel.Normal)
                Debug.LogWarning($"[{nameof(SteamTransport)}] - 向ID为 {clientId} 的远程客户端发送数据包失败，客户端未连接。");
        }

        /// <summary>
        /// 轮询网络事件
        /// </summary>
        /// <param name="clientId">触发事件的客户端ID</param>
        /// <param name="payload">事件携带的数据</param>
        /// <param name="receiveTime">接收时间</param>
        /// <returns>网络事件类型</returns>
        public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload,
            out float receiveTime)
        {
            connectionManager?.Receive();
            socketManager?.Receive();

            clientId = 0;
            receiveTime = Time.realtimeSinceStartup;
            payload = default;
            return NetworkEvent.Nothing;
        }

        /// <summary>
        /// 启动客户端
        /// </summary>
        /// <returns>是否成功启动</returns>
        public override bool StartClient()
        {
            if (LogLevel <= LogLevel.Developer)
                Debug.Log($"[{nameof(SteamTransport)}] - 正在以客户端身份启动。");

            connectionManager = SteamNetworkingSockets.ConnectRelay<ConnectionManager>(targetSteamId);
            connectionManager.Interface = this;
            return true;
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <returns>是否成功启动</returns>
        public override bool StartServer()
        {
            if (LogLevel <= LogLevel.Developer)
                Debug.Log($"[{nameof(SteamTransport)}] - 正在以服务器身份启动。");

            socketManager = SteamNetworkingSockets.CreateRelaySocket<SocketManager>();
            socketManager.Interface = this;
            return true;
        }

        #endregion

        #region ConnectionManager Implementation

        /// <summary>
        /// 用于缓存接收到的数据的字节数组
        /// </summary>
        private byte[] payloadCache = new byte[4096];

        /// <summary>
        /// 确保缓存数组有足够的容量
        /// </summary>
        /// <param name="size">所需的最小容量</param>
        private void EnsurePayloadCapacity(int size)
        {
            if (payloadCache.Length >= size)
                return;

            payloadCache = new byte[Math.Max(payloadCache.Length * 2, size)];
        }

        /// <summary>
        /// 当正在连接时调用
        /// </summary>
        /// <param name="info">连接信息</param>
        void IConnectionManager.OnConnecting(ConnectionInfo info)
        {
            if (LogLevel <= LogLevel.Developer)
                Debug.Log($"[{nameof(SteamTransport)}] - 正在与Steam用户 {info.Identity.SteamId} 建立连接。");
        }

        /// <summary>
        /// 当连接建立时调用
        /// </summary>
        /// <param name="info">连接信息</param>
        void IConnectionManager.OnConnected(ConnectionInfo info)
        {
            InvokeOnTransportEvent(NetworkEvent.Connect, ServerClientId, default, Time.realtimeSinceStartup);

            if (LogLevel <= LogLevel.Developer)
                Debug.Log($"[{nameof(SteamTransport)}] - 已与Steam用户 {info.Identity.SteamId} 建立连接。");
        }

        /// <summary>
        /// 当连接断开时调用
        /// </summary>
        /// <param name="info">连接信息</param>
        void IConnectionManager.OnDisconnected(ConnectionInfo info)
        {
            InvokeOnTransportEvent(NetworkEvent.Disconnect, ServerClientId, default, Time.realtimeSinceStartup);

            if (LogLevel <= LogLevel.Developer)
                Debug.Log($"[{nameof(SteamTransport)}] - 与Steam用户 {info.Identity.SteamId} 的连接已断开。");
        }

        /// <summary>
        /// 当收到消息时调用
        /// </summary>
        /// <param name="data">消息数据的指针</param>
        /// <param name="size">消息大小</param>
        /// <param name="messageNum">消息编号</param>
        /// <param name="recvTime">接收时间</param>
        /// <param name="channel">通道</param>
        unsafe void IConnectionManager.OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            EnsurePayloadCapacity(size);

            fixed (byte* payload = payloadCache)
            {
                UnsafeUtility.MemCpy(payload, (byte*)data, size);
            }

            InvokeOnTransportEvent(NetworkEvent.Data, ServerClientId, new ArraySegment<byte>(payloadCache, 0, size),
                Time.realtimeSinceStartup);
        }

        #endregion

        #region SocketManager Implementation

        /// <summary>
        /// 当有新的连接请求时调用
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="info">连接信息</param>
        void ISocketManager.OnConnecting(SocketConnection connection, ConnectionInfo info)
        {
            if (LogLevel <= LogLevel.Developer)
                Debug.Log($"[{nameof(SteamTransport)}] - 正在接受来自Steam用户 {info.Identity.SteamId} 的连接。");

            connection.Accept();
        }

        /// <summary>
        /// 当新的连接建立时调用
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="info">连接信息</param>
        void ISocketManager.OnConnected(SocketConnection connection, ConnectionInfo info)
        {
            if (!connectedClients.ContainsKey(connection.Id))
            {
                connectedClients.Add(connection.Id, new Client()
                {
                    connection = connection,
                    steamId = info.Identity.SteamId
                });

                InvokeOnTransportEvent(NetworkEvent.Connect, connection.Id, default, Time.realtimeSinceStartup);

                if (LogLevel <= LogLevel.Developer)
                    Debug.Log($"[{nameof(SteamTransport)}] - 已与Steam用户 {info.Identity.SteamId} 建立连接。");
            }
            else if (LogLevel <= LogLevel.Normal)
                Debug.LogWarning($"[{nameof(SteamTransport)}] - 与ID为 {connection.Id} 的客户端建立连接失败，客户端已连接。");
        }

        /// <summary>
        /// 当连接断开时调用
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="info">连接信息</param>
        void ISocketManager.OnDisconnected(SocketConnection connection, ConnectionInfo info)
        {
            if (connectedClients.Remove(connection.Id))
            {
                InvokeOnTransportEvent(NetworkEvent.Disconnect, connection.Id, default, Time.realtimeSinceStartup);

                if (LogLevel <= LogLevel.Developer)
                    Debug.Log($"[{nameof(SteamTransport)}] - Steam用户 {info.Identity.SteamId} 已断开连接");
            }
            else if (LogLevel <= LogLevel.Normal)
                Debug.LogWarning($"[{nameof(SteamTransport)}] - 断开ID为 {connection.Id} 的客户端连接失败，客户端未连接。");
        }

        /// <summary>
        /// 当收到消息时调用
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="identity">网络身份</param>
        /// <param name="data">消息数据的指针</param>
        /// <param name="size">消息大小</param>
        /// <param name="messageNum">消息编号</param>
        /// <param name="recvTime">接收时间</param>
        /// <param name="channel">通道</param>
        unsafe void ISocketManager.OnMessage(SocketConnection connection, NetIdentity identity, IntPtr data, int size,
            long messageNum, long recvTime, int channel)
        {
            EnsurePayloadCapacity(size);

            fixed (byte* payload = payloadCache)
            {
                UnsafeUtility.MemCpy(payload, (byte*)data, size);
            }

            InvokeOnTransportEvent(NetworkEvent.Data, connection.Id, new ArraySegment<byte>(payloadCache, 0, size),
                Time.realtimeSinceStartup);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// 初始化Steamworks
        /// </summary>
        /// <returns>协程</returns>
        private IEnumerator InitSteamworks()
        {
            yield return new WaitUntil(() => SteamClient.IsValid);

            SteamNetworkingUtils.InitRelayNetworkAccess();

            if (LogLevel <= LogLevel.Developer)
                Debug.Log($"[{nameof(SteamTransport)}] - 已初始化Steam中继网络访问。");

            userSteamId = SteamClient.SteamId;

            if (LogLevel <= LogLevel.Developer)
                Debug.Log($"[{nameof(SteamTransport)}] - 已获取用户Steam ID。");
        }

        #endregion
    }
}