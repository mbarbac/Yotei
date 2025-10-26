namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public class Test_ArrayExtensions
{
    class Persona(string name) : ICloneable
    {
        public string Name { get; } = name.ThrowWhenNull();
        public override string ToString() => Name;
        public Persona Clone() => new(Name);
        object ICloneable.Clone() => Clone();

        public static Persona[] Generate(int num)
        {
            var items = new Persona[num];
            for (int i = 0; i < num; i++) items[i] = new($"Name{i}");
            return items;
        }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_TypedEnumeration()
    {
        var num = 100;
        var source = Persona.Generate(num);

        num = 0; foreach (var item in source.AsEnumerable()) num++;
        Assert.Equal(100, num);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Clone_Standard()
    {
        var source = Array.Empty<Persona>();
        var target = (Persona[])source.Clone();
        Assert.NotSame(source, target);

        var num = 100;
        source = Persona.Generate(num);
        target = (Persona[])source.Clone();

        Assert.NotSame(source, target);
        for (int i = 0; i < num; i++) Assert.Same(source[i], target[i]);
    }

    //[Enforced]
    [Fact]
    public void Test_Clone_Deep()
    {
        var source = Array.Empty<Persona>();
        var target = source.Clone(false); Assert.NotSame(source, target);
        target = source.Clone(true); Assert.NotSame(source, target);

        var num = 100;
        source = Persona.Generate(num);
        target = source.Clone(false);
        Assert.NotSame(source, target);
        for (int i = 0; i < num; i++) Assert.Same(source[i], target[i]);

        target = source.Clone(true);
        Assert.NotSame(source, target);
        for (int i = 0; i < num; i++)
        {
            Assert.NotSame(source[i], target[i]);
            Assert.Equal(source[i].Name, target[i].Name);
        }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Clear()
    {
        var source = Array.Empty<Persona>();
        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);

        var num = 100;
        source = Persona.Generate(num);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(source.Length, target.Length);
        for (int i = 0; i < num; i++) Assert.Null(target[i]);
    }
}