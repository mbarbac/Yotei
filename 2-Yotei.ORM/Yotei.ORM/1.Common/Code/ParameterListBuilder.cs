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
        GetKey = (item) => item?.Name ?? throw new ArgumentNullException(nameof(item));
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

    public IEngine Engine { get; }

    // ----------------------------------------------------

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

    public int AddNew(object? value, out T item)
    {
        item = new Parameter(NextName(), value);
        return Add(item);
    }

    public int InsertNew(int index, object? value, out T item)
    {
        item = new Parameter(NextName(), value);
        return Insert(index, item);
    }
}