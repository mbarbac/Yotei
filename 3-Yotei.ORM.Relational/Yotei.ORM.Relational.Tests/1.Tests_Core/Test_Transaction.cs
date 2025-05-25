using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using DB = Yotei.ORM.Relational.FakeDB.DB;

namespace Yotei.ORM.Relational.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_Transaction
{
    static List<IRecord> Do_Select(IConnection connection)
    {
        var cmd = connection.Records.Raw(
            "SELECT * FROM [Regions] WHERE [Id] = 'ZZZ'");

        WriteLine(true, $"\n> Command: {cmd}");
        var list = cmd.ToList();

        foreach (var item in list) WriteLine(true, $"\n- {item}");
        return list;
    }

    static List<IRecord> Do_Insert(IConnection connection)
    {
        var cmd = connection.Records.Raw(
            "INSERT INTO [Regions] ([Id]) OUTPUT INSERTED.* VALUES ('ZZZ')");

        WriteLine(true, $"\n> Command: {cmd}");
        var list = cmd.ToList();

        foreach (var item in list) WriteLine(true, $"\n- {item}");
        return list;
    }

    static List<IRecord> Do_Delete(IConnection connection)
    {
        var cmd = connection.Records.Raw(
            "DELETE FROM [Regions] OUTPUT DELETED.* WHERE [Id] = 'ZZZ'");

        WriteLine(true, $"\n> Command: {cmd}");
        var list = cmd.ToList();

        foreach (var item in list) WriteLine(true, $"\n- {item}");
        return list;
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_Commit()
    {
        var engine = new FakeEngine();
        using var connection = new Code.Connection(engine, DB.ConnectionString);

        lock (DB.Syncroot)
        {
            DB.IsLongDatabase = DB.LongEnvironment;
            DB.EnsureDbContents(connection);

            List<IRecord> list;
            ITransaction transaction;

            using (transaction = new Code.Transaction(connection))
            {
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);

                transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(transaction.IsActive);

                list = Do_Insert(connection); Assert.Single(list);
                list = Do_Select(connection); Assert.Single(list);
                list = Do_Delete(connection); Assert.Single(list);
                list = Do_Select(connection); Assert.Empty(list);

                transaction.Commit();
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
            }

            list = Do_Select(connection); Assert.Empty(list);
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_No_Commit()
    {
        var engine = new FakeEngine();
        using var connection = new Code.Connection(engine, DB.ConnectionString);

        lock (DB.Syncroot)
        {
            DB.IsLongDatabase = DB.LongEnvironment;
            DB.EnsureDbContents(connection);

            List<IRecord> list;
            ITransaction transaction;

            using (transaction = new Code.Transaction(connection))
            {
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);

                transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(transaction.IsActive);

                list = Do_Insert(connection); Assert.Single(list);
                list = Do_Select(connection); Assert.Single(list);
            }

            Assert.False(connection.IsOpen);
            Assert.False(transaction.IsActive);

            list = Do_Select(connection); Assert.Empty(list);
        }
    }
}
