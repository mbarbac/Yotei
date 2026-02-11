namespace Yotei.Tools.CoreGenerator;

// ========================================================
internal static partial class NamedTypeSymbolExtensions
{
    extension(INamedTypeSymbol source)
    {
        /// <summary>
        /// Tries to find a copy constructor declared in the given named type. By default this
        /// method works in strict mode, meaning that the constructor unique parameter must be
        /// the same as the type itself. In not-strict mode, it is enough if that parameter can
        /// be assigned this this type.
        /// </summary>
        /// <param name="strict"></param>
        /// <returns></returns>
        public IMethodSymbol? FindCopyConstructor(bool strict = true)
        {
            var methods = source.GetMembers().OfType<IMethodSymbol>().Where(static x =>
                x.MethodKind == MethodKind.Constructor &&
                x.IsStatic == false &&
                x.Parameters.Length == 1)
                .ToDebugArray();

            var comparer = SymbolEqualityComparer.Default;
            var method = methods.FirstOrDefault(x => comparer.Equals(source, x.Parameters[0].Type));
            if (method is null && !strict)
                method = methods.FirstOrDefault(x => source.IsAssignableTo(x.Parameters[0].Type));

            return method;
        }
    }
}