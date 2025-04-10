namespace Yotei.ORM.Tools.Templates;

// ========================================================
public interface IElement { }

// ========================================================
public class Element(string name) : IElement
{
    public string Name { get; set; } = name;
    public override string ToString() => Name ?? string.Empty;
}