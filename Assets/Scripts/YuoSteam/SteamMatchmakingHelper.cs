using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;

namespace YuoSteam
{
    /// <summary>
    /// SteamMatchmaking功能的辅助类，封装了SteamMatchmaking类中的方法，并添加了中文注释。
    /// </summary>
    public static class SteamMatchmakingHelper
    {
        /// <summary>
        /// 创建一个新的不可见大厅。调用 SetPublic 方法可以将其设为公开。
        /// </summary>
        /// <param name="maxMembers">大厅的最大成员数，默认为100。</param>
        /// <returns>返回创建的大厅对象，如果创建失败则返回null。</returns>
        public static async Task<Lobby?> CreateLobbyAsync(int maxMembers = 100)
        {
            return await SteamMatchmaking.CreateLobbyAsync(maxMembers);
        }

        /// <summary>
        /// 尝试直接加入指定的大厅。
        /// </summary>
        /// <param name="lobbyId">要加入的大厅的SteamId。</param>
        /// <returns>返回加入的大厅对象，如果加入失败则返回null。</returns>
        public static async Task<Lobby?> JoinLobbyAsync(SteamId lobbyId)
        {
            return await SteamMatchmaking.JoinLobbyAsync(lobbyId);
        }

        /// <summary>
        /// 获取您收藏的服务器列表。
        /// </summary>
        /// <returns>返回收藏服务器的信息集合。</returns>
        public static IEnumerable<ServerInfo> GetFavoriteServers()
        {
            return SteamMatchmaking.GetFavoriteServers();
        }

        /// <summary>
        /// 获取您在游戏历史记录中添加的服务器列表。
        /// </summary>
        /// <returns>返回历史服务器的信息集合。</returns>
        public static IEnumerable<ServerInfo> GetHistoryServers()
        {
            return SteamMatchmaking.GetHistoryServers();
        }

        /// <summary>
        /// 获取大厅查询对象，用于查询和管理大厅列表。
        /// </summary>
        public static LobbyQuery LobbyList => SteamMatchmaking.LobbyList;
    }
}