namespace Yotei.Tools.WithGenerator;

// =========================================================
/// <summary>
/// Emulates the 'with' keyword for non-record types by generating a 'With[name](value)' method
/// for the members (properties and fields) decorated with this attribute. That method returns a
/// new instance of the host type where the old value has been replaced by the given one.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute : Attribute
{
    /// <summary>
    /// If <c>true</c> instructs the generator to not produce virtual-alike methods.
    /// </summary>
    public bool PreventVirtual { get; set; }
}