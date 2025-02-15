namespace RatNest.Test.Client;

public class TextBoxField : FormElementBase
{
    private static int serial = 0;

    private INamedValue peerValue;

    public NamedValue<string> Value { get; private set; } = new($"field{serial++}");

    public TextBoxField(IFormRegion region) : base(region)
    {
        AddNamedValue(Value);
    }

    public async Task Initialize()
    {
        //TODO Look up value by name or reference ID
        peerValue = Parent
            .GetGlobalValues()
            .Single((it) => it.Name != Value.Name);

        Value.ValueChanged += RecalculateState;
        peerValue.ValueChanged += RecalculateState;

        await RecalculateState();
    }

    public bool ValuesSame { get; set; }

    private async Task RecalculateState()
    {
        var oldValuesSame = ValuesSame;
        ValuesSame = (peerValue.Value as string) == Value.Value as string;

        if (oldValuesSame != ValuesSame)
        {
            await InvokeStateChanged();
        }
    }
}
