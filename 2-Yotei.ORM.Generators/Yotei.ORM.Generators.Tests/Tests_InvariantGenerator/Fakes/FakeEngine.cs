namespace Yotei.ORM.Generators.Invariant.Tests;

// ========================================================
[InheritWiths<IEngine>]
public partial class FakeEngine : Engine
{
    public FakeEngine() : base() { }
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}