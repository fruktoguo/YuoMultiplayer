using System.Collections.Generic;
using YuoTools.Main.Ecs;

namespace YuoTools
{
    public static class YuoSystemEx
    {
        public static void DestroyAll<T>(this IEnumerable<T> list) where T : YuoComponent
        {
            foreach (var item in list)
            {
                item.Entity.Destroy();
            }
        }

        public static void DestroyAll(this IEnumerable<YuoEntity> list)
        {
            foreach (var item in list)
            {
                item.Destroy();
            }
        }
    }
}