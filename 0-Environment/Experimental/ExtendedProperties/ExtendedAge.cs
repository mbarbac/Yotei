namespace Experimental.ExtendedProperties;

// ========================================================
// The sample class for which an extension property is to be defined.
public sealed record Person(string Name);

// ========================================================
public static class PersonExtensions
{
    extension(Person source)
    {
        // The extended property. In this case is an 'int' but it can be of any type. If it is
        // o a value one, it must be boxed (albeit StrongBox<T> also carries benefits for class
        // alike ones. In this case, we use the lambda to provide an initial value when needed.
        public int Age
        {
            get => _Ages.GetValue(source, static _ => new(1)).Value; // '1' for debug purposes
            set => _Ages.AddOrUpdate(source, new(value));
        }
    }

    // TValue must be of a reference type, hence why we StrongBox<T> it, which also bring features
    // such as sharing the value without copying, mutating in place, etc. In addition, if T were a
    // class, it prevents GC to collect the carried reference while the entry in the CWT is alive.
    static readonly ConditionalWeakTable<Person, StrongBox<int>> _Ages = [];
    
    // For debug purposes.
    public static int Size => _Ages.Count();

    // For debug purposes.
    public static void Clear() => _Ages.Clear();
}