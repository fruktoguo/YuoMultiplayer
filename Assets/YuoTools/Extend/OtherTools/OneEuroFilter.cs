using UnityEngine;

/// <summary>
/// 这个类是一个通用的One Euro Filter。它可以处理任何类型的输入，只要提供合适的距离计算和插值函数。
/// </summary>
public class OneEuroFilter<T>
{
    /// <summary>
    /// 过滤后的值。这个值可以直接使用。
    /// </summary>
    public T FilteredValue { get; private set; }

    /// <summary>
    /// 过滤器的最小截止频率。这个值越大，过滤的结果越平滑，但是延迟越大。
    /// </summary>
    public float MinimumCutOff = 0.1f;

    /// <summary>
    /// 过滤器的beta值。这个值越大，过滤的结果对快速变化的输入越敏感。
    /// </summary>
    public float Beta = 100;

    /// <summary>
    /// 导数滤波器的截止频率。这个值越大，过滤的结果对输入的变化速度越敏感。
    /// </summary>
    public float DerivativeCutOff = 4f;

    /// <summary>
    /// 这个变量用于记录是否是第一次更新过滤器。
    /// </summary>
    public bool IsFirstUpdate;

    /// <summary>
    /// 用于获取低通过滤后的增量速度以辅助调整 dcCutOff
    /// </summary>
    public float DeltaSpeed { get; private set; }

    /// <summary>
    /// 这个变量是一个低通滤波器，用于过滤输入值。
    /// </summary>
    private LowPassFilterBase<T> inputFilter;

    /// <summary>
    /// 这个变量是一个低通滤波器，用于过滤输入值的导数。
    /// </summary>
    private LowPassFilterBase<T> derivativeFilter;

    /// <summary>
    /// 这个是过滤器的构造函数。在构造函数中，我们创建了两个低通滤波器，并设置IsFirstUpdate为true。
    /// </summary>
    public OneEuroFilter(LowPassFilterBase<T> inputFilterBase, LowPassFilterBase<T> derivativeFilterBase)
    {
        inputFilter = inputFilterBase;
        derivativeFilter = derivativeFilterBase;
        IsFirstUpdate = true;
    }

    public OneEuroFilter()
    {
        IsFirstUpdate = true;
        switch (FilteredValue)
        {
            case float:
                inputFilter = new FloatLowPassFilter() as LowPassFilterBase<T>;
                derivativeFilter = new FloatLowPassFilter() as LowPassFilterBase<T>;
                break;
            case Vector3:
                inputFilter = new Vector3LowPassFilter() as LowPassFilterBase<T>;
                derivativeFilter = new Vector3LowPassFilter() as LowPassFilterBase<T>;
                break;
            case Vector2:
                inputFilter = new Vector2LowPassFilter() as LowPassFilterBase<T>;
                derivativeFilter = new Vector2LowPassFilter() as LowPassFilterBase<T>;
                break;
            case Quaternion:
                inputFilter = new QuaternionLowPassFilter() as LowPassFilterBase<T>;
                derivativeFilter = new QuaternionLowPassFilter() as LowPassFilterBase<T>;
                break;
            default:
                Debug.LogError("Unknown type of FilteredValue");
                break;
        }
    }

    /// <summary>
    /// 这个函数是输入的步进函数。它根据输入的新值和时间间隔，计算过滤后的值。
    /// </summary>
    /// <param name="newValue">新的输入值</param>
    /// <param name="deltaTime">时间间隔</param>
    /// <returns>过滤后的值</returns>
    public T Step(T newValue, float deltaTime)
    {
        if (deltaTime > 0)
        {
            var moveDx = inputFilter.GetMovement(newValue, inputFilter.PreviousValue);

            var edx = derivativeFilter.Filter(moveDx, GetAlpha(deltaTime, DerivativeCutOff));

            DeltaSpeed = derivativeFilter.GetMagnitude(edx);

            float cutoff = MinimumCutOff + Beta * Mathf.Abs(IsFirstUpdate ? 0 : DeltaSpeed);

            FilteredValue = inputFilter.Filter(newValue, GetAlpha(deltaTime, cutoff));

            IsFirstUpdate = false;
        }

        return FilteredValue;
    }

    /// <summary>
    /// 这个函数用于重置过滤器。它会将过滤值和上一次的值都设置为默认值，并将IsFirstUpdate设置为true。
    /// </summary>
    public void Reset()
    {
        FilteredValue = default;
        inputFilter.Reset();
        derivativeFilter.Reset();
        IsFirstUpdate = true;
    }

