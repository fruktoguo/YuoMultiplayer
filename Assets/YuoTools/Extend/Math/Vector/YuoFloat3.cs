using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YuoTools.Extend.MathFunction
{
    public struct YuoFloat3 : IEquatable<YuoFloat3>, IFormattable
    {
        public float x, y, z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YuoFloat3(float x, float y, float z) => (this.x, this.y, this.z) = (x, y, z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(YuoFloat3 other) => x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider) =>
            $"({x.ToString(format, formatProvider)},{y.ToString(format, formatProvider)},{z.ToString(format, formatProvider)})";

        public float Magnitude => Mathf.Sqrt(x * x + y * y + z * z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(YuoFloat3 a, YuoFloat3 b) => (a - b).Magnitude;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator +(YuoFloat3 a, YuoFloat3 b) => new YuoFloat3(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator -(YuoFloat3 a, YuoFloat3 b) => new YuoFloat3(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator *(YuoFloat3 a, float b) => new YuoFloat3(a.x * b, a.y * b, a.z * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator /(YuoFloat3 a, float b) => new YuoFloat3(a.x / b, a.y / b, a.z / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator +(YuoFloat3 a, float b) => new YuoFloat3(a.x + b, a.y + b, a.z + b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoFloat3 operator -(YuoFloat3 a, float b) => new YuoFloat3(a.x - b, a.y - b, a.z - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt3(YuoFloat3 a) => new((int)a.x, (int)a.y, (int)a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat2(YuoFloat3 a) => new(a.x, a.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat3(YuoFloat2 a) => new(a.x, a.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3(YuoFloat3 a) => new(a.x, a.y, a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat3(Vector3 a) => new(a.x, a.y, a.z);

        public static readonly YuoFloat3
            Right = new(1, 0, 0),
            Left = new(-1, 0, 0),
            Up = new(0, 1, 0),
            Down = new(0, -1, 0),
            Forward = new(0, 0, 1),
            Backward = new(0, 0, -1),
            Zero = new(0, 0, 0);


        // 双分量重组
        public YuoFloat2 xx => new(x, x);
        public YuoFloat2 xy => new(x, y);
        public YuoFloat2 xz => new(x, z);
        public YuoFloat2 yx => new(y, x);
        public YuoFloat2 yy => new(y, y);
        public YuoFloat2 yz => new(y, z);
        public YuoFloat2 zx => new(z, x);
        public YuoFloat2 zy => new(z, y);
        public YuoFloat2 zz => new(z, z);

        // 三分量重组
        public YuoFloat3 xxx => new(x, x, x);
        public YuoFloat3 xxy => new(x, x, y);
        public YuoFloat3 xxz => new(x, x, z);
        public YuoFloat3 xyx => new(x, y, x);
        public YuoFloat3 xyy => new(x, y, y);
        public YuoFloat3 xyz => new(x, y, z);
        public YuoFloat3 xzx => new(x, z, x);
        public YuoFloat3 xzy => new(x, z, y);
        public YuoFloat3 xzz => new(x, z, z);
        public YuoFloat3 yxx => new(y, x, x);
        public YuoFloat3 yxy => new(y, x, y);
        public YuoFloat3 yxz => new(y, x, z);
        public YuoFloat3 yyx => new(y, y, x);
        public YuoFloat3 yyy => new(y, y, y);
        public YuoFloat3 yyz => new(y, y, z);
        public YuoFloat3 yzx => new(y, z, x);
        public YuoFloat3 yzy => new(y, z, y);
        public YuoFloat3 yzz => new(y, z, z);
        public YuoFloat3 zxx => new(z, x, x);
        public YuoFloat3 zxy => new(z, x, y);
        public YuoFloat3 zxz => new(z, x, z);
        public YuoFloat3 zyx => new(z, y, x);
        public YuoFloat3 zyy => new(z, y, y);
        public YuoFloat3 zyz => new(z, y, z);
        public YuoFloat3 zzx => new(z, z, x);
        public YuoFloat3 zzy => new(z, z, y);
        public YuoFloat3 zzz => new(z, z, z);
    }
}