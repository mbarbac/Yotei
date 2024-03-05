using System.Runtime.InteropServices.Marshalling;
using T = Yotei.ORM.IMetadataEntry;

namespace Yotei.ORM.Code;

// ========================================================
[Cloneable]
[DebuggerDisplay("{ToDebugString(6)}")]
public sealed partial class SchemaEntryBuilder : IEnumerable<T>
{
    readonly List<T> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public SchemaEntryBuilder(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntryBuilder(
        IEngine engine, IEnumerable<T> range) : this(engine) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    SchemaEntryBuilder(
        SchemaEntryBuilder source) : this(source.Engine) => AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Identifier.Count == 0 ? "-" : Identifier.ToString());
        if (IsPrimaryKey) sb.Append(", Primary");
        if (IsUniqueValued) sb.Append(", Unique");
        if (IsReadOnly) sb.Append(", ReadOnly");

        foreach (var item in Items)
        {
            if (KnownTags.ContainsAny(item.Tag)) continue;
            sb.Append($", {item}");
        }

        return sb.ToString();
    }

    string ToDebugString(int count) => Count == 0 ? "0:[]" : (
        Count < count
        ? $"{Count}:[{string.Join(", ", this.Select(ItemToString))}]"
        : $"{Count}:[{string.Join(", ", this.Take(count).Select(ItemToString))}, ...]");

    string ItemToString(T item) => item?.ToString() ?? "-";

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }
    IKnownTags KnownTags => Engine.KnownTags;

    /// <summary>
    /// The identifier by which this instance is known.
    /// </summary>
    public IIdentifier Identifier
    {
        get
        {
            if (_Identifier == null)
            {
                _Identifier = new Identifier(Engine);

                var tags = KnownTags.IdentifierTags;
                var count = tags.Count;
                if (count > 0)
                {
                    var values = new string?[count];
                    for (int i = 0; i < count; i++)
                    {
                        var index = IndexOfAny(tags[i]);
                        if (index >= 0) values[i] = (string?)Items[index].Value;
                    }

                    while (values.Length > 0 && values[0] == null) values = values.RemoveAt(0);
                    if (values.Length > 0) _Identifier = new Identifier(Engine, values);
                }
            }

            return _Identifier;
        }
        set
        {
            value.ThrowWhenNull();

            if (Engine != value.Engine) throw new ArgumentException(
                "Engine of the given identifier is not the one of this instance.")
                .WithData(value)
                .WithData(this);

            var tags = KnownTags.IdentifierTags;
            var count = tags.Count;

            for (int i = 0; i < count; i++)
            {
                var tag = tags[i];
                var index = IndexOfAny(tag);
                if (index >= 0) Items.Remove(Items[index]);
            }

            if (count > 0)
            {
                var values = value.Select(x => x.UnwrappedValue).ToArray().ResizeHead(count);
                var done = false;

                for (int i = 0; i < count; i++)
                {
                    var temp = values[i];
                    if (temp == null && !done) continue;

                    done = true;
                    var tag = tags[i];
                    Items.Add(new MetadataEntry(tag, temp));
                }
            }

            _Identifier = value;
        }
    }
    IIdentifier? _Identifier;

    /// <summary>
    /// Determines if this instance describes a primary key entry, or that it is part of the
    /// primary key group, if any, or not.
    /// </summary>
    public bool IsPrimaryKey
    {
        get
        {
            if (_IsPrimaryKey == null)
            {
                _IsPrimaryKey = false;

                var tag = KnownTags.PrimaryKeyTag;
                if (tag != null)
                {
                    var index = IndexOfAny(tag);
                    if (index >= 0) _IsPrimaryKey = (bool)Items[index].Value!;
                }
            }

            return _IsPrimaryKey.Value;
        }
        set
        {
            var tag = KnownTags.PrimaryKeyTag;
            if (tag != null)
            {
                var index = IndexOfAny(tag);
                if (index >= 0) Items.Remove(Items[index]);

                Items.Add(new MetadataEntry(tag, value));
            }

            _IsPrimaryKey = value;
        }
    }
    bool? _IsPrimaryKey;

    /// <summary>
    /// Determines if this instance describes an unique valued entry, or that it is part of the
    /// unique valued group, if any, or not.
    /// </summary>
    public bool IsUniqueValued
    {
        get
        {
            if (_IsUniqueValued == null)
            {
                _IsUniqueValued = false;

                var tag = KnownTags.UniqueValuedTag;
                if (tag != null)
                {
                    var index = IndexOfAny(tag);
                    if (index >= 0) _IsUniqueValued = (bool)Items[index].Value!;
                }
            }

            return _IsUniqueValued.Value;
        }
        set
        {
            var tag = KnownTags.UniqueValuedTag;
            if (tag != null)
            {
                var index = IndexOfAny(tag);
                if (index >= 0) Items.Remove(Items[index]);

                Items.Add(new MetadataEntry(tag, value));
            }

            _IsUniqueValued = value;
        }
    }
    bool? _IsUniqueValued;

    /// <summary>
    /// Determines if this instance describes a read only entry, or not.
    /// </summary>
    public bool IsReadOnly
    {
        get
        {
            if (_IsReadOnly == null)
            {
                _IsReadOnly = false;

                var tag = KnownTags.ReadOnlyTag;
                if (tag != null)
                {
                    var index = IndexOfAny(tag);
                    if (index >= 0) _IsReadOnly = (bool)Items[index].Value!;
                }
            }

            return _IsReadOnly.Value;
        }
        set
        {
            var tag = KnownTags.ReadOnlyTag;
            if (tag != null)
            {
                var index = IndexOfAny(tag);
                if (index >= 0) Items.Remove(Items[index]);

                Items.Add(new MetadataEntry(tag, value));
            }

            _IsReadOnly = value;
        }
    }
    bool? _IsReadOnly;

    /// <summary>
    /// The collection of tag names carried by all entries in this instance.
    /// </summary>
    public IEnumerable<string> Names
    {
        get
        {
            foreach (var item in Items)
                foreach (var name in item.Tag) yield return name;
        }
    }

    // ----------------------------------------------------

    int IndexOf(string name)
    {
        name = name.NotNullNotEmpty();

        for (int i = 0; i < Items.Count; i++) if (Items[i].Tag.Contains(name)) return i;
        return -1;
    }

    int IndexOfAny(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++) if (Items[i].Tag.ContainsAny(range)) return i;
        return -1;
    }

    int IndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    int LastIndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets the value of the metadata pair that contains the given tag name.
    /// <br/> If such metadata pair cannot be found using the given tag name, and exception will
    /// be thrown.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public object? this[string name] => TryGetValue(name, out var value)
        ? value
        : throw new NotFoundException("Entry for the given tag name not found.").WithData(name);

    /// <summary>
    /// Obtains the value of the metadata pair that contains the given tag name, provided that
    /// there is such metadata pair.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(string name, out object? value)
    {
        var index = IndexOf(name);
        if (index >= 0)
        {
            value = Items[index].Value;
            return true;
        }

        value = null;
        return false;
    }

    /// <summary>
    /// Obtains the value of the metadata pair that contains the given tag name, provided that
    /// there is such metadata pair.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue<V>(string name, out V value)
    {
        if (TryGetValue(name, out var temp))
        {
            value = (V)temp!;
            return true;
        }

        value = default!;
        return false;
    }

    /// <summary>
    /// Determines if this collection contains a metadata pair that contains the given tag name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => IndexOf(name) >= 0;

    /// <summary>
    /// Determines if this collection contains a metadata pair that contains any of the names from
    /// the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool ContainsAny(IEnumerable<string> range) => IndexOfAny(range) >= 0;

    /// <summary>
    /// Returns the metadata pair that contains the given tag name, or null if such cannot be
    /// found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public T? Find(string name)
    {
        var index = IndexOf(name);
        return index >= 0 ? Items[index] : null;
    }

    /// <summary>
    /// Returns the metadata pair that contains any of the names from the given range, or null if
    /// such cannot be found.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public T? Find(IEnumerable<string> range)
    {
        var index = IndexOfAny(range);
        return index >= 0 ? Items[index] : null;
    }

    /// <summary>
    /// Returns the first metadata pair that matches the given predicate, or null if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? Find(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return Items[i];
        return null;
    }

    /// <summary>
    /// Returns the last metadata pair that matches the given predicate, or null if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? FindLast(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return Items[i];
        return null;
    }

    /// <summary>
    /// Returns all the metadata pairs that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<T> FindAll(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        List<T> list = [];
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(Items[i]);
        return list;
    }

    /// <summary>
    /// Gets an array with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    /// <summary>
    /// Gets a list with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => new(Items);

    // ----------------------------------------------------

    void ClearCache(IEnumerable<string> range)
    {
        foreach (var name in range) ClearCache(name);
    }

    void ClearCache(string name)
    {
        if (KnownTags.IdentifierTags.Contains(name)) _Identifier = null;
        else if (KnownTags.PrimaryKeyTag != null && KnownTags.PrimaryKeyTag.Contains(name)) _IsPrimaryKey = null;
        else if (KnownTags.UniqueValuedTag != null && KnownTags.UniqueValuedTag.Contains(name)) _IsUniqueValued = null;
        else if (KnownTags.ReadOnlyTag != null && KnownTags.ReadOnlyTag.Contains(name)) _IsReadOnly = null;
    }

    T Adjust(T item)
    {
        item.ThrowWhenNull();

        if (KnownTags.IdentifierTags.ContainsAny(item.Tag))
        {
            var source = (string?)item.Value;
            var value = new IdentifierPart(Engine, source).UnwrappedValue;

            if (string.Compare(source, value, !Engine.CaseSensitiveNames) != 0)
            {
                item = new MetadataEntry(item.Tag, value);
            }
        }
        else if (KnownTags.PrimaryKeyTag != null && KnownTags.PrimaryKeyTag.ContainsAny(item.Tag))
        {
            if (item.Value is not bool) throw new InvalidCastException(
                $"Value of {IsPrimaryKey} must be a boolean.")
                .WithData(item);
        }
        else if (KnownTags.UniqueValuedTag != null && KnownTags.UniqueValuedTag.ContainsAny(item.Tag))
        {
            if (item.Value is not bool) throw new InvalidCastException(
                $"Value of {IsUniqueValued} must be a boolean.")
                .WithData(item);
        }
        else if (KnownTags.ReadOnlyTag != null && KnownTags.ReadOnlyTag.ContainsAny(item.Tag))
        {
            if (item.Value is not bool) throw new InvalidCastException(
                $"Value of {IsReadOnly} must be a boolean.")
                .WithData(item);
        }

        return item;
    }

    /// <summary>
    /// Replaces the metadata pair that contains the given tag name with the new given one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Replace(string name, T item)
    {
        name = name.NotNullNotEmpty();
        item = Adjust(item);

        var index = IndexOf(name); // May not exist, but being a known tag...
        if (index < 0)
        {
            if (KnownTags.ContainsAny(item.Tag))
            {
                Items.Add(item);

                ClearCache(item.Tag);
                return 1;
            }
        }
        else // Removing existing and adding...
        {
            Items.RemoveAt(index);
            Items.Add(item);

            ClearCache(name);
            return 1;
        }

        return 0; // We have not found an entry to replace...
    }

    /// <summary>
    /// Replaces the value of the metadata pair that contains the given tag name with the new
    /// given one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int Replace(string name, object? value)
    {
        name = name.NotNullNotEmpty();

        var index = IndexOf(name); // May not exist, but being a known tag...
        if (index < 0)
        {
            if (KnownTags.Contains(name))
            {
                var sensitive = KnownTags.CaseSensitiveTags;
                var item = new MetadataEntry(new MetadataTag(sensitive, name), value);
                Items.Add(item);

                ClearCache(name);
                return 1;
            }
        }
        else // Removing existing and adding...
        {
            var item = Items[index];
            Items.RemoveAt(index);

            item = new MetadataEntry(item.Tag, value);
            item = Adjust(item);
            Items.Add(item);

            ClearCache(name);
            return 1;
        }

        return 0; // We have not found an entry to replace...
    }

    /// <summary>
    /// Adds the given metadata pair added to this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Add(T item)
    {
        item = Adjust(item);

        var index = IndexOfAny(item.Tag);
        if (index >= 0) throw new DuplicateException(
            "A tag name from the given element already exists in this collection.")
            .WithData(item)
            .WithData(this);

        Items.Add(item);        
        ClearCache(item.Tag);
        return 1;
    }

    /// <summary>
    /// Adds the metadata pairs from the given range added to this collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = Add(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Removes the metadata pair that contains the given tag name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int Remove(string name)
    {
        if (Count == 0) return 0;

        var index = IndexOf(name);
        if (index >= 0)
        {
            Items.RemoveAt(index);

            ClearCache(name);
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// Removes the first metadata pair that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int Remove(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var index = IndexOf(predicate);
        if (index >= 0)
        {
            var tag = Items[index]; Items.RemoveAt(index);
            
            ClearCache(tag.Tag);
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// Removes the last metadata pair that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int RemoveLast(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var index = LastIndexOf(predicate);
        if (index >= 0)
        {
            var tag = Items[index]; Items.RemoveAt(index);

            ClearCache(tag.Tag);
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// Removes all the metadata pairs that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int RemoveAll(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0)
            {
                var tag = Items[index]; Items.RemoveAt(index);

                num++;
                ClearCache(tag.Tag);
            }
            else break;
        }
        return num;
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    /// <returns></returns>
    public int Clear()
    {
        if (Items.Count == 0 &&
            _Identifier == null &&
            _IsPrimaryKey == null &&
            _IsUniqueValued == null &&
            _IsReadOnly == null)
            return 0;

        var num = Items.Count > 0 ? Items.Count : 1;
        Items.Clear();
        _Identifier = null;
        _IsPrimaryKey = null;
        _IsUniqueValued = null;
        _IsReadOnly = null;

        return num;
    }
}