#pragma warning disable IDE0018 // Inline variable declaration

using Yotei.ORM.Code.Internal;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IValueTranslators"/>
[Cloneable]
[DebuggerDisplay("{ToDebugString(6)}")]
public sealed partial class ValueTranslators : IValueTranslators
{
    readonly List<IValueTranslator> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ValueTranslators() { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    ValueTranslators(ValueTranslators source) => Items.AddRange(source.Items);

    /// <inheritdoc/>
    public IEnumerator<IValueTranslator> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    public string ToDebugString(int count)
    {
        if (Count == 0) return "0:[]";
        if (count == 0) return ToString();

        return Count <= count
            ? $"{Count}:[{string.Join(", ", Items.Select(x => x.ToString()))}]"
            : $"{Count}:[{string.Join(", ", Items.Take(count).Select(x => x.ToString()))}]";
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public IEnumerable<Type> SourceTypes => Items.Select(x => x.SourceType);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IValueTranslator? Find(Type type, bool inherit = false)
    {
        type.ThrowWhenNull();

        lock (Items)
        {
            var item = Items.Find(x => x.SourceType == type);
            if (item != null) return item;

            if (inherit)
            {
                var list = Items.FindAll(x => x.SourceType.IsAssignableFrom(type));

                if (list.Count == 1) return list[0];
                if (list.Count > 1)
                {
                    int parentDistance;
                    int ifaceDistance;

                    List<(IValueTranslator, int, int)> temps = [];
                    for (int i = 0; i < list.Count; i++)
                    {
                        var temp = list[i];
                        var done = type.InheritsFrom(temp.SourceType, out parentDistance, out ifaceDistance);
                        
                        if (done) temps.Add(new(temp, parentDistance, ifaceDistance));
                        else throw new UnExpectedException("Assignable type does not inherit.")
                            .WithData(type, "source")
                            .WithData(temp.SourceType, "target");
                    }

                    // A most derived parent...
                    var items = temps.OrderBy(x => x.Item2).Where(x => x.Item2 != int.MaxValue).ToList();
                    if (items.Count == 1) return items[0].Item1;

                    // A most derived interface...
                    items = temps.OrderBy(x => x.Item3).Where(x => x.Item3 != int.MaxValue).ToList();
                    if (items.Count == 1) return items[0].Item1;

                    // Many found...
                    throw new DuplicateException(
                        "Several registered types can be used for the given one.")
                        .WithData(type)
                        .WithData(list);
                }
            }

            return null;
        }
    }

    /// <inheritdoc/>
    public IValueTranslator? Find<TSource>(bool inherit = false)
    {
        return Find(typeof(TSource), inherit);
    }

    /// <inheritdoc/>
    public void Add<TSource, TTarget>(IValueTranslator<TSource, TTarget> translator)
    {
        translator.ThrowWhenNull();

        lock (Items)
        {
            var item = Items.Find(x => x.SourceType == translator.SourceType);
            if (item != null) throw new DuplicateException(
                "The source type of the given translator is already registered into this collection.")
                .WithData(translator);

            Items.Add(translator);
        }
    }

    /// <inheritdoc/>
    public void Add<TSource, TTarget>(Func<TSource, Locale, TTarget> translator)
    {
        var item = new ValueTranslator<TSource, TTarget>(translator);
        Add(item);
    }

    /// <inheritdoc/>
    public void AddOrReplace<TSource, TTarget>(IValueTranslator<TSource, TTarget> translator)
    {
        translator.ThrowWhenNull();

        lock (Items)
        {
            var index = Items.FindIndex(x => x.SourceType == translator.SourceType);
            if (index >= 0) Items[index] = translator;
            else Items.Add(translator);
        }
    }

    /// <inheritdoc/>
    public void AddOrReplace<TSource, TTarget>(Func<TSource, Locale, TTarget> translator)
    {
        var item = new ValueTranslator<TSource, TTarget>(translator);
        AddOrReplace(item);
    }

    /// <inheritdoc/>
    public void AddRange(IEnumerable<IValueTranslator> range)
    {
        range.ThrowWhenNull();

        if (ReferenceEquals(this, range)) return;
        lock (Items)
        {
            foreach (var item in range)
            {
                item.ThrowWhenNull();

                var temp = Items.Find(x => x.SourceType == item.SourceType);
                if (temp != null) throw new DuplicateException(
                    "The source type of translator in range is already registered into this collection.")
                    .WithData(item);

                Items.Add(item);
            }
        }
    }

    /// <inheritdoc/>
    public bool Remove(Type type)
    {
        type.ThrowWhenNull();

        lock (Items)
        {
            var index = Items.FindIndex(x => x.SourceType == type);
            if (index >= 0)
            {
                Items.RemoveAt(index);
                return true;
            }
            return false;
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        lock (Items) Items.Clear();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public object? Translate(object? source, Locale locale, bool inherit = false)
    {
        if (source == null) return null;

        var type = source.GetType();
        var item = Find(type, inherit);

        return item == null ? source : item.Translate(source, locale);
    }

    /// <inheritdoc/>
    public object? Translate<TSource>(TSource source, Locale locale, bool inherit = false)
    {
        var item = Find<TSource>(inherit);
        return item == null ? source : item.Translate(source, locale);
    }
}