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
        public Chain(bool sensitive) : base()
        {
            Sensitive = sensitive;
            Validate = static x => x.ThrowWhenNull(); // Nulls not accepted...
            FlattenElements = true; // Flatten collection elements...
            AreEqual = (x, y) => new MyComparer(Sensitive).Equals(x, y); // New comparer on stack for testing purposes only...
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
        var source = new Chain(false);
        Assert.Empty(source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var chain = new Chain(false, []);
        Assert.Empty(chain);

        chain = new Chain(false, [xone, xtwo, xthree]);
        Assert.Equal(3, chain.Count);
        Assert.True(chain.Contains(xone)); Assert.True(chain.Contains(new Named("ONE")));
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));
        Assert.False(chain.Contains(xfour));

        chain = new Chain(true, [xone, xtwo, xthree]);
        Assert.True(chain.Contains(xone)); Assert.False(chain.Contains(new Named("ONE")));

        try { chain = new Chain(false, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { chain = new Chain(false, [xone, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { chain = new Chain(false, [xone, xone]); Assert.Fail(); } catch (DuplicateException) { }
        try { chain = new Chain(false, [xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var target = chain.Clone();

        Assert.NotSame(chain, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains(xone));
        Assert.True(target.Contains(xtwo));
        Assert.True(target.Contains(xthree));

        Assert.Same(chain.Validate, ((Chain)target).Validate);
        Assert.Equal(chain.FlattenElements, ((Chain)target).FlattenElements);
        Assert.Same(chain.AreEqual, ((Chain)target).AreEqual);
        Assert.Same(chain.IsValidDuplicate, ((Chain)target).IsValidDuplicate);
        Assert.Equal(chain.Sensitive, ((Chain)target).Sensitive);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);

        Assert.False(chain.Find(x => x is null, out var item));
        Assert.Null(item);

        Assert.True(chain.Find(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.True(ReferenceEquals(xone, item) || ReferenceEquals(xthree, item));

        Assert.True(chain.FindAll(
            x => x is Named named && named.Name.Contains('e'),
            out var items));

        Assert.Equal(2, items.Count);
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

        chain = new Chain(false, [xone, xtwo]) { IsValidDuplicate = (_, _) => true };
        done = chain.Add(xone);
        Assert.Equal(1, done);
        Assert.True(chain.Contains(xone));
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.FindAll(x => ReferenceEquals(x, xone), out var items));
        Assert.Equal(2, items.Count);
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

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var done = chain.Remove(xfour);
        Assert.Equal(0, done);

        done = chain.Remove(xtwo);
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.True(chain.Contains(xone));
        Assert.True(chain.Contains(xthree));
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_Flatten()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var done = chain.Remove(new Chain(false));
        Assert.Equal(0, done);

        done = chain.Remove(new Chain(false, [xtwo, xfour]));
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.True(chain.Contains(xone));
        Assert.True(chain.Contains(xthree));

        chain = new Chain(false) { IsValidDuplicate = (_, _) => true };
        chain.AddRange([xone, xtwo, xone]);
        done = chain.Remove(new Chain(false, [xtwo, xfour]));
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.True(chain.Contains(xone));
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_All()
    {
        var chain = new Chain(false) { IsValidDuplicate = (_, _) => true };
        chain.AddRange([xone, xtwo, xthree, xone]);

        var done = chain.RemoveAll(xone, out var items);
        Assert.Equal(2, done);
        Assert.Equal(2, chain.Count);
        Assert.True(chain.Contains(xtwo));
        Assert.True(chain.Contains(xthree));
        Assert.Equal(2, items.Count);
        
        Assert.Contains(xone, items);
        Assert.Contains(xone, items);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var done = chain.Remove(x => x is Named named && named.Name.Contains('e'), out var item);
        
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.True(chain.Contains(xtwo));
        Assert.True(ReferenceEquals(xone, item) || ReferenceEquals(xthree, item));
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate_All()
    {
        var chain = new Chain(false, [xone, xtwo, xthree]);
        var done = chain.RemoveAll(x => x is Named named && named.Name.Contains('e'), out var items);
        
        Assert.Equal(2, done);
        Assert.Single(chain);
        Assert.True(chain.Contains(xtwo));
        Assert.Equal(2, items.Count);
        
        Assert.Contains(xone, items);
        Assert.Contains(xthree, items);
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