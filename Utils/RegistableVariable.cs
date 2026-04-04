using System;
using UnityEngine.Pool;

public class RegistableVariable<T>
{
    private static ObjectPool<RegistableVariable<T>> pool = new(
        createFunc: () => new RegistableVariable<T>(default),
        actionOnRelease: v => v.OnValueChanged = null,
        actionOnGet: v => v.value = default
        );
    private T value;
    public T Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
            OnValueChanged?.Invoke(this.value);
        }
    }
    public Action<T> OnValueChanged;
    public RegistableVariable(T value)
    {
        this.value = value;
    }
    public static RegistableVariable<T> Get(T value = default)
    {
        var v=pool.Get();
        v.value = value;
        return v;
    }
    public static void Release(RegistableVariable<T> v)=>pool.Release(v);
    public override string ToString()
    {
        return value.ToString();
    }
}