#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ImageProcessorEditor
{
    [MenuItem("Tools/YuoTools/Convert Non-Transparent To White")]
    static void ConvertNonTransparentToWhite()
    {
        foreach (var obj in Selection.objects)
        {
            var texture = obj as Texture2D;
            if (texture != null)
            {
                Convert(texture);
            }
        }
    }

    static void Convert(Texture2D texture)
    {
        var path = AssetDatabase.GetAssetPath(texture);
        var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        if (textureImporter != null)
        {
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();

            var pixels = texture.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a >= 0.1f) // if not transparent
                {
                    pixels[i] = Color.white;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
            AssetDatabase.ImportAsset(path);
        }
    }
}
#endif