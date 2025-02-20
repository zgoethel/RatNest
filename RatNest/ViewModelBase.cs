namespace RatNest;

public abstract class ViewModelBase
{
    public event Func<Task> StateChanged;

    protected async Task InvokeStateChanged()
    {
        await StateChanged.InvokeHandler();
    }

    public bool Loading { get; private set; } = true;

    public FormRegion TopLevel { get; } = new(null, topLevel: true);

    protected abstract void CreateElements();

    protected abstract Task ConfigureLogic();

    public virtual async Task Initialize()
    {
        try
        {
            CreateElements();

            await ConfigureLogic();

            await TopLevel.Initialize();
        } finally
        {
            Loading = false;
            await InvokeStateChanged();
        }
    }
}
