namespace RatNest.Test.Client.Elements;

public class TextBoxField : FormElementBase
{
    private INamedValue peerValue;

    public TextBoxField(IFormRegion region) : base(region)
    {
    }

    public NamedValue<string> Value { get; private set; }

    public bool ValuesSame { get; set; }

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

        Value.ValueChanged += RecalculateState;
        peerValue.ValueChanged += RecalculateState;

        await RecalculateState();
    }

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
