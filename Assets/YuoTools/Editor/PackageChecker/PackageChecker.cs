using System;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public abstract class PackageChecker
{
    private ListRequest _listRequest; // 列表请求
    private readonly string _packageName; // 包名
    private readonly string _defineSymbol; // 定义符号

    [Obsolete("Obsolete")]
    protected PackageChecker(string packageName, string defineSymbol)
    {
        _packageName = packageName;
        _defineSymbol = defineSymbol;
        _listRequest = Client.List(); // 开始列出包
        EditorApplication.update += Progress; // 注册更新回调
    }

    [Obsolete("Obsolete")]
    private void Progress()
    {
        if (_listRequest.IsCompleted) // 检查请求是否完成
        {
            if (_listRequest.Status == StatusCode.Success) // 如果请求成功
            {
                bool packageInstalled = _listRequest.Result.Any(package => package.name.Contains(_packageName)); // 检查包是否已安装
                SetScriptingDefineSymbol(_defineSymbol, packageInstalled); // 设置脚本定义符号
            }
            else if (_listRequest.Status >= StatusCode.Failure) // 如果请求失败
            {
                Debug.LogError("列出包失败: " + _listRequest.Error.message); // 输出错误信息
            }

            EditorApplication.update -= Progress; // 取消注册更新回调
        }
    }

    [Obsolete("Obsolete")]
    private void SetScriptingDefineSymbol(string symbol, bool enable)
    {
        var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup; // 获取当前目标组
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').ToList(); // 获取当前定义符号

        if (enable)
        {
            if (!defines.Contains(symbol)) // 如果定义符号不包含在列表中
            {
                defines.Add(symbol); // 添加定义符号
            }
        }
        else
        {
            if (defines.Contains(symbol)) // 如果定义符号包含在列表中
            {
                defines.Remove(symbol); // 移除定义符号
            }
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defines)); // 设置新的定义符号列表
    }
}