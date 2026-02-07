#pragma warning disable IDE0065

namespace ns1.ns2
{
    namespace ns3.ns4
    {
        using Yotei.Tools.WithGenerator;

        //[InheritsWith]
        public partial class Tp1<T> { }

        [InheritsWith]
        public partial class Tp2<T> : Tp1<T>
        {
            public T? Name { get; }
        }
    }
}