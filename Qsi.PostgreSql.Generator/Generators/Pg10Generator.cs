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

        private HashSet<string> _tagSet = new HashSet<string>(
            new string[]
            {
                "T_Invalid",
                "T_IndexInfo",
                "T_ExprContext",
                "T_ProjectionInfo",
                "T_JunkFilter",
                "T_ResultRelInfo",
                "T_EState",
                "T_TupleTableSlot",
                "T_Plan",
                "T_Result",
                "T_ProjectSet",
                "T_ModifyTable",
                "T_Append",
                "T_MergeAppend",
                "T_RecursiveUnion",
                "T_BitmapAnd",
                "T_BitmapOr",
                "T_Scan",
                "T_SeqScan",
                "T_SampleScan",
                "T_IndexScan",
                "T_IndexOnlyScan",
                "T_BitmapIndexScan",
                "T_BitmapHeapScan",
                "T_TidScan",
                "T_SubqueryScan",
                "T_FunctionScan",
                "T_ValuesScan",
                "T_TableFuncScan",
                "T_CteScan",
                "T_NamedTuplestoreScan",
                "T_WorkTableScan",
                "T_ForeignScan",
                "T_CustomScan",
                "T_Join",
                "T_NestLoop",
                "T_MergeJoin",
                "T_HashJoin",
                "T_Material",
                "T_Sort",
                "T_Group",
                "T_Agg",
                "T_WindowAgg",
                "T_Unique",
                "T_Gather",
                "T_GatherMerge",
                "T_Hash",
                "T_SetOp",
                "T_LockRows",
                "T_Limit",
                "T_NestLoopParam",
                "T_PlanRowMark",
                "T_PlanInvalItem",
                "T_PlanState",
                "T_ResultState",
                "T_ProjectSetState",
                "T_ModifyTableState",
                "T_AppendState",
                "T_MergeAppendState",
                "T_RecursiveUnionState",
                "T_BitmapAndState",
                "T_BitmapOrState",
                "T_ScanState",
                "T_SeqScanState",
                "T_SampleScanState",
                "T_IndexScanState",
                "T_IndexOnlyScanState",
                "T_BitmapIndexScanState",
                "T_BitmapHeapScanState",
                "T_TidScanState",
                "T_SubqueryScanState",
                "T_FunctionScanState",
                "T_TableFuncScanState",
                "T_ValuesScanState",
                "T_CteScanState",
                "T_NamedTuplestoreScanState",
                "T_WorkTableScanState",
                "T_ForeignScanState",
                "T_CustomScanState",
                "T_JoinState",
                "T_NestLoopState",
                "T_MergeJoinState",
                "T_HashJoinState",
                "T_MaterialState",
                "T_SortState",
                "T_GroupState",
                "T_AggState",
                "T_WindowAggState",
                "T_UniqueState",
                "T_GatherState",
                "T_GatherMergeState",
                "T_HashState",
                "T_SetOpState",
                "T_LockRowsState",
                "T_LimitState",
                "T_Alias",
                "T_RangeVar",
                "T_TableFunc",
                "T_Expr",
                "T_Var",
                "T_Const",
                "T_Param",
                "T_Aggref",
                "T_GroupingFunc",
                "T_WindowFunc",
                "T_ArrayRef",
                "T_FuncExpr",
                "T_NamedArgExpr",
                "T_OpExpr",
                "T_DistinctExpr",
                "T_NullIfExpr",
                "T_ScalarArrayOpExpr",
                "T_BoolExpr",
                "T_SubLink",
                "T_SubPlan",
                "T_AlternativeSubPlan",
                "T_FieldSelect",
                "T_FieldStore",
                "T_RelabelType",
                "T_CoerceViaIO",
                "T_ArrayCoerceExpr",
                "T_ConvertRowtypeExpr",
                "T_CollateExpr",
                "T_CaseExpr",
                "T_CaseWhen",
                "T_CaseTestExpr",
                "T_ArrayExpr",
                "T_RowExpr",
                "T_RowCompareExpr",
                "T_CoalesceExpr",
                "T_MinMaxExpr",
                "T_SQLValueFunction",
                "T_XmlExpr",
                "T_NullTest",
                "T_BooleanTest",
                "T_CoerceToDomain",
                "T_CoerceToDomainValue",
                "T_SetToDefault",
                "T_CurrentOfExpr",
                "T_NextValueExpr",
                "T_InferenceElem",
                "T_TargetEntry",
                "T_RangeTblRef",
                "T_JoinExpr",
                "T_FromExpr",
                "T_OnConflictExpr",
                "T_IntoClause",
                "T_ExprState",
                "T_AggrefExprState",
                "T_WindowFuncExprState",
                "T_SetExprState",
                "T_SubPlanState",
                "T_AlternativeSubPlanState",
                "T_DomainConstraintState",
                "T_PlannerInfo",
                "T_PlannerGlobal",
                "T_RelOptInfo",
                "T_IndexOptInfo",
                "T_ForeignKeyOptInfo",
                "T_ParamPathInfo",
                "T_Path",
                "T_IndexPath",
                "T_BitmapHeapPath",
                "T_BitmapAndPath",
                "T_BitmapOrPath",
                "T_TidPath",
                "T_SubqueryScanPath",
                "T_ForeignPath",
                "T_CustomPath",
                "T_NestPath",
                "T_MergePath",
                "T_HashPath",
                "T_AppendPath",
                "T_MergeAppendPath",
                "T_ResultPath",
                "T_MaterialPath",
                "T_UniquePath",
                "T_GatherPath",
                "T_GatherMergePath",
                "T_ProjectionPath",
                "T_ProjectSetPath",
                "T_SortPath",
                "T_GroupPath",
                "T_UpperUniquePath",
                "T_AggPath",
                "T_GroupingSetsPath",
                "T_MinMaxAggPath",
                "T_WindowAggPath",
                "T_SetOpPath",
                "T_RecursiveUnionPath",
                "T_LockRowsPath",
                "T_ModifyTablePath",
                "T_LimitPath",
                "T_EquivalenceClass",
                "T_EquivalenceMember",
                "T_PathKey",
                "T_PathTarget",
                "T_RestrictInfo",
                "T_PlaceHolderVar",
                "T_SpecialJoinInfo",
                "T_AppendRelInfo",
                "T_PartitionedChildRelInfo",
                "T_PlaceHolderInfo",
                "T_MinMaxAggInfo",
                "T_PlannerParamItem",
                "T_RollupData",
                "T_GroupingSetData",
                "T_StatisticExtInfo",
                "T_MemoryContext",
                "T_AllocSetContext",
                "T_SlabContext",
                "T_Value",
                "T_Integer",
                "T_Float",
                "T_String",
                "T_BitString",
                "T_Null",
                "T_List",
                "T_IntList",
                "T_OidList",
                "T_ExtensibleNode",
                "T_RawStmt",
                "T_Query",
                "T_PlannedStmt",
                "T_InsertStmt",
                "T_DeleteStmt",
                "T_UpdateStmt",
                "T_SelectStmt",
                "T_AlterTableStmt",
                "T_AlterTableCmd",
                "T_AlterDomainStmt",
                "T_SetOperationStmt",
                "T_GrantStmt",
                "T_GrantRoleStmt",
                "T_AlterDefaultPrivilegesStmt",
                "T_ClosePortalStmt",
                "T_ClusterStmt",
                "T_CopyStmt",
                "T_CreateStmt",
                "T_DefineStmt",
                "T_DropStmt",
                "T_TruncateStmt",
                "T_CommentStmt",
                "T_FetchStmt",
                "T_IndexStmt",
                "T_CreateFunctionStmt",
                "T_AlterFunctionStmt",
                "T_DoStmt",
                "T_RenameStmt",
                "T_RuleStmt",
                "T_NotifyStmt",
                "T_ListenStmt",
                "T_UnlistenStmt",
                "T_TransactionStmt",
                "T_ViewStmt",
                "T_LoadStmt",
                "T_CreateDomainStmt",
                "T_CreatedbStmt",
                "T_DropdbStmt",
                "T_VacuumStmt",
                "T_ExplainStmt",
                "T_CreateTableAsStmt",
                "T_CreateSeqStmt",
                "T_AlterSeqStmt",
                "T_VariableSetStmt",
                "T_VariableShowStmt",
                "T_DiscardStmt",
                "T_CreateTrigStmt",
                "T_CreatePLangStmt",
                "T_CreateRoleStmt",
                "T_AlterRoleStmt",
                "T_DropRoleStmt",
                "T_LockStmt",
                "T_ConstraintsSetStmt",
                "T_ReindexStmt",
                "T_CheckPointStmt",
                "T_CreateSchemaStmt",
                "T_AlterDatabaseStmt",
                "T_AlterDatabaseSetStmt",
                "T_AlterRoleSetStmt",
                "T_CreateConversionStmt",
                "T_CreateCastStmt",
                "T_CreateOpClassStmt",
                "T_CreateOpFamilyStmt",
                "T_AlterOpFamilyStmt",
                "T_PrepareStmt",
                "T_ExecuteStmt",
                "T_DeallocateStmt",
                "T_DeclareCursorStmt",
                "T_CreateTableSpaceStmt",
                "T_DropTableSpaceStmt",
                "T_AlterObjectDependsStmt",
                "T_AlterObjectSchemaStmt",
                "T_AlterOwnerStmt",
                "T_AlterOperatorStmt",
                "T_DropOwnedStmt",
                "T_ReassignOwnedStmt",
                "T_CompositeTypeStmt",
                "T_CreateEnumStmt",
                "T_CreateRangeStmt",
                "T_AlterEnumStmt",
                "T_AlterTSDictionaryStmt",
                "T_AlterTSConfigurationStmt",
                "T_CreateFdwStmt",
                "T_AlterFdwStmt",
                "T_CreateForeignServerStmt",
                "T_AlterForeignServerStmt",
                "T_CreateUserMappingStmt",
                "T_AlterUserMappingStmt",
                "T_DropUserMappingStmt",
                "T_AlterTableSpaceOptionsStmt",
                "T_AlterTableMoveAllStmt",
                "T_SecLabelStmt",
                "T_CreateForeignTableStmt",
                "T_ImportForeignSchemaStmt",
                "T_CreateExtensionStmt",
                "T_AlterExtensionStmt",
                "T_AlterExtensionContentsStmt",
                "T_CreateEventTrigStmt",
                "T_AlterEventTrigStmt",
                "T_RefreshMatViewStmt",
                "T_ReplicaIdentityStmt",
                "T_AlterSystemStmt",
                "T_CreatePolicyStmt",
                "T_AlterPolicyStmt",
                "T_CreateTransformStmt",
                "T_CreateAmStmt",
                "T_CreatePublicationStmt",
                "T_AlterPublicationStmt",
                "T_CreateSubscriptionStmt",
                "T_AlterSubscriptionStmt",
                "T_DropSubscriptionStmt",
                "T_CreateStatsStmt",
                "T_AlterCollationStmt",
                "T_A_Expr",
                "T_ColumnRef",
                "T_ParamRef",
                "T_A_Const",
                "T_FuncCall",
                "T_A_Star",
                "T_A_Indices",
                "T_A_Indirection",
                "T_A_ArrayExpr",
                "T_ResTarget",
                "T_MultiAssignRef",
                "T_TypeCast",
                "T_CollateClause",
                "T_SortBy",
                "T_WindowDef",
                "T_RangeSubselect",
                "T_RangeFunction",
                "T_RangeTableSample",
                "T_RangeTableFunc",
                "T_RangeTableFuncCol",
                "T_TypeName",
                "T_ColumnDef",
                "T_IndexElem",
                "T_Constraint",
                "T_DefElem",
                "T_RangeTblEntry",
                "T_RangeTblFunction",
                "T_TableSampleClause",
                "T_WithCheckOption",
                "T_SortGroupClause",
                "T_GroupingSet",
                "T_WindowClause",
                "T_ObjectWithArgs",
                "T_AccessPriv",
                "T_CreateOpClassItem",
                "T_TableLikeClause",
                "T_FunctionParameter",
                "T_LockingClause",
                "T_RowMarkClause",
                "T_XmlSerialize",
                "T_WithClause",
                "T_InferClause",
                "T_OnConflictClause",
                "T_CommonTableExpr",
                "T_RoleSpec",
                "T_TriggerTransition",
                "T_PartitionElem",
                "T_PartitionSpec",
                "T_PartitionBoundSpec",
                "T_PartitionRangeDatum",
                "T_PartitionCmd",
                "T_IdentifySystemCmd",
                "T_BaseBackupCmd",
                "T_CreateReplicationSlotCmd",
                "T_DropReplicationSlotCmd",
                "T_StartReplicationCmd",
                "T_TimeLineHistoryCmd",
                "T_SQLCmd",
                "T_TriggerData",
                "T_EventTriggerData",
                "T_ReturnSetInfo",
                "T_WindowObjectData",
                "T_TIDBitmap",
                "T_InlineCodeBlock",
                "T_FdwRoutine",
                "T_IndexAmRoutine",
                "T_TsmRoutine",
                "T_ForeignKeyCacheInfo"
            });

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

                _tagSet.Remove(nodeTypeValue);
                Console.WriteLine(_tagSet.Count);

                if (_tagSet.Count == 205)
                {
                    Console.Clear();
                    Console.WriteLine(string.Join("\r\n", _tagSet));
                }

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
