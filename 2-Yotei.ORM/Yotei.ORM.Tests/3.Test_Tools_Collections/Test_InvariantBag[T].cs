#pragma warning disable IDE0018

namespace Yotei.ORM.Tests.Collections;

// ========================================================
//[Enforced]
public static partial class Test_InvariantBag_T
{
    public interface IElement { }

    public class Named(string name) : IElement
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? "-";
    }

    readonly static Named xone = new("one");
    readonly static Named xtwo = new("two");
    readonly static Named xthree = new("three");
    readonly static Named xfour = new("four");
    readonly static Named xfive = new("five");

    // ----------------------------------------------------

    [DebuggerDisplay("{ToDebugString(4)}")]
    [Cloneable]
    public partial class Builder : CoreBag<IElement>, IElement
    {
        public Builder() : base() { }
        public Builder(IEnumerable<IElement> range) : base(range) { }
        protected Builder(Builder source) : this()
        {
            AcceptDuplicates = source.AcceptDuplicates;
            IgnoreCase = source.IgnoreCase;
            Flatten = source.Flatten;
            AddRange(source);
        }

        public override IElement ValidateElement(IElement value) => value.ThrowWhenNull();
        public override bool CompareElements(IElement source, IElement target) => Comparer.Equals(source, target);
        public override bool FlattenInput(IElement _) => Flatten;
        public override IEnumerable<IElement> FindDuplicates(IElement value) => base.FindDuplicates(value);
        public override bool IncludeDuplicated(IElement source, IElement target)
            => AcceptDuplicates
            ? true
            : throw new DuplicateException("Duplicates not allowed.").WithData(source).WithData(target);

        public bool Flatten // Used to test flatten or not scenarios...
        {
            get;
            set
            {
                if (field == value) return;
                if (Count == 0) { field = value; return; }

                var range = ToList(); Clear();
                field = value;
                AddRange(range);
            }
        }
        = false;

        public bool AcceptDuplicates // Used to test duplicates accepted or not scenarios...
        {
            get;
            set
            {
                if (field == value) return;
                if (Count == 0) { field = value; return; }

                var range = ToList(); Clear();
                field = value;
                AddRange(range);
            }
        }
        = false;

        public bool IgnoreCase // Used to test key equality scenarios...
        {
            get => Comparer.IgnoreCase;
            set
            {
                if (value == Comparer.IgnoreCase) return;
                if (Count == 0) { Comparer = new(value); return; }

                var range = ToList(); Clear();
                Comparer = new(value);
                AddRange(range);
            }
        }

        MyComparer Comparer = new(false);
        readonly struct MyComparer(bool ignoreCase) : IEqualityComparer<IElement>
        {
            readonly public bool IgnoreCase = ignoreCase;
            public bool Equals(IElement? x, IElement? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return x is Named xnamed && y is Named ynamed
                    ? string.Compare(xnamed.Name, ynamed.Name, IgnoreCase) == 0
                    : ReferenceEquals(x, y);
            }
            public int GetHashCode(IElement? obj) => throw new NotImplementedException();
        }
    }

    // ----------------------------------------------------

    [DebuggerDisplay("{ToDebugString(4)}")]
    [Cloneable(ReturnType = typeof(IInvariantBag<IElement>))]
    public partial class Chain : InvariantBag<IElement>, IElement
    {
        protected override Builder Items { get; } = [];

        public Chain() { }
        public Chain(IEnumerable<IElement> range) : this() => Items.AddRange(range);
        protected Chain(Chain source) => Items = source.ThrowWhenNull().Items.Clone();

        public bool Flatten // Using 'set' instead of 'init' only for testing purposes...
        {
            get => Items.Flatten;
            set => Items.Flatten = value;
        }

        public bool AcceptDuplicates // Using 'set' instead of 'init' only for testing purposes...
        {
            get => Items.AcceptDuplicates;
            set => Items.AcceptDuplicates = value;
        }

        public bool IgnoreCase // Using 'set' instead of 'init' only for testing purposes...
        {
            get => Items.IgnoreCase;
            set => Items.IgnoreCase = value;
        }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var chain = new Chain();
        Assert.Empty(chain);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var chain = new Chain([]);
        Assert.Empty(chain);

        chain = new Chain([xone, xtwo, xthree]);
        Assert.Equal(3, chain.Count);
        Assert.Contains(xone, chain);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xthree, chain);

        chain = new Chain([xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.True(chain.Contains(x => x is Named named && named.Name == "ONE"));

        try { _ = new Chain(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain([xone, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain([xone, xone]); Assert.Fail(); } catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_With_Duplicates()
    {
        var chain = new Chain() { AcceptDuplicates = true };
        chain = (Chain)chain.AddRange([xone, xone]);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.Contains(xone, chain);

        chain = new Chain() { IgnoreCase = true };
        try { chain.AddRange([xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain() { IgnoreCase = true, AcceptDuplicates = true };
        chain = (Chain)chain.AddRange([xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.True(chain.Contains(x => x is Named named && named.Name == "ONE"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Chain();
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Empty(target);

        source = new Chain([xone, xtwo]) { AcceptDuplicates = true, IgnoreCase = true, Flatten = true };
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.IsType<Chain>(target);
        Assert.True(((Chain)target).AcceptDuplicates);
        Assert.True(((Chain)target).IgnoreCase);
        Assert.True(((Chain)target).Flatten);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        IElement item;
        List<IElement> range;
        var chain = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        chain = (Chain)chain.AddRange([xone, xtwo, xone, xthree]);

        Assert.False(chain.Find(x => x is null, out item));
        Assert.Null(item);

        Assert.True(chain.Find(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Contains(item, chain);

        Assert.True(chain.FindAll(x => x is Named named && named.Name.Contains('e'), out range));
        Assert.Equal(3, range.Count);
        Assert.Contains(xone, range);
        Assert.Contains(xthree, range);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Chain();
        var target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Contains(xone, target);

        source = (Chain)target;
        target = source.Add(xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(xone); Assert.Fail(); } catch (DuplicateException) { }

        source = new Chain([xone]) { IgnoreCase = true };
        try { source.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source.AcceptDuplicates = true;
        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xone, target);

        source = (Chain)target;
        target = source.Add(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xone, target);
        Assert.True(target.Contains(x => x is Named named && named.Name == "ONE"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested_With_Flatten()
    {
        var source = new Chain([xone, xtwo]) { Flatten = true };
        var target = source.Add(new Chain([xthree, xfour]));

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
        Assert.Contains(xfour, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested_No_Flatten()
    {
        Chain other;
        var source = new Chain([xone, xtwo]);
        var target = source.Add(new Chain([xthree, xfour]));

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        var list = target.ToList();
        Assert.Same(xone, list[0]);
        Assert.Same(xtwo, list[1]);

        other = Assert.IsType<Chain>(list[2]);
        Assert.Equal(2, other.Count);
        Assert.Contains(xthree, other);
        Assert.Contains(xfour, other);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
        Assert.Contains(xfour, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Nested_With_Flatten()
    {
        var source = new Chain([xone, xtwo]) { Flatten = true };
        var target = source.AddRange([xthree, new Chain([xfour, xfive])]);

        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
        Assert.Contains(xfour, target);
        Assert.Contains(xfive, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Nested_No_Flatten()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.AddRange([xthree, new Chain([xfour, xfive])]);

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);

        var list = target.ToList();
        Assert.Same(xone, list[0]);
        Assert.Same(xtwo, list[1]);
        Assert.Same(xthree, list[2]);

        var other = Assert.IsType<Chain>(list[3]);
        Assert.Equal(2, other.Count);
        Assert.Contains(xfour, other);
        Assert.Contains(xfive, other);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new Chain() { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(xfour);
        Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        source = new Chain() { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);
        target = source.RemoveAll(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_IgnoreCase()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.RemoveAll(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Nested_No_Flatten()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(new Chain([xone, xtwo]));
        Assert.Same(source, target);
        Assert.Equal(4, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Nested_With_Flatten()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true, Flatten = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(new Chain([xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        source = new Chain() { AcceptDuplicates = true, IgnoreCase = true, Flatten = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        target = source.RemoveAll(new Chain([xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Contains(xthree, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(x => x is Named named && named.Name is null);
        Assert.Same(source, target);

        target = source.Remove(x => x is Named named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        target = source.RemoveAll(x => x is Named named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain();
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain([xone, xtwo, xthree]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}