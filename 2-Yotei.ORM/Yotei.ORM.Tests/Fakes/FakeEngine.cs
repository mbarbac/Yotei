namespace Yotei.ORM.Tests;

// ========================================================
[WithGenerator(Specs = "(source)+@")]
public partial class FakeEngine : Engine
{
    public FakeEngine() { }
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}