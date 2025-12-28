namespace Experimental;

// ========================================================
/// <summary>
/// Base class for smart enumerations.
/// </summary>
/// <typeparam name="T"></typeparam>
/// PENDING:
/// - Equality, Hashing, etc
public abstract class SmartEnum<T> where T : SmartEnum<T>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="name"></param>
    protected SmartEnum(int value, string name)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name);

        Value = value;
        Name = name.Trim();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Name;

    /// <summary>
    /// The value of this instance.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// The name of this instance.
    /// </summary>
    public string Name { get; }
}