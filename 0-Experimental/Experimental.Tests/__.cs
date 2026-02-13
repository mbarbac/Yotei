namespace Experimental.Tests;
/*
// ========================================================
//[Enforced]
public static class Test_
{
    public record Options
    {
        public bool UseName
        {
            get;
            init
            {
                HeadOptions = HeadOptions is null ? null : HeadOptions with { };
                field = value;
            }
        }

        public Options? HeadOptions { get; init; }

        enum Mode { Empty, Default, Full };
        Options(Mode mode)
        {
            switch (mode)
            {
                case Mode.Full:
                    HeadOptions = this;
                    break;
            }
        }

        public Options() : this(Mode.Empty) { }
        public static Options Full { get; } = new(Mode.Full);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        var source = new Options();
        Assert.False(source.UseName);
        Assert.Null(source.HeadOptions);

        var target = source with { UseName = true };
        Assert.True(target.UseName);
        Assert.Same(source.HeadOptions, target.HeadOptions);
    }

    //[Enforced]
    [Fact]
    public static void Test_Full()
    {
        var source = Options.Full;
        Assert.False(source.UseName);
        Assert.Same(source, source.HeadOptions);

        var target = source with { UseName = true };
        Assert.True(target.UseName);
        Assert.NotNull(target.HeadOptions);
        Assert.NotSame(source.HeadOptions, target.HeadOptions);
    }
}*/