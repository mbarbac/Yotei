using TPair = System.Collections.Generic.KeyValuePair<string, int>;

namespace Yotei.Tools.Generators.Tests.WithGenerator
{
    // ====================================================
    [Enforced]
    public static partial class Test_Miscelanea
    {
        //[Enforced]
        [Fact]
        public static void Test() { }

        // ================================================

        public partial interface IFoo<T>
        {
            [WithGenerator]
            string Name { get; }
        }

        [WithGenerator]
        public partial interface IBar<K, T> : IFoo<string>
        {
            [WithGenerator]
            TPair? MyPair { get; }
        }
    }
}