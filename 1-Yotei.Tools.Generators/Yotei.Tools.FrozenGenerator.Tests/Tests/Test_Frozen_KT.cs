namespace Yotei.Tools.FrozenGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Frozen_KT
{
    public interface IElement<R> { }

    public class Element<R>(string name) : IElement<R>
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }

    static readonly Element<byte> xone = new("one");
    static readonly Element<byte> xtwo = new("two");
    static readonly Element<byte> xthree = new("three");
    static readonly Element<byte> xfour = new("four");
    static readonly Element<byte> xfive = new("five");

    // ----------------------------------------------------

    [IFrozenList(typeof(string), typeof(IElement<byte>))]
    public partial interface IChain<R> : IElement<R>
    {
        new IChain<R> Remove(string? key);
    }

    // ----------------------------------------------------

    [Cloneable]
    public partial class Builder<R> : CoreList<string, IElement<byte>>
    {
        public Builder(bool sensitive) => Sensitive = sensitive;
        public Builder(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;
        public Builder(bool sensitive, IEnumerable<IElement<byte>> range) : this(sensitive) => AddRange(range);
        protected Builder(Builder<R> source) : this(source.Sensitive) => AddRange(source);

        public bool Sensitive { get; }
        public override IElement<byte> ValidateItem(IElement<byte> item) => item.ThrowWhenNull();
        public override string GetKey(IElement<byte> item) => item is Element<R> named
            ? named.Name
            : throw new ArgumentException("Element is not a named instance.").WithData(item);
        public override string ValidateKey(string key) => key.NotNullNotEmpty();
        public override bool CompareKeys(string x, string y) => string.Compare(x, y, !Sensitive) == 0;
        public override bool CanInclude(IElement<byte> item, IElement<byte> source)
            => ReferenceEquals(item, source)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);
    }

    // ----------------------------------------------------

    [FrozenList(typeof(string), typeof(IElement<byte>))]
    public partial class Chain<R> : IChain<R>
    {
        protected override Builder<R> Items => _Items ??= new Builder<R>(Sensitive);
        Builder<R>? _Items = null;

        public Chain(bool sensitive) => Sensitive = sensitive;
        public Chain(bool sensitive, int capacity) : this(sensitive) => Items.Capacity = capacity;
        public Chain(bool sensitive, IEnumerable<IElement<byte>> range) : this(sensitive) => Items.AddRange(range);
        protected Chain(Chain<R> source) : this(source.Sensitive) => Items.AddRange(source);

        public bool Sensitive { get; }


        public override Chain<R> Remove(string? key) => (Chain<R>)base.Remove(key!);
        IChain<R> IChain<R>.Remove(string? key) => Remove(key);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new Chain<byte>(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var items = new Chain<byte>(false, []);
        Assert.Empty(items);

        items = new Chain<byte>(false, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new Chain<byte>(false, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain<byte>(false, [null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Chain<byte>(false, [new Element<byte>("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Duplicates()
    {
        var items = new Chain<byte>(false, [xone, xone]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);

        items = new Chain<byte>(false, [xone, xtwo, xone]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        try { _ = new Chain<byte>(false, [xone, new Element<byte>("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Chain<byte>(true, [xone, new Element<byte>("one")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Extended()
    {
        var other = new Chain<byte>(false, [xtwo, xthree]);
        var items = new Chain<byte>(false, [xone, other]);

        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var items = new Chain<byte>(false, [xone, xtwo, xthree, xone]);
        var target = items.Clone();

        Assert.NotSame(items, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var items = new Chain<byte>(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf("xfive"));

        Assert.Equal(0, items.IndexOf("one"));
        Assert.Equal(0, items.IndexOf("ONE"));

        Assert.Equal(3, items.LastIndexOf("one"));
        Assert.Equal(3, items.LastIndexOf("ONE"));

        var list = items.IndexesOf("one");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.IndexesOf("ONE");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var items = new Chain<byte>(false, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(x => ((Element<byte>)x).Name.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => ((Element<byte>)x).Name.Contains('n')));
        Assert.Equal(3, items.LastIndexOf(x => ((Element<byte>)x).Name.Contains('n')));

        var list = items.IndexesOf(x => ((Element<byte>)x).Name.Contains('n'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Reverse()
    {
        var source = new Chain<byte>(false, [xone, xtwo, xthree, xfour]);
        var target = source.Reverse();

        Assert.NotSame(source, target);
        Assert.Same(xfour, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xone, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Sort()
    {
        var source = new Chain<byte>(false, [xone, xtwo, xthree, xfour]);
        var target = source.Sort(StringComparer.Ordinal);

        Assert.NotSame(source, target);
        Assert.Same(xfour, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xtwo, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new Chain<byte>(false, [xone, xtwo, xthree, xfour]);
        var target = source.GetRange(0, 4);
        Assert.Same(source, target);

        target = source.GetRange(0, 0);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        try { source.GetRange(-1, 0); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.GetRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.GetRange(5, 0); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.GetRange(0, 5); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Chain<byte>(false, [xone, xtwo, xthree]);
        var target = source.Replace(1, xtwo);
        Assert.Same(source, target);

        target = source.Replace(1, xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xone, target[1]);
        Assert.Same(xthree, target[2]);

        try { source.Replace(1, new Element<byte>("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Extended()
    {
        var other = new Chain<byte>(false, [xfour, xfive]);
        var source = new Chain<byte>(false, [xone, xtwo, xthree]);

        var target = source.Replace(1, other);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xfour, target[1]);
        Assert.Same(xfive, target[2]);
        Assert.Same(xthree, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Extended_Empty()
    {
        var other = new Chain<byte>(false);
        var source = new Chain<byte>(false, [xone, xtwo, xthree]);

        var target = source.Replace(1, other);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xthree, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Chain<byte>(false, [xone, xtwo]);
        var target = source.Add(xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { _ = source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Add(new Element<byte>("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Duplicates()
    {
        var source = new Chain<byte>(false, [xone, xtwo]);
        var target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        source = new Chain<byte>(false, [xone, xtwo]);
        try { _ = source.Add(new Element<byte>("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Extended()
    {
        var other = new Chain<byte>(false, []);
        var source = new Chain<byte>(false, [xone, xtwo, xthree]);
        var target = source.Add(other);
        Assert.Same(source, target);

        other = new Chain<byte>(false, [xfour, xfive]);
        target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Chain<byte>(false, [xone, xtwo]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { _ = source.AddRange([new Element<byte>("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.AddRange([xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.AddRange([xfive, new Element<byte>("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Extended()
    {
        var other = new Chain<byte>(false, []);
        var source = new Chain<byte>(false, [xone, xtwo]);
        var target = source.AddRange([other]);
        Assert.Same(source, target);

        other = new Chain<byte>(false, [xfour, xfive]);
        target = source.AddRange([xthree, other]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Chain<byte>(false, [xone, xtwo]);
        var target = source.Insert(2, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { _ = source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Insert(0, new Element<byte>("")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Duplicates()
    {
        var source = new Chain<byte>(false, [xone, xtwo]);
        var target = source.Insert(2, xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        source = new Chain<byte>(false, [xone, xtwo]);
        try { _ = source.Insert(0, new Element<byte>("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Extended()
    {
        var other = new Chain<byte>(false, []);
        var source = new Chain<byte>(false, [xone, xtwo, xthree]);
        var target = source.Insert(3, other);
        Assert.Same(source, target);

        other = new Chain<byte>(false, [xfour, xfive]);
        target = source.Insert(3, other);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Chain<byte>(false, [xone, xtwo]);
        var target = source.InsertRange(2, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { _ = source.InsertRange(0, [new Element<byte>("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.InsertRange(0, [xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.InsertRange(0, [xfive, new Element<byte>("")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Extended()
    {
        var other = new Chain<byte>(false, []);
        var source = new Chain<byte>(false, [xone, xtwo]);
        var target = source.InsertRange(2, [other]);
        Assert.Same(source, target);

        other = new Chain<byte>(false, [xfour, xfive]);
        target = source.InsertRange(2, [xthree, other]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Chain<byte>(false, [xone, xtwo, xthree, xone]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        try { source.RemoveAt(999); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new Chain<byte>(false, [xone, xtwo, xthree, xone]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 4);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        source = new Chain<byte>(false, [xone, xtwo, xthree, xone]);
        target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(5, 0); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = source.RemoveRange(0, 5); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new Chain<byte>(false, [xone, xtwo, xthree, xone]);
        var target = source.Remove("four");
        Assert.Same(source, target);

        target = source.Remove("one");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.Remove("ONE");
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

        target = source.RemoveLast("ONE");
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
        var source = new Chain<byte>(false, [xone, xtwo, xthree, xone]);
        var target = source.Remove(x => ((Element<byte>)x).Name.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => ((Element<byte>)x).Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast(x => ((Element<byte>)x).Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => ((Element<byte>)x).Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain<byte>(false);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain<byte>(false, [xone, xtwo, xthree, xone]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}