namespace Yotei.ORM.Relational.FakeDB;

// ========================================================
public static class DB
{
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
}