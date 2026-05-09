namespace Microsoft.CodeAnalysis;

/// <summary>
/// Used to decorate types (such as marker attributes) that are embedded in the assembly and
/// should not be publicly exposed.
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
internal sealed class EmbeddedAttribute : Attribute { }