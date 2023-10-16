namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_InvariantList
{
    public interface IElement { }
    public class NameElement(string name) : IElement
    {
        public override string ToString() => Name;
        public string Name { get; set; } = name;
    }
    public class ChainElement : InvariantList<IElement>, IElement
    {
        protected class InnerList : CoreList<IElement>
        {
            ChainElement Master;
            public InnerList(ChainElement master)
            {
                Master = master.ThrowWhenNull();
                Validate = (item, add) =>
                {
                    ArgumentNullException.ThrowIfNull(item);
                    if (item is NameElement named) named.Name.NotNullNotEmpty();
                    return item;
                };
                Compare = (inner, other) =>
                {
                    return inner is NameElement inamed && other is NameElement onamed
                        ? string.Compare(inamed.Name, onamed.Name, !Master.CaseSensitive) == 0
                        : ReferenceEquals(inner, other);
                };
                AcceptDuplicate = (item) =>
                {
                    throw new DuplicateException("Duplicated element.")
                        .WithData(item)
                        .WithData(this);
                };
                ExpandNested = (item) => true;
            }
        }
        protected override InnerList CreateItems() => new(this);
        public ChainElement(bool sensitive) : base() => CaseSensitive = sensitive;
        public ChainElement(bool sensitive, IElement item) : this(sensitive) => Items.Add(item);
        public ChainElement(bool sensitive, IEnumerable<IElement> range) : this(sensitive) => Items.AddRange(range);
        protected ChainElement(ChainElement source) : this(source.CaseSensitive) => Items.AddRange(source);
        public override ChainElement Clone() => new(this);
        public bool CaseSensitive
        {
            get => _CaseSensitive;
            set
            {
                if (_CaseSensitive == value) return;
                _CaseSensitive = value;

                if (Count > 0)
                {
                    var range = ToArray();
                    Items.Clear();
                    Items.AddRange(range);
                }
            }
        }
        bool _CaseSensitive = false;
    }

    // ----------------------------------------------------

    static NameElement xone = new("one");
    static NameElement xtwo = new("two");
    static NameElement xthree = new("three");
    static NameElement xfour = new("four");
    static NameElement xfive = new("five");
    static NameElement xsix = new("six");

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new ChainElement(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        NameElement item;

        var items = new ChainElement(false, xone);
        Assert.Single(items);
        item = Assert.IsType<NameElement>(items[0]); Assert.Equal("one", item.Name);

        try { _ = new ChainElement(false, (IElement)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new ChainElement(false, new NameElement("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new ChainElement(false, new NameElement[] { xone, new("ONE") }); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new ChainElement(true, new[] { xone, xtwo, xthree });
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.CaseSensitive, target.CaseSensitive);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Settings()
    {
        var source = new ChainElement(true, new[] { xone, new("ONE") });
        try
        {
            _ = new ChainElement(true, (IEnumerable<IElement>)source) { CaseSensitive = false };
            Assert.Fail();
        }
        catch (DuplicateException) { }

    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        Assert.Equal(0, items.IndexOf(new NameElement("one")));
        Assert.Equal(1, items.IndexOf(new NameElement("TWO")));
        Assert.Equal(2, items.IndexOf(new NameElement("thrEE")));

        var list = items.IndexesOf(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(2, list.Count);
        Assert.Same(xone, items[list[0]]);
        Assert.Same(xthree, items[list[1]]);

        items = new ChainElement(true, new NameElement[] { xone, new("ONE") });
        Assert.Equal(-1, items.LastIndexOf(new NameElement("OnE")));
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.GetRange(1, 0);
        Assert.Empty(target);

        target = source.GetRange(0, 3);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Replace(0, xfour);
        Assert.Equal(3, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { _ = source.Replace(0, new NameElement("THREE")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Many()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Replace(0, new ChainElement(false, new[] { xfour, xfive }));
        Assert.Equal(4, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xfive, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);

        try { _ = source.Replace(0, new NameElement("THREE")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Add(xfour);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Many()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Add(new ChainElement(false));
        Assert.Same(source, target);

        target = source.Add(new ChainElement(false, new[] { xfour, xfive }));
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.AddRange(new[] { xfour, xfive });
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Many()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.AddRange(
            new IElement[] { xfour, new ChainElement(false, new[] { xfive, xsix }) });

        Assert.Equal(6, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
        Assert.Same(xsix, target[5]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Insert(0, xfour);
        Assert.Equal(4, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);

        try { source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(0, new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Many()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Insert(0, new ChainElement(false));
        Assert.Same(source, target);

        target = source.Insert(0, new ChainElement(false, new[] { xfour, xfive }));
        Assert.Equal(5, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xfive, target[1]);
        Assert.Same(xone, target[2]);
        Assert.Same(xtwo, target[3]);
        Assert.Same(xthree, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.InsertRange(0, new[] { xfour, xfive });
        Assert.Equal(5, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xfive, target[1]);
        Assert.Same(xone, target[2]);
        Assert.Same(xtwo, target[3]);
        Assert.Same(xthree, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Many()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.InsertRange(0,
            new IElement[] { xfour, new ChainElement(false, new[] { xfive, xsix }) });

        Assert.Equal(6, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xfive, target[1]);
        Assert.Same(xsix, target[2]);
        Assert.Same(xone, target[3]);
        Assert.Same(xtwo, target[4]);
        Assert.Same(xthree, target[5]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.RemoveAt(0);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Remove(new NameElement("x"));
        Assert.Same(source, target);

        target = source.Remove(new NameElement("ONE"));
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Many()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Remove(new ChainElement(false));
        Assert.Same(source, target);

        target = source.Remove(new ChainElement(false, new NameElement[] { new("ONE"), new("TWO") }));
        Assert.Single(target);
        Assert.Same(xthree, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree, xfour });
        var target = source.Remove(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xfour, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Predicate()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree, xfour });
        var target = source.RemoveLast(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xfour, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Predicate()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree, xfour });
        var target = source.RemoveAll(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xfour, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 2);
        Assert.Single(target);
        Assert.Same(xthree, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Clear();
        Assert.Empty(target);
    }
}