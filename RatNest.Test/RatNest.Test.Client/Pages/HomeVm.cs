using RatNest.Elements;
using RatNest.Logic;

namespace RatNest.Test.Client.Pages;

public class HomeVm : ViewModelBase
{
    public FormRegion LeftArea { get; private set; }
    public FormRegion RightArea { get; private set; }

    public CheckBoxField LeftCheck { get; private set; }
    public CheckBoxField RightCheck { get; private set; }

    public TextBoxField LeftField { get; private set; }
    public TextBoxField RightField { get; private set; }

    public override async Task Initialize()
    {
        await base.Initialize();

        await LeftField.SetValue("Hello, world!");
        await RightField.SetValue("Foo bar");
    }

    protected override void CreateElements()
    {
        LeftArea = new(TopLevel);
        RightArea = new(TopLevel);

        LeftCheck = new(TopLevel, initialValue: true);
        LeftCheck.Value.Rename("Set IsVisible directly");
        RightCheck = new(TopLevel, initialValue: true);
        RightCheck.Value.Rename("Toggle Hidden flag in State");

        LeftField = new(LeftArea, initialValue: "");
        RightField = new(RightArea, initialValue: "");
    }

    protected override async Task ConfigureLogic()
    {
        LeftCheck.Value.ValueChanged += async () =>
        {
            await LeftArea.SetIsVisible(LeftCheck.Value.Value);
        };

        RightCheck.Value.ValueChanged += async () =>
        {
            await RightArea.SetState(RightCheck.Value.Value
                ? FormElementState.None
                : FormElementState.Hidden);
        };

        await LeftField.ConfigureLogic(async (it) => await ConfigureLogic(it, LeftField, RightField));
        await RightField.ConfigureLogic(async (it) => await ConfigureLogic(it, RightField, LeftField));
    }

    private static async Task ConfigureLogic(LogicRuleSet logicRules, TextBoxField field, TextBoxField other)
    {
        {
            var eval = LogicRule.ForState(FormElementState.Required);
            logicRules.CreateLogicRule(eval);
        }
        {
            var eval = LogicRule
                .ForState(FormElementState.Invalid)
                .WhenEquals(0, "Invalid");
            var rule = logicRules.CreateLogicRule(eval);

            await rule.SetSelectedValues(field.Value);
        }
        {
            var eval = LogicRule
                .ForValidation("Fields can't equal each other")
                .WhenEquals(0, 1);
            var rule = logicRules.CreateLogicRule(eval);

            await rule.SetSelectedValues(field.Value, other.Value);
        }
        {
            var eval = LogicRule
                .ForValidation("Hello, world!")
                .WhenEquals(0, "Special Invalid");
            var rule = logicRules.CreateLogicRule(eval);

            await rule.SetSelectedValues(field.Value);
        }
        {
            var eval = LogicRule
                .ForState(FormElementState.Disabled)
                .WhenEquals(0, "Disabled");
            var rule = logicRules.CreateLogicRule(eval);

            await rule.SetSelectedValues(other.Value);
        }
        {
            var eval = LogicRule
                .ForState(FormElementState.Hidden)
                .WhenEquals(0, "Hidden");
            var rule = logicRules.CreateLogicRule(eval);

            await rule.SetSelectedValues(other.Value);
        }
    }
}
