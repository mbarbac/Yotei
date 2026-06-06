namespace Yotei.ORM.Relational.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Transaction
{
    const string SERVER = "(LocalDB)\\MSSQLLocalDB";
    const string DATABASE = "YoteiDB";
    const string CNSTRING = "Server=(LocalDB)\\MSSQLLocalDB;Database=YoteiDB;";

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Start_OnClosedConnection_Explicit()
    {
        var engine = new FakeEngine();
        using var cn = new Code.Connection(engine, CNSTRING);

        ITransaction tx;
        using (tx = cn.StartTransaction())
        {
            Assert.True(cn.IsOpen);
            Assert.Same(cn.Transaction, tx);

            tx.Commit();
            Assert.False(cn.IsOpen);
        }
        Assert.True(tx.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_OnClosedConnection_Implicit()
    {
        var engine = new FakeEngine();
        using var cn = new Code.Connection(engine, CNSTRING);

        ITransaction tx;
        using (tx = cn.StartTransaction())
        {
            Assert.True(cn.IsOpen);
            Assert.Same(cn.Transaction, tx);
        }
        Assert.True(tx.IsDisposed);
        Assert.False(cn.IsOpen);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Start_OnOpenedConnection_Explicit()
    {
        var engine = new FakeEngine();
        using var cn = new Code.Connection(engine, CNSTRING);

        cn.Open();
        Assert.True(cn.IsOpen);

        ITransaction tx;
        using (tx = cn.StartTransaction())
        {
            Assert.Same(cn.Transaction, tx);

            tx.Commit();
            Assert.True(cn.IsOpen);
        }
        Assert.True(tx.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_OnOpenedConnection_Implicit()
    {
        var engine = new FakeEngine();
        using var cn = new Code.Connection(engine, CNSTRING);

        cn.Open();
        Assert.True(cn.IsOpen);

        ITransaction tx;
        using (tx = cn.StartTransaction())
        {
            Assert.True(cn.IsOpen);
            Assert.Same(cn.Transaction, tx);
        }
        Assert.True(tx.IsDisposed);
        Assert.True(cn.IsOpen);
    }
}