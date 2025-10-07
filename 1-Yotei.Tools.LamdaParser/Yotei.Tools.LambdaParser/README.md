# Yotei Lambda Parser
Expressions do not allow dynamic operations, so we cannot specify arbitrary logic, methods or properties not known to the compiler, or any other late-bound element.

### How it works
The parser invokes the given dynamic lambda delegate using a set of dynamic objects as the lambda arguments. While executing the expression, it "takes note" of the dynamic operations bound to those dynamic arguments, generating a tree of dynamic operations that is the one returned.

### Limitations

<p> Any not-dynamic argument passed to the parser, as well as any value obtained from external elements (such as invoking methods), is captured as a tree tokenm when the tree is generated. If different values would be needed, then you need to parse the expression again.

<p> Some constructions are not supported (but this also happens with the standard expression trees, so this is ok).
