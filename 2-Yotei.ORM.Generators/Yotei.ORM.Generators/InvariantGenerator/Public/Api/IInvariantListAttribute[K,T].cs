#nullable enable
namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates types where the 'IInvariantList[K,T]' interface will be implemented, including its
/// 'Clone' capabilities.
/// <br/> Includes the interface in the base list if needed.
/// <br/> Derived types must maintain base compatibility.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute<K, T> : Attribute
{
    /// <summary>
    /// The type of the keys the collection elements are associated with.
    /// </summary>
    public Type KType => typeof(K);

    /// <summary>
    /// The type of the collection's elements.
    /// </summary>
    public Type TType => typeof(T);

    /// <summary>
    /// If not null, specifies the return type of the generated method. Otherwise, the decorated
    /// host type will be used by default.
    /// <br/> Derived types must maintain base compatibility.
    /// </summary>
    public Type? ReturnType { get; set; } = null;
}