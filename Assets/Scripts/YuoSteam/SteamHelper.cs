using Steamworks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Steamworks.Data;

namespace YuoSteam
{
    /// <summary>
    /// Steam好友功能的辅助类，封装了SteamFriends类中的方法，并添加了中文注释。
    /// </summary>
    public static class SteamFriendsHelper
    {
        /// <summary>
        /// 获取当前用户的Steam好友列表。
        /// </summary>
        /// <returns>返回好友的SteamId列表。</returns>
        public static List<SteamId> GetFriendsIDList()
        {
            return SteamFriends.GetFriends().Select(f => f.Id).ToList();
        }

        /// <summary>
        /// 获取被屏蔽的用户列表。
        /// </summary>
        /// <returns>返回被屏蔽用户的Friend对象列表。</returns>
        public static IEnumerable<Friend> GetBlockedUsers()
        {
            return SteamFriends.GetBlocked();
        }

        /// <summary>
        /// 获取待处理的好友请求列表。
        /// </summary>
        /// <returns>返回待处理好友请求的Friend对象列表。</returns>
        public static IEnumerable<Friend> GetFriendRequests()
        {
            return SteamFriends.GetFriendsRequested();
        }

        /// <summary>
        /// 获取当前用户所在游戏服务器上的好友列表。
        /// </summary>
        /// <returns>返回同一游戏服务器上的Friend对象列表。</returns>
        public static IEnumerable<Friend> GetFriendsOnSameGameServer()
        {
            return SteamFriends.GetFriendsOnGameServer();
        }

        /// <summary>
        /// 获取请求添加为好友的用户列表。
        /// </summary>
        /// <returns>返回请求添加为好友的Friend对象列表。</returns>
        public static IEnumerable<Friend> GetFriendsRequestingFriendship()
        {
            return SteamFriends.GetFriendsRequestingFriendship();
        }

        /// <summary>
        /// 获取与当前用户一起玩的用户列表。
        /// </summary>
        /// <returns>返回与当前用户一起玩的Friend对象列表。</returns>
        public static IEnumerable<Friend> GetPlayedWith()
        {
            return SteamFriends.GetPlayedWith();
        }

        /// <summary>
        /// 根据来源SteamId获取好友列表。
        /// </summary>
        /// <param name="steamId">来源用户的SteamId。</param>
        /// <returns>返回来源用户的Friend对象列表。</returns>
        public static IEnumerable<Friend> GetFriendsFromSource(SteamId steamId)
        {
            return SteamFriends.GetFromSource(steamId);
        }

        /// <summary>
        /// 打开Steam覆盖层对话框。
        /// </summary>
        /// <param name="type">对话框类型，例如 "friends", "community", "players" 等。</param>
        public static void OpenOverlay(string type)
        {
            SteamFriends.OpenOverlay(type);
        }

        /// <summary>
        /// 打开指定用户的Steam覆盖层。
        /// </summary>
        /// <param name="id">目标用户的SteamId。</param>
        /// <param name="type">覆盖层类型，例如 "steamid", "chat" 等。</param>
        public static void OpenUserOverlay(SteamId id, string type)
        {
            SteamFriends.OpenUserOverlay(id, type);
        }

        /// <summary>
        /// 打开指定App的Steam商店覆盖层。
        /// </summary>
        /// <param name="id">目标App的AppId。</param>
        public static void OpenStoreOverlay(AppId id)
        {
            SteamFriends.OpenStoreOverlay(id);
        }

        /// <summary>
        /// 打开指定URL的Steam覆盖层网页浏览器。
        /// </summary>
        /// <param name="url">目标URL。</param>
        /// <param name="modal">是否以模态方式打开。</param>
        public static void OpenWebOverlay(string url, bool modal = false)
        {
            SteamFriends.OpenWebOverlay(url, modal);
        }

        /// <summary>
        /// 打开邀请对话框以邀请用户加入指定的大厅。
        /// </summary>
        /// <param name="lobby">目标大厅的SteamId。</param>
        public static void OpenGameInviteOverlay(SteamId lobby)
        {
            SteamFriends.OpenGameInviteOverlay(lobby);
        }

