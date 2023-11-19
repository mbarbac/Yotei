namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_InvariantList_KT
{
    public class Element(string name)
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }

    [Cloneable]
    public partial class Chain : InvariantList<string, Element>
    {
        public Chain(bool sensitive) : base() => CaseSensitive = sensitive;
        public Chain(bool sensitive, Element item) : this(sensitive) => Items.Add(item);
        public Chain(bool sensitive, IEnumerable<Element> range) : this(sensitive) => Items.AddRange(range);
        protected Chain(Chain source) : this(source.CaseSensitive) => Items.AddRange(source.Items);

        public override Element ValidateItem(Element item)
        {
            item.ThrowWhenNull();
            item.Name.NotNullNotEmpty();
            return item;
        }
        public override bool CanDuplicate(Element source, Element target)
        {
            if (ReferenceEquals(source, target)) return true;
            throw new DuplicateException("Duplicated element.").WithData(target);
        }
        public override string GetKey(Element item) => item.Name;
        public override string ValidateKey(string key) => key.NotNullNotEmpty();
        public override bool Compare(string source, string target)
            => string.Compare(source, target, !CaseSensitive) == 0;

        [WithGenerator]
        public bool CaseSensitive
        {
            get => _CaseSensitive;
            init
            {
                if (_CaseSensitive == value) return;
                _CaseSensitive = value;

                if (Count == 0) return;
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
        bool _CaseSensitive;
    }

    // ----------------------------------------------------

    static readonly Element xone = new("one");
    static readonly Element xtwo = new("two");
    static readonly Element xthree = new("three");
    static readonly Element xfour = new("four");
    static readonly Element xfive = new("five");
    static readonly Element xsix = new("six");

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var chain = new Chain(false);
        Assert.Empty(chain);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var chain = new Chain(false, xone);
        Assert.Single(chain);
        Assert.Same(xone, chain[0]);

        try { _ = new Chain(false, (Element)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(false, (Element)new("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var chain = new Chain(false, []);
        Assert.Empty(chain);

        chain = new Chain(false, [xone, xtwo, xthree]);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        try { _ = new Chain(false, (IEnumerable<Element>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(false, [xone, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(false, [xone, new("one")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_With_Duplicates()
    {
        var chain = new Chain(false, [xone, xtwo, xthree, xone]);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xone, chain[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Settings()
    {
        var source = new Chain(true, [xone, new("ONE")]);
        Assert.Equal(2, source.Count);

        try { _ = source.WithCaseSensitive(false); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Key()
    {
        var chain = new Chain(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, chain.IndexOf("any"));

        Assert.Equal(0, chain.IndexOf("one"));
        Assert.Equal(3, chain.LastIndexOf("one"));

        var list = chain.IndexesOf("one");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var chain = new Chain(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, chain.IndexOf(x => x.Name.Contains('z')));

        Assert.Equal(0, chain.IndexOf(x => x.Name.Contains('e')));
        Assert.Equal(3, chain.LastIndexOf(x => x.Name.Contains('e')));

        var list = chain.IndexesOf(x => x.Name.Contains('e'));
        Assert.Equal(3, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Method()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(0, new("one"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotSame(xone, target[0]); Assert.Equal("one", target[0].Name);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { source.Replace(0, new("two")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Replace(0, new("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.Add(xfour);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);

        try { source.Add(new("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(new("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xfour, xfive]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.Insert(3, xfour);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        target = source.Insert(3, xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);

        try { source.Insert(3, new("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Insert(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(3, new("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.InsertRange(3, []);
        Assert.Same(source, target);

        target = source.InsertRange(3, [xfour, xfive]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
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
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(1, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);
        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("one");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast("one");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll("one");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);
        var target = source.Remove(x => x.Name.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => x.Name.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast(x => x.Name.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => x.Name.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xtwo, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain(false);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain(false, [xone, xtwo, xthree, xone]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}