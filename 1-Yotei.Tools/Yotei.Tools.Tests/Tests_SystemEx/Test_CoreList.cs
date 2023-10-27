using System.Security.Cryptography.X509Certificates;
using TItem = Yotei.Tools.Tests.Test_CoreList.IElement;
using TKey = string;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_CoreList
{
    /// <summary>
    /// The generic element member of the test collection.
    /// </summary>
    public interface IElement { }

    /// <summary>
    /// The named element identified by its name.
    /// </summary>
    /// <param name="name"></param>
    public class NameElement(string name) : TItem
    {
        public override string ToString() => Name;
        public string Name { get; set; } = name;
    }

    /// <summary>
    /// The nested element that is itself a collection.
    /// </summary>
    public class ChainElement : CoreList<string, TItem>, TItem
    {
        public ChainElement(bool sensitive) => CaseSensitive = sensitive;
        public ChainElement(bool sensitive, TItem item) : this(sensitive) => Add(item);
        public ChainElement(bool sensitive, IEnumerable<TItem> range) : this(sensitive) => AddRange(range);
        protected ChainElement(ChainElement source)
        {
            source.ThrowWhenNull();

            CaseSensitive = source.CaseSensitive;
            AddRange(source);
        }
        public override ChainElement Clone() => new(this);

        public bool CaseSensitive
        {
            get => _CaseSensitive;
            set
            {
                if (_CaseSensitive == value) return;

                _CaseSensitive = value; if (Count > 0)
                {
                    var range = ToArray();
                    Clear();
                    AddRange(range);
                }
            }
        }
        bool _CaseSensitive;

        public override TItem ValidateItem(TItem item) => item.ThrowWhenNull();
        public override TKey GetKey(TItem item)
        {
            return item is NameElement named
                ? named.Name
                : throw new UnExpectedException("Item is not a named element.").WithData(item);
        }
        public override TKey ValidateKey(TKey key) => key.NotNullNotEmpty();
        public override bool CompareKeys(TKey inner, TKey other)
        {
            return string.Compare(inner, other, !CaseSensitive) == 0;
        }
        public override bool AcceptDuplicated(TItem item)
        {
            // We accept duplicates only if they are strictly the same instance...
            if (this.Any(x => ReferenceEquals(x, item))) return true;
            throw new DuplicateException("Duplicated element.").WithData(item);
        }
        public override bool ExpandNexted(TItem _) => true;
    }

    // ====================================================

    static readonly NameElement xone = new("one");
    static readonly NameElement xtwo = new("two");
    static readonly NameElement xthree = new("three");
    static readonly NameElement xfour = new("four");
    static readonly NameElement xfive = new("five");
    static readonly NameElement xsix = new("six");

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
        var items = new ChainElement(false, xone);
        Assert.Single(items);
        Assert.Same(xone, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new ChainElement(false, (IEnumerable<IElement>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new ChainElement(false, [xone, new NameElement("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_Same_Duplicated()
    {
        var items = new ChainElement(false, [xone, xtwo, xone]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_CaseSensitive()
    {
        var items = new ChainElement(true, [xone, xtwo, new NameElement("ONE")]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Equal("ONE", ((NameElement)items[2]).Name);

        items = new ChainElement(true, [xone, xtwo, xone]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        try { _ = new ChainElement(true, [xone, xtwo, new NameElement("one")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new ChainElement(false, [xone, xtwo, xone]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_Sensitive()
    {
        var source = new ChainElement(true, [xone, xtwo, new NameElement("ONE")]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Behaviors()
    {
        var items = new ChainElement(false, [xone, xtwo, xone]);
        items.CaseSensitive = true;
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        items = new ChainElement(true, [xone, xtwo, new NameElement("ONE")]);
        try { items.CaseSensitive = false; Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Key()
    {
        var items = new ChainElement(false, [xone, xtwo, xone]);
        Assert.Equal(0, items.IndexOf("ONE"));
        Assert.Equal(1, items.IndexOf("TWO"));
        Assert.Equal(-1, items.IndexOf("x"));

        Assert.Equal(2, items.LastIndexOf("ONE"));

        var list = items.IndexesOf("ONE");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var items = new ChainElement(false, [xone, xtwo, xone]);

        var list = items.IndexesOf(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);

        list = items.IndexesOf(x => x is NameElement named && named.Name == "x");
        Assert.Empty(list);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });

        var list = items.GetRange(1, 0);
        Assert.Empty(list);

        list = items.GetRange(1, 2);
        Assert.Equal(2, list.Count);
        Assert.Same(xtwo, list[0]);
        Assert.Same(xthree, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        
        var done = items.Replace(0, xone);
        Assert.Equal(0, done);

        done = items.Replace(0, new NameElement("one"));
        Assert.Equal(1, done);
        Assert.NotSame(xone, items[0]);
        Assert.Equal("one", ((NameElement)items[0]).Name);

        try { _ = items.Replace(2, new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Many()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);

        var done = items.Replace(0, new ChainElement(false, [xfour, xfive]));
        Assert.Equal(2, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xfive, items[1]);
        Assert.Same(xtwo, items[2]);
        Assert.Same(xthree, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Setter()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        items[0] = xone;
        Assert.Same(xone, items[0]);

        items[0] = new NameElement("one");
        Assert.NotSame(xone, items[0]);
        Assert.Equal("one", ((NameElement)items[0]).Name);

        try { items[2] = new NameElement("TWO"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Setter_Many()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        items[0] = new ChainElement(false, [xfour, xfive]);

        Assert.Equal(4, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xfive, items[1]);
        Assert.Same(xtwo, items[2]);
        Assert.Same(xthree, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        var done = items.Add(xfour);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);

        try { items.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Add(new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }

        items = new ChainElement(false, [xone, xtwo, xthree]);
        done = items.Add(xone);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xone, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Many()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        var done = items.Add(new ChainElement(false));
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        done = items.Add(new ChainElement(false, [xfour, xone]));
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xone, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        var done = items.AddRange([xfour, xone]);
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xone, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Many()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        var done = items.AddRange([xfour, new ChainElement(false, [xfive, xone])]);
        Assert.Equal(3, done);
        Assert.Equal(6, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xfour, items[3]);
        Assert.Same(xfive, items[4]);
        Assert.Same(xone, items[5]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        var done = items.Insert(0, xfour);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Same(xtwo, items[2]);
        Assert.Same(xthree, items[3]);

        try { items.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.Insert(0, new NameElement("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }

        items = new ChainElement(false, [xone, xtwo, xthree]);
        done = items.Insert(3, xone);
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
        Assert.Same(xone, items[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Many()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        var done = items.Insert(0, new ChainElement(false));
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        done = items.Insert(0, new ChainElement(false, [xfour, xfive]));
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xfive, items[1]);
        Assert.Same(xone, items[2]);
        Assert.Same(xtwo, items[3]);
        Assert.Same(xthree, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        var done = items.InsertRange(0, new[] { xfour, xfive });
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xfive, items[1]);
        Assert.Same(xone, items[2]);
        Assert.Same(xtwo, items[3]);
        Assert.Same(xthree, items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Many()
    {
        var items = new ChainElement(false, [xone, xtwo, xthree]);
        var done = items.InsertRange(0, [xfour, new ChainElement(false, [xfive, xsix])]);
        Assert.Equal(3, done);
        Assert.Equal(6, items.Count);
        Assert.Same(xfour, items[0]);
        Assert.Same(xfive, items[1]);
        Assert.Same(xsix, items[2]);
        Assert.Same(xone, items[3]);
        Assert.Same(xtwo, items[4]);
        Assert.Same(xthree, items[5]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.RemoveAt(0);
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
       
        var done = items.RemoveRange(0, 0);
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        done = items.RemoveRange(0, 2);
        Assert.Equal(2, done);
        Assert.Single(items);
        Assert.Same(xthree, items[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree, xone });
        var done = items.Remove("ONE");
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xone, items[2]);

        items = new ChainElement(false, new[] { xone, xtwo, xthree, xone });
        done = items.RemoveLast("ONE");
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        items = new ChainElement(false, new[] { xone, xtwo, xthree, xone });
        done = items.RemoveAll("ONE");
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree, xfour });
        var done = items.Remove(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xthree, items[1]);
        Assert.Same(xfour, items[2]);

        items = new ChainElement(false, new[] { xone, xtwo, xthree, xfour });
        done = items.RemoveLast(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xfour, items[2]);

        items = new ChainElement(false, new[] { xone, xtwo, xthree, xfour });
        done = items.RemoveAll(x => x is NameElement named && named.Name.Contains('e'));
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Same(xtwo, items[0]);
        Assert.Same(xfour, items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var items = new ChainElement(false, new[] { xone, xtwo, xthree });
        var done = items.Clear();
        Assert.Equal(3, done);
        Assert.Empty(items);
    }
}