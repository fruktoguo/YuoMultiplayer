using System;
using System.Collections.Generic;
using UnityEngine;

namespace YuoTools
{
    public class YuoObjectPool<T>
    {
        public YuoObjectPool(Func<T> createFunc, Action<T> resetAction = null, Action<T> removeAction = null,
            Action<T> destroyAction = null, int maxPoolCount = -1)
        {
            onCreatItem = createFunc;
            onResetItem = resetAction;
            onRemoveItem = removeAction;
            onDestroyItem = destroyAction;
            MaxPoolCount = maxPoolCount;
        }

        public LinkedList<T> Actives = new();

        [SerializeField] private Queue<T> pools = new();

        private Func<T> onCreatItem; // 创建对象的委托
        private Action<T> onResetItem; // 重置对象的委托
        private Action<T> onRemoveItem; // 销毁对象的委托
        private Action<T> onDestroyItem; // 销毁对象的委托

        public void Remove(T item)
        {
            if (Actives.Contains(item))
            {
                Actives.Remove(item);
                if (MaxPoolCount >= 0 && pools.Count > MaxPoolCount)
                    onDestroyItem?.Invoke(item);
                else
                    pools.Enqueue(item);
                onRemoveItem?.Invoke(item);
            }
            else
            {
                $"要移除的物体 [{item}] 不属于该对象池".Log("#ff0000");
            }
        }

        public int ActiveCount => Actives.Count;

        /// <summary>
        /// 池子最大数量，负数则为无限制
        /// </summary>
        public int MaxPoolCount;

        /// <summary>
        /// 创建物体的预设体
        /// </summary>
        public T ItemPrefab;

        /// <summary>
        /// 临时变量
        /// </summary>
        private T itemTemp;

        /// <summary>
        /// 获取一个item
        /// </summary>
        /// <returns></returns>
        public T GetItem()
        {
            itemTemp = pools.Count > 0 ? pools.Dequeue() : onCreatItem.Invoke();

            onResetItem?.Invoke(itemTemp);

            Actives.AddLast(itemTemp);
            return itemTemp;
        }
    }
}