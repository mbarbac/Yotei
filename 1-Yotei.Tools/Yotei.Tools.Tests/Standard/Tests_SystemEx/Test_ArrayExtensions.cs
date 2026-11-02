#if NETSTANDARD2_0
namespace Yotei.Tools.Std20.Tests;
#pragma warning disable CS0436
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static class Test_ArrayExtensions
{
    public class Item<T>(T value) : ICloneable
    {
        public T Value { get; set; } = value;
        public override string ToString() => Value?.ToString() ?? "";
        public Item<T> Clone() => new(Value);
        object ICloneable.Clone() => Clone();
    }

    public class MyComparer<T> : IEqualityComparer<Item<T>>
    {
        readonly Func<T, T, bool> _Equals;
        readonly Func<T, int> _Hashcode;

        [SuppressMessage("", "IDE0290")]
        public MyComparer(Func<T, T, bool> equals = null!, Func<T, int> hashcode = null!)
        {
            _Equals = equals ?? ((x, y) => x.EqualsEx(y));
            _Hashcode = hashcode ?? ((x) => x?.GetHashCode() ?? 0);
        }
        public bool Equals([AllowNull] Item<T> x, [AllowNull] Item<T> y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            return _Equals(x.Value, y.Value);
        }
        public int GetHashCode([DisallowNull] Item<T> obj) => _Hashcode(obj.Value);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Enumerators()
    {
        int[] nums = [97, 98, 99];

        var stditer = nums.GetEnumerator();
        var type = stditer.GetType();
        Assert.False(type.IsGenericType);

        var xtriter = nums.GetTypedEnumerator();
        type = xtriter.GetType();
        Assert.True(type.IsGenericType);
        type = type.GetGenericArguments()[0];
        Assert.Equal(typeof(int), type);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Clone()
    {
        Item<int>[] source = [];
        Item<int>[] target = (Item<int>[])source.Clone(); Assert.NotSame(source, target);
        Assert.Empty(target);

        source = [new(1), new(2)];
        target = (Item<int>[])source.Clone(); Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);

        target = source.Clone(false); Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);

        target = source.Clone(true); Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.NotSame(source[0], target[0]); Assert.Equal(source[0].Value, target[0].Value);
        Assert.NotSame(source[1], target[1]); Assert.Equal(source[1].Value, target[1].Value);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Inherit_SequenceEqual()
    {
        Item<int>[] nums;
        bool same;

        nums = []; same = nums.SequenceEqual([]); Assert.True(same);
        
        nums = [new(1), new(2)];
        same = nums.SequenceEqual([]); Assert.False(same);
        same = nums.SequenceEqual([new(1), new(2)]); Assert.False(same);
        same = nums.SequenceEqual([new(1), new(2)], new MyComparer<int>()); Assert.True(same);
    }

    [Fact]
    public static void Inherit_SequenceCompare()
    {
        Item<int>[] nums;
        int same;

        nums = []; same = nums.SequenceCompareTo([]); Assert.Equal(0, same);
        nums = [new(1), new(2)]; same = nums.SequenceCompareTo([new(1), new(2)]); Assert.Equal(0, same);
        nums = [new(1), new(2)]; same = nums.SequenceCompareTo([new(1), new(2), new(3)]); Assert.Equal(-1, same);
        nums = [new(1), new(2), new(3)]; same = nums.SequenceCompareTo([new(1), new(2)]); Assert.Equal(+1, same);
        nums = [new(1), new(1)]; same = nums.SequenceCompareTo([new(1), new(2)]); Assert.Equal(-1, same);
        nums = [new(1), new(2)]; same = nums.SequenceCompareTo([new(1), new(1)]); Assert.Equal(+1, same);

        nums = []; same = nums.SequenceCompareTo([], new MyComparer<int>()); Assert.Equal(0, same);
        //nums = [new(1), new(2)]; same = nums.SequenceCompareTo([new(1), new(2)], new MyComparer<int>()); Assert.Equal(0, same);
        //nums = [new(1), new(2)]; same = nums.SequenceCompareTo([new(1), new(2), new(3)], new MyComparer<int>()); Assert.Equal(-1, same);
        //nums = [new(1), new(2), new(3)]; same = nums.SequenceCompareTo([new(1), new(2)], new MyComparer<int>()); Assert.Equal(+1, same);
        //nums = [new(1), new(1)]; same = nums.SequenceCompareTo([new(1), new(2)], new MyComparer<int>()); Assert.Equal(-1, same);
        //nums = [new(1), new(2)]; same = nums.SequenceCompareTo([new(1), new(1)], new MyComparer<int>()); Assert.Equal(+1, same);
    }
}
/*
        
        

        
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Contains()
    {
        Item<int> nums;
        Predicate<int> Predicate(int value) => x => x == value;

        nums = []; Assert.False(nums.Contains(9));
        nums = [9]; Assert.True(nums.Contains(9));
        nums = [0, 9]; Assert.True(nums.Contains(9));
        nums = [9, 0]; Assert.True(nums.Contains(9));

        nums = []; Assert.False(nums.Contains(9, new MyComparer<int>()));
        nums = [9]; Assert.True(nums.Contains(9, new MyComparer<int>()));
        nums = [0, 9]; Assert.True(nums.Contains(9, new MyComparer<int>()));
        nums = [9, 0]; Assert.True(nums.Contains(9, new MyComparer<int>()));

        nums = []; Assert.False(nums.Contains(Predicate(9)));
        nums = [9]; Assert.True(nums.Contains(Predicate(9)));
        nums = [0, 9]; Assert.True(nums.Contains(Predicate(9)));
        nums = [9, 0]; Assert.True(nums.Contains(Predicate(9)));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOf()
    {
        List<int> indexes;
        int index;
        Item<int> nums;
        Predicate<int> Predicate(int value) => x => x == value;

        nums = [1, 2, 3, 1, 2];

        index = nums.IndexOf(99); Assert.Equal(-1, index);
        index = nums.IndexOf(2); Assert.Equal(1, index);
        index = nums.IndexOf(2, new MyComparer<int>()); Assert.Equal(1, index);
        index = nums.IndexOf(Predicate(2)); Assert.Equal(1, index);

        index = nums.LastIndexOf(99); Assert.Equal(-1, index);
        index = nums.LastIndexOf(1); Assert.Equal(3, index);
        index = nums.LastIndexOf(1, new MyComparer<int>()); Assert.Equal(3, index);
        index = nums.LastIndexOf(Predicate(1)); Assert.Equal(3, index);

        indexes = nums.IndexesOf(99); Assert.Empty(indexes);
        indexes = nums.IndexesOf(1); Assert.True(indexes is [0, 3]);
        indexes = nums.IndexesOf(1, new MyComparer<int>()); Assert.True(indexes is [0, 3]);
        indexes = nums.IndexesOf(Predicate(1)); Assert.True(indexes is [0, 3]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_FirstCropIndex()
    {
        int index;
        Item<int> nums;
        Predicate<int> Predicate(int value) => x => x == value;

        nums = []; index = nums.FirstCropIndex(9); Assert.Equal(-1, index);
        nums = [9]; index = nums.FirstCropIndex(9); Assert.Equal(-1, index);
        nums = [9, 9, 9]; index = nums.FirstCropIndex(9); Assert.Equal(-1, index);
        nums = [0, 1]; index = nums.FirstCropIndex(9); Assert.Equal(0, index);
        nums = [9, 0, 1]; index = nums.FirstCropIndex(9); Assert.Equal(1, index);
        nums = [9, 9, 0, 1]; index = nums.FirstCropIndex(9); Assert.Equal(2, index);
        nums = [0, 9, 0, 1]; index = nums.FirstCropIndex(9); Assert.Equal(0, index);

        nums = []; index = nums.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        nums = [9]; index = nums.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        nums = [9, 9, 9]; index = nums.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        nums = [0, 1]; index = nums.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(0, index);
        nums = [9, 0, 1]; index = nums.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        nums = [9, 9, 0, 1]; index = nums.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(2, index);
        nums = [0, 9, 0, 1]; index = nums.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(0, index);

        nums = []; index = nums.FirstCropIndex(Predicate(9)); Assert.Equal(-1, index);
        nums = [9]; index = nums.FirstCropIndex(Predicate(9)); Assert.Equal(-1, index);
        nums = [9, 9, 9]; index = nums.FirstCropIndex(Predicate(9)); Assert.Equal(-1, index);
        nums = [0, 1]; index = nums.FirstCropIndex(Predicate(9)); Assert.Equal(0, index);
        nums = [9, 0, 1]; index = nums.FirstCropIndex(Predicate(9)); Assert.Equal(1, index);
        nums = [9, 9, 0, 1]; index = nums.FirstCropIndex(Predicate(9)); Assert.Equal(2, index);
        nums = [0, 9, 0, 1]; index = nums.FirstCropIndex(Predicate(9)); Assert.Equal(0, index);
    }

    [Fact]
    public static void Test_LastCropIndex()
    {
        int index;
        Item<int> nums;
        Predicate<int> Predicate(int value) => x => x == value;

        nums = []; index = nums.LastCropIndex(9); Assert.Equal(-1, index);
        nums = [9]; index = nums.LastCropIndex(9); Assert.Equal(-1, index);
        nums = [9, 9, 9]; index = nums.LastCropIndex(9); Assert.Equal(-1, index);
        nums = [0, 1]; index = nums.LastCropIndex(9); Assert.Equal(1, index);
        nums = [0, 1, 9]; index = nums.LastCropIndex(9); Assert.Equal(1, index);
        nums = [0, 1, 9, 9]; index = nums.LastCropIndex(9); Assert.Equal(1, index);
        nums = [0, 1, 9, 0]; index = nums.LastCropIndex(9); Assert.Equal(3, index);

        nums = []; index = nums.LastCropIndex(0, new MyComparer<int>()); Assert.Equal(-1, index);
        nums = [9]; index = nums.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        nums = [9, 9, 9]; index = nums.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        nums = [0, 1]; index = nums.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        nums = [0, 1, 9]; index = nums.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        nums = [0, 1, 9, 9]; index = nums.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        nums = [0, 1, 9, 0]; index = nums.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(3, index);

        nums = []; index = nums.LastCropIndex(Predicate(9)); Assert.Equal(-1, index);
        nums = [9]; index = nums.LastCropIndex(Predicate(9)); Assert.Equal(-1, index);
        nums = [9, 9, 9]; index = nums.LastCropIndex(Predicate(9)); Assert.Equal(-1, index);
        nums = [0, 1]; index = nums.LastCropIndex(Predicate(9)); Assert.Equal(1, index);
        nums = [0, 1, 9]; index = nums.LastCropIndex(Predicate(9)); Assert.Equal(1, index);
        nums = [0, 1, 9, 9]; index = nums.LastCropIndex(Predicate(9)); Assert.Equal(1, index);
        nums = [0, 1, 9, 0]; index = nums.LastCropIndex(Predicate(9)); Assert.Equal(3, index);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_TrimStart()
    {
        ReadOnlySpan<int> source;
        ReadOnlySpan<int> target;
        Predicate<int> Predicate(int value) => x => x == value;

        source = []; target = source.TrimStart(); Assert.True(target.IsEmpty);
        source = [9]; target = source.TrimStart(9); Assert.True(target.IsEmpty);
        source = [9, 0]; target = source.TrimStart(9); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimStart(9); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimStart(9); Assert.True(target is [0, 9, 9]);

        source = [9]; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target.IsEmpty);
        source = [9, 0]; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target is [0, 9, 9]);

        source = [9]; target = source.TrimStart(Predicate(9)); Assert.True(target.IsEmpty);
        source = [9, 0]; target = source.TrimStart(Predicate(9)); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimStart(Predicate(9)); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimStart(Predicate(9)); Assert.True(target is [0, 9, 9]);
    }

    [Fact]
    public static void Test_TrimEnd()
    {
        ReadOnlySpan<int> source;
        ReadOnlySpan<int> target;
        Predicate<int> Predicate(int value) => x => x == value;

        source = []; target = source.TrimEnd(); Assert.True(target.IsEmpty);
        source = [9]; target = source.TrimEnd(9); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.TrimEnd(9); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimEnd(9); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimEnd(9); Assert.True(target is [9, 9, 0]);

        source = [9]; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target is [9, 9, 0]);

        source = [9]; target = source.TrimEnd(Predicate(9)); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.TrimEnd(Predicate(9)); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimEnd(Predicate(9)); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimEnd(Predicate(9)); Assert.True(target is [9, 9, 0]);
    }

    [Fact]
    public static void Test_Trim()
    {
        ReadOnlySpan<int> source;
        ReadOnlySpan<int> target;
        Predicate<int> Predicate(int value) => x => x == value;

        source = []; target = source.Trim(); Assert.True(target.IsEmpty);
        source = [9]; target = source.Trim(9); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.Trim(9); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.Trim(9); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.Trim(9); Assert.True(target is [0]);
        source = [9, 0, 0, 9]; target = source.Trim(9); Assert.True(target is [0, 0]);
        source = [0, 9, 9, 0]; target = source.Trim(9); Assert.True(target is [0, 9, 9, 0]);

        source = [9]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [9, 0, 0, 9]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0, 0]);
        source = [0, 9, 9, 0]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0, 9, 9, 0]);

        source = [9]; target = source.Trim(Predicate(9)); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.Trim(Predicate(9)); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.Trim(Predicate(9)); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.Trim(Predicate(9)); Assert.True(target is [0]);
        source = [9, 0, 0, 9]; target = source.Trim(Predicate(9)); Assert.True(target is [0, 0]);
        source = [0, 9, 9, 0]; target = source.Trim(Predicate(9)); Assert.True(target is [0, 9, 9, 0]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_StartsWith()
    {
        Item<int> nums;
        Predicate<int> Predicate(int value) => x => x == value;

        nums = []; Assert.False(nums.StartsWith(9));
        nums = [9]; Assert.True(nums.StartsWith(9));
        nums = [0, 9]; Assert.False(nums.StartsWith(9));
        nums = [9, 0]; Assert.True(nums.StartsWith(9));

        nums = []; Assert.False(nums.StartsWith(9, new MyComparer<int>()));
        nums = [9]; Assert.True(nums.StartsWith(9, new MyComparer<int>()));
        nums = [0, 9]; Assert.False(nums.StartsWith(9, new MyComparer<int>()));
        nums = [9, 0]; Assert.True(nums.StartsWith(9, new MyComparer<int>()));

        nums = []; Assert.False(nums.StartsWith(Predicate(9)));
        nums = [9]; Assert.True(nums.StartsWith(Predicate(9)));
        nums = [0, 9]; Assert.False(nums.StartsWith(Predicate(9)));
        nums = [9, 0]; Assert.True(nums.StartsWith(Predicate(9)));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_EndsWith()
    {
        Item<int> nums;
        Predicate<int> Predicate(int value) => x => x == value;

        nums = []; Assert.False(nums.EndsWith(9));
        nums = [9]; Assert.True(nums.EndsWith(9));
        nums = [0, 9]; Assert.True(nums.EndsWith(9));
        nums = [9, 0]; Assert.False(nums.EndsWith(9));

        nums = []; Assert.False(nums.EndsWith(9, new MyComparer<int>()));
        nums = [9]; Assert.True(nums.EndsWith(9, new MyComparer<int>()));
        nums = [0, 9]; Assert.True(nums.EndsWith(9, new MyComparer<int>()));
        nums = [9, 0]; Assert.False(nums.EndsWith(9, new MyComparer<int>()));

        nums = []; Assert.False(nums.EndsWith(Predicate(9)));
        nums = [9]; Assert.True(nums.EndsWith(Predicate(9)));
        nums = [0, 9]; Assert.True(nums.EndsWith(Predicate(9)));
        nums = [9, 0]; Assert.False(nums.EndsWith(Predicate(9)));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_ContainsAny()
    {
        ReadOnlySpan<int> nums = [1, 2, 3, 1, 2];

        Assert.False(nums.ContainsAny([98, 99]));
        Assert.False(nums.ContainsAny([98, 99], new MyComparer<int>()));
        Assert.True(nums.ContainsAny([98, 99, 1, 2]));
        Assert.True(nums.ContainsAny([98, 99, 1, 2], new MyComparer<int>()));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOfAny()
    {
        List<int> indexes;
        int index;
        Item<int> nums;

        nums = [1, 2, 3, 1, 2];

        index = nums.IndexOfAny([98, 99]); Assert.Equal(-1, index);
        index = nums.IndexOfAny([9, 3]); Assert.Equal(2, index);
        index = nums.IndexOfAny([9, 3], new MyComparer<int>()); Assert.Equal(2, index);

        index = nums.LastIndexOfAny([98, 99]); Assert.Equal(-1, index);
        index = nums.LastIndexOfAny([9, 3]); Assert.Equal(2, index);
        index = nums.LastIndexOfAny([9, 1], new MyComparer<int>()); Assert.Equal(3, index);

        indexes = nums.IndexesOfAny([98, 99]); Assert.Empty(indexes);
        indexes = nums.IndexesOfAny([9, 3, 1]);
        Assert.Equal(3, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(2, indexes[1]);
        Assert.Equal(3, indexes[2]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_StartsWithAny()
    {
        bool found;
        Item<int> nums;

        nums = []; found = nums.StartsWithAny([0, 99]); Assert.False(found);
        nums = [new(1), new(2)]; found = nums.StartsWithAny([0, 3]); Assert.False(found);
        nums = [new(1), new(2)]; found = nums.StartsWithAny([0, 1]); Assert.True(found);
        nums = [new(1), new(2)]; found = nums.StartsWithAny([0, 1], new MyComparer<int>()); Assert.True(found);
    }

    [Fact]
    public static void Test_EndsWithAny()
    {
        bool found;
        Item<int> nums;

        nums = []; found = nums.EndsWithAny([0, 99]); Assert.False(found);
        nums = [new(1), new(2)]; found = nums.EndsWithAny([0, 3]); Assert.False(found);
        nums = [2, 1]; found = nums.EndsWithAny([0, 1]); Assert.True(found);
        nums = [2, 1]; found = nums.EndsWithAny([0, 1], new MyComparer<int>()); Assert.True(found);
    }

    // ----------------------------------------------------

    [Fact]
    [SuppressMessage("", "IDE0018")]
    public static void Test_TryFind()
    {
        int value;
        List<int> list;
        Item<int> nums;

        nums = []; Assert.False(nums.TryFind(x => x != 0, out value)); Assert.Equal(0, value);
        nums = [1, 3, 2]; Assert.True(nums.TryFind(x => x > 1, out value)); Assert.Equal(3, value);

        nums = []; Assert.False(nums.TryFindLast(x => x != 0, out value)); Assert.Equal(0, value);
        nums = [3, 2, 1]; Assert.True(nums.TryFindLast(x => x > 1, out value)); Assert.Equal(2, value);

        nums = []; Assert.False(nums.TryFindAll(x => x != 0, out list)); Assert.Empty(list);
        nums = [1, 3, 2, 1]; Assert.True(nums.TryFindAll(x => x > 1, out list));
        Assert.Equal(2, list.Count);
        Assert.Equal(3, list[0]);
        Assert.Equal(2, list[1]);
    }
 */