    /// <summary>
    /// 这个函数用于设置过滤器的参数。
    /// </summary>
    /// <param name="minCutOff">最小截止频率</param>
    /// <param name="beta">beta值</param>
    /// <param name="dcCutOff">导数滤波器的截止频率</param>
    public void SetParameter(float minCutOff, float beta, float dcCutOff)
    {
        this.MinimumCutOff = minCutOff;
        this.Beta = beta;
        this.DerivativeCutOff = dcCutOff;
    }

    public void SetParameter(Vector3 parameter) => SetParameter(parameter.x, parameter.y, parameter.z);

    /// <summary>
    /// 这个函数用于计算过滤器的alpha值。alpha值用于在过滤器中平衡新值和旧值的权重。
    /// </summary>
    /// <param name="dt">频率</param>
    /// <param name="cutoff">截止频率</param>
    /// <returns>alpha值</returns>
    private float GetAlpha(float dt, float cutoff)
    {
        float tau = 1f / (2.0f * Mathf.PI * cutoff);
        return 1 - Mathf.Exp(-0.69314718056f * dt / tau);
    }

    /// <summary>
    /// 这是一个手动设置的适用于XR的手部滤波器的参数。
    /// </summary>
    public readonly Vector3 XRHandParameter = new(0.1f, 50, 4);
}

public class FloatLowPassFilter : LowPassFilterBase<float>
{
    public override float Lerp(float a, float b, float t)
    {
        return Mathf.Lerp(a, b, t);
    }

    public override float Distance(float a, float b)
    {
        return Mathf.Abs(a - b);
    }

    public override float GetMovement(float a, float b)
    {
        return a - b;
    }

    public override float GetMagnitude(float a)
    {
        return Mathf.Abs(a);
    }
}

public class Vector3LowPassFilter : LowPassFilterBase<Vector3>
{
    public override Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return Vector3.Lerp(a, b, t);
    }

    public override float Distance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    public override Vector3 GetMovement(Vector3 a, Vector3 b)
    {
        return a - b;
    }

    public override float GetMagnitude(Vector3 a)
    {
        return a.magnitude;
    }
}

public class Vector2LowPassFilter : LowPassFilterBase<Vector2>
{
    public override Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, t);
    }

    public override float Distance(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b);
    }

    public override Vector2 GetMovement(Vector2 a, Vector2 b)
    {
        return a - b;
    }

    public override float GetMagnitude(Vector2 a)
    {
        return a.magnitude;
    }
}

public class QuaternionLowPassFilter : LowPassFilterBase<Quaternion>
{
    public override Quaternion Lerp(Quaternion a, Quaternion b, float t)
    {
        return Quaternion.Slerp(a, b, t);
    }

    public override float Distance(Quaternion a, Quaternion b)
    {
        return Quaternion.Angle(a, b);
    }

    public override Quaternion GetMovement(Quaternion a, Quaternion b)
    {
        return a * Quaternion.Inverse(b);
    }

    public override float GetMagnitude(Quaternion a)
    {
        return Quaternion.Angle(Quaternion.identity, a);
    }
}


public abstract class LowPassFilterBase<T>
{
    /// <summary>
    /// 过滤后的值。这个值可以直接使用。
    /// </summary>
    public T PreviousValue => previousFilteredValue;

    /// <summary>
    /// 这个变量用于记录是否是第一次更新过滤器。
    /// </summary>
    private bool isFirstUpdate = true;

    /// <summary>
    /// 这些变量用于存储过滤后的值和上一次的值。
    /// </summary>
    private T filteredValue;

    private T previousFilteredValue;

    /// <summary>
    /// 这个函数用于重置过滤器。它会将过滤值和上一次的值都设置为默认值，并将IsFirstUpdate设置为true。
    /// </summary>
    public void Reset()
    {
        isFirstUpdate = true;
        filteredValue = previousFilteredValue = default;
    }

    /// <summary>
    /// 这个函数是输入的滤波函数。它根据输入的新值和alpha值，计算过滤后的值。
    /// </summary>
    /// <param name="x">输入值</param>
    /// <param name="alpha">alpha值</param>
    /// <returns>过滤后的值</returns>
    public T Filter(T x, float alpha)
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            previousFilteredValue = x;
        }

        filteredValue = Lerp(previousFilteredValue, x, alpha);
        previousFilteredValue = filteredValue;
        return filteredValue;
    }

    public abstract T Lerp(T a, T b, float t);

    public abstract float Distance(T a, T b);

    public abstract T GetMovement(T a, T b);

    public abstract float GetMagnitude(T a);
}