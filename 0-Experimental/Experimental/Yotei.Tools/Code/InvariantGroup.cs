using IHost = Experimental.IInvariantGroup;
using IItem = string;

namespace Experimental;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
public class InvariantGroup : IHost
{
    /// <summary>
    /// Represents the actual contents carried by this instance.
    /// </summary>
    protected class InnerList : CoreList<IItem>
    {
        public InnerList(InvariantGroup master)
        {
            Master = master.ThrowWhenNull();
            Behavior = CoreList.Behavior.Add;
            ExpandNested = false;
        }
        InvariantGroup Master;

        public override IItem Validate(IItem item, bool add = false) => base.Validate(item, add);
        public override bool Equivalent(IItem inner, IItem other) => base.Equivalent(inner, other);

        protected override int OnAdd(string item) => base.OnAdd(item);
        protected override int OnInsert(int index, string item) => base.OnInsert(index, item);
        protected override int OnRemoveAt(int index) => base.OnRemoveAt(index);
        protected override int OnRemoveRange(int index, int count) => base.OnRemoveRange(index, count);
        protected override int OnClear() => base.OnClear();
    }

    /// <summary>
    /// The actual contents carried by this instance.
    /// </summary>
    protected InnerList Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantGroup() => Items = new(this);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public InvariantGroup(IItem item) : this() => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantGroup(IEnumerable<IItem> range) : this() => Items.AddRange(range);

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual InvariantGroup Clone() => new(this);
    IHost IHost.Clone() => Clone();
    object ICloneable.Clone() => Clone();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<IItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IItem this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public bool Contains(object criteria) => throw new NotImplementedException();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public int FindIndex(object criteria) => throw new NotImplementedException();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public int FindLastIndex(object criteria) => throw new NotImplementedException();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public int FindAllIndexes(object criteria) => throw new NotImplementedException();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(IItem item) => Items.Contains(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(IItem item) => Items.IndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(IItem item) => Items.LastIndexOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(IItem item) => Items.IndexesOf(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<IItem> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<IItem> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<IItem> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<IItem> predicate) => Items.IndexesOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IItem> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IHost GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;
        else
        {
            var range = Items.GetRange(index, count);
            var temp = Clone();
            temp.Items.Clear();
            temp.Items.AddRange(range);
            return temp;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Replace(int index, IItem item)
    {
        var temp = Clone();
        var done = temp.Items.Replace(index, item);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Add(IItem item)
    {
        var temp = Clone();
        var done = temp.Items.Add(item);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost AddRange(IEnumerable<IItem> range)
    {
        var temp = Clone();
        var done = temp.Items.AddRange(range);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Insert(int index, IItem item)
    {
        var temp = Clone();
        var done = temp.Items.Insert(index, item);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost InsertRange(int index, IEnumerable<IItem> range)
    {
        var temp = Clone();
        var done = temp.Items.InsertRange(index, range);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IHost RemoveAt(int index)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAt(index);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Remove(IItem item)
    {
        var temp = Clone();
        var done = temp.Items.Remove(item);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost RemoveLast(IItem item)
    {
        var temp = Clone();
        var done = temp.Items.RemoveLast(item);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost RemoveAll(IItem item)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAll(item);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IHost RemoveRange(int index, int count)
    {
        var temp = Clone();
        var done = temp.Items.RemoveRange(index, count);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IHost Remove(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var done = temp.Items.Remove(predicate);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IHost RemoveLast(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var done = temp.Items.RemoveLast(predicate);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IHost RemoveAll(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var done = temp.Items.RemoveAll(predicate);
        return done > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IHost Clear()
    {
        var temp = Clone();
        var done = temp.Items.Clear();
        return done > 0 ? temp : this;
    }
}