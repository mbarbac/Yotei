namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_Transaction_Custom
{
    public class CustomTransaction : DisposableClass, ITransaction
    {
        public CustomTransaction(IConnection connection)
        {
            Connection = connection;
            if (connection is Connection valid) valid.AddTransaction(this);
        }
        protected override void OnDispose(bool disposing)
        {
            if (IsDisposed || !disposing) return;

            if (Connection is Connection valid) valid.RemoveTransaction(this);
            try { if (IsActive) Abort(); } catch { }
        }
        protected override async ValueTask OnDisposeAsync(bool disposing)
        {
            if (IsDisposed || !disposing) return;

            if (Connection is Connection valid) valid.RemoveTransaction(this);
            try { if (IsActive) await AbortAsync().ConfigureAwait(false); } catch { }
        }

        public IConnection Connection { get; }
        public bool IsActive => Level > 0;
        public int Level { get; set; }

        public void Start()
        {
            if (!Connection.IsOpen) Connection.Open();
            Level++;
        }
        public ValueTask StartAsync(CancellationToken token = default) { Start(); return ValueTask.CompletedTask; }
        public void Commit()
        {
            if (Level == 1 && Connection.IsOpen) Connection.Close();
            if (Level > 0) Level--;
        }
        public ValueTask CommitAsync(CancellationToken token = default) { Commit(); return ValueTask.CompletedTask; }
        public void Abort()
        {
            if (Connection.IsOpen) Connection.Close();
            Level = 0;
        }
        public ValueTask AbortAsync() { Abort(); return ValueTask.CompletedTask; }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Creation()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            using (transaction = new CustomTransaction(connection))
            {
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);
            }

            Assert.False(connection.IsDisposed);
            Assert.True(transaction.IsDisposed);
        }
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Commit()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            using (transaction = new CustomTransaction(connection))
            {
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);

                var num = 3;
                for (int i = 1; i <= num; i++)
                {
                    transaction.Start();
                    Assert.True(connection.IsOpen);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i, transaction.Level);
                }
                for (int i = num - 1; i >= 0; i--)
                {
                    transaction.Commit();
                    Assert.Equal(i > 0, connection.IsOpen);
                    Assert.Equal(i > 0, transaction.IsActive);
                    Assert.Equal(i, transaction.Level);
                }

                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);
            }

            Assert.False(connection.IsDisposed);
            Assert.True(transaction.IsDisposed);
        }
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Abort()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            using (transaction = new CustomTransaction(connection))
            {
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);

                var num = 3;
                for (int i = 1; i <= num; i++)
                {
                    transaction.Start();
                    Assert.True(connection.IsOpen);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i, transaction.Level);
                }

                transaction.Abort();
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);
            }

            Assert.False(connection.IsDisposed);
            Assert.True(transaction.IsDisposed);
        }
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_ImplicitAbort()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            using (transaction = new CustomTransaction(connection))
            {
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);

                var num = 3;
                for (int i = 1; i <= num; i++)
                {
                    transaction.Start();
                    Assert.True(connection.IsOpen);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i, transaction.Level);
                }
            }

            Assert.False(connection.IsDisposed);
            Assert.True(transaction.IsDisposed);
        }
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_ConnectionDisposed()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            transaction = new CustomTransaction(connection);
            Assert.False(connection.IsOpen);
            Assert.False(transaction.IsActive);
            Assert.False(transaction.IsDisposed);

            var num = 3;
            for (int i = 1; i <= num; i++)
            {
                transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(transaction.IsActive);
                Assert.Equal(i, transaction.Level);
            }
        }

        Assert.True(connection.IsDisposed);
        Assert.True(transaction.IsDisposed);
    }
}