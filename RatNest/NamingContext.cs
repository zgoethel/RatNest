namespace RatNest;

public class NamingContext
{
    public Dictionary<string, INamedValue> valuesByName = new();
    public IReadOnlyDictionary<string, INamedValue> ValuesByName => valuesByName;

    public Dictionary<Guid, INamedValue> valuesByRefId = new();
    public IReadOnlyDictionary<Guid, INamedValue> ValuesByRefId => valuesByRefId;

    private void HandleRename(string oldName, string newName)
    {
        if (valuesByName.Remove(oldName, out var value))
        {
            valuesByName[newName] = value;
        }
    }

    public void AddValue(INamedValue value)
    {
        valuesByName[value.Name] = value;
        valuesByRefId[value.RefId] = value;

        value.Renamed += HandleRename;
    }

    public void RemoveValue(INamedValue value)
    {
        value.Renamed -= HandleRename;

        valuesByName.Remove(value.Name);
        valuesByRefId.Remove(value.RefId);
    }
}
