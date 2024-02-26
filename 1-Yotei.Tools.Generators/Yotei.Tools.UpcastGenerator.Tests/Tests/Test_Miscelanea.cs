namespace Yotei.Tools.UpcastGenerator.Tests.Miscelanea
{
    public interface IFoo<K, T>
    {
        IFoo<K, T> Legend { get; }
        IFoo<K, T> this[int index] { get; }
    }

    [Upcast(ChangeProperties = true)]
    public partial interface IBar<K> : IFoo<K, string>
    {
        //new IBar<K> Legend { get; }
        //new IBar<K> this[int index] { get; }
    }

    // ----------------------------------------------------

    public class Foo<K, T> : IFoo<K, T>
    {
        public Foo<K, T> this[int index]
        {
            get => _Legend!;
            set => _Legend = value;
        }
        IFoo<K, T> IFoo<K, T>.this[int index] => this[index];

        public Foo<K, T> Legend
        {
            get => _Legend!;
            set => _Legend = value;
        }
        Foo<K, T>? _Legend = null;
        IFoo<K, T> IFoo<K, T>.Legend => Legend;
    }

    //[Upcast(ChangeProperties = true)]
    public partial class Bar<K> : Foo<K, string>
    {
        //public new Bar<K> this[int index]
        //{
        //    get => (Bar<K>)base[index];
        //    set => base[index] = value;
        //}

        //public new Bar<K> Legend
        //{
        //    get => (Bar<K>)base.Legend;
        //    set => base.Legend = value;
        //}
    }

    // ----------------------------------------------------

    public partial class Other<K> : IBar<K>
    {
        public Other<K> this[int index]
        {
            get => _Value;
            set => _Value = value;
        }
        Other<K> _Value = default!;
        IFoo<K, string> IFoo<K, string>.this[int index] => (IFoo<K, string>)_Value;

        public Other<K> Legend { get => _Value; }
        IBar<K> IBar<K>.Legend => Legend;
        IFoo<K, string> IFoo<K, string>.Legend => _Value;
    }

    // ====================================================
}