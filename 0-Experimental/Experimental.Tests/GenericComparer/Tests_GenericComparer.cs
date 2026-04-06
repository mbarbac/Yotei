using System.ComponentModel.DataAnnotations;

namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_GenericComparer
{
    public class Car(string maker, string model, int year)
    {
        [SuppressMessage("", "IDE0044")]
        string PartNumber = string.Empty;

        public string Maker { get; } = maker.NotNullNotEmpty(true);
        public string Model { get; } = model.NotNullNotEmpty(true);
        public int Year { get; } = year > 1900 ? year : throw new ArgumentException("Invalid year");
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "PartNumber")]
    private static extern ref string ThePartNumber(Car car);

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Public_Elements()
    {
        var car1 = new Car("Toyota", "Yaris", 2022);
        var car2 = new Car("Toyota", "Yaris", 2022);
        var car3 = new Car("Toyota", "Yaris", 2025);

        Assert.False(car1.Equals(car2));
        Assert.False(car2.Equals(car3));
        Assert.False(car3.Equals(car1));

        var comparer = new GenericComparer<Car>();

        Assert.True(comparer.Equals(car1, car2));
        Assert.False(comparer.Equals(car1, car3));
        Assert.False(comparer.Equals(car2, car3));
    }

    //[Enforced]
    [Fact]
    public static void Test_Private_Elements()
    {
        var car1 = new Car("Toyota", "Yaris", 2022);
        var car2 = new Car("Toyota", "Yaris", 2022);

        var comparer = new GenericComparer<Car>(usePrivateFields: true);
        Assert.True(comparer.Equals(car1, car2));

        ThePartNumber(car1) = "part1"; Assert.Equal("part1", ThePartNumber(car1));
        ThePartNumber(car2) = "part2"; Assert.Equal("part2", ThePartNumber(car2));

        Assert.False(comparer.Equals(car1, car2));
    }
}