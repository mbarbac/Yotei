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
            for (int i = 0; i < num; i++) items[i] = new(GetName(i));
            return items;
        }
        public static string GetName(int num) => $"Name{num}";
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
    public void Test_IndexOf()
    {
        var name = Persona.GetName(0);
        var item = new Persona(name);
        Persona[] source = [item, ..Persona.Generate(5)];
        Assert.Equal(0, source.IndexOf(item));
        Assert.Equal(0, source.LastIndexOf(item));

        Assert.Equal(0, source.IndexOf(x => x.Name == name));
        Assert.Equal(1, source.LastIndexOf(x => x.Name == name));
        Assert.Equal([0, 1], source.IndexesOf(x => x.Name == name));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Trim()
    {
        var source = Persona.Generate(3);
        var target = source.Trim();
        Assert.Same(source, target);

        source = [null!, null!, .. source, null!, null!];
        target = source.Trim();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Equal(Persona.GetName(0), target[0].Name);
        Assert.Equal(Persona.GetName(1), target[1].Name);
        Assert.Equal(Persona.GetName(2), target[2].Name);
    }

    //[Enforced]
    [Fact]
    public void Test_TrimStart()
    {
        var source = Persona.Generate(3);
        var target = source.TrimStart();
        Assert.Same(source, target);

        source = [null!, null!, .. source, null!, null!];
        target = source.TrimStart();
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Length);
        Assert.Equal(Persona.GetName(0), target[0].Name);
        Assert.Equal(Persona.GetName(1), target[1].Name);
        Assert.Equal(Persona.GetName(2), target[2].Name);
        Assert.Null(target[3]);
        Assert.Null(target[4]);
    }

    //[Enforced]
    [Fact]
    public void TrimEnd()
    {
        var source = Persona.Generate(3);
        var target = source.TrimEnd();
        Assert.Same(source, target);

        source = [null!, null!, .. source, null!, null!];
        target = source.TrimEnd();
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Length);
        Assert.Null(target[0]);
        Assert.Null(target[1]);
        Assert.Equal(Persona.GetName(0), target[2].Name);
        Assert.Equal(Persona.GetName(1), target[3].Name);
        Assert.Equal(Persona.GetName(2), target[4].Name);
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