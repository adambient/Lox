using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_simple_class
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"
class Cake {
  taste() {
    var adjective = ""delicious"";
    print ""The "" + this.flavor + "" cake is "" + adjective + ""!"";
  }
}

var cake = Cake();
cake.flavor = ""German chocolate"";
cake.taste(); // Prints ""The German chocolate cake is delicious!"".
";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_result = () =>
            console.GetStdOut().ShouldEqual("The German chocolate cake is delicious!\r\n");
    }
}
