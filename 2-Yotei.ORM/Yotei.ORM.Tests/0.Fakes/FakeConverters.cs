namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable]
public partial class FakeConverters : ValueConverterList
{
    public FakeConverters()
    {
        Add(new ValueConverter<DateOnly, DateTime>((x, _) => x.ToDateTime(new TimeOnly())));
        Add(new ValueConverter<DateTime, DateOnly>((x, _) => new DateOnly(x.Year, x.Month, x.Day)));
    }

    protected FakeConverters(FakeConverters source) : base(source) { }
}