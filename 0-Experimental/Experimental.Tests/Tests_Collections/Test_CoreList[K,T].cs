#pragma warning disable IDE0028

using TKey = string;
using TItem = Experimental.Collections.Tests.Test_CoreList_KT.IElement;

namespace Experimental.Collections.Tests;

// ========================================================
//[Enforced]
public static class Test_CoreList_KT
{
    public interface IElement { }

    public class Named(string name) : TItem
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

    public class Chain : CoreList<TKey, TItem>, TItem
    {
        public Chain() : base() { }      
        public Chain(IEnumerable<TItem> values) : this() => AddRange(values);
        protected Chain(Chain source) : base(source) { }

        public override Chain Clone() => new(this);
        protected override void OnCreating(CoreList<string, TItem> source)
        {
            base.OnCreating(source);
            IgnoreCase = ((Chain)source).IgnoreCase;
        }

        public override TItem ValidateElement(TItem value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return value;
        }

        public override TKey GetKey(TItem value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return value is Named named
                ? named.Name
                : throw new InvalidOperationException("Value is not a named element.");
        }

        public override TKey ValidateKey(TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return key.NotNullNotEmpty(trim: true);
        }

        public override bool CompareKeys(TKey source, TKey target)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);
            return string.Compare(source, target, IgnoreCase) == 0;
        }

        public bool IgnoreCase
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
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var chain = new Chain();
        Assert.Empty(chain);
        Assert.False(chain.FlattenElements);
        Assert.Null(chain.AllowDuplicates);
        Assert.False(chain.IgnoreCase);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var chain = new Chain([]);
        Assert.Empty(chain);

        chain = new([xone, xtwo, xthree]);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new([xone, xone]); // Duplicate ignored...
        Assert.Single(chain);
        Assert.Same(xone, chain[0]);

        chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xone]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);

        chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Equal("ONE", ((Named)chain[1]).Name);

        chain = new Chain() { AllowDuplicates = false, IgnoreCase = true };
        try { chain.AddRange([xone, new Named("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Chain();
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.FlattenElements, target.FlattenElements);
        Assert.Equal(source.AllowDuplicates, target.AllowDuplicates);
        Assert.Equal(source.IgnoreCase, target.IgnoreCase);

        source = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        source.AddRange([xone, xtwo, xthree, xone]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
        Assert.Equal(source.FlattenElements, target.FlattenElements);
        Assert.Equal(source.AllowDuplicates, target.AllowDuplicates);
        Assert.Equal(source.IgnoreCase, target.IgnoreCase);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf()
    {
        var chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xthree, xone]);

        var index = chain.IndexOf("any"); Assert.Equal(-1, index);

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

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xthree, xone]);

        var index = chain.IndexOf(x => x is null); Assert.Equal(-1, index);

        index = chain.IndexOf(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(0, index);

        index = chain.LastIndexOf(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(3, index);

        var nums = chain.IndexesOf(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
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

        chain = new Chain() { AllowDuplicates = null, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xthree]);
        done = chain.Replace(1, xone);
        Assert.Equal(0, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain() { AllowDuplicates = false, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xthree]);
        try { done = chain.Replace(1, new Named("ONE")); Assert.Fail(); }
        catch (DuplicateException) { }
        Assert.Equal(0, done);

        chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xthree]);
        done = chain.Replace(1, xone);
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);
        Assert.Same(xthree, chain[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Nested_Empty()
    {
        var chain = new Chain() { FlattenElements = true };
        chain.AddRange([xone, xtwo, xthree]);

        var done = chain.Replace(1, new Chain());
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xthree, chain[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Nested()
    {
        var xalpha = new Named("alpha");
        var xbeta = new Named("beta");
        var other = new Chain([xalpha, xbeta]);

        var chain = new Chain() { FlattenElements = true };
        chain.AddRange([xone, xtwo, xthree]);

        var done = chain.Replace(1, other);
        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xalpha, chain[1]);
        Assert.Same(xbeta, chain[2]);
        Assert.Same(xthree, chain[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var chain = new Chain();
        var done = chain.Add(xone);
        Assert.Equal(1, done);
        Assert.Single(chain);
        Assert.Same(xone, chain[0]);

        done = chain.Add(xtwo);
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);

        chain = new Chain() { AllowDuplicates = null, IgnoreCase = true };
        chain.AddRange([xone, xtwo]);
        done = chain.Add(new Named("ONE"));
        Assert.Equal(0, done);

        chain = new Chain() { AllowDuplicates = false, IgnoreCase = true };
        chain.AddRange([xone, xtwo]);
        try { chain.Add(new Named("ONE")); Assert.Fail(); }
        catch (DuplicateException) { }

        chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo]);
        done = chain.Add(new Named("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Equal("ONE", ((Named)chain[2]).Name);
    }

    //[Enforced]
    //[Fact]
    //public static void Test_AddNested

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange

    //[Enforced]
    //[Fact]
    //public static void Test_AddRangeNested

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Insert

    //[Enforced]
    //[Fact]
    //public static void Test_InsertNested

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_InsertRange

    //[Enforced]
    //[Fact]
    //public static void Test_InsertRangeNested

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_RemoveAt

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_RemoveRange

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Remove

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Remove_Predicate

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Clear
}