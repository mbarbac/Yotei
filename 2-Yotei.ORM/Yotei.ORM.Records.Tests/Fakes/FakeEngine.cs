namespace Yotei.ORM.Records.Tests;

// ========================================================
[WithGenerator]
public partial class FakeEngine : Code.Engine
{
    public FakeEngine() => KnownTags = new FakeKnownTags(CASESENSITIVETAGS);
    protected FakeEngine(FakeEngine source) : base(source) { }
    public override string ToString() => "FakeEngine";
}