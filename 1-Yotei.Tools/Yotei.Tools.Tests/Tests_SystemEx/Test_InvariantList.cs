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
        public ChainElement(bool sensitive)
        {
            CaseSensitive = sensitive;
            Validate = (item, _) =>
            {
                ArgumentNullException.ThrowIfNull(item);
                if (item is NameElement named) named.Name.NotNullNotEmpty();
                return item;
            };
            Compare = (inner, other) =>
            {
                return inner is NameElement inamed && other is NameElement onamed
                    ? string.Compare(inamed.Name, onamed.Name, !CaseSensitive) == 0
                    : ReferenceEquals(inner, other);
            };
            AcceptDuplicate = (item) =>
            {
                throw new DuplicateException("Duplicated element.").WithData(item).WithData(this);
            };
            ExpandNested = (item) => true;
        }
        public ChainElement(bool sensitive, IElement item) : this(sensitive) => Items.Add(item);
        public ChainElement(bool sensitive, IEnumerable<IElement> range) : this(sensitive) => Items.AddRange(range);
        protected ChainElement(ChainElement source) : this(source.CaseSensitive) => Items.AddRange(source);
        public override ChainElement Clone() => new(this);
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
        var items = new ChainElement(false, xone);
        Assert.Single(items);
        Assert.Same(xone, items[0]);

        try { _ = new ChainElement(false, (IElement)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new ChainElement(false, new NameElement("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new ChainElement(false, (IEnumerable<IElement>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new ChainElement(false, [xone, new NameElement("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_ChangeBehaviors()
    {
        try
        {
            var items = new ChainElement(true, [xone, new NameElement("ONE")])
            { CaseSensitive = false };
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        Assert.Equal(0, items.IndexOf(new NameElement("one")));
        Assert.Equal(1, items.IndexOf(new NameElement("TWO")));
        Assert.Equal(2, items.IndexOf(new NameElement("THRee")));

        var list = items.IndexesOf(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(2, list.Count);
        Assert.Same(xone, items[list[0]]);
        Assert.Same(xthree, items[list[1]]);

        items = new ChainElement(true, [xone, new NameElement("ONE")]);
        Assert.Equal(-1, items.LastIndexOf(new NameElement("OnE")));
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Strict()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        Assert.Equal(0, items.IndexOf(xone, true));
        Assert.Equal(1, items.IndexOf(xtwo, true));
        Assert.Equal(2, items.IndexOf(xthree, true));

        Assert.Equal(-1, items.IndexOf(new NameElement("one"), true));
        Assert.Equal(-1, items.IndexOf(new NameElement("TWO"), true));
        Assert.Equal(-1, items.IndexOf(new NameElement("THRee"), true));
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(0, source.Count);
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
        var target = source.Replace(0, new NameElement("ONE"));
        Assert.Same(source, target);

        target = source.Replace(0, xfour);
        Assert.Equal(3, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { _ = source.Replace(0, new NameElement("THREE")); Assert.Fail(); }
        catch (DuplicateException) { }

        // Because 'strict' is true we don't consider xone equal to 'ONE', and so we replace it.
        target = source.Replace(0, new NameElement("ONE"), true);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same("ONE", ((NameElement)target[0]).Name);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Many()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Replace(0, new ChainElement(false, [xfour, xfive]));
        Assert.Same(xfour, target[0]);
        Assert.Same(xfive, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new ChainElement(false, [xone, xtwo, xthree]);
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
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var target = source.Add(new ChainElement(false));
        Assert.Same(source, target);

        target = source.Add(new ChainElement(false, [xfour, xfive]));
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
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var target = source.AddRange([xfour, xfive]);
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
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var target = source.AddRange([xfour, new ChainElement(false, [xfive, xsix])]);
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
        var source = new ChainElement(false, [xone, xtwo, xthree]);
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
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var target = source.Insert(0, new ChainElement(false));
        Assert.Same(source, target);

        target = source.Insert(0, new ChainElement(false, [xfour, xfive]));
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
        var source = new ChainElement(false, [xone, xtwo, xthree]);
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
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var target = source.InsertRange(0, [xfour, new ChainElement(false, [xfive, xsix])]);
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
    public static void Test_RemoveRange()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.RemoveRange(0, 0);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveRange(0, 2);
        Assert.Single(target);
        Assert.Same(xthree, target[0]);
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
    public static void Test_Remove_Strict()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Remove(new NameElement("ONE"), true);
        Assert.Same(source, target);

        target = source.Remove(xone, true);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Many()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Remove(new ChainElement(false, new NameElement[] { new("ONE"), new("TWO") }));
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
    public static void Test_Clear()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Clear();
        Assert.Empty(target);
    }
}