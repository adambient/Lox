using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_regression_40
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"fun caller(g) {
  g();
  // g should be a function, not nil.
  print g == nil; // expect: false
}

fun callCaller() {
  var capturedVar = ""before"";
  var a = ""a"";

  fun f() {
    // Commenting the next line out prevents the bug!
    capturedVar = ""after"";

    // Returning anything also fixes it, even nil:
    //return nil;
  }

  caller(f);
}

callCaller();
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"false\r\n"
);
    }
}
