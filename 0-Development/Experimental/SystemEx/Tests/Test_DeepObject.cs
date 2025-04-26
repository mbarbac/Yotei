using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Experimental.SystemEx;

// ========================================================
//[Enforced]
public static class Test_DeepObject
{
    //[Enforced]
    [Fact]
    public static void Validate_Standard_Scenarios()
    {
        string str;
        dynamic obj;

        obj = new DeepObject();
        obj["FirstName"] = "James";
        obj["LastName"] = "Bond";
        str = obj.ToString();
        Assert.Equal("(.[([FirstName]='James'), ([LastName]='Bond')])", str);

        obj = new DeepObject();
        obj.Age = 50;
        obj.Roles[0] = "Spy";
        obj.Roles[1] = "Lover";
        obj.Books["1965", 5] = "Thunderball";
        obj.Gear.Gun = true;
        obj.Gear.Knife = null;
        str = obj.ToString();
        Assert.Equal(
           "(.[(Age='50'), " +
           "(Roles[([0]='Spy'), ([1]='Lover')]), " +
           "(Books[([1965, 5]='Thunderball')]), " +
           "(Gear[(Gun='True'), " +
           "(Knife='NULL')])])",
           str);
    }

    //[Enforced]
    [Fact]
    public static void Validate_Null_Indexes()
    {
        string str;
        dynamic obj;

        obj = new DeepObject();
        obj[null] = "James";
        str = obj.ToString();
        Assert.Equal("(.[([NULL]='James')])", str);

        obj[null, null] = "Bond";
        str = obj.ToString();
        Assert.Equal("(.[([NULL]='James'), ([NULL, NULL]='Bond')])", str);
    }
}