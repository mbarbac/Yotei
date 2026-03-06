namespace Yotei.ORM.Tests.Collections;

// ========================================================
//[Enforced]
public static partial class Test_InvariantList_KT
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

    [DebuggerDisplay("{ToDebugString(3)}")]
    [Cloneable]
    public partial class Builder : CoreList<string, IElement>, IElement
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
        public override string GetKey(
            IElement value) => value == null || value is not Named named ? null! : named.Name;
        public override string ValidateKey(string key) => key.NotNullNotEmpty(trim: true);
        public override bool CompareKeys(string source, string target) => Comparer.Equals(source, target);
        public override bool FlattenInput(IElement _) => Flatten;
        public override IEnumerable<IElement> FindDuplicates(string key) => base.FindDuplicates(key);
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
        readonly struct MyComparer(bool ignoreCase) : IEqualityComparer<string>
        {
            readonly public bool IgnoreCase = ignoreCase;
            public bool Equals(string? x, string? y) => string.Compare(x, y, IgnoreCase) == 0;
            public int GetHashCode(string? obj) => throw new NotImplementedException();
        }
    }

    // ----------------------------------------------------

    [DebuggerDisplay("{ToDebugString(4)}")]
    [Cloneable(ReturnType = typeof(IInvariantList<string, IElement>))]
    public partial class Chain : InvariantList<string, IElement>, IElement
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
    public static void Test_IndexOf_Key()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xthree, xone]);

        var index = source.IndexOf("four"); Assert.Equal(-1, index);

        index = source.IndexOf("one"); Assert.Equal(0, index);
        index = source.IndexOf("ONE"); Assert.Equal(0, index);

        index = source.LastIndexOf("one"); Assert.Equal(3, index);
        index = source.LastIndexOf("ONE"); Assert.Equal(3, index);

        var nums = source.IndexesOf("one");
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);

        nums = source.IndexesOf("ONE");
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
        try { source.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source.AcceptDuplicates = true;
        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xone, target[1]);

        target = source.Add(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Equal("ONE", ((Named)target[2]).Name);
    }

    /*

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested_With_Flatten()
    {
        var source = new Chain([xone, xtwo]) { Flatten = true };
        var done = source.Add(new Chain([xthree, xfour]));

        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested_No_Flatten()
    {
        Chain other;
        var source = new Chain([xone, xtwo]);
        var done = source.Add(new Chain([xthree, xfour]));

        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        other = Assert.IsType<Chain>(source[2]);
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
        var done = source.AddRange([]);
        Assert.Equal(0, done);

        done = source.AddRange([xthree, xfour]);
        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Nested_With_Flatten()
    {
        var source = new Chain([xone, xtwo]) { Flatten = true };
        var done = source.AddRange([xthree, new Chain([xfour, xfive])]);

        Assert.Equal(3, done);
        Assert.Equal(5, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);
        Assert.Same(xfive, source[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Nested_No_Flatten()
    {
        var source = new Chain([xone, xtwo]);
        var done = source.AddRange([xthree, new Chain([xfour, xfive])]);

        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        var other = Assert.IsType<Chain>(source[3]);
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
        var done = source.Insert(0, xone);
        Assert.Equal(1, done);
        Assert.Single(source);
        Assert.Same(xone, source[0]);

        done = source.Insert(1, xtwo);
        Assert.Equal(1, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);

        done = source.Insert(0, xthree);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xthree, source[0]);
        Assert.Same(xone, source[1]);
        Assert.Same(xtwo, source[2]);

        try { source.Insert(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Insert(0, xone); Assert.Fail(); } catch (DuplicateException) { }
        try { source.Insert(-1, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Insert(4, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }

        source.IgnoreCase = true;
        try { source.Insert(0, new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source.AcceptDuplicates = true;
        done = source.Insert(3, xone);
        Assert.Equal(1, done);
        Assert.Equal(4, source.Count);
        Assert.Same(xthree, source[0]);
        Assert.Same(xone, source[1]);
        Assert.Same(xtwo, source[2]);
        Assert.Same(xone, source[3]);

        done = source.Insert(0, new Named("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(5, source.Count);
        Assert.Equal("ONE", ((Named)source[0]).Name);
        Assert.Same(xthree, source[1]);
        Assert.Same(xone, source[2]);
        Assert.Same(xtwo, source[3]);
        Assert.Same(xone, source[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Nested_With_Flatten()
    {
        var source = new Chain([xone, xtwo]) { Flatten = true };
        var done = source.Insert(2, new Chain([xthree, xfour]));

        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Nested_No_Flatten()
    {
        Chain other;
        var source = new Chain([xone, xtwo]);
        var done = source.Insert(2, new Chain([xthree, xfour]));

        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        other = Assert.IsType<Chain>(source[2]);
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
        var done = source.InsertRange(0, []);
        Assert.Equal(0, done);

        done = source.InsertRange(2, [xthree, xfour]);
        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
        Assert.Same(xfour, source[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Nested_With_Flatten()
    {
        var source = new Chain([xone, xtwo]) { Flatten = true };
        var done = source.InsertRange(1, [xthree, new Chain([xfour, xfive])]);

        Assert.Equal(3, done);
        Assert.Equal(5, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xthree, source[1]);
        Assert.Same(xfour, source[2]);
        Assert.Same(xfive, source[3]);
        Assert.Same(xtwo, source[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Nested_No_Flatten()
    {
        var source = new Chain([xone, xtwo]);
        var done = source.InsertRange(1, [xthree, new Chain([xfour, xfive])]);

        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xthree, source[1]);
        var other = Assert.IsType<Chain>(source[2]);
        Assert.Equal(2, other.Count);
        Assert.Same(xfour, other[0]);
        Assert.Same(xfive, other[1]);
        Assert.Same(xtwo, source[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var done = source.RemoveAt(0);

        Assert.Equal(1, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);

        try { source.RemoveAt(-1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveAt(2); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_Empty()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var done = source.RemoveRange(0, 0);

        Assert.Equal(0, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var done = source.RemoveRange(0, 1);
        Assert.Equal(1, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);

        source = new Chain([xone, xtwo, xthree]);
        done = source.RemoveRange(1, 2);
        Assert.Equal(2, done);
        Assert.Single(source);
        Assert.Same(xone, source[0]);

        source = new Chain([xone, xtwo, xthree]);
        try { source.RemoveRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(3, 1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(0, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(1, 3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(2, 2); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Key()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source.AddRange([xone, xtwo, xone, xthree]);

        var done = source.Remove("any");
        Assert.Equal(0, done);

        done = source.Remove("ONE");
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xone, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source.AddRange([xone, xtwo, xone, xthree]);
        done = source.RemoveLast("ONE");
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source.AddRange([xone, xtwo, xone, xthree]);
        done = source.RemoveAll("ONE");
        Assert.Equal(2, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source.AddRange([xone, xtwo, xone, xthree]);

        var done = source.Remove(x => x is Named named && named.Name is null);
        Assert.Equal(0, done);

        done = source.Remove(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xone, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source.AddRange([xone, xtwo, xone, xthree]);
        done = source.RemoveLast(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.Same(xone, source[0]);
        Assert.Same(xtwo, source[1]);
        Assert.Same(xthree, source[2]);

        source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source.AddRange([xone, xtwo, xone, xthree]);
        done = source.RemoveAll(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(2, done);
        Assert.Equal(2, source.Count);
        Assert.Same(xtwo, source[0]);
        Assert.Same(xthree, source[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain();
        var done = source.Clear();
        Assert.Equal(0, done);

        source = new Chain([xone, xtwo, xthree]);
        done = source.Clear();
        Assert.Equal(3, done);
        Assert.Empty(source);
    }
 */
}