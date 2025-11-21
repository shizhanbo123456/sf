using System;

public class RegistableVariable<T>
{
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
}