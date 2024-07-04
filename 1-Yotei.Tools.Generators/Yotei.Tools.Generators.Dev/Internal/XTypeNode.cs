namespace Yotei.Tools.Generators.Dev
{
    using Yotei.Tools.Generators.Dev.Tests;

    // =========================================================
    /// <inheritdoc cref="TypeNode"/>
    internal class XTypeNode : TypeNode
    {
        public XTypeNode(INode parent, INamedTypeSymbol symbol) : base(parent, symbol) { }
        public XTypeNode(INode parent, TypeCandidate candidate) : base(parent, candidate) { }

        protected override void EmitCore(SourceProductionContext context, CodeBuilder cb)
        {
            var targetT = Symbol.GetMembers().OfType<IMethodSymbol>().Where(x => x.Name == "MethodT").First().ReturnType as INamedTypeSymbol;
            var targetInt = Symbol.GetMembers().OfType<IMethodSymbol>().Where(x => x.Name == "MethodInt").First().ReturnType as INamedTypeSymbol;

            targetT!.Match(typeof(Test_FakeAttribute.Target<>));
            targetT!.Match(typeof(Test_FakeAttribute.Target<int>));

            targetInt!.Match(typeof(Test_FakeAttribute.Target<>));
            targetInt!.Match(typeof(Test_FakeAttribute.Target<int>));

            targetInt!.Match(typeof(Test_FakeAttribute.Target<long>));
        }
    }
}

namespace Yotei.Tools.Generators.Dev.Tests
{
    public class Test_FakeAttribute
    {
        public class Target<T> { }
    }
}