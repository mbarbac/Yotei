using T = Yotei.ORM.IParameter;
using K = string;

namespace Yotei.ORM.Code;

// ========================================================
[Cloneable]
public sealed partial class ParameterListBuilder : CoreList<K, T>
{
    public ParameterListBuilder(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        ValidateItem = (item) =>
        {
            item.ThrowWhenNull(); ValidateKey(item.Name);
            return item;
        };
        GetKey = (item) => item?.Name ?? throw new ArgumentNullException();
        ValidateKey = (key) => key.NotNullNotEmpty();
        CompareKeys = (x, y) => string.Compare(x, y, !Engine.CaseSensitiveNames) == 0;
        Duplicates = (@this, key) => @this.IndexesOf(key);
        CanInclude = (x, item) => ReferenceEquals(x, item)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);
    }
    public ParameterListBuilder(IEngine engine, T item) : this(engine) => Add(item);
    public ParameterListBuilder(IEngine engine, IEnumerable<T> range) : this(engine) => AddRange(range);
    ParameterListBuilder(ParameterListBuilder source) : this(source.Engine) => AddRange(source);

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// Returns the next available parameter's name.
    /// </summary>
    /// <returns></returns>
    public string NextName()
    {
        for (int i = Count; i < int.MaxValue; i++)
        {
            var name = $"{Engine.ParameterPrefix}{i}";
            var index = IndexOf(name);
            if (index < 0) return name;
        }
        throw new UnExpectedException("Range of integers exhausted.");
    }

    /// <summary>
    /// Returns a new instance with a new element, built using the given value and the next
    /// available name, is added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int AddNew(object? value, out T? item)
    {
        item = new Parameter(NextName(), value);
        return Add(item);
    }

    /// <summary>
    /// Returns a new instance with a new element, built using the given value and the next
    /// available name, is inserted into it at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int InsertNew(int index, object? value, out T? item)
    {
        item = new Parameter(NextName(), value);
        return Insert(index, item);
    }
}