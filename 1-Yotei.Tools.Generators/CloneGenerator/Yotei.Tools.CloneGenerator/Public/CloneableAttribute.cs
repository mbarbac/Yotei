namespace Yotei.Tools.CloneGenerator;

// ========================================================
/// <summary>
/// Used to decorate types for which a parameterless 'Clone()' method will be generated.
/// <br/> Regular types must implement a copy constructor.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute : Attribute
{
    /// <summary>
    /// The return type of the generated method. Its default value of '<c>null</c>' means that
    /// this type shall be the type of the decorated host.
    /// </summary>
    public Type? ReturnType { get; set; }

    /// <summary>
    /// Whether to generate a virtual-alike method, or not. The default value of this property
    /// is considered to be '<c>true</c>'.
    /// </summary>
    public bool UseVirtual { get; set; }
}

// ========================================================
/// <summary>
/// Used to decorate types for which a parameterless 'Clone()' method will be generated.
/// <br/> Regular types must implement a copy constructor.
/// </summary>
/// <typeparam name="T">The return type of the generated method.</typeparam>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = false)]
public class CloneableAttribute<T> : Attribute
{
    /// <summary>
    /// Whether to generate a virtual-alike method, or not. The default value of this property
    /// is considered to be '<c>true</c>'.
    /// </summary>
    public bool UseVirtual { get; set; }
}