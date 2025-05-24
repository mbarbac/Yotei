namespace Yotei.ORM.Relational.FakeDB;

// ========================================================
public static partial class DB
{
    /// <summary>
    /// The authoritative list of elements.
    /// </summary>
    public static ImmutableList<TalentDTO> Talents
    {
        get
        {
            if (_Talents == null)
            {
                _Talents = _TalentsShort.ToImmutableList();

                if (IsLongDatabase)
                    _Talents = _Talents.AddRange(_TalentsLong);
            }
            return _Talents;
        }
    }

    static ImmutableList<TalentDTO> _Talents = null!;

    readonly static TalentDTO[] _TalentsShort = new[]
    {
        new TalentDTO() { Id = "sales", Description = "Sales Talent" },
        new TalentDTO() { Id = "tech", Description = "Tech Talent" },
        new TalentDTO() { Id = "mktg", Description = "Marketing Talent" },
        new TalentDTO() { Id = "hr", Description = "Human Resources Talent" }
    };

    readonly static TalentDTO[] _TalentsLong = [];
}