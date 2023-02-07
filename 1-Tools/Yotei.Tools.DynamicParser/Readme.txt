/**********************************************************
== NEEDED TO PREVENT COMPILATION ERRORS ==

CONSIDERATIONS:
=================================================

- For performance reasons, the DLR is built to cache the logic that produces the results
  of the dynamic operations, using the type of the call site and the type of its arguments.
  This rules cannot be used by DynamicParser, because it needs to produce brand new results
  each time it parses a new dynamic lambda expression.

  To prevent this, DynamicParser keeps a version property for each result, so that only the
  last one is considered valid. When it is not, then DynamicParser invalidates the cache
  entry in the DLR enforcing it to create a new rule.

  CAVEAT: This solution may affect the DLR if it is used for other purposes along with Dynamic
  Parser. This should be a very rare case, though. TODO: Investigate how/why when executing
  tests under the test execution engine then they are executing in a context that don't reuse
  DLR rules.

- The value of the properties in the DynamicNode classes are returned when their names are
  the same as the names specified in the dynamic lambda expressions. This is by design because
  when this happens the DLR identifies they exist and so no dynamic operations are triggered.
  Instead of preventing that names to be used (ie: 'Id'), I have used long and convoluted
  property names that are easily identified.

TESTING:
=================================================

- To validate the rules are not reused, along with using the same types we also reused the
  same names in the dynamic lambda expressions over and over again. When DEBUG TRACING is
  enabled, we can see how these rules are invalidated.

- We also need to take into consideration the engine used for running the tests (ie: Xunit).
  I've found that when running the tests in parallel, then each of them is executed in a
  kind of private context that does not resuse DLR rules - but this is not the standard
  case, where these rules will be reused. For this reasons, my test environment has a program
  that identifies the test methods and runs them in order, under the same context, and so
  resuing (and invalidating) the DLR rules as expected.

LIMITATIONS:
=================================================

- The unary POST INCREMENT / DECREMENT operators do not work (their PRE ones do).
  They are internally converted to a setter of the node to its incremented one:
  ie: (x.Alpha = (Increment x.Alpha))
  but, then that original node is the one used afterwards, and not the setter one. So we are
  losing the operator triggered dynamic operations. I've found no way to prevent this to
  happen.

- Something similar happens with CONVERT/CAST operations. In this case, I have opted to
  solve the situation by intercepting the argument-level special method 'Cast', which is
  then parsed as an ad-hoc conversion node. There are two overrides intercepted:

    - x.Cast(type, expr)
    - x.Cast<Type>(expr)

  Both forms must be invoked from the dynamic argument itself. If this is not the case, or
  if the syntax is not correct, then the method is not intercepted ans just parsed as a
  regular one.
 
 == NEEDED TO PREVENT COMPILATION ERRORS ==
**********************************************************/