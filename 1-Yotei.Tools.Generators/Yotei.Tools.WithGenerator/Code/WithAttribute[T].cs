namespace Yotei.Tools.WithGenerator;

// ========================================================
/// <summary>
/// Used to decorate members (properties and fields) of non-record C# types for which appropiate
/// 'With[Name](value)' methods will be emitted, emulating the 'with' keyword.
/// <br/> The generic argument specifies the type used as the return one of the generated method.
/// <br/> Not-interface types must implement a protected or private copy constructor.
/// <br/> C# record types are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public class WithAttribute<T> : Attribute
{
    /// <summary>
    /// If used specifies whether the generated methods are virtual-alike ones, or not. If not
    /// used, then the generator tries to generate virtual-alike ones.
    /// </summary>
    public bool VirtualMethod { get; set; }
}