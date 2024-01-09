namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a command that suppports paging functionality.
/// </summary>
public partial interface IPagedCommand : ICommand
{
    /// <summary>
    /// Determines if this instance support native paging, or rather it has to be emulated by
    /// the framework.
    /// </summary>
    bool NativePaging { get; }

    /// <summary>
    /// The number of records to skip, or a negative value if this property is ignored.
    /// </summary>
    [WithGenerator]
    int Skip { get; set; }

    /// <summary>
    /// The number of records to take, or a negative value if this property is ignored.
    /// </summary>
    [WithGenerator]
    int Take { get; set; }
}