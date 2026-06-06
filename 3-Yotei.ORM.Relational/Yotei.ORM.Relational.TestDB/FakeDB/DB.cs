namespace Yotei.ORM.Relational.TestDB;

// ========================================================
public static partial class DB
{
    // Used to lock database operations.
    public static Lock SyncRoot { get; } = new();

    // Used to determine if the default collections are in short or long mode.
    public static bool LongEnvironment { get; set; } = !Environment.IsDebug;

    // ----------------------------------------------------
}