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
}