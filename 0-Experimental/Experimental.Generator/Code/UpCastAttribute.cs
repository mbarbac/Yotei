namespace Experimental.Generator;

// =========================================================
/// <summary>
/// Decorates types for which the inherited method and properties whose return type is the one
/// specified are upcasted to the decorated one. Several attributes can be used to upcast many
/// base types.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
    Inherited = false,
    AllowMultiple = true)]
public class UpCastAttribute<T> : Attribute { }