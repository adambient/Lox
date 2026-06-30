namespace Lox
{
    public class Environment
    {
        public Environment? Enclosing { get; }
        readonly Dictionary<string, object?> values = new();

        public Environment(Environment? enclosing = null) =>
            Enclosing = enclosing;

        public object? Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }

            if (Enclosing != null)
            {
                return Enclosing.Get(name);
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Assign(Token name, object? value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (Enclosing != null)
            {
                Enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Define(string name, object? value) =>
            values[name] = value;

        public object? GetAt(int distance, string name) =>
            Ancestor(distance).values[name];

        public void AssignAt(int distance, Token name, object? value) =>
            Ancestor(distance).values[name.Lexeme] = value;

        Environment Ancestor(int distance)
        {
            var environment = this;
            for (int i = 0; i < distance; i++)
            {
                environment = environment.Enclosing!;
            }

            return environment;
        }
    }
}
