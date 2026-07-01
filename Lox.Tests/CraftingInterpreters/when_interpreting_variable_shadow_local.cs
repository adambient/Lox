using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_variable_shadow_local
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"{
  var a = ""local"";
  {
    var a = ""shadow"";
    print a; // expect: shadow
  }
  print a; // expect: local
}
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"shadow\r\nlocal\r\n"
);
    }
}
