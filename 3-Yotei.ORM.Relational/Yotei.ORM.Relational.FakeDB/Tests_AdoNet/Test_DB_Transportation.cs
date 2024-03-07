using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.ORM.Relational.FakeDB;

// ========================================================
//[Enforced]
public static class Test_DB_Transportation
{
    static void PrintSchema(DataTable table)
    {
        WriteLine();
        WriteLine(Green, "==============================");
        WriteLine(Green, "Schema:");

        for (int i = 0; i < table.Rows.Count; i++)
        {
            WriteLine();
            Write(Green, "Row: "); WriteLine($"{i}");

            var row = table.Rows[i];
            for (int k = 0; k < table.Columns.Count; k++)
            {
                var name = table.Columns[k].ColumnName;
                var value = row[k] is DBNull ? null : row[k];
                WriteLine($"- {name}: {value.Sketch()}");
            }
        }
    }

    static void PrintContents(DbDataReader reader)
    {
        WriteLine();
        WriteLine(Green, "==============================");
        WriteLine(Green, "Contents:");

        while(reader.Read())
        {
            WriteLine();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                WriteLine($"{i:00}: {value.Sketch()}");
            }
        }
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    public static void Test_Query_Plates()
    {
        var factory = SqlClientFactory.Instance;
        var cnstr =
            "Data Source=(localdb)\\MSSQLLocalDB;" +
            "Initial Catalog=Transportation;" +
            "Integrated Security=True;" +
            "Connect Timeout=30;" +
            "Encrypt=False;" +
            "Trust Server Certificate=False;" +
            "Application Intent=ReadWrite;" +
            "Multi Subnet Failover=False";

        using var conn = factory.CreateConnection();
        conn.ConnectionString = cnstr;
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT * FROM Plates
            """;

        using var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
        using var table = reader.GetSchemaTable() ?? throw new UnExpectedException();
        PrintSchema(table);
        PrintContents(reader);
    }
}