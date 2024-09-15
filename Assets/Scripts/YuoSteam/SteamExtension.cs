using Steamworks;
using Steamworks.Data;
using UnityEngine;
using YuoTools;
using Color = UnityEngine.Color; // 引入Steamworks库

namespace YuoSteam
{
    public static class SteamExtension
    {
        private const string PasswordKey = "password"; // 密码键常量
        private const string NameKey = "name"; // 名称键常量

        /// <summary>
        /// 设置房间密码
        /// </summary>
        /// <param name="lobby">房间</param>
        /// <param name="password">房间密码</param> // 修改注释内容
        public static void SetLobbyPassword(this Lobby lobby, string password)
        {
            lobby.SetData(PasswordKey, password);
        }

        /// <summary>
        /// 获取房间密码
        /// </summary>
        /// <param name="lobby">房间</param> // 修改注释内容
        /// <returns>返回房间密码</returns>
        public static string GetLobbyPassword(this Lobby lobby)
        {
            return lobby.GetData(PasswordKey);
        }

        /// <summary>
        /// 检查房间是否设置了密码
        /// </summary>
        /// <param name="lobby">房间</param> // 修改注释内容
        /// <returns>如果房间设置了密码则返回true，否则返回false</returns>
        public static bool HasLobbyPassword(this Lobby lobby)
        {
            string password = lobby.GetData(PasswordKey);
            return !string.IsNullOrEmpty(password);
        }

        /// <summary>
        /// 设置房间名称
        /// </summary>
        /// <param name="lobby">房间</param> // 修改注释内容
        /// <param name="name">房间名称</param>
        public static void SetLobbyName(this Lobby lobby, string name)
        {
            lobby.SetData(NameKey, name);
        }

        /// <summary>
        /// 获取房间名称
        /// </summary>
        /// <param name="lobby">房间</param> // 修改注释内容
        /// <returns>返回房间名称</returns>
        public static string GetLobbyName(this Lobby lobby)
        {
            return lobby.GetData(NameKey);
        }

        public static Texture2D ToTexture2D(this Image image)
        {
            int width = (int)image.Width;
            int height = (int)image.Height;
            var texture = new Texture2D(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var color = image.GetPixel(x, height - y -1);
                    Color unityColor = new Color(color.r / 255f, color.g / 255f, color.b / 255f,
                        color.a / 255f);
                    texture.SetPixel(x, y, unityColor);
                }
            }

            texture.Apply();
            return texture;
        }
    }
}