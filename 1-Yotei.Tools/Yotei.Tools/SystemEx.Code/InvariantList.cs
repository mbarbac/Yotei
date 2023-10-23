namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{IItem, IKey}"/>
/// </summary>
/// <typeparam name="IItem"></typeparam>
/// <typeparam name="IKey"></typeparam>
public abstract class InvariantList<IItem, IKey> : IInvariantList<IItem, IKey>
{
    // Represents the contents of this instance.
    protected class InnerList : CoreList<IItem, IKey>
    {
        InvariantList<IItem, IKey> Master;
        public InnerList(InvariantList<IItem, IKey> master) => Master = master.ThrowWhenNull();
        protected InnerList(InnerList source) : this(source.Master) => AddRange(source);
        public override InnerList Clone() => new(this);

        public override IItem ValidateItem(IItem item) => Master.ValidateItem(item);
        public override IKey GetKey(IItem item) => Master.GetKey(item);
        public override IKey ValidateKey(IKey key) => Master.ValidateKey(key);
        public override bool CompareKeys(IKey inner, IKey other) => Master.CompareKeys(inner, other);
        public override bool AcceptDuplicated(IItem item) => Master.AcceptDuplicated(item);
        public override bool ExpandNested(IItem item) => Master.ExpandNested(item);
    }

    // The actual contents carried by this instance.
    protected InnerList Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantList() => Items = new(this);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public InvariantList(IItem item) : this() => Items.Add(item);

    /// <summary>
    /// Initializes a new instance with elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantList(IEnumerable<IItem> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected InvariantList(InvariantList<IItem, IKey> source)
    {
        source.ThrowWhenNull();

        Items = new(this);
        Items.AddRange(source);
    }

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    public abstract InvariantList<IItem, IKey> Clone();
    IInvariantList<IItem, IKey> IInvariantList<IItem, IKey>.Clone() => Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract IItem ValidateItem(IItem item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract IKey GetKey(IItem item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public abstract IKey ValidateKey(IKey key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="inner"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public abstract bool CompareKeys(IKey inner, IKey other);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract bool AcceptDuplicated(IItem item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public abstract bool ExpandNested(IItem item);

    // ----------------------------------------------------

    /// <summary>
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
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(IKey key) => Items.Contains(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(IKey key) => Items.IndexOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(IKey key) => Items.LastIndexOf(key);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(IKey key) => Items.IndexesOf(key);

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
    public virtual IInvariantList<IItem, IKey> GetRange(int index, int count)
    {
        if (count == Count && index == 0) return this;
        if (count == 0) return Clear();

        var range = Items.GetRange(index, count);
        var temp = Clone();
        temp.Items.Clear();
        temp.Items.AddRange(range);
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> Replace(int index, IItem item)
    {
        var temp = Clone();
        var num = temp.Items.Replace(index, item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> Add(IItem item)
    {
        var temp = Clone();
        var num = temp.Items.Add(item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> AddRange(IEnumerable<IItem> range)
    {
        var temp = Clone();
        var num = temp.Items.AddRange(range);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> Insert(int index, IItem item)
    {
        var temp = Clone();
        var num = temp.Items.Insert(index, item);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> InsertRange(int index, IEnumerable<IItem> range)
    {
        var temp = Clone();
        var num = temp.Items.InsertRange(index, range);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> RemoveAt(int index)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAt(index);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> RemoveRange(int index, int count)
    {
        var temp = Clone();
        var num = temp.Items.RemoveRange(index, count);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> Remove(IKey key)
    {
        var temp = Clone();
        var num = temp.Items.Remove(key);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> RemoveLast(IKey key)
    {
        var temp = Clone();
        var num = temp.Items.RemoveLast(key);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> RemoveAll(IKey key)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(key);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> Remove(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var num = temp.Items.Remove(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> RemoveLast(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var num = temp.Items.RemoveLast(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> RemoveAll(Predicate<IItem> predicate)
    {
        var temp = Clone();
        var num = temp.Items.RemoveAll(predicate);
        return num > 0 ? temp : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IInvariantList<IItem, IKey> Clear()
    {
        var temp = Clone();
        var num = temp.Items.Clear();
        return num > 0 ? temp : this;
    }
}