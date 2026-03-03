#pragma warning disable IDE0028

namespace Yotei.ORM.Tests.Collections;

// ========================================================
//[Enforced]
public static partial class Test_CoreList_KT
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

    // ----------------------------------------------------

    [DebuggerDisplay("{ToDebugString(3)}")]
    [Cloneable(ReturnType = typeof(ICoreList<string, IElement>))]
    public partial class Chain : CoreList<string, IElement>, IElement
    {
        public Chain() : base() { }
        public Chain(IEnumerable<IElement> range) : base(range) { }
        protected Chain(Chain source) : this()
        {
            AcceptDuplicates = source.AcceptDuplicates;
            IgnoreCase = source.IgnoreCase;
            AddRange(source);
        }

        public override IElement ValidateElement(IElement value) => value.ThrowWhenNull();
        public override string GetKey(
            IElement value) => value == null || value is not Named named ? null! : named.Name;
        public override string ValidateKey(string key) => key.NotNullNotEmpty(trim: true);
        public override bool CompareKeys(string source, string target) => Comparer.Equals(source, target);
        public override bool FlattenInput(IElement _) => true;
        public override IEnumerable<IElement> FindDuplicates(string key) => base.FindDuplicates(key);
        public override bool IncludeDuplicate(IElement source, IElement target)
            => AcceptDuplicates
            ? true
            : throw new DuplicateException("Duplicates not allowed.").WithData(source).WithData(target);

        public bool AcceptDuplicates
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

        public bool IgnoreCase
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
        chain.AddRange([xone, xone]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);

        chain = new Chain() { IgnoreCase = true };
        try { chain.AddRange([xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain() { IgnoreCase = true, AcceptDuplicates = true };
        chain.AddRange([xone, new Named("ONE")]);
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

        source = new Chain([xone, xtwo, xthree]) { AcceptDuplicates = true, IgnoreCase = true };
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.IsType<Chain>(target);
        Assert.True(((Chain)target).AcceptDuplicates);
        Assert.True(((Chain)target).IgnoreCase);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Setter()
    {
        var chain = new Chain([xone, xtwo, xthree]);
        chain[0] = xfour;
        Assert.Equal(3, chain.Count);
        Assert.Same(xfour, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        try { chain[-1] = xfour; Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { chain[3] = xfour; Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var chain = new Chain([xone, xtwo, xthree]);
        var done = chain.Replace(0, xone);
        Assert.Equal(0, done);

        done = chain.Replace(0, new Named("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Equal("ONE", ((Named)chain[0]).Name);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        var xother = new Named("other");
        try { chain.Replace(-1, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { chain.Replace(3, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Same()
    {
        var chain = new Chain([xone, xtwo, xthree]) { IgnoreCase = true };
        var done = chain.Replace(0, xone);
        Assert.Equal(0, done);

        done = chain.Replace(0, new Named("ONE"));
        Assert.Equal(0, done);

        done = chain.Replace(1, xtwo); Assert.Equal(0, done);
        try { chain.Replace(1, xone); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain() { IgnoreCase = true, AcceptDuplicates = true };
        chain.AddRange([xone, xtwo, xthree]);
        done = chain.Replace(1, xtwo); Assert.Equal(0, done);
        done = chain.Replace(1, new Named("TWO")); Assert.Equal(0, done);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty()
    {
        var chain = new Chain([xone, xtwo, xthree]);
        var done = chain.Replace(0, new Chain());

        Assert.Equal(0, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Range()
    {        
        var xalpha = new Named("alpha");
        var xbeta = new Named("beta");

        var chain = new Chain([xone, xtwo, xthree]);
        var done = chain.Replace(0, new Chain([xalpha, xbeta]));
        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xalpha, chain[0]);
        Assert.Same(xbeta, chain[1]);
        Assert.Same(xtwo, chain[2]);
        Assert.Same(xthree, chain[3]);

        chain = new Chain([xone, xtwo, xthree]);
        done = chain.Replace(1, new Chain([xalpha, xbeta]));
        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xalpha, chain[1]);
        Assert.Same(xbeta, chain[2]);
        Assert.Same(xthree, chain[3]);

        chain = new Chain([xone, xtwo, xthree]);
        done = chain.Replace(2, new Chain([xalpha, xbeta]));
        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xalpha, chain[2]);
        Assert.Same(xbeta, chain[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Key()
    {
        var chain = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xthree, xone]);

        var index = chain.IndexOf("four"); Assert.Equal(-1, index);

        index = chain.IndexOf("one"); Assert.Equal(0, index);
        index = chain.IndexOf("ONE"); Assert.Equal(0, index);

        index = chain.LastIndexOf("one"); Assert.Equal(3, index);
        index = chain.LastIndexOf("ONE"); Assert.Equal(3, index);

        var nums = chain.IndexesOf("one");
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);

        nums = chain.IndexesOf("ONE");
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var chain = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        var index = chain.IndexOf(x => x is null); Assert.Equal(-1, index);

        index = chain.IndexOf(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(0, index);

        index = chain.LastIndexOf(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(2, index);

        var nums = chain.IndexesOf(x => x is Named named && named.Name.Contains('e'));
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
        var chain = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        Assert.False(chain.Find(x => x is null, out item));
        Assert.Null(item);

        Assert.True(chain.Find(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xone, item);

        Assert.True(chain.FindLast(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xthree, item);

        Assert.True(chain.FindAll(x => x is Named named && named.Name.Contains('e'), out range));
        Assert.Equal(3, range.Count);
        Assert.Same(xone, range[0]);
        Assert.Same(xone, range[1]);
        Assert.Same(xthree, range[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Key() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate() => throw null;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear() => throw null;
}