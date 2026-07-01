using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Interpreter))]
    public class when_interpreting_super_super_at_top_level
    {
        static TestConsoleWriter console = new();
        static Lox lox = new Lox(console);
        static string? source;

        Establish context = () =>
            source = @"super.foo(""bar""); // Error at 'super': Can't use 'super' outside of a class.
super.foo; // Error at 'super': Can't use 'super' outside of a class.";

        Because of = () =>
            lox.Run(source);

        It should_return_correct_error = () =>
            console.GetStdErr().ShouldEqual(
"[line 1] Error at 'super':Can't use 'super' outside of a class.[line 2] Error at 'super':Can't use 'super' outside of a class."
);
    }
}
