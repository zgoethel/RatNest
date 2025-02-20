using Microsoft.AspNetCore.Components;

namespace RatNest.Elements;

public abstract class InputViewBase<TValue, TField> : ComponentBase
    where TField : InputFieldBase<TValue>
{
    private TField prevField;
    [Parameter]
    public TField Field { get; set; }

    protected string EffectiveLabel => $"{Field.Value.Name}{(Field.IsRequired ? "*" : "")}";

    private async Task Redraw()
    {
        await InvokeAsync(StateHasChanged);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (prevField != Field)
        {
            if (prevField is not null)
            {
                Field.StateChanged -= Redraw;
                Field.ValueChanged -= Redraw;
            }
            if (Field is not null)
            {
                Field.StateChanged += Redraw;
                Field.ValueChanged += Redraw;
            }
        }
        prevField = Field;
    }
}
