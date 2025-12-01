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

    [Cloneable(ReturnType = typeof(ICoreList<string, IElement>))]
    public partial class Chain : CoreList<string, IElement>, IElement
    {
        static string OnGetKey(IElement item) => item is Named named ? named.Name : null!;

        // Custom behaviors for testing purposes...
        public Chain(bool sensitive) : base(OnGetKey)
        {
            Sensitive = sensitive;
            ValidateKey = static x => x.NotNullNotEmpty(true); // Nulls or empties not accepted...
            CompareKeys = (x, y) => new MyComparer(Sensitive).Equals(x, y); // On-stack just for testing...
            ValidateElement = static x => x.ThrowWhenNull(); // Nulls not accepted...
            FlattenElements = true; // Flatten input elements...
            IncludeDuplicate = static (_, _) => throw new DuplicateException(); // No duplicates...
        }

        public Chain(bool sensitive, IEnumerable<IElement> range)
            : this(sensitive)
            => AddRange(range);

        protected Chain(Chain source) : base(source) => Sensitive = source.Sensitive;

        public bool Sensitive { get; }
        readonly struct MyComparer(bool Sensitive) : IEqualityComparer<string?>
        {
            public bool Equals(string? x, string? y) => string.Compare(x, y, !Sensitive) == 0;
            public int GetHashCode(string? obj) => throw new NotImplementedException();
        }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var chain = new Chain(false);
        Assert.Empty(chain);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var chain = new Chain(false, []);
        Assert.Empty(chain);

        chain = new Chain(false, [xone, xtwo, xthree]);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain(true, [xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Equal("ONE", ((Named)chain[1]).Name);

        try { _ = new Chain(false, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(false, [xone, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(false, [xone, xone]); Assert.Fail(); } catch (DuplicateException) { }
        try { _ = new Chain(false, [xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.Clone();

        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        Assert.Same(source.GetKey, target.GetKey);
        Assert.Same(source.ValidateKey, target.ValidateKey);
        Assert.Same(source.CompareKeys, target.CompareKeys);
        Assert.Same(source.ValidateElement, target.ValidateElement);
        Assert.Equal(source.FlattenElements, target.FlattenElements);
        Assert.Same(source.GetDuplicates, target.GetDuplicates);
        Assert.Same(source.IncludeDuplicate, target.IncludeDuplicate);
        Assert.Equal(source.Sensitive, ((Chain)target).Sensitive);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Key()
    {
        var chain = new Chain(false) { IncludeDuplicate = (_, _) => true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        var index = chain.IndexOf("four"); Assert.Equal(-1, index);

        index = chain.IndexOf("one"); Assert.Equal(0, index);
        index = chain.IndexOf("ONE"); Assert.Equal(0, index);

        index = chain.LastIndexOf("one"); Assert.Equal(2, index);
        index = chain.LastIndexOf("ONE"); Assert.Equal(2, index);

        var nums = chain.IndexesOf("one");
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(2, nums[1]);

        nums = chain.IndexesOf("ONE");
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(2, nums[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var chain = new Chain(false) { IncludeDuplicate = (_, _) => true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        var index = chain.IndexOf(x => x is null);
        Assert.Equal(-1, index);

        index = chain.IndexOf(x => x is Named named && named.Name.Contains('e'));
        Assert.Equal(0, index);

        index = chain.LastIndexOf(x => x is Named named && named.Name.Contains('e'));
        Assert.Equal(3, index);

        var nums = chain.IndexesOf(x => x is Named named && named.Name.Contains('e'));
        Assert.Equal(3, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(2, nums[1]);
        Assert.Equal(3, nums[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var chain = new Chain(false) { IncludeDuplicate = (_, _) => true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        Assert.False(chain.Find(x => x is null, out var item));
        Assert.Null(item);

        Assert.True(chain.Find(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.Same(xone, item);

        Assert.True(chain.FindLast(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.Same(xthree, item);

        Assert.True(chain.FindAll(x => x is Named named && named.Name.Contains('e'), out var items));
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Same(xthree, items[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var chain = new Chain(false, [xone, xtwo]);
        var done = chain.Add(xthree);
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        try { chain.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { chain.Add(xone); Assert.Fail(); } catch (DuplicateException) { }
        try { chain.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain(false, [xone, xtwo]) { IncludeDuplicate = (_, _) => true };
        done = chain.Add(xone);
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xone, chain[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Flatten()
    {
        var chain = new Chain(false, [xone, xtwo]);
        var done = chain.Add(new Chain(false));
        Assert.Equal(0, done);

        done = chain.Add(new Chain(false, [xthree, xfour]));
        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);

        try { chain.Add(new Chain(false, [new Named("ONE")])); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Range()
    {
        var chain = new Chain(false, [xone, xtwo]);
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
    public static void Test_Add_Range_Flatten()
    {
        var chain = new Chain(false, [xone, xtwo]);
        var done = chain.AddRange([new Chain(false, [xthree, xfour]), new Named("FIVE")]);
        Assert.Equal(3, done);
        Assert.Equal(5, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);
        Assert.Equal("FIVE", ((Named)chain[4]).Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var chain = new Chain(false, [xone, xtwo]);
        var done = chain.Insert(2, xthree);
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        done = chain.Insert(0, xfour);
        Assert.Equal(1, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xfour, chain[0]);
        Assert.Same(xone, chain[1]);
        Assert.Same(xtwo, chain[2]);
        Assert.Same(xthree, chain[3]);

        try { chain.Insert(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { chain.Insert(0, xone); Assert.Fail(); } catch (DuplicateException) { }
        try { chain.Insert(0, new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain(false, [xone, xtwo]) { IncludeDuplicate = (_, _) => true };
        done = chain.Insert(2, xone);
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xone, chain[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Flatten()
    {
        var chain = new Chain(false, [xone, xtwo]);
        var done = chain.Insert(0, new Chain(false));
        Assert.Equal(0, done);

        done = chain.Insert(2, new Chain(false, [xthree, xfour]));
        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);

        try { chain.Insert(0, new Chain(false, [new Named("ONE")])); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Range()
    {
        var chain = new Chain(false, [xone, xtwo]);
        var done = chain.InsertRange(0, []);
        Assert.Equal(0, done);

        done = chain.InsertRange(0, [xthree, xfour]);
        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Same(xthree, chain[0]);
        Assert.Same(xfour, chain[1]);
        Assert.Same(xone, chain[2]);
        Assert.Same(xtwo, chain[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Range_Flatten()
    {
        var chain = new Chain(false, [xone, xtwo]);
        var done = chain.InsertRange(2, [new Chain(false, [xthree, xfour]), new Named("FIVE")]);
        Assert.Equal(3, done);
        Assert.Equal(5, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);
        Assert.Equal("FIVE", ((Named)chain[4]).Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);

        var done = chain.Replace(1, xtwo, out var item);
        Assert.Equal(0, done);
        Assert.Null(item);

        done = chain.Replace(1, new Named("two"), out item); // Because not same reference
        Assert.Equal(1, done);
        Assert.Same(xtwo, item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Equal("two", ((Named)chain[1]).Name);
        Assert.Same(xthree, chain[2]);

        try { chain.Replace(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { chain.Replace(1, new Named("ONE")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);

        var done = chain.Replace(1, new Chain(false, []), out var item);
        Assert.Equal(0, done);
        Assert.Null(item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Many()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);

        var xalpha = new Named("alpha");
        var xbeta = new Named("beta");

        var done = chain.Replace(1, new Chain(false, [xalpha, xbeta]), out var item);
        Assert.Equal(2, done);
        Assert.Same(xtwo, item);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xalpha, chain[1]);
        Assert.Same(xbeta, chain[2]);
        Assert.Same(xthree, chain[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);

        var done = chain.RemoveAt(0, out var item);
        Assert.True(done);
        Assert.Same(xone, item);
        Assert.Equal(2, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_Empty()
    {
        var chain = new Chain(false, [xone, xtwo, xthree, xfour]);
        var done = chain.RemoveRange(0, 0, out var items);
        Assert.Equal(0, done);
        Assert.Empty(items);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xfour, chain[3]);
    }
    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_Others()
    {
        var chain = new Chain(false, [xone, xtwo, xthree, xfour]);
        var done = chain.RemoveRange(1, 3, out var items);
        Assert.Equal(3, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xfour, items[2]);
        Assert.Single(chain);
        Assert.Same(xone, chain[0]);

        chain = new Chain(false, [xone, xtwo, xthree, xfour]);
        done = chain.RemoveRange(0, 4, out items);
        Assert.Equal(4, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Empty(chain);

        try { items.RemoveRange(0, -1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { items.RemoveRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { items.RemoveRange(5, 0); Assert.Fail(); } catch (ArgumentException) { }
        try { items.RemoveRange(0, 5); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Key()
    {
        var done = 0;
        var chain = new Chain(false) { IncludeDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.Remove("four", out var item);
        Assert.Equal(0, done);
        Assert.Null(item);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xone, chain[3]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.Remove("one", out item);
        Assert.Equal(1, done);
        Assert.Same(xone, item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
        Assert.Same(xone, chain[2]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.Remove("ONE", out item);
        Assert.Equal(1, done);
        Assert.Same(xone, item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
        Assert.Same(xone, chain[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Key()
    {
        var done = 0;
        var chain = new Chain(false) { IncludeDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.RemoveLast("one", out var item);
        Assert.Equal(1, done);
        Assert.Same(xone, item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Key()
    {
        var done = 0;
        var chain = new Chain(false) { IncludeDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.RemoveAll("one", out var items);
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.RemoveAll("ONE", out items);
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var done = 0;
        var chain = new Chain(false) { IncludeDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.Remove(x => x is Named named && named.Name.Contains('e'), out var item);
        Assert.Equal(1, done);
        Assert.Same(xone, item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
        Assert.Same(xone, chain[2]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.RemoveLast(x => x is Named named && named.Name.Contains('e'), out item);
        Assert.Equal(1, done);
        Assert.Same(xone, item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.RemoveAll(x => x is Named named && named.Name.Contains('e'), out var items);
        Assert.Equal(3, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);
        Assert.Single(chain);
        Assert.Same(xtwo, chain[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var chain = new Chain(false);
        var done = chain.Clear();
        Assert.Equal(0, done);

        chain = new Chain(false, [xone, xtwo, xthree]);
        done = chain.Clear();
        Assert.Equal(3, done);
        Assert.Empty(chain);
    }
}