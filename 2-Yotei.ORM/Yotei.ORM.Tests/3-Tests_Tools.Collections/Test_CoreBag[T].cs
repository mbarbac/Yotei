#pragma warning disable xUnit2017

namespace Yotei.ORM.Tools.Collections.Tests;

// ========================================================
//[Enforced]
public static partial class Test_CoreBag_T
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

    [Cloneable(ReturnType = typeof(ICoreBag<IElement>))]
    [DebuggerDisplay("{ToDebugString(3)}")]
    public partial class Chain : CoreBag<IElement>, IElement
    {
        public Chain()
        {
            IgnoreCase = false;
            AcceptDuplicates = false;
        }
        public Chain(IEnumerable<IElement> range) : this() => AddRange(range);
        protected Chain(Chain other) : base(other) { }
        protected override void OnCreating(CoreBag<IElement> other)
        {
            base.OnCreating(other);
            IgnoreCase = ((Chain)other).IgnoreCase;
            AcceptDuplicates = ((Chain)other).AcceptDuplicates;
        }

        public override IElement ValidateElement(IElement value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return value;
        }
        public override bool CompareElements(IElement source, IElement target)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);

            return source is Named snamed && target is Named tnamed
                ? string.Compare(snamed.Name, tnamed.Name, IgnoreCase) == 0
                : ReferenceEquals(source, target);
        }
        public override bool AllowDuplicate(IElement value, IEnumerable<IElement> range)
        {
            if (AcceptDuplicates) return true;
            throw new DuplicateException("Duplicated value").WithData(value);
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

        chain = new([xone, xtwo, xthree]);
        Assert.Equal(3, chain.Count);
        Assert.Contains(xone, chain);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xthree, chain);

        chain = new([xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.True(chain.Contains(new Named("ONE")));

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
        Assert.Contains(xone, chain);
        Assert.Contains(xone, chain);

        chain = new Chain() { IgnoreCase = true };
        try { chain.AddRange([xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain() { IgnoreCase = true, AcceptDuplicates = true };
        chain.AddRange([xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.True(chain.Contains(new Named("ONE")));
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

        source = new Chain([xone, xtwo])
        {
            FlattenElements = false,
            AcceptDuplicates = true,
            IgnoreCase = true
        };
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.IsType<Chain>(target);
        Assert.False(((Chain)target).FlattenElements);
        Assert.True(((Chain)target).AcceptDuplicates);
        Assert.True(((Chain)target).IgnoreCase);
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

        Assert.False(chain.TryFind(x => x is null, out item));
        Assert.Null(item);

        Assert.True(chain.TryFind(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xone, item);

        Assert.True(chain.TryFindAll(x => x is Named named && named.Name.Contains('e'), out range));
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
        Assert.Contains(xone, chain);

        done = chain.Add(xtwo);
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.Contains(xtwo, chain);

        try { chain.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { chain.Add(xone); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain([xone]) { IgnoreCase = true };
        try { chain.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        chain.AcceptDuplicates = true;
        done = chain.Add(xone);
        Assert.Equal(1, done);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.Contains(xone, chain);

        done = chain.Add(new Named("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Contains(xone, chain);
        Assert.Contains(xone, chain);
        Assert.True(chain.Contains(new Named("ONE")));
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested()
    {
        var chain = new Chain([xone, xtwo]);
        var done = chain.Add(new Chain([xthree, xfour]));

        Assert.Equal(2, done);
        Assert.Equal(4, chain.Count);
        Assert.Contains(xone, chain);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xthree, chain);
        Assert.Contains(xfour, chain);
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
        Assert.Contains(xone, chain);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xthree, chain);
        Assert.Contains(xfour, chain);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Nested()
    {
        var chain = new Chain([xone, xtwo]);
        var done = chain.AddRange([xthree, new Chain([xfour, xfive])]);

        Assert.Equal(3, done);
        Assert.Equal(5, chain.Count);
        Assert.Contains(xone, chain);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xthree, chain);
        Assert.Contains(xfour, chain);
        Assert.Contains(xfive, chain);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_value()
    {
        var chain = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        var done = chain.Remove(new Named("any"));
        Assert.Equal(0, done);

        done = chain.Remove(xone);
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xone, chain);
        Assert.Contains(xthree, chain);

        chain = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);
        done = chain.Remove(new Named("ONE"));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xone, chain);
        Assert.Contains(xthree, chain);

        chain = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);
        done = chain.RemoveAll(xone);
        Assert.Equal(2, done);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xthree, chain);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value_Nested()
    {
        var chain = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        var done = chain.Remove(new Chain([xtwo, xone]));
        Assert.Equal(2, done);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.Contains(xthree, chain);

        chain = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        done = chain.RemoveAll(new Chain([xtwo, xone]));
        Assert.Equal(3, done);
        Assert.Single(chain);
        Assert.Contains(xthree, chain);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var chain = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);

        var done = chain.Remove(x => x is Named named && named.Name is null);
        Assert.Equal(0, done);

        done = chain.Remove(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(1, done);
        Assert.Equal(3, chain.Count);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xone, chain);
        Assert.Contains(xthree, chain);

        chain = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        chain.AddRange([xone, xtwo, xone, xthree]);
        done = chain.RemoveAll(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(2, done);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xthree, chain);
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