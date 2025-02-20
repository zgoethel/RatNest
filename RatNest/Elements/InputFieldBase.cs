using RatNest.Logic;

namespace RatNest.Elements;

public abstract class InputFieldBase<T> : FormElementBase
{
    private readonly LogicRuleSet logicRules;
    private readonly bool resetOnHidden;

    public InputFieldBase(IFormRegion region, bool resetOnHidden = true) : base(region)
    {
        logicRules = new(State);
        this.resetOnHidden = resetOnHidden;
    }

    public abstract string DefaultNamePrefix { get; }

    public NamedValue<T> Value { get; private set; }

    public abstract bool IsBlank { get; }

    public abstract T BlankValue { get; }

    private bool prevIsEffectivelyBlank = false;
    public bool IsEffectivelyBlank => IsHidden || !Parent.IsEffectivelyVisible();

    private string prevValidationMessage = "";
    public string ValidationMessage => "" switch
    {
        _ when IsInvalid => logicRules.ValidationMessages.FirstOrDefault("Invalid"),
        _ when IsRequired && IsBlank => "Required",
        _ => ""
    };

    public override void Create()
    {
        Value = new(Parent.NamingContext?.GetUniqueName(DefaultNamePrefix) ?? DefaultNamePrefix, value: BlankValue);
        AddNamedValue(Value);
    }

    public async Task ConfigureLogic(Func<LogicRuleSet, Task> config, bool clearRules = true)
    {
        logicRules.PauseStateUpdates = true;
        if (clearRules)
        {
            await logicRules.ClearLogicRules();
        }

        await config(logicRules);

        logicRules.PauseStateUpdates = false;
    }

    public override async Task Initialize()
    {
        logicRules.StateChanged += async () =>
        {
            var forceRedraw = prevValidationMessage != ValidationMessage;
            prevValidationMessage = ValidationMessage;

            await SetState(logicRules.State, forceRedraw: forceRedraw);

            await CheckIfBecameHidden();
        };
        await logicRules.EvaluateRules();

        Parent.IsVisibleChanged += CheckIfBecameHidden;
    }

    private async Task CheckIfBecameHidden()
    {
        if (!prevIsEffectivelyBlank && IsEffectivelyBlank && resetOnHidden)
        {
            await Value.SetValue(BlankValue);
        }
        prevIsEffectivelyBlank = IsEffectivelyBlank;
    }

    public async Task SetValue(T value)
    {
        if (resetOnHidden && IsEffectivelyBlank)
        {
            return;
        }

        await Value.SetValue(value);

        await InvokeStateChanged();
    }
}
