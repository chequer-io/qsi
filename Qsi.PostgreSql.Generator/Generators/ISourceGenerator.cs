using System;
using CppAst;
using Qsi.PostgreSql.Generator.Models;

namespace Qsi.PostgreSql.Generator.Generators
{
    internal interface ISourceGenerator
    {
        event Func<CppType, GenerateResult> ResolveType;

        GenerateResult Generate(CppTypeDeclaration cppType);
    }
}
