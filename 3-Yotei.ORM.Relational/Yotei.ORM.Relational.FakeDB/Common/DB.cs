using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Relational.FakeDB;

// ========================================================
public static partial class DB
{
    /// <summary>
    /// Shared connection string with the test database.
    /// </summary>
    public const string ConnectionString =
       "Data Source = (localdb)\\MSSQLLocalDB;" +
       "Initial Catalog = YoteiDB;" +
       "Integrated Security = True;" +
       "Persist Security Info = False;" +
       "Pooling = False;" +
       "Multiple Active Result Sets = True;" +
       "Encrypt = False;" +
       "TrustServerCertificate = False;" +
       "Connect Timeout = 10;" +
       "ApplicationIntent = ReadWrite;" +
       "MultiSubnetFailover = False";

    /// <summary>
    /// The object that can be used to serialize operations on the underlying database.
    /// </summary>
    public static object Syncroot { get; } = new();

    /// <summary>
    /// Determines the default mode of the test environment.
    /// </summary>
    public static bool LongEnvironment =>
#if DEBUG
        false;
#else
        true;
#endif

    /// <summary>
    /// Determines if the test database is a long or short one.
    /// </summary>
    public static bool IsLongDatabase
    {
        get => _IsLongDatabase;
        set
        {
            if (_Initialized && _IsLongDatabase == value) return;

            _Regions = null!;
            _Countries = null!;
            _Employees = null!;
            _Talents = null!;
            _EmployeeTalents = null!;

            _Initialized = true;
            _IsLongDatabase = value;
        }
    }
    static bool _IsLongDatabase = false;
    static bool _Initialized = false;

    // ----------------------------------------------------

