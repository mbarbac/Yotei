using TPair = System.Collections.Generic.KeyValuePair<string, string?>;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// An immutable object that contains the ordered collection of metadata pairs that describes
/// the maximal structure of the identifiers in an underlyins database. Each pair contains its
/// tag name, and its default value.
/// <br/> Pairs with duplicated tag names are not allowed.
/// <br/> Tags names cannot contain embedded dots.
/// </summary>
[Cloneable]
public sealed partial class IdentifierPairs : IEnumerable<TPair>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public IdentifierPairs(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public IdentifierPairs(IEngine engine, TPair item) : this(engine) => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierPairs(IEngine engine, IEnumerable<TPair> range) : this(engine) => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierPairs(IdentifierPairs source) : this(source.Engine) => AddRangeInternal(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TPair> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => string.Join('.', Items.Select(x => $"{x.Key}:{x.Value}"));

    // ----------------------------------------------------

    readonly List<TPair> Items = [];

    static TPair ValidateItem(TPair pair) { ValidateKey(pair.Key); return pair; }
    static string GetKey(TPair pair) => pair.Key;
    static string ValidateKey(string tag)
    {
        tag = tag.NotNullNotEmpty();
        if (tag.Contains('.')) throw new ArgumentException(
            "Identifier tag names cannot contain dots.").WithData(tag);
        return tag;
    }
    bool Compare(string x, string y) => string.Compare(x, y, !Engine.CaseSensitiveTags) == 0;
    bool IsSameElement(TPair x, TPair y) => Compare(x.Key, y.Key) && x.Value == y.Value;
    static bool AcceptDuplicates(TPair _, TPair y) => throw new DuplicateException("Duplicated element.").WithData(y);

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TPair this[int index] => Items[index];

    /// <summary>
    /// Determines if this collection contains an element with the given tag name.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag) => IndexOf(tag) >= 0;

    /// <summary>
    /// Returns the index of the element with the given tag name, or -1 if not found.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public int IndexOf(string tag)
    {
        tag = ValidateKey(tag);
        return IndexOf(x => Compare(GetKey(x), tag));
    }

    List<int> IndexesOf(string tag)
    {
        tag = ValidateKey(tag);
        return IndexesOf(x => Compare(GetKey(x), tag));
    }

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<TPair> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<TPair> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<TPair> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<TPair> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var nums = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) nums.Add(i);
        return nums;
    }

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public TPair[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<TPair> ToList() => new(Items);

    /// <summary>
    /// Minimizes the memory consumption of this collection.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements starting from the given index.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IdentifierPairs GetRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.GetRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int GetRangeInternal(int index, int count)
    {
        if (count == 0 && index >= 0) return ClearInternal();
        if (index == 0 && count == Count) return 0;

        var range = Items.GetRange(index, count);
        Items.Clear();
        Items.AddRange(range);
        return count;
    }

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the new
    /// given one, if not equal to the existing one. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IdentifierPairs Replace(int index, TPair item)
    {
        var clone = Clone();
        var num = clone.ReplaceInternal(index, item);
        return num > 0 ? clone : this;
    }
    int ReplaceInternal(int index, TPair item)
    {
        item = ValidateItem(item);

        var temp = Items[index];
        if (IsSameElement(temp, item)) return 0;

        RemoveAtInternal(index);
        return InsertInternal(index, item);
    }

    /// <summary>
    /// Returns a new instance where the given element has been added to the collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IdentifierPairs Add(TPair item)
    {
        var clone = Clone();
        var num = clone.AddInternal(item);
        return num > 0 ? clone : this;
    }
    int AddInternal(TPair item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = IndexesOf(key);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicates(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// collection. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public IdentifierPairs AddRange(IEnumerable<TPair> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<TPair> range)
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
    /// Returns a new instance where the given element has been inserted into the collection at
    /// the given index. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IdentifierPairs Insert(int index, TPair item)
    {
        var clone = Clone();
        var num = clone.InsertInternal(index, item);
        return num > 0 ? clone : this;
    }
    int InsertInternal(int index, TPair item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = IndexesOf(key);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicates(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// Returns a new instance the elements from the given range have been inserted into the
    /// collection, starting at the given index. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public IdentifierPairs InsertRange(int index, IEnumerable<TPair> range)
    {
        var clone = Clone();
        var num = clone.InsertRangeInternal(index, range);
        return num > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<TPair> range)
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
    /// Returns a new instance where the element at the given index has been removed from the
    /// original collection. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IdentifierPairs RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.RemoveAtInternal(index);
        return num > 0 ? clone : this;
    }
    int RemoveAtInternal(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Returns a new instance where the given number of elements, starting from the given index,
    /// have been removed from the collection. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IdentifierPairs RemoveRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.RemoveRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int RemoveRangeInternal(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// Returns a new instance where the element with the given tag name has been removed. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public IdentifierPairs Remove(string tag)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(tag);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(string tag)
    {
        var index = IndexOf(tag);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IdentifierPairs Remove(Predicate<TPair> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<TPair> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IdentifierPairs RemoveLast(Predicate<TPair> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<TPair> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// Returns a new instance where all elements that match the given predicate has been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IdentifierPairs RemoveAll(Predicate<TPair> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<TPair> predicate)
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
    /// Returns a new instance where all the elements have been removed. If no changes are
    /// detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    public IdentifierPairs Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    int ClearInternal()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}