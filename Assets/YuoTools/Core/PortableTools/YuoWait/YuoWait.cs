using System;
using System.Collections.Generic;
using ET;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace YuoTools
{
    public class YuoWait
    {
        // 存储WaitForSeconds对象的字典
        private static Dictionary<int, WaitForSeconds> waits = new();

        // 存储WaitForSecondsRealtime对象的字典
        private static Dictionary<int, WaitForSecondsRealtime> waitsRealtime = new();

        #region 协程相关方法

        /// <summary>
        /// 返回一个WaitForSeconds对象
        /// </summary>
        /// <param name="time">等待时间（秒）</param>
        /// <returns>WaitForSeconds对象</returns>
        public static WaitForSeconds WaitTime(float time = 1)
        {
            int key = (int)(time * 1000);
            if (!waits.ContainsKey(key))
            {
                waits.Add(key, new WaitForSeconds(time));
            }

            return waits[key];
        }

        /// <summary>
        /// 返回一个不受TimeScale影响的WaitForSecondsRealtime对象
        /// </summary>
        /// <param name="time">等待时间（秒）</param>
        /// <returns>WaitForSecondsRealtime对象</returns>
        public static WaitForSecondsRealtime WaitUnscaledTime(float time = 1)
        {
            int key = (int)(time * 1000);
            if (!waitsRealtime.ContainsKey(key))
            {
                waitsRealtime.Add(key, new WaitForSecondsRealtime(time));
            }

            return waitsRealtime[key];
        }

        #endregion

        #region 异步任务相关方法

        /// <summary>
        /// 异步等待指定时间（受TimeScale影响）
        /// </summary>
        /// <param name="waitTime">等待时间（秒）</param>
        /// <returns>ETTask</returns>
        public static async ETTask WaitTimeAsync(float waitTime) => await YuoAwait_Mono.Instance.WaitTimeAsync(waitTime);

        /// <summary>
        /// 异步等待指定帧数
        /// </summary>
        /// <param name="frame">等待帧数</param>
        /// <returns>ETTask</returns>
        public static async ETTask WaitFrameAsync(int frame = 1) => await YuoAwait_Mono.Instance.WaitFrameAsync(frame);

        /// <summary>
        /// 异步等待指定时间（不受TimeScale影响）
        /// </summary>
        /// <param name="waitTime">等待时间（秒）</param>
        /// <returns>ETTask</returns>
        public static async ETTask WaitUnscaledTimeAsync(float waitTime) => 
            await YuoAwait_Mono.Instance.WaitUnscaledTimeAsync(waitTime);

        /// <summary>
        /// 再一段时间内每帧执行一次
        /// </summary>
        /// <param name="waitTime"></param>
        /// <param name="action"></param>
        public static async ETTask InvokeEveryTimeAsync(float waitTime, UnityAction<float> action)
        {
            var timer = 0f;
            while (timer < waitTime)
            {
                action.Invoke(timer);
                timer += Time.deltaTime;
                await WaitFrameAsync();
            }

            action.Invoke(waitTime);
        }

        /// <summary>
        /// 再一段时间内每帧执行一次(不受TimeScale影响)
        /// </summary>
        /// <param name="waitTime"></param>
        /// <param name="action"></param>
        public static async ETTask InvokeEveryUnscaledTimeAsync(float waitTime, UnityAction action)
        {
            var timer = 0f;
            while (timer < waitTime)
            {
                action.Invoke();
                timer += Time.unscaledDeltaTime;
                await WaitFrameAsync();
            }

            action.Invoke();
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <returns>ETTask</returns>
        public static ETTask<T> ResourcesLoadAsync<T>(string path) where T : Object => YuoAwait_Mono.Instance.ResourcesLoadAsync<T>(path);

        /// <summary>
        /// 异步等待指定按键按下
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>ETTask</returns>
        public static async ETTask WaitInputKeyDownAsync(KeyCode keyCode)
        {
            await WaitFrameAsync();

            while (!Input.GetKeyDown(keyCode))
            {
                await WaitFrameAsync();
            }
        }
        
        public static async ETTask WaitInputAnyKeyDownAsync()
        {
            await WaitFrameAsync();

            while (!Input.anyKeyDown)
            {
                await WaitFrameAsync();
            }
        }
        
        public static async ETTask WaitInputMouseDownAsync(int button)
        {
            await WaitFrameAsync();

            while (!Input.GetMouseButtonDown(button))
            {
                await WaitFrameAsync();
            }
        }

        /// <summary>
        /// 异步等待指定按键按住
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>ETTask</returns>
        public static async ETTask WaitInputKeyAsync(KeyCode keyCode)
        {
            await WaitFrameAsync();
            while (!Input.GetKey(keyCode))
            {
                await WaitFrameAsync();
            }
        }

        /// <summary>
        /// 异步等待直到指定条件为真
        /// </summary>
        /// <param name="condition">条件函数</param>
        /// <returns>ETTask</returns>
        public static async ETTask WaitUntilAsync(Func<bool> condition)
        {
            while (!condition())
            {
                await WaitFrameAsync();
            }
        }

        #endregion
    }
}