    /// <summary>
    /// Ensures that the DB contents match the ones managed by the test environment.
    /// </summary>
    /// <param name="connection"></param>
    public static void EnsureDbContents(IConnection connection)
    {
        WriteLine(true, "\n> Ensuring database contents...");
        var wasopen = connection.IsOpen;

        lock (Syncroot)
        {
            try
            {
                if (!wasopen) connection.Open();

                if (!IsPrepared(connection))
                {
                    DeleteDbContents(connection);
                    InsertDbContents(connection);
                }
            }
            finally
            {
                if (!wasopen) connection.Close();
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to determine if the database contents are ready or not.
    /// </summary>
    /// <param name="connection"></param>
    public static bool IsPrepared(IConnection connection)
    {
        if (!IsPreparedRegions(connection)) return false;
        if (!IsPreparedCountries(connection)) return false;
        if (!IsPreparedEmployees(connection)) return false;
        if (!IsPreparedTalents(connection)) return false;
        if (!IsPreparedEmployeeTalents(connection)) return false;

        return true;
    }

    // Determines readiness of Regions...
    static bool IsPreparedRegions(IConnection connection)
    {
        WriteLine(true, "- Ensuring Regions...");

        var temps = connection.Records.Raw("SELECT * FROM [Regions]").ToList();
        if (temps.Count == 0) return false;

        var index = temps[0].Schema!.IndexOf("Id");
        if (index < 0) return false;

        foreach (var item in Regions)
        {
            var temp = temps.Where(x => ((string?)x[index]) == item.Id).FirstOrDefault();
            if (temp == null) return false;
            temps.Remove(temp);
        }

        return temps.Count == 0;
    }

    // Determines readiness of Countries...
    static bool IsPreparedCountries(IConnection connection)
    {
        WriteLine(true, "- Ensuring Countries...");

        var temps = connection.Records.Raw("SELECT * FROM [Countries]").ToList();
        if (temps.Count == 0) return false;

        var index = temps[0].Schema!.IndexOf("Id");
        if (index < 0) return false;

        foreach (var item in Countries)
        {
            var temp = temps.Where(x => ((string?)x[index]) == item.Id).FirstOrDefault();
            if (temp == null) return false;
            temps.Remove(temp);
        }

        return temps.Count == 0;
    }

    // Determines readiness of Employees...
    static bool IsPreparedEmployees(IConnection connection)
    {
        WriteLine(true, "- Ensuring Employees...");

        var temps = connection.Records.Raw("SELECT * FROM [Employees]").ToList();
        if (temps.Count == 0) return false;

        var index = temps[0].Schema!.IndexOf("Id");
        if (index < 0) return false;

        foreach (var item in Employees)
        {
            var temp = temps.Where(x => ((string?)x[index]) == item.Id).FirstOrDefault();
            if (temp == null) return false;
            temps.Remove(temp);
        }

        return temps.Count == 0;
    }

    // Determines readiness of Talents...
    static bool IsPreparedTalents(IConnection connection)
    {
        WriteLine(true, "- Ensuring Talents...");

        var temps = connection.Records.Raw("SELECT * FROM [Talents]").ToList();
        if (temps.Count == 0) return false;

        var index = temps[0].Schema!.IndexOf("Id");
        if (index < 0) return false;

        foreach (var item in Talents)
        {
            var temp = temps.Where(x => ((string?)x[index]) == item.Id).FirstOrDefault();
            if (temp == null) return false;
            temps.Remove(temp);
        }

        return temps.Count == 0;
    }

    // Determines readiness of EmployeeTalents...
    static bool IsPreparedEmployeeTalents(IConnection connection)
    {
        WriteLine(true, "- Ensuring EmployeeTalents...");

        var temps = connection.Records.Raw("SELECT * FROM [EmployeeTalents]").ToList();
        if (temps.Count == 0) return false;

        var indexEmp = temps[0].Schema!.IndexOf("EmployeeId");
        if (indexEmp < 0) return false;

        var indexTal = temps[0].Schema!.IndexOf("EmployeeId");
        if (indexTal < 0) return false;

        foreach (var item in EmployeeTalents)
        {
            var temp = temps.Where(x => 
                ((string?)x[indexEmp]) == item.EmployeeId &&
                ((string?)x[indexTal]) == item.TalentId)
                .FirstOrDefault();

            if (temp == null) return false;
            temps.Remove(temp);
        }

        return temps.Count == 0;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Deletes all the contents of the test database.
    /// </summary>
    /// <param name="connection"></param>
    public static void DeleteDbContents(IConnection connection)
    {
        WriteLine(true, "\n> Deleting database contents...");
        var wasopen = connection.IsOpen;

        lock (Syncroot)
        {
            try
            {
                if (!wasopen) connection.Open();

                WriteLine(true, "- Deleting EmployeeTalents...");
                connection.Records.Raw("DELETE FROM EmployeeTalents").Execute();

                WriteLine(true, "- Deleting Talents...");
                connection.Records.Raw("DELETE FROM Talents").Execute();

                WriteLine(true, "- Deleting Employees...");
                connection.Records.Raw("DELETE FROM Employees").Execute();

                WriteLine(true, "- Deleting Countries...");
                connection.Records.Raw("DELETE FROM Countries").Execute();

                WriteLine(true, "- Deleting Regions...");
                connection.Records.Raw("DELETE FROM Regions").Execute();
            }
            finally
            {
                if (!wasopen) connection.Close();
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Deletes all the contents of the test database.
    /// </summary>
    /// <param name="connection"></param>
    public static void InsertDbContents(IConnection connection)
    {
        WriteLine(true, "\n> Inserting database contents...");
        var wasopen = connection.IsOpen;

        lock (Syncroot)
        {
            try
            {
                if (!wasopen) connection.Open();

                InsertRegions(connection);
                InsertCountries(connection);
                InsertEmployees(connection);
                InsertTalents(connection);
                InsertEmployeeTalents(connection);
            }
            finally
            {
                if (!wasopen) connection.Close();
            }
        }
    }

    // Inserts the contents of Regions...
    static void InsertRegions(IConnection connection)
    {
        Write(true, "- Inserting Regions...");

        int num = 0; foreach (var item in Regions)
        {
            Write(true, ".");
            var str = GetCommand(item);
            connection.Records.Raw(str).Execute();
            num++;
        }
        WriteLine(true, $" = {num}");

        static string GetCommand(RegionDTO item) =>
            "INSERT INTO [Regions] " +
            "(Id, Name, ParentId) VALUES (" +
            $"{ToValue(item.Id)}, " +
            $"{ToValue(item.Name)}, " +
            $"{ToValue(item.ParentId)}" +
            ")";
    }

    // Inserts the contents of Countries...
    static void InsertCountries(IConnection connection)
    {
        Write(true, "- Inserting Countries...");

        int num = 0; foreach (var item in Countries)
        {
            Write(true, ".");
            var str = GetCommand(item);
            connection.Records.Raw(str).Execute();
            num++;
        }
        WriteLine(true, $" = {num}");

        static string GetCommand(CountryDTO item) =>
            "INSERT INTO [Countries] " +
            "(Id, Name, RegionId) VALUES (" +
            $"{ToValue(item.Id)}, " +
            $"{ToValue(item.Name)}, " +
            $"{ToValue(item.RegionId)}" +
            ")";
    }

    // Inserts the contents of Employees...
    static void InsertEmployees(IConnection connection)
    {
        Write(true, "- Inserting Employees...");

        int num = 0; foreach (var item in Employees)
        {
            Write(true, ".");
            var str = GetCommand(item);
            connection.Records.Raw(str).Execute();
            num++;
        }
        WriteLine(true, $" = {num}");

        static string GetCommand(EmployeeDTO item) =>
            "INSERT INTO [Employees] " +
            "(Id, FirstName, LastName, BirthDate, Active, JoinDate, StartTime, Photo, CountryId, ManagerId) VALUES (" +
            $"{ToValue(item.Id)}, " +
            $"{ToValue(item.FirstName)}, " +
            $"{ToValue(item.LastName)}, " +
            $"{ToValue(item.BirthDate)}, " +
            $"{ToValue(item.Active)}, " +
            $"{ToValue(item.JoinDate)}, " +
            $"{ToValue(item.StartTime)}, " +
            $"{ToValue(item.Photo)}, " +
            $"{ToValue(item.CountryId)}, " +
            $"{ToValue(item.ManagerId)}" +
            ")";
    }

    // Inserts the contents of Talents...
    static void InsertTalents(IConnection connection)
    {
        Write(true, "- Inserting Talents...");

        int num = 0; foreach (var item in Talents)
        {
            Write(true, ".");
            var str = GetCommand(item);
            connection.Records.Raw(str).Execute();
            num++;
        }
        WriteLine(true, $" = {num}");

        static string GetCommand(TalentDTO item) =>
            "INSERT INTO [Talents] " +
            "(Id, Description) VALUES (" +
            $"{ToValue(item.Id)}, " +
            $"{ToValue(item.Description)}" +
            ")";
    }

    // Inserts the contents of EmployeeTalents...
    static void InsertEmployeeTalents(IConnection connection)
    {
        Write(true, "- Inserting EmployeeTalents...");

        int num = 0; foreach (var item in EmployeeTalents)
        {
            Write(true, ".");
            var str = GetCommand(item);
            connection.Records.Raw(str).Execute();
            num++;
        }
        WriteLine(true, $" = {num}");

        static string GetCommand(EmployeeTalentDTO item) =>
            "INSERT INTO [EmployeeTalents] " +
            "(EmployeeId, TalentId) VALUES (" +
            $"{ToValue(item.EmployeeId)}, " +
            $"{ToValue(item.TalentId)}" +
            ")";
    }

    /// <summary>
    /// Gets an appropriate string for the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static string ToValue(object? value)
    {
        return value switch
        {
            DateOnly item => $"'{item.Year:0000}-{item.Month}-{item.Day}'",
            TimeOnly item => $"'{item.Hour}:{item.Minute}:{item.Second}'",
            string item => $"'{item}'",
            byte[] item => $"'{item.Sketch()}'",
            true => "'TRUE'",
            false => "'FALSE'",
            null => "NULL",
            _ => value.ToString() ?? string.Empty
        };
    }
}