#pragma warning disable CS0436

namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable(ReturnType = typeof(IEngine))]
[InheritsWith(ReturnType = typeof(IEngine))]
public partial class FakeEngine : Engine
{
    public FakeEngine() : base() { }
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}