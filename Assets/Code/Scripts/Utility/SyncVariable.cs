using System.Collections;

public class SyncVariable<T>
{
    public delegate void ValueChangedCallback(T previous, T next);
    public ValueChangedCallback OnValueChanged;

    public SyncVariable(T value)
    {
        this.inner = value;
    }

    private T inner;
    public T Value
    {
        get => inner;
        set
        {
            if (OnValueChanged != null) OnValueChanged(inner, value);
            inner = value;
        }
    }

    public void Sync(T value)
    {
        this.inner = value;
    }
}
