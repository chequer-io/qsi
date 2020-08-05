using System.Collections.Generic;
using CppAst;
using CSharpSyntax;

namespace Qsi.PostgreSql.Generator.Generators
{
    internal interface ISourceGenerator
    {
        IEnumerable<BaseTypeDeclarationSyntax> Generate(CppClass cppClass);

        IEnumerable<BaseTypeDeclarationSyntax> Generate(CppEnum cppEnum);
    }
}
