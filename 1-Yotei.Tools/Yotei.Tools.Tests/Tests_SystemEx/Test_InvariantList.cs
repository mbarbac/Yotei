namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_InvariantList
{
    public interface IElement { }
    public class NameElement : IElement
    {
        public NameElement(string name) => Name = name;
        public override string ToString() => Name;
        public string Name { get; set; }
    }
    public class ChainElement : InvariantList<IElement>, IElement
    {
        public ChainElement(bool sensitive) : base() => CaseSensitive = sensitive;
        public ChainElement(bool sensitive, IElement item) : this(sensitive) => AddInternal(item);
        public ChainElement(bool sensitive, IEnumerable<IElement> range) : this(sensitive) => AddRangeInternal(range);
        public override ChainElement Clone() => new(CaseSensitive, (IEnumerable<IElement>)this);

        public bool CaseSensitive
        {
            get => _CaseSensitive;
            init
            {
                if (_CaseSensitive == value) return;
                _CaseSensitive = value;

                var range = ToList();
                ClearInternal();
                AddRangeInternal(range);
            }
        }
        bool _CaseSensitive = false;

        protected override IElement Validate(IElement item, bool _) => item.ThrowWhenNull();
        protected override bool Compare(IElement x, IElement y)
            => x is NameElement nx && y is NameElement ny
            ? string.Compare(nx.Name, ny.Name, !CaseSensitive) == 0
            : ReferenceEquals(x, y);
        protected override bool IgnoreDuplicate(IElement _) => false;
        protected override void ThrowWhenDuplicate(IElement item)
            => throw new DuplicateException("Duplicate item.").WithData(item);
        protected override bool ExpandNested => true;
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
        var items = new ChainElement(false);
        Assert.Empty(items);
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

        try { items = new(false, new[] { XOne, new NameElement("ONE") }); Assert.Fail(); }
        catch (DuplicateException) { }
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

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.Clone();
        Assert.Equal(source.CaseSensitive, target.CaseSensitive);
        Assert.Same(XOne, target[0]);
        Assert.Same(XTwo, target[1]);
        Assert.Same(XThree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Settings()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree })
        {
            CaseSensitive = true
        };
        Assert.Equal(3, source.Count);
        Assert.Same(XOne, source[0]);
        Assert.Same(XTwo, source[1]);
        Assert.Same(XThree, source[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.GetRange(1, 0);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Same(XTwo, target[0]);
        Assert.Same(XThree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceItems()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.ReplaceItem(0, XFour);
        Assert.NotSame(source, target);
        Assert.Same(XFour, target[0]);
        Assert.Same(XTwo, target[1]);
        Assert.Same(XThree, target[2]);

        try { target.ReplaceItem(0, new NameElement("THREE")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceItemsMany()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var other = new ChainElement(false, new[] { XFour, XFive });

        var target = source.ReplaceItem(0, other);
        Assert.NotSame(source, target);
        Assert.Same(XFour, target[0]);
        Assert.Same(XFive, target[1]);
        Assert.Same(XTwo, target[2]);
        Assert.Same(XThree, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.Add(XFour);
        Assert.Same(XOne, target[0]);
        Assert.Same(XTwo, target[1]);
        Assert.Same(XThree, target[2]);
        Assert.Same(XFour, target[3]);

        try { target.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { target.Add(new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddMany()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var other = new ChainElement(false, new[] { XFour, XFive });

        var target = source.Add(other);
        Assert.Equal(5, target.Count);
        Assert.Same(XOne, target[0]);
        Assert.Same(XTwo, target[1]);
        Assert.Same(XThree, target[2]);
        Assert.Same(XFour, target[3]);
        Assert.Same(XFive, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.AddRange(new[] { XFour, XFive });
        Assert.Equal(5, target.Count);
        Assert.Same(XOne, target[0]);
        Assert.Same(XTwo, target[1]);
        Assert.Same(XThree, target[2]);
        Assert.Same(XFour, target[3]);
        Assert.Same(XFive, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.Insert(0, XFour);
        Assert.Equal(4, target.Count);
        Assert.Same(XFour, target[0]);
        Assert.Same(XOne, target[1]);
        Assert.Same(XTwo, target[2]);
        Assert.Same(XThree, target[3]);

        try { target.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertMany()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var other = new ChainElement(false, new[] { XFour, XFive });

        var target = source.Insert(0, other);
        Assert.Equal(5, target.Count);
        Assert.Same(XFour, target[0]);
        Assert.Same(XFive, target[1]);
        Assert.Same(XOne, target[2]);
        Assert.Same(XTwo, target[3]);
        Assert.Same(XThree, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.InsertRange(0, new[] { XFour, XFive });
        Assert.Equal(5, target.Count);
        Assert.Same(XFour, target[0]);
        Assert.Same(XFive, target[1]);
        Assert.Same(XOne, target[2]);
        Assert.Same(XTwo, target[3]);
        Assert.Same(XThree, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.RemoveAt(1);
        Assert.Equal(2, target.Count);
        Assert.Same(XOne, target[0]);
        Assert.Same(XThree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveItem()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.Remove(new NameElement("TWO"));
        Assert.Equal(2, target.Count);
        Assert.Same(XOne, target[0]);
        Assert.Same(XThree, target[1]);

        target = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var other = target.Remove(XFour);
        Assert.Same(target, other);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveItemMany()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var other = new ChainElement(false, new[] { XTwo, XThree });

        var target = source.Remove(other);
        Assert.Single(target);
        Assert.Same(XOne, target[0]);

        target = new ChainElement(false, new[] { XOne, XTwo, XThree });
        other = new ChainElement(false, new[] { XFour, XFive });
        var another = target.Remove(other);
        Assert.Same(target, another);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.Clear();
        Assert.Empty(target);
    }
}