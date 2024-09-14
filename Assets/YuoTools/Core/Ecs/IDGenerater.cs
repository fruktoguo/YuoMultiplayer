using System;
using System.Threading;
using YuoTools.Extend.Helper;

namespace YuoTools.Main.Ecs
{
    public class IDGenerate
    {
        private static IDGenerate instance;

        public static IDGenerate Instance => instance ??= new IDGenerate();

        public enum IDType
        {
            Scene,
        }

        public static long GetID(IDType type, long id)
        {
            switch (type)
            {
                case IDType.Scene:
                    return id + 10000;
                default:
                    return id;
            }
        }

        public static long GetID(YuoEntity entity)
        {
            return entity.GetHashCode();
        }

        public static long GetID(string name)
        {
            return name.GetHashCode() + 10000000000;
        }

        public IDGenerate()
        {
            long epoch1970Tick = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000;
            epoch2022 = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000 - epoch1970Tick;
        }

        public const int MaxZone = 1024;

        public const int Mask14BIT = 0x3fff;
        public const int Mask30BIT = 0x3fffffff;
        public const int Mask20BIT = 0xfffff;

        private long epoch2022;

        private int value;
        private int instanceIdValue;

        private uint TimeSince2022()
        {
            uint a = (uint)((TimeInfo.Instance.FrameTime - this.epoch2022) / 1000);
            return a;
        }

        long ToLong(ulong time, ulong v)
        {
            ulong result = 0;
            result |= time;
            result <<= 32;
            result |= v;
            return (long)result;
        }

        public long GenerateInstanceId()
        {
            uint time = this.TimeSince2022();
            uint v = (uint)Interlocked.Add(ref this.instanceIdValue, 1);
            return ToLong(time, v);
        }
    }
}