namespace Experimental.WeakObjects;

// ========================================================
//[Enforced]
public static class Test_WeakHolder
{
    const int ALIVE_LOOPS = 3;
    const int RELEASE_LOOPS = 10;
    const int ALIVE_MS = 550;
    const int SLEEP_MS = 400;

    public class Persona
    {
        public Persona(string name) => Name = name;
        public override string ToString() => Name;
        public string Name { get; }
    }

    //[Enforced]
    [Fact]
    public static void Validate()
    {
        var holder = Create();
        KeepAlive(holder);
        Release(holder);
    }

    // ----------------------------------------------------

    static WeakHolder<Persona> Create()
    {
        Console.WriteLine("\n> Creating...");

        var target = new Persona("Cervantes");
        var holder = new WeakHolder<Persona>(target);
        return holder;
    }

    static void KeepAlive(WeakHolder<Persona> holder)
    {
        Console.WriteLine("\n> Keeping Alive...");

        for (int i = 0; i < ALIVE_LOOPS; i++)
        {
            Console.WriteLine($"- Loop: #{i}");

            holder.Hydrate();
            if (!holder.IsValid) throw new InvalidOperationException("Not alive...");

            GC.Collect(); GC.WaitForPendingFinalizers();
            GC.Collect();
            Thread.Sleep(SLEEP_MS);
        }
    }

    static void Release(WeakHolder<Persona> holder)
    {
        Console.WriteLine("\n> Releasing...");

        for (int i = 0; i < RELEASE_LOOPS; i++)
        {
            Console.WriteLine($"- Loop: #{i}");

            holder.Weaken(TimeSpan.FromMilliseconds(ALIVE_MS));
            if (!holder.IsValid) break;

            GC.Collect(); GC.WaitForPendingFinalizers();
            GC.Collect();
            Thread.Sleep(SLEEP_MS);
        }

        if (holder.IsValid) throw new InvalidOperationException("Still alive...");
    }
}