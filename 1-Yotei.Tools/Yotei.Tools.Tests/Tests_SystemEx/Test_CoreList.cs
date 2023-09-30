using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;

namespace Yotei.Tools.Tests;

// ========================================================
/// <summary>
/// Base scenario:
/// <br/>- Validation: not null elements.
/// <br/>- Comparer: by name using the case sensitive settings.
/// <br/>- Behavior: throw when duplicates.
/// <br/>- Flatten: yes.
/// </summary>
//[Enforced]
public static class Test_CoreList
{
    public interface IElement { }
    public class NameElement : IElement
    {
        public NameElement(string name) => Name = name;
        public override string ToString() => Name;
        public string Name { get; set; }
    }
    public class ChainElement : CoreList<IElement>, IElement
    {
        public ChainElement(bool sensitive)
        {
            Validator = (item, _) => item.ThrowWhenNull();
            Comparer = (x, y) => x is NameElement nx && y is NameElement ny
                ? string.Compare(nx.Name, ny.Name, !CaseSensitive) == 0
                : ReferenceEquals(x, y);
            Behavior = CoreListBehavior.Throw;
            Flatten = true;
            CaseSensitive = sensitive;
        }
        public ChainElement(bool caseSensitive, IElement item) : this(caseSensitive) => Add(item);
        public ChainElement(bool caseSensitive, IEnumerable<IElement> range) : this(caseSensitive) => AddRange(range);
        public override ChainElement Clone() => (ChainElement)base.Clone();
        protected override ChainElement OnClone() => new(CaseSensitive)
        {
            Validator = Validator,
            Behavior = Behavior,
            Flatten = Flatten,
        };
        public bool CaseSensitive
        {
            get => _CaseSensitive;
            set
            {
                if (_CaseSensitive == value) return;
                _CaseSensitive = value;

                if (Count > 0)
                {
                    var range = ToList();
                    Clear();
                    AddRange(range);
                }
            }
        }
        bool _CaseSensitive = default!;
    }

    // ====================================================

    readonly static NameElement XOne = new("one");
    readonly static NameElement XTwo = new("two");
    readonly static NameElement XThree = new("three");
    readonly static NameElement XFour = new("four");
    readonly static NameElement XFive = new("five");

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new ChainElement(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var items = new ChainElement(false, XOne);
        Assert.Single(items);
        Assert.Same(XOne, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        Assert.Equal(3, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);

        try { items = new(false, new[] { XOne, new NameElement("ONE") }); Assert.Fail(); }
        catch (DuplicateException) { }
    }

#pragma warning disable xUnit2017
    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        List<int> list;

        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        Assert.True(items.Contains(new NameElement("ONE")));
        Assert.Equal(1, items.IndexOf(new NameElement("TWO")));
        Assert.True(items.Contains(x => x is NameElement named && named.Name.Contains('w')));

        list = items.IndexesOf(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);

        items = new ChainElement(true, new[] { XOne, new NameElement("oNe"), XThree });
        Assert.Equal(-1, items.LastIndexOf(new NameElement("ONE")));
    }
#pragma warning restore

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var target = source.Clone();
        Assert.Equal(source.CaseSensitive, target.CaseSensitive);
        Assert.Same(XOne, target[0]);
        Assert.Same(XTwo, target[1]);
        Assert.Same(XThree, target[2]);

