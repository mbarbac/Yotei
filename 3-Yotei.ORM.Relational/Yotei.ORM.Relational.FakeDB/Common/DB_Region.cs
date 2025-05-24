namespace Yotei.ORM.Relational.FakeDB;

// ========================================================
public static partial class DB
{
    /// <summary>
    /// The authoritative list of elements.
    /// </summary>
    public static ImmutableList<RegionDTO> Regions
    {
        get
        {
            if (_Regions == null)
            {
                _Regions = _RegionShort.ToImmutableList();

                if (IsLongDatabase)
                    _Regions = _Regions.AddRange(_RegionLong);
            }
            return _Regions;
        }
    }

    static ImmutableList<RegionDTO> _Regions = null!;

    readonly static RegionDTO[] _RegionShort = new[]
    {
        new RegionDTO() {Id = "000", Name = "World" },
        new RegionDTO() {Id = "100", Name = "Americas", ParentId = "000" },
        new RegionDTO() {Id = "110", Name = "North America", ParentId = "100" },
        new RegionDTO() {Id = "111", Name = "Canada", ParentId = "110" },
        new RegionDTO() {Id = "112", Name = "US Administration", ParentId = "110" },
        new RegionDTO() {Id = "113", Name = "US Commercial", ParentId = "110" },
        new RegionDTO() {Id = "120", Name = "Central America", ParentId = "100" },
        new RegionDTO() {Id = "200", Name = "Europe, Middle East & Africa", ParentId = "000" },
        new RegionDTO() {Id = "210", Name = "EMEA North", ParentId = "200" },
        new RegionDTO() {Id = "220", Name = "EMEA South", ParentId = "200" },
        new RegionDTO() {Id = "221", Name = "West Mediterranean", ParentId = "220" },
        new RegionDTO() {Id = "223", Name = "Middle East", ParentId = "220" },
        new RegionDTO() {Id = "225", Name = "Africa", ParentId = "220" },
        new RegionDTO() {Id = "230", Name = "EMEA Central", ParentId = "200" },
        new RegionDTO() {Id = "240", Name = "EMEA East", ParentId = "200" },
        new RegionDTO() {Id = "300", Name = "Asia and Pacific", ParentId = "000" },
        new RegionDTO() {Id = "310", Name = "Japan", ParentId = "300" }
    };

    readonly static RegionDTO[] _RegionLong = [];
}