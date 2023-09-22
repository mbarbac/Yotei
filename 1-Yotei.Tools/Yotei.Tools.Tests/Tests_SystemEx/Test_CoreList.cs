using NuGet.Frameworks;

namespace Yotei.Tools.Tests;

// ========================================================
[Enforced]
public static class Test_CoreList
{
    static IElement TestValidator(IElement element, bool _) => element.ThrowWhenNull();
    static bool TestComparer(bool caseSensitive, IElement x, IElement y)
    {
        if (x is NameElement nx &&
            y is NameElement ny &&
            string.Compare(nx.Name, ny.Name, !caseSensitive) == 0) return true;

        return ReferenceEquals(x, y);
    }
    static CoreListBehavior TestBehavior = CoreListBehavior.Throw;
    static bool TestFlatten = true;

    // ----------------------------------------------------

    public interface IElement { }
    public class NameElement : IElement
    {
        public NameElement(string name) => Name = name;
        public override string ToString() => Name;
        public string Name { get; set; }
    }
    public class ChainElement : CoreList<IElement>, IElement
    {
        public ChainElement(bool caseSensitive)
        {
            Validator = (item, add) => TestValidator(item, add);
            Comparer = (x, y) => TestComparer(CaseSensitive, x, y);
            Flatten = TestFlatten;
            Behavior = TestBehavior; 
            CaseSensitive = caseSensitive;
        }
        public ChainElement(bool caseSensitive, IElement item) : this(caseSensitive) => Add(item);
        public ChainElement(bool caseSensitive, IEnumerable<IElement> range) : this(caseSensitive) => AddRange(range);
        public override ChainElement Clone()
        {
            var temp = new ChainElement(CaseSensitive);
            temp.CopySettings(this);
            temp.AddRange(this);
            return temp;
        }
        public override void CopySettings(ICoreList<IElement> source)
        {
            if (source is ChainElement item)
            {
                CaseSensitive = item.CaseSensitive;
                Flatten = item.Flatten;
                Behavior = item.Behavior;
            }
            else base.CopySettings(source);
        }
        public bool CaseSensitive
        {
            get => _CaseSensitive;
            set
            {
                if (_CaseSensitive == value) return;

                _CaseSensitive = value; if (Count > 0)
                {
                    var range = ToList();
                    Clear();
                    AddRange(range);
                }
            }
        }
        bool _CaseSensitive = false;
    }

    // ----------------------------------------------------

    readonly static NameElement XOne = new("one");
    readonly static NameElement XTwo = new("two");
    readonly static NameElement XThree = new("three");
    readonly static NameElement XFour = new("four");
    readonly static NameElement XFive = new("five");

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var source = new ChainElement(false);
        Assert.Empty(source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var items = new ChainElement(false, XOne);
        Assert.Single(items);
        Assert.Same(XOne, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        Assert.Equal(3, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);

        try { items = new ChainElement(false, new[] { XOne, new NameElement("ONE") }); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.CaseSensitive, target.CaseSensitive);
        Assert.Same(XOne, target[0]);
        Assert.Same(XTwo, target[1]);
        Assert.Same(XThree, target[2]);

        source.CaseSensitive = !source.CaseSensitive;
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.CaseSensitive, target.CaseSensitive);
    }

#pragma warning disable xUnit2017
    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        List<int> list;

        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        Assert.True(items.Contains(new NameElement("ONE")));
        Assert.Equal(1, items.IndexOf(new NameElement("TWO")));
        Assert.True(items.Contains(x => x is NameElement named && named.Name.Contains('w')));
        list = items.IndexesOf(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);

        items = new ChainElement(true, new[] { XOne, new NameElement("oNe"), XThree });
        Assert.Equal(-1, items.LastIndexOf(new NameElement("ONE")));
    }
