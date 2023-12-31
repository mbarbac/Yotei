using TPair = System.Collections.Generic.KeyValuePair<string, int>;

namespace Yotei.Tools.Generators.Tests.WithGenerator
{
    // ====================================================
    //[Enforced]
    public static partial class Test_WithNullables
    {
        //[Enforced]
        [Fact]
        public static void Test_With_Method()
        {
            var source = new Temporal();
            Assert.Null(source.MetaPair);

            var target = source.WithMetaPair(new TPair("age", 50));
            Assert.NotNull(target.MetaPair);
            Assert.Equal("age", target.MetaPair.Value.Key);
            Assert.Equal(50, target.MetaPair.Value.Value);
        }

        // ------------------------------------------------
        public partial class Temporal
        {
            public Temporal() { }
            protected Temporal(Temporal source) => MetaPair = source.MetaPair;

            [WithGenerator]
            public TPair? MetaPair { get; init; }
        }
    }
}