namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================

public partial interface IFake2
{
    [Named]
    public string FirstName { get; set; }
}

[Cloneable(ReturnType = typeof(string))]
partial interface IFake2
{
}

[Cloneable<IFake2>]
partial interface IFake2
{
}