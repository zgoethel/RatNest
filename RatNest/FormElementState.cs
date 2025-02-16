namespace RatNest;

[Flags]
public enum FormElementState
{
    None = 0,
    Disabled = 1,
    Hidden = 2,
    Invalid = 4,
    Required = 8
}
