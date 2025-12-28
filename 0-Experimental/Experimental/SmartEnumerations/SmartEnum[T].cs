namespace Experimental;

// ========================================================
public abstract class SmartEnum<T> where T : SmartEnum<T>
{
    protected SmartEnum(int value, string name)
    {
        Value = value;
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Description must not be null or empty.", nameof(name))
            : name.Trim();
    }

    public override string ToString() => Name;

    public int Value { get; }
    public string Name { get; }
}