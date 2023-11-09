namespace Yotei.ORM.Tests;

// ========================================================
[WithGenerator]
public partial class FakeEngine : Engine
{
    public FakeEngine() => KnownTags = new FakeKnownTags();
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}