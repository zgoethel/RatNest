namespace RatNest.Elements;

public class TextBoxField : InputFieldBase<string>
{
    private readonly string initialValue;


    public TextBoxField(IFormRegion region, string initialValue = "", bool resetOnHidden = true) : base(region, resetOnHidden)
    {
        this.initialValue = initialValue;
    }

    public override string DefaultNamePrefix => "Text Box";

    public override bool IsBlank => string.IsNullOrEmpty(Value.Value);

    //TODO Reconsider
    public override string BlankValue => initialValue;

    public override void Create()
    {
        base.Create();
    }

    public override async Task Initialize()
    {
        await base.Initialize();

        await Value.SetValue(initialValue);
        await InvokeStateChanged();
    }
}
