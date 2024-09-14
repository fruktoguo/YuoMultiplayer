using System.Collections.Generic;
using UnityEngine;

namespace YuoTools
{
    public abstract class PoolsBase<T>
    {
        public LinkedList<T> Actives = new();

        [SerializeField] private Queue<T> pools = new();

        public void Remove(T item)
        {
            if (Actives.Contains(item))
            {
                Actives.Remove(item);
                if (MaxPoolCount >= 0 && pools.Count > MaxPoolCount)
                    OnDestroyItem(item);
                else
                    pools.Enqueue(item);
                OnRemoveItem(item);
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
        public int MaxPoolCount = -1;

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
            itemTemp = pools.Count > 0 ? pools.Dequeue() : CreatItem();

            OnResetItem(itemTemp);

            Actives.AddLast(itemTemp);
            return itemTemp;
        }

        /// <summary>
        /// 创建新的item
        /// </summary>
        /// <returns></returns>
        public abstract T CreatItem();

        /// <summary>
        /// 获取一个item时自动重置
        /// </summary>
        /// <param name="item"></param>
        public abstract void OnResetItem(T item);

        public virtual void OnRemoveItem(T item)
        {
        }

        /// <summary>
        /// 超出最大池子容量时销毁
        /// </summary>
        /// <param name="item"></param>
        public abstract void OnDestroyItem(T item);
    }
}