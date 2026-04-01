namespace Yotei.ORM.Generators.InvariantGenerator.Tests;

// ========================================================
[InheritsWith<IEngine>]
[Cloneable<IEngine>]
public partial class FakeEngine : Engine
{
    public FakeEngine() : base() { }
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}