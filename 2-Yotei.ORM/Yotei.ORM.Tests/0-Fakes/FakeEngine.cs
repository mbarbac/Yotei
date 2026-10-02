namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable(ReturnType = typeof(IEngine))]
[InheritsWith(ReturnType = typeof(IEngine))]
public partial class FakeEngine : Engine
{
    public FakeEngine(bool ignoreTagsCase = IGNORETAGSCASE) : base() => KnownTags = new FakeKnownTags(ignoreTagsCase);
    protected FakeEngine(FakeEngine other) : base(other) { }
    public override string ToString() => "FakeEngine";
}