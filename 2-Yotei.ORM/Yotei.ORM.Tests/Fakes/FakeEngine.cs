namespace Yotei.ORM.Tests;

// ========================================================
[WithGenerator]
public partial class FakeEngine : ORM.Code.Engine
{
    public FakeEngine() { }
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}