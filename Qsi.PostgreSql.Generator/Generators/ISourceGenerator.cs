using System.Collections.Generic;
using CppAst;
using CSharpSyntax;

namespace Qsi.PostgreSql.Generator.Generators
{
    internal interface ISourceGenerator
    {
        IEnumerable<SyntaxNode> Generate(CppClass cppClass);

        IEnumerable<SyntaxNode> Generate(CppEnum cppEnum);

        IEnumerable<SyntaxNode> Generate(CppTypedef typedef);
    }
}
