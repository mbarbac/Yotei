namespace Yotei.ORM.Core.Tests;

// ========================================================
//[Enforced]
public static class Test_Connection
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        Assert.Equal(Connection.RETRIES, connection.Retries);
        Assert.Equal(Connection.RETRYINTERVAL, connection.RetryInterval.Milliseconds);
        Assert.Equal(CultureInfo.CurrentCulture, connection.Locale.CultureInfo);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        IEngine engine = new FakeEngine();
        IConnection source = new FakeConnection(engine);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Retries, target.Retries);
        Assert.Equal(source.RetryInterval, target.RetryInterval);
        Assert.Equal(source.Locale, target.Locale);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        IEngine engine = new FakeEngine();
        IConnection source = new FakeConnection(engine);

        var target = source.WithRetries(100);
        Assert.Same(source, target);
        Assert.Equal(100, source.Retries);

        target = source.WithRetryInterval(TimeSpan.FromSeconds(100));
        Assert.Same(source, target);
        Assert.Equal(100, source.RetryInterval.TotalSeconds);

        var locale = new Locale(CultureInfo.GetCultureInfo("es-ES"));
        target = source.WithLocale(locale);
        Assert.Same(source, target);
        Assert.Equal(locale, source.Locale);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_And_Dispose()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.NotNull(connection.Transaction);
            Assert.False(connection.Transaction.IsDisposed);
            Assert.False(connection.IsDisposed);
        }

        Assert.True(connection.Transaction.IsDisposed);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Close()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);

            connection.Open();
            Assert.True(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);

            connection.Close();
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
            Assert.False(connection.Transaction.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Dispose()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);

            connection.Open();
            Assert.True(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }
}