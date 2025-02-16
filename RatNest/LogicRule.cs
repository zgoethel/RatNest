namespace RatNest;

public class LogicRule
{
    public delegate FormElementState EvaluatorFunc(INamedValue[] selectedValues, FormElementState accumulated);

    public LogicRule(EvaluatorFunc evaluator)
    {
        Evaluator = evaluator;
    }

    public EvaluatorFunc Evaluator { get; private set; }

    public FormElementState Evaluate(FormElementState accumulated)
    {
        return Evaluator(SelectedValues, accumulated);
    }

    public INamedValue[] SelectedValues { get; private set; } = Array.Empty<INamedValue>();

    public event Func<Task> SelectedValuesChanged;

    public async Task InvokeSelectedValuesChanged()
    {
        await SelectedValuesChanged.InvokeHandler();
    }

    public async Task SetSelectedValues(params INamedValue[] values)
    {
        SelectedValues = values;

        await InvokeSelectedValuesChanged();
    }
}