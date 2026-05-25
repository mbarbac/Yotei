#nullable enable

namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// Decorates types for which a <see cref="ICloneable.Clone"/> method will be generated.
/// <br/> Regular types (not interface or abstract ones) must implement a copy constructor.
/// <br/> The generic type of the attribute becomes the return type of the generated method.
/// Derived types must maintain base compatibility.
/// <br/> Records are not supported.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute<T> : Attribute
{
    /// <summary>
    /// Determines if the generated method will be a virtual-alike one, or not. If not used, the
    /// default value of this property is true.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public bool UseVirtual { get; set; } = true;
}