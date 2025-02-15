namespace RatNest.Test.Client.Elements;

public class TextBoxField : FormElementBase
{
    private static int serial = 0;

    private INamedValue peerValue;

    public NamedValue<string> Value { get; private set; } = new($"field{serial++}");

    public TextBoxField(IFormRegion region) : base(region)
    {
    }

    public override void Create()
    {
        AddNamedValue(Value);
    }

    public async Task Initialize()
    {
        (_, peerValue) = Parent
            .NamingContext
            .ValuesByName
            .Single((it) => it.Key != Value.Name);

        Value.ValueChanged += RecalculateState;
        peerValue.ValueChanged += RecalculateState;

        await RecalculateState();
    }

    public bool ValuesSame { get; set; }

    private async Task RecalculateState()
    {
        var oldValuesSame = ValuesSame;
        ValuesSame = peerValue.Value as string == Value.Value as string;

        if (oldValuesSame != ValuesSame)
        {
            await InvokeStateChanged();
        }
    }
}
