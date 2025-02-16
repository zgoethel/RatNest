namespace RatNest;

public class LogicRuleSet
{
    private List<INamedValue> prevUniqueValues = new();

    public LogicRuleSet(FormElementState initialState = FormElementState.None)
    {
        State = initialState;
    }

    public FormElementState State { get; private set; } = FormElementState.None;

    public event Func<Task> StateChanged;

    private List<string> validationMessages = new();
    public IReadOnlyList<string> ValidationMessages => validationMessages;

    private async Task InvokeStateChanged()
    {
        await StateChanged.InvokeHandler();
    }

    public bool PauseStateUpdates { get; set; } = false;

    private readonly List<LogicRule> rules = new();
    public IReadOnlyList<LogicRule> Rules => rules;

    public async Task EvaluateRules()
    {
        if (PauseStateUpdates)
        {
            return;
        }

        var state = FormElementState.None;
        validationMessages.Clear();

        foreach (var rule in Rules)
        {
            state |= rule.Evaluate(state, validationMessages.Add);
        }

        if (state.HasFlag(FormElementState.Disabled)
            || state.HasFlag(FormElementState.Hidden))
        {
            if (state.HasFlag(FormElementState.Required))
            {
                state -= FormElementState.Required;
            }
            if (state.HasFlag(FormElementState.Invalid))
            {
                state -= FormElementState.Invalid;
            }
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
            .Where((it) => it is not null)
            .DistinctBy((it) => it.RefId)
            .ToList();

        var unsubscribe = prevUniqueValues.Where((it) => !currUniqueValues.Contains(it));
        var subscribe = currUniqueValues.Where((it) => !prevUniqueValues.Contains(it));

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
