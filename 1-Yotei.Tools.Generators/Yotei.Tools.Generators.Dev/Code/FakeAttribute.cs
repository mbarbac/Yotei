namespace Yotei.Tools.Generators.Dev;

// =========================================================
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public class FakeAttribute : Attribute
{
    public FakeAttribute(Type type) => Type = type;
    public Type Type { get; }
    public string? Description { get; set; }
}