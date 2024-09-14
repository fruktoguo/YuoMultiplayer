using UnityEngine;

#if UNITY_EDITOR

namespace YuoTools.UI
{
    using UnityEditor;

    public class AssetChangeListener : AssetPostprocessor
    {
        // 当资产导入时调用
        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            bool assetChanged = importedAssets.Length > 0 || deletedAssets.Length > 0 || movedAssets.Length > 0;

            if (assetChanged)
            {
                if (Resources.Load<UIAutoLoadConfig>("UIAutoLoadConfig") is { } config)
                {
                    foreach (var link in config.linkDir)
                    {
                        // 检查导入的资源
                        foreach (var asset in importedAssets)
                            if(asset.StartsWith(link)){ config.Refresh(); return;}

                        // 检查删除的资源
                        foreach (var asset in deletedAssets)
                            if(asset.StartsWith(link)){ config.Refresh(); return;}

                        // 检查移动的资源
                        foreach (var asset in movedAssets)
                            if(asset.StartsWith(link)){ config.Refresh(); return;}

                        // 刷新所有包含更改的文件夹
                        foreach (var folder in movedFromAssetPaths)
                            if(folder.StartsWith(link)){ config.Refresh(); return;}
                    }
                }
            }
        }
    }
}
#endif