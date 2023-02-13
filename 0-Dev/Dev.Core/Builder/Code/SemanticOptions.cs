namespace Dev.Builder;

// ========================================================
/// <summary>
/// The options by which a semantic version value can be increased.
/// </summary>
[Flags]
public enum SemanticOptions
{
    None = 0,
    Major = 0b000_001,
    Minor = 0b000_010,
    Patch = 0b000_100,
    Beta = 0b001_000,
    BetaExpand = 0b010_000,
}