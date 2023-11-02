namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Record_Compare
{
    //[Enforced]
    [Fact]
    public static void Test_No_Changes()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);

        builder.Add("007", "Employees.Id", isPrimaryKey: true);
        builder.Add("James", "Employees.FirstName");
        builder.Add("Bond", "LastName");
        builder.Add("UK", "Country");
        var source = builder.Create();

        builder.Clear();
        builder.Add("Bond", "LastName");
        builder.Add("James", "Employees.FirstName");
        builder.Add("007", "Employees.Id", isPrimaryKey: true);
        builder.Add(50, "Age");
        var target = builder.Create();

        var record = source.CompareTo(target);
        Assert.Null(record);

        record = source.CompareTo(target, orphanSources: true);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal("UK", record[0]); Assert.Equal(record.Schema[0], source.Schema[3]);

        record = source.CompareTo(target, orphanTargets: true);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal(50, record[0]); Assert.Equal(record.Schema[0], target.Schema[3]);
    }
    
    //[Enforced]
    [Fact]
    public static void Test_Changes()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);

        builder.Add("007", "Employees.Id", isPrimaryKey: true);
        builder.Add("James", "Employees.FirstName");
        builder.Add("Bond", "LastName");
        builder.Add("UK", "Country");
        var source = builder.Create();

        builder.Clear();
        builder.Add("Bond", "LastName");
        builder.Add("James", "Employees.FirstName");
        builder.Add("008", source.Schema[0]);
        builder.Add(50, "Age");
        var target = builder.Create();

        var record = source.CompareTo(target);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal("008", record[0]); Assert.Same(source.Schema[0], record.Schema[0]);

        record = source.CompareTo(target, orphanSources: true);
        Assert.NotNull(record);
        Assert.Equal(2, record.Count);
        Assert.Equal("008", record[0]); Assert.Same(source.Schema[0], record.Schema[0]);
        Assert.Equal("UK", record[1]); Assert.Equal(record.Schema[1], source.Schema[3]);

        record = source.CompareTo(target, orphanTargets: true);
        Assert.NotNull(record);
        Assert.Equal(2, record.Count);
        Assert.Equal("008", record[0]); Assert.Same(source.Schema[0], record.Schema[0]);
        Assert.Equal(50, record[1]); Assert.Equal(record.Schema[1], target.Schema[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Duplicate_Sources()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);

        builder.Add("007", "Employees.Id", isPrimaryKey: true);
        builder.Add("James", "Employees.FirstName");
        builder.Add("008", builder.Schema[0]);
        var source = builder.Create();

        builder.Clear();
        builder.Add("Bond", "LastName");
        builder.Add("James", "Employees.FirstName");
        builder.Add("007", "Employees.Id", isPrimaryKey: true);
        var target = builder.Create();

        var record = source.CompareTo(target);
        Assert.Null(record);

        record = source.CompareTo(target, orphanSources: true);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal("008", record[0]); Assert.Equal(record.Schema[0], source.Schema[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Duplicate_Targets()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);

        builder.Add("007", "Employees.Id", isPrimaryKey: true);
        builder.Add("James", "Employees.FirstName");
        builder.Add("Bond", "LastName");
        var source = builder.Create();

        // Keeping order of appearance of ID...
        builder.Clear();
        builder.Add("007", source.Schema[0]);
        builder.Add("008", source.Schema[0]);
        var target = builder.Create();

        var record = source.CompareTo(target);
        Assert.Null(record);

        record = source.CompareTo(target, orphanTargets: true);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal("008", record[0]); Assert.Equal(record.Schema[0], source.Schema[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Duplicate_Targets_Reverse()
    {
        var engine = new FakeEngine();
        var builder = new RecordBuilder(engine);

        builder.Add("007", "Employees.Id", isPrimaryKey: true);
        builder.Add("James", "Employees.FirstName");
        builder.Add("Bond", "LastName");
        var source = builder.Create();

        // Reversing order of appearance of ID...
        builder.Clear();
        builder.Add("008", source.Schema[0]);
        builder.Add("007", source.Schema[0]);
        var target = builder.Create();

        var record = source.CompareTo(target);
        Assert.NotNull(record);
        Assert.Single(record);
        Assert.Equal("008", record[0]); Assert.Equal(record.Schema[0], source.Schema[0]);

        record = source.CompareTo(target, orphanTargets: true);
        Assert.NotNull(record);
        Assert.Equal(2, record.Count);
        Assert.Equal("008", record[0]); Assert.Equal(record.Schema[0], source.Schema[0]);
        Assert.Equal("007", record[1]); Assert.Equal(record.Schema[1], source.Schema[0]);
    }
}