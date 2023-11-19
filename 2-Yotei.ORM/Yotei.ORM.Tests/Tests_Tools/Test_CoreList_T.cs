namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_CoreList_T
{
    public class Element(string name)
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }

    [Cloneable]
    public partial class Chain : CoreList<Element>
    {
        public Chain(bool sensitive) => CaseSensitive = sensitive;
        public Chain(bool sensitive, Element item) : this(sensitive) => Add(item);
        public Chain(bool sensitive, IEnumerable<Element> range) : this(sensitive) => AddRange(range);
        protected Chain(Chain source)
        {
            CaseSensitive = source.CaseSensitive;
            AddRange(source);
        }

        public override Element Validate(Element item)
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
        public override bool Compare(Element source, Element target)
            => string.Compare(source.Name, target.Name, !CaseSensitive) == 0;

        public bool CaseSensitive
        {
            get => _CaseSensitive;
            set
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
        var chain = new Chain(true, [xone, new("ONE")]);
        Assert.Equal(2, chain.Count);

        try { chain.CaseSensitive = false; Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Surrogate_Element()
    {
        var chain = new Chain(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, chain.IndexOf(new Element("any")));

        Assert.Equal(0, chain.IndexOf(new Element("one")));
        Assert.Equal(3, chain.LastIndexOf(new Element("one")));

        var list = chain.IndexesOf(new Element("one"));
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
        var chain = new Chain(false, [xone, xtwo, xthree, xone]);
        
        var range = chain.GetRange(0, 0);
        Assert.Empty(range);

        range = chain.GetRange(1, 2);
        Assert.Equal(2, range.Count);
        Assert.Same(xtwo, range[0]);
        Assert.Same(xthree, range[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Setter()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        chain[0] = xone;
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain[0] = new("one");
        Assert.Equal(3, chain.Count);
        Assert.NotSame(xone, chain[0]); Assert.Equal("one", chain[0].Name);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        try { chain[0] = new("two"); Assert.Fail(); }
        catch (DuplicateException) { }

        try { chain[0] = null!; Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { chain[0] = new(""); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Method()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var done = chain.Replace(0, xone);
        Assert.Equal(0, done);

        done = chain.Replace(0, new("one"));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.NotSame(xone, chain[0]); Assert.Equal("one", chain[0].Name);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        try { chain.Replace(0, new("two")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { chain.Replace(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { chain.Replace(0, new("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var done = chain.Add(xfour);
        Assert.Equal(1, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);

        chain = new Chain(false, [xone, xtwo, xthree]);
        done = chain.Add(xone);
        Assert.Equal(1, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xone, chain[3]);

        try { chain.Add(new("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { chain.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { chain.Add(new("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var done = chain.AddRange([]);
        Assert.Equal(0, done);
        Assert.Equal(3, chain.Count);

        chain = new Chain(false, [xone, xtwo, xthree]);
        done = chain.AddRange([xfour, xfive]);
        Assert.Equal(2, done);
        Assert.Equal(5, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);
        Assert.Same(xfive, chain[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var done = chain.Insert(3, xfour);
        Assert.Equal(1, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);

        chain = new Chain(false, [xone, xtwo, xthree]);
        done = chain.Insert(3, xone);
        Assert.Equal(1, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xone, chain[3]);

        try { chain.Insert(3, new("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { chain.Insert(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { chain.Insert(3, new("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var done = chain.InsertRange(3, []);
        Assert.Equal(0, done);
        Assert.Equal(3, chain.Count);

        chain = new Chain(false, [xone, xtwo, xthree]);
        done = chain.InsertRange(3, [xfour, xfive]);
        Assert.Equal(2, done);
        Assert.Equal(5, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);
        Assert.Same(xfive, chain[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var done = chain.RemoveAt(0);
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var done = chain.RemoveRange(0, 0);
        Assert.Equal(0, done);

        done = chain.RemoveRange(1, 0);
        Assert.Equal(0, done);

        done = chain.RemoveRange(1, 2);
        Assert.Equal(2, done);
        Assert.Single(chain);
        Assert.Same(xone, chain[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var chain = new Chain(false, [xone, xtwo, xthree, xone]);
        var done = chain.Remove(new Element("any"));
        Assert.Equal(0, done);
        Assert.Equal(4, chain.Count);

        chain = new Chain(false, [xone, xtwo, xthree, xone]);
        done = chain.Remove(new Element("one"));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
        Assert.Same(xone, chain[2]);

        chain = new Chain(false, [xone, xtwo, xthree, xone]);
        done = chain.RemoveLast(new Element("one"));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain(false, [xone, xtwo, xthree, xone]);
        done = chain.RemoveAll(new Element("one"));
        Assert.Equal(2, done);
        Assert.Equal(2, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var chain = new Chain(false, [xone, xtwo, xthree, xone]);
        var done = chain.Remove(x => x.Name.Contains('z'));
        Assert.Equal(0, done);
        Assert.Equal(4, chain.Count);

        chain = new Chain(false, [xone, xtwo, xthree, xone]);
        done = chain.Remove(x => x.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
        Assert.Same(xone, chain[2]);

        chain = new Chain(false, [xone, xtwo, xthree, xone]);
        done = chain.RemoveLast(x => x.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain(false, [xone, xtwo, xthree, xone]);
        done = chain.RemoveAll(x => x.Name.Contains('e'));
        Assert.Equal(3, done);
        Assert.Single(chain);
        Assert.Same(xtwo, chain[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var chain = new Chain(false);
        var done = chain.Clear();
        Assert.Equal(0, done);

        chain = new Chain(false, [xone, xtwo, xthree, xone]);
        done = chain.Clear();
        Assert.Equal(4, done);
        Assert.Empty(chain);
    }
}