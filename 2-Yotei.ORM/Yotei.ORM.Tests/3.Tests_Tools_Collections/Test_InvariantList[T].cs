using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Yotei.ORM.Tests.Collections;

// ========================================================
//[Enforced]
public static partial class Test_InvariantList_T
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

    // ----------------------------------------------------

    partial class Chain
    {
        [Cloneable]
        [DebuggerDisplay("{ToDebugString(3)}")]
        public partial class Builder : CoreList<IElement>
        {
            public Builder(bool sensitive)
            {
                Sensitive = sensitive;
                ValidateElement = static x => x.ThrowWhenNull(); // No nulls...
                FlattenElements = true; // Flatten input elements...
                CompareElements = (x, y) => new MyComparer(Sensitive).Equals(x, y); // On-stack just for testing...
                IncludeDuplicate = static (_, _) => throw new DuplicateException(); // No duplicates...
            }

            public Builder(
                bool sensitive, IEnumerable<IElement> range) : this(sensitive) => AddRange(range);

            protected Builder(Builder source)
                : base(source.ThrowWhenNull())
                => Sensitive = source.Sensitive;

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
    }

    // ----------------------------------------------------

    [Cloneable(ReturnType = typeof(IInvariantList<IElement>))]
    [DebuggerDisplay("{ToDebugString(3)}")]
    public partial class Chain : InvariantList<IElement>, IElement
    {
        protected override Builder Items { get; }
        public Chain(Builder builder) => Items = builder.ThrowWhenNull();

        public Chain(bool sensitive) => Items = new Builder(sensitive);
        public Chain(bool sensitive, IEnumerable<IElement> range) : this(sensitive) => Items.AddRange(range);
        protected Chain(Chain source) => Items = source.ThrowWhenNull().Items.Clone();
        
        public override Builder ToBuilder() => (Builder)base.ToBuilder();
        public bool Sensitive => Items.Sensitive;
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

        ValidateSetting(source, (Chain)target, nameof(CoreList<>.ValidateElement));
        ValidateSetting(source, (Chain)target, nameof(CoreList<>.FlattenElements));
        ValidateSetting(source, (Chain)target, nameof(CoreList<>.CompareElements));
        ValidateSetting(source, (Chain)target, nameof(CoreList<>.GetDuplicates));
        ValidateSetting(source, (Chain)target, nameof(CoreList<>.IncludeDuplicate));

        static void ValidateSetting(Chain source, Chain target, string name)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var items = typeof(Chain).GetProperties(flags).Where(x =>
                x.Name == "Items" &&
                x.DeclaringType == typeof(Chain)).First();

            flags = BindingFlags.Public | BindingFlags.Instance;
            var prop = typeof(Chain.Builder).GetProperty(name, flags);
            Assert.NotNull(prop);

            var svalue = prop.GetValue(items.GetValue(source));
            var tvalue = prop.GetValue(items.GetValue(target));
            Assert.Equal(svalue, tvalue);
        }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Item()
    {
        var builder = new Chain.Builder(false) { IncludeDuplicate = (_, _) => true };
        var chain = new Chain(builder);
        chain = (Chain)chain.AddRange([xone, xtwo, xone, xthree]);

        var index = chain.IndexOf(xfour); Assert.Equal(-1, index);

        index = chain.IndexOf(xone); Assert.Equal(0, index);
        index = chain.IndexOf(new Named("ONE")); Assert.Equal(0, index);

        index = chain.LastIndexOf(xone); Assert.Equal(2, index);
        index = chain.LastIndexOf(new Named("ONE")); Assert.Equal(2, index);

        var nums = chain.IndexesOf(xone);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(2, nums[1]);

        nums = chain.IndexesOf(new Named("ONE"));
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(2, nums[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var builder = new Chain.Builder(false) { IncludeDuplicate = (_, _) => true };
        var chain = new Chain(builder);
        chain = (Chain)chain.AddRange([xone, xtwo, xone, xthree]);

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
        var builder = new Chain.Builder(false) { IncludeDuplicate = (_, _) => true };
        var chain = new Chain(builder);
        chain = (Chain)chain.AddRange([xone, xtwo, xone, xthree]);

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
        var source = new Chain(false, [xone, xtwo]);
        
        var target = source.Add(xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(xone); Assert.Fail(); } catch (DuplicateException) { }
        try { source.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        var builder = new Chain.Builder(false) { IncludeDuplicate = (_, _) => true };
        source = new Chain(builder);
        source = (Chain)source.AddRange([xone, xtwo]);

        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Flatten()
    {
        var source = new Chain(false, [xone, xtwo]);
        var target = source.Add(new Chain(false));
        Assert.Same(source, target);

        target = source.Add(new Chain(false, [xthree, xfour]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { source.Add(new Chain(false, [new Named("ONE")])); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Chain(false, [xone, xtwo]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Flatten()
    {
        var source = new Chain(false, [xone, xtwo]);
        var target = source.AddRange([new Chain(false, [xthree, xfour]), new Named("FIVE")]);
        
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Equal("FIVE", ((Named)target[4]).Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Chain(false, [xone, xtwo]);

        var target = source.Insert(2, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.Insert(0, xfour);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);

        try { source.Insert(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Insert(0, xone); Assert.Fail(); } catch (DuplicateException) { }
        try { source.Insert(0, new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        var builder = new Chain.Builder(false) { IncludeDuplicate = (_, _) => true };
        source = new Chain(builder);
        source = (Chain)source.AddRange([xone, xtwo]);

        target = source.Insert(2, xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Flatten()
    {
        var source = new Chain(false, [xone, xtwo]);
        var target = source.Insert(0, new Chain(false));
        Assert.Same(source, target);

        target = source.Insert(2, new Chain(false, [xthree, xfour]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { source.Insert(0, new Chain(false, [new Named("ONE")])); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Chain(false, [xone, xtwo]);
        var target = source.InsertRange(0, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Flatten()
    {
        var source = new Chain(false, [xone, xtwo]);
        var target = source.InsertRange(2, [new Chain(false, [xthree, xfour]), new Named("FIVE")]);

        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Equal("FIVE", ((Named)target[4]).Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_Same()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);

        var target = source.Replace(1, xtwo);
        Assert.Same(source, target);

        target = source.Replace(1, new Named("TWO"), out var item);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Standard()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);

        var target = source.Replace(1, xfour, out var item);
        Assert.NotSame(source, target);
        Assert.Same(xtwo, item);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xfour, target[1]);
        Assert.Same(xthree, target[2]);

        try { source.Replace(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Replace(1, new Named("ONE")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);

        var target = source.Replace(1, new Chain(false, []), out var item);
        Assert.Same(source, target);
    }
}