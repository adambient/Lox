namespace Lox
{
    public class LoxClass : ILoxCallable
    {
        readonly string name;
        readonly LoxClass? superclass;
        readonly Dictionary<string, LoxFunction> methods;

        public LoxClass(string name, LoxClass? superclass, Dictionary<string, LoxFunction> methods)
        {
            this.name = name;
            this.superclass = superclass;
            this.methods = methods;
        }

        public LoxFunction? FindMethod(string name)
        {
            if (methods.ContainsKey(name))
            {
                return methods[name];
            }

            if (superclass != null)
            {
                return superclass.FindMethod(name);
            }

            return null; ;
        }

        public override string ToString() => name;

        int ILoxCallable.Arity()
        {
            var initializer = FindMethod("init");
            if (initializer == null)
            {
                return 0;
            }

            return initializer.Arity();
        }

        object? ILoxCallable.Call(Interpreter interpreter, List<object?> arguments)
        {
            var instance = new LoxInstance(this);

            var initializer = FindMethod("init");
            if (initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }
    }
}
