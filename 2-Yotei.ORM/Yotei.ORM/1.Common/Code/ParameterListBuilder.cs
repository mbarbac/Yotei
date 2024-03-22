using T = Yotei.ORM.IParameter;
using K = string;

namespace Yotei.ORM.Code;

// ========================================================
[Cloneable]
public sealed partial class ParameterListBuilder : CoreList<K, T>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ParameterListBuilder(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();

        ValidateItem = (item) =>
        {
            item.ThrowWhenNull(); ValidateKey(item.Name);
            return item;
        };
        GetKey = (item) => item.ThrowWhenNull().Name;
        ValidateKey = (key) => key.NotNullNotEmpty();
        CompareKeys = (x, y) => string.Compare(x, y, !Engine.CaseSensitiveNames) == 0;
        GetDuplicates = IndexesOf;

        // We need to use 'ReferenceEquals' and not 'Equals': the parameter instance may carry a
        // different value, or even change it after it is in the collection...
        CanInclude = (item, x) => ReferenceEquals(item, x)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public ParameterListBuilder(IEngine engine, T item) : this(engine) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements with the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    /// <exception cref="System.NullReferenceException"></exception>
    public ParameterListBuilder(IEngine engine, IEnumerable<T> range) : this(engine) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    ParameterListBuilder(ParameterListBuilder source) : this(source.Engine) => AddRange(source);

    // ----------------------------------------------------

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

    // ----------------------------------------------------

    /// <summary>
    /// Adds a new element using the given value and the next available parameter's name.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int AddNew(object? value, out T item)
    {
        item = new Parameter(NextName(), value);
        return Add(item);
    }

    /// <summary>
    /// Inserts at the given index a new element using the given value and the next available
    /// parameter's name.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int InsertNew(int index, object? value, out T item)
    {
        item = new Parameter(NextName(), value);
        return Insert(index, item);
    }
}