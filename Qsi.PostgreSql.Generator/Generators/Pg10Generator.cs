using System;
using System.Collections.Generic;
using CppAst;
using CSharpSyntax;
using Qsi.PostgreSql.Generator.Models;

namespace Qsi.PostgreSql.Generator.Generators
{
    internal class Pg10Generator : ISourceGenerator
    {
        private readonly GenerateConfig _config;

        public Pg10Generator(GenerateConfig config)
        {
            _config = config;
        }

        public IEnumerable<BaseTypeDeclarationSyntax> Generate(CppClass cppClass)
        {
            Console.WriteLine(cppClass.Name);
            yield break;
        }

        public IEnumerable<BaseTypeDeclarationSyntax> Generate(CppEnum cppEnum)
        {
            var csEnum = new EnumDeclarationSyntax
            {
                Modifiers = Modifiers.Internal,
                Identifier = cppEnum.Name
            };

            foreach (var cppMember in cppEnum.Items)
            {
                csEnum.Members.Add(new EnumMemberDeclarationSyntax
                {
                    Identifier = cppMember.Name,
                    EqualsValue = new EqualsValueClauseSyntax
                    {
                        Value = Syntax.LiteralExpression(cppMember.Value)
                    }
                });
            }

            yield return csEnum;
        }
    }
}
