namespace Experimental.Tests;

// ========================================================
public class SmartDerived : SmartEnum<SmartDerived>
{
    public static readonly SmartDerived Pending = new(PENDING, "Pending");
    public static readonly SmartDerived Processing = new(PROCESSING, "Processing");
    public static readonly SmartDerived Shipped = new(SHIPPED, "Shipped");
    public static readonly SmartDerived Delivered = new(DELIVERED, "Delivered");

    private SmartDerived(int value, string name) : base(value, name) { }

    public bool CanTransitionTo(SmartDerived next)
    {
        return (Value, next.Value) switch
        {
            (PENDING, PROCESSING) => true,
            (PROCESSING, SHIPPED) => true,
            (SHIPPED, DELIVERED) => true,
            _ => false
        };
    }
    private const int PENDING = 0;
    private const int PROCESSING = 1;
    private const int SHIPPED = 2;
    private const int DELIVERED = 3;
}