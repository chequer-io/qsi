using System.Collections.Generic;
using System.Linq;
using CppAst;
using CSharpSyntax;
using Qsi.PostgreSql.Generator.Extensions;
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

        public IEnumerable<SyntaxNode> Generate(CppClass cppClass)
        {
            var usingDirectives = new List<UsingDirectiveSyntax>
            {
                Syntax.UsingDirective("Qsi.PostgreSql.Internal.Postgres")
            };

            var csClass = new ClassDeclarationSyntax
            {
                Modifiers = Modifiers.Internal | Modifiers.Sealed,
                Identifier = CreateMemberName(cppClass),
                BaseList = new BaseListSyntax
                {
                    Types =
                    {
                        Syntax.ParseName("IPgTree")
                    }
                }
            };

            if (!string.IsNullOrEmpty(cppClass.Name))
            {
                var pgNodeAttribute = Syntax.Attribute(
                    "PgNode",
                    Syntax.AttributeArgumentList(Syntax.AttributeArgument(Syntax.LiteralExpression(cppClass.Name))));

                csClass.AttributeLists.Add(Syntax.AttributeList(pgNodeAttribute));
            }

            IEnumerable<SyntaxNode> nestedNodes = cppClass.Classes
                .SelectMany(Generate)
                .ToArray();

            foreach (var usingDirective in nestedNodes.OfType<UsingDirectiveSyntax>())
            {
                if (usingDirectives.Any(d => d.Name.AreEquals(usingDirective.Name)))
                    continue;

                usingDirectives.Add(usingDirective);
            }

            foreach (var memberNode in nestedNodes.OfType<MemberDeclarationSyntax>())
            {
                csClass.Members.Add(memberNode);
            }

            foreach (var usingDirective in usingDirectives)
            {
                yield return usingDirective;
            }

            yield return csClass;
        }

        private string CreateMemberName(ICppMember cppMember)
        {
            if (!string.IsNullOrWhiteSpace(cppMember.Name))
                return cppMember.Name;

            var stack = 0;

            while (cppMember != null)
            {
                stack++;

                if (cppMember is CppElement cppElement && cppElement.Parent is ICppMember cppMemberParent)
                {
                    cppMember = cppMemberParent;
                }
                else
                {
                    cppMember = null;
                }
            }

            return $"Unknown{stack}";
        }

        public IEnumerable<SyntaxNode> Generate(CppTypedef typedef)
        {
            //Console.WriteLine(typedef.Name);
            yield break;
        }

        public IEnumerable<SyntaxNode> Generate(CppEnum cppEnum)
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
