#pragma warning disable CA1859

namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_IFace_DefaultImplementation
{
    public interface IPersona
    {
        public string Language() => "-"; // Has default implementation.
        public abstract int Volume();
        public abstract IPersona This();
    }

    public class NoLanguage : IPersona
    {
        public IPersona This() => this;
        public int Volume() => 1;
    }

    // No explicit 'Language' implementation, inherits 'base interface' one...
    //[Enforced]
    [Fact]
    public static void Test_NoExplicitImplementation()
    {
        IPersona iface = new NoLanguage();
        Assert.Equal("-", iface.Language());
        Assert.Equal(1, iface.Volume());

        NoLanguage item = new();
        // Assert.Equal("-", item.Language()); => CS1061: no definition for 'Language'
        Assert.Equal(1, item.Volume());
    }

    // ====================================================

    public class Spanish : IPersona
    {
        public string Language() => "Spanish";
        public int Volume() => 2;
        public IPersona This() => this;
    }

    //[Enforced]
    [Fact]
    public static void Test_OverridenImplementation()
    {
        IPersona iface = new Spanish();
        Assert.Equal("Spanish", iface.Language());
        Assert.Equal(2, iface.Volume());

        Spanish item = new();
        Assert.Equal("Spanish", item.Language());
        Assert.Equal(2, item.Volume());
    }

    // ====================================================

    public interface ISpeaker
    {
        public string Language() => "-";
    }

    // Because we are using an interface, we can 'invoke' the base one.
    public interface ISpeakerEx : ISpeaker
    {
        public new string Language() => ((ISpeaker)this).Language() + "Ex";
    }

    // As we do not implement 'Language' in this type, it can only be accessed through either
    // the ISpeaker or ISpeakerEx interface.
    public class SpeakerEx : ISpeakerEx { }

    //[Enforced]
    [Fact]
    public static void Test_UsingBaseImplementation()
    {
        ISpeaker ifacebase = new SpeakerEx();
        Assert.Equal("-", ifacebase.Language());

        ISpeakerEx ifaceex = new SpeakerEx();
        Assert.Equal("-Ex", ifaceex.Language());

        //SpeakerEx item = new();
        //Assert.Equal(..., item.Language());
    }

    // ====================================================

    public interface INeoSpeaker
    {
        public string Language() => NeoLanguage();
        protected static string NeoLanguage() => "-";
    }

    // The only way of calling a base implementation *from a class* is by using a helper method
    // that is invoked by the 'overriding' implementation...
    public class English : INeoSpeaker
    {
        public string Language() => INeoSpeaker.NeoLanguage() + "English";
    }

    //[Enforced]
    [Fact]
    public static void Test_FromClass()
    {
        INeoSpeaker ifacebase = new English();
        Assert.Equal("-English", ifacebase.Language());

        English item = new();
        Assert.Equal("-English", item.Language());
    }
}