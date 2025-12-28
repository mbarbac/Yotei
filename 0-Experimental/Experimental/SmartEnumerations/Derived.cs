namespace Experimental.Tests;

// ========================================================
public class OrderStatus : SmartEnum<OrderStatus>
{
    public  static readonly OrderStatus Pending = new(0, "Pending");
    public static readonly OrderStatus Processing = new(1, "Processing");
    public static readonly OrderStatus Shipped = new(2, "Shipped");
    public static readonly OrderStatus Delivered = new(3, "Delivered");

    // Private constructor.
    private OrderStatus(int value, string name) : base(value, name) { }

    //public bool CanTransitionTo(OrderStatus next)
    //{
    //    var done = this switch
    //    {
    //        OrderStatus.Pending => true,
    //    };
    //    return done;
    //}
}