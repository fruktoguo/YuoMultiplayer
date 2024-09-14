using Toolbox.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class SceneFastLoadToolbar
{
    static SceneFastLoadToolbar()
    {
        EditorApplication.update -= Check;
        EditorApplication.update += Check;
    }

    private static void Check()
    {
        ToolboxEditorToolbar.OnToolbarGuiLeft -= OnToolbarGui;
        ToolboxEditorToolbar.OnToolbarGuiLeft += OnToolbarGui;
    }

    private static SceneFastLoadScriptable fastLoadScriptable;

    private static void OnToolbarGui()
    {
        if (fastLoadScriptable == null) fastLoadScriptable = Resources.Load<SceneFastLoadScriptable>("SceneFastLoad");
        if (fastLoadScriptable == null) return;
        GUILayout.FlexibleSpace();
        foreach (var scene in fastLoadScriptable.scenes)
        {
            var style = Style.SceneStyle;
            style.fixedWidth = GUI.skin.button.CalcSize(new GUIContent(scene.name)).x;
            if (GUILayout.Button(scene.name, style))
            {
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene));
            }

            GUILayout.Space(5);
        }
    }

    private static class Style
    {
        internal static readonly GUIStyle SceneStyle = new("CommandMid")
        {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove,
        };
    }
}