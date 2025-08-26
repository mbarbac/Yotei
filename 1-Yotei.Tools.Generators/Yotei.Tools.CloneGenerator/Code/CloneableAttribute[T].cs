namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// Used to decorate host types for which a 'Clone()' method will be declared or implemented.
/// <br/> The generic argument specifies the type used as the return one of the generated methods.
/// <br/> Not-interface types must implement a protected or private copy constructor.
/// <br/> C# record types are not supported.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute<T> : Attribute
{
    /// <summary>
    /// If used specifies whether the generated methods are virtual-alike ones, or not. If not
    /// used, then the generator tries to generate virtual-alike ones.
    /// </summary>
    public bool VirtualMethod { get; set; }
}