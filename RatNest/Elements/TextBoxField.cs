namespace RatNest.Elements;

public class TextBoxField : InputFieldBase<string>
{

    public TextBoxField(IFormRegion region) : base(region)
    {
    }

    public override string DefaultNamePrefix => "Text Box";

    public override bool IsBlank => string.IsNullOrEmpty(Value.Value);

    public override string BlankValue => "";

    public override void Create()
    {
        base.Create();
    }

    public override async Task Initialize()
    {
        await base.Initialize();
    }
}
