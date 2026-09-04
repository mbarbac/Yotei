namespace Experimental.ExtendedProperties;

// ========================================================
public sealed record Person(string Name);

// ========================================================
public static class PersonExtensions
{
    static readonly ConditionalWeakTable<Person, StrongBox<string?>> _Nicknames = [];
    public static int Size => _Nicknames.Count();
    public static void Clear() => _Nicknames.Clear();

    extension(Person person)
    {
        public string? Nickname
        {
            get => _Nicknames.TryGetValue(person, out var box) ? box.Value : null;
            set => _Nicknames.AddOrUpdate(person, new(value));
        }
    }
}