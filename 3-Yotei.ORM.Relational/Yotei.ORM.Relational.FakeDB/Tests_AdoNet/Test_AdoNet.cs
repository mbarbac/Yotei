using static System.Console;
namespace Yotei.ORM.Relational.FakeDB;

// ========================================================
//[Enforced]
public static class Test_AdoNet
{
    public const string ConnectionString =
       "Data Source = (localdb)\\MSSQLLocalDB;" +
       "Initial Catalog = KeroseneDB;" +
       "Integrated Security = True;" +
       "Persist Security Info = False;" +
       "Pooling = False;" +
       "Multiple Active Result Sets = True;" +
       "Encrypt = False;" +
       "TrustServerCertificate = False;" +
       "Connect Timeout = 10;" +
       "ApplicationIntent = ReadWrite;" +
       "MultiSubnetFailover = False";

    // ----------------------------------------------------

    [Enforced]
    [Fact]
    public static void Test_Query_Employees()
    {
        var factory = SqlClientFactory.Instance;

        using var conn = factory.CreateConnection();
        conn.ConnectionString = ConnectionString;
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = $$"""
            SELECT TOP 3 * FROM Employees AS [Emp]
            """;

        using var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
        using var table = reader.GetSchemaTable() ?? throw new UnExpectedException();

        WriteLine("========================================");
        WriteLine("Schema:");
        for (int i = 0; i < table.Rows.Count; i++)
        {
            WriteLine();
            WriteLine($"Row: {i}");
            var row = table.Rows[i];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                var name = table.Columns[j].ColumnName;
                var value = row[j] is DBNull ? null : row[j];
                WriteLine($"- {name}: {value.Sketch()}");
            }
        }

        WriteLine();
        WriteLine("========================================");
        WriteLine("Contents:");
        while (reader.Read())
        {
            WriteLine();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                WriteLine($"{i:00}: {value.Sketch()}");
            }
        }
    }
}