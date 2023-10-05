#pragma warning disable CS0436

namespace Yotei.ORM.Core.Tests;

// ========================================================
[WithGenerator("(source)+*")]
public partial class FakeEngine : Engine
{
    public FakeEngine() : base() { }
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}