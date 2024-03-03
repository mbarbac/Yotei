namespace Experiments.Designs;

// ========================================================
public interface IUpcast<T> { }

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class UpcastExcludeAttribute : Attribute
{
    [SuppressMessage("", "IDE0290")]
    public UpcastExcludeAttribute(Type type, string name, params Type[] args)
    {
        Type = type;
        Name = name;
        Arguments = args;
    }
    public Type Type { get; }
    public string Name { get; }
    public Type[] Arguments { get; }
}

// ========================================================
public interface IFrozenList<K, T>
{
    IFrozenList<K, T> this[K key] { get; }
    IFrozenList<K, T> Add(T item);
}

public class FrozenList<K, T> : IFrozenList<K, T>
{
    public IFrozenList<K, T> this[K key] { get => this; }
    public IFrozenList<K, T> Add(T item) => this;
}

// ========================================================

// CS1962: typeof cannot be used with dynamic
// [UpcastExclude(typeof(IFrozenList<string, dynamic>), nameof(Add), typeof(dynamic))]
[UpcastExclude(typeof(IFrozenList<string, dynamic>), "Add.T")]
[UpcastExclude(typeof(IFrozenList<string, dynamic>), "this.string")] // string? String?
public partial interface ITagList<T> : IUpcast<IFrozenList<string, T>> { }
public partial interface ITagList<T> : IFrozenList<string, T>
{
    new ITagList<T> this[string key] { get; }
    new ITagList<T> Add(T item);
}