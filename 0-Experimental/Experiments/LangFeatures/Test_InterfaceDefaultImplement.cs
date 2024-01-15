#pragma warning disable CA1859

namespace Experiments.LangFeatures;

// ========================================================
//[Enforced]
public static class Test_InterfaceDefaultImplement
{
    public interface IPersona
    {
        public string Language() => "Default";
        public abstract int Volume();
        public abstract IPersona This();
    }
    public class Spanish : IPersona
    {
        public string Language() => "Spanish";
        public int Volume() => 1;
        public IPersona This() => this;
    }

    //[Enforced]
    [Fact]
    public static void Test1()
    {
        IPersona item = new Spanish();
        Assert.Equal("Spanish", Tester1(item));
        Assert.Equal("Spanish", Tester1<IPersona>(item));
        Assert.Equal(1, item.Volume());
    }
    static string Tester1<T>(T item) where T : IPersona => item.Language();

    // ----------------------------------------------------

    // No explicit implementation, inherits 'Language' as public one.
    public class Other : IPersona
    {
        public virtual int Volume() => 2;
        public virtual IPersona This() => this;
    }

    //[Enforced]
    [Fact]
    public static void Test2()
    {
        IPersona item = new Other();
        Assert.Equal("Default", item.Language());
        Assert.Equal(2, item.Volume());
    }
    
    // ----------------------------------------------------

    public class Another : Other
    {
        public override int Volume() => 3;
        public override Another This() => this; // Can override
    }

    //[Enforced]
    [Fact]
    public static void Test3()
    {
        IPersona item = new Another();
        Assert.Equal("Default", item.Language());
        Assert.Equal(3, item.Volume());

        var concrete = new Another();
        Assert.Equal("Default", item.Language());
        Assert.Equal(3, item.Volume());
    }

    // ----------------------------------------------------

    public interface IManager
    {
        public int GetLevel() => Level();

        // This MUST be 'abstract' so that 'GetLevel' invokes the appropriate inherited one.
        // If it has implementation, then this interface implementation is called instead.
        protected abstract int Level();
    }
    public class ManagerOne : IManager
    {
        protected virtual int Level() => 1;
        int IManager.Level() => Level();
    }
    public class ManagerTwo : ManagerOne
    {
        protected override int Level() => 2;
    }

    //[Enforced]
    [Fact]
    public static void Test4()
    {
        IManager item = new ManagerTwo();
        Assert.Equal(2, item.GetLevel());

        item = new ManagerOne();
        Assert.Equal(1, item.GetLevel());
    }
}