        /// <summary>
        /// 标记目标用户为"已一起游戏"。
        /// </summary>
        /// <param name="steamId">目标用户的SteamId。</param>
        public static void SetPlayedWith(SteamId steamId)
        {
            SteamFriends.SetPlayedWith(steamId);
        }

        /// <summary>
        /// 请求指定用户的信息。
        /// </summary>
        /// <param name="steamId">目标用户的SteamId。</param>
        /// <param name="nameOnly">是否仅请求用户名。</param>
        /// <returns>如果正在获取数据则返回true，否则返回false。</returns>
        public static bool RequestUserInformation(SteamId steamId, bool nameOnly = true)
        {
            return SteamFriends.RequestUserInformation(steamId, nameOnly);
        }

        /// <summary>
        /// 异步获取指定用户的小头像。
        /// </summary>
        /// <param name="steamId">目标用户的SteamId。</param>
        /// <returns>返回小头像的Image对象，如果获取失败则返回null。</returns>
        public static async Task<Image?> GetSmallAvatarAsync(SteamId steamId)
        {
            return await SteamFriends.GetSmallAvatarAsync(steamId);
        }

        /// <summary>
        /// 异步获取指定用户的中头像。
        /// </summary>
        /// <param name="steamId">目标用户的SteamId。</param>
        /// <returns>返回中头像的Image对象，如果获取失败则返回null。</returns>
        public static async Task<Image?> GetMediumAvatarAsync(SteamId steamId)
        {
            return await SteamFriends.GetMediumAvatarAsync(steamId);
        }

        /// <summary>
        /// 异步获取指定用户的大头像。
        /// </summary>
        /// <param name="steamId">目标用户的SteamId。</param>
        /// <returns>返回大头像的Image对象，如果获取失败则返回null。</returns>
        public static async Task<Image?> GetLargeAvatarAsync(SteamId steamId)
        {
            return await SteamFriends.GetLargeAvatarAsync(steamId);
        }

        /// <summary>
        /// 获取当前用户的Rich Presence信息。
        /// </summary>
        /// <param name="key">Rich Presence的键。</param>
        /// <returns>如果找到对应键，则返回对应的值；否则返回null。</returns>
        public static string GetRichPresence(string key)
        {
            return SteamFriends.GetRichPresence(key);
        }

        /// <summary>
        /// 设置当前用户的Rich Presence信息。
        /// </summary>
        /// <param name="key">Rich Presence的键。</param>
        /// <param name="value">Rich Presence的值。</param>
        /// <returns>设置是否成功。</returns>
        public static bool SetRichPresence(string key, string value)
        {
            return SteamFriends.SetRichPresence(key, value);
        }

        /// <summary>
        /// 清除当前用户的所有Rich Presence信息。
        /// </summary>
        public static void ClearRichPresence()
        {
            SteamFriends.ClearRichPresence();
        }

        /// <summary>
        /// 获取或设置是否监听Steam好友的消息。
        /// </summary>
        public static bool ListenForFriendsMessages
        {
            get => SteamFriends.ListenForFriendsMessages;
            set => SteamFriends.ListenForFriendsMessages = value;
        }

        /// <summary>
        /// 异步检查当前用户是否正在关注指定用户。
        /// </summary>
        /// <param name="steamId">要检查的目标用户SteamId。</param>
        /// <returns>如果正在关注则返回true，否则返回false。</returns>
        public static async Task<bool> IsFollowingAsync(SteamId steamId)
        {
            return await SteamFriends.IsFollowing(steamId);
        }

        /// <summary>
        /// 异步获取指定用户的关注者数量。
        /// </summary>
        /// <param name="steamId">目标用户的SteamId。</param>
        /// <returns>返回关注者数量。</returns>
        public static async Task<int> GetFollowerCountAsync(SteamId steamId)
        {
            return await SteamFriends.GetFollowerCount(steamId);
        }

        /// <summary>
        /// 异步获取当前用户正在关注的用户列表。
        /// </summary>
        /// <returns>返回正在关注的用户SteamId数组。</returns>
        public static async Task<SteamId[]> GetFollowingListAsync()
        {
            return await SteamFriends.GetFollowingList();
        }
    }

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