namespace Yotei.ORM.Generators.Invariant;

// ========================================================
/// <summary>
/// Decorates types for which 'InvariantList[K,T]' will be used as their base one. Unless abstract,
/// decorated hosts must implement a copy constructor.
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
    /// The type of the keys in the collection.
    /// </summary>
    public Type KType => typeof(K);

    /// <summary>
    /// The type of the elements in the collection.
    /// </summary>
    public Type TType => typeof(T);

    /// <summary>
    /// The return type of the generated methods.
    /// <br/> The default value of this setting is the type of the decorated host.
    /// </summary>
    public Type ReturnType { get; set; } = null!;
}