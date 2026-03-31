namespace Yotei.ORM.Tests.Collections;

// ========================================================
//[Enforced]
public static partial class Test_InvariantList_T
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
    public partial class Builder : CoreList<IElement>, IElement
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
    [Cloneable(ReturnType = typeof(IInvariantList<IElement>))]
    public partial class Chain : InvariantList<IElement>, IElement
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
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain([xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Equal("ONE", ((Named)chain[1]).Name);

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
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);

        chain = new Chain() { IgnoreCase = true };
        try { chain.AddRange([xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain() { IgnoreCase = true, AcceptDuplicates = true };
        chain = (Chain)chain.AddRange([xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Equal("ONE", ((Named)chain[1]).Name);
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

        source = new Chain() { AcceptDuplicates = true, IgnoreCase = true, Flatten = true };
        source = (Chain)source.AddRange([xone, xtwo]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.IsType<Chain>(target);
        Assert.True(((Chain)target).AcceptDuplicates);
        Assert.True(((Chain)target).IgnoreCase);
        Assert.True(((Chain)target).Flatten);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(0, new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("ONE", ((Named)target[0]).Name);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        var xother = new Named("other");
        try { source.Replace(-1, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Replace(3, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Same()
    {
        var source = new Chain([xone, xtwo, xthree]) { IgnoreCase = true };
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        try { source.Replace(1, xone); Assert.Fail(); } catch (DuplicateException) { }

        source = new Chain() { IgnoreCase = true, AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xthree]);
        target = source.Replace(1, xtwo);
        Assert.Same(source, target);

        target = source.Replace(1, new Named("TWO"));
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty_With_Flatten()
    {
        var source = new Chain([xone, xtwo, xthree]) { Flatten = true };
        var target = source.Replace(0, new Chain());
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty_No_Flatten()
    {
        var source = new Chain([xone, xtwo, xthree]) { Flatten = false };
        var target = source.Replace(0, new Chain());
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);

        var other = Assert.IsType<Chain>(target[0]);
        Assert.Empty(other);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Range_With_Flatten()
    {
        var xalpha = new Named("alpha");
        var xbeta = new Named("beta");
        var source = new Chain([xone, xtwo, xthree]) { Flatten = true };

        var target = source.Replace(0, new Chain([xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xalpha, target[0]);
        Assert.Same(xbeta, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(1, new Chain([xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xalpha, target[1]);
        Assert.Same(xbeta, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(2, new Chain([xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xalpha, target[2]);
        Assert.Same(xbeta, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Range_No_Flatten()
    {
        var xalpha = new Named("alpha");
        var xbeta = new Named("beta");
        var source = new Chain([xone, xtwo, xthree]);

        var target = source.Replace(1, new Chain([xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[2]);

        var other = Assert.IsType<Chain>(target[1]);
        Assert.Equal(2, other.Count);
        Assert.Same(xalpha, other[0]);
        Assert.Same(xbeta, other[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Item()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xthree, xone]);

        var index = source.IndexOf(xfour); Assert.Equal(-1, index);

        index = source.IndexOf(xone); Assert.Equal(0, index);
        index = source.IndexOf(new Named("ONE")); Assert.Equal(0, index);

        index = source.LastIndexOf(xone); Assert.Equal(3, index);
        index = source.LastIndexOf(new Named("ONE")); Assert.Equal(3, index);

        var nums = source.IndexesOf(xone);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);

        nums = source.IndexesOf(new Named("ONE"));
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var index = source.IndexOf(x => x is null); Assert.Equal(-1, index);

        index = source.IndexOf(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(0, index);

        index = source.LastIndexOf(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(2, index);

        var nums = source.IndexesOf(x => x is Named named && named.Name.Contains('e'));
        Assert.Equal(3, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(2, nums[1]);
        Assert.Equal(3, nums[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        IElement item;
        List<IElement> range;
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        Assert.False(source.Find(x => x is null, out item));
        Assert.Null(item);

        Assert.True(source.Find(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xone, item);

        Assert.True(source.FindLast(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xthree, item);

        Assert.True(source.FindAll(x => x is Named named && named.Name.Contains('e'), out range));
        Assert.Equal(3, range.Count);
        Assert.Same(xone, range[0]);
        Assert.Same(xone, range[1]);
        Assert.Same(xthree, range[2]);
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
        Assert.Same(xone, target[0]);

        source = new Chain([xone]);
        target = source.Add(xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(xone); Assert.Fail(); } catch (DuplicateException) { }

        source.IgnoreCase = true;
        try { source.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source.AcceptDuplicates = true;
        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xone, target[1]);

        target = source.Add(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal("ONE", ((Named)target[1]).Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested_With_Flatten()
    {
        var source = new Chain([xone, xtwo]) { Flatten = true };
        var target = source.Add(new Chain([xthree, xfour]));

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
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
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        other = Assert.IsType<Chain>(target[2]);
        Assert.Equal(2, other.Count);
        Assert.Same(xthree, other[0]);
        Assert.Same(xfour, other[1]);
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
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Nested_With_Flatten()
    {
        var source = new Chain([xone, xtwo]) { Flatten = true };
        var target = source.AddRange([xthree, new Chain([xfour, xfive])]);

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
    public static void Test_AddRange_Nested_No_Flatten()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.AddRange([xthree, new Chain([xfour, xfive])]);

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        var other = Assert.IsType<Chain>(target[3]);
        Assert.Equal(2, other.Count);
        Assert.Same(xfour, other[0]);
        Assert.Same(xfive, other[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Chain();
        var target = source.Insert(0, xone);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        source = (Chain)target;
        target = source.Insert(1, xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        source = (Chain)target;
        target = source.Insert(0, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xthree, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);

        try { source.Insert(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Insert(0, xone); Assert.Fail(); } catch (DuplicateException) { }
        try { source.Insert(-1, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Insert(4, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }

        source.IgnoreCase = true;
        try { source.Insert(0, new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source = (Chain)target;
        source.AcceptDuplicates = true;
        target = source.Insert(3, xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xthree, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xone, target[3]);

        source = (Chain)target;
        target = source.Insert(0, new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("ONE", ((Named)target[0]).Name);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);
        Assert.Same(xtwo, target[3]);
        Assert.Same(xone, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Nested_With_Flatten()
    {
        var source = new Chain([xone, xtwo]) { Flatten = true };
        var target = source.Insert(2, new Chain([xthree, xfour]));

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Nested_No_Flatten()
    {
        Chain other;
        var source = new Chain([xone, xtwo]);
        var target = source.Insert(2, new Chain([xthree, xfour]));

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        other = Assert.IsType<Chain>(target[2]);
        Assert.Equal(2, other.Count);
        Assert.Same(xthree, other[0]);
        Assert.Same(xfour, other[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.InsertRange(0, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Nested_With_Flatten()
    {
        var source = new Chain([xone, xtwo]) { Flatten = true };
        var target = source.InsertRange(1, [xthree, new Chain([xfour, xfive])]);

        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xfour, target[2]);
        Assert.Same(xfive, target[3]);
        Assert.Same(xtwo, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Nested_No_Flatten()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.InsertRange(1, [xthree, new Chain([xfour, xfive])]);

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        var other = Assert.IsType<Chain>(target[2]);
        Assert.Equal(2, other.Count);
        Assert.Same(xfour, other[0]);
        Assert.Same(xfive, other[1]);
        Assert.Same(xtwo, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.RemoveAt(0);

        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        try { source.RemoveAt(-1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveAt(3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_Empty()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        try { source.RemoveRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(3, 1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(0, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(1, 3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(2, 2); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(xfour);
        Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
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
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Nested_No_Flatten()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(new Chain([xone, xtwo]));
        Assert.Same(source, target);
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
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAll(new Chain([xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xthree, target[0]);
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
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(x => x is Named named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => x is Named named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
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