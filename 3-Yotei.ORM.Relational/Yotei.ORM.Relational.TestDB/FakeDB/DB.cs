using static Yotei.Tools.ConsoleExtensions;
using static System.ConsoleColor;

namespace Yotei.ORM.Relational.TestDB;

// ========================================================
public static partial class DB
{
    // A default connection string.
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

    // Used to lock database operations.
    public static Lock SyncRoot { get; } = new();

    // Used to determine if the default collections are in short or long mode.
    public static bool LongEnvironment
    {
        get;
        set
        {
            if (field == value) return;
            Regions = null!;
            Countries = null!;
            Employees = null!;
            Talents = null!;
            EmployeeTalents = null!;
        }
    }
    = !Environment.IsDebug;

    // ----------------------------------------------------

    // ----------------------------------------------------

    // ----------------------------------------------------

    // ----------------------------------------------------
}