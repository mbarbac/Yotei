namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Implements the 'IFrozenList{T}' on the decorated interface.
/// <br/> The inherited members are upcasted so that their new return type is the decorated interface.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IFrozenListAttribute<T> : Attribute { }