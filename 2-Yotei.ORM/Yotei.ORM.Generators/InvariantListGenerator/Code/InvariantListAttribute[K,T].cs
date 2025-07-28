namespace Yotei.ORM.Generators;

// ========================================================
/// <summary>
/// Decorates class where the 'InvariantList{K,T}' one is is used as its base one. The return
/// types are adjusted to the first direct interface that is an 'IInvariantList' itself, or to
/// the host class type if not found.
/// <br/> 'Clone()' capability is also added automatically if possible.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute<K, T> : Attribute
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