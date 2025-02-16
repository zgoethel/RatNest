namespace RatNest;

public interface IFormRegion
{
    IFormRegion Parent { get; }

    NamingContext NamingContext { get; }

    bool IsVisible { get; }

    event Func<Task> IsVisibleChanged;

    Task InvokeIsVisibleChanged();

    IEnumerable<IFormRegion> ChildRegions { get; }

    IReadOnlyList<FormElementBase> Elements { get; }

    void AddElement(FormElementBase element);

    bool RemoveElement(FormElementBase element);

    event Action<INamedValue> ValueAdded;

    event Action<INamedValue> ValueRemoved;

    void InvokeValueAdded(INamedValue value);

    void InvokeValueRemoved(INamedValue value);
}

public class FormRegion : FormElementBase, IFormRegion
{
    public FormRegion(IFormRegion parent, bool topLevel = false) : base(parent)
    {
        if (topLevel)
        {
            CreateNamingContext();
        }
    }

    public override void Create()
    {
        if (Parent is not null)
        {
            ValueAdded += Parent.InvokeValueAdded;
            ValueRemoved += Parent.InvokeValueRemoved;
        }
    }

    public override async Task Initialize()
    {
        foreach (var element in Elements)
        {
            await element.Initialize();
        }
    }

    private NamingContext namingContext;
    public NamingContext NamingContext
    {
        get => namingContext ?? Parent?.NamingContext;
        private set
        {
            namingContext = value;
        }
    }

    public bool IsVisible { get; private set; } = true;

    public async Task SetIsVisible(bool isVisible)
    {
        IsVisible = isVisible;

        await InvokeIsVisibleChanged();
    }

    public event Func<Task> IsVisibleChanged;

    public async Task InvokeIsVisibleChanged()
    {
        await IsVisibleChanged.InvokeHandler();
    }

    public IEnumerable<IFormRegion> ChildRegions => Elements
        .Where((it) => it is IFormRegion)
        .Cast<IFormRegion>();

    private readonly List<FormElementBase> elements = new();
    public IReadOnlyList<FormElementBase> Elements => elements;

    public void AddElement(FormElementBase element)
    {
        elements.Add(element);

        foreach (var v in element.Values)
        {
            ValueAdded?.Invoke(v);
        }

        if (element is IFormRegion region)
        {
            IsVisibleChanged += region.InvokeIsVisibleChanged;
        }
    }

    public bool RemoveElement(FormElementBase element)
    {
        if (elements.Remove(element))
        {
            foreach (var v in element.Values)
            {
                ValueRemoved?.Invoke(v);
            }

            if (element is IFormRegion region)
            {
                IsVisibleChanged -= region.InvokeIsVisibleChanged;
            }

            return true;
        }
        return false;
    }

    public event Action<INamedValue> ValueAdded;

    public event Action<INamedValue> ValueRemoved;

    public void InvokeValueAdded(INamedValue value)
    {
        ValueAdded?.Invoke(value);
    }

    public void InvokeValueRemoved(INamedValue value)
    {
        ValueRemoved?.Invoke(value);
    }

    public void CreateNamingContext()
    {
        NamingContext = new();

        ValueAdded += NamingContext.AddValue;
        ValueRemoved += NamingContext.RemoveValue;
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
        for (; region?.Parent is not null; region = region.Parent);

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

    public static bool IsEffectivelyVisible(this IFormRegion region)
    {
        ArgumentNullException.ThrowIfNull(region, nameof(region));

        for (;
            region is not null && region.IsVisible;
            region = region.Parent);

        return region is null;
    }
}
