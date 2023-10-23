namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_InvariantList
{
    // The generic element type...
    public interface IElement { }

    // The core element type, which is identified by its string key...
    public class NameElement(string name) : IElement
    {
        public override string ToString() => Name;
        public string Name { get; set; } = name;
    }

    // A nested element type...
    public class ChainElement : InvariantList<IElement, string>, IElement
    {
        public ChainElement(bool sensitive) => CaseSensitive = sensitive;
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

        public ChainElement WithCaseSensitive(bool value) => new(this)
        {
            CaseSensitive = value,
        };

        public override IElement ValidateItem(IElement item) => item.ThrowWhenNull();
        public override string GetKey(IElement item)
        {
            return item is NameElement named
                ? named.Name
                : throw new UnExpectedException("Cannot obtain a key from a not named element").WithData(item);
        }
        public override string ValidateKey(string key) => key.NotNullNotEmpty();
        public override bool CompareKeys(string inner, string other)
        {
            return string.Compare(inner, other, !CaseSensitive) == 0;
        }
        public override bool AcceptDuplicated(IElement item)
        {
            if (this.Any(x => ReferenceEquals(x, item))) return true;
            throw new DuplicateException("Duplicated element.").WithData(item);
        }
        public override bool ExpandNested(IElement _) => true;
    }

    // ====================================================

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

        items = new ChainElement(false, [xone, xtwo, xone]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_Sensitive()
    {
        var items = new ChainElement(true, [xone, xtwo, new NameElement("ONE")]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Equal("ONE", ((NameElement)items[2]).Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new ChainElement(false, [xone, xtwo, xone]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_Sensitive()
    {
        var source = new ChainElement(true, [xone, xtwo, new NameElement("ONE")]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Behaviors()
    {
        var items = new ChainElement(true, [xone, xtwo, new NameElement("ONE")]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Equal("ONE", ((NameElement)items[2]).Name);

        try { items.WithCaseSensitive(false); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Behaviors_Sensitive()
    {
        var source = new ChainElement(true, [xone, xtwo, xone]);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xone, source[2]);

        var target = source.WithCaseSensitive(false);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Key()
    {
        var items = new ChainElement(false, [xone, xtwo, xone]);
        Assert.Equal(0, items.IndexOf("ONE"));
        Assert.Equal(1, items.IndexOf("TWO"));
        Assert.Equal(-1, items.IndexOf("x"));

        Assert.Equal(2, items.LastIndexOf("ONE"));

        var list = items.IndexesOf("ONE");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var items = new ChainElement(false, [xone, xtwo, xone]);

        var list = items.IndexesOf(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);

        list = items.IndexesOf(x => x is NameElement named && named.Name == "x");
        Assert.Empty(list);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.GetRange(1, 0);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(0, new NameElement("one"));
        Assert.NotSame(source, target);
        Assert.NotSame(xone, target[0]);
        Assert.Equal("one", ((NameElement)target[0]).Name);

        try { _ = source.Replace(2, new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Many()
    {
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var chain = new ChainElement(false, [xfour, xfive]);

        var target = source.Replace(0, chain);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
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
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }

        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Many()
    {
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var target = source.Add(new ChainElement(false));
        Assert.Same(source, target);

        target = source.Add(new ChainElement(false, [xfour, xone]));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xone, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var target = source.AddRange([xfour, xone]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xone, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Many()
    {
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var target = source.AddRange([xfour, new ChainElement(false, [xfive, xone])]);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
        Assert.Same(xone, target[5]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var target = source.Insert(0, xfour);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);

        try { source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(0, new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }

        target = source.Insert(3, xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Many()
    {
        var source = new ChainElement(false, [xone, xtwo, xthree]);
        var target = source.Insert(0, new ChainElement(false));
        Assert.Same(source, target);

        target = source.Insert(0, new ChainElement(false, [xfour, xfive]));
        Assert.NotSame(source, target);
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
        Assert.NotSame(source, target);
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
        Assert.NotSame(source, target);
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
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree });
        var target = source.RemoveRange(0, source.Count);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.RemoveRange(1, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xthree, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree, xone });
        var target = source.Remove("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new ChainElement(false, new[] { xone, xtwo, xthree, xfour });
        var target = source.Remove(x => x is NameElement named && named.Name.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xfour, target[2]);

        target = source.RemoveLast(x => x is NameElement named && named.Name.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xfour, target[2]);

        target = source.RemoveAll(x => x is NameElement named && named.Name.Contains('e'));
        Assert.NotSame(source, target);
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
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}