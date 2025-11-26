#pragma warning disable xUnit2017
using System.Xml.Schema;

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
            Validate = static x => x.ThrowWhenNull();
            FlattenElements = true;
            AreEqual = (x, y) => new MyComparer(Sensitive).Equals(x, y); // Creating new comparer on the stack for testing purposes only...
            IsValidDuplicate = (x, y) => throw new DuplicateException();
        }

        public Chain(bool sensitive, IEnumerable<IElement> range)
            : this(sensitive)
            => AddRange(range);

        protected Chain(Chain source) : base(source) => Sensitive = source.Sensitive;

        public bool Sensitive
        {
            get;
            set
            {
                if (field == value) return;

                if (Count == 0) field = value;
                else
                {
                    var range = ToArray();
                    Clear();
                    field = value; AddRange(range);
                }
            }
        }

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
        var source = new Chain(false, []);
        Assert.Empty(source);

        source = new Chain(false, [xone, xtwo, xthree]);
        Assert.Equal(3, source.Count);
        Assert.True(source.Contains(xone)); Assert.True(source.Contains(new Named("ONE")));
        Assert.True(source.Contains(xtwo));
        Assert.True(source.Contains(xthree));
        Assert.False(source.Contains(xfour));

        source = new Chain(true, [xone, xtwo, xthree]);
        Assert.True(source.Contains(xone)); Assert.False(source.Contains(new Named("ONE")));

        try { source = new Chain(false, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source = new Chain(false, [xone, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source = new Chain(false, [xone, xone]); Assert.Fail(); } catch (DuplicateException) { }
        try { source = new Chain(false, [xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains(xone));
        Assert.True(target.Contains(xtwo));
        Assert.True(target.Contains(xthree));

        Assert.Same(source.Validate, ((Chain)target).Validate);
        Assert.Equal(source.FlattenElements, ((Chain)target).FlattenElements);
        Assert.Same(source.AreEqual, ((Chain)target).AreEqual);
        Assert.Same(source.IsValidDuplicate, ((Chain)target).IsValidDuplicate);
        Assert.Equal(source.Sensitive, ((Chain)target).Sensitive);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);

        Assert.False(source.Find(x => x is null, out var item));
        Assert.Null(item);

        Assert.True(source.Find(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.True(ReferenceEquals(xone, item) || ReferenceEquals(xthree, item));

        Assert.True(source.FindAll(
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
        var source = new Chain(false, [xone, xtwo]);
        var done = source.Add(xthree);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.True(source.Contains(xone));
        Assert.True(source.Contains(xtwo));
        Assert.True(source.Contains(xthree));

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(xone); Assert.Fail(); } catch (DuplicateException) { }
        try { source.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source = new Chain(false, [xone, xtwo]) { IsValidDuplicate = (_, _) => true };
        done = source.Add(xone);
        Assert.Equal(1, done);
        Assert.True(source.Contains(xone));
        Assert.True(source.Contains(xtwo));
        Assert.True(source.FindAll(x => ReferenceEquals(x, xone), out var items));
        Assert.Equal(2, items.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Flatten()
    {
        var source = new Chain(false, [xone, xtwo]);
        var done = source.Add(new Chain(false));
        Assert.Equal(0, done);

        done = source.Add(new Chain(false, [xthree, xfour]));
        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.True(source.Contains(xone));
        Assert.True(source.Contains(xtwo));
        Assert.True(source.Contains(xthree));
        Assert.True(source.Contains(xfour));
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Range()
    {
        var source = new Chain(false, [xone, xtwo]);
        var done = source.AddRange([]);
        Assert.Equal(0, done);

        done = source.AddRange([xthree, xfour]);
        Assert.Equal(2, done);
        Assert.Equal(4, source.Count);
        Assert.True(source.Contains(xone));
        Assert.True(source.Contains(xtwo));
        Assert.True(source.Contains(xthree));
        Assert.True(source.Contains(xfour));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var done = source.Remove(xfour);
        Assert.Equal(0, done);

        done = source.Remove(xtwo);
        Assert.Equal(1, done);
        Assert.Equal(2, source.Count);
        Assert.True(source.Contains(xone));
        Assert.True(source.Contains(xthree));
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_Flatten()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var done = source.Remove(new Chain(false));
        Assert.Equal(0, done);

        done = source.Remove(new Chain(false, [xtwo, xfour]));
        Assert.Equal(1, done);
        Assert.Equal(2, source.Count);
        Assert.True(source.Contains(xone));
        Assert.True(source.Contains(xthree));

        source = new Chain(false) { IsValidDuplicate = (_, _) => true };
        source.AddRange([xone, xtwo, xone]);
        done = source.Remove(new Chain(false, [xtwo, xfour]));
        Assert.Equal(1, done);
        Assert.Equal(2, source.Count);
        Assert.True(source.Contains(xone));
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_All()
    {
        var source = new Chain(false) { IsValidDuplicate = (_, _) => true };
        source.AddRange([xone, xtwo, xthree, xone]);

        var done = source.RemoveAll(xone, out var items);
        Assert.Equal(2, done);
        Assert.Equal(2, source.Count);
        Assert.True(source.Contains(xtwo));
        Assert.True(source.Contains(xthree));
        Assert.Equal(2, items.Count);
        
        Assert.Contains(xone, items);
        Assert.Contains(xone, items);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var done = source.Remove(x => x is Named named && named.Name.Contains('e'), out var item);
        
        Assert.Equal(1, done);
        Assert.Equal(2, source.Count);
        Assert.True(source.Contains(xtwo));
        Assert.True(ReferenceEquals(xone, item) || ReferenceEquals(xthree, item));
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate_All()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var done = source.RemoveAll(x => x is Named named && named.Name.Contains('e'), out var items);
        
        Assert.Equal(2, done);
        Assert.Single(source);
        Assert.True(source.Contains(xtwo));
        Assert.Equal(2, items.Count);
        
        Assert.Contains(xone, items);
        Assert.Contains(xthree, items);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain(false);
        var done = source.Clear();
        Assert.Equal(0, done);

        source = new Chain(false, [xone, xtwo, xthree]);
        done = source.Clear();
        Assert.Equal(3, done);
        Assert.Empty(source);
    }
}