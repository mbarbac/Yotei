namespace Yotei.ORM.Tests;

// ========================================================
[InheritWiths]
public partial class FakeEngine : Code.Engine
{
    public FakeEngine() : base() { }
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}