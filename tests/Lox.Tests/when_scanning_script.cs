using Machine.Specifications;

namespace Lox.Tests
{
    [Subject(typeof(Scanner))]
    public class when_scanning_script
    {
        static Scanner scanner;
        static List<Token> tokens;

        Establish context = () =>
            scanner = new Scanner(@"if (true) { }");

        Because of = () =>
            tokens = scanner.ScanTokens();

        It should_create_correct_tokens = () =>
            tokens.Select(x => x.Type).ShouldEqual([
                TokenTypeEnum.IF,
                TokenTypeEnum.LEFT_PAREN,
                TokenTypeEnum.TRUE,
                TokenTypeEnum.RIGHT_PAREN,
                TokenTypeEnum.LEFT_BRACE,
                TokenTypeEnum.RIGHT_BRACE,
                TokenTypeEnum.EOF]);
    }
}
