#nullable enable
namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates types where the 'InvariantList[K,T]' class will be used as its base one, and its
/// methods reimplemented, including its 'Clone' capabilities.
/// <br/> Includes the collection in the base list if needed.
/// <br/> Regular types (not abstract ones) must implement a copy constructor.
/// <br/> Records are not supported.
/// <br/> Derived types must maintain base compatibility.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute<K, T> : Attribute
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