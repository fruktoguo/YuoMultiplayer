using UnityEngine;
using System.IO;

public partial class YuoToolsSettings : ScriptableObject
{
    // 配置文件存放路径
    private static string GetSettingsPath<T>() where T : YuoToolsSettings
    {
        return $"ProjectSettings/{typeof(T).Name}.json";
    }

    // 获取或创建设置文件实例
    public static YuoToolsSettings GetOrCreateSettings()
    {
        string path = GetSettingsPath<YuoToolsSettings>();
        if (File.Exists(path))
        {
            // 如果文件存在，从JSON文件中加载
            var settings = ScriptableObject.CreateInstance<YuoToolsSettings>();
            JsonUtility.FromJsonOverwrite(File.ReadAllText(path), settings);
            return settings;
        }
        else
        {
            // 如果文件不存在，创建一个新的实例
            var settings = ScriptableObject.CreateInstance<YuoToolsSettings>();
            SaveSettings(settings);
            return settings;
        }
    }

    // 保存设置到文件
    public static void SaveSettings<T>(T settings) where T : YuoToolsSettings
    {
        string path = GetSettingsPath<T>();
        File.WriteAllText(path, JsonUtility.ToJson(settings, true));
    }
}