using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Qsi.SqlServer.Diagnostics
{
    internal sealed partial class SqlServerRawTreeVisitor
    {
        public override void ExplicitVisit(StatementList node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecuteStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecuteOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ResultSetsExecuteOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ResultSetDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(InlineResultSetDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ResultColumnDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SchemaObjectResultSetDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecuteSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecuteContext node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecuteParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecutableEntity node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ProcedureReferenceName node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecutableProcedureReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecutableStringList node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AdHocDataSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ViewOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterViewStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateViewStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateOrAlterViewStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ViewStatementBody node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ViewForAppendOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ViewDistributionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ViewDistributionPolicy node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ViewRoundRobinDistributionPolicy node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ViewHashDistributionPolicy node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TriggerObject node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TriggerOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecuteAsTriggerOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TriggerAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTriggerStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateTriggerStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateOrAlterTriggerStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TriggerStatementBody node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(Identifier node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterProcedureStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateProcedureStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateOrAlterProcedureStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ProcedureReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MethodSpecifier node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ProcedureStatementBody node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ProcedureStatementBodyBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FunctionStatementBody node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ProcedureOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecuteAsProcedureOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FunctionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(InlineFunctionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecuteAsFunctionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(XmlNamespaces node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(XmlNamespacesElement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(XmlNamespacesDefaultElement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(XmlNamespacesAliasElement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CommonTableExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WithCtesAndXmlNamespaces node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FunctionReturnType node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableValuedFunctionReturnType node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DataTypeReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ParameterizedDataTypeReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SqlDataTypeReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UserDataTypeReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(XmlDataTypeReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ScalarFunctionReturnType node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SelectFunctionReturnType node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DeclareTableVariableBody node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DeclareTableVariableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(NamedTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SchemaObjectFunctionTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableHint node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IndexTableHint node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralTableHint node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryDerivedTable node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(InlineDerivedTable node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SubqueryComparisonPredicate node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExistsPredicate node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LikePredicate node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(InPredicate node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FullTextPredicate node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UserDefinedTypePropertyAccess node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(StatementWithCtesAndXmlNamespaces node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SelectStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ForClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BrowseForClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ReadOnlyForClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(XmlForClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(XmlForClauseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(JsonForClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(JsonForClauseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UpdateForClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OptimizerHint node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralOptimizerHint node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableHintsOptimizerHint node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ForceSeekTableHint node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OptimizeForOptimizerHint node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UseHintList node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(VariableValuePair node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WhenClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SimpleWhenClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SearchedWhenClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CaseExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SimpleCaseExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SearchedCaseExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(NullIfExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CoalesceExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IIfCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FullTextTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SemanticTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OpenXmlTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OpenJsonTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OpenRowsetTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(InternalOpenRowset node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BulkOpenRowset node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OpenQueryTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AdHocTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SchemaDeclarationItem node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SchemaDeclarationItemOpenjson node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ConvertCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TryConvertCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ParseCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TryParseCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CastCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TryCastCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AtTimeZoneCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FunctionCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CallTarget node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExpressionCallTarget node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MultiPartIdentifierCallTarget node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UserDefinedTypeCallTarget node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LeftFunctionCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RightFunctionCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PartitionFunctionCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OverClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ParameterlessCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ScalarSubquery node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OdbcFunctionCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExtractFromExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OdbcConvertSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterFunctionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BeginEndBlockStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BeginEndAtomicBlockStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AtomicBlockOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralAtomicBlockOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IdentifierAtomicBlockOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffAtomicBlockOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BeginTransactionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BreakStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnWithSortOrder node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CommitTransactionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RollbackTransactionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SaveTransactionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ContinueStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateDefaultStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateFunctionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateOrAlterFunctionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateRuleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DeclareVariableElement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DeclareVariableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GoToStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IfStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LabelStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MultiPartIdentifier node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SchemaObjectName node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ChildObjectName node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ProcedureParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TransactionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WhileStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DeleteStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UpdateDeleteSpecificationBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DeleteSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(InsertStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(InsertSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UpdateStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UpdateSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateSchemaStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WaitForStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ReadTextStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UpdateTextStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WriteTextStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TextModificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LineNoStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SecurityStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GrantStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DenyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RevokeStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterAuthorizationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(Permission node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SecurityTargetObject node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SecurityTargetObjectName node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SecurityPrincipal node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SecurityStatementBody80 node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GrantStatement80 node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DenyStatement80 node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RevokeStatement80 node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SecurityElement80 node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CommandSecurityElement80 node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PrivilegeSecurityElement80 node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(Privilege80 node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SecurityUserClause80 node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SqlCommandIdentifier node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AssignmentSetClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FunctionCallSetClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(InsertSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ValuesInsertSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SelectInsertSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecuteInsertSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RowValue node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PrintStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UpdateCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TSEqualCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PrimaryExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(Literal node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IntegerLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(NumericLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RealLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MoneyLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BinaryLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(StringLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(NullLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IdentifierLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DefaultLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MaxLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OdbcLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralRange node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ValueExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(VariableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OptionValue node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffOptionValue node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralOptionValue node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GlobalVariableExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IdentifierOrValueExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IdentifierOrScalarExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SchemaObjectNameOrValueExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ParenthesisExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnReferenceExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(NextValueForExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SequenceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SequenceOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DataTypeSequenceOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ScalarExpressionSequenceOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateSequenceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterSequenceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropSequenceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SecurityPolicyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SecurityPredicateAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SecurityPolicyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateSecurityPolicyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterSecurityPolicyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropSecurityPolicyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateColumnMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnMasterKeyParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnMasterKeyStoreProviderNameParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnMasterKeyPathParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnMasterKeyEnclaveComputationsParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropColumnMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnEncryptionKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateColumnEncryptionKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterColumnEncryptionKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropColumnEncryptionKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnEncryptionKeyValue node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnEncryptionKeyValueParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnMasterKeyNameParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnEncryptionAlgorithmNameParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EncryptedValueParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalTableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalTableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalTableLiteralOrIdentifierOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalTableDistributionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalTableRejectTypeOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalTableDistributionPolicy node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalTableReplicatedDistributionPolicy node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalTableRoundRobinDistributionPolicy node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalTableShardedDistributionPolicy node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateExternalTableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropExternalTableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalDataSourceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalDataSourceOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalDataSourceLiteralOrIdentifierOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateExternalDataSourceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterExternalDataSourceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropExternalDataSourceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalFileFormatStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalFileFormatOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalFileFormatLiteralOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalFileFormatUseDefaultTypeOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalFileFormatContainerOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateExternalFileFormatStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropExternalFileFormatStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AssemblyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateAssemblyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterAssemblyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AssemblyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffAssemblyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PermissionSetAssemblyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AddFileSpec node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateXmlSchemaCollectionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterXmlSchemaCollectionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropXmlSchemaCollectionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AssemblyName node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableAlterPartitionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableRebuildStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableChangeTrackingModificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableFileTableNamespaceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableSetStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LockEscalationTableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileStreamOnTableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileTableDirectoryTableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileTableCollateFileNameTableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileTableConstraintNameTableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MemoryOptimizedTableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DurabilityTableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RemoteDataArchiveTableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RemoteDataArchiveAlterTableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RemoteDataArchiveDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RemoteDataArchiveDatabaseSetting node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RemoteDataArchiveDbServerSetting node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RemoteDataArchiveDbCredentialSetting node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RetentionPeriodDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SystemVersioningTableOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableAddTableElementStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableConstraintModificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableSwitchStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableSwitchOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LowPriorityLockWaitTableSwitchOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TruncateTargetTableSwitchOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropClusteredConstraintOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropClusteredConstraintStateOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropClusteredConstraintValueOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropClusteredConstraintMoveOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableDropTableElement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableDropTableElementStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableTriggerModificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EnableDisableTriggerStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TryCatchStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateTypeStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateTypeUdtStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateTypeUddtStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateSynonymStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecuteAsClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueueOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueueStateOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueueProcedureOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueueValueOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueueExecuteAsOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RouteOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RouteStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterRouteStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateRouteStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueueStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterQueueStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateQueueStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IndexDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SystemTimePeriodDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IndexType node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PartitionSpecifier node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterIndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateXmlIndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateSelectiveXmlIndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileGroupOrPartitionScheme node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateIndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IndexStateOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IndexExpressionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MaxDurationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WaitAtLowPriorityOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnlineIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IgnoreDupKeyIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OrderIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnlineIndexLowPriorityLockWaitOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LowPriorityLockWaitOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LowPriorityLockWaitMaxDurationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LowPriorityLockWaitAbortAfterWaitOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FullTextIndexColumn node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateFullTextIndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FullTextIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ChangeTrackingFullTextIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(StopListFullTextIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SearchPropertyListFullTextIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FullTextCatalogAndFileGroup node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EventTypeGroupContainer node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EventTypeContainer node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EventGroupContainer node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateEventNotificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EventNotificationObjectScope node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ApplicationRoleOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ApplicationRoleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateApplicationRoleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterApplicationRoleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RoleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateRoleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterRoleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterRoleAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RenameAlterRoleAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AddMemberAlterRoleAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropMemberAlterRoleAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateServerRoleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterServerRoleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropServerRoleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UserLoginOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UserStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateUserStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterUserStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(StatisticsOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ResampleStatisticsOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(StatisticsPartitionRange node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffStatisticsOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralStatisticsOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateStatisticsStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UpdateStatisticsStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ReturnStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DeclareCursorStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CursorDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CursorOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetVariableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CursorId node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CursorStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OpenCursorStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CloseCursorStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CryptoMechanism node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OpenSymmetricKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CloseSymmetricKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OpenMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CloseMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DeallocateCursorStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FetchType node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FetchCursorStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WhereClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropUnownedObjectStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropObjectsStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropDatabaseStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropChildObjectsStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropIndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropIndexClauseBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BackwardsCompatibleDropIndexClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropIndexClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MoveToDropIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileStreamOnDropIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropStatisticsStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropTableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropProcedureStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropFunctionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropViewStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropDefaultStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropRuleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropTriggerStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropSchemaStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RaiseErrorLegacyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RaiseErrorStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ThrowStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UseStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(KillStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(KillQueryNotificationSubscriptionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(KillStatsJobStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CheckpointStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ReconfigureStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ShutdownStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetUserStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TruncateTableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetOnOffStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PredicateSetStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetStatisticsStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetRowCountStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetOffsetsStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetCommand node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GeneralSetCommand node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetFipsFlaggerCommand node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetCommandStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetTransactionIsolationLevelStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetTextSizeStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetIdentityInsertStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetErrorLevelStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateDatabaseStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileDeclaration node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileDeclarationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(NameFileDeclarationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileNameFileDeclarationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SizeFileDeclarationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MaxSizeFileDeclarationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileGrowthFileDeclarationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileGroupDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseScopedConfigurationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseScopedConfigurationSetStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DatabaseConfigurationClearOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DatabaseConfigurationSetOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffPrimaryConfigurationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MaxDopConfigurationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GenericConfigurationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseCollateStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseRebuildLogStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseAddFileStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseAddFileGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseRemoveFileGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseRemoveFileStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseModifyNameStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseModifyFileStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseModifyFileGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseTermination node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseSetStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AutoCreateStatisticsDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ContainmentDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(HadrDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(HadrAvailabilityGroupDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DelayedDurabilityDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CursorDefaultDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RecoveryDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TargetRecoveryTimeDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PageVerifyDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PartnerDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WitnessDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ParameterizationDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IdentifierDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ChangeTrackingDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ChangeTrackingOptionDetail node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AutoCleanupChangeTrackingOptionDetail node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ChangeRetentionChangeTrackingOptionDetail node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AcceleratedDatabaseRecoveryDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryStoreDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryStoreOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryStoreDesiredStateOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryStoreCapturePolicyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryStoreSizeCleanupPolicyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryStoreDataFlushIntervalOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryStoreIntervalLengthOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryStoreMaxStorageSizeOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryStoreMaxPlansPerQueryOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryStoreTimeCleanupPolicyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AutomaticTuningDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AutomaticTuningOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AutomaticTuningForceLastGoodPlanOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AutomaticTuningCreateIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AutomaticTuningDropIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AutomaticTuningMaintainIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileStreamDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CatalogCollationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MaxSizeDatabaseOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableAlterIndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterTableAlterColumnStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnEncryptionDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnEncryptionDefinitionParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnEncryptionKeyNameParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnEncryptionTypeParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnEncryptionAlgorithmParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IdentityOptions node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnStorageOptions node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ConstraintDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateTableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FederationScheme node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableDataCompressionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableDistributionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableDistributionPolicy node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableReplicateDistributionPolicy node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableRoundRobinDistributionPolicy node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableHashDistributionPolicy node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableIndexType node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableClusteredIndexType node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableNonClusteredIndexType node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TablePartitionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PartitionSpecifications node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TablePartitionOptionSpecifications node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LocationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RenameEntityStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DataCompressionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CompressionPartitionRange node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CheckConstraintDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DefaultConstraintDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ForeignKeyConstraintDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(NullableConstraintDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GraphConnectionBetweenNodes node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GraphConnectionConstraintDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UniqueConstraintDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BackupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BackupDatabaseStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BackupTransactionLogStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RestoreStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RestoreOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ScalarExpressionRestoreOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MoveRestoreOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(StopRestoreOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileStreamRestoreOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BackupOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BackupEncryptionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DeviceInfo node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MirrorToClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BackupRestoreFileInfo node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BulkInsertBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BulkInsertStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(InsertBulkStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BulkInsertOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralBulkInsertOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OrderBulkInsertOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ColumnDefinitionBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalTableColumnDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(InsertBulkColumnDefinition node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DbccStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DbccOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DbccNamedLiteral node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateAsymmetricKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreatePartitionFunctionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PartitionParameterType node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreatePartitionSchemeStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RemoteServiceBindingStatementBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RemoteServiceBindingOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffRemoteServiceBindingOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UserRemoteServiceBindingOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateRemoteServiceBindingStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterRemoteServiceBindingStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EncryptionSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AssemblyEncryptionSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FileEncryptionSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ProviderEncryptionSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CertificateStatementBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterCertificateStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateCertificateStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CertificateOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateContractStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ContractMessage node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CredentialStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateCredentialStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterCredentialStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MessageTypeStatementBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateMessageTypeStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterMessageTypeStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateAggregateStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateEndpointStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterEndpointStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterCreateEndpointStatementBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EndpointAffinity node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EndpointProtocolOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralEndpointProtocolOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AuthenticationEndpointProtocolOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PortsEndpointProtocolOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CompressionEndpointProtocolOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ListenerIPEndpointProtocolOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IPv4 node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SoapMethod node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PayloadOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EnabledDisabledPayloadOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WsdlPayloadOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LoginTypePayloadOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralPayloadOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SessionTimeoutPayloadOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SchemaPayloadOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CharacterSetPayloadOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RolePayloadOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AuthenticationPayloadOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EncryptionPayloadOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SymmetricKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateSymmetricKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(KeyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(KeySourceKeyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlgorithmKeyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IdentityValueKeyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ProviderKeyNameKeyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreationDispositionKeyOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterSymmetricKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FullTextCatalogStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FullTextCatalogOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffFullTextCatalogOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateFullTextCatalogStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterFullTextCatalogStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterCreateServiceStatementBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateServiceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterServiceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ServiceContract node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BinaryExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BuiltInFunctionTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GlobalFunctionTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ComputeClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ComputeFunction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PivotedTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UnpivotedTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UnqualifiedJoin node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableSampleClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ScalarExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BooleanExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BooleanNotExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BooleanParenthesisExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BooleanComparisonExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BooleanBinaryExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BooleanIsNullExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GraphMatchPredicate node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GraphMatchLastNodePredicate node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GraphMatchNodeExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GraphMatchRecursivePredicate node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GraphMatchExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GraphMatchCompositeExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GraphRecursiveMatchQuantifier node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExpressionWithSortOrder node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GroupByClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GroupingSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExpressionGroupingSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CompositeGroupingSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CubeGroupingSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RollupGroupingSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GrandTotalGroupingSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GroupingSetsGroupingSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OutputClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OutputIntoClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(HavingClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IdentityFunctionCall node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(JoinParenthesisTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OrderByClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(JoinTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QualifiedJoin node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OdbcQualifiedJoinTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueryParenthesisExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QuerySpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FromClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SelectElement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SelectScalarExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SelectStarExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SelectSetVariable node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableReferenceWithAlias node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TableReferenceWithAliasAndColumns node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DataModificationTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ChangeTableChangesTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ChangeTableVersionTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BooleanTernaryExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TopRowFilter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OffsetClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UnaryExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BinaryQueryExpression node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(VariableTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(VariableMethodCallTableReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropPartitionFunctionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropPartitionSchemeStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropSynonymStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropAggregateStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropAssemblyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropApplicationRoleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropFullTextCatalogStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropFullTextIndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropLoginStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropRoleStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropTypeStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropUserStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropSymmetricKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropAsymmetricKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropCertificateStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropCredentialStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterPartitionFunctionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterPartitionSchemeStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterFullTextIndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterFullTextIndexAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SimpleAlterFullTextIndexAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetStopListAlterFullTextIndexAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SetSearchPropertyListAlterFullTextIndexAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropAlterFullTextIndexAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AddAlterFullTextIndexAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterColumnAlterFullTextIndexAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateSearchPropertyListStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterSearchPropertyListStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SearchPropertyListAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AddSearchPropertyListAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropSearchPropertyListAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropSearchPropertyListStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateLoginStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateLoginSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PasswordCreateLoginSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PrincipalOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffPrincipalOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralPrincipalOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IdentifierPrincipalOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WindowsCreateLoginSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalCreateLoginSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CertificateCreateLoginSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AsymmetricKeyCreateLoginSource node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PasswordAlterPrincipalOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterLoginStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterLoginOptionsStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterLoginEnableDisableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterLoginAddDropCredentialStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RevertStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropContractStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropEndpointStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropMessageTypeStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropQueueStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropRemoteServiceBindingStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropRouteStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropServiceStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SignatureStatementBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AddSignatureStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropSignatureStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropEventNotificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExecuteAsStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EndConversationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MoveConversationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GetConversationGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ReceiveStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SendStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WaitForSupportedStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterSchemaStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterAsymmetricKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterServiceMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BeginConversationTimerStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BeginDialogStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DialogOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ScalarExpressionDialogOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffDialogOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BackupCertificateStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BackupRestoreMasterKeyStatementBase node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BackupServiceMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RestoreServiceMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BackupMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RestoreMasterKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ScalarExpressionSnippet node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BooleanExpressionSnippet node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(StatementListSnippet node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SelectStatementSnippet node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SchemaObjectNameSnippet node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TSqlFragmentSnippet node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TSqlStatementSnippet node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(IdentifierSnippet node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TSqlScript node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TSqlBatch node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TSqlStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DataModificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DataModificationSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MergeStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MergeSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MergeActionClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MergeAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UpdateMergeAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DeleteMergeAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(InsertMergeAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateTypeTableStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SensitivityClassificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SensitivityClassificationOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AddSensitivityClassificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropSensitivityClassificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AuditSpecificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AuditSpecificationPart node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AuditSpecificationDetail node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AuditActionSpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DatabaseAuditAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AuditActionGroupReference node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateDatabaseAuditSpecificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseAuditSpecificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropDatabaseAuditSpecificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateServerAuditSpecificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterServerAuditSpecificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropServerAuditSpecificationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ServerAuditStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateServerAuditStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterServerAuditStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropServerAuditStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AuditTarget node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AuditOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(QueueDelayAuditOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AuditGuidAuditOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnFailureAuditOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(StateAuditOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AuditTargetOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MaxSizeAuditTargetOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(RetentionDaysAuditTargetOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MaxRolloverFilesAuditTargetOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralAuditTargetOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffAuditTargetOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DatabaseEncryptionKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateDatabaseEncryptionKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterDatabaseEncryptionKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropDatabaseEncryptionKeyStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ResourcePoolStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ResourcePoolParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ResourcePoolAffinitySpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateResourcePoolStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterResourcePoolStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropResourcePoolStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalResourcePoolStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalResourcePoolParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ExternalResourcePoolAffinitySpecification node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateExternalResourcePoolStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterExternalResourcePoolStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropExternalResourcePoolStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WorkloadGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WorkloadGroupResourceParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WorkloadGroupImportanceParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WorkloadGroupParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateWorkloadGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterWorkloadGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropWorkloadGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BrokerPriorityStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BrokerPriorityParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateBrokerPriorityStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterBrokerPriorityStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropBrokerPriorityStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateFullTextStopListStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterFullTextStopListStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FullTextStopListAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropFullTextStopListStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateCryptographicProviderStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterCryptographicProviderStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropCryptographicProviderStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EventSessionObjectName node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EventSessionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateEventSessionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EventDeclaration node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EventDeclarationSetParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SourceDeclaration node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EventDeclarationCompareFunctionParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TargetDeclaration node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SessionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(EventRetentionSessionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MemoryPartitionSessionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralSessionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(MaxDispatchLatencySessionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(OnOffSessionOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterEventSessionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropEventSessionStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterResourceGovernorStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateSpatialIndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SpatialIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SpatialIndexRegularOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BoundingBoxSpatialIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(BoundingBoxParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GridsSpatialIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(GridParameter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CellsPerObjectSpatialIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterServerConfigurationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(ProcessAffinityRange node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterServerConfigurationDiagnosticsLogOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterServerConfigurationHadrClusterOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterServerConfigurationSetSoftNumaStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterServerConfigurationSoftNumaOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AvailabilityGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateAvailabilityGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterAvailabilityGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AvailabilityReplica node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AvailabilityReplicaOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralReplicaOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AvailabilityModeReplicaOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(FailoverModeReplicaOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(PrimaryRoleReplicaOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SecondaryRoleReplicaOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AvailabilityGroupOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(LiteralAvailabilityGroupOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterAvailabilityGroupAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterAvailabilityGroupFailoverAction node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterAvailabilityGroupFailoverOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropAvailabilityGroupStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateFederationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(AlterFederationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DropFederationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(UseFederationStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DiskStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(DiskStatementOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CreateColumnStoreIndexStatement node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WindowFrameClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WindowDelimiter node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(WithinGroupClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(SelectiveXmlIndexPromotedPath node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(TemporalClause node)
        {
            MakeTree(node, base.ExplicitVisit);
        }

        public override void ExplicitVisit(CompressionDelayIndexOption node)
        {
            MakeTree(node, base.ExplicitVisit);
        }
    }
}