#pragma warning restore

    //[Enforced]
    [Fact]
    public static void Test_Change_Settings()
    {
        var items = new ChainElement(true, new[] { XOne, new NameElement("oNe") });
        try { items.CaseSensitive = false; Assert.Fail(); }
        catch (DuplicateException) { }

        items = new(true, new[] { XOne, new NameElement("oNe") });
        items.Behavior = CoreListBehavior.Ignore;
        items.Add(XOne);
        Assert.Equal(2, items.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var list = items.GetRange(1, 0);
        Assert.Empty(list);

        list = items.GetRange(1, 2);
        Assert.Equal(2, list.Count);
        Assert.Same(XTwo, list[0]);
        Assert.Same(XThree, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceItem()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items[0] = XFour;
        Assert.Equal(3, items.Count);
        Assert.Same(XFour, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);

        try { items[0] = new NameElement("THREE"); Assert.Fail(); }
        catch (DuplicateException) { }

        items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Ignore;
        items[0] = new NameElement("THREE");
        Assert.Same(XOne, items[0]);

        items.Behavior = CoreListBehavior.Add;
        items[0] = new NameElement("THREE");
        Assert.Equal("THREE", ((NameElement)items[0]).Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceItem_Many()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var other = new ChainElement(false, new[] { XFour, XFive });
        items[0] = other;
        Assert.Equal(4, items.Count);
        Assert.Same(XFour, items[0]);
        Assert.Same(XFive, items[1]);
        Assert.Same(XTwo, items[2]);
        Assert.Same(XThree, items[3]);

        items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Ignore;
        other = new ChainElement(true, new[] { XTwo, new NameElement("THREE") });
        items[0] = other;
        Assert.Equal(3, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var done = items.Add(XFour);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
        Assert.Same(XFour, items[3]);

        try { items.Add(new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }

        items.Behavior = CoreListBehavior.Ignore;
        items.Add(new NameElement("TWO"));
        Assert.Equal(4, items.Count);

        items.Behavior = CoreListBehavior.Add;
        items.Add(new NameElement("TWO"));
        Assert.Equal(5, items.Count);
    }
}
/*

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Add(T item)
    {
        if (Flatten && item is IEnumerable<T> range) return AddRange(range);

        item = Validator(item, true);

        if (Behavior is CoreListBehavior.Throw or CoreListBehavior.Ignore)
        {
            var temp = IndexOf(item);
            if (temp >= 0)
            {
                if (Behavior == CoreListBehavior.Ignore) return 0;

                throw new DuplicateException(
                    "The element to add is a duplicate one.")
                    .WithData(item)
                    .WithData(this);
            }
        }

        Items.Add(item);
        return 1;
    }
    int IList.Add(object? value)
    {
        var index = Count; Add((T)value!);
        return index;
    }
    void ICollection<T>.Add(T item) => Add(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var count = 0; foreach (var item in range)
        {
            var temp = Add(item);
            count += temp;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Insert(int index, T item)
    {
        if (Flatten && item is IEnumerable<T> range) return InsertRange(index, range);

        item = Validator(item, true);

        if (Behavior is CoreListBehavior.Throw or CoreListBehavior.Ignore)
        {
            var temp = IndexOf(item);
            if (temp >= 0)
            {
                if (Behavior == CoreListBehavior.Ignore) return 0;

                throw new DuplicateException(
                    "The element to insert is a duplicate one.")
                    .WithData(item)
                    .WithData(this);
            }
        }

        Items.Insert(index, item);
        return 1;
    }
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var count = 0; foreach (var item in range)
        {
            var temp = Insert(index, item);
            count += temp;
            index += temp;
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Remove(T item)
    {
        if (Flatten && item is IEnumerable<T> range)
        {
            var count = 0; foreach (var temp in range)
            {
                var num = Remove(temp);
                count += num;
            }
            return count;
        }
        else
        {
            var index = IndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }
    void IList.Remove(object? value) => Remove((T)value!);
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int RemoveLast(T item)
    {
        if (Flatten && item is IEnumerable<T> range)
        {
            var count = 0; foreach (var temp in range)
            {
                var num = RemoveLast(temp);
                count += num;
            }
            return count;
        }
        else
        {
            var index = LastIndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int RemoveAll(T item)
    {
        if (Flatten && item is IEnumerable<T> range)
        {
            var count = 0; foreach (var temp in range)
            {
                var num = RemoveAll(temp);
                count += num;
            }
            return count;
        }
        else
        {
            var count = 0; while (true)
            {
                var temp = IndexOf(item);

                if (temp >= 0) count += RemoveAt(temp);
                else break;
            }
            return count;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int RemoveRange(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int Remove(Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int RemoveLast(Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int RemoveAll(Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var count = 0; while (true)
        {
            var index = IndexOf(predicate);
            if (index < 0) break;

            count += RemoveAt(index);
        }
        return count;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public int Clear()
    {
        var count = Count; if (count > 0) Items.Clear();
        return count;
    }
    void ICollection<T>.Clear() => Clear();
    void IList.Clear() => Clear();
 */