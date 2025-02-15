namespace RatNest;

public static class Extensions
{
    public static async Task InvokeHandler(this Func<Task> handler)
    {
        if (handler is null)
        {
            return;
        }
        foreach (Func<Task> func in handler.GetInvocationList())
        {
            await func();
        }
    }

    public static async Task InvokeHandler<T>(this Func<T, Task> handler, T value)
    {
        if (handler is null)
        {
            return;
        }
        foreach (Func<T, Task> func in handler.GetInvocationList())
        {
            await func(value);
        }
    }
}
