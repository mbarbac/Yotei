namespace Yotei.ORM.Tests.Collections;

// ========================================================
//[Enforced]
public static partial class Test_InvariantList_KT
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
        public partial class Builder : CoreList<string, IElement>
        {
            static string OnGetKey(IElement item) => item is Named named ? named.Name : null!;
            public Builder(bool sensitive) : base(OnGetKey)
            {
                Sensitive = sensitive;

                FlattenElements = true; // Flatten input elements...
                ValidateKey = static x => x.NotNullNotEmpty(true); // Nulls or empties not accepted...
                CompareKeys = (x, y) => new MyComparer(Sensitive).Equals(x, y); // On-stack just for testing...
                ValidateElement = static x => x.ThrowWhenNull(); // Nulls not accepted...
                IncludeDuplicate = static (_, _) => throw new DuplicateException(); // No duplicates...
            }

            public Builder(
                bool sensitive, IEnumerable<IElement> range) : this(sensitive) => AddRange(range);

            protected Builder(Builder source)
                : base(source.ThrowWhenNull())
                => Sensitive = source.Sensitive;

            public bool Sensitive { get; }
            readonly struct MyComparer(bool Sensitive) : IEqualityComparer<string?>
            {
                public bool Equals(string? x, string? y) => string.Compare(x, y, !Sensitive) == 0;
                public int GetHashCode(string? obj) => throw new NotImplementedException();
            }
        }
    }

    // ----------------------------------------------------

    [Cloneable(ReturnType = typeof(IInvariantList<string, IElement>))]
    [DebuggerDisplay("{ToDebugString(3)}")]
    public partial class Chain : InvariantList<string, IElement>, IElement
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

        ValidateSetting(source, (Chain)target, nameof(CoreList<,>.GetKey));
        ValidateSetting(source, (Chain)target, nameof(CoreList<,>.ValidateKey));
        ValidateSetting(source, (Chain)target, nameof(CoreList<,>.CompareKeys));
        ValidateSetting(source, (Chain)target, nameof(CoreList<,>.ValidateElement));
        ValidateSetting(source, (Chain)target, nameof(CoreList<,>.FlattenElements));
        ValidateSetting(source, (Chain)target, nameof(CoreList<,>.GetDuplicates));
        ValidateSetting(source, (Chain)target, nameof(CoreList<,>.IncludeDuplicate));

        static void ValidateSetting(Chain source, Chain target, string name)
        {
            var bsource = GetBuilder(source);
            var btarget = GetBuilder(target);

            var vsource = GetBuilderValue(bsource, name);
            var vtarget = GetBuilderValue(btarget, name);
            Assert.Equal(vsource, vtarget);
        }
    }

    public static Chain.Builder GetBuilder(Chain chain)
    {
        var flags = BindingFlags.NonPublic | BindingFlags.Instance;
        var prop = typeof(Chain).GetProperties(flags).Where(x =>
            x.Name == "Items" &&
            x.DeclaringType == typeof(Chain)).First();

        var builder = (Chain.Builder)prop.GetValue(chain)!;
        Assert.NotNull(builder);
        return builder;
    }

    public static PropertyInfo GetBuilderProperty(string name)
    {
        var flags = BindingFlags.Public | BindingFlags.Instance;
        var prop = typeof(Chain.Builder).GetProperty(name, flags);
        Assert.NotNull(prop);
        return prop;
    }

    public static object? GetBuilderValue(Chain.Builder builder, string name)
    {
        var prop = GetBuilderProperty(name);
        return prop.GetValue(builder);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Key()
    {
        var builder = new Chain.Builder(false) { IncludeDuplicate = (_, _) => true };
        var chain = new Chain(builder);
        chain = (Chain)chain.AddRange([xone, xtwo, xone, xthree]);

        var index = chain.IndexOf("FOUR"); Assert.Equal(-1, index);

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

    /*

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

    //[Enforced]
    [Fact]
    public static void Test_Replace_Many()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);

        var xalpha = new Named("alpha");
        var xbeta = new Named("beta");
        var target = source.Replace(1, new Chain(false, [xalpha, xbeta]), out var item);

        Assert.NotSame(source, target);
        Assert.Same(xtwo, item);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xalpha, target[1]);
        Assert.Same(xbeta, target[2]);
        Assert.Same(xthree, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);

        var target = source.RemoveAt(0, out var item);
        Assert.NotSame(source, target);
        Assert.Same(xone, item);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_Empty()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xfour]);
        var target = source.RemoveRange(0, 0, out var items);
        
        Assert.Same(source, target);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_Others()
    {
        var source = new Chain(false, [xone, xtwo, xthree, xfour]);
        
        var target = source.RemoveRange(1, 3, out var items);
        Assert.NotSame(source, target);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xfour, items[2]);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        target = source.RemoveRange(0, 4, out items);
        Assert.NotSame(source, target);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Empty(target);

        try { source.RemoveRange(0, -1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(5, 0); Assert.Fail(); } catch (ArgumentException) { }
        try { source.RemoveRange(0, 5); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var builder = new Chain.Builder(false) { IncludeDuplicate = (_, _) => true };
        var source = new Chain(builder);
        source = (Chain)source.AddRange([xone, xtwo, xthree, xone]);

        var target = source.Remove(xfour, out var items);
        Assert.Same(source, target);
        Assert.Empty(items);

        target = source.Remove(xone, out items);
        Assert.NotSame(source, target);
        Assert.Single(items);
        Assert.Same(xone, items[0]);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.Remove(new Named("ONE"), out items);
        Assert.NotSame(source, target);
        Assert.Single(items);
        Assert.Same(xone, items[0]);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Item()
    {
        var builder = new Chain.Builder(false) { IncludeDuplicate = (_, _) => true };
        var source = new Chain(builder);
        source = (Chain)source.AddRange([xone, xtwo, xthree, xone]);

        var target = source.RemoveLast(xone, out var items);
        Assert.NotSame(source, target);
        Assert.Single(items);
        Assert.Same(xone, items[0]);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(new Named("ONE"), out items);
        Assert.NotSame(source, target);
        Assert.Single(items);
        Assert.Same(xone, items[0]);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Item()
    {
        var builder = new Chain.Builder(false) { IncludeDuplicate = (_, _) => true };
        var source = new Chain(builder);
        source = (Chain)source.AddRange([xone, xtwo, xthree, xone]);

        var target = source.RemoveAll(xone, out var items);
        Assert.NotSame(source, target);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAll(new Named("ONE"), out items);
        Assert.NotSame(source, target);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_Flatten()
    {
        var builder = new Chain.Builder(false) { IncludeDuplicate = (_, _) => true };
        var source = new Chain(builder);
        source = (Chain)source.AddRange([xone, xtwo, xthree, xone]);

        var target = source.Remove(new Chain(false), out var items);
        Assert.Same(source, target);
        Assert.Empty(items);

        target = source.Remove(new Chain(false, [xone, xthree]), out items);
        Assert.NotSame(source, target);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);

        target = source.RemoveLast(new Chain(false, [xone, xthree]), out items);
        Assert.NotSame(source, target);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        target = source.RemoveAll(new Chain(false, [xone, xthree]), out items);
        Assert.NotSame(source, target);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Single(target);
        Assert.Same(xtwo, target[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var builder = new Chain.Builder(false) { IncludeDuplicate = (_, _) => true };
        var source = new Chain(builder);
        source = (Chain)source.AddRange([xone, xtwo, xthree, xone]);

        var target = source.Remove(x => x is Named named && named.Name.Contains('e'), out var item);
        Assert.NotSame(source, target);
        Assert.Same(xone, item);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast(x => x is Named named && named.Name.Contains('e'), out item);
        Assert.NotSame(source, target);
        Assert.Same(xone, item);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => x is Named named && named.Name.Contains('e'), out var items);
        Assert.NotSame(source, target);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);
        Assert.Single(target);
        Assert.Same(xtwo, target[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain(false);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain(false, [xone, xtwo, xthree]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
    */
}