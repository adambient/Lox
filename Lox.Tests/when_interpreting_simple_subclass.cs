using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_simple_subclass
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"
class Doughnut {
  cook() {
    print ""Fry until golden brown."";
  }
}

class BostonCream < Doughnut {
  cook() {
    super.cook();
    print ""Pipe full of custard and coat with chocolate."";
  }
}

BostonCream().cook();
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual("Fry until golden brown.\r\nPipe full of custard and coat with chocolate.\r\n");
    }
}
