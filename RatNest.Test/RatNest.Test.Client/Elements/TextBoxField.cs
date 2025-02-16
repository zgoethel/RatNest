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
            var eval = LogicRule.ForState(FormElementState.Required);
            var rule = logicRules.CreateLogicRule(eval);
        }
        {
            var eval = LogicRule
                .ForState(FormElementState.Invalid)
                .WhenEquals(0, "Invalid");
            var rule = logicRules.CreateLogicRule(eval);

            await rule.SetSelectedValues(Value);
        }
        {
            var eval = LogicRule
                .ForValidation("Hello, world!")
                .WhenEquals(0, "Special Invalid");
            var rule = logicRules.CreateLogicRule(eval);

            await rule.SetSelectedValues(Value);
        }
        {
            var eval = LogicRule
                .ForState(FormElementState.Disabled)
                .WhenEquals(0, "Disabled");
            var rule = logicRules.CreateLogicRule(eval);

            await rule.SetSelectedValues(peerValue);
        }
        {
            var eval = LogicRule
                .ForState(FormElementState.Hidden)
                .WhenEquals(0, "Hidden");
            var rule = logicRules.CreateLogicRule(eval);

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
