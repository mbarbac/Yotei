namespace Experimental;

// ========================================================
/// <summary>
/// Represents a enfoced member whose value in the instance being built will be obtained from an
/// external variable, instead of using the original value in the source host type.
/// </summary>
internal class EnforcedMember
{
    public string Name = default!;
    public string ValueName = default!;
}