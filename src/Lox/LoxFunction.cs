namespace Lox
{
    public class LoxFunction : ILoxCallable
    {
        readonly Stmt.Function declaration;
        readonly Environment closure;
        public LoxFunction(Stmt.Function declaration, Environment closure)
        {
            this.declaration = declaration;
            this.closure = closure;
        }
        int ILoxCallable.Arity() => declaration.Params.Count;
        object? ILoxCallable.Call(Interpreter interpreter, List<object?> arguments)
        {
            var environment = new Environment(closure);
            for (int i = 0; i < declaration.Params.Count; i++)
            {
                environment.Define(declaration.Params[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.Body, environment);
            }
            catch (ReturnException exception) // using exception for control flow here
            {
                return exception.Value;
            }

            return null;
        }
        public override string ToString() => $"<fn {declaration.Name.Lexeme}>";
    }
}
