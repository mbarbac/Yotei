namespace Experiments.Designs.UpcastGenerator;

// ========================================================
/// <summary>
/// Used to wrap the types that appears in the inheritance list of a given host
/// type so that the methods in the inherited one, whose return type is exactly
/// that type, are upcasted to the host one.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IUpcast<T> { }

/// <summary>
/// Used to wrap the types that appears in the inheritance list of a given host
/// type so that the methods and properties in the inherited one, whose return
/// or member type is exactly that type, are upcasted to the host one.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IUpcastEx<T> { }

// ========================================================
public interface IFrozenList<T> : IEnumerable<T>
{
    int Count { get; }
    IFrozenList<T>? this[int index] { get; set; }
    IFrozenList<T>? Instance { get; set; }
    IFrozenList<T>? Add(T item);
}

// ========================================================
public class FrozenList<T> : IFrozenList<T>
{
    public IEnumerator<T> GetEnumerator() => throw null!;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => throw null!;
    public IFrozenList<T>? this[int index] { get => throw null!; set { } }
    public IFrozenList<T>? Instance { get => throw null!; set { } }
    public virtual IFrozenList<T>? Add(T item) => throw null!;
}

// ========================================================
public partial interface ITagList<T> : IUpcast<IFrozenList<T>?>
{ }
partial interface ITagList<T> : IFrozenList<T>
{
    new ITagList<T>? Instance { get; set; }
    new ITagList<T>? Add(T item);
}

// ========================================================
public partial class TagList<T> : IUpcast<FrozenList<T>>, IUpcast<ITagList<T>>
{ }
partial class TagList<T> : FrozenList<T>, ITagList<T>
{
    public new TagList<T>? this[int index]
    {
        get => (TagList<T>?)base[index];
        set => base[index] = value;
    }
    public new TagList<T>? Instance
    {
        get => (TagList<T>?)base.Instance;
        set => base.Instance = value;
    }
    public override TagList<T>? Add(T item) => (TagList<T>?)base.Add(item);

    ITagList<T>? ITagList<T>.Instance
    {
        get => Instance;
        set => Instance = (TagList<T>?)value;
    }
    ITagList<T>? ITagList<T>.Add(T item) => Add(item);
}