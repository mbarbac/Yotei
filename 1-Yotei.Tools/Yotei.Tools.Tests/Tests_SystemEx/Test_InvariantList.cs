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
        protected class Surrogate(ChainElement master) : CoreList<IElement>
        {
            readonly ChainElement Master = master;
            public override IElement Validate(IElement item, bool add)
            {
                ArgumentNullException.ThrowIfNull(item);
                if (item is NameElement named) named.Name.NotNullNotEmpty();
                return item;
            }
            public override bool Equivalent(IElement inner, IElement other)
            {
                return inner is NameElement inamed && other is NameElement onamed
                    ? string.Compare(inamed.Name, onamed.Name, !Master.CaseSensitive) == 0
                    : ReferenceEquals(inner, other);
            }
        }
        protected override Surrogate GetItems() => new(this)
        {
            Behavior = CoreList.Behavior.Throw,
            ExpandNested = true,
        };
        public ChainElement(bool sensitive) => CaseSensitive = sensitive;
        public ChainElement(bool sensitive, IElement item) : this(sensitive) => Items.Add(item);
        public ChainElement(bool sensitive, IEnumerable<IElement> range) : this(sensitive) => Items.AddRange(range);
        protected override ChainElement Clone() => new(CaseSensitive, (IEnumerable<IElement>)this);
        public bool CaseSensitive
        {
            get => _CaseSensitive;
            init
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
}
/*
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
        var items = new ChainElement(true, new[] { xone, new("ONE") });
        try { items.CaseSensitive = false; Assert.Fail(); }
        catch(DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree});
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
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var list = items.GetRange(1, 0);
        Assert.Empty(list);

        list = items.GetRange(1, 2);
        Assert.Equal(2, list.Count);
        Assert.Same(xtwo, list[0]);
        Assert.Same(xthree, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        items[0] = xfour;
        Assert.Equal(3, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { items[0] = new NameElement("THREE"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Many()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        items[0] = new ChainElement(false, new[] { xfour, xfive });
        Assert.Equal(4, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xfive, items[1]);
        Assert.Same(xtwo, items[2]);
        Assert.Same(xthree, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.Add(xfour);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);

        try { items.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Add(new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Many()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.Add(new ChainElement(false));
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        done = items.Add(new ChainElement(false, new[] { xfour, xfive }));
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.AddRange(new[] { xfour, xfive });
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Many()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.AddRange(new IElement[] { xfour, new ChainElement(false, new[] { xfive, xsix }) });
        Assert.Equal(3, done);
        Assert.Equal(6, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);
        Assert.Same(xsix, items[5]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.Insert(0, xfour);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Same(xtwo, items[2]);
        Assert.Same(xthree, items[3]);        

        try { items.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Insert(0, new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Many()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.Insert(0, new ChainElement(false));
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        done = items.Insert(0, new ChainElement(false, new[] { xfour, xfive }));
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xfive, items[1]);
        Assert.Same(xone, items[2]);
        Assert.Same(xtwo, items[3]);
        Assert.Same(xthree, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.InsertRange(0, new[] { xfour, xfive });
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xfive, items[1]);
        Assert.Same(xone, items[2]);
        Assert.Same(xtwo, items[3]);
        Assert.Same(xthree, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Many()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.InsertRange(0, new IElement[] { xfour, new ChainElement(false, new[] { xfive, xsix }) });
        Assert.Equal(3, done);
        Assert.Equal(6, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xfive, items[1]);
        Assert.Same(xsix, items[2]);
        Assert.Same(xone, items[3]);
        Assert.Same(xtwo, items[4]);
        Assert.Same(xthree, items[5]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.RemoveAt(0);
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.Remove(new NameElement("x"));
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        done = items.Remove(new NameElement("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Many()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.Remove(new ChainElement(false, new NameElement[] { new("ONE"), new("TWO") }));
        Assert.Equal(2, done);
        Assert.Single(items);
        Assert.Same(xthree, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree, xfour });
        var done = items.Remove(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xfour, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Predicate()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree, xfour });
        var done = items.RemoveLast(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xfour, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Predicate()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree, xfour });
        var done = items.RemoveAll(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xfour, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.RemoveRange(0, 0);
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        done = items.RemoveRange(0, 2);
        Assert.Equal(2, done);
        Assert.Single(items);
        Assert.Same(xthree, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.Clear();
        Assert.Equal(3, done);
        Assert.Empty(items);
    }
}*/