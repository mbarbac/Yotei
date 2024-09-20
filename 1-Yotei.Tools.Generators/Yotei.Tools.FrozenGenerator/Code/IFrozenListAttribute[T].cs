namespace Yotei.Tools.FrozenGenerator;

// =========================================================
/// <summary>
/// Implements the 'IFrozenList{T}' on the decorated interface so that its inherited members
/// are upcasted having their new return type being the decorated interface.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class IFrozenListAttribute<T> : Attribute { }