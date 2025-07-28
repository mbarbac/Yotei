namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable(ReturnInterface = true)]
[InheritWiths]
public partial class FakeEngine : Engine, IEngine
{
    public FakeEngine() : base() { }
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}