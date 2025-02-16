﻿namespace RatNest;

public abstract class FormElementBase
{
    public FormElementBase(IFormRegion parent)
    {
        Parent = parent;

        Create();

        Parent?.AddElement(this);
    }

    public abstract void Create();

    public abstract Task Initialize();

    public IFormRegion Parent { get; private set; }

    private readonly List<INamedValue> values = new();
    public IReadOnlyList<INamedValue> Values => values;

    public event Func<Task> ValueChanged;

    private async Task InvokeValueChanged()
    {
        await ValueChanged.InvokeHandler();
    }

    protected void AddNamedValue(INamedValue value)
    {
        values.Add(value);
        value.ValueChanged += InvokeValueChanged;
    }

    public FormElementState State { get; private set; } = FormElementState.None;

    public async Task SetState(FormElementState state)
    {
        State = state;

        await InvokeStateChanged();
    }

    public bool IsDisabled => State.HasFlag(FormElementState.Disabled);
    public bool IsHidden => State.HasFlag(FormElementState.Hidden);
    public bool IsInvalid => State.HasFlag(FormElementState.Invalid);
    public bool IsRequired => State.HasFlag(FormElementState.Required);

    public event Func<Task> StateChanged;

    public async Task InvokeStateChanged()
    {
        await StateChanged.InvokeHandler();
    }
}