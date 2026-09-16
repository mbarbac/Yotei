namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable(ReturnType = typeof(IEngine))]
[InheritsWith(ReturnType = typeof(IEngine))]
public partial class FakeEngine : Engine
{
    public FakeEngine() : base() { }
    protected FakeEngine(FakeEngine other) : base(other) { }
    public override string ToString() => "FakeEngine";
}