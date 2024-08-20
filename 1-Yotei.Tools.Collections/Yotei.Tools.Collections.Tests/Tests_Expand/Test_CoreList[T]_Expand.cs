namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static partial class Test_CoreList_T_Expand
{
    public interface IElement { }

    public class Named(string name) : IElement
    {
        public string Name { get; set; } = name;
        public override string ToString() => Name ?? string.Empty;
    }
    static readonly Named xone = new("one");
    static readonly Named xtwo = new("two");
    static readonly Named xthree = new("three");
    static readonly Named xfour = new("four");

    [Cloneable]
    public partial class Chain : CoreList<IElement>, IElement
    {
        public Chain(bool sensitive)
        {
            Sensitive = sensitive;
            ValidateItem = (x) =>
            {
                x.ThrowWhenNull();
                if (x is Named named) named.Name.NotNullNotEmpty();
                return x;
            };
            CompareItems = (x, y) =>
            {
                return x is Named xnamed && y is Named ynamed
                    ? string.Compare(xnamed.Name, ynamed.Name, !Sensitive) == 0
                    : ReferenceEquals(x, y);
            };
            CanInclude = (x, y) =>
            {
                return ReferenceEquals(x, y)
                    ? true
                    : throw new DuplicateException("Duplicated element.").WithData(x);
            };
            GetDuplicates = IndexesOf;
            ExpandItems = true;
        }
        public Chain(bool sensitive, IElement item) : this(sensitive) => Add(item);
        public Chain(bool sensitive, IEnumerable<IElement> range) : this(sensitive) => AddRange(range);
        protected Chain(Chain source) : this(source.Sensitive) => AddRange(source);

        public bool Sensitive
        {
            get => _Sensitive;
            set
            {
                if (_Sensitive == value) return;
                if (_Sensitive = value) Reload();
            }
        }
        bool _Sensitive;
    }

    // ====================================================

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new Chain(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var items = new Chain(false, xone);
        Assert.Single(items);
        Assert.Same(xone, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var items = new Chain(false, []);
        Assert.Empty(items);

        items = new Chain(false, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Extended()
    {
        var temps = new Chain(false, [xone, xtwo, xthree]);
        var items = new Chain(false, (IElement)temps);

        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Extended()
    {
        var items = new Chain(false) { ExpandItems = false };
        Assert.Empty(items);

        items.Add(xone);
        items.Add(new Chain(false, [xtwo, xthree]));
        Assert.Equal(2, items.Count);

        items.ExpandItems = true;
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var items = new Chain(false, [xone, xtwo]);
        var others = new Chain(false, [xthree, xfour, xone]);

        items.Add(others);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xone, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var items = new Chain(false, [xone, xtwo]);
        var others = new Chain(false, [xthree, xfour, xone]);

        items.Insert(0, others);
        Assert.Equal(5, items.Count);
        Assert.Same(xthree, items[0]);
        Assert.Same(xfour, items[1]);
        Assert.Same(xone, items[2]);
        Assert.Same(xone, items[3]);
        Assert.Same(xtwo, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var others = new Chain(false, [xfour, xone]);

        items.Replace(1, others);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xfour, items[1]);
        Assert.Same(xone, items[2]);
        Assert.Same(xthree, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty()
    {
        var items = new Chain(false, [xone, xtwo, xthree]);
        var others = new Chain(false);

        items.Replace(1, others);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xthree, items[1]);
    }
}