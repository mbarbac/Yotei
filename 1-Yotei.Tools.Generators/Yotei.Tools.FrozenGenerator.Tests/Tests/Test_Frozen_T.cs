namespace Yotei.Tools.FrozenGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_FrozenIFace_T
{
    public class Element(string name)
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }

    static readonly Element xone = new("one");
    static readonly Element xtwo = new("two");
    static readonly Element xthree = new("three");
    static readonly Element xfour = new("four");
    static readonly Element xfive = new("five");

    // ====================================================

    [Cloneable]
    public partial class Builder : CoreList<Element>
    {
        public Builder(bool sensitive)
        {
            Sensitive = sensitive;
            ValidateItem = (x) => { x.ThrowWhenNull().Name.NotNullNotEmpty(); return x; };
            CompareItems = (x, y) => string.Compare(x.ThrowWhenNull().Name, y.ThrowWhenNull().Name, !Sensitive) == 0;
            GetDuplicates = IndexesOf;
            CanInclude = (x, y) => ReferenceEquals(x, y)
                ? true
                : throw new DuplicateException("Duplicated element.").WithData(x);
        }
        public Builder(bool sensitive, Element item) : this(sensitive) => Add(item);
        public Builder(bool sensitive, IEnumerable<Element> range) : this(sensitive) => AddRange(range);
        protected Builder(Builder source) : this(source.Sensitive) => AddRange(source);

        public bool Sensitive
        {
            get => _Sensitive;
            set
            {
                if (_Sensitive == value) return;
                _Sensitive = value;
                Reload();
            }
        }
        bool _Sensitive;
    }

    // ====================================================

    [IFrozenList<Element>]
    public partial interface IChain : IEquatable<IChain>
    {
        bool Sensitive { get; }
    }

    // ----------------------------------------------------

    [FrozenList<Element>]
    public partial class Chain : IChain
    {
        protected override Builder Items { get; }

        public Chain(bool sensitive) => Items = new(sensitive);
        public Chain(bool sensitive, Element item) : this(sensitive) => Items.Add(item);
        public Chain(bool sensitive, IEnumerable<Element> range) : this(sensitive) => Items.AddRange(range);
        protected Chain(Chain source) : this(source.Sensitive) => Items.AddRange(source);

        public bool Sensitive => Items.Sensitive;

        public bool Equals(IChain? other)
        {
            if (other is null) return false;

            if (Sensitive != other.Sensitive) return false;
            if (Count != other.Count) return false;
            for (int i = 0; i < Count; i++)
                if (!Items.CompareItems(Items[i], other[i])) return false;

            return true;
        }
        public override bool Equals(object? obj) => Equals(obj as IChain);
        public static bool operator ==(Chain x, IChain y) => x is not null && x.Equals(y);
        public static bool operator !=(Chain x, IChain y) => !(x == y);
        public override int GetHashCode()
        {
            var code = HashCode.Combine(0);
            for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
            return code;
        }
    }

    // ====================================================

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new Chain(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var items = new Chain(false, xone);
        Assert.Single(items);
        Assert.Same(xone, items[0]);

        try { _ = new Chain(false, (Element)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(false, new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var items = new Chain(false, []);
        Assert.Empty(items);

        items = new Chain(false, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new Chain(false, (IEnumerable<Element>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(false, [xone, null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Chain(false, [xone, new Element("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_With_Duplicates()
    {
        var items = new Chain(false, [xone, xone]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);

        try { _ = new Chain(false, [xone, new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Chain(false, [xone, new Element("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }

        items = new Chain(true, [xone, new Element("ONE")]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Equal("ONE", items[1].Name);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Equals()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);
        var target = source.Clone();
        Assert.True(source == target);
        Assert.False(source != target);

        target = new Chain(false, [xone, new("TWO"), xthree, xone]);
        Assert.True(source == target);
        Assert.False(source != target);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(xfive));

        Assert.Equal(0, items.IndexOf(new Element("one")));
        Assert.Equal(0, items.IndexOf(new Element("ONE")));

        Assert.Equal(3, items.LastIndexOf(new Element("one")));
        Assert.Equal(3, items.LastIndexOf(new Element("ONE")));

        var list = items.IndexesOf(new Element("one"));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.IndexesOf(new Element("ONE"));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var items = new Chain(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(x => x.Name.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => x.Name.Contains('n')));
        Assert.Equal(3, items.LastIndexOf(x => x.Name.Contains('n')));

        var list = items.IndexesOf(x => x.Name.Contains('n'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.GetRange(0, 4);
        Assert.Same(source, target);

        target = source.GetRange(0, 0);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        try { source.GetRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { source.GetRange(5, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { source.GetRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.GetRange(0, 5); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(1, xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal(xone, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);

        try { _ = source.Replace(1, new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Replace(1, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Replace(1, new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Chain(false, [xone, xtwo]);

        var target = source.Add(xthree);
        Assert.NotSame(source, target);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.Add(new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Add(new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Chain(false, [xone, xtwo]);

        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        target = source.AddRange([xone]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.AddRange([new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.AddRange([xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.AddRange([xfive, new Element("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Chain(false, [xone, xtwo]);

        var target = source.Insert(2, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.Insert(2, xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.Insert(2, new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Insert(2, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Insert(2, new Element("")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = source.Insert(-1, xfive); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { _ = source.Insert(3, xfive); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Chain(false, [xone, xtwo]);

        var target = source.InsertRange(2, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        target = source.InsertRange(2, [xone]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.InsertRange(2, [new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.InsertRange(2, [xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.InsertRange(2, [xfive, new Element("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveRange(0, 4);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(4, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { _ = source.RemoveRange(0, 5); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xone]);

        var target = source.Remove(new Element("four"));
        Assert.Same(source, target);

        target = source.Remove(new Element("one"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.Remove(new Element("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast(new Element("one"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(new Element("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(new Element("one"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAll(new Element("ONE"));
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

        target = source.Remove(x => x.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast(x => x.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => x.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
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