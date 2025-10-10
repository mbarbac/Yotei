namespace Yotei.ORM.Tests;

// ========================================================
public record FakeEngine : Engine
{
    public FakeEngine() : base() { }
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";

    public static bool Compare(FakeEngine? x, FakeEngine? y) => Engine.Compare(x, y);
    public virtual bool Equals(FakeEngine? other) => Compare(this, other);
    public override int GetHashCode() => base.GetHashCode();
}