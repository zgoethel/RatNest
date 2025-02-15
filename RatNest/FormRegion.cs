namespace RatNest;

public interface IFormRegion
{
    IFormRegion Parent { get; }

    bool IsVisible { get; }

    IEnumerable<IFormRegion> ChildRegions { get; }

    IEnumerable<FormElementBase> Elements { get; }
}

public class FormRegion : IFormRegion
{
    public FormRegion(IFormRegion parent)
    {
        Parent = parent;
    }

    public IFormRegion Parent { get; private set; }

    public bool IsVisible { get; set; } = true;

    public IEnumerable<IFormRegion> ChildRegions => Array.Empty<IFormRegion>();

    private List<FormElementBase> elements = new();
    public IEnumerable<FormElementBase> Elements => elements;

    public void AddElement(FormElementBase element)
    {
        elements.Add(element);
    }
}

public static class FormRegionExtensions
{
    public static IEnumerable<INamedValue> GetAllValues(this IFormRegion region)
    {
        return region.Elements
            .SelectMany((it) => it.Values)
            .Union(region.ChildRegions
                .SelectMany((it) => it.GetAllValues()));
    }

    public static IFormRegion GetTopLevel(this IFormRegion region)
    {
        for (; region?.Parent is not null; region = region.Parent) ;

        return region;
    }

    public static IEnumerable<INamedValue> GetGlobalValues(this IFormRegion region)
    {
        return region
            .GetTopLevel()
            .GetAllValues();
    }
    public static IEnumerable<FormElementBase> GetAllElements(this IFormRegion region)
    {
        return region.Elements
            .Union(region.ChildRegions
                .SelectMany((it) => it.GetAllElements()));
    }
}
