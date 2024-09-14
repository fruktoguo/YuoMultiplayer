using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YuoTools.Extend.MathFunction
{
    public struct YuoInt3 : IEquatable<YuoInt3>, IFormattable
    {
        public int x, y, z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YuoInt3(int x, int y, int z) => (this.x, this.y, this.z) = (x, y, z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(YuoInt3 other) => x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider) =>
            $"({x.ToString(format, formatProvider)},{y.ToString(format, formatProvider)},{z.ToString(format, formatProvider)})";

        public float Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Mathf.Sqrt(x * x + y * y + z * z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(YuoInt3 a, YuoInt3 b) => (a - b).Magnitude;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator +(YuoInt3 a, YuoInt3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator -(YuoInt3 a, YuoInt3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator *(YuoInt3 a, int b) => new(a.x * b, a.y * b, a.z * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator /(YuoInt3 a, int b) => new(a.x / b, a.y / b, a.z / b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator +(YuoInt3 a, int b) => new(a.x + b, a.y + b, a.z + b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static YuoInt3 operator -(YuoInt3 a, int b) => new(a.x - b, a.y - b, a.z - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoFloat3(YuoInt3 a) => new(a.x, a.y, a.z);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt2(YuoInt3 a) => new(a.x, a.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt3(YuoInt2 a) => new(a.x, a.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3(YuoInt3 a) => new(a.x, a.y, a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator YuoInt3(Vector3 a) => new((int)a.x, (int)a.y, (int)a.z);

        public static readonly YuoInt3
            Right = new(1, 0, 0),
            Left = new(-1, 0, 0),
            Up = new(0, 1, 0),
            Down = new(0, -1, 0),
            Forward = new(0, 0, 1),
            Backward = new(0, 0, -1),
            Zero = new(0, 0, 0),
            One = new(1, 1, 1);
        
        
        // 双分量重组
        public YuoInt2 xx => new(x, x);
        public YuoInt2 xy => new(x, y);
        public YuoInt2 xz => new(x, z);
        public YuoInt2 yx => new(y, x);
        public YuoInt2 yy => new(y, y);
        public YuoInt2 yz => new(y, z);
        public YuoInt2 zx => new(z, x);
        public YuoInt2 zy => new(z, y);
        public YuoInt2 zz => new(z, z);

        // 三分量重组
        public YuoInt3 xxx => new(x, x, x);
        public YuoInt3 xxy => new(x, x, y);
        public YuoInt3 xxz => new(x, x, z);
        public YuoInt3 xyx => new(x, y, x);
        public YuoInt3 xyy => new(x, y, y);
        public YuoInt3 xyz => new(x, y, z);
        public YuoInt3 xzx => new(x, z, x);
        public YuoInt3 xzy => new(x, z, y);
        public YuoInt3 xzz => new(x, z, z);
        public YuoInt3 yxx => new(y, x, x);
        public YuoInt3 yxy => new(y, x, y);
        public YuoInt3 yxz => new(y, x, z);
        public YuoInt3 yyx => new(y, y, x);
        public YuoInt3 yyy => new(y, y, y);
        public YuoInt3 yyz => new(y, y, z);
        public YuoInt3 yzx => new(y, z, x);
        public YuoInt3 yzy => new(y, z, y);
        public YuoInt3 yzz => new(y, z, z);
        public YuoInt3 zxx => new(z, x, x);
        public YuoInt3 zxy => new(z, x, y);
        public YuoInt3 zxz => new(z, x, z);
        public YuoInt3 zyx => new(z, y, x);
        public YuoInt3 zyy => new(z, y, y);
        public YuoInt3 zyz => new(z, y, z);
        public YuoInt3 zzx => new(z, z, x);
        public YuoInt3 zzy => new(z, z, y);
        public YuoInt3 zzz => new(z, z, z);
    }
}