﻿namespace RatNest;

public static class Extensions
{
    public static async Task InvokeHandler(this Func<Task> handler)
    {
        if (handler is null)
        {
            return;
        }
        foreach (Func<Task> func in handler
            .GetInvocationList()
            .Cast<Func<Task>>())
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
        foreach (Func<T, Task> func in handler
            .GetInvocationList()
            .Cast<Func<T, Task>>())
        {
            await func(value);
        }
    }

    public static INamedValue GetSelected(this INamedValue[] values, int index)
    {
        if (index >= values.Length)
        {
            return null;
        }
        return values[index];
    }
}
