namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Transaction
{
    //[Enforced]
    [Fact]
    public static void Test_Create_INFRASTRUCTURE()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        using var connection = new FakeConnection(engine);
        using var transaction = new FakeTransaction(connection);

        Assert.False(transaction.IsActive);
        Assert.False(transaction.IsDisposed);
        Assert.Null(connection.Transaction);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Commit_INFRASTRUCTURE()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine() { IgnoreCase = true };

        using (connection = new FakeConnection(engine))
        using (transaction = new FakeTransaction(connection))
        {
            Assert.False(transaction.IsActive);

            transaction.Start(); Assert.True(transaction.IsActive);
            Assert.Same(transaction, connection.Transaction);

            transaction.Commit(); Assert.False(transaction.IsActive);
            Assert.Null(connection.Transaction);
        }
        Assert.False(transaction.IsActive);
        Assert.True(transaction.IsDisposed);
        Assert.True(connection.IsDisposed);
        Assert.Null(connection.Transaction);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Abort_INFRASTRUCTURE()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine() { IgnoreCase = true };

        using (connection = new FakeConnection(engine))
        using (transaction = new FakeTransaction(connection))
        {
            Assert.False(transaction.IsActive);

            transaction.Start(); Assert.True(transaction.IsActive);
            Assert.Same(transaction, connection.Transaction);

            transaction.Abort(); Assert.False(transaction.IsActive);
            Assert.Null(connection.Transaction);
        }
        Assert.False(transaction.IsActive);
        Assert.True(transaction.IsDisposed);
        Assert.True(connection.IsDisposed);
        Assert.Null(connection.Transaction);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Dispose_INFRASTRUCTURE()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine() { IgnoreCase = true };

        using (connection = new FakeConnection(engine))
        using (transaction = new FakeTransaction(connection))
        {
            Assert.False(transaction.IsActive);

            transaction.Start(); Assert.True(transaction.IsActive);
            Assert.Same(transaction, connection.Transaction);
        }
        Assert.False(transaction.IsActive);
        Assert.True(transaction.IsDisposed);
        Assert.True(connection.IsDisposed);        
        Assert.Null(connection.Transaction);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Standard_Start_Commit()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine() { IgnoreCase = true };

        using (connection = new FakeConnection(engine))
        using (transaction = connection.StartTransaction())
        {
            Assert.True(transaction.IsActive);
            Assert.True(connection.IsOpen);
            Assert.Same(transaction, connection.Transaction);

            transaction.Commit();
            Assert.False(transaction.IsActive);
            Assert.False(connection.IsOpen);
            Assert.Null(connection.Transaction);
        }
        Assert.False(transaction.IsActive);
        Assert.True(transaction.IsDisposed);
        Assert.Null(connection.Transaction);
        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Start_Abort()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine() { IgnoreCase = true };

        using (connection = new FakeConnection(engine))
        using (transaction = connection.StartTransaction())
        {
            Assert.True(transaction.IsActive);
            Assert.True(connection.IsOpen);
            Assert.Same(transaction, connection.Transaction);

            transaction.Abort();
            Assert.False(transaction.IsActive);
            Assert.False(connection.IsOpen);
            Assert.Null(connection.Transaction);
        }
        Assert.False(transaction.IsActive);
        Assert.True(transaction.IsDisposed);
        Assert.Null(connection.Transaction);
        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Start_Dispose()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine() { IgnoreCase = true };

        using (connection = new FakeConnection(engine))
        using (transaction = connection.StartTransaction())
        {
            Assert.True(transaction.IsActive);
            Assert.True(connection.IsOpen);
            Assert.Same(transaction, connection.Transaction);
        }
        Assert.False(transaction.IsActive);
        Assert.True(transaction.IsDisposed);
        Assert.Null(connection.Transaction);
        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }
}