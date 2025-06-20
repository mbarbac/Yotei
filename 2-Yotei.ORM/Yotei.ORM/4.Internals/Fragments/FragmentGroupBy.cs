namespace Yotei.ORM.Internals;

/// <summary>
/// Represents the ability of parsing GROUP BY clauses.
/// <br/>- Standard syntax: 'x => x.Element'.
/// </summary>
/// <remarks>
/// GROUP BY clauses accept complex specifications:
/// <br/>- Example: 'GROUP BY EXTRACT(YEAR FROM date), ...'
/// </remarks>
public static partial class FragmentGroupBy { }