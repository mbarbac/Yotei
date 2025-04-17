namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a multi-part database identifier.
/// </summary>
[IInvariantList<AsNullable<string>, IIdentifierPart>]
public partial interface IIdentifierChain : IIdentifier
{
}