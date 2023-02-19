namespace Dev.Builder;

// ========================================================
public static class PackableExtensions
{
    /// <summary>
    /// Orders the given collection of packable projects by their cross references.
    /// </summary>
    /// <param name="packables"></param>
    /// <returns></returns>
    public static ImmutableArray<Packable> OrderByReferences(this IEnumerable<Packable> packables)
    {
        packables = packables.ThrowIfNull();

        var list = packables.Select(x => new Weighted(x)).ToList();
        foreach (var item in list)
        {
            var references = item.Packable.Project.GetReferences();
            foreach (var reference in references)
            {
                var temp = list.Find(x => string.Compare(
                    x.Packable.Name, reference.Name, Program.Comparison) == 0);

                if (temp != null) temp.Count++;
            }
        }

        var order = list.OrderBy(x => x.Count).ToList();
        order.Reverse();
        return order.Select(x => x.Packable).ToImmutableArray();
    }

    class Weighted
    {
        public int Count = 0;
        public Packable Packable = Packable.Empty;
        public Weighted(Packable packable) => Packable = packable.ThrowIfNull();
        public override string ToString() => $"{Packable}: {Count}";
    }
}