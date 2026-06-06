namespace Yotei.ORM.Relational.TestDB;

// ========================================================
public static partial class DB
{
    /// <summary>
    /// The authoritative list of elements.
    /// </summary>
    public static ImmutableList<CountryDTO> Countries
    {
        get
        {
            if (field == null)
            {
                field = [.. _CountryShort];
                if (LongEnvironment) field.AddRange(_CountryLong);
            }
            return field;
        }
        set;
    }

    // ----------------------------------------------------

    readonly static CountryDTO[] _CountryShort =
    [
        new CountryDTO() { Id = "ca", Name = "Canada", RegionId = "111" },
        new CountryDTO() { Id = "usx", Name = "US Administration", RegionId = "112" },
        new CountryDTO() { Id = "us", Name = "United States of America", RegionId = "113" },
        new CountryDTO() { Id = "mx", Name = "Mexico", RegionId = "120" },
        new CountryDTO() { Id = "uk", Name = "United Kingdom", RegionId = "210" },
        new CountryDTO() { Id = "ie", Name = "Ireland", RegionId = "210" },
        new CountryDTO() { Id = "es", Name = "España", RegionId = "221" },
        new CountryDTO() { Id = "pt", Name = "Portugal", RegionId = "221" },
        new CountryDTO() { Id = "it", Name = "Italy", RegionId = "221" },
        new CountryDTO() { Id = "ae", Name = "United Arab Emirates", RegionId = "223" },
        new CountryDTO() { Id = "za", Name = "Republic of South Africa", RegionId = "225" },
        new CountryDTO() { Id = "jp", Name = "Japan", RegionId = "310" }
    ];

    readonly static CountryDTO[] _CountryLong = [];
}