namespace Yotei.ORM.Relational.FakeDB;

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
            if (_Countries == null)
            {
                _Countries = _CountriesShort.ToImmutableList();

                if (IsLongDatabase)
                    _Countries = _Countries.AddRange(_CountriesLong);
            }
            return _Countries;
        }
    }

    static ImmutableList<CountryDTO> _Countries = null!;

    readonly static CountryDTO[] _CountriesShort = new[]
    {
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
    };

    readonly static CountryDTO[] _CountriesLong = [];
}