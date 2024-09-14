using System.Collections;
using System.Collections.Generic;
using ET;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YuoTools
{
    public class YuoAwait_Mono : SingletonMono<YuoAwait_Mono>
    {
        [SerializeField] private YuoObjectPool<AwaitItem> awaitPools;

        public override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            gameObject.name = "YuoAwait";
            awaitPools = new YuoObjectPool<AwaitItem>(createFunc: () => new AwaitItem(), resetAction: item =>
            {
                item.timeType = TimeType.Normal;
                item.tcs = ETTask.Create(true);
            });
        }

        private void Update()
        {
            CheckTimerOut();
        }

        private List<AwaitItem> removeList = new List<AwaitItem>();

        /// <summary>
        /// 检查并处理超时的AwaitItem
        /// </summary>
        private void CheckTimerOut()
        {
            if (awaitPools.Actives.Count > 0)
            {
                foreach (var item in awaitPools.Actives)
                {
                    switch (item.timeType)
                    {
                        case TimeType.Normal:
                            if (Time.time < item.TargetTime) continue;
                            break;
                        case TimeType.Unscaled:
                            if (Time.unscaledTime < item.TargetTime) continue;
                            break;
                        case TimeType.Frame:
                            if (Time.frameCount < item.TargetFrame) continue;
                            break;
                    }

                    removeList.Add(item);
                }


                if (removeList.Count > 0)
                {
                    foreach (var item in removeList)
                    {
                        item.tcs.SetResult();
                        awaitPools.Remove(item);
                    }

                    removeList.Clear();
                }
            }

            if (_frameAwaits.Count > 0)
            {
                _frameAwaits.Clear();
            }
        }

        /// <summary>
        /// 等待指定的无缩放时间
        /// </summary>
        /// <param name="waitTime">等待时间</param>
        /// <returns></returns>
        public async ETTask WaitUnscaledTimeAsync(float waitTime)
        {
            AwaitItem item = awaitPools.GetItem();
            item.timeType = TimeType.Unscaled;
            item.CreatTime = Time.unscaledTime;
            item.TargetTime = Time.unscaledTime + waitTime;
            await item.tcs;
        }

        /// <summary>
        /// 等待指定的时间
        /// </summary>
        /// <param name="waitTime">等待时间</param>
        /// <returns></returns>
        public async ETTask WaitTimeAsync(float waitTime)
        {
            AwaitItem item = awaitPools.GetItem();
            item.timeType = TimeType.Normal;
            item.CreatTime = Time.time;
            waitTime = Mathf.Clamp(waitTime, 0.00001f, float.MaxValue);
            item.TargetTime = Time.time + waitTime;
            await item.tcs;
        }

        /// <summary>
        /// 等待指定的帧数
        /// </summary>
        /// <param name="frame">等待帧数</param>
        /// <returns></returns>
        public async ETTask WaitFrameAsync(int frame)
        {
            var item = CreateFrameAwait(frame);
            await item.tcs;
        }

        Dictionary<int, AwaitItem> _frameAwaits = new();

        /// <summary>
        /// 优化,相同帧数时会使用同一个Task
        /// </summary>
        AwaitItem GetFrameAwait(int frame)
        {
            if (!_frameAwaits.TryGetValue(frame, out var item))
            {
                item = CreateFrameAwait(frame);
                _frameAwaits.Add(frame, item);
            }

            return item;
        }

        AwaitItem CreateFrameAwait(int frame)
        {
            var item = awaitPools.GetItem();
            item.timeType = TimeType.Frame;
            item.CreatFrame = Time.frameCount;
            frame = Mathf.Clamp(frame, 1, int.MaxValue);
            item.TargetFrame = Time.frameCount + frame;
            return item;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <returns></returns>
        public ETTask<T> ResourcesLoadAsync<T>(string path)
            where T : Object
        {
            ETTask<T> tcs = ETTask<T>.Create(true);
            Instance.StartCoroutine(LoadAsset(path, tcs));
            return tcs;
        }

        /// <summary>
        /// 协程加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="tcs">任务完成源</param>
        /// <returns></returns>
        private IEnumerator LoadAsset<T>(string path, ETTask<T> tcs)
            where T : Object
        {
            ResourceRequest asset = Resources.LoadAsync<T>(path);
            yield return asset;
            var go = asset.asset as T;
            tcs.SetResult(go);
        }

        private class AwaitItem
        {
            public ETTask tcs = ETTask.Create(true);

            /// <summary>
            /// 添加时间
            /// </summary>
            public float CreatTime = 0;

            /// <summary>
            /// 目标时间
            /// </summary>
            public float TargetTime = 0;

            /// <summary>
            /// 目标帧数
            /// </summary>
            public int TargetFrame = 0;

            /// <summary>
            /// 添加帧数
            /// </summary>
            public float CreatFrame = 0;

            public TimeType timeType = TimeType.Normal;
        }

        public enum TimeType
        {
            Normal = 0,
            Unscaled = 1,
            Frame = 2
        }
    }
}