using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using DB = Yotei.ORM.Relational.FakeDB.DB;

namespace Yotei.ORM.Relational.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_RawCommand
{
    //[Enforced]
    [Fact]
    public static void Test_Query_All()
    {
        var engine = new FakeEngine();
        using var connection = new Code.Connection(engine, DB.ConnectionString);

        lock (DB.Syncroot)
        {
            DB.IsLongDatabase = DB.LongEnvironment;
            DB.EnsureDbContents(connection);

            var cmd = connection.Records.Raw("SELECT * FROM Employees");

            WriteLine(true, $"\n> Command: {cmd}");
            var num = 0;

            foreach (var item in cmd)
            {
                WriteLine(true, $"\n- {item}");
                num++;
            }

            var count = DB.Employees.Count;
            Assert.Equal(count, num);
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Query_Filtered()
    {
        var engine = new FakeEngine();
        using var connection = new Code.Connection(engine, DB.ConnectionString);

        lock (DB.Syncroot)
        {
            DB.IsLongDatabase = DB.LongEnvironment;
            DB.EnsureDbContents(connection);

            var dt = new DateOnly(1969, 1, 1);
            var cmd = connection.Records.Raw(
                "SELECT * FROM Employees WHERE BirthDate >= {0}", dt);

            WriteLine(true, $"\n> Command: {cmd}");
            var num = 0;

            foreach (var item in cmd)
            {
                WriteLine(true, $"\n- {item}");
                num++;
            }

            var count = DB.Employees
                .Where(x => x.BirthDate is not null && x.BirthDate > dt)
                .Count();

            Assert.Equal(count, num);
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Query_Multiple_Tables()
    {
        var engine = new FakeEngine();
        using var connection = new Code.Connection(engine, DB.ConnectionString);

        lock (DB.Syncroot)
        {
            DB.IsLongDatabase = DB.LongEnvironment;
            DB.EnsureDbContents(connection);

            var dt = new DateOnly(1969, 1, 1);
            var cmd = connection.Records.Raw(
                "SELECT * FROM Employees AS Emp, Countries AS Ctry " +
                "WHERE Emp.BirthDate >= {0} AND Ctry.Id = Emp.CountryId",
                dt);

            WriteLine(true, $"\n> Command: {cmd}");
            var num = 0;

            foreach (var item in cmd)
            {
                WriteLine(true, $"\n- {item}");
                num++;
            }

            var count = DB.Employees
                .Where(x => x.BirthDate is not null && x.BirthDate > dt)
                .Count();

            Assert.Equal(count, num);
        }
    }

    // ----------------------------------------------------

    static List<IRecord> Do_Select(IConnection connection)
    {
        var cmd = connection.Records.Raw(
            "SELECT * FROM Employees WHERE Id = {0}", "007");

        WriteLine(true, $"\n> Command: {cmd}");
        var list = cmd.ToList();

        foreach (var item in list) WriteLine(true, $"\n- {item}");
        return list;
    }

    static List<IRecord> Do_Insert(IConnection connection)
    {
        var cmd = connection.Records.Raw(
            "INSERT INTO Employees " +
            "(Id, FirstName, LastName, CountryId, BirthDate, JoinDate, Photo) " +
            "OUTPUT INSERTED.* " +
            "VALUES ( {0}, {1}, {2}, {3}, {4}, {5}, {6} )",
            "007",
            "James",
            "Bond",
            "uk",
            new DateOnly(1950, 1, 1),
            null,
            new byte[] { 0, 0, 7 });

        WriteLine(true, $"\n> Command: {cmd}");
        var list = cmd.ToList();

        foreach (var item in list) WriteLine(true, $"\n- {item}");
        return list;
    }

    static List<IRecord> Do_Delete(IConnection connection)
    {
        var cmd = connection.Records.Raw(
            "DELETE FROM Employees OUTPUT DELETED.* WHERE Id = {0}", "007");

        WriteLine(true, $"\n> Command: {cmd}");
        var list = cmd.ToList();

        foreach (var item in list) WriteLine(true, $"\n- {item}");
        return list;
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        using var connection = new Code.Connection(engine, DB.ConnectionString);

        lock (DB.Syncroot)
        {
            DB.IsLongDatabase = DB.LongEnvironment;
            DB.EnsureDbContents(connection);

            List<IRecord> list;
            list = Do_Insert(connection); Assert.Single(list);
            list = Do_Select(connection); Assert.Single(list);

            try { Do_Insert(connection); Assert.Fail(); }
            catch (SqlException) { }
        }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Delete()
    {
        var engine = new FakeEngine();
        using var connection = new Code.Connection(engine, DB.ConnectionString);

        lock (DB.Syncroot)
        {
            DB.IsLongDatabase = DB.LongEnvironment;
            DB.EnsureDbContents(connection);

            List<IRecord> list;
            list = Do_Insert(connection); Assert.Single(list);
            list = Do_Select(connection); Assert.Single(list);
            list = Do_Delete(connection); Assert.Single(list);
            list = Do_Select(connection); Assert.Empty(list);
        }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Stored_Procedure()
    {
        var engine = new FakeEngine();
        using var connection = new Code.Connection(engine, DB.ConnectionString);

        lock (DB.Syncroot)
        {
            DB.IsLongDatabase = DB.LongEnvironment;
            DB.EnsureDbContents(connection);

            var cmd = connection.Records.Raw(
                "EXEC employee_insert @FirstName = {0}, @LastName = {1}",
                "James",
                "Bond");

            WriteLine(true, $"\n> Command: {cmd}");
            var num = 0;

            foreach (var item in cmd)
            {
                WriteLine(true, $"\n- {item}");
                num++;
            }

            var count = 1;
            Assert.Equal(count, num);
        }
    }
}
