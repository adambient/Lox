using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_super_in_top_level_function
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"  super.bar(); // Error at 'super': Can't use 'super' outside of a class.
fun foo() {
}";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"[line 1] Error at 'super':Can't use 'super' outside of a class."
);
    }
}
