#pragma warning disable CS0436

namespace Yotei.ORM.Tests;

// ========================================================
[WithGenerator("(source)+@")]
public partial class FakeEngine : Engine
{
    public FakeEngine() => KnownTags = new FakeKnownTags();
    protected FakeEngine(Engine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}