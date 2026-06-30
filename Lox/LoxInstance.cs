namespace Lox
{
    public class LoxInstance
    {
        readonly LoxClass klass;
        readonly Dictionary<string, object?> fields = new();

        public LoxInstance(LoxClass klass)
        {
            this.klass = klass;
        }

        public object? Get(Token name)
        {
            if (fields.ContainsKey(name.Lexeme))
            {
                return fields[name.Lexeme];
            }

            var method = klass.FindMethod(name.Lexeme);
            if (method != null)
            {
                return method.Bind(this);
            }

            throw new RuntimeException(name, $"Undefined proeprty '{name.Lexeme}'.");
        }

        public void Set(Token name, object? value) =>
            fields[name.Lexeme] = value;

        public override string ToString() => klass.ToString() + " instance";
    }
}
