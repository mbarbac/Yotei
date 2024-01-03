namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// An immutable object that represents the metadata tag associated with a given metadata entry
/// obtained from an underlying database. Each instance carries a collection of valid names for
/// that metadata entry, and an optional default value.
/// <br/> Duplicates are not allowed.
/// </summary>
[Cloneable]
public sealed partial class MetadataTag : IEnumerable<string>
{
    /// <summary>
    /// Initializes a new instance with the given name.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="name"></param>
    public MetadataTag(IEngine engine, string name)
    {
        Engine = engine.ThrowWhenNull();
        AddInternal(name);
    }

    /// <summary>
    /// Initializes a new instance with the names from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="names"></param>
    public MetadataTag(IEngine engine, IEnumerable<string> names)
    {
        Engine = engine.ThrowWhenNull();
        AddRangeInternal(names);

        if (Count == 0) throw new ArgumentException(
            "Cannot initialize a new instance with an empty collection.")
            .WithData(names);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    MetadataTag(MetadataTag source)
    {
        Engine = source.Engine;
        if (source.HasValue) Value = source.Value;
        AddRangeInternal(source);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<string> GetEnumerator() => Names.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var str = this[0];
        if (Count > 1) str += $" ({string.Join(", ", Names[1..])})";
        if (HasValue) str += $":'{Value.Sketch()}'";
        return str;
    }

    // ----------------------------------------------------

    readonly List<string> Names = [];
    bool Compare(string x, string y) => string.Compare(x, y, !Engine.CaseSensitiveNames) == 0;

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// The optional default value carried by this instance, which is not a valid one unless it
    /// has been explicitly set.
    /// </summary>
    [WithGenerator]
    public object? Value
    {
        get => _Value;
        init { _Value = value; HasValue = true; }
    }
    object? _Value;

    /// <summary>
    /// Whether this instance carries an optional default value, or not.
    /// </summary>
    public bool HasValue { get; private set; }

    /// <summary>
    /// Returns a new instance where the default value carried by the original instance has been
    /// removed, if any. If no changes where detected, then the original instance is returned.
    /// </summary>
    /// <returns></returns>
    public MetadataTag ClearValue()
    {
        if (HasValue)
        {
            var clone = Clone();
            clone._Value = null;
            clone.HasValue = false;
            return clone;
        }
        else return this;
    }

    /// <summary>
    /// Gets the number of names in this collection.
    /// </summary>
    public int Count => Names.Count;

    /// <summary>
    /// Gets the name stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string this[int index] => Names[index];

    /// <summary>
    /// Determines if this collection contains the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => IndexOf(name) >= 0;

    /// <summary>
    /// Determines if this collection contains any name from the given range.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public bool ContainsAny(IEnumerable<string> names)
    {
        names.ThrowWhenNull();

        foreach (var name in names) if (IndexOf(name) >= 0) return true;
        return false;
    }

    /// <summary>
    /// Returns the index of the given name in this collection, or -1 if it is not found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int IndexOf(string name)
    {
        name = name.NotNullNotEmpty();
        return IndexOf(x => Compare(x, name));
    }

    /// <summary>
    /// Determines if this collection contains a name that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<string> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first ocurrence of a name that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<string> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = 0; i < Names.Count; i++) if (predicate(Names[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of a name that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<string> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = Names.Count - 1; i >= 0; i--) if (predicate(Names[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the indexes of the names in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<string> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var nums = new List<int>();
        for (int i = 0; i < Names.Count; i++) if (predicate(Names[i])) nums.Add(i);
        return nums;
    }

    /// <summary>
    /// Returns an array with the names in this collection.
    /// </summary>
    /// <returns></returns>
    public string[] ToArray() => Names.ToArray();

    /// <summary>
    /// Returns a list with the names in this collection.
    /// </summary>
    /// <returns></returns>
    public List<string> ToList() => new(Names);

    /// <summary>
    /// Minimizes the memory consumption of this collection.
    /// </summary>
    public void Trim() => Names.TrimExcess();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of names starting from the given index, as
    /// far as the count of the resulting instance if 1 or bigger. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public MetadataTag GetRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.GetRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int GetRangeInternal(int index, int count)
    {
        if (count == 0) return 0;
        if (index == 0 && count == Count) return 0;

        var range = Names.GetRange(index, count);
        Names.Clear();
        Names.AddRange(range);
        return count;
    }

