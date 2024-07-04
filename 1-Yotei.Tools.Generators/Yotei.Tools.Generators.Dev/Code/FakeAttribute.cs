namespace Yotei.Tools.Generators.Dev;

// =========================================================
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public class FakeAttribute : Attribute { }

// =========================================================
//[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
//public class FakeAttribute<T> : Attribute { }

// =========================================================
//[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
//public class FakeAttribute : Attribute
//{
//    public FakeAttribute(Type mandatory) { Mandatory = mandatory; Optional = ""; }
//    public Type Mandatory { get; }
//    public string Optional { get; set; }
//}