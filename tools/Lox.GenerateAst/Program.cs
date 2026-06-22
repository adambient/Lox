using System.Text;

namespace Lox.GenerateAst
{
    internal class Program
    {
        const string TAB = "    "; // tab is currently 4 spaces

        static async Task<int> Main(string[] args)
        {
            // default to our own location
            var outputDir = "C:\\projects\\lox\\src\\Lox\\";
            if (args.Length >= 1)
            {
                outputDir = args[0];
            }

            await DefineAstAsync(outputDir, "Expr", [
                "Binary   : Expr Left, Token Operator, Expr Right",
                "Grouping : Expr Expression",
                "Literal  : object? Value",
                "Unary    : Token Operator, Expr Right"
                ]);

            return 0;
        }

        static async Task DefineAstAsync(string outputDir, string baseName, List<string> types)
        {
            var path = outputDir + "/" + baseName + ".cs";
            using (var writer = new StreamWriter(path, append: false, encoding: Encoding.UTF8))
            {
                await writer.WriteLineAsync("namespace Lox");
                await writer.WriteLineAsync("{");
                await writer.WriteLineAsync($"{TAB}public abstract record {baseName}");
                await writer.WriteLineAsync($"{TAB}{{");

                await DefineVisitorAsync(writer, baseName, types);

                // the AST classes
                foreach (var type in types)
                {
                    var className = type.Split(":")[0].Trim();
                    var fields = type.Split(":")[1].Trim();
                    await DefineTypeAsync(writer, baseName, className, fields);
                }

                // the base Accept() method
                await writer.WriteLineAsync();
                await writer.WriteLineAsync($"{TAB}{TAB}public abstract TOut Accept<TOut>(IVisitor<TOut> visitor);");


                await writer.WriteLineAsync($"{TAB}}}");
                await writer.WriteLineAsync("}");
            }

            static async Task DefineTypeAsync(StreamWriter writer, string baseName, string className, string fieldList)
            {
                await writer.WriteLineAsync($"{TAB}{TAB}public record {className}({fieldList}) : {baseName}");
                await writer.WriteLineAsync($"{TAB}{TAB}{{");

                // visitor pattern
                await writer.WriteLineAsync($"{TAB}{TAB}{TAB}public override TOut Accept<TOut>(IVisitor<TOut> visitor) =>");
                await writer.WriteLineAsync($"{TAB}{TAB}{TAB}{TAB}visitor.Visit{className}{baseName}(this);");

                await writer.WriteLineAsync($"{TAB}{TAB}}}");
            }

            static async Task DefineVisitorAsync(StreamWriter writer, string baseName, List<string> types)
            {
                await writer.WriteLineAsync($"{TAB}{TAB}public interface IVisitor<TOut>");
                await writer.WriteLineAsync($"{TAB}{TAB}{{");

                foreach (var type in types)
                {
                    var typeName = type.Split(":")[0].Trim();
                    await writer.WriteLineAsync($"{TAB}{TAB}{TAB}TOut Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
                }

                await writer.WriteLineAsync($"{TAB}{TAB}}}");
            }
        }
    }
}
