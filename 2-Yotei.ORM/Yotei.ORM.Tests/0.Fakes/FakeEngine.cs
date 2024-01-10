namespace Yotei.ORM.Tests;

// ========================================================
[WithGenerator]
public partial class FakeEngine : Code.Engine
{
    public FakeEngine() { }
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}