using System;

namespace YuoTools.Main.Ecs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SystemOrderAttribute : Attribute
    {
        public short Order { get; } = 0;

        /// <summary>
        /// system排序,数值更小的优先执行
        /// </summary>
        /// <param name="order"></param>
        public SystemOrderAttribute(short order = 0)
        {
            Order = order;
        }
    }
}
