namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ArrayExtensions
{
    public readonly struct SItem<T>(T value) : ICloneable
    {
        public T Value { get; } = value;
        public override string ToString() => Value?.ToString() ?? "";
        public SItem<T> Clone() => new(Value);
        object ICloneable.Clone() => Clone();
    }

    public class CItem<T>(T value) : ICloneable
    {
        public T Value { get; } = value;
        public override string ToString() => Value?.ToString() ?? "";
        public CItem<T> Clone() => new(Value);
        object ICloneable.Clone() => Clone();
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Typed_Enumeration()
    {
        var max = 100;
        var num = 0;
        string?[] items = [.. Enumerable.Range(0, 100).Select(i => i.ToString())];
        foreach (var item in items) num++;
        Assert.Equal(max, num);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        // Value type...
        var ssource = new SItem<int>[] { new(1), new(2), new(3) };
        var starget = (SItem<int>[])ssource.Clone();
        Assert.NotSame(ssource, starget);
        for (int i = 0; i < 3; i++) Assert.Equal(ssource[i], starget[i]);

        starget = ssource.Clone(true);
        Assert.NotSame(ssource, starget);
        for (int i = 0; i < 3; i++) Assert.Equal(ssource[i], starget[i]);

        // Reference type...
        var csource = new CItem<int>[] { new(1), new(2), new(3) };
        var ctarget = (CItem<int>[])csource.Clone();
        Assert.NotSame(csource, ctarget);
        for (int i = 0; i < 3; i++) Assert.Same(csource[i], ctarget[i]);

        ctarget = csource.Clone(true);
        Assert.NotSame(csource, ctarget);
        for (int i = 0; i < 3; i++) Assert.NotSame(csource[i], ctarget[i]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf()
    {
        int index;
        var source = new SItem<int>[] { new(1), new(2), new(3) };

        // Using element's type equality...
        index = source.IndexOf(new SItem<int>(2)); Assert.Equal(1, index);

        // Using element's type equality and index...
        index = source.IndexOf(1, new SItem<int>(2)); Assert.Equal(1, index);
        index = source.IndexOf(2, new SItem<int>(2)); Assert.Equal(-1, index);
        index = source.LastIndexOf(1, new SItem<int>(2)); Assert.Equal(1, index);
        index = source.LastIndexOf(2, new SItem<int>(2)); Assert.Equal(-1, index);

        // Using predicate...
        index = source.IndexOf(x => x.Value < 3); Assert.Equal(0, index);
        index = source.LastIndexOf(x => x.Value < 3); Assert.Equal(1, index);

        // Using predicate and index...
        index = source.IndexOf(2, x => x.Value < 3); Assert.Equal(-1, index);
        index = source.LastIndexOf(2, x => x.Value < 3); Assert.Equal(-1, index);

        // Indexes...
        var list = source.IndexesOf(x => x.Value % 2 == 1);
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_TrimStart()
    {
        string?[] source = ["1", "2", "3"];
        var target = source.TrimStart(x => x == null);
        Assert.Same(source, target);

        source = [null, null, "1", "2", "3", null];
        target = source.TrimStart(x => x == null);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Length);
        Assert.Equal("1", target[0]);
        Assert.Equal("2", target[1]);
        Assert.Equal("3", target[2]);
        Assert.Null(target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_TrimEnd()
    {
        string?[] source = ["1", "2", "3"];
        var target = source.TrimEnd(x => x == null);
        Assert.Same(source, target);

        source = [null, "1", "2", "3", null, null];
        target = source.TrimEnd(x => x == null);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Length);
        Assert.Null(target[0]);
        Assert.Equal("1", target[1]);
        Assert.Equal("2", target[2]);
        Assert.Equal("3", target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Trim()
    {
        string?[] source = ["1", "2", "3"];
        var target = source.Trim(x => x == null);
        Assert.Same(source, target);

        source = [null, null, "1", "2", "3", null, null];
        target = source.Trim(x => x == null);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Equal("1", target[0]);
        Assert.Equal("2", target[1]);
        Assert.Equal("3", target[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ResizeHead()
    {
        var source = new SItem<int>[] { new(1), new(2), new(3), new(4) };
        var target = source.ResizeHead(3);
        Assert.Equal(3, target.Length);
        Assert.Equal(2, target[0].Value);
        Assert.Equal(3, target[1].Value);
        Assert.Equal(4, target[2].Value);

        target = source.ResizeHead(5, new(99));
        Assert.Equal(5, target.Length);
        Assert.Equal(99, target[0].Value);
        Assert.Equal(1, target[1].Value);
        Assert.Equal(2, target[2].Value);
        Assert.Equal(3, target[3].Value);
        Assert.Equal(4, target[4].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ResizeTail()
    {
        var source = new SItem<int>[] { new(1), new(2), new(3), new(4) };
        var target = source.ResizeTail(3);
        Assert.Equal(3, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);

        target = source.ResizeTail(5, new(99));
        Assert.Equal(5, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);
        Assert.Equal(4, target[3].Value);
        Assert.Equal(99, target[4].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = Array.Empty<SItem<int>>();
        var target = source.Add(new(1));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal(1, target[0].Value);

        source = [new(1), new(2)];
        target = source.Add(new(3));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange_FromArray()
    {
        var source = Array.Empty<SItem<int>>();
        var items = Array.Empty<SItem<int>>();
        var target = source.AddRange(items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Empty(target);

        source = [];
        items = [new(1), new(2)];
        target = source.AddRange(items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        source = [new(1), new(2)];
        items = [];
        target = source.AddRange(items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        items = [new(3), new(4)];
        target = source.AddRange(items);
        Assert.NotSame(items, target);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);
        Assert.Equal(4, target[3].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_FromList()
    {
        var source = Array.Empty<SItem<int>>();
        var items = new List<SItem<int>>();
        var target = source.AddRange(items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Empty(target);

        source = [];
        items = [new(1), new(2)];
        target = source.AddRange(items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        source = [new(1), new(2)];
        items = [];
        target = source.AddRange(items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        items = [new(3), new(4)];
        target = source.AddRange(items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Equal(4, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);
        Assert.Equal(4, target[3].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_FromOthers()
    {
        IEnumerable<SItem<int>> Items(int num, int ini)
        { for (int i = 0; i < num; i++) yield return new(ini + i); }

        var source = Array.Empty<SItem<int>>();
        var target = source.AddRange(Items(0, 0));
        Assert.NotSame(source, target);
        Assert.Empty(target);

        source = [];
        target = source.AddRange(Items(2, 1));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        source = [new(1), new(2)];
        target = source.AddRange(Items(0, 0));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        target = source.AddRange(Items(2, 3));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);
        Assert.Equal(4, target[3].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = Array.Empty<SItem<int>>();
        var target = source.Insert(0, new(1));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal(1, target[0].Value);

        source = [new(1), new(2)];
        target = source.Insert(0, new(3));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Equal(3, target[0].Value);
        Assert.Equal(1, target[1].Value);
        Assert.Equal(2, target[2].Value);

        target = source.Insert(2, new(3));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);

        try { target = source.Insert(-1, new(99)); Assert.Fail(); } catch (IndexOutOfRangeException) { }
        try { target = source.Insert(3, new(99)); Assert.Fail(); } catch (IndexOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_FromArray()
    {
        var source = Array.Empty<SItem<int>>();
        var items = Array.Empty<SItem<int>>();
        var target = source.InsertRange(0, items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Empty(target);

        source = [];
        items = [new(1), new(2)];
        target = source.InsertRange(0, items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        source = [new(1), new(2)];
        items = [];
        target = source.InsertRange(0, items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        source = [new(1), new(2), new(3)];
        items = [new(4), new(5), new(6)];
        target = source.InsertRange(1, items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Equal(6, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(4, target[1].Value);
        Assert.Equal(5, target[2].Value);
        Assert.Equal(6, target[3].Value);
        Assert.Equal(2, target[4].Value);
        Assert.Equal(3, target[5].Value);

        target = source.InsertRange(0, items);
        Assert.Equal(6, target.Length);
        Assert.Equal(4, target[0].Value);
        Assert.Equal(5, target[1].Value);
        Assert.Equal(6, target[2].Value);
        Assert.Equal(1, target[3].Value);
        Assert.Equal(2, target[4].Value);
        Assert.Equal(3, target[5].Value);

        target = source.InsertRange(3, items);
        Assert.Equal(6, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);
        Assert.Equal(4, target[3].Value);
        Assert.Equal(5, target[4].Value);
        Assert.Equal(6, target[5].Value);

        try { target = source.InsertRange(-1, items); Assert.Fail(); } catch (IndexOutOfRangeException) { }
        try { target = source.InsertRange(4, items); Assert.Fail(); } catch (IndexOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_FromList()
    {
        var source = Array.Empty<SItem<int>>();
        var items = new List<SItem<int>>();
        var target = source.InsertRange(0, items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Empty(target);

        source = [];
        items = [new(1), new(2)];
        target = source.InsertRange(0, items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        source = [new(1), new(2)];
        items = [];
        target = source.InsertRange(0, items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        source = [new(1), new(2), new(3)];
        items = [new(4), new(5), new(6)];
        target = source.InsertRange(1, items);
        Assert.NotSame(source, target);
        Assert.NotSame(items, target);
        Assert.Equal(6, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(4, target[1].Value);
        Assert.Equal(5, target[2].Value);
        Assert.Equal(6, target[3].Value);
        Assert.Equal(2, target[4].Value);
        Assert.Equal(3, target[5].Value);

        target = source.InsertRange(0, items);
        Assert.Equal(6, target.Length);
        Assert.Equal(4, target[0].Value);
        Assert.Equal(5, target[1].Value);
        Assert.Equal(6, target[2].Value);
        Assert.Equal(1, target[3].Value);
        Assert.Equal(2, target[4].Value);
        Assert.Equal(3, target[5].Value);

        target = source.InsertRange(3, items);
        Assert.Equal(6, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);
        Assert.Equal(4, target[3].Value);
        Assert.Equal(5, target[4].Value);
        Assert.Equal(6, target[5].Value);

        try { target = source.InsertRange(-1, items); Assert.Fail(); } catch (IndexOutOfRangeException) { }
        try { target = source.InsertRange(4, items); Assert.Fail(); } catch (IndexOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_FromOthers()
    {
        IEnumerable<SItem<int>> Items(int num, int ini)
        { for (int i = 0; i < num; i++) yield return new(ini + i); }

        var source = Array.Empty<SItem<int>>();
        var target = source.InsertRange(0, Items(0, 0));
        Assert.NotSame(source, target);
        Assert.Empty(target);

        source = [];
        target = source.InsertRange(0, Items(2, 1));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        source = [new(1), new(2)];
        target = source.InsertRange(0, Items(0, 0));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        source = [new(1), new(2), new(3)];
        target = source.InsertRange(1, Items(3, 4));
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(4, target[1].Value);
        Assert.Equal(5, target[2].Value);
        Assert.Equal(6, target[3].Value);
        Assert.Equal(2, target[4].Value);
        Assert.Equal(3, target[5].Value);

        target = source.InsertRange(0, Items(3, 4));
        Assert.Equal(6, target.Length);
        Assert.Equal(4, target[0].Value);
        Assert.Equal(5, target[1].Value);
        Assert.Equal(6, target[2].Value);
        Assert.Equal(1, target[3].Value);
        Assert.Equal(2, target[4].Value);
        Assert.Equal(3, target[5].Value);

        target = source.InsertRange(3, Items(3, 4));
        Assert.Equal(6, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);
        Assert.Equal(4, target[3].Value);
        Assert.Equal(5, target[4].Value);
        Assert.Equal(6, target[5].Value);

        try { target = source.InsertRange(-1, Items(3, 4)); Assert.Fail(); } catch (IndexOutOfRangeException) { }
        try { target = source.InsertRange(4, Items(3, 4)); Assert.Fail(); } catch (IndexOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = Array.Empty<SItem<int>>();
        try { source.RemoveAt(0); Assert.Fail(); } catch (IndexOutOfRangeException) { }

        source = [new(1)];
        var target = source.RemoveAt(0);
        Assert.Empty(target);

        source = [new(1), new(2), new(3)];
        target = source.RemoveAt(0);
        Assert.Equal(2, target.Length);
        Assert.Equal(2, target[0].Value);
        Assert.Equal(3, target[1].Value);

        target = source.RemoveAt(1);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(3, target[1].Value);

        target = source.RemoveAt(2);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        try { source.RemoveAt(3); Assert.Fail(); } catch (IndexOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = Array.Empty<SItem<int>>();
        var target = source.RemoveRange(0, 0);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        source = [new(1), new(2), new(3)];
        target = source.RemoveRange(0, 0);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);

        target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(2, target[0].Value);
        Assert.Equal(3, target[1].Value);

        target = source.RemoveRange(2, 1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);

        target = source.RemoveRange(0, 3);
        Assert.Empty(target);

        try { source.RemoveRange(-1, 1); Assert.Fail(); } catch (IndexOutOfRangeException) { }
        try { source.RemoveRange(3, 0); Assert.Fail(); } catch (IndexOutOfRangeException) { }
        try { source.RemoveRange(0, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(0, -1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = Array.Empty<SItem<int>>();
        var target = source.Remove(new SItem<int>(0));
        Assert.NotSame(source, target);
        Assert.Empty(target);

        source = [new(1), new(2), new(3), new(2)];
        target = source.Remove(new SItem<int>(0));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);
        Assert.Equal(2, target[3].Value);

        target = source.Remove(new SItem<int>(2));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(3, target[1].Value);
        Assert.Equal(2, target[2].Value);

        target = source.RemoveLast(new SItem<int>(2));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(2, target[1].Value);
        Assert.Equal(3, target[2].Value);

        target = source.RemoveAll(new SItem<int>(2));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Equal(1, target[0].Value);
        Assert.Equal(3, target[1].Value);
    }
}