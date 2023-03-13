namespace Dev.Tester;

// ========================================================
/// <summary>
/// A collection of element specifications.
/// </summary>
public class ElementList : IEnumerable<Element>
{
    readonly List<Element> Items = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ElementList() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Count:{Count}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<Element> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// The number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Returns the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Element this[int index] => Items[index];

    /// <summary>
    /// Returns the first element in this collection that matches the given specifications,
    /// or null.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public Element? Find(Element specs)
    {
        specs = specs.ThrowIfNull();
        return Items.Find(x => x.Match(specs));
    }

    /// <summary>
    /// Returns the first element in this collection that matches the given specifications,
    /// or null.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public Element? Find(string? assemblyName, string? typeName, string? methodName)
    {
        var specs = new Element(assemblyName, typeName, methodName);
        return Find(specs);
    }

    /// <summary>
    /// Returns a list with the elements in this collection that match the given specifications.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public List<Element> FindAll(Element specs)
    {
        specs = specs.ThrowIfNull();
        return Items.FindAll(x => x.Match(specs));
    }

    /// <summary>
    /// Returns a list with the elements in this collection that match the given specifications.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public List<Element> FindAll(string? assemblyName, string? typeName, string? methodName)
    {
        var specs = new Element(assemblyName, typeName, methodName);
        return FindAll(specs);
    }

    /// <summary>
    /// Adds the given item into this collection.
    /// </summary>
    /// <param name="item"></param>
    public void Add(Element item)
    {
        item = item.ThrowIfNull();
        Items.Add(item);
    }

    /// <summary>
    /// Adds into this collection a new item with the given specifications.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    public void Add(string? assemblyName, string? typeName, string? methodName)
    {
        var item = new Element(assemblyName, typeName, methodName);
        Add(item);
    }

    /// <summary>
    /// Removes the given item from this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(Element item)
    {
        item = item.ThrowIfNull();
        return Items.Remove(item);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();
}