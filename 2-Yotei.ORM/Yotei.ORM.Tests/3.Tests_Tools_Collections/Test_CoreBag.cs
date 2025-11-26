#pragma warning disable xUnit2017

namespace Yotei.ORM.Tests.Collections;

// ========================================================
//[Enforced]
public static class Test_CoreBag
{
    public interface IElement { }

    public class Element(string name) : IElement
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? "-";
    }

    readonly struct MyComparer(bool Sensitive) : IEqualityComparer<IElement>
    {
        public bool Equals(IElement? x, IElement? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;

            return x is Element xnamed && y is Element ynamed
                ? string.Compare(xnamed.Name, ynamed.Name, !Sensitive) == 0
                : ReferenceEquals(x, y);
        }
        public int GetHashCode(IElement obj) => throw new NotImplementedException();
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var source = new CoreBag<IElement>();
        Assert.Empty(source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var xone = new Element("one");
        var xtwo = new Element("two");
        var xthree = new Element("three");

        var source = new CoreBag<IElement>([]);
        Assert.Empty(source);

        source = new CoreBag<IElement>([xone, xtwo, xthree]);
        Assert.Equal(3, source.Count);
        Assert.True(source.Contains(xone));
        Assert.True(source.Contains(xtwo));
        Assert.True(source.Contains(xthree));

        source = new CoreBag<IElement>([xone, xone]);
        Assert.Equal(2, source.Count);
        Assert.True(source.Contains(xone));
        Assert.True(source.Contains(xone));

        try
        {
            source = new CoreBag<IElement>([xone, xone])
            { IsValidDuplicate = (x, y) => throw new DuplicateException(), };
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_NoSensitive()
    {
        var xone = new Element("one");
        var xtwo = new Element("two");
        var xthree = new Element("three");

        var source = new CoreBag<IElement>([])
        {
            AreEqual = (x, y) => new MyComparer(false).Equals(x, y),
            IsValidDuplicate = (x, y) => throw new DuplicateException(),
        };
        Assert.Empty(source);

        source = new CoreBag<IElement>([xone, xtwo, xthree])
        {
            IsValidDuplicate = (x, y) => throw new DuplicateException(),
            AreEqual = (x, y) => new MyComparer(false).Equals(x, y),
        };
        Assert.Equal(3, source.Count);
        Assert.True(source.Contains(xone));
        Assert.True(source.Contains(xtwo));
        Assert.True(source.Contains(xthree));

        try
        {
            source = new CoreBag<IElement>([xone, new Element("ONE")])
            {
                AreEqual = (x, y) => new MyComparer(false).Equals(x, y),
                IsValidDuplicate = (x, y) => throw new DuplicateException(),
            };
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var xone = new Element("one");
        var xtwo = new Element("two");
        var xthree = new Element("three");
        var source = new CoreBag<IElement>([xone, xtwo, xthree])
        {
            AreEqual = (x, y) => new MyComparer(false).Equals(x, y),
            IsValidDuplicate = (x, y) => throw new DuplicateException(),
        };

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains(xone));
        Assert.True(target.Contains(xtwo));
        Assert.True(target.Contains(xthree));

        Assert.Same(source.Validate, ((CoreBag<IElement>)target).Validate);
        Assert.Equal(source.FlattenElements, ((CoreBag<IElement>)target).FlattenElements);
        Assert.Same(source.IsValidDuplicate, ((CoreBag<IElement>)target).IsValidDuplicate);
        Assert.Same(source.AreEqual, ((CoreBag<IElement>)target).AreEqual);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var xone = new Element("one");
        var xtwo = new Element("two");
        var xthree = new Element("three");
        var source = new CoreBag<IElement>([xone, xtwo, xthree])
        {
            AreEqual = (x, y) => new MyComparer(false).Equals(x, y),
            IsValidDuplicate = (x, y) => throw new DuplicateException(),
        };

        Assert.False(source.Find(x => x is null, out var item));
        Assert.Null(item);

        Assert.True(source.Find(x => x is Element named && named.Name.Contains('e'), out item));
        Assert.True(ReferenceEquals(xone, item) || ReferenceEquals(xthree, item));
    }

    //[Enforced]
    [Fact]
    public static void Test_FindAll()
    {
        var xone = new Element("one");
        var xtwo = new Element("two");
        var xthree = new Element("three");
        var source = new CoreBag<IElement>([xone, xtwo, xthree])
        {
            AreEqual = (x, y) => new MyComparer(false).Equals(x, y),
            IsValidDuplicate = (x, y) => throw new DuplicateException(),
        };

        Assert.True(source.FindAll(
            x => x is Element named && named.Name.Contains('e'),
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
        var xone = new Element("one");
        var xtwo = new Element("two");
        var xthree = new Element("three");
        var source = new CoreBag<IElement>([xone, xtwo]);

        var done = source.Add(xthree);
        Assert.Equal(1, done);
        Assert.Equal(3, source.Count);
        Assert.True(source.Contains(xone));
        Assert.True(source.Contains(xtwo));
        Assert.True(source.Contains(xthree));

        source = new CoreBag<IElement>([xone, xtwo])
        {
            Validate = (x) => x.ThrowWhenNull(),
            AreEqual = (x, y) => new MyComparer(false).Equals(x, y),
            IsValidDuplicate = (x, y) => throw new DuplicateException(),
        };
        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }
}