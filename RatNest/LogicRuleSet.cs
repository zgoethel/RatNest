namespace RatNest;

public class LogicRuleSet
{
    private List<INamedValue> prevUniqueValues = new();

    public FormElementState State { get; private set; } = FormElementState.None;

    public event Func<Task> StateChanged;

    private async Task InvokeStateChanged()
    {
        await StateChanged.InvokeHandler();
    }

    private readonly List<LogicRule> rules = new();
    public IReadOnlyList<LogicRule> Rules => rules;

    public async Task EvaluateRules()
    {
        var state = FormElementState.None;
        foreach (var rule in Rules)
        {
            state |= rule.Evaluate(state);
        }

        if (state != State)
        {
            State = state;

            await InvokeStateChanged();
        }
    }

    private async Task UpdateSubscriptions()
    {
        var currUniqueValues = Rules
            .SelectMany((it) => it.SelectedValues)
            .DistinctBy((it) => it.RefId)
            .ToList();

        var unsubscribe = prevUniqueValues.Where((it) => !currUniqueValues.Any((_it) => it.RefId == it.RefId));
        var subscribe = currUniqueValues.Where((it) => !prevUniqueValues.Any((_it) => it.RefId == it.RefId));

        foreach (var value in unsubscribe)
        {
            value.ValueChanged -= EvaluateRules;
        }

        foreach (var value in subscribe)
        {
            value.ValueChanged += EvaluateRules;
        }

        prevUniqueValues = currUniqueValues;

        await EvaluateRules();
    }

    public LogicRule CreateLogicRule(LogicRule.EvaluatorFunc evaluator)
    {
        var rule = new LogicRule(evaluator);
        rule.SelectedValuesChanged += UpdateSubscriptions;

        rules.Add(rule);

        return rule;
    }
}
