namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// Use to decorate types for which a 'Clone()' method will be declared or implemented, provided
/// such method is not explicitly declared or implemented.
/// <br/> Not-interface types must implement a private or protected copy constructor.
/// <br/> C# record types are not supported.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute : Attribute
{
    /// <summary>
    /// Instructs the generator to produce methods whose return type is the decorated one. The
    /// default behavior is to use as the return type the first explicit interface that is itself
    /// a cloneable one, if such is possible.
    /// <br/> This property is ignored if the host type is an interface.
    /// </summary>
    public bool ReturnDecorated { get; set; }

    /// <summary>
    /// If <c>true</c> instructs the generator not to emit a virtual-alike method.
    /// <br/> The default value of this property is <c>false</c>.
    /// <br/> This property is ignored if the host type is an interface.
    /// </summary>
    public bool PreventVirtual { get; set; }

    /// <summary>
    /// If <c>true</c>, add to the type a <see cref="ICloneable"/> interface if it was not among
    /// the type's interfaces yet.
    /// <br/> The default value of this property is <c>false</c>.
    /// </summary>
    public bool AddICloneable { get; set; }
}