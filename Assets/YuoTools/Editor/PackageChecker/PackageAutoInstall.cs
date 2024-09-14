using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

/// <summary>
/// 通用的包自动安装基类
/// </summary>
public abstract class PackageAutoInstall
{
    private ListRequest _listRequest;
    private Queue<string> _packagesToInstall;
    private AddRequest _addRequest;

    /// <summary>
    /// 构造函数，接受包名列表作为参数
    /// </summary>
    /// <param name="packageNames">包名列表</param>
    protected PackageAutoInstall(params string[] packageNames)
    {
        _packagesToInstall = new Queue<string>(packageNames);
        _listRequest = Client.List();
        EditorApplication.update += Progress;
    }

    /// <summary>
    /// 进度更新方法，检查包是否安装并自动引入包
    /// </summary>
    private void Progress()
    {
        if (_listRequest != null && _listRequest.IsCompleted)
        {
            if (_listRequest.Status == StatusCode.Success)
            {
                var installedPackages = _listRequest.Result.Select(package => package.name).ToHashSet();

                // 过滤掉已安装的包
                _packagesToInstall = new Queue<string>(_packagesToInstall.Where(package => !installedPackages.Contains(package)));

                // 如果有未安装的包，开始安装第一个包
                if (_packagesToInstall.Count > 0)
                {
                    // 显示确认对话框
                    string packagesList = string.Join("\n", _packagesToInstall);
                    if (EditorUtility.DisplayDialog("确认安装包", "是否要安装以下包?\n" + packagesList, "是", "否"))
                    {
                        _addRequest = Client.Add(_packagesToInstall.Dequeue());
                    }
                    else
                    {
                        _packagesToInstall.Clear();
                        EditorApplication.update -= Progress;
                    }
                }
            }
            else if (_listRequest.Status >= StatusCode.Failure)
            {
                Debug.LogError("列出包失败: " + _listRequest.Error.message);
            }

            _listRequest = null;
        }

        if (_addRequest != null && _addRequest.IsCompleted)
        {
            if (_addRequest.Status == StatusCode.Success)
            {
                Debug.Log("成功添加包: " + _addRequest.Result.packageId);
            }
            else if (_addRequest.Status >= StatusCode.Failure)
            {
                Debug.LogError("添加包失败: " + _addRequest.Error.message);
            }

            // 如果还有未安装的包，继续安装下一个包
            if (_packagesToInstall.Count > 0)
            {
                _addRequest = Client.Add(_packagesToInstall.Dequeue());
            }
            else
            {
                _addRequest = null;
                EditorApplication.update -= Progress;
            }
        }
    }
}