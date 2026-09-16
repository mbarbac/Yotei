/*#pragma warning disable xUnit2017, IDE0028, IDE0018

namespace Yotei.ORM.Tools.Collections.Tests;

// ========================================================
//[Enforced]
public static partial class Test_InvariantBag_T
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
    public partial class Builder : CoreBag<IElement>, IElement
    {
        public Builder()
        {
            IgnoreCase = false;
            AcceptDuplicates = false;
        }
        public Builder(IEnumerable<IElement> range) : this() => AddRange(range);
        protected Builder(Builder other) : base(other) { }
        protected override void OnCreating(CoreBag<IElement> other)
        {
            base.OnCreating(other);
            IgnoreCase = ((Builder)other).IgnoreCase;
            AcceptDuplicates = ((Builder)other).AcceptDuplicates;
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

    [Cloneable(ReturnType = typeof(IInvariantBag<IElement>))]
    [DebuggerDisplay("{ToDebugString(3)}")]
    public partial class Chain : InvariantBag<IElement>, IElement
    {
        protected override Builder CreateItems() => [];

        public Chain() : base() { }
        public Chain(IEnumerable<IElement> range) : base(range) { }
        protected Chain(Chain other) : base(other) { }

        public Builder Builder => (Builder)Items;

        public bool FlattenElements { get => Builder.FlattenElements; set => Builder.FlattenElements = value; }
        public bool IgnoreCase { get => Builder.IgnoreCase; set => Builder.IgnoreCase = value; }
        public bool AcceptDuplicates { get => Builder.AcceptDuplicates; set => Builder.AcceptDuplicates = value; }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var source = new Chain();
        Assert.Empty(source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var source = new Chain([]);
        Assert.Empty(source);

        source = new([xone, xtwo, xthree]);
        Assert.Equal(3, source.Count);
        Assert.Contains(xone, source);
        Assert.Contains(xtwo, source);
        Assert.Contains(xthree, source);

        source = new([xone, new Named("ONE")]);
        Assert.Equal(2, source.Count);
        Assert.Contains(xone, source);
        Assert.True(source.Contains(new Named("ONE")));

        try { _ = new Chain(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain([xone, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain([xone, xone]); Assert.Fail(); } catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_With_Duplicates()
    {
        var source = new Chain() { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xone]);
        Assert.Equal(2, source.Count);
        Assert.Contains(xone, source);
        Assert.Contains(xone, source);

        source = new Chain() { IgnoreCase = true };
        try { source.AddRange([xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }

        source = new Chain() { IgnoreCase = true, AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, new Named("ONE")]);
        Assert.Equal(2, source.Count);
        Assert.Contains(xone, source);
        Assert.True(source.Contains(new Named("ONE")));
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
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        Assert.False(source.TryFind(x => x is null, out item));
        Assert.Null(item);

        Assert.True(source.TryFind(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xone, item);

        Assert.True(source.TryFindAll(x => x is Named named && named.Name.Contains('e'), out range));
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
        var source = new Chain();
        var target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Contains(xone, target);

        source = (Chain)target;
        target = source.Add(xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(xone); Assert.Fail(); } catch (DuplicateException) { }

        source = new Chain([xone]) { IgnoreCase = true };
        try { source.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source.AcceptDuplicates = true;
        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xone, target);

        source = (Chain)target;
        target = source.Add(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xone, target);
        Assert.True(target.Contains(new Named("ONE")));
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.Add(new Chain([xthree, xfour]));

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
        Assert.Contains(xfour, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
        Assert.Contains(xfour, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Nested()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.AddRange([xthree, new Chain([xfour, xfive])]);

        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
        Assert.Contains(xfour, target);
        Assert.Contains(xfive, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_value()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(new Named("any"));
        Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        target = source.Remove(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        target = source.RemoveAll(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value_Nested()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(new Chain([xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        target = source.RemoveAll(new Chain([xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Contains(xthree, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain() { AcceptDuplicates = true, IgnoreCase = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(x => x is Named named && named.Name is null);
        Assert.Same(source, target);

        target = source.Remove(x => x is Named named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        target = source.RemoveAll(x => x is Named named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain();
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain([xone, xtwo, xthree]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}*/