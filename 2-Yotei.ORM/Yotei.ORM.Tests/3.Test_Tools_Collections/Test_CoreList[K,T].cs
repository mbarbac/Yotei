#pragma warning disable CS0436, IDE0028

namespace Yotei.ORM.Tests.Tools.Collections;

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
    readonly static Named xfive = new("five");

    // ----------------------------------------------------

    [DebuggerDisplay("{ToDebugString(3)}")]
    [Cloneable(ReturnType = typeof(ICoreList<string, IElement>))]
    public partial class Chain : CoreList<string, IElement>, IElement
    {
        public Chain() : base() => Comparer = new(this);
        public Chain(IEnumerable<IElement> range) : this() => AddRange(range);
        protected Chain(Chain source) : this()
        {
            AllowDuplicates = source.AllowDuplicates;
            IgnoreCase = source.IgnoreCase;
            AddRange(source);
        }

        public override IElement ValidateElement(IElement value) => value.ThrowWhenNull();
        public override string GetKey(IElement value) => value is Named named ? named.Name : null!;
        public override string ValidateKey(string key) => key.NotNullNotEmpty(trim: true);
        public override bool CompareKeys(
            string source, string target) => Comparer.Equals(source, target);
        public override IEnumerable<IElement> FindDuplicates(string key) => base.FindDuplicates(key);
        public override bool AcceptDuplicated(
            IElement source, IElement other) => AllowDuplicates ? true : throw new DuplicateException().WithData(other);

        public bool AllowDuplicates
        {
            get;
            set
            {
                if (field == value) return;
                if (Count == 0) { field = value; return; }

                var range = ToList(); Clear();
                field = value; AddRange(range);
            }
        }
        = false;

        public bool IgnoreCase
        {
            get;
            set
            {
                if (field == value) return;
                if (Count == 0) { field = value; return; }

                var range = ToList(); Clear();
                field = value; AddRange(range);
            }
        }
        = false;

        readonly MyComparer Comparer;
        readonly struct MyComparer(Chain master) : IEqualityComparer<string>
        {
            public bool Equals(string? x, string? y) => string.Compare(x, y, master.IgnoreCase) == 0;
            public int GetHashCode(string? _) => throw new NotImplementedException();
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
        var chain = new Chain() { AllowDuplicates = true };
        chain.AddRange([xone, xone]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);

        chain = new Chain() { IgnoreCase = true };
        try { chain.AddRange([xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain() { IgnoreCase = true, AllowDuplicates = true };
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

        source = new Chain([xone, xtwo]) { AllowDuplicates = true, IgnoreCase = true };
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.IsType<Chain>(target);
        Assert.True(((Chain)target).AllowDuplicates);
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

    // ----------------------------------------------------

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

        chain = new Chain() { IgnoreCase = true, AllowDuplicates = true };
        chain.AddRange([xone, xtwo, xthree]);
        done = chain.Replace(1, xtwo); Assert.Equal(0, done);
        done = chain.Replace(1, new Named("TWO")); Assert.Equal(0, done);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty_Nested()
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
    public static void Test_Replace_Range_Nested()
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
        var chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
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
        var chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
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
        var chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
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

        try { chain.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { chain.Add(xone); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain([xone]) { IgnoreCase = true };
        try { chain.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        chain.AllowDuplicates = true;
        done = chain.Add(xone);
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);

        done = chain.Add(new Named("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);
        Assert.Equal("ONE", ((Named)chain[2]).Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested()
    {
        var chain = new Chain([xone, xtwo]);
        var done = chain.Add(new Chain([xthree, xfour]));

        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var chain = new Chain([xone, xtwo]);
        var done = chain.AddRange([]);
        Assert.Equal(0, done);

        done = chain.AddRange([xthree, xfour]);
        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Nested()
    {
        var chain = new Chain([xone, xtwo]);
        var done = chain.AddRange([xthree, new Chain([xfour, xfive])]);

        Assert.Equal(3, done);
        Assert.Equal(5, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);
        Assert.Same(xfive, chain[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var chain = new Chain();
        var done = chain.Insert(0, xone);
        Assert.Equal(1, done);
        Assert.Single(chain);
        Assert.Same(xone, chain[0]);

        done = chain.Insert(1, xtwo);
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);

        done = chain.Insert(0, xthree);
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xthree, chain[0]);
        Assert.Same(xone, chain[1]);
        Assert.Same(xtwo, chain[2]);

        try { chain.Insert(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { chain.Insert(0, xone); Assert.Fail(); } catch (DuplicateException) { }
        try { chain.Insert(-1, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { chain.Insert(4, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }

        chain.IgnoreCase = true;
        try { chain.Insert(0, new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        chain.AllowDuplicates = true;
        done = chain.Insert(3, xone);
        Assert.Equal(1, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xthree, chain[0]);
        Assert.Same(xone, chain[1]);
        Assert.Same(xtwo, chain[2]);
        Assert.Same(xone, chain[3]);

        done = chain.Insert(0, new Named("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(5, chain.Count);
        Assert.Equal("ONE", ((Named)chain[0]).Name);
        Assert.Same(xthree, chain[1]);
        Assert.Same(xone, chain[2]);
        Assert.Same(xtwo, chain[3]);
        Assert.Same(xone, chain[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Nested()
    {
        var chain = new Chain([xone, xtwo]);
        var done = chain.Insert(2, new Chain([xthree, xfour]));

        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var chain = new Chain([xone, xtwo]);
        var done = chain.InsertRange(0, []);
        Assert.Equal(0, done);

        done = chain.InsertRange(2, [xthree, xfour]);
        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Nested()
    {
        var chain = new Chain([xone, xtwo]);
        var done = chain.InsertRange(1, [xthree, new Chain([xfour, xfive])]);

        Assert.Equal(3, done);
        Assert.Equal(5, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xthree, chain[1]);
        Assert.Same(xfour, chain[2]);
        Assert.Same(xfive, chain[3]);
        Assert.Same(xtwo, chain[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var chain = new Chain([xone, xtwo, xthree]);
        var done = chain.RemoveAt(0);

        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);

        try { chain.RemoveAt(-1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { chain.RemoveAt(2); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_Empty()
    {
        var chain = new Chain([xone, xtwo, xthree]);
        var done = chain.RemoveRange(0, 0);

        Assert.Equal(0, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var chain = new Chain([xone, xtwo, xthree]);
        var done = chain.RemoveRange(0, 1);
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);

        chain = new Chain([xone, xtwo, xthree]);
        done = chain.RemoveRange(1, 2);
        Assert.Equal(2, done);
        Assert.Single(chain);
        Assert.Same(xone, chain[0]);

        chain = new Chain([xone, xtwo, xthree]);
        try { chain.RemoveRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { chain.RemoveRange(3, 1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { chain.RemoveRange(0, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { chain.RemoveRange(1, 3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { chain.RemoveRange(2, 2); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Key()
    {
        var chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        var done = chain.Remove("any");
        Assert.Equal(0, done);

        done = chain.Remove("ONE");
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xone, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);
        done = chain.RemoveLast("ONE");
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);
        done = chain.RemoveAll("ONE");
        Assert.Equal(2, done);
        Assert.Equal(2, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        var done = chain.Remove(x => x is Named named && named.Name is null);
        Assert.Equal(0, done);

        done = chain.Remove(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xone, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);
        done = chain.RemoveLast(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain() { AllowDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);
        done = chain.RemoveAll(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(2, done);
        Assert.Equal(2, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var chain = new Chain();
        var done = chain.Clear();
        Assert.Equal(0, done);

        chain = new Chain([xone, xtwo, xthree]);
        done = chain.Clear();
        Assert.Equal(3, done);
        Assert.Empty(chain);
    }
}