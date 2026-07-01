using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_variable_in_middle_of_block
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"{
  var a = ""a"";
  print a; // expect: a
  var b = a + "" b"";
  print b; // expect: a b
  var c = a + "" c"";
  print c; // expect: a c
  var d = b + "" d"";
  print d; // expect: a b d
}
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual(
"a\r\na b\r\na c\r\na b d\r\n"
);
    }
}
