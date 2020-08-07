using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CppAst;
using CSharpSyntax;
using Microsoft.CodeAnalysis.CSharp;
using Qsi.PostgreSql.Generator.Extensions;
using Qsi.PostgreSql.Generator.Models;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace Qsi.PostgreSql.Generator.Generators
{
    internal class Pg10Generator : ISourceGenerator
    {
        private const string nodeInterfaceName = "IPg10Node";
        private const string nodeTypeFieldName = "Type";
        private const string nodeTypeName = "NodeTag";

        public event Func<CppType, GenerateResult> ResolveType;

        private readonly Dictionary<string, string> _typeMap;

        public Pg10Generator(GenerateConfig config)
        {
            _typeMap = config.TypeMap ?? new Dictionary<string, string>();
        }

        public GenerateResult Generate(CppTypeDeclaration cppType)
        {
            switch (cppType)
            {
                case CppEnum cppEnum:
                    return GenerateEnum(cppEnum);

                case CppTypedef cppTypedef:
                    return GenerateTypedef(cppTypedef);

                case CppClass cppClass:
                    return GenerateClass(cppClass);
            }

            throw new NotSupportedException(cppType.ToString());
        }

        public GenerateResult GenerateEnum(CppEnum cppEnum)
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

            return new GenerateResult(cppEnum)
            {
                Type = csEnum
            };
        }

        private GenerateResult GenerateTypedef(CppTypedef cppTypedef)
        {
            var elementType = cppTypedef.ElementType;
            var array = false;

            if (elementType is CppPointerType pointerType)
            {
                array = true;
                elementType = pointerType.ElementType;
            }

            if (elementType.TypeKind != CppTypeKind.StructOrClass)
                throw new NotSupportedException(elementType.GetDisplayName());

            var csType = ConvertToCSharpType(elementType);

            if (array)
                csType += "[]";

            _typeMap[cppTypedef.Name] = csType;

            return new GenerateResult(cppTypedef);
        }

        private GenerateResult GenerateClass(CppClass cppClass)
        {
            if (cppClass.Name == "Node")
                return CreateNodeInterface(cppClass);

            var usingDirectives = new List<UsingDirectiveSyntax>();

            var csClass = new ClassDeclarationSyntax
            {
                Modifiers = Modifiers.Internal | Modifiers.Sealed,
                Identifier = CreateMemberName(cppClass)
            };

            if (cppClass.Fields.Any(f => f.Type.GetDisplayName() == nodeTypeName))
            {
                // using Qsi.PostgreSql.Internal.Postgres;
                usingDirectives.Add(Syntax.UsingDirective("Qsi.PostgreSql.Internal.Postgres"));

                // [PgNode("..")]
                var pgNodeAttribute = Syntax.Attribute(
                    "PgNode",
                    Syntax.AttributeArgumentList(Syntax.AttributeArgument(Syntax.LiteralExpression(cppClass.Name))));

                csClass.AttributeLists.Add(Syntax.AttributeList(pgNodeAttribute));

                // .. : IPg10Node
                csClass.BaseList = new BaseListSyntax
                {
                    Types =
                    {
                        Syntax.ParseName(nodeInterfaceName)
                    }
                };

                string nodeTypeValue = cppClass.Name == "MemoryContextData" ?
                    "T_MemoryContext" :
                    $"T_{cppClass.Name}";

                // IPg10Node::Type
                csClass.Members.Add(new PropertyDeclarationSyntax
                {
                    Modifiers = Modifiers.Public,
                    Identifier = nodeTypeFieldName,
                    Type = Syntax.ParseName(nodeTypeName),
                    AccessorList = Syntax.AccessorList(
                        Syntax.AccessorDeclaration(
                            AccessorDeclarationKind.Get,
                            Syntax.Block(
                                Syntax.ReturnStatement(
                                    Syntax.MemberAccessExpression(Syntax.ParseName(nodeTypeName), nodeTypeValue)
                                )
                            )
                        )
                    )
                });
            }

            var nestedClasses = new List<GenerateResult>();

            foreach (var field in cppClass.Fields)
            {
                string fieldName = field.Name;
                string typeName;

                if (cppClass.Equals(field.Type.Parent))
                {
                    var nestedClass = GenerateClass((CppClass)field.Type);

                    nestedClasses.Add(nestedClass);
                    typeName = nestedClass.Type.Identifier;
                }
                else
                {
                    typeName = ConvertToCSharpType(field.Type);
                }

                // IPg10Node::Type
                if (typeName == nodeTypeName && fieldName == "type")
                    continue;

                if (SyntaxFacts.GetKeywordKind(fieldName) != SyntaxKind.None)
                    fieldName = $"@{fieldName}";

                csClass.Members.Add(new PropertyDeclarationSyntax
                {
                    Modifiers = Modifiers.Public,
                    AccessorList = Syntax.AccessorList(
                        Syntax.AccessorDeclaration(AccessorDeclarationKind.Get, null),
                        Syntax.AccessorDeclaration(AccessorDeclarationKind.Set, null)),
                    Identifier = fieldName,
                    Type = Syntax.ParseName(typeName)
                });
            }

            foreach (var nestedClass in nestedClasses)
            {
                foreach (var usingDirective in nestedClass.UsingDirectives ?? Array.Empty<UsingDirectiveSyntax>())
                {
                    if (usingDirectives.Any(d => d.Name.AreEquals(usingDirective.Name)))
                        continue;

                    usingDirectives.Add(usingDirective);
                }

                csClass.Members.Add(nestedClass.Type);
            }

            return new GenerateResult(cppClass)
            {
                Type = csClass,
                UsingDirectives = usingDirectives.ToArray()
            };
        }

        private GenerateResult CreateNodeInterface(CppClass cppClass)
        {
            return new GenerateResult(cppClass)
            {
                Type = new InterfaceDeclarationSyntax
                {
                    Modifiers = Modifiers.Internal,
                    Identifier = nodeInterfaceName,
                    BaseList = new BaseListSyntax
                    {
                        Types =
                        {
                            Syntax.ParseName("IPgNode")
                        }
                    },
                    Members =
                    {
                        new PropertyDeclarationSyntax
                        {
                            AccessorList = Syntax.AccessorList(Syntax.AccessorDeclaration(AccessorDeclarationKind.Get, null)),
                            Identifier = nodeTypeFieldName,
                            Type = Syntax.ParseName(nodeTypeName)
                        }
                    }
                }
            };
        }

        private static string CreateMemberName(ICppMember cppMember)
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

        private string ConvertToCSharpType(CppType type)
        {
            string typeName = null;

            while (true)
            {
                switch (type.TypeKind)
                {
                    case CppTypeKind.Typedef:
                    {
                        if (_typeMap.TryGetValue(((CppTypedef)type).Name, out var childType))
                        {
                            typeName = childType;
                            break;
                        }

                        type = ((CppTypedef)type).ElementType;
                        continue;
                    }

                    case CppTypeKind.Enum:
                    case CppTypeKind.StructOrClass:
                    {
                        var memberName = ((ICppMember)type).Name;

                        if (_typeMap.TryGetValue(memberName, out var result))
                        {
                            typeName = result;
                            break;
                        }

                        _typeMap[memberName] = memberName;

                        var resolve = ResolveType?.Invoke(type) ?? throw new InvalidOperationException();
                        typeName = resolve.Type.Identifier;
                        break;
                    }

                    case CppTypeKind.Primitive:
                    {
                        typeName = ConvertToCSharpType((CppPrimitiveType)type);
                        break;
                    }

                    case CppTypeKind.Array:
                    case CppTypeKind.Pointer:
                    {
                        var elementType = ((CppTypeWithElementType)type).ElementType;

                        if (elementType is CppPrimitiveType primitiveType && primitiveType.Kind == CppPrimitiveKind.Char)
                        {
                            typeName = "string";
                            break;
                        }

                        typeName = ConvertToCSharpType(elementType);

                        if (elementType.TypeKind == CppTypeKind.Pointer || elementType.TypeKind == CppTypeKind.Primitive)
                        {
                            typeName += "[]";
                        }

                        break;
                    }

                    case CppTypeKind.Qualified:
                    {
                        typeName = ConvertToCSharpType(((CppQualifiedType)type).ElementType);
                        break;
                    }

                    case CppTypeKind.Function:
                    {
                        typeName = "string";
                        break;
                    }
                }

                if (typeName == null)
                    throw new NotSupportedException(type.GetDisplayName());

                return typeName;
            }
        }

        private static string ConvertToCSharpType(CppPrimitiveType type)
        {
            switch (type.Kind)
            {
                case CppPrimitiveKind.Bool:
                case CppPrimitiveKind.Char:
                case CppPrimitiveKind.Double:
                case CppPrimitiveKind.Float:
                case CppPrimitiveKind.Int:
                case CppPrimitiveKind.Short:
                    return type.Kind.ToString().ToLower();

                case CppPrimitiveKind.Void:
                    return "object";

                case CppPrimitiveKind.LongDouble:
                    return "double";

                case CppPrimitiveKind.LongLong:
                    return "long";

                case CppPrimitiveKind.WChar:
                case CppPrimitiveKind.UnsignedShort:
                    return "ushort";

                case CppPrimitiveKind.UnsignedChar:
                    return "byte";

                case CppPrimitiveKind.UnsignedInt:
                    return "uint";

                case CppPrimitiveKind.UnsignedLongLong:
                    return "ulong";
            }

            throw new NotSupportedException(type.Kind.ToString());
        }
    }
}
