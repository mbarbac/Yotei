namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Use to decorate the members (properties and interfaces) of a not C# record type for which a
/// 'With[Name]' method will be emitted to emulate the 'with' keyword, provided such methods are
/// not explicitly declared or implemented.
/// <br/> Not-interface types must implement a private or protected copy constructor.
/// <br/> C# record types are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute : Attribute
{
    /// <summary>
    /// Instructs the generator to produce methods whose return type is the first interface of
    /// the host type that ultimately has decorated the member. Otherwise, the return type of
    /// these methods is the host type itself.
    /// <br/> The default value of this property is <c>false</c>.
    /// <br/> This property is ignored if the host type is an interface.
    /// </summary>
    public bool ReturnInterface { get; set; }

    /// <summary>
    /// If <c>true</c> instructs the generator not to emit a virtual-alike method.
    /// <br/> The default value of this property is <c>false</c>.
    /// <br/> This property is ignored if the host type is an interface.
    /// </summary>
    public bool PreventVirtual { get; set; }
}