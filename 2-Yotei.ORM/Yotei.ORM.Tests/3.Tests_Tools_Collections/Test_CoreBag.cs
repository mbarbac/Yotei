#pragma warning disable xUnit2017

namespace Yotei.ORM.Tests.Collections;

// ========================================================
//[Enforced]
public static partial class Test_CoreBag
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

    // Custom behaviors for testing purposes
    [Cloneable(ReturnType = typeof(ICoreBag<IElement>))]
    public partial class Chain : CoreBag<IElement>, IElement
    {
        public Chain(bool sensitive) : base()
        {
            Sensitive = sensitive;
            Validate = static x => x.ThrowWhenNull(); // Nulls not accepted...
            FlattenElements = true; // Flatten collection elements...
            AreEqual = (x, y) => new MyComparer(Sensitive).Equals(x, y); // New on stack for testing purposes only...
            IsValidDuplicate = (x, y) => throw new DuplicateException(); // No duplicates...
        }

        public Chain(bool sensitive, IEnumerable<IElement> range)
            : this(sensitive)
            => AddRange(range);

        protected Chain(Chain source) : base(source) => Sensitive = source.Sensitive;

        public bool Sensitive { get; }
        readonly struct MyComparer(bool Sensitive) : IEqualityComparer<IElement>
        {
            public bool Equals(IElement? x, IElement? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;

                return x is Named xnamed && y is Named ynamed
                    ? string.Compare(xnamed.Name, ynamed.Name, !Sensitive) == 0
                    : ReferenceEquals(x, y);
            }
            public int GetHashCode(IElement obj) => throw new NotImplementedException();
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
        Assert.True(chain.Contains(xone));
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));

        chain = new Chain(true, [xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.True(chain.Contains(xone));
        Assert.True(chain.Find(x => x is Named named && named.Name == "ONE"));

        try { _ = new Chain(false, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(false, [xone, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(false, [xone, xone]); Assert.Fail(); } catch (DuplicateException) { }
        try { _ = new Chain(false, [xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }
    }

    /*

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

        Assert.Same(source.Validate, ((Chain)target).Validate);
        Assert.Equal(source.FlattenElements, ((Chain)target).FlattenElements);
        Assert.Same(source.AreEqual, ((Chain)target).AreEqual);
        Assert.Same(source.IsValidDuplicate, ((Chain)target).IsValidDuplicate);
        Assert.Equal(source.Sensitive, ((Chain)target).Sensitive);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Item()
    {
        var chain = new Chain(false) { IsValidDuplicate = (_, _) => true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        var index = chain.IndexOf(xfour); Assert.Equal(-1, index);

        index = chain.IndexOf(xone); Assert.Equal(0, index);
        index = chain.IndexOf(new Named("ONE")); Assert.Equal(0, index);

        index = chain.LastIndexOf(xone); Assert.Equal(2, index);
        index = chain.LastIndexOf(new Named("ONE")); Assert.Equal(2, index);

        var nums = chain.AllIndexesOf(xone);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(2, nums[1]);

        nums = chain.AllIndexesOf(new Named("ONE"));
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(2, nums[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var chain = new Chain(false) { IsValidDuplicate = (_, _) => true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        var index = chain.IndexOf(x => x is null);
        Assert.Equal(-1, index);

        index = chain.IndexOf(x => x is Named named && named.Name.Contains('e'));
        Assert.Equal(0, index);

        index = chain.LastIndexOf(x => x is Named named && named.Name.Contains('e'));
        Assert.Equal(3, index);

        var nums = chain.AllIndexesOf(x => x is Named named && named.Name.Contains('e'));
        Assert.Equal(3, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(2, nums[1]);
        Assert.Equal(3, nums[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find_Values()
    {
        var chain = new CoreList<int>([1, 2, 3, 4]);

        Assert.False(chain.Find(x => x > 10, out var value));

        Assert.True(chain.Find(x => x is > 1 and < 4, out value)); Assert.Equal(2, value);
        Assert.True(chain.FindLast(x => x is > 1 and < 4, out value)); Assert.Equal(3, value);
        Assert.True(chain.FindAll(x => x is > 1 and < 4, out var values));
        Assert.Equal(2, values.Count);
        Assert.Equal(2, values[0]);
        Assert.Equal(3, values[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var chain = new Chain(false) { IsValidDuplicate = (_, _) => true };
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

        chain = new Chain(false, [xone, xtwo]) { IsValidDuplicate = (_, _) => true };
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

        chain = new Chain(false, [xone, xtwo]) { IsValidDuplicate = (_, _) => true };
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
    public static void Test_Replace_Same()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);

        var done = chain.Replace(1, xtwo);
        Assert.Equal(0, done);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        IElement? item = null;
        done = chain.Replace(1, new Named("TWO"), x => item = x);
        Assert.Equal(0, done);
        Assert.Null(item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Standard()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);

        IElement? item = null;
        var done = chain.Replace(1, xfour, x => item = x);
        Assert.Equal(1, done);
        Assert.Same(xtwo, item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xfour, chain[1]);
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

        IElement? item = null;
        var done = chain.Replace(1, new Chain(false, []), x => item = x);
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
        IElement? item = null;
        var done = chain.Replace(1, new Chain(false, [xalpha, xbeta]), x => item = x);
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

        IElement? item = null;
        var done = chain.RemoveAt(0, x => item = x);
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
        List<IElement> items = [];
        var chain = new Chain(false, [xone, xtwo, xthree, xfour]);
        var done = chain.RemoveRange(0, 0, x => items.Add(x));
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
        List<IElement> items = [];
        var chain = new Chain(false, [xone, xtwo, xthree, xfour]);
        var done = chain.RemoveRange(1, 3, x => items.Add(x));
        Assert.Equal(3, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xfour, items[2]);
        Assert.Single(chain);
        Assert.Same(xone, chain[0]);

        items = [];
        chain = new Chain(false, [xone, xtwo, xthree, xfour]);
        done = chain.RemoveRange(0, 4, x => items.Add(x));
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
    public static void Test_Remove_Item()
    {
        IElement? item;
        var done = 0;
        var chain = new Chain(false) { IsValidDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        item = null;
        done = chain.Remove(xfour);
        Assert.Equal(0, done);
        Assert.Null(item);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
        Assert.Same(xone, chain[3]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        item = null;
        done = chain.Remove(xone, x => item = x);
        Assert.Equal(1, done);
        Assert.Same(xone, item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
        Assert.Same(xone, chain[2]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        item = null;
        done = chain.Remove(new Named("ONE"), x => item = x);
        Assert.Equal(1, done);
        Assert.Same(xone, item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
        Assert.Same(xone, chain[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Item()
    {
        IElement? item;
        var done = 0;
        var chain = new Chain(false) { IsValidDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        item = null;
        done = chain.RemoveLast(xone, x => item = x);
        Assert.Equal(1, done);
        Assert.Same(xone, item);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Item()
    {
        List<IElement> items;
        var done = 0;
        var chain = new Chain(false) { IsValidDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        items = [];
        done = chain.RemoveAll(xone, x => items.Add(x));
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        items = [];
        done = chain.RemoveAll(new Named("ONE"), x => items.Add(x));
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
    public static void Test_Remove_Item_Flatten()
    {
        List<IElement> items;
        var done = 0;
        var chain = new Chain(false) { IsValidDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.Remove(new Chain(false));
        Assert.Equal(0, done);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        items = [];
        done = chain.Remove(new Chain(false, [xone, xthree]), x => items.Add(x));
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xone, chain[1]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        items = [];
        done = chain.RemoveLast(new Chain(false, [xone, xthree]), x => items.Add(x));
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        items = [];
        done = chain.RemoveAll(new Chain(false, [xone, xthree]), x => items.Add(x));
        Assert.Equal(3, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Single(chain);
        Assert.Same(xtwo, chain[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        List<IElement> items;
        var done = 0;
        var chain = new Chain(false) { IsValidDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        items = [];
        done = chain.Remove(x => x is Named named && named.Name.Contains('e'), x => items.Add(x));
        Assert.Equal(1, done);
        Assert.Single(items);
        Assert.Same(xone, items[0]);
        Assert.Equal(3, chain.Count);
        Assert.Same(xtwo, chain[0]);
        Assert.Same(xthree, chain[1]);
        Assert.Same(xone, chain[2]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        items = [];
        done = chain.RemoveLast(x => x is Named named && named.Name.Contains('e'), x => items.Add(x));
        Assert.Equal(1, done);
        Assert.Single(items);
        Assert.Same(xone, items[0]);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        items = [];
        done = chain.RemoveAll(x => x is Named named && named.Name.Contains('e'), x => items.Add(x));
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
    */
}