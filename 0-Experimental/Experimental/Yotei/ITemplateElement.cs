namespace Experimental.Yotei;

// ========================================================
public interface ITemplateKey : IEquatable<ITemplateKey>
{
    string Value { get; }
}
public interface ITemplateElement
{
    ITemplateKey Key { get; }
}