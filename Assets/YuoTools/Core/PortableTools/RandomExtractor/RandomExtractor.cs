namespace YuoTools
{
    using System.Collections.Generic;
    using UnityEngine;

    public class RandomExtractor<T>
    {
        public string Name;
        [SerializeField] private List<RandomItem<T>> rm = new List<RandomItem<T>>();
        private List<RandomItem<T>> rmBackup = new List<RandomItem<T>>();

        /// <summary>
        ///进行一次放回抽取,并返回抽到的物品,如果奖池中没有物品,则会返回null
        /// </summary>
        /// <param name="newProp">如果需要在抽取之后,更改下次抽到该物品的抽到概率,则修改newProp为大于-1的值,修改为0即为不放回抽取</param>
        /// <param name="AutoReset">开启自动重置后,当抽取器中没有item时,将会自动重置(不放回抽取时可以使用)</param>
        /// <returns></returns>
        public RandomItem<T> GetItem(int newProp = -1, bool AutoReset = true)
        {
            if (rmBackup.Count > 0)
            {
                int[] ints = new int[rm.Count];
                for (int i = 0; i < rm.Count; i++)
                {
                    if (i != 0) ints[i] = ints[i - 1] + rm[i].Prop;
                    else ints[i] = rm[i].Prop;
                }

                int r = UnityEngine.Random.Range(0, ints[ints.Length - 1]);
                for (int i = 0; i < ints.Length; i++)
                {
                    if (ints[i] > r)
                    {
                        if (newProp > -1)
                        {
                            rm[i].Prop = newProp;
                        }

                        rm[i].RandomNum++;
                        return rm[i];
                    }
                }

                if (AutoReset && rm.Count != 0)
                {
                    ReSet();
                    return GetItem(newProp);
                }
            }

            return null;
        }

        public void SetProp(T obj, int prop)
        {
            foreach (var item in rm)
            {
                if (item.Value.Equals(obj))
                {
                    item.Prop = prop;
                }
            }
        }

        /// <summary>
        /// 添加一个随机物品,prop为抽到该物品的几率,obj为物品本身
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="obj"></param>
        public RandomItem<T> Add(int prop, T obj)
        {
            rm.Add(new RandomItem<T>(prop, obj));
            rmBackup.Add(new RandomItem<T>(prop, obj));
            return rm[^1];
        }

        /// <summary>
        /// 重置抽取器
        /// </summary>
        public void ReSet()
        {
            for (int i = 0; i < rm.Count; i++)
            {
                rm[i].Prop = rmBackup[i].Prop;
            }
        }

        public int GetNowNum
        {
            get
            {
                int i = 0;
                foreach (var item in rm)
                {
                    if (item.Prop > 0) i++;
                }

                return i;
            }
        }

        public void Clear()
        {
            rm.Clear();
            rmBackup.Clear();
        }
    }

    [System.Serializable]
    public class RandomItem<T>
    {
        /// <summary>
        /// 物体被抽到的概率
        /// </summary>
        public int Prop;

        /// <summary>
        /// 该物体被抽到过的次数
        /// </summary>
        public int RandomNum;

        public T Value;

        public RandomItem(int prop, T obj)
        {
            Prop = prop;
            Value = obj;
            RandomNum = 0;
        }
    }
}
