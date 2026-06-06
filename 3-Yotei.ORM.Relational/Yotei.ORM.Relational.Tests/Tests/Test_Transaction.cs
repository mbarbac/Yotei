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
    public static void Test_Start_OnClosedConnection()
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
}