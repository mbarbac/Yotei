namespace Experimental;

// ========================================================
public static class Net8Reflection
{
    public class Persona
    {
        public Persona(string name, int age) { Name = name; _Age = age; }
        public override string ToString() => $"Name:{Name}, Age:{_Age}";
        public int AgeGetter() => _Age;

        public string Name { get; set; }
        private readonly int _Age = 0;
    }
}