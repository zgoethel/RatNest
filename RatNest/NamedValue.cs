namespace RatNest;

public interface INamedValue
{
    string Name { get; set; }

    Guid RefId { get; }

    object Value { get; }

    Task SetValue(object value);

    event Func<Task> ValueChanged;
}

public class NamedValue<T> : INamedValue
{
    public NamedValue(string name, Guid? refId = null, T value = default)
    {
        refId ??= Guid.NewGuid();

        Name = name;
        RefId = refId.Value;
        Value = value;
    }

    public string Name { get; set; }

    public Guid RefId { get; private set; }

    public T Value { get; private set; }

    public async Task SetValue(T value)
    {
        Value = value;

        await ValueChanged.InvokeHandler();
    }

    object INamedValue.Value => Value;

    async Task INamedValue.SetValue(object value)
    {
        await SetValue((T)value);
    }

    public event Func<Task> ValueChanged;
}