using CppAst;
using CSharpSyntax;

namespace Qsi.PostgreSql.Generator.Models
{
    public sealed class GenerateResult
    {
        public CppTypeDeclaration CppType { get; }

        public UsingDirectiveSyntax[] UsingDirectives { get; set; }

        public BaseTypeDeclarationSyntax Type { get; set; }

        public GenerateResult(CppTypeDeclaration cppTypeDeclaration)
        {
            CppType = cppTypeDeclaration;
        }
    }
}
