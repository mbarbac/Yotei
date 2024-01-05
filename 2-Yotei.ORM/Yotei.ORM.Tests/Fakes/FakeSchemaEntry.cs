using KnownTags = Yotei.ORM.Records.Code.KnownTags;
using TPair = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable]
[WithGenerator]
public partial class FakeSchemaEntry : Records.Code.SchemaEntry
{
    protected override KnownTags GetKnownTags() => new FakeKnownTags(Engine);

    public FakeSchemaEntry(IEngine engine) : base(engine) { }
    public FakeSchemaEntry(IEngine engine, IEnumerable<TPair> metadata) : base(engine, metadata) { }
    public FakeSchemaEntry(
        IEngine engine,
        IIdentifier identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TPair>? metadata = null) 
        : base(engine, identifier, isPrimaryKey, isUniqueValued, isReadOnly, metadata) { }
    public FakeSchemaEntry(
        IEngine engine,
        string identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TPair>? metadata = null)
        : base(engine, identifier, isPrimaryKey, isUniqueValued, isReadOnly, metadata) { }
    protected FakeSchemaEntry(FakeSchemaEntry source) : base(source) { }
}