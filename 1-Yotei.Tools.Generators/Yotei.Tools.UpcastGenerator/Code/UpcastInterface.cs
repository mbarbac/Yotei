namespace Yotei.Tools.UpcastGenerator;

// ========================================================
public static class UpcastInterface
{
    public static string IUpcast { get; } = nameof(IUpcast);
    public static string IUpcastEx { get; } = nameof(IUpcastEx);

    // Remarks: these interfaces must be public to prevent they being less accessible than the
    // actual types they may wrap over.

    public static string Code(string nsName) => $$"""
        namespace {{nsName}}
        {
            /// <summary>
            /// Used to wrap the types that appears in the inheritance list of a given host type
            /// so that the methods in the inherited one, whose return type is exactly that type,
            /// are upcasted to the host one.
            /// <br/> The standard rules about a class or struct having just one base type, and
            /// that interfaces must come after the base types, apply.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public interface {{IUpcast}}<T> { }

            /// <summary>
            /// Used to wrap the types that appears in the inheritance list of a given host type
            /// so that the methods and properties in the inherited one, whose return or member
            /// type is exactly that type, are upcasted to the host one.
            /// <br/> The standard rules about a class or struct having just one base type, and
            /// that interfaces must come after the base types, apply.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public interface {{IUpcastEx}}<T> { }
        }
        """;
}