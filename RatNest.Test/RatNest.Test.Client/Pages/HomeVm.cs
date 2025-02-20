using RatNest.Elements;
using RatNest.Logic;

namespace RatNest.Test.Client.Pages;

public class HomeVm
{
    public FormRegion TopLevel { get; private set; }

    public FormRegion LeftArea { get; private set; }

    public FormRegion RightArea { get; private set; }

    public TextBoxField LeftField { get; private set; }

    public TextBoxField RightField { get; private set; }

    private void CreateElements()
    {
        TopLevel = new(null, topLevel: true);
        LeftArea = new(TopLevel);
        RightArea = new(TopLevel);

        LeftField = new(LeftArea, initialValue: "Hello, world!");
        RightField = new(RightArea, initialValue: "Foo bar");
    }

    private async Task ConfigureLogic(LogicRuleSet logicRules, TextBoxField field, TextBoxField other)
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

    public async Task Initialize()
    {
        CreateElements();

        await LeftField.ConfigureLogic(async (it) => await ConfigureLogic(it, LeftField, RightField));
        await RightField.ConfigureLogic(async (it) => await ConfigureLogic(it, RightField, LeftField));

        await TopLevel.Initialize();
    }
}
