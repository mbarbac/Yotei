using T = Yotei.ORM.ISchemaEntry;
using K = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Code;

// ========================================================
[Cloneable]
public sealed partial class SchemaBuilder : CoreList<K, T>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaBuilder(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();

        ValidateItem = (item) =>
        {
            item.ThrowWhenNull();

            if (Engine != item.Engine) throw new ArgumentException(
                "The engine of the given entry is not the one of this instance.")
                .WithData(item)
                .WithData(this);
            
            ValidateKey(item.Identifier);
            return item;
        };
        GetKey = (item) =>
        {
            item.ThrowWhenNull();
            return item.Identifier;
        };
        ValidateKey = (key) =>
        {
            key.ThrowWhenNull();
            if (key.Count == 0) throw new ArgumentException("Identifier cannot be empty.").WithData(key);
            if (key.Value == null) throw new ArgumentException("Value of identifier cannot be null.").WithData(key);
            if (key[^1].Value == null) throw new ArgumentException("Value of last part of identifier cannot be null.").WithData(key);
            return key;
        };
        CompareKeys = (kx, ky) => string.Compare(kx.Value, ky.Value, !Engine.CaseSensitiveNames) == 0;
        GetDuplicates = (key) =>
        {
            var nums = IndexesOf(x => x.Identifier.Match(key.Value));
            var temp = IndexesOf(x => key.Match(x.Identifier.Value));
            foreach (var num in temp) if (!nums.Contains(num)) nums.Add(num);
            return nums;
        };
        CanInclude = (item, x) => ReferenceEquals(item, x)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public SchemaBuilder(IEngine engine, T item) : this(engine) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaBuilder(IEngine engine, IEnumerable<T> range) : this(engine) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    SchemaBuilder(SchemaBuilder source) : this(source.Engine) => AddRange(source);

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains an element with the given key.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool Contains(string identifier) => IndexOf(identifier) >= 0;

    /// <summary>
    /// Gets the index of the element in this collection with the given key, or -1 if it is
    /// not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public int IndexOf(string identifier)
    {
        var key = new Identifier(Engine, identifier);
        return IndexOf(key);
    }

    /// <summary>
    /// Gets the index of the last element in this collection with the given key, or -1 if it
    /// is not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public int LastIndexOf(string identifier)
    {
        var key = new Identifier(Engine, identifier);
        return LastIndexOf(key);
    }

    /// <summary>
    /// Gets the indexes of the elements in this collection with the given key.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public List<int> IndexesOf(string identifier)
    {
        var key = new Identifier(Engine, identifier);
        return IndexesOf(key);
    }

    /// <summary>
    /// Returns the collection of indexes of the elements in this instances whose identifiers
    /// match the given specifications. Matching is performed by comparing parts from right to
    /// left, where any null or empty part in the specifications is considered an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public List<int> Match(string? specs) => Match(specs, out _);

    /// <summary>
    /// Returns the collection of indexes of the elements in this instances whose identifiers
    /// match the given specifications. Matching is performed by comparing parts from right to
    /// left, where any null or empty part in the specifications is considered an implicit match.
    /// <br/> If the returned collection contains just one element, then it is placed in the out
    /// argument. Otherwise, it is set to null.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    public List<int> Match(string? specs, out T? unique)
    {
        List<int> nums = [];

        for (int i = 0; i < Count; i++)
        {
            var item = this[i];
            if (item.Identifier.Match(specs)) nums.Add(i);
        }

        unique = nums.Count == 1 ? this[nums[0]] : null;
        return nums;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Removes the first element with the given identifier.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int Remove(string identifier)
    {
        var index = IndexOf(identifier);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes the last element with the given identifier.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int RemoveLast(string identifier)
    {
        var index = LastIndexOf(identifier);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes all elements with the given identifier.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int RemoveAll(string identifier)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(identifier);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }
}