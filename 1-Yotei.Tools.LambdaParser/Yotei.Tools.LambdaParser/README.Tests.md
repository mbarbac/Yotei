# Yotei Dynamic Lambda Expression Parser Tests

# Testing Yotei Lambda Parser

For unknown reasons, when running the Yotei Lambda Parser sone test cases fail, but only when using XUnit.

In addition, this doesn't happen in a consistent way: if the any test is
executed individually, it passes, but when executing them all as a whole,
some of them fail, although not always the same ones.

Yotei uses a test runner platform (named 'Runner', what else?), which is
a console application that discovers the test cases and run them. When it
is used for the Lambda Parser test cases, no one fails.

## Speculations

From one hand, it seems XUnit uses custom execution context instances for
its own needs. From the other hand, Yotei Lambda Parser tweaks the DLR
execution environment so that it doesn't catch the results it produces
in order to render a brand new correct one each time it is invoked.

Although I was unable to intercept XUnit calls into the DLR, my personal
speculation is that there is some sort of interaction between the XUnit
execution environment and the DLR.