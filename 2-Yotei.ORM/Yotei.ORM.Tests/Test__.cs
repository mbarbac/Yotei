namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_
{
    //[Enforced]
    [Fact]
    public static void Test()
    {
        IEnumerableCommand cmd = default!;
        cmd.Select(x => x.name);
        cmd.Select(x => new { Id = x.Emp.Id });
    }
}