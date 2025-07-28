namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates interfaces where the 'IInvariantList{K,T}' one is to be implemented. The return
/// types are adjusted to the type of the host interface.
/// <br/> 'Clone()' capability is also added automatically if possible.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IInvariantListAttribute<K, T> : Attribute
{
    /// <summary>
    /// The type of the keys of the implemented invariant list interface.
    /// </summary>
    public Type KType => typeof(K);

    /// <summary>
    /// The type of the elements of the implemented invariant list interface.
    /// </summary>
    public Type TType => typeof(T);
}