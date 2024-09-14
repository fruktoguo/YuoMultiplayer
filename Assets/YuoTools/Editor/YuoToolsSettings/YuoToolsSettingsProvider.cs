using UnityEditor;

public class YuoToolsSettingsProvider : SettingsProvider
{
    private SerializedObject _settings;

    public YuoToolsSettingsProvider(string path, SettingsScope scope)
        : base(path, scope)
    {
    }

    [SettingsProvider]
    public static SettingsProvider CreateYuoToolsSettingsProvider()
    {
        var provider = new YuoToolsSettingsProvider("Project/YuoTools Settings", SettingsScope.Project);
        provider._settings = new SerializedObject(YuoToolsSettings.GetOrCreateSettings());
        return provider;
    }

    public override void OnGUI(string searchContext)
    {
        _settings.Update();

        base.OnGUI(searchContext);

        AutoDrawProperties(_settings);

        if (_settings.ApplyModifiedProperties())
        {
            // 当有更改时，保存设置
            YuoToolsSettings.SaveSettings(_settings.targetObject as YuoToolsSettings);
        }
    }

    public static void AutoDrawProperties(SerializedObject serializedObject)
    {
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;

        while (property.NextVisible(enterChildren))
        {
            // 排除 m_Script 属性（这是Unity自动生成的脚本引用，不需要显示）
            if (property.name == "m_Script")
                continue;

            EditorGUILayout.PropertyField(property, true);
            enterChildren = false;
        }
    }
}