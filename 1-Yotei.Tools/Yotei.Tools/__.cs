namespace Yotei.Tools;

// ========================================================
public class TypeFake
{
    public void MyMethod()
    {
        throw new DuplicateException();
    }
}