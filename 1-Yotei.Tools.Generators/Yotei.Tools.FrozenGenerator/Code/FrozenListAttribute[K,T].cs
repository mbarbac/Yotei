namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Decorates classes for which the 'FrozenList[K, T]' one will be used as their base type,
/// and its related elements upcasted to the decorated type.
/// <br/><br/>
/// By convention, the first interface listed in the type declaration, if any, will be used to
/// implement explicitly the 'FrozenList[K, T]' methods.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class FrozenListAttribute<K, T> : Attribute { }