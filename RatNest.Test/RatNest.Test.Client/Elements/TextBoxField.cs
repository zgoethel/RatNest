namespace RatNest.Test.Client.Elements;

public class TextBoxField : FormElementBase
{
    private readonly LogicRuleSet logicRules;

    private INamedValue peerValue;

    public TextBoxField(IFormRegion region) : base(region)
    {
        logicRules = new(State);
    }

    public NamedValue<string> Value { get; private set; }

    public bool ValuesSame { get; set; }

    public bool IsBlank => string.IsNullOrEmpty(Value.Value);

    private bool prevIsEffectivelyBlank = false;
    public bool IsEffectivelyBlank => IsHidden || !Parent.IsEffectivelyVisible();

    public string ValidationMessage => "" switch
    {
        _ when IsInvalid => logicRules.ValidationMessages.FirstOrDefault("Invalid"),
        _ when IsRequired && IsBlank => "Required",
        _ => ""
    };

    public override void Create()
    {
        Value = new(Parent.NamingContext?.GetUniqueName("field") ?? "field");
        AddNamedValue(Value);
    }

    public override async Task Initialize()
    {
        (_, peerValue) = Parent
            .NamingContext
            .ValuesByName
            .Single((it) => it.Key != Value.Name);

        logicRules.PauseStateUpdates = true;
        {
            var rule = logicRules.CreateLogicRule((values, accum, emitMessage) =>
            {
                return FormElementState.Required;
            });
        }
        {
            var rule = logicRules.CreateLogicRule((values, accum, emitMessage) =>
            {
                if (values.GetSelected(0)?.Value?.Equals("Invalid") == true)
                {
                    return FormElementState.Invalid;
                }

                if (values.GetSelected(0)?.Value?.Equals("Special Invalid") == true)
                {
                    emitMessage("Hello, world!");

                    return FormElementState.Invalid;
                }

                return FormElementState.None;
            });
            await rule.SetSelectedValues(Value);
        }
        {
            var rule = logicRules.CreateLogicRule((values, accum, emitMessage) =>
            {
                if (values.GetSelected(0)?.Value?.Equals("Disabled") == true)
                {
                    return FormElementState.Disabled;
                }
                return FormElementState.None;
            });
            await rule.SetSelectedValues(peerValue);
        }
        {
            var rule = logicRules.CreateLogicRule((values, accum, emitMessage) =>
            {
                if (values.GetSelected(0)?.Value?.Equals("Hidden") == true)
                {
                    return FormElementState.Hidden;
                }
                return FormElementState.None;
            });
            await rule.SetSelectedValues(peerValue);
        }
        logicRules.PauseStateUpdates = false;

        logicRules.StateChanged += async () =>
        {
            await SetState(logicRules.State);

            if (!prevIsEffectivelyBlank && IsEffectivelyBlank)
            {
                await Value.SetValue("");
            }
            prevIsEffectivelyBlank = IsEffectivelyBlank;
        };
        await logicRules.EvaluateRules();

        Parent.IsVisibleChanged += async () =>
        {
            if (!prevIsEffectivelyBlank && IsEffectivelyBlank)
            {
                await Value.SetValue("");
            }
            prevIsEffectivelyBlank = IsEffectivelyBlank;
        };
    }
}
