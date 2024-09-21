using System.Numerics;

namespace Yotei.Tools.FrozenGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Frozen_KT
{
    public interface IElement<R> { }

    public class Element<R>(string name) : IElement<R>
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }
    static readonly Element<int> xone = new("one");
    static readonly Element<int> xtwo = new("two");
    static readonly Element<int> xthree = new("three");
    static readonly Element<int> xfour = new("four");
    static readonly Element<int> xfive = new("five");

    // ----------------------------------------------------

    [IFrozenList(typeof(string), typeof(IElement<int>))]
    public partial interface IChain { }

    [Cloneable]
    public partial class Builder<R> : CoreList<string, IElement<int>>
    {
        public Builder(bool sensitive) => Sensitive = sensitive;
        public Builder(bool sensitive, int capacity) : this(sensitive) => Capacity = capacity;
        public Builder(bool sensitive, IEnumerable<IElement<int>> range) : this(sensitive) => AddRange(range);
        protected Builder(Builder<R> source) : this(source.Sensitive) => AddRange(source);

        public bool Sensitive { get; }
        public override IElement<int> ValidateItem(IElement<int> item) => item.ThrowWhenNull();
        public override string GetKey(IElement<int> item) => item is Element<R> named
            ? named.Name
            : throw new ArgumentException("Element is not a named instance.").WithData(item);
        public override string ValidateKey(string key) => key.NotNullNotEmpty();
        public override bool CompareKeys(string x, string y) => string.Compare(x, y, !Sensitive) == 0;
        public override bool CanInclude(IElement<int> item, IElement<int> source)
            => ReferenceEquals(item, source)
            ? true
            : throw new DuplicateException("Duplicated element.").WithData(item);
    }

    [FrozenList(typeof(string), typeof(IElement<int>))]
    public partial class Chain<R> : IChain
    {
        protected override Builder<R> Items => _Items ??= new Builder<R>(Sensitive);
        Builder<R>? _Items = null;

        public Chain(bool sensitive) => Sensitive = sensitive;
        public Chain(bool sensitive, int capacity) : this(sensitive) => Items.Capacity = capacity;
        public Chain(bool sensitive, IEnumerable<IElement<int>> range) : this(sensitive) => Items.AddRange(range);
        protected Chain(Chain<R> source) : this(source.Sensitive) => Items.AddRange(source);

        public bool Sensitive { get; }
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test()
    {
    }
}