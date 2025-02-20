namespace RatNest;

public abstract class ViewModelBase
{
    public event Func<Task> StateChanged;

    protected async Task InvokeStateChanged()
    {
        await StateChanged.InvokeHandler();
    }
}
