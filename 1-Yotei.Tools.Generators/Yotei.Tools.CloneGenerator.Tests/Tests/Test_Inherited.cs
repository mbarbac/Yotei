namespace Yotei.Tools.CloneGenerator.Tests.Inherited
{
    // ====================================================
    public class PrivateClone(string name)
    {
        public string Name { get; set; } = name;

        [SuppressMessage("", "IDE0051")]
        PrivateClone Clone() => new(Name);
    }

    [Cloneable]
    public partial class PrivateCloneBis(string name) : PrivateClone(name)
    {
        PrivateCloneBis(PrivateCloneBis source) : this(source.Name) { }
    }

    // ====================================================
    //[Enforced]
    public static class Test_Inherited
    {
        //[Enforced]
        [Fact]
        public static void Test_PrivateClone()
        {
            var type = typeof(PrivateClone);
            var method = type.GetMethod("Clone", BindingFlags.NonPublic | BindingFlags.Instance);
            var source = new PrivateClone("Bond");
            var target = (PrivateClone)(method!.Invoke(source, null))!;
            Assert.Equal(source.Name, target.Name);

            type = typeof(PrivateCloneBis);
            method = type.GetMethod("Clone", BindingFlags.NonPublic | BindingFlags.Instance);
            source = new PrivateCloneBis("Bond");
            target = (PrivateCloneBis)(method!.Invoke(source, null))!;
            Assert.Equal(source.Name, target.Name);
            Assert.True(method.IsPrivate);
        }
    }

    // ====================================================

    [Cloneable] public partial interface IA { }

    [Cloneable]
    public partial class TA : IA
    {
        public TA() { }
        public TA(TA _) { }
    }

    [Cloneable]
    public partial class TB : TA
    {
        public TB() { }
        public TB(TB source) : base(source) { }
    }

    [Cloneable]
    public partial class TC : TB
    {
        public TC() { }
        public TC(TC source) : base(source) { }
    }
}