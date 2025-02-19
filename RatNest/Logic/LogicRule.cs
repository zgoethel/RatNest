namespace RatNest.Logic;

public class LogicRule
{
    public delegate FormElementState EvaluatorFunc(INamedValue[] selectedValues, FormElementState accumulated, Action<string> emitMessage);

    public LogicRule(EvaluatorFunc evaluator)
    {
        Evaluator = evaluator;
    }

    public EvaluatorFunc Evaluator { get; private set; }

    public FormElementState Evaluate(FormElementState accumulated, Action<string> emitMessage)
    {
        return Evaluator(SelectedValues, accumulated, emitMessage);
    }

    public INamedValue[] SelectedValues { get; private set; } = Array.Empty<INamedValue>();

    public async Task SetSelectedValues(params INamedValue[] values)
    {
        SelectedValues = values;

        await InvokeSelectedValuesChanged();
    }

    public event Func<Task> SelectedValuesChanged;

    public async Task InvokeSelectedValuesChanged()
    {
        await SelectedValuesChanged.InvokeHandler();
    }

    public static EvaluatorFunc ForState(FormElementState state)
    {
        return (_, _, _) =>
        {
            return state;
        };
    }

    public static EvaluatorFunc ForValidation(string message)
    {
        return (_, _, emitMessage) =>
        {
            emitMessage(message);

            return FormElementState.Invalid;
        };
    }
}

public static class LogicRuleExtensions
{
    public static LogicRule.EvaluatorFunc WhenEquals(this LogicRule.EvaluatorFunc evaluator, int index, int secondIndex, bool not = false)
    {
        return (selectedValues, accumulated, emitMessage) =>
        {
            if (selectedValues.GetSelected(index) is null
                || selectedValues.GetSelected(secondIndex) is null)
            {
                return FormElementState.None;
            }

            var value = selectedValues.GetSelected(index);
            var secondValue = selectedValues.GetSelected(secondIndex);

            if (not ^ object.Equals(value.Value, secondValue.Value))
            {
                return evaluator(selectedValues, accumulated, emitMessage);
            }

            return FormElementState.None;
        };
    }

    public static LogicRule.EvaluatorFunc WhenEquals(this LogicRule.EvaluatorFunc evaluator, int index, object staticValue, bool not = false)
    {
        return (selectedValues, accumulated, emitMessage) =>
        {
            if (selectedValues.GetSelected(index) is null)
            {
                return FormElementState.None;
            }

            var value = selectedValues.GetSelected(index);

            if (not ^ object.Equals(value.Value, staticValue))
            {
                return evaluator(selectedValues, accumulated, emitMessage);
            }

            return FormElementState.None;
        };
    }
}