        source.CaseSensitive = !source.CaseSensitive;
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.CaseSensitive, target.CaseSensitive);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Settings()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Validator = (item, _) => item;
        items.Add(null!);
        Assert.Equal(4, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
        Assert.Null(items[3]);

        items = new(true, new[] { XOne, new NameElement("ONE") });
        items.Behavior = CoreListBehavior.Ignore;
        items.Add(XOne);
        Assert.Equal(2, items.Count);

        items = new(true, new[] { XOne, new NameElement("ONE") });
        try { items.CaseSensitive = false; Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var list = items.GetRange(1, 0);
        Assert.Empty(list);

        list = items.GetRange(1, 2);
        Assert.Equal(2, list.Count);
        Assert.Same(XTwo, list[0]);
        Assert.Same(XThree, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceItems()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items[0] = XFour;
        Assert.Equal(3, items.Count);
        Assert.Same(XFour, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);

        try { items[0] = new NameElement("THREE"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceItems_IgnoreDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Ignore;
        items[0] = new NameElement("THREE");
        Assert.Equal(3, items.Count);
        Assert.Same(XOne, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceItems_AddDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Add;
        items[0] = new NameElement("THREE");
        Assert.Equal(3, items.Count);
        Assert.Equal("THREE", (((NameElement)items[0]).Name));
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceItemsMany()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var other = new ChainElement(false, new[] { XFour, XFive });
        items[0] = other;
        Assert.Equal(4, items.Count);
        Assert.Same(XFour, items[0]);
        Assert.Same(XFive, items[1]);
        Assert.Same(XTwo, items[2]);
        Assert.Same(XThree, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceItemsMany_IgnoreDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Ignore;

        var other = new ChainElement(false, new[] { XTwo, new NameElement("THREE") });
        items[0] = other;
        Assert.Equal(3, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceItemsMany_AddDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Add;

        var other = new ChainElement(false, new[] { XTwo, new NameElement("THREE") });
        items[0] = other;
        Assert.Equal(4, items.Count);
        Assert.Same(XTwo, items[0]);
        Assert.Equal("THREE", ((NameElement)items[1]).Name);
        Assert.Same(XTwo, items[2]);
        Assert.Same(XThree, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var done = items.Add(XFour);
        Assert.Equal(4, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
        Assert.Same(XFour, items[3]);

        try { items.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Add(new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_IgnoreDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Ignore;

        var done = items.Add(new NameElement("TWO"));
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_AddDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Add;

        var done = items.Add(new NameElement("TWO"));
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
        Assert.Same("TWO", ((NameElement)items[3]).Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddMany()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var other = new ChainElement(false, new[] { XFour, XFive });

        var done = items.Add(other);
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
        Assert.Same(XFour, items[3]);
        Assert.Same(XFive, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddMany_IgnoreDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Ignore;

        var other = new ChainElement(false, new[] { XOne, XFour });
        var done = items.Add(other);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
        Assert.Same(XFour, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddMany_AddDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Add;

        var other = new ChainElement(false, new[] { XOne, XFour });
        var done = items.Add(other);
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
        Assert.Same(XOne, items[3]);
        Assert.Same(XFour, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var done = items.AddRange(new[] { XFour, XFive });
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
        Assert.Same(XFour, items[3]);
        Assert.Same(XFive, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_IgnoreDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Ignore;

        var done = items.AddRange(new[] { XOne, XFour });
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
        Assert.Same(XFour, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_AddDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Add;

        var done = items.AddRange(new[] { XOne, XFour });
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
        Assert.Same(XOne, items[3]);
        Assert.Same(XFour, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var done = items.Insert(0, XFour);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(XFour, items[0]);
        Assert.Same(XOne, items[1]);
        Assert.Same(XTwo, items[2]);
        Assert.Same(XThree, items[3]);

        try { items.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_IgnoreDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Ignore;

        var done = items.Insert(0, XThree);
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_AddDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Add;

        var done = items.Insert(3, XThree);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
        Assert.Same(XThree, items[2]);
        Assert.Same(XThree, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertMany()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var other = new ChainElement(false, new[] { XFour, XFive });

        var done = items.Insert(0, other);
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(XFour, items[0]);
        Assert.Same(XFive, items[1]);
        Assert.Same(XOne, items[2]);
        Assert.Same(XTwo, items[3]);
        Assert.Same(XThree, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertMany_IgnoreDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Ignore;

        var other = new ChainElement(false, new[] { XOne, XFour });
        var done = items.Insert(0, other);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(XFour, items[0]);
        Assert.Same(XOne, items[1]);
        Assert.Same(XTwo, items[2]);
        Assert.Same(XThree, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertMany_AddDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Add;

        var other = new ChainElement(false, new[] { XOne, XFour });
        var done = items.Insert(0, other);
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XFour, items[1]);
        Assert.Same(XOne, items[2]);
        Assert.Same(XTwo, items[3]);
        Assert.Same(XThree, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });

        var done = items.InsertRange(0, new[] { XFour, XFive });
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(XFour, items[0]);
        Assert.Same(XFive, items[1]);
        Assert.Same(XOne, items[2]);
        Assert.Same(XTwo, items[3]);
        Assert.Same(XThree, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_IgnoreDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Ignore;

        var done = items.InsertRange(0, new[] { XOne, XFour });
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(XFour, items[0]);
        Assert.Same(XOne, items[1]);
        Assert.Same(XTwo, items[2]);
        Assert.Same(XThree, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_AddDuplicates()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        items.Behavior = CoreListBehavior.Add;

        var done = items.InsertRange(0, new[] { XOne, XFour });
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XFour, items[1]);
        Assert.Same(XOne, items[2]);
        Assert.Same(XTwo, items[3]);
        Assert.Same(XThree, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var done = items.RemoveAt(1);
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XThree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveItem()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var done = items.Remove(new NameElement("TWO"));
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XThree, items[1]);

        items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        done = items.Remove(XFour);
        Assert.Equal(0, done);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveItemMany()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var other = new ChainElement(false, new[] { XTwo, XThree });
        var done = items.Remove(other);
        Assert.Equal(2, done);
        Assert.Single(items);
        Assert.Same(XOne, items[0]);

        items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        other = new ChainElement(false, new[] { XFour, XFive });
        done = items.Remove(other);
        Assert.Equal(0, done);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast()
    {
        var items = new ChainElement(false) { Behavior = CoreListBehavior.Add };
        items.AddRange(new[] { XOne, new NameElement("ONE"), XThree });

        var done = items.RemoveLast(new NameElement("oNe"));
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XThree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLastMany()
    {
        var items = new ChainElement(false) { Behavior = CoreListBehavior.Add };
        items.AddRange(new[] { XOne, XTwo, XOne, XFour });

        var other = new ChainElement(false) { XOne, XTwo };
        var done = items.RemoveLast(other);
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XFour, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll()
    {
        var items = new ChainElement(false) { Behavior = CoreListBehavior.Add };
        items.AddRange(new[] { XOne, new NameElement("ONE"), XThree });

        var done = items.RemoveAll(new NameElement("oNe"));
        Assert.Equal(2, done);
        Assert.Single(items);
        Assert.Same(XThree, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAllMany()
    {
        var items = new ChainElement(false) { Behavior = CoreListBehavior.Add };
        items.AddRange(new[] { XOne, XOne, XTwo, XThree });

        var done = items.RemoveAll(new NameElement("oNe"));
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(XTwo, items[0]);
        Assert.Same(XThree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var done = items.RemoveRange(1, 2);
        Assert.Equal(2, done);
        Assert.Single(items);
        Assert.Same(XOne, items[0]);

        items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        done = items.RemoveRange(1, 0);
        Assert.Equal(0, done);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemovePredicate()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var done = items.Remove(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Same(XTwo, items[0]);
        Assert.Same(XThree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLastPredicate()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var done = items.RemoveLast(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Same(XOne, items[0]);
        Assert.Same(XTwo, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAllPredicate()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var done = items.RemoveAll(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(2, done);
        Assert.Single(items);
        Assert.Same(XTwo, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var items = new ChainElement(false, new[] { XOne, XTwo, XThree });
        var done = items.Clear();
        Assert.Equal(3, done);
        Assert.Empty(items);

        items = new ChainElement(false);
        done = items.Clear();
        Assert.Equal(0, done);
    }
}