    /// <summary>
    /// Returns a new instance where the name at the given index has been replaced by the new
    /// given one, if not equal to the existing one. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public MetadataTag Replace(int index, string name)
    {
        var clone = Clone();
        var num = clone.ReplaceInternal(index, name);
        return num > 0 ? clone : this;
    }
    int ReplaceInternal(int index, string name)
    {
        name = name.NotNullNotEmpty();

        if (Names[index] == name) return 0;
        RemoveAtInternal(index);
        return InsertInternal(index, name);
    }

    /// <summary>
    /// Returns a new instance where the given name has been added to the collection.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public MetadataTag Add(string name)
    {
        var clone = Clone();
        var num = clone.AddInternal(name);
        return num > 0 ? clone : this;
    }
    int AddInternal(string name)
    {
        var temp = IndexOf(name);
        if (temp >= 0) throw new DuplicateException(
            "The given name is already in this instance.").WithData(name);

        Names.Add(name);
        return 1;
    }

    /// <summary>
    /// Returns a new instance where the names from the given range have been added to it. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public MetadataTag AddRange(IEnumerable<string> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = AddInternal(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Returns a new instance where the given name has been inserted into the collection at the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public MetadataTag Insert(int index, string name)
    {
        var clone = Clone();
        var num = clone.InsertInternal(index, name);
        return num > 0 ? clone : this;
    }
    int InsertInternal(int index, string name)
    {
        var temp = IndexOf(name);
        if (temp >= 0) throw new DuplicateException(
            "The given name is already in this instance.").WithData(name);

        Names.Insert(index, name);
        return 1;
    }

    /// <summary>
    /// Returns a new instance the names from the given range have been inserted into it. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public MetadataTag InsertRange(int index, IEnumerable<string> range)
    {
        var clone = Clone();
        var num = clone.InsertRangeInternal(index, range);
        return num > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = InsertInternal(index, item);
            index += r;
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Returns a new instance where the name at the given index has been removed from the original
    /// collection. If the count of the produced instance is less than one then an invalid operation
    /// exception is thrown.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public MetadataTag RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.RemoveAtInternal(index);
        return num > 0 ? clone : this;
    }
    int RemoveAtInternal(int index)
    {
        if (Names.Count == 1) throw new InvalidOperationException(
            "Cannot remove the default name.")
            .WithData(this);

        Names.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Returns a new instance where the given number of names, starting from the given index,
    /// have been removed from the collection. If the count of the produced instance is less than
    /// one then an invalid operation exception is thrown. If no changes are detected, returns the
    /// original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public MetadataTag RemoveRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.RemoveRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int RemoveRangeInternal(int index, int count)
    {
        if (count > 0)
        {
            if ((Count - count) <= 0) throw new InvalidOperationException(
                "Cannot remove the default name.")
                .WithData(this);

            Names.RemoveRange(index, count);
            return count;
        }
        else return 0;
    }

    /// <summary>
    /// Returns a new instance where the given name has been removed. If the count of the produced
    /// instance is less than one then an invalid operation exception is thrown. If no changes are
    /// detected, returns the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public MetadataTag Remove(string name)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(name);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(string name)
    {
        var index = IndexOf(name);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where the first name that matches the given predicate has been
    /// removed. If the count of the produced instance is less than one then an invalid operation
    /// exception is thrown. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public MetadataTag Remove(Predicate<string> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<string> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where the last name that matches the given predicate has been
    /// removed. If the count of the produced instance is less than one then an invalid operation
    /// exception is thrown. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public MetadataTag RemoveLast(Predicate<string> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<string> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where all names that match the given predicate has been removed. If
    /// the count of the produced instance is less than one then an invalid operation exception is
    /// thrown. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public MetadataTag RemoveAll(Predicate<string> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<string> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAtInternal(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// Returns a new instance where all the names, but the first one, have been removed. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    public MetadataTag Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    int ClearInternal()
    {
        var num = Names.Count;
        if (num > 1)
        {
            while (Names.Count > 1) Names.RemoveAt(1);
            return num - 1;
        }
        return 0;
    }
}