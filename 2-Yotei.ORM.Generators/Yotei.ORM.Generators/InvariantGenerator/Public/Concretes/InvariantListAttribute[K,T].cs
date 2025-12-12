namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// Decorates class types for which 'InvariantList[K,T]' will be used as its base one.
/// Use '<see cref="IsNullable{T}"/>' if any is a nullable one.
/// The inherited methods will be upcasted to the decorated host type, unless otherwise specified.
/// Clone functionality is handled automatically.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InvariantListAttribute<K, T> : Attribute
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public InvariantListAttribute() { }
    
    // ----------------------------------------------------

    /// <summary>
    /// The type of the keys of the implemented collection.
    /// </summary>
    public Type KType => typeof(K);

    /// <summary>
    /// The type of the elements of the implemented collection.
    /// </summary>
    public Type TType => typeof(T);

    /// <summary>
    /// The return type of the upcasted methods.
    /// <br/> The default value of this setting is the decorated type itself.
    /// </summary>
    public Type ReturnType { get; set; } = null!;
}