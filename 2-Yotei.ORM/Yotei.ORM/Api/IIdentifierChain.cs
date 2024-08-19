namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a multi-part database identifier.
/// </summary>
[IFrozenList<IIdentifierPart>]
public partial interface IIdentifierChain : IIdentifier
{
}