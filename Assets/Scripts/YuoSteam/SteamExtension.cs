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
        /// <param name="lobby">房间对象</param>
        /// <param name="password">要设置的房间密码</param>
        public static void SetLobbyPassword(this Lobby lobby, string password)
        {
            lobby.SetData(PasswordKey, password); // 使用Steamworks的SetData方法设置房间密码
        }

        /// <summary>
        /// 获取房间密码
        /// </summary>
        /// <param name="lobby">房间对象</param>
        /// <returns>返回房间密码</returns>
        public static string GetLobbyPassword(this Lobby lobby)
        {
            return lobby.GetData(PasswordKey); // 使用Steamworks的GetData方法获取房间密码
        }

        /// <summary>
        /// 检查房间是否设置了密码
        /// </summary>
        /// <param name="lobby">房间对象</param>
        /// <returns>如果房间设置了密码则返回true，否则返回false</returns>
        public static bool HasLobbyPassword(this Lobby lobby)
        {
            string password = lobby.GetData(PasswordKey); // 获取房间密码
            return !string.IsNullOrEmpty(password); // 检查密码是否为空
        }

        /// <summary>
        /// 设置房间名称
        /// </summary>
        /// <param name="lobby">房间对象</param>
        /// <param name="name">要设置的房间名称</param>
        public static void SetLobbyName(this Lobby lobby, string name)
        {
            lobby.SetData(NameKey, name); // 使用Steamworks的SetData方法设置房间名称
        }

        /// <summary>
        /// 获取房间名称
        /// </summary>
        /// <param name="lobby">房间对象</param>
        /// <returns>返回房间名称</returns>
        public static string GetLobbyName(this Lobby lobby)
        {
            return lobby.GetData(NameKey); // 使用Steamworks的GetData方法获取房间名称
        }

        public static Texture2D ToTexture2D(this Image image)
        {
            int width = (int)image.Width; // 获取图片宽度
            int height = (int)image.Height; // 获取图片高度
            var texture = new Texture2D(width, height); // 创建一个新的Texture2D对象
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var color = image.GetPixel(x, height - y - 1); // 获取图片像素颜色
                    Color unityColor = new Color(color.r / 255f, color.g / 255f, color.b / 255f, color.a / 255f); // 转换为Unity的Color对象
                    texture.SetPixel(x, y, unityColor); // 设置Texture2D的像素颜色
                }
            }

            texture.Apply(); // 应用更改
            return texture; // 返回转换后的Texture2D对象
        }
    }
}