using System.IO;
using CSharpSyntax;
using CSharpSyntax.Printer;

namespace Qsi.PostgreSql.Generator.Extensions
{
    internal static class TypeSyntaxExtension
    {
        public static bool AreEquals(this TypeSyntax a, TypeSyntax b)
        {
            return a.Print() == b.Print();
        }

        public static string Print(this TypeSyntax typeSyntax)
        {
            using var writer = new StringWriter();
            var printer = new SyntaxPrinter(new SyntaxWriter(writer));

            printer.Visit(typeSyntax);
            printer.Dispose();

            return writer.ToString();
        }
    }
}
