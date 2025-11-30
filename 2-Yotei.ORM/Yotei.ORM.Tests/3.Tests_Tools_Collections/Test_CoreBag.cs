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

    [Cloneable(ReturnType = typeof(ICoreBag<IElement>))]
    public partial class Chain : CoreBag<IElement>, IElement
    {
        // Custom behaviors for testing purposes...
        public Chain(bool sensitive) : base()
        {
            Sensitive = sensitive;
            ValidateElement = static x => x.ThrowWhenNull(); // Nulls not accepted...
            FlattenElements = true; // Flatten input elements...
            CompareItems = (x, y) => new MyComparer(Sensitive).Equals(x, y); // On-stack just for testing...
            IncludeDuplicate = static (_, _) => throw new DuplicateException(); // No duplicates...
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

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.Clone();

        Assert.Equal(3, target.Count);
        Assert.True(target.Contains(xone));
        Assert.True(target.Contains(xtwo));
        Assert.True(target.Contains(xthree));

        Assert.Same(source.ValidateElement, ((Chain)target).ValidateElement);
        Assert.Equal(source.FlattenElements, ((Chain)target).FlattenElements);
        Assert.Same(source.CompareItems, ((Chain)target).CompareItems);
        Assert.Same(source.GetDuplicates, ((Chain)target).GetDuplicates);
        Assert.Same(source.IncludeDuplicate, ((Chain)target).IncludeDuplicate);
        Assert.Equal(source.Sensitive, ((Chain)target).Sensitive);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find_Values()
    {
        var chain = new CoreBag<int>([1, 2, 3, 4]);

        Assert.False(chain.Find(x => x > 10, out var value));
        Assert.True(chain.FindAll(x => x is > 1 and < 4, out var values));
        Assert.Equal(2, values.Count);
        Assert.Contains(2, values);
        Assert.Contains(3, values);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var chain = new Chain(false) { IncludeDuplicate = (_, _) => true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        Assert.False(chain.Find(x => x is null, out var item));
        Assert.Null(item);

        Assert.True(chain.Find(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.True(ReferenceEquals(xone, item) || ReferenceEquals(xthree, item));

        Assert.True(chain.FindAll(x => x is Named named && named.Name.Contains('e'), out var items));
        Assert.Equal(3, items.Count);
        Assert.Contains(xone, items);
        Assert.Contains(xthree, items);
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
        Assert.True(chain.Contains(xone));
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));

        try { chain.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { chain.Add(xone); Assert.Fail(); } catch (DuplicateException) { }
        try { chain.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain(false, [xone, xtwo]) { IncludeDuplicate = (_, _) => true };
        done = chain.Add(xone);
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.True(chain.Contains(xone));
        Assert.True(chain.Contains(xtwo));
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
        Assert.True(chain.Contains(xone));
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));
        Assert.True(chain.Contains(xfour));

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
        Assert.True(chain.Contains(xone));
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));
        Assert.True(chain.Contains(xfour));
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Range_Flatten()
    {
        var chain = new Chain(false, [xone, xtwo]);
        var done = chain.AddRange([new Chain(false, [xthree, xfour]), new Named("FIVE")]);
        Assert.Equal(3, done);
        Assert.Equal(5, chain.Count);
        Assert.True(chain.Contains(xone));
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));
        Assert.True(chain.Contains(xfour));
        Assert.True(chain.Find(x => x is Named named && named.Name == "FIVE"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var done = 0;
        var chain = new Chain(false) { IncludeDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.Remove(xfour, out var items);
        Assert.Equal(0, done);
        Assert.Empty(items);
        Assert.Equal(4, chain.Count);
        Assert.True(chain.Contains(xone));
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.Remove(xone, out items);
        Assert.Equal(1, done);
        Assert.Single(items);
        Assert.Contains(xone, items);
        Assert.Equal(3, chain.Count);
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));
        Assert.True(chain.Contains(xone));

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.Remove(new Named("ONE"), out items);
        Assert.Equal(1, done);
        Assert.Single(items);
        Assert.Contains(xone, items);
        Assert.Equal(3, chain.Count);
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));
        Assert.True(chain.Contains(xone));
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Item()
    {
        var done = 0;
        var chain = new Chain(false) { IncludeDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.RemoveAll(xone, out var items);
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Contains(xone, items);
        Assert.Equal(2, chain.Count);
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.RemoveAll(new Named("ONE"), out items);
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Contains(xone, items);
        Assert.Equal(2, chain.Count);
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_Flatten()
    {
        var done = 0;
        var chain = new Chain(false) { IncludeDuplicate = (_, _) => true };

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.Remove(new Chain(false));
        Assert.Equal(0, done);

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.Remove(new Chain(false, [xone, xthree]), out var items);
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Contains(xone, items);
        Assert.Contains(xthree, items);
        Assert.Equal(2, chain.Count);
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xone));

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.RemoveAll(new Chain(false, [xone, xthree]), out items);
        Assert.Equal(3, done);
        Assert.Equal(3, items.Count);
        Assert.Contains(xone, items);
        Assert.Contains(xthree, items);
        Assert.Single(chain);
        Assert.True(chain.Contains(xtwo));
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
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));
        Assert.True(chain.Contains(xone));

        chain.Clear(); chain.AddRange([xone, xtwo, xthree, xone]);
        done = chain.RemoveAll(x => x is Named named && named.Name.Contains('e'), out var items);
        Assert.Equal(3, done);
        Assert.Equal(3, items.Count);
        Assert.Contains(xone, items);
        Assert.Contains(xthree, items);
        Assert.Same(xone, items[2]);
        Assert.Single(chain);
        Assert.True(chain.Contains(xtwo));
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