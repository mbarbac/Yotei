namespace Dev.Runner;

// ========================================================
/// <summary>
/// Represents a  collection of test inclusion or test exclusion specifications.
/// </summary>
internal class ElementList : IEnumerable<Element>
{
    readonly List<Element> _Items = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ElementList() { }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ToString() => $"Count:{Count}";

    /// <summary>
    /// Returns an enumerator that can iterate through the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Element> GetEnumerator() => _Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// The number of elements in this collection.
    /// </summary>
    public int Count => _Items.Count;

    /// <summary>
    /// Returns the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Element this[int index] => _Items[index];

    /// <summary>
    /// Returns the index at which the given element is stored in this collection, or -1 if it is
    /// not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(Element item)
    {
        item = item.ThrowIfNull();
        return _Items.IndexOf(item);
    }

    /// <summary>
    /// Returns the element in this collection whose specifications match the given ones.
    /// Null type or method names are considered an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public Element? Find(Element specs)
    {
        specs = specs.ThrowIfNull();
        return _Items.Find(x => x.Match(specs));
    }

    /// <summary>
    /// Returns the element in this collection whose specifications match the given ones.
    /// Null type or method names are considered an implicit match.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public Element? Find(string assemblyName, string? typeName = null, string? methodName = null)
    {
        var specs = new Element(assemblyName, typeName, methodName);
        return Find(specs);
    }

    /// <summary>
    /// Adds into this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    public void Add(Element item)
    {
        item = item.ThrowIfNull();

        var temp = Find(item);
        if (temp != null) throw new DuplicateException("Metadata already exists.");

        _Items.Add(item);
    }

    /// <summary>
    /// Adds into this collection a new element for the given metadata, or returns the existing
    /// one.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public Element Add(string assemblyName, string? typeName = null, string? methodName = null)
    {
        var item = new Element(assemblyName, typeName, methodName);

        var temp = Find(item);
        if (temp == null)
        {
            _Items.Add(item);
            return item;
        }
        else return temp;
    }

    /// <summary>
    /// Removes from this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(Element item)
    {
        item = item.ThrowIfNull();
        return _Items.Remove(item);
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => _Items.Clear();
}