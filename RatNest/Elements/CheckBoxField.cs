namespace RatNest.Elements;

public class CheckBoxField : InputFieldBase<bool>
{
    private readonly bool initialValue;


    public CheckBoxField(IFormRegion region, bool initialValue = false, bool resetOnHidden = true) : base(region, resetOnHidden)
    {
        this.initialValue = initialValue;
    }

    public override string DefaultNamePrefix => "Check Box";

    public override bool IsBlank => false;

    public override bool BlankValue => initialValue;

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
