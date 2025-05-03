namespace Yotei.ORM.Records;

// ========================================================
[Cloneable]
public partial interface ICommand
{
    ICommandInfo GetCommandInfo();
}