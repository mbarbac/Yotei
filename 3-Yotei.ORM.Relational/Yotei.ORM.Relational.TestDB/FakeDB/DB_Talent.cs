namespace Yotei.ORM.Relational.TestDB;

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
            if (field == null)
            {
                field = [.. _TalentShort];
                if (LongEnvironment) field.AddRange(_TalentLong);
            }
            return field;
        }
        set;
    }

    // ----------------------------------------------------

    readonly static TalentDTO[] _TalentShort =
    [
        new TalentDTO() { Id = "sales", Description = "Sales Talent" },
        new TalentDTO() { Id = "tech", Description = "Tech Talent" },
        new TalentDTO() { Id = "mktg", Description = "Marketing Talent" },
        new TalentDTO() { Id = "hr", Description = "Human Resources Talent" }
    ];

    readonly static TalentDTO[] _TalentLong = [];
}