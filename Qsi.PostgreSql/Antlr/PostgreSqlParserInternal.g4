parser grammar PostgreSqlParserInternal;


options {
    tokenVocab = PostgreSqlLexerInternal;
    superClass = PostgreSqlParserBase;
}


@header
{
using Qsi.Data;
using Qsi.Tree;
using Qsi.PostgreSql.Internal;
}
@members
{
}

root
   : EOF 
   | statement (SEMI EOF? | EOF)
   ;

statement
    // DDL
   : alterStatement
   | createStatement
   | dropStatement
   
   // DML
   | selectStatement
   | insertStatement
   | updateStatement
   | deleteStatement
   | truncateStatement
   
   // Uncategorized
   | notifyStatement
   | resetStatement
   | setStatement
   ;

//----------------- DDL statements -------------------------------------------------------------------------------------

/**
* ALTER STATEMENT
*/
alterStatement
    : alterAggregateStatement
    | alterCollationStatement
    | alterConversionStatement
    | alterDatabaseStatement
    | alterDefaultPrivilegesStatement
    | alterDomainStatement
    | alterEventTriggerStatement
    | alterExtensionStatement
    | alterForeignDataWrapperStatement
    | alterForeignTableStatement
    | alterFunctionStatement
    | alterGroupStatement
    | alterIndexStatement
    | alterLanguageStatement
    | alterLargeObjectStatement
    | alterMaterializedViewStatement
    | alterOperatorStatement
    | alterOperatorClassStatement
    | alterOperatorFamilyStatement
    | alterPolicyStatement
    | alterPublicationStatement
    | alterRoleStatement
    | alterRoutineStatement
    | alterRuleStatement
    | alterSchemaStatement
    | alterSequenceStatement
    | alterServerStatement
    | alterStatisticsStatement
    | alterSubscriptionStatement
    | alterTableStatement
    | alterTablespaceStatement
    | alterTextSearchConfigurationStatement
    | alterTextSearchDictionaryStatement
    | alterTextSearchParserStatement
    | alterTextSearchTemplateStatement
    | alterTriggerStatement
    | alterTypeStatement
    | alterUserMappingStatement
    | alterViewStatement
    ;

// TODO: Move this script to a proper place.
// General postfix for alter statement
alterGenericPostfix
    : RENAME TO columnIdentifier
    | SET SCHEMA columnIdentifier
    | OWNER TO role
    ;

/**
 * ALTER AGGREGATE
 *
 * See: https://www.postgresql.org/docs/14/sql-alteraggregate.html
 */
alterAggregateStatement
    : ALTER AGGREGATE aggregateDefinition alterGenericPostfix
    ;

/**
 * ALTER COLLATION
 *
 * See: https://www.postgresql.org/docs/14/sql-altercollation.html
 */
alterCollationStatement
    : ALTER COLLATION qualifiedIdentifier (alterGenericPostfix | REFRESH VERSION_P)
    ;

/**
 * ALTER CONVERSION
 *
 * See: https://www.postgresql.org/docs/14/sql-alterconversion.html
 */
alterConversionStatement
    : ALTER CONVERSION_P qualifiedIdentifier alterGenericPostfix
    ;

/**
 * ALTER DATABASE
 *
 * See: https://www.postgresql.org/docs/14/sql-alterdatabase.html
 */
alterDatabaseStatement
    : ALTER DATABASE columnIdentifier alterDatabasePostfix
    ;

alterDatabasePostfix
    : alterGenericPostfix
    | WITH? createDatabaseItem+
    | SET TABLESPACE columnIdentifier
    | SET setStatementTarget
    | resetStatement
    ;

/**
 * ALTER DEFAULT PRIVILEGES
 *
 * See: https://www.postgresql.org/docs/14/sql-alterdefaultprivileges.html
 */
alterDefaultPrivilegesStatement
    : ALTER DEFAULT PRIVILEGES alterDefaultPrivilegesOption+ alterDefaultPrivilegesAction
    ;

alterDefaultPrivilegesOption    
    : IN_P SCHEMA columnIdentifierList
    | FOR (ROLE | USER) roleList
    ;

alterDefaultPrivilegesAction
    : GRANT privileges ON alterDefaultPrivilegesTarget TO granteeList (WITH GRANT OPTION)?
    | REVOKE (GRANT OPTION FOR)? privileges ON alterDefaultPrivilegesTarget FROM granteeList (CASCADE | RESTRICT)?
    ;

alterDefaultPrivilegesTarget
    : TABLES
    | FUNCTIONS
    | ROUTINES
    | SEQUENCES
    | TYPES_P
    | SCHEMAS
    ;

/**
 * ALTER DOMAIN
 *
 * See: https://www.postgresql.org/docs/14/sql-alterdomain.html
 */
alterDomainStatement
    : ALTER DOMAIN_P qualifiedIdentifier alterDomainPostfix
    ;

alterDomainPostfix
    : alterGenericPostfix
    | RENAME CONSTRAINT columnIdentifier TO columnIdentifier
    | alter_column_default
    | (SET | DROP) NOT NULL_P
    | ADD_P tableConstraint
    | DROP CONSTRAINT (IF_P EXISTS)? columnIdentifier (CASCADE | RESTRICT)?
    | VALIDATE CONSTRAINT columnIdentifier
    ;

alter_column_default
    : SET DEFAULT expression
    | DROP DEFAULT
    ;

/**
 * ALTER EVENT TRIGGER
 *
 * See: https://www.postgresql.org/docs/14/sql-altereventtrigger.html
 */
alterEventTriggerStatement
    : ALTER EVENT TRIGGER columnIdentifier alterEventTriggerPostfix
    ;

alterEventTriggerPostfix
    : alterGenericPostfix
    | ENABLE_P (REPLICA | ALWAYS)?
    | DISABLE_P
    ;

/**
 * ALTER EXTENSION
 *
 * See: https://www.postgresql.org/docs/14/sql-alterextension.html
 */
alterExtensionStatement
    : ALTER EXTENSION columnIdentifier alterExtensionPostfix
    ;

alterExtensionPostfix
    : alterGenericPostfix
    | UPDATE alterExtensionUpdateItem*
    | (ADD_P | DROP) alterExtensionTarget
    ;

alterExtensionUpdateItem
    : TO noReservedWordOrString
    ;

alterExtensionTarget
    : object_type_name columnIdentifier
    | object_type_any_name qualifiedIdentifier
    | AGGREGATE aggregateDefinition
    | CAST '(' type AS type ')'
    | (DOMAIN_P | TYPE_P) type
    | (FUNCTION | PROCEDURE | ROUTINE) functionDefinition
    | OPERATOR operatorDefinition
    | OPERATOR (CLASS | FAMILY) qualifiedIdentifier USING columnIdentifier
    | TRANSFORM FOR type LANGUAGE columnIdentifier
    ;

/**
 * ALTER FOREIGN DATA WRAPPER
 *
 * See: https://www.postgresql.org/docs/14/sql-alterforeigndatawrapper.html
 */
alterForeignDataWrapperStatement
    : ALTER FOREIGN DATA_P WRAPPER columnIdentifier alterForeignDataWrapperOptions
    ;

alterForeignDataWrapperOptions
    : alterGenericPostfix
    | foreignDataWrapperOptions? alterGenericOptions
    | foreignDataWrapperOptions
    ;

/**
 * ALTER FOREIGN TABLE
 *
 * See: https://www.postgresql.org/docs/14/sql-alterforeigntable.html
 */
alterForeignTableStatement
    : ALTER FOREIGN TABLE (IF_P EXISTS)? tableName alterForeignTablePostfix
    ;

alterForeignTablePostfix
    : alterGenericOptions
    | alterTableCommandList
    | RENAME COLUMN? columnIdentifier TO columnIdentifier
    ;

/**
 * ALTER FUNCTION, ALTER PROCEDURE
 *
 * See: https://www.postgresql.org/docs/14/sql-alterfunction.html
 * See also: https://www.postgresql.org/docs/14/sql-alterprocedure.html
 */
alterFunctionStatement
    : ALTER (FUNCTION | PROCEDURE) functionDefinition alterFunctionPostfix
    ;

alterFunctionPostfix
    : alterGenericPostfix
    | NO? DEPENDS ON EXTENSION columnIdentifier
    ;

/**
 * ALTER GROUP
 *
 * See: https://www.postgresql.org/docs/14/sql-altergroup.html
 */
alterGroupStatement
    : ALTER GROUP_P role alterGroupPostfix
    ;

alterGroupPostfix
    : RENAME TO role
    | (ADD_P | DROP) USER roleList
    ;

/**
 * ALTER INDEX
 *
 * See: https://www.postgresql.org/docs/14/sql-alterindex.html
 */
alterIndexStatement
    : ALTER INDEX (IF_P EXISTS)? qualifiedIdentifier alterIndexPostfix
    | ALTER INDEX ALL IN_P TABLESPACE columnIdentifier (OWNED BY roleList)? SET TABLESPACE columnIdentifier NOWAIT?
    | ALTER INDEX qualifiedIdentifier NO? DEPENDS ON EXTENSION columnIdentifier
    ;

alterIndexPostfix
    : alterTableCommandList
    | indexPartitionCommand
    | RENAME TO columnIdentifier
    ;

indexPartitionCommand
    : ATTACH PARTITION qualifiedIdentifier
    ;

/**
 * ALTER LANGUAGE
 *
 * See: https://www.postgresql.org/docs/14/sql-alterlanguage.html
 */
alterLanguageStatement
    : ALTER PROCEDURAL? LANGUAGE columnIdentifier alterGenericPostfix
    ;

/**
 * ALTER LARGE OBJECT
 *
 * See: https://www.postgresql.org/docs/14/sql-alterlargeobject.html
 */
alterLargeObjectStatement
    : ALTER LARGE_P OBJECT_P numericOnly OWNER TO role 
    ;

/**
 * ALTER MATERIALIZED VIEW
 *
 * See: https://www.postgresql.org/docs/14/sql-altermaterializedview.html
 */
alterMaterializedViewStatement
    : ALTER MATERIALIZED VIEW (IF_P EXISTS)? qualifiedIdentifier alterGenericPostfix
    | ALTER MATERIALIZED VIEW (IF_P EXISTS)? qualifiedIdentifier RENAME COLUMN? columnIdentifier TO columnIdentifier
    | ALTER MATERIALIZED VIEW (IF_P EXISTS)? qualifiedIdentifier alterTableCommandList
    | ALTER MATERIALIZED VIEW qualifiedIdentifier NO? DEPENDS ON EXTENSION columnIdentifier
    | ALTER MATERIALIZED VIEW ALL IN_P TABLESPACE columnIdentifier (OWNED BY roleList)? SET TABLESPACE columnIdentifier NOWAIT?
    ;

/**
 * ALTER OPERATOR
 *
 * See: https://www.postgresql.org/docs/14/sql-alteroperator.html
 */
alterOperatorStatement
    : ALTER OPERATOR operatorDefinition alterGenericPostfix
    | ALTER OPERATOR operatorDefinition SET '(' alterOperatorOptionList ')'
    ;

alterOperatorOptionList
    : alterOperatorOption (',' alterOperatorOption)*
    ;

alterOperatorOption
    : columnLabelIdentifier '=' (NONE | alterOperatorArgument)
    ;

alterOperatorArgument
    : functionType
    | reservedKeyword
    | qualifiedOperator
    | numericOnly
    | string
    ;

/**
 * ALTER OPERATOR CLASS
 *
 * See: https://www.postgresql.org/docs/14/sql-alteropclass.html
 */
alterOperatorClassStatement
    : ALTER OPERATOR CLASS qualifiedIdentifier alterGenericPostfix
    ;

/**
 * ALTER OPERATOR FAMILY
 *
 * See: https://www.postgresql.org/docs/14/sql-alteropfamily.html
 */
alterOperatorFamilyStatement
    : ALTER OPERATOR FAMILY qualifiedIdentifier USING columnIdentifier alterOperatorFamilyPostfix
    ;

alterOperatorFamilyPostfix
    : alterGenericPostfix
    | ADD_P createOperatorClassItemList
    | DROP dropOperatorClassItemList
    ;

dropOperatorClassItemList
    : dropOperatorClassItem (',' dropOperatorClassItem)*
    ;

dropOperatorClassItem
    : (OPERATOR | FUNCTION) unsignedInt '(' type (',' type)* ')'
    ;

/**
 * ALTER POLICY
 *
 * See: https://www.postgresql.org/docs/14/sql-alterpolicy.html
 */
alterPolicyStatement
    : ALTER POLICY (IF_P EXISTS)? columnIdentifier ON qualifiedIdentifier RENAME TO columnIdentifier
    | ALTER POLICY columnIdentifier ON qualifiedIdentifier
        (TO roleList)?
        (USING '(' expression ')')?
        (WITH CHECK '(' expression ')')?
    ;

/**
 * ALTER PUBLICATION
 *
 * See: https://www.postgresql.org/docs/14/sql-alterpublication.html
 */
alterPublicationStatement
    : ALTER PUBLICATION columnIdentifier alterPublicationPostfix
    ;

alterPublicationPostfix
    : alterGenericPostfix
    | SET '(' definitionList ')'
    | (ADD_P | SET | DROP) TABLE tableName (',' tableName)*
    ;

/**
 * ALTER ROLE, ALTER USER
 *
 * ALTER USER is an alias for ALTER ROLE.
 *
 * See: https://www.postgresql.org/docs/14/sql-alterrole.html
 * See also: https://www.postgresql.org/docs/14/sql-alteruser.html
 */
alterRoleStatement
    : ALTER (ROLE | USER) ALL? role alterRolePostfix?
    ;

alterRolePostfix
    : WITH? alterRoleOption+
    | (IN_P DATABASE columnIdentifier)? (SET setTarget | resetStatement)
    | RENAME TO role
    ;

alterRoleOption
    : PASSWORD (string | NULL_P)
    | (ENCRYPTED | UNENCRYPTED) PASSWORD string
    | INHERIT
    | CONNECTION LIMIT signedInt
    | VALID UNTIL string
    | USER roleList
    | identifier
    ;

/**
 * ALTER ROUTINE
 *
 * See: https://www.postgresql.org/docs/14/sql-alterroutine.html
 */
alterRoutineStatement
    : ALTER ROUTINE functionDefinition alterRoutinePostfix
    ;

alterRoutinePostfix
    : alterGenericPostfix
    | NO? DEPENDS ON EXTENSION columnIdentifier
    ;

/**
 * ALTER RULE
 *
 * See: https://www.postgresql.org/docs/14/sql-alterrule.html
 */
alterRuleStatement
    : ALTER RULE columnIdentifier ON qualifiedIdentifier RENAME TO columnIdentifier 
    ;

/**
 * ALTER SCHEMA
 *
 * See: https://www.postgresql.org/docs/14/sql-alterschema.html
 */
alterSchemaStatement
    : ALTER SCHEMA columnIdentifier alterGenericPostfix
    ;

/**
 * ALTER SEQUENCE
 *
 * See: https://www.postgresql.org/docs/14/sql-altersequence.html
 */
alterSequenceStatement
    : ALTER SEQUENCE (IF_P EXISTS)? qualifiedIdentifier alterSequencePostfix
    ;

alterSequencePostfix
    : alterGenericPostfix
    | alterTableCommandList
    | sequenceOptionList
    ;

/**
 * ALTER SERVER
 *
 * See: https://www.postgresql.org/docs/14/sql-alterserver.html
 */
alterServerStatement
    : ALTER SERVER columnIdentifier alterServerPostfix
    ;

alterServerPostfix
    : alterGenericPostfix
    | alterGenericOptions
    | (VERSION_P (string | NULL_P)) alterGenericOptions?
    ;

/**
 * ALTER STATISTICS
 *
 * See: https://www.postgresql.org/docs/14/sql-alterstatistics.html
 */
alterStatisticsStatement
    : ALTER STATISTICS columnIdentifier (SET STATISTICS unsignedInt | alterGenericPostfix)
    ;

/**
 * ALTER SUBSCRIPTION
 *
 * See: https://www.postgresql.org/docs/14/sql-altersubscription.html
 */
alterSubscriptionStatement
    : ALTER SUBSCRIPTION columnIdentifier alterSubscriptionPostfix
    ;

alterSubscriptionPostfix
    : alterGenericPostfix
    | SET '(' definitionList ')'
    | CONNECTION string
    | REFRESH PUBLICATION definitionListClause?
    | (SET | ADD_P | DROP) PUBLICATION columnLabelIdentifierList definitionListClause?
    | ENABLE_P
    | DISABLE_P
    ;

/**
 * ALTER TABLE
 *
 * See: https://www.postgresql.org/docs/14/sql-altertable.html
 */
alterTableStatement
    : ALTER TABLE (IF_P EXISTS)? alterTablePostfix
    ;

alterTablePostfix
    : tableName alterGenericPostfix
    | tableName RENAME (COLUMN | CONSTRAINT)? columnIdentifier TO columnIdentifier
    | tableName (alterTableCommandList | partitionCommand)
    | columnIdentifier (OWNED BY roleList)? SET TABLESPACE columnIdentifier NOWAIT?
    ;

alterTableCommandList
    : alterTableCommand (',' alterTableCommand)*
    ;

alterTableCommand
    : ADD_P COLUMN? (IF_P NOT EXISTS)? columnDefinition
    | ALTER COLUMN? columnIdentifier alter_column_default
    | ALTER COLUMN? columnIdentifier (SET | DROP) NOT NULL_P
    | ALTER COLUMN? columnIdentifier DROP EXPRESSION (IF_P EXISTS)?
    | ALTER COLUMN? columnIdentifier DROP IDENTITY_P (IF_P EXISTS)?
    | ALTER COLUMN? columnIdentifier (SET | RESET) '(' relOptionList ')'
    | ALTER COLUMN? columnIdentifier SET STORAGE columnIdentifier
    | ALTER COLUMN? columnIdentifier ADD_P GENERATED (ALWAYS | BY DEFAULT) AS IDENTITY_P ('(' sequenceOptionList ')')?
    | ALTER COLUMN? columnIdentifier alterIdentitiyColumnOption+
    | ALTER COLUMN? (columnIdentifier | unsignedInt) SET STATISTICS signedInt
    | ALTER COLUMN? columnIdentifier (SET DATA_P)? TYPE_P type collateClause? (USING expression)?
    | ALTER COLUMN? columnIdentifier alterGenericOptions
    | DROP COLUMN? (IF_P EXISTS)? columnIdentifier (CASCADE | RESTRICT)?
    | ADD_P tableConstraint
    | ALTER CONSTRAINT columnIdentifier constraintAttribute*
    | VALIDATE CONSTRAINT columnIdentifier
    | DROP CONSTRAINT (IF_P EXISTS)? columnIdentifier (CASCADE | RESTRICT)?
    | CLUSTER ON columnIdentifier
    | SET (WITHOUT (OIDS | CLUSTER) | LOGGED | UNLOGGED)
    | ENABLE_P (ALWAYS | REPLICA)? TRIGGER (columnIdentifier | ALL | USER)
    | DISABLE_P TRIGGER (columnIdentifier | ALL | USER)
    | ENABLE_P (ALWAYS | REPLICA)? RULE columnIdentifier
    | DISABLE_P RULE columnIdentifier
    | NO? INHERIT qualifiedIdentifier
    | OF qualifiedIdentifier
    | NOT OF
    | OWNER TO role
    | SET TABLESPACE columnIdentifier
    | SET '(' relOptionList ')'
    | RESET '(' relOptionList ')'
    | REPLICA IDENTITY_P replicaIdentity
    | (ENABLE_P | DISABLE_P | NO? FORCE) ROW LEVEL SECURITY
    | alterGenericOptions
    ;

alterIdentitiyColumnOption
    : RESTART (WITH numericOnly)?
    | SET (sequenceOptionElement | GENERATED (ALWAYS | BY DEFAULT))
    ;

replicaIdentity
    : NOTHING
    | FULL
    | DEFAULT
    | USING INDEX columnIdentifier
    ;

partitionCommand
    : ATTACH PARTITION qualifiedIdentifier partitionBoundOptions
    | DETACH PARTITION qualifiedIdentifier
    ;

/**
 * ALTER TABLESPACE
 *
 * See: https://www.postgresql.org/docs/14/sql-altertablespace.html
 */
alterTablespaceStatement
    : ALTER TABLESPACE columnIdentifier alterTablespacePostfix
    ;

alterTablespacePostfix
    : (SET | RESET) '(' relOptionList ')'
    | alterGenericPostfix
    ;

/**
 * ALTER TEXT SEARCH CONFIGURATION
 *
 * See: https://www.postgresql.org/docs/14/sql-altertsconfig.html
 */
alterTextSearchConfigurationStatement
    : ALTER TEXT_P SEARCH CONFIGURATION qualifiedIdentifier alterTextSearchConfigurationPostfix
    ;

alterTextSearchConfigurationPostfix
    : alterGenericPostfix
    | ADD_P MAPPING FOR columnIdentifierList WITH qualifiedIdentifierList
    | ALTER MAPPING FOR columnIdentifierList WITH qualifiedIdentifierList
    | ALTER MAPPING REPLACE qualifiedIdentifier WITH qualifiedIdentifier
    | ALTER MAPPING FOR columnIdentifierList REPLACE qualifiedIdentifier WITH qualifiedIdentifier
    | DROP MAPPING FOR columnIdentifierList
    | DROP MAPPING IF_P EXISTS FOR columnIdentifierList
    ;

/**
 * ALTER TEXT SEARCH DICTIONARY
 *
 * See: https://www.postgresql.org/docs/14/sql-altertsdictionary.html
 */
alterTextSearchDictionaryStatement
    : ALTER TEXT_P SEARCH DICTIONARY qualifiedIdentifier ('(' definitionList ')' | alterGenericPostfix)
    ;

/**
 * ALTER TEXT SEARCH PARSER
 *
 * See: https://www.postgresql.org/docs/14/sql-altertsparser.html
 */
alterTextSearchParserStatement
    : ALTER TEXT_P SEARCH PARSER qualifiedIdentifier alterGenericPostfix
    ;

/**
 * ALTER TEXT SEARCH TEMPLATE
 *
 * See: https://www.postgresql.org/docs/14/sql-altertstemplate.html
 */
alterTextSearchTemplateStatement
    : ALTER TEXT_P SEARCH TEMPLATE qualifiedIdentifier alterGenericPostfix
    ;

/**
 * ALTER TRIGGER
 *
 * See: https://www.postgresql.org/docs/14/sql-altertrigger.html
 */
alterTriggerStatement
    : ALTER TRIGGER columnIdentifier ON qualifiedIdentifier alterTriggerPostfix
    ;

alterTriggerPostfix
    : RENAME TO columnIdentifier
    | NO? DEPENDS ON EXTENSION columnIdentifier
    ;

/**
 * ALTER TYPE
 *
 * See: https://www.postgresql.org/docs/14/sql-altertype.html
 */
alterTypeStatement
    : ALTER TYPE_P qualifiedIdentifier alterTypePostfix
    ;

alterTypePostfix
    : alterGenericPostfix
    | alterTypeCommandList
    | SET '(' alterOperatorOptionList ')'
    | ADD_P VALUE_P (IF_P NOT EXISTS)? string ((BEFORE | AFTER) string)?
    | RENAME VALUE_P string TO string
    | RENAME ATTRIBUTE columnIdentifier TO columnIdentifier (CASCADE | RESTRICT)?
    ;

alterTypeCommandList
    : alterTypeCommand (',' alterTypeCommand)*
    ;

alterTypeCommand
    : ADD_P ATTRIBUTE columnDefinitionList (CASCADE | RESTRICT)?
    | DROP ATTRIBUTE (IF_P EXISTS)? columnIdentifier (CASCADE | RESTRICT)?
    | ALTER ATTRIBUTE columnIdentifier (SET DATA_P)? TYPE_P type collateClause? (CASCADE | RESTRICT)?
    ;

/**
 * ALTER USER MAPPING
 *
 * See: https://www.postgresql.org/docs/14/sql-alterusermapping.html
 */
alterUserMappingStatement
    : ALTER USER MAPPING FOR (role | USER) SERVER columnIdentifier alterGenericOptions
    ;

/**
 * ALTER VIEW
 *
 * See: https://www.postgresql.org/docs/14/sql-alterview.html
 */
alterViewStatement
    : ALTER VIEW (IF_P EXISTS)? columnIdentifier alterViewPostfix
    ;

alterViewPostfix
    : alterGenericPostfix
    | alterTableCommandList
    ;

/**
 * CREATE STATEMENT
 */
createStatement
    : createAccessMethodStatement
    | createAggregateStatement
    | createCastStatement
    | createCollationStatement
    | createConversionStatement
    | createDatabaseStatement
    | createDomainStatement
    | createEventTriggerStatement
    | createExtensionStatement
    | createForeignDataWrapperStatement
    | createForeignTableStatement
    | createFunctionStatement
    | createIndexStatement
    | createLanguageStatement
    | createMaterializedViewStatement
    | createOperatorStatement
    | createOperatorClassStatement
    | createOperatorFamilyStatement
    | createPolicyStatement
    | createPublicationStatement
    | createRoleStatement
    | createRuleStatement
    | createSchemaStatement
    | createSequenceStatement
    | createServerStatement
    | createTableStatement
    | createTablespaceStatement
    | createTextSearchConfigurationStatement
    | createTextSearchDictionaryStatement
    | createTextSearchParserStatement
    | createTextSearchTemplateStatement
    | createTransformStatement
    | createTriggerStatement
    | createTypeStatement
    | createUserMappingStatement
    | createViewStatement
    ;

/**
 * CREATE ACCESS METHOD
 *
 * See: https://www.postgresql.org/docs/14/sql-create-access-method.html
 */
createAccessMethodStatement
    : CREATE ACCESS METHOD columnIdentifier TYPE_P (INDEX | TABLE) HANDLER qualifiedIdentifier
    ;

/**
 * CREATE AGGREGATE
 *
 * See: https://www.postgresql.org/docs/14/sql-createaggregate.html
 */
createAggregateStatement
    : CREATE (OR REPLACE)? AGGREGATE functionName createAggregateArgumentOption
    ;

createAggregateArgumentOption
    : '(' aggregateArgumentDefinitions ')' '(' definitionList ')'
    | '(' aggregateArgumentListOldSyntax ')'
    ;

/**
 * CREATE CAST
 *
 * See: https://www.postgresql.org/docs/14/sql-createcast.html
 */
createCastStatement
    : CREATE CAST '(' type AS type ')' createCastOption castContext?
    ;

createCastOption
    : WITH FUNCTION functionDefinition
    | WITHOUT FUNCTION
    | WITH INOUT
    ;

castContext
    : AS (IMPLICIT_P | ASSIGNMENT)
    ;

/**
 * CREATE COLLATION
 *
 * See: https://www.postgresql.org/docs/14/sql-createcollation.html
 */
createCollationStatement
    : CREATE COLLATION (IF_P NOT EXISTS)? qualifiedIdentifier '(' definitionList ')'
    ;

/**
 * CREATE CONVERSION
 *
 * See: https://www.postgresql.org/docs/14/sql-createconversion.html
 */
createConversionStatement
    : CREATE DEFAULT? CONVERSION_P qualifiedIdentifier FOR string TO string FROM qualifiedIdentifier
    ;

/**
 * CREATE DATABASE
 *
 * See: https://www.postgresql.org/docs/14/sql-createdatabase.html
 */
createDatabaseStatement
    : CREATE DATABASE columnIdentifier WITH? createDatabaseItem*
    ;

createDatabaseItem
    : createDatabaseItemName EQUAL? (signedInt | booleanOrString | DEFAULT)
    ;

createDatabaseItemName
    : identifier
    | CONNECTION LIMIT
    | ENCODING
    | LOCATION
    | OWNER
    | TABLESPACE
    | TEMPLATE
    ;

/**
 * CREATE DOMAIN
 *
 * See: https://www.postgresql.org/docs/14/sql-createdomain.html
 */
createDomainStatement
    : CREATE DOMAIN_P qualifiedIdentifier AS? type columnConstraint*
    ;

/**
 * CREATE EVENT TRIGGER
 *
 * See: https://www.postgresql.org/docs/14/sql-createeventtrigger.html
 */
createEventTriggerStatement
    : CREATE EVENT TRIGGER columnIdentifier ON columnLabelIdentifier (WHEN eventTriggerWhenList)?
        EXECUTE (FUNCTION | PROCEDURE) functionName '(' ')'
    ;

eventTriggerWhenList
    : eventTriggerItem (COMMA eventTriggerItem)*
    ;

eventTriggerItem
    : columnIdentifier IN_P '(' string (COMMA string)* ')'
    ;

/**
 * CREATE EXTENSION
 *
 * See: https://www.postgresql.org/docs/14/sql-createextension.html
 */ 
createExtensionStatement
    : CREATE EXTENSION (IF_P NOT EXISTS)? columnIdentifier WITH? createExtensionOptionList?
    ;

createExtensionOptionList
    : createExtensionOption+
    ;

createExtensionOption
    : SCHEMA columnIdentifier
    | VERSION_P noReservedWordOrString
    | FROM noReservedWordOrString
    | CASCADE
    ;

/**
 * CREATE FOREIGN DATA WRAPPER
 *
 * See: https://www.postgresql.org/docs/14/sql-createforeigndatawrapper.html
 */
createForeignDataWrapperStatement
    : CREATE FOREIGN DATA_P WRAPPER qualifiedIdentifier foreignDataWrapperOptions? genericOptions?
    ;

foreignDataWrapperOptions
    : foreignDataWrapperOption (COMMA foreignDataWrapperOption)*
    ;

foreignDataWrapperOption
    : HANDLER qualifiedIdentifier
    | NO HANDLER
    | VALIDATOR qualifiedIdentifier
    | VALIDATOR
    ;

/**
 * CREATE FOREIGN TABLE
 *
 * See: https://www.postgresql.org/docs/14/sql-createforeigntable.html
 */
createForeignTableStatement
    : CREATE FOREIGN TABLE (IF_P NOT EXISTS)? qualifiedIdentifier createTableOptions SERVER columnIdentifier genericOptions?
    ;

/**
 * CREATE FUNCTION, CREATE PROCEDURE
 *
 * See: https://www.postgresql.org/docs/14/sql-createfunction.html
 * See also: https://www.postgresql.org/docs/14/sql-createprocedure.html
 */
createFunctionStatement
    : CREATE (OR REPLACE)? (FUNCTION | PROCEDURE) functionName '(' argumentDefinitionWithDefaultList ')'
        (RETURNS (functionType | TABLE '(' functionColumnDefinitionList ')'))?
        createFunctionOptionList
    ;

functionColumnDefinitionList
    : functionColumnDefinition (COMMA functionColumnDefinition)*
    ;

functionColumnDefinition
    : typeFunctionIdentifier functionType
    ;

createFunctionOptionList
    : createFunctionOption+ /*{ ParseRoutineBody(_localctx); }*/ // From open-source PG parser, dunno why it's here.
    ;

createFunctionOption
    : AS string (',' string)? /* locals[ParserRuleContext definition] */ // From open-source PG parser, dunno why it's here.
    | LANGUAGE noReservedWordOrString
    | TRANSFORM transformTypeList
    | WINDOW
    | functionStatementOption
    ;

transformTypeList
    : FOR TYPE_P type (COMMA FOR TYPE_P type)*
    ;

functionStatementOption
    : (CALLED | RETURNS NULL_P) ON NULL_P INPUT_P
    | STRICT_P
    | IMMUTABLE
    | STABLE
    | VOLATILE
    | EXTERNAL? SECURITY (DEFINER | INVOKER)
    | NOT? LEAKPROOF
    | (COST | ROWS) numericOnly
    | SUPPORT qualifiedIdentifier
    | SET setTarget
    | resetStatement
    | PARALLEL columnIdentifier
    ;

/**
 * CREATE INDEX
 */
createIndexStatement
    : CREATE UNIQUE? INDEX CONCURRENTLY? (IF_P NOT EXISTS)? columnIdentifier
        ON tableName (USING columnIdentifier)? '(' indexList ')'
        includeClause?
        withOptionsClause?
        tableSpaceClause?
        whereClause?
    ;

/**
 * CREATE LANGUAGE
 *
 * See: https://www.postgresql.org/docs/14/sql-createlanguage.html
 */
createLanguageStatement
    : CREATE (OR REPLACE)? TRUSTED? PROCEDURAL? LANGUAGE columnIdentifier
        (HANDLER qualifiedIdentifier (INLINE_P qualifiedIdentifier)? validatorClause?)?
    ;

validatorClause
    : VALIDATOR qualifiedIdentifier
    | NO VALIDATOR
    ;

/**
 * CREATE MATERIALIZED VIEW
 *
 * See: https://www.postgresql.org/docs/14/sql-creatematerializedview.html
 */
createMaterializedViewStatement
    : CREATE UNLOGGED? MATERIALIZED VIEW (IF_P NOT EXISTS)? createMatViewTarget
        AS selectStatement (WITH (DATA_P | NO DATA_P))?
    ;

createMatViewTarget
    : qualifiedIdentifier ('(' columnIdentifierList ')')?
        usingClause?
        withOptionsClause?
        tableSpaceClause?
    ;

/**
 * CREATE OPERATOR
 *
 * See: https://www.postgresql.org/docs/14/sql-createoperator.html
 */
createOperatorStatement
    : CREATE OPERATOR operator '(' definitionList ')'
    ;

/**
 * CREATE OPERATOR CLASS
 *
 * See: https://www.postgresql.org/docs/14/sql-createopclass.html
 */
createOperatorClassStatement
    : CREATE OPERATOR CLASS qualifiedIdentifier DEFAULT?
        FOR TYPE_P type USING columnIdentifier
        (FAMILY qualifiedIdentifier)? AS createOperatorClassItemList
    ;

createOperatorClassItemList
    : createOperatorClassItem (',' createOperatorClassItem)*
    ;

createOperatorClassItem
    : OPERATOR unsignedInt operatorDefinition createOperatorClassPurpose? RECHECK?
    | FUNCTION unsignedInt ('(' type (',' type)* ')')? argumentDefinitionList
    | STORAGE type
    ;

createOperatorClassPurpose
    : FOR (SEARCH | ORDER BY qualifiedIdentifier)
    ;

/**
 * CREATE OPERATOR FAMILY
 *
 * See: https://www.postgresql.org/docs/14/sql-createopfamily.html
 */
createOperatorFamilyStatement
    : CREATE OPERATOR FAMILY qualifiedIdentifier USING columnIdentifier
    ;

/**
 * CREATE POLICY
 *
 * See: https://www.postgresql.org/docs/14/sql-createpolicy.html
 */
createPolicyStatement
    : CREATE POLICY columnIdentifier ON qualifiedIdentifier
        (AS identifier)?
        (FOR (ALL | SELECT | INSERT | UPDATE | DELETE_P))?
        (TO roleList)?
        (USING expressionParens)?
        (WITH CHECK expressionParens)?
    ;

/**
 * CREATE PUBLICATION
 *
 * See: https://www.postgresql.org/docs/14/sql-createpublication.html
 */
createPublicationStatement
    : CREATE PUBLICATION qualifiedIdentifier forTableClause? definitionListClause?
    ;

forTableClause
    : FOR (TABLE tableName (',' tableName)* | ALL TABLES)
    ;

/**
 * CREATE ROLE, CREATE USER, CREATE GROUP
 *
 * CREATE USER, CREATE GROUP is an alias for CREATE ROLE.
 *
 * See: https://www.postgresql.org/docs/14/sql-createrole.html
 * See also: https://www.postgresql.org/docs/14/sql-createuser.html
 * See also: https://www.postgresql.org/docs/14/sql-creategroup.html
 */
createRoleStatement
    : CREATE (ROLE | USER | GROUP_P) role WITH? createRoleOption*
    ;

createRoleOption
    : alterRoleOption
    | SYSID unsignedInt
    | (ADMIN | ROLE | IN_P (ROLE | GROUP_P)) roleList
    ;

/**
 * CREATE RULE
 *
 * See: https://www.postgresql.org/docs/14/sql-createrule.html
 */
createRuleStatement
    : CREATE (OR REPLACE)? RULE columnIdentifier
        AS ON (SELECT | UPDATE | INSERT | DELETE_P)
        TO qualifiedIdentifier whereClause?
        DO (INSTEAD | ALSO)? ruleAction
    ;

ruleAction
    : NOTHING
    | ruleActionStatement
    | '(' ruleActionStatementList ')'
    ;

ruleActionStatementList
    : ruleActionStatement (';' ruleActionStatement)*
    ;

ruleActionStatement
    : selectStatement
    | insertStatement
    | updateStatement
    | deleteStatement
    | notifyStatement
    ;

/**
 * CREATE SCHEMA
 *
 * See: https://www.postgresql.org/docs/14/sql-createschema.html
 */
createSchemaStatement
    : CREATE SCHEMA (IF_P NOT EXISTS)? (columnIdentifier? AUTHORIZATION role | columnIdentifier) schemaStatement*
    ;

schemaStatement
    : createTableStatement
    | createIndexStatement
    | createSequenceStatement
//    | createTrigger // TODO: Implement later.
//    | grantStatement
//    | viewStatement
    ;

/**
 * CREATE SEQUENCE
 *
 * See: https://www.postgresql.org/docs/14/sql-createsequence.html
 */
createSequenceStatement
    : CREATE tempOption? SEQUENCE (IF_P NOT EXISTS)? qualifiedIdentifier ('(' sequenceOptionList ')')?
    ;

/**
 * CREATE SERVER
 *
 * See: https://www.postgresql.org/docs/14/sql-createserver.html
 */
createServerStatement
    : CREATE SERVER (IF_P NOT EXISTS)? columnIdentifier
        (TYPE_P string)?
        (VERSION_P (string | NULL_P))?
        FOREIGN DATA_P WRAPPER columnIdentifier genericOptions?
    ;

/**
 * CREATE TABLE
 *
 * See: https://www.postgresql.org/docs/14/sql-createtable.html
 */
createTableStatement
    : CREATE tempOption? TABLE (IF_P NOT EXISTS)? qualifiedIdentifier createTableOptions
        partitionByClause?
        usingClause?
        withOptionsClause?
        onCommitClause?
        tableSpaceClause?
    ;

tempOption
    : TEMPORARY
    | TEMP
    | LOCAL (TEMPORARY | TEMP)
    | GLOBAL (TEMPORARY | TEMP)
    | UNLOGGED
    ;

createTableOptions
    : '(' tableElementList? ')' inheritsClause? 
    | OF qualifiedIdentifier typedTableElementList?
    | partitionOfClause
    ;

tableElementList
    : tableElement (COMMA tableElement)*
    ;

tableElement
    : columnDefinition
    | tableLikeClause
    | tableConstraint
    ;

tableLikeClause
    : LIKE qualifiedIdentifier tableLikeOption*
    ;

tableLikeOption
    : (INCLUDING | EXCLUDING) simpleTableLikeOption
    ;

simpleTableLikeOption
    : COMMENTS
    | CONSTRAINTS
    | DEFAULTS
    | IDENTITY_P
    | GENERATED
    | INDEXES
    | STATISTICS
    | STORAGE
    | ALL
    ;

/**
 * CREATE TABLESPACE
 *
 * See: https://www.postgresql.org/docs/14/sql-createstatistics.html
 */
createTablespaceStatement
    : CREATE STATISTICS (IF_P NOT EXISTS)? qualifiedIdentifier
        ('(' columnIdentifierList ')')?
        ON expressionList
        FROM fromItemList
    ;

/**
 * CREATE TEXT SEARCH CONFIGURATION
 *
 * See: https://www.postgresql.org/docs/14/sql-createtsconfig.html
 */
createTextSearchConfigurationStatement
    : CREATE TEXT_P SEARCH CONFIGURATION qualifiedIdentifier '(' definitionList ')'
    ;

/**
 * CREATE TEXT SEARCH DICTIONARY
 *
 * See: https://www.postgresql.org/docs/14/sql-createtsdictionary.html
 */
createTextSearchDictionaryStatement
    : CREATE TEXT_P SEARCH DICTIONARY qualifiedIdentifier '(' definitionList ')'
    ;

/**
 * CREATE TEXT SEARCH PARSER
 *
 * See: https://www.postgresql.org/docs/14/sql-createtsparser.html
 */
createTextSearchParserStatement
    : CREATE TEXT_P SEARCH PARSER qualifiedIdentifier '(' definitionList ')'
    ;

/**
 * CREATE TEXT SEARCH TEMPLATE
 *
 * See: https://www.postgresql.org/docs/14/sql-createtsparser.html
 */
createTextSearchTemplateStatement
    : CREATE TEXT_P SEARCH TEMPLATE qualifiedIdentifier '(' definitionList ')'
    ;

/**
 * CREATE TRANSFORM
 *
 * See: https://www.postgresql.org/docs/14/sql-createtransform.html
 */
createTransformStatement
    : CREATE (OR REPLACE)? TRANSFORM FOR type LANGUAGE columnIdentifier '(' transformElementList ')'
    ;

transformElementList
    : FROM SQL_P WITH FUNCTION functionDefinition ',' TO SQL_P WITH FUNCTION functionDefinition
    | TO SQL_P WITH FUNCTION functionDefinition ',' FROM SQL_P WITH FUNCTION functionDefinition
    | FROM SQL_P WITH FUNCTION functionDefinition
    | TO SQL_P WITH FUNCTION functionDefinition
    ;

/**
 * CREATE TRIGGER
 *
 * See: https://www.postgresql.org/docs/14/sql-createtrigger.html
 */
createTriggerStatement
    : CREATE (OR REPLACE)? TRIGGER columnIdentifier triggeractiontime triggerEvents
        ON qualifiedIdentifier triggerReferencing? triggerFor triggerWhen?
        triggerExecuteClause
    | CREATE (OR REPLACE)? CONSTRAINT TRIGGER columnIdentifier AFTER triggerEvents
        ON qualifiedIdentifier ((FROM qualifiedIdentifier)? constraintAttribute* FOR EACH ROW) triggerWhen?
        triggerExecuteClause
    ;

triggerExecuteClause
    : EXECUTE (FUNCTION | PROCEDURE) functionName '(' triggerFunctionArguments? ')'
    ;

triggeractiontime
    : BEFORE
    | AFTER
    | INSTEAD OF
    ;

triggerEvents
    : triggerEvent (OR triggerEvent)*
    ;

triggerEvent
    : INSERT
    | DELETE_P
    | UPDATE (OF columnIdentifierList)?
    | TRUNCATE
    ;

triggerReferencing
    : REFERENCING triggerTranslation+
    ;

triggerTranslation
    : (NEW | OLD) (TABLE | ROW) AS? columnIdentifier
    ;

triggerFor
    : FOR EACH? (ROW | STATEMENT)
    ;

triggerWhen
    : WHEN expressionParens
    ;

triggerFunctionArguments
    : triggerFunctionArgument (',' triggerFunctionArgument)*
    ;

triggerFunctionArgument
    : unsignedInt
    | float
    | string
    | columnLabelIdentifier
    ;

/**
 * CREATE TYPE
 *
 * See: https://www.postgresql.org/docs/14/sql-createtype.html
 */
createTypeStatement
    : CREATE TYPE_P qualifiedIdentifier createTypeOption?
    ;

createTypeOption
    : '(' definitionList ')'
    | AS '(' columnDefinitionList? ')'
    | AS ENUM_P '(' enumList? ')'
    | AS RANGE '(' definitionList ')'
    ;

enumList
    : string (',' string)*
    ;

/**
 * CREATE USER MAPPING
 *
 * See: https://www.postgresql.org/docs/14/sql-createusermapping.html
 */
createUserMappingStatement
    : CREATE USER MAPPING (IF_P NOT EXISTS)? FOR (role | USER) SERVER columnIdentifier genericOptions
    ;

/**
 * CREATE VIEW
 *
 * See: https://www.postgresql.org/docs/14/sql-createview.html
 */
createViewStatement
    : CREATE (OR REPLACE)? tempOption? RECURSIVE? VIEW qualifiedIdentifier
        (columnIdentifierList | '(' columnIdentifierList ')')?
        withOptionsClause?
        AS selectStatement (WITH (CASCADED | LOCAL)? CHECK OPTION)?
    ;

/**
 * DROP STATEMENT
 */
dropStatement
    : dropAccessMethodStatement
    | dropAggregateStatement
    | dropCastStatement
    | dropCollationStatement
    | dropConversionStatement
    | dropDatabaseStatement
    | dropDomainStatement
    | dropEventTriggerStatement
    | dropExtensionStatement
    | dropForeignDataWrapperStatement
    | dropForeignTableStatement
    | dropFunctionStatement
    | dropIndexStatement
    | dropLanguageStatement
    | dropMaterializedViewStatement
    | dropOperatorStatement
    | dropOperatorClassStatement
    | dropOperatorFamilyStatement
    | dropOwnedStatement
    | dropPolicyStatement
    | dropPublicationStatement
    | dropRoleStatement
    | dropRuleStatement
    | dropSchemaStatement
    | dropSequenceStatement
    | dropServerStatement
    | dropStatisticsStatement
    | dropSubscriptionStatement
    | dropTableStatement
    | dropTablespaceStatement
    | dropTextSearchConfigurationStatement
    | dropTextSearchDictionaryStatement
    | dropTextSearchParserStatement
    | dropTextSearchTemplateStatement
    | dropTransformStatement
    | dropTriggerStatement
    | dropTypeStatement
    | dropUserMappingsStatement
    | dropViewStatement
;

/**
 * DROP ACCESS METHOD
 *
 * See: https://www.postgresql.org/docs/14/sql-drop-access-method.html
 */
dropAccessMethodStatement
    : DROP ACCESS METHOD (IF_P EXISTS)? columnIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP AGGREGATE
 *
 * See: https://www.postgresql.org/docs/14/sql-dropaggregate.html
 */
dropAggregateStatement
    : DROP AGGREGATE (IF_P EXISTS)? aggregateDefinition (',' aggregateDefinition)* (CASCADE | RESTRICT)?
    ;

/**
 * DROP CAST
 *
 * See: https://www.postgresql.org/docs/14/sql-dropcast.html
 */
dropCastStatement
    : DROP CAST (IF_P EXISTS)? '(' type AS type ')' (CASCADE | RESTRICT)?
    ;

/**
 * DROP COLLATION
 *
 * See: https://www.postgresql.org/docs/14/sql-dropcollation.html
 */
dropCollationStatement
    : DROP COLLATION (IF_P EXISTS)? qualifiedIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP CONVERSION
 *
 * See: https://www.postgresql.org/docs/14/sql-dropconversion.html
 */
dropConversionStatement
    : DROP CONVERSION_P (IF_P EXISTS)? qualifiedIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP DATABASE
 *
 * See: https://www.postgresql.org/docs/14/sql-dropdatabase.html
 */
dropDatabaseStatement
    : DROP DATABASE (IF_P EXISTS)? columnIdentifier (WITH? '(' FORCE? ')')?
    ;

/**
 * DROP DOMAIN
 *
 * See: https://www.postgresql.org/docs/14/sql-dropdomain.html
 */
dropDomainStatement
    : DROP DOMAIN_P (IF_P EXISTS)? type (',' type)* (CASCADE | RESTRICT)?
    ;

/**
 * DROP EVENT TRIGGER
 *
 * See: https://www.postgresql.org/docs/14/sql-dropeventtrigger.html
 */
dropEventTriggerStatement
    : DROP EVENT TRIGGER (IF_P EXISTS)? columnIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP EXTENSION
 *
 * See: https://www.postgresql.org/docs/14/sql-dropextension.html
 */
dropExtensionStatement
    : DROP EXTENSION (IF_P EXISTS)? columnIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP FOREIGN DATA WRAPPER
 *
 * See: https://www.postgresql.org/docs/14/sql-dropforeigndatawrapper.html
 */
dropForeignDataWrapperStatement
    : DROP FOREIGN DATA_P WRAPPER (IF_P EXISTS)? columnIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP FOREIGN TABLE
 *
 * See: https://www.postgresql.org/docs/14/sql-dropforeigntable.html
 */
dropForeignTableStatement
    : DROP FOREIGN TABLE (IF_P EXISTS)? qualifiedIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP FUNCTION, DROP PROCEDURE, DROP ROUTINE
 *
 * See: https://www.postgresql.org/docs/14/sql-dropfunction.html
 * See also: https://www.postgresql.org/docs/14/sql-dropprocedure.html
 * See also: https://www.postgresql.org/docs/14/sql-droproutine.html
 */
dropFunctionStatement
    : DROP (FUNCTION | PROCEDURE | ROUTINE) (IF_P EXISTS)? functionDefinitionList (CASCADE | RESTRICT)?
    ;

/**
 * DROP INDEX
 *
 * See: https://www.postgresql.org/docs/14/sql-dropindex.html
 */
dropIndexStatement
    : DROP INDEX CONCURRENTLY? (IF_P EXISTS)? qualifiedIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP LANGUAGE
 *
 * See: https://www.postgresql.org/docs/14/sql-droplanguage.html
 */
dropLanguageStatement
    : DROP PROCEDURAL? LANGUAGE (IF_P EXISTS)? columnIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP MATERIALIZED VIEW
 *
 * See: https://www.postgresql.org/docs/14/sql-dropmaterializedview.html
 */
dropMaterializedViewStatement
    : DROP MATERIALIZED VIEW (IF_P EXISTS)? qualifiedIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP OPERATOR
 *
 * See: https://www.postgresql.org/docs/14/sql-dropoperator.html
 */
dropOperatorStatement
    : DROP OPERATOR (IF_P EXISTS)? operatorDefinitionList (CASCADE | RESTRICT)?
    ;

/**
 * DROP OPERATOR CLASS
 *
 * See: https://www.postgresql.org/docs/14/sql-dropopclass.html
 */
dropOperatorClassStatement
    : DROP OPERATOR CLASS (IF_P EXISTS)? qualifiedIdentifier USING columnIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP OPERATOR FAMILY
 *
 * See: https://www.postgresql.org/docs/14/sql-dropopfamily.html
 */
dropOperatorFamilyStatement
    : DROP OPERATOR FAMILY (IF_P EXISTS)? qualifiedIdentifier USING columnIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP OWNED
 *
 * See: https://www.postgresql.org/docs/14/sql-drop-owned.html
 */
dropOwnedStatement
    : DROP OWNED BY roleList (CASCADE | RESTRICT)?
    ;

/**
 * DROP POLICY
 *
 * See: https://www.postgresql.org/docs/14/sql-droppolicy.html
 */
dropPolicyStatement
    : DROP POLICY (IF_P EXISTS)? columnIdentifier ON qualifiedIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP PUBLICATION
 *
 * See: https://www.postgresql.org/docs/14/sql-droppublication.html
 */
dropPublicationStatement
    : DROP PUBLICATION (IF_P EXISTS)? qualifiedIdentifierList (CASCADE | RESTRICT)?
    ;
/**
 * DROP ROLE, DROP GROUP, DROP USER
 *
 * DROP GROUP and DROP USER are an alias for DROP ROLE.
 *
 * See: https://www.postgresql.org/docs/14/sql-droprole.html
 * See also: https://www.postgresql.org/docs/14/sql-dropgroup.html
 * See also: https://www.postgresql.org/docs/14/sql-dropuser.html
 */
dropRoleStatement
    : DROP (ROLE | GROUP_P | USER) (IF_P EXISTS)? roleList 
    ;

/**
 * DROP RULE
 *
 * See: https://www.postgresql.org/docs/14/sql-droprule.html
 */
dropRuleStatement
    : DROP RULE (IF_P EXISTS)? columnIdentifier ON qualifiedIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP SCHEMA
 *
 * See: https://www.postgresql.org/docs/14/sql-dropschema.html
 */
dropSchemaStatement
    : DROP SCHEMA (IF_P EXISTS)? columnIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP SEQUENCE
 *
 * See: https://www.postgresql.org/docs/14/sql-dropsequence.html
 */
dropSequenceStatement
    : DROP SEQUENCE (IF_P EXISTS)? qualifiedIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP SERVER
 *
 * See: https://www.postgresql.org/docs/14/sql-dropserver.html
 */
dropServerStatement
    : DROP SERVER (IF_P EXISTS)? columnIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP STATISTICS
 *
 * See: https://www.postgresql.org/docs/14/sql-dropstatistics.html
 */
dropStatisticsStatement
    : DROP STATISTICS (IF_P EXISTS)? qualifiedIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP SUBSCRIPTION
 *
 * See: https://www.postgresql.org/docs/14/sql-dropsubscription.html
 */
dropSubscriptionStatement
    : DROP SUBSCRIPTION (IF_P EXISTS)? columnIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP TABLE
 *
 * See: https://www.postgresql.org/docs/14/sql-droptable.html
 */
dropTableStatement
    : DROP TABLE (IF_P EXISTS)? qualifiedIdentifierList (CASCADE | RESTRICT)?
    ;

/**
 * DROP TABLESPACE
 *
 * See: https://www.postgresql.org/docs/14/sql-droptablespace.html
 */
dropTablespaceStatement
    : DROP TABLESPACE (IF_P EXISTS)? columnIdentifier
    ;

/**
 * DROP TEXT SEARCH CONFIGURATION
 *
 * See: https://www.postgresql.org/docs/14/sql-droptsconfig.html
 */
dropTextSearchConfigurationStatement
    : DROP TEXT_P SEARCH CONFIGURATION (IF_P EXISTS)? qualifiedIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP TEXT SEARCH DICTIONARY
 *
 * See: https://www.postgresql.org/docs/14/sql-droptsdictionary.html
 */
dropTextSearchDictionaryStatement
    : DROP TEXT_P SEARCH DICTIONARY (IF_P EXISTS)? qualifiedIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP TEXT SEARCH PARSER
 *
 * See: https://www.postgresql.org/docs/14/sql-droptsparser.html
 */
dropTextSearchParserStatement
    : DROP TEXT_P SEARCH PARSER (IF_P EXISTS)? qualifiedIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP TEXT SEARCH TEMPLATE
 *
 * See: https://www.postgresql.org/docs/14/sql-droptstemplate.html
 */
dropTextSearchTemplateStatement
    : DROP TEXT_P SEARCH TEMPLATE (IF_P EXISTS)? qualifiedIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP TRANSFORM
 *
 * See: https://www.postgresql.org/docs/14/sql-droptransform.html
 */
dropTransformStatement
    : DROP TRANSFORM (IF_P EXISTS)? FOR type LANGUAGE columnIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP TRIGGER
 *
 * See: https://www.postgresql.org/docs/14/sql-droptrigger.html
 */
dropTriggerStatement
    : DROP TRIGGER (IF_P EXISTS)? columnIdentifier ON qualifiedIdentifier (CASCADE | RESTRICT)?
    ;

/**
 * DROP TYPE
 *
 * See: https://www.postgresql.org/docs/14/sql-droptype.html
 */
dropTypeStatement
    : DROP TYPE_P (IF_P EXISTS)? type (',' type)* (CASCADE | RESTRICT)?
    ;

/**
 * DROP USER MAPPING
 *
 * See: https://www.postgresql.org/docs/14/sql-dropusermapping.html
 */
dropUserMappingsStatement
    : DROP USER MAPPING (IF_P EXISTS)? FOR (role | USER) SERVER columnIdentifier
    ;

/**
 * DROP VIEW
 *
 * See: https://www.postgresql.org/docs/14/sql-dropview.html
 */
dropViewStatement
    : DROP VIEW (IF_P EXISTS)? qualifiedIdentifierList (CASCADE | RESTRICT)?
    ;

//----------------- DML statements -------------------------------------------------------------------------------------

/**
 * SELECT
 */
selectStatement
    : queryExpressionParens                                 // (SELECT ...)
    | queryExpression                                       // SELECT ...
    ;

// Expression without parantheses.
queryExpression
    : queryExpressionNoWith                                 // SELECT ...
    | withClause queryExpressionNoWith                      // WITH ... SELECT ...
    ;

// Expression without with clause.
// TODO: Optimize; extremely slow when intaking many brackets.
queryExpressionNoWith
    : (queryExpressionBody | queryExpressionParens)         // SELECT ...
    orderByClause?                                          // ORDER BY ...
    limitClause?                                            // LIMIT ... (OFFSET ...)
    forClause?                                              // FOR ... (OF ...)
    ;

// Expression with parantheses.
queryExpressionParens
    : '(' (queryExpressionParens | queryExpression) ')'
    ;

// Simpler query expression.
queryExpressionBody
    : (queryPrimary | queryExpressionParens) queryExpressionSet*;

// Query expression with set expressions(union, intersect, except).
queryExpressionSet
    : setOperator setOperatorOption? (queryPrimary | queryExpressionParens)
    ;

// Primary query.
queryPrimary
    : SELECT selectOption* selectItemList
        intoClause?
        fromClause?
        whereClause?
        groupByClause?
        havingClause?
        windowClause?                           # selectQueryPrimary
    | VALUES valueList                          # valuesQueryPrimary
    | TABLE tableName                           # tableQueryPrimary
    ;

// Values
valueList
    : valueItem (COMMA valueItem)*
    ;

valueItem
    : '(' valueColumnList ')'
    ;

valueColumnList
    : valueColumn (COMMA valueColumn)*
    ;

valueColumn
    : expression
    | DEFAULT
    ;

// Columns of the select statement.
selectItemList
    : (selectItem) (COMMA selectItem)*
    ;

selectItem
    : expression aliasClause? 
    | STAR
    ;

// Options for select statement.
selectOption
    : ALL
    | DISTINCT (ON '(' expressionList ')')?
    ;

/**
 * INSERT
 */
insertStatement
    : withClause insertStatementNoWith
    | insertStatementNoWith
    ;

insertStatementNoWith
    : INSERT INTO tableName (AS columnIdentifier)?
        ('(' qualifiedIdentifierList ')')?
        overridingOption?
        (DEFAULT VALUES | queryPrimary)
        onConflictClause?
        returningClause?
    ;
    
overridingOption
    : OVERRIDING (SYSTEM_P | USER) VALUE_P
    ;
    
onConflictClause
    : ON CONFLICT conflictTarget? conflictAction
    ;
    
conflictTarget
    : '(' indexList ')' whereClause?
    | ON CONSTRAINT columnIdentifier
    ;

conflictAction
    : DO (NOTHING | updateConflictAction)
    ;

updateConflictAction
    : UPDATE SET updateSetList whereClause?;

/**
 * UPDATE
 */
updateStatement
    : updateStatementNoWith
    | withClause updateStatementNoWith
    ;
    
updateStatementNoWith
    : UPDATE ONLY? tableName STAR? aliasClause? SET updateSetList
        fromClause?
        (whereClause | WHERE CURRENT_P OF cursorName)?
        returningClause?
    ;

updateSetList
    : updateSet (COMMA updateSet)*
    ;

updateSet
    : columnIdentifier EQUAL (expression | DEFAULT)                                                       #columnUpdateSet
    | '(' qualifiedIdentifierList ')' EQUAL ROW? '(' updateSetExpressionList ')'   #columnListUpdateSet
    | '(' qualifiedIdentifierList ')' EQUAL '(' queryPrimary ')'                   #subqueryUpdateSet
    ;
    
updateSetExpressionList
    : updateSetExpression (COMMA updateSetExpression)*
    ;

updateSetExpression
    : expression
    | DEFAULT
    ;

/**
 * DELETE
 */
deleteStatement
    : deleteStatementNoWith
    | withClause deleteStatementNoWith
    ;

deleteStatementNoWith
    : DELETE_P FROM ONLY? tableName STAR? aliasClause? 
        (USING fromItemList)?
        (whereClause | WHERE CURRENT_P OF cursorName)
        returningClause?
    ;

/**
 * TRUNCATE
 */
// NOTE: Asterisk may be included at relation expresion.
truncateStatement
    : TRUNCATE TABLE? ONLY? tableList
    (RESTART IDENTITY_P | CONTINUE_P IDENTITY_P)?
    (CASCADE | RESTRICT)
    ;

//----------------- Uncategorized Statements ---------------------------------------------------------------------------

/**
 * NOTIFY
 *
 * See: https://www.postgresql.org/docs/14/sql-notify.html
 */
notifyStatement
    : NOTIFY columnIdentifier (',' string)?
    ;

/**
 * RESET
 *
 * See: https://www.postgresql.org/docs/14/sql-reset.html
 */
resetStatement
    : RESET resetTarget
    ;

resetTarget
    : genericReset
    | TIME ZONE
    | TRANSACTION ISOLATION LEVEL
    | SESSION AUTHORIZATION
    ;

genericReset
    : qualifiedIdentifier
    | ALL
    ;

/**
 * SET
 *
 * See: https://www.postgresql.org/docs/14/sql-set.html
 */
setStatement
    : SET (SESSION | LOCAL)? setStatementTarget
    ;

setStatementTarget
    : TRANSACTION transactionModeList
    | SESSION CHARACTERISTICS AS TRANSACTION transactionModeList
    | setTarget
    ;

setTarget
    : genericSet
    | qualifiedIdentifierList FROM CURRENT_P
    | TIME ZONE timezone
    | (CATALOG_P | SCHEMA | TRANSACTION SNAPSHOT) string
    | NAMES (string | DEFAULT)?
    | (ROLE | SESSION AUTHORIZATION) noReservedWordOrString
    | SESSION AUTHORIZATION noReservedWordOrString
    | XML_P OPTION documentOrContent
    ;

genericSet
    : qualifiedIdentifier (TO | EQUAL) (booleanOrString | numericOnly)
    ;

transactionModeList
    : transactionMode (',' transactionMode)*
    ;

transactionMode
    : ISOLATION LEVEL isoLevel
    | READ ONLY
    | READ WRITE
    | DEFERRABLE
    | NOT DEFERRABLE
    ;

isoLevel
    : READ (UNCOMMITTED | COMMITTED)
    | REPEATABLE READ
    | SERIALIZABLE
    ;

timezone
    : string
    | identifier
    | interval string intervalOption?
    | interval '(' unsignedInt ')' string
    | numericOnly
    | DEFAULT
    | LOCAL
    ;

//----------------- CLAUSES --------------------------------------------------------------------------------------------

/**
 * ALIAS
 */
aliasClause
    : AS? aliasClauseBody
    ;
    
aliasClauseBody
    : columnIdentifier ('(' columnIdentifierList ')')?
    ;

/**
 * COLLATE
 */
collateClause
    : COLLATE qualifiedIdentifier
    ;

/**
 * DefinitionlistClause
 */
definitionListClause
    : WITH '(' definitionList ')'
    ;

/**
 * FOR .. (OF ..);
 */
forClause
    : FOR lockStrengthOption (OF tableList)? (NOWAIT | SKIP_P LOCKED)?
    ;

lockStrengthOption
    : UPDATE
    | NO KEY UPDATE
    | SHARE
    | KEY SHARE
    ;

/**
 * FROM
 */
fromClause
    : FROM fromItemList
    ;

// List of from item.
fromItemList
    : fromItem (COMMA fromItem)*
    ;

fromItem
    : fromItemPrimary joinClause*
    ;

// Item that can be an element of the from clause.
fromItemPrimary
    : ONLY? tableFromItem
    | LATERAL_P? functionFromItem
    | LATERAL_P? subqueryFromItem
    ;

// Table item.
tableFromItem
    : tableName STAR? aliasClause? tableSampleClause?
    ;

// Function item.
functionFromItem
    : windowlessFunctionExpression (WITH ORDINALITY)? aliasClause?                               # functionFromItemDefault
    | functionExpression AS aliasName '(' columnDefinitionList ')'      # functionFromItemWithAs
    | rowsFromFunctionPrimary (WITH ORDINALITY)? aliasClause?                       # functionFromItemWithRows
    ;

// Function item that has a ROWS FROM block.
rowsFromFunctionPrimary
    : ROWS FROM '(' functionExpression ')';

// Subquery item.
subqueryFromItem
    : queryExpressionParens aliasClause?;

// Join item.
joinClause
    : join (ON expression | USING '(' columnIdentifierList ')')? aliasClause?
    ;

// Join
join
    : CROSS JOIN fromItemPrimary
    | NATURAL? joinType? JOIN fromItemPrimary
    ;
    
joinType
    : (FULL | LEFT | RIGHT | INNER_P) OUTER_P?
    ;

/**
 * GROUP BY
 */
groupByClause
    : GROUP_P BY (ALL | DISTINCT)? groupByItemList
    ;

/**
 * HAVING
 */
havingClause
    : HAVING expression
    ;

/**
 * INCLUDE
 */
includeClause
    : INCLUDE '(' indexList ')'
    ;

/**
 * INHERITS
 */
inheritsClause
    : INHERITS '(' qualifiedIdentifierList ')'
    ;

/**
 * INTO
 */
intoClause
    : INTO intoClauseOptions? TABLE? tableName
    ;

// Table options for into clause.
intoClauseOptions
    : TEMPORARY
    | TEMP
    | UNLOGGED
    ;

/**
 * LIMIT, FETCH, OFFSET
 */
limitClause
    : limit (offset)?
    | offset (limit)?
    ;
 
limit
    : LIMIT expression | ALL
    | FETCH (FIRST_P | NEXT) expression (ROW | ROWS) (ONLY | WITH TIES)
    ;

offset
    : OFFSET (expression (ROW | ROWS)? | ALL)
    ;

/**
 * ON COMMIT
 */
onCommitClause
    : ON COMMIT (DROP | DELETE_P ROWS | PRESERVE ROWS)
    ;

/**
 * ORDER BY
 */
orderByClause
    : ORDER BY orderList
    ;

orderList
    : orderExpression (COMMA orderExpression)*
    ;

orderExpression
    : expression (ASC | DESC)?
    ;

/**
 * PARTITION BY
 */
partitionByClause
    : PARTITION BY qualifiedIdentifier '(' partitionParam (COMMA partitionParam)* ')'
    ;

partitionParam
    : columnIdentifier collateClause? qualifiedIdentifier?
    | windowlessFunctionExpression collateClause? qualifiedIdentifier?
    | '(' expression ')' collateClause? qualifiedIdentifier?
    ;

/**
 * PARTITION OF
 */
partitionOfClause
    : PARTITION OF qualifiedIdentifier typedTableElementList? partitionBoundOptions
    ;

typedTableElementList
    : typedTableElement (COMMA typedTableElement)*
    ;

typedTableElement
    : columnOptions
    | tableConstraint
    ;

columnOptions
    : columnIdentifier (WITH OPTIONS)? columnConstraint*
    ;

partitionBoundOptions
    : FOR VALUES WITH '(' hashBound (COMMA hashBound)* ')'
    | FOR VALUES IN_P '(' expressionList ')'
    | FOR VALUES FROM '(' expressionList ')' TO '(' expressionList ')'
    | DEFAULT
    ;

hashBound
    : noReservedKeywords unsignedInt
    ;

/**
 * RETURNING
 */
// NOTE: The syntax of the RETURNING list is identical to that of the output list of SELECT.
// see: https://www.postgresql.org/docs/14/sql-update.html
// also: https://www.postgresql.org/docs/14/sql-insert.html
returningClause
    : RETURNING returningItemList
    ;

returningItemList
    : selectItemList
    ;

/**
 * TABLESAMPLE
 */
tableSampleClause
    : TABLESAMPLE functionName '(' expressionList ')' (REPEATABLE '(' seed ')')?
    ;

seed
    : expression
    ;

/**
 * TABLESPACE
 */
tableSpaceClause
    : TABLESPACE columnIdentifier
    ;

/**
 * USING
 */
usingClause
    : USING columnIdentifier
    ;

/**
 * USING INDEX
 */
usingIndexClause
    : USING INDEX columnIdentifier
    ;

/**
 * USING INDEX TABLESPACE
 */
usingIndexTablespaceClause
    : USING INDEX TABLESPACE columnIdentifier
    ;

/**
 * WHERE
 */
whereClause
    : WHERE expression;

/**
 * WINDOW
 */
windowClause
    : WINDOW windowDefinitionList;
    
windowDefinitionList
    : windowDefinition (COMMA windowDefinition)*;

// NOTE: Same structure as the OVER clause.
windowDefinition
    : columnIdentifier AS windowSpecification;

windowSpecification
    : '(' windowName? (PARTITION BY expressionList)? orderByClause? frameClause? ')';

frameClause
    : (RANGE | ROWS | GROUPS) (
        frameBound
        | BETWEEN frameBound AND frameBound
    ) (frameExclusion)?;
    
frameBound
    : UNBOUNDED frameBoundOption
    | offsetValue frameBoundOption
    | CURRENT_P ROW;

offsetValue
    : expression
    ;
    
frameBoundOption
    : PRECEDING
    | FOLLOWING;

frameExclusion
    : EXCLUDE (CURRENT_P ROW | GROUP_P | TIES | NO OTHERS);

/**
 * WITH (Query)
 */
withClause
    : WITH RECURSIVE? commonTableExpression (COMMA commonTableExpression)*;

/**
 * WITH (Options...)
 */
withOptionsClause
    : WITH '(' relOptionList ')'
    ;

// CTE(Common Table Expression).
commonTableExpression
    : subqueryName ('(' columnIdentifierList ')')? 
    AS commonTableExpressionOption? 
    '(' commonTableExpressionStatements ')';

commonTableExpressionOption
    : MATERIALIZED
    | NOT MATERIALIZED;

commonTableExpressionStatements
    : selectStatement
    | updateStatement
    | insertStatement
    | deleteStatement
    ;

//----------------- EXPRESSIONS ----------------------------------------------------------------------------------------

/**
 * Expression List
 */
expressionList
    : expression (COMMA expression)*
    ;

/**
 * Expression
 *
 * See: https://www.postgresql.org/docs/14/sql-syntax-lexical.html#SQL-PRECEDENCE
 * for operator precedences.
 */

//exp:
//expNoParen
//expNoParen subscript+ field*
//expParen (subscript | field)*

//expression
//    : expressionParens
//    | expressionNoParens
//    ;

//expression
//    : expressionNoParens //arraySubscript+ ((DOT columnLabelIdentifier)+)* arraySubscript*
//    | expressionParens //(arraySubscript | DOT columnLabelIdentifier)*
//    ;

expressionParens
    : '(' expression ')'
    ;

expression
    : andExpression
    | expression OR expression
    ;

andExpression
    : booleanExpression
    | NOT andExpression
    | andExpression AND andExpression
    ;

/**
 * Boolean Expression - with IS keyword
 */
booleanExpression
    : comparisonExpression
    | booleanExpression (ISNULL | NOTNULL)
    | booleanExpression IS NOT? NULL_P
    | booleanExpression IS NOT? TRUE_P
    | booleanExpression IS NOT? FALSE_P
    | booleanExpression IS NOT? UNKNOWN
    | booleanExpression IS NOT? DISTINCT FROM booleanExpression
    ;

/**
 * Comparison Expression - with comparison operators, subqueries
 */
// TODO: likeExpressionOptions and subqueryOperator have same operators.
//       Lookahead increases because of that (k = 5). Should we fix this?
comparisonExpression
    : qualifiedOperatorExpression (likeExpressionOptions qualifiedOperatorExpression (ESCAPE expression)?)? # comparisonExpressionLike
    | comparisonExpression comparisonOperator comparisonExpression                                          # comparisonExpressionBase
    | comparisonExpression subqueryOperator subqueryType (queryExpressionParens | '(' expression ')')       # comparisonExpressionSubquery
    ;

likeExpressionOptions
    : NOT? (LIKE | ILIKE | IN_P)
    | SIMILAR TO
    | BETWEEN SYMMETRIC?
    ;

subqueryOperator
    : simpleOperator
    | OPERATOR '(' operator ')'
    | NOT? (LIKE | ILIKE)
    ;


subqueryType
    : ANY
    | SOME
    | ALL
    ;

/**
 * Qualified Operator Expression - with user defined operators
 */
qualifiedOperatorExpression
    : unaryQualifiedOperatorExpression (qualifiedWithoutMathOperator unaryQualifiedOperatorExpression)*
    ;

unaryQualifiedOperatorExpression
    : qualifiedWithoutMathOperator? arithmeticExpression
    ;

/**
 * Arithmetic Expression - with basic mathematical operators
 */
arithmeticExpression
    : collateExpression
    | (PLUS | MINUS) arithmeticExpression
    | arithmeticExpression CARET arithmeticExpression
    | arithmeticExpression (STAR | SLASH | PERCENT) arithmeticExpression
    | arithmeticExpression (PLUS | MINUS) arithmeticExpression
    ;

/**
 * Collate Expression - with COLLATE keyword.
 */
collateExpression
    : typecastExpression (COLLATE qualifiedIdentifier)?
    ;

/**
 * Typecast Expression - with :: keyword.
 */
typecastExpression
    : valueExpression (TYPECAST type)*
    ;

/**
 * Value Expression
 * See: https://www.postgresql.org/docs/14/sql-expressions.html
 */
// TODO: Check lookahead
valueExpression
    : (EXISTS | UNIQUE | ARRAY)? queryExpressionParens  # valueExpressionSubquery
    | ARRAY OPEN_BRACKET expressionList CLOSE_BRACKET   # valueExpressionArray
    | GROUPING '(' expressionList ')'                   # valueExpressionGrouping
//    | qualifiedIdentifier                               # valueExpressionColumn // TODO: reduce max k
    | constant                                          # valueExpressionConstant
    | caseExpression                                    # valueExpressionCase
    | functionExpression                                # valueExpressionFunction // TODO: reduce max k
    | row                                               # valueExpressionRow
    | row OVERLAPS row                                  # valueExpressionRowOverlaps // TODO: reduce max k            
//    | PARAM                                             # valueExpressionParam
    | starIdentifier                                    # valueExpressionStar
    | subscriptExpression                               # valueExpressionSubscript
    ;

subscriptExpression
//    : expressionParens DOT columnLabelIdentifier
//    | expressionParens subscriptIndirection
    : expressionParens indirection?
    | parameterMarker indirection?
    | qualifiedIdentifier
    | subscriptExpression subscriptIndirection
    ;

parameterMarker
    : PARAM
    ;

subscriptIndirection
    : subscript indirection?
    ;

//exp:
//  expNoParen
//  expParen
//expNoParen:
//  expValue
//expValue:
//  expParen field
//  exp subscript
//  exp subscript field?

/**
 * Case Expression
 */
caseExpression
    : CASE expression? whenClauseList defaultCase? END_P
    ;

whenClauseList
    : whenClause+
    ;

whenClause
    : WHEN expression THEN expression
    ;

defaultCase
    : ELSE expression
    ;

/**
 * ROW Expression
 */
row
    : explicitRow
    | implicitRow
    ;

explicitRow
    : ROW '(' expressionList? ')'
    ;

/*
Copied from the open-source PG parser:

TO DO:
for some reason v1
implicit_row: '(' expr_list COMMA a_expr ')';
works better than v2
implicit_row: '(' expr_list  ')';
while looks like they are almost the same, except v2 requieres at least 2 items in list
while v1 allows single item in list
*/

// NOTE: This node may cause a large amount of lookahead,
//       because the parser must check whether there is a comma or not.
//       Would there be a better solution?
implicitRow
    : '(' expression COMMA expressionList ')'
    ;

/**
 * Function Expression
 *
 * See 4.2.6, 4.2.7, 4.2.8 of: 
 * https://www.postgresql.org/docs/14/sql-expressions.html#SQL-EXPRESSIONS-FUNCTION-CALLS
 */
 // TODO: Resolve ambiguity
functionExpression
    : functionCall withinGroupClause? filterClause? overClause?
    | commonFunctionExpression
    ;

windowlessFunctionExpression
    : commonFunctionExpression
    | functionCall
    ;

functionCall
    : functionName '(' functionCallArgument? ')'
    ;

// TODO: Resolve ambiguity
// TODO: Filter names of common function expression
functionName
    : typeFunctionIdentifier
    | qualifiedIdentifier
    ;

functionCallArgument
    : argumentList (COMMA VARIADIC argument)? orderByClause?
    | VARIADIC argument orderByClause?
    | (ALL | DISTINCT) argumentList orderByClause?
    | STAR
    ;

commonFunctionExpression
    : COLLATION FOR '(' expression ')'
    | CURRENT_DATE
    | CURRENT_TIME ('(' signedInt ')')?
    | CURRENT_TIMESTAMP ('(' signedInt ')')?
    | LOCALTIME ('(' signedInt ')')?
    | LOCALTIMESTAMP ('(' signedInt ')')?
    | CURRENT_ROLE
    | CURRENT_USER
    | SESSION_USER
    | USER
    | CURRENT_CATALOG
    | CURRENT_SCHEMA
    | CAST '(' expression AS type ')'
    | EXTRACT '(' extractList? ')'
    | NORMALIZE '(' expression (COMMA unicodeNormalForm)? ')'
    | OVERLAY '(' overlayList ')'
    | POSITION '(' positionList ')'
    | SUBSTRING '(' substringList ')'
    | TREAT '(' expression AS type ')'
    | TRIM '(' (BOTH | LEADING | TRAILING)? trimList ')'
    | NULLIF '(' expression COMMA expression ')'
    | COALESCE '(' expressionList ')'
    | GREATEST '(' expressionList ')'
    | LEAST '(' expressionList ')'
    | XMLCONCAT '(' expressionList ')'
    | XMLELEMENT '(' NAME_P columnLabelIdentifier (COMMA (xmlAttributes | expressionList))? ')'
    | XMLEXISTS '(' expression xmlExistsArgument ')'
    | XMLFOREST '(' xmlAttributeList ')'
    | XMLPARSE '(' documentOrContent expression xmlWhitespaceOption ')'
    | XMLPI '(' NAME_P columnLabelIdentifier (COMMA expression)? ')'
    | XMLROOT '(' XML_P expression COMMA xmlRootVersion xmlRootStandalone? ')'
    | XMLSERIALIZE '(' documentOrContent expression AS simpleType ')'
    ;

// EXTRACT
extractList
    : extractArgument FROM expression
    ;

extractArgument
    : identifier
    | YEAR_P
    | MONTH_P
    | DAY_P
    | HOUR_P
    | MINUTE_P
    | SECOND_P
    | string
    ;

// NORMALIZE
unicodeNormalForm
    : NFC
    | NFD
    | NFKC
    | NFKD
    ;

// OVERLAY
overlayList
    : expression PLACING expression FROM expression (FOR expression)?
    ;

// POSITION
positionList
    : expression IN_P expression // TODO: Should we consider binary calculation only expression such as b_expr?
    ;

// SUBSTRING
substringList
    : expression FROM expression FOR expression
    | expression FOR expression FROM expression
    | expression FROM expression
    | expression FOR expression
    | expression SIMILAR expression ESCAPE expression
    | expressionList
    ;

// TRIM
trimList
    : expression FROM expressionList
    | FROM expressionList
    | expressionList
    ;

// XML Functions
xmlAttributes
    : XMLATTRIBUTES '(' xmlAttributeList ')'
    ;

xmlAttributeList
    : xmlAttribute (COMMA xmlAttribute)*
    ;

xmlAttribute
    : expression (AS columnLabelIdentifier)?
    ;

xmlExistsArgument
    : PASSING xmlPassingMech? valueExpression xmlPassingMech?
    ;

xmlPassingMech
    : BY (REF | VALUE_P)
    ;

xmlWhitespaceOption
    : (PRESERVE | STRIP_P) WHITESPACE_P
    ;

xmlRootVersion
    : VERSION_P (expression | NO VALUE_P)
    ;

xmlRootStandalone
    : COMMA STANDALONE_P (YES_P | NO VALUE_P?)
    ;

documentOrContent
    : DOCUMENT_P
    | CONTENT_P
    ;

/**
 * WITHIN GROUP Clause
 */
withinGroupClause
    : WITHIN GROUP_P '(' orderByClause ')'
    ;

/**
 * FILTER clause
 */
filterClause
    : FILTER '(' WHERE expression ')'
    ;

/**
 * OVER clause
 */
overClause
    : OVER (windowSpecification | columnIdentifier)
    ;

//----------------- OPERATORS ------------------------------------------------------------------------------------------

mathOperator
    : PLUS
    | MINUS
    | STAR
    | SLASH
    | PERCENT
    | CARET
    | comparisonOperator
    ;

comparisonOperator
    : EQUAL
    | NOT_EQUALS
    | GREATER_EQUALS
    | GT
    | LESS_EQUALS
    | LT
    ;

setOperator
    : UNION
    | INTERSECT
    | EXCEPT;

setOperatorOption
    : ALL
    | DISTINCT;

qualifiedWithoutMathOperator
    : Operator
    | OPERATOR '(' operator ')'
    ;

qualifiedOperator
    : simpleOperator
    | OPERATOR '(' operator ')'
    ;

operator
    : (columnIdentifier DOT)* simpleOperator
    ;

simpleOperator
    : Operator
    | mathOperator
    ;

operatorDefinitionList
    : operatorDefinition (',' operatorDefinition)*
    ;

operatorDefinition
    : operator operatorArgumentTypes
    ;

operatorArgumentTypes
    : '(' type ')'
    | '(' type ',' type ')'
    | '(' NONE ',' type ')'
    | '(' type ',' NONE ')'
    ;

//----------------- IDENTIFIERS ----------------------------------------------------------------------------------------

/**
 * Identifier
 */
identifier returns [QsiIdentifier id]
    : t=Identifier { $id = new QsiIdentifier($t.text, false); }
    | t=QuotedIdentifier { $id = new QsiIdentifier($t.text, true); }
    ;

columnLabelIdentifier returns [QsiIdentifier id]
    : i=identifier { $id = $i.id; }
    | rKey=reservedKeyword { $id = new QsiIdentifier($rKey.text, false); }
    | nrKey=nonReservedKeyword { $id = new QsiIdentifier($nrKey.text, false); }
    | cKey=columnKeyword { $id = new QsiIdentifier($cKey.text, false); }
    | tKey=typeFunctionKeyword { $id = new QsiIdentifier($tKey.text, false); }
    | plKey=plsqlNonreservedKeyword { $id = new QsiIdentifier($plKey.text, false); }
    ;

columnIdentifier returns [QsiIdentifier id]
    : i=identifier { $id = $i.id; }
    | nrKey=nonReservedKeyword { $id = new QsiIdentifier($nrKey.text, false); }
    | cKey=columnKeyword { $id = new QsiIdentifier($cKey.text, false); }
    | plKey=plsqlNonreservedKeyword { $id = new QsiIdentifier($plKey.text, false); }
    ;

typeFunctionIdentifier returns [QsiIdentifier id]
    : i=identifier { $id = $i.id; }
    | nrKey=nonReservedKeyword { $id = new QsiIdentifier($nrKey.text, false); }
    | tKey=typeFunctionKeyword { $id = new QsiIdentifier($tKey.text, false); }
    | plKey=plsqlNonreservedKeyword { $id = new QsiIdentifier($plKey.text, false); }
    ;

// TODO: Check names are correctly using identifiers.
aliasName returns [QsiIdentifier id]
    : i=identifier { $id = $i.id; }
    ;

cursorName
    : columnIdentifier
    ;

subqueryName
    : columnIdentifier
    ;

qualifiedIdentifier
    : columnIdentifier indirection?
    ;

indirection
    : (DOT columnLabelIdentifier)+
    ;

starIdentifier
    : columnIdentifier DOT STAR
    ;

tableName
    : qualifiedIdentifier STAR?
    | ONLY (qualifiedIdentifier | '(' qualifiedIdentifier ')')
    ;

windowName
    : columnIdentifier
    ;

noReservedKeywords
    : identifier
    | nonReservedKeyword
    | columnKeyword
    | typeFunctionKeyword
    ;

/**
 * Indirection
 * 
 * Nodes that are able to come right after the column identifier.
 * Consists of dot attribute and subscript. 
 */
//indirection
//    : DOT (columnLabelIdentifier | STAR)
////    | arraySubscript
//    ;

subscript
    : OPEN_BRACKET (expression | expression? COLON expression?) CLOSE_BRACKET
    ;


/**
 * Identifier List
 */
identifierList returns [List<QsiIdentifier> list]
    @init { $list = new List<QsiIdentifier>(); }
    : i=identifier { $list.Add($i.id); } ( COMMA i=identifier { $list.Add($i.id); } )*
    ;

argumentList
    : argument (COMMA argument)*
    ;

argument
    : expression
    | typeFunctionIdentifier (COLON_EQUALS | EQUALS_GREATER) expression
    ;

qualifiedIdentifierList
    : qualifiedIdentifier (COMMA qualifiedIdentifier)*
    ;

columnIdentifierList
    : columnIdentifier (COMMA columnIdentifier)*
    ;

columnLabelIdentifierList
    : columnLabelIdentifier (',' columnLabelIdentifier)*
    ;

groupByItemList
    : groupByItem (COMMA groupByItem)*
    ;

groupByItem
    : expression
    | (CUBE | ROLLUP) '(' expressionList ')'
    | GROUPING SETS '(' groupByItemList ')'
    | '(' ')'
    ;

tableList
    : qualifiedIdentifierList
    ;

//----------------- Types ----------------------------------------------------------------------------------------------

/**
 * PostgreSQL type.
 *
 * See: https://www.postgresql.org/docs/14/datatype.html
 * and also: https://www.postgresql.org/docs/14/extend-type-system.html
 */
type
    : SETOF? simpleType typeOption?
    | qualifiedIdentifier PERCENT (ROWTYPE | TYPE_P)
    ;

typeOption
    : (OPEN_BRACKET signedInt? CLOSE_BRACKET)+
    | ARRAY (OPEN_BRACKET signedInt CLOSE_BRACKET)*
    ;

simpleType
    : genericType
    | numericType
    | bitType
    | characterType
    | dateTimeType
    | intervalType
    ;
/**
 * Generic Type
 *
 * This type includes user-defined types and other built-in types.
 * For more info about user-defined types, see: https://www.postgresql.org/docs/14/xtypes.html
 * For more info about built-in types, see: https://www.postgresql.org/docs/14/datatype.html
 */
genericType
    : typeFunctionIdentifier ('(' expressionList ')')?
    ;

/**
 * Bit Type
 *
 * See: https://www.postgresql.org/docs/14/datatype-bit.html
 * https://www.postgresql.org/docs/14/datatype-binary.html
 */
bitType
    : BIT VARYING? ('(' unsignedInt ')')?
    ;

/**
 * Character Type
 *
 * See: https://www.postgresql.org/docs/14/datatype-character.html
 */
characterType
    : characterPrefix ('(' unsignedInt ')')?
    ;

characterPrefix
    : (CHARACTER | CHAR_P | NCHAR) VARYING?
    | VARCHAR
    | NATIONAL (CHARACTER | CHAR_P) VARYING?
    ;

/**
 * Numeric type
 *
 * See: https://www.postgresql.org/docs/14/datatype-numeric.html
 * Note that serial types are not true types, therefore not implemented.
 */
numericType
    : SMALLINT
    | INTEGER
    | INT_P
    | BIGINT
    | DECIMAL_P ('(' expressionList ')')?
    | DEC ('(' expressionList ')')?
    | NUMERIC ('(' expressionList ')')?
    | REAL
    | DOUBLE_P PRECISION
    | FLOAT_P ('(' unsignedInt ')')?
    | BOOLEAN_P
    ;

/**
 * Date/Time Type
 *
 * See: https://www.postgresql.org/docs/14/datatype-datetime.html
 */
dateTimeType
    : (TIMESTAMP | TIME) ('(' signedInt ')')? timezoneOption
    ;

/**
 * Interval Type (subset of Date/Time Type)
 *
 * See: https://www.postgresql.org/docs/14/datatype-datetime.html
 *
 * Copied from the open-source PG parser:
 * TO DO with_la was used
 */ 
intervalType
    : INTERVAL (intervalOption | '(' signedInt ')')
    ;

timezoneOption
    : (WITH | WITHOUT) TIME ZONE
    ;

//----------------- CONSTANTS ------------------------------------------------------------------------------------------

/**
 * PostgreSQL Constant.
 *
 * See: https://www.postgresql.org/docs/14/sql-syntax-lexical.html#SQL-SYNTAX-CONSTANTS
 */
constant
    : signedInt
    | float
    | hex
    | bin
    | string
    | functionName (string | '(' argumentList orderByClause ')' string)
    | constType string
    | interval
    | TRUE_P
    | FALSE_P
    | NULL_P
    ;

/**
 * Interval Constant
 *
 * See: https://www.postgresql.org/docs/14/datatype-datetime.html
 */
interval
    : INTERVAL (string intervalOption? | '(' signedInt ')' string)
    ;

intervalOption
    : YEAR_P
    | MONTH_P
    | DAY_P
    | HOUR_P
    | MINUTE_P
    | intervalSecond
    | YEAR_P TO MONTH_P
    | DAY_P TO (HOUR_P | MINUTE_P | intervalSecond)
    | HOUR_P TO (MINUTE_P | intervalSecond)
    | MINUTE_P TO intervalSecond
    ;

intervalSecond
    : SECOND_P ('(' signedInt ')')?
    ;

constType
    : numericType
    | bitType
    | characterType
    | dateTimeType
    ;

// unsigned
unsignedInt
    : Integral
    ;

// signed
signedInt
    : PLUS? unsignedInt
    | MINUS unsignedInt
    ;

// float
float
    : Numeric
    ;

// hexadecimal
hex
    : HexadecimalStringConstant
    ;

// binary
bin
    : BinaryStringConstant
    ;

// string
string
    : StringConstant
    | BeginDollarStringConstant DollarText* EndDollarStringConstant
    | EscapeStringConstant
    ;

/**
 * Boolean or string
 * In PostgreSQL, you can represent boolean value without TRUE and FALSE.
 * e.g. ON, OFF, YES, 0 and so on.
 * 
 * FMI, see: https://www.postgresql.org/docs/14/datatype-boolean.html
 */
booleanOrString
    : TRUE_P
    | FALSE_P
    | ON
    | noReservedWordOrString
    ;

noReservedWordOrString
    : noReservedKeywords
    | string
    ;

//----------------- TEMPORARY NODES ------------------------------------------------------------------------------------
// Nodes that are not grouped, but required to implement other nodes.

/**
 * Aggregate Definition
 */
aggregateDefinition
    : functionName '(' aggregateArgumentDefinitions ')'
    ;

aggregateArgumentDefinitions
    : STAR
    | argumentDefinitionList
    | ORDER BY argumentDefinitionList
    | argumentDefinitionList ORDER BY argumentDefinitionList
    ;

aggregateArgumentListOldSyntax
    : aggregateArgumentsOldSyntax (COMMA aggregateArgumentsOldSyntax)*
    ;

aggregateArgumentsOldSyntax
    : identifier EQUAL definitionArgument
    ;

/**
 * Column Definition
 */
columnDefinitionList
    : columnDefinition (COMMA columnDefinition)*
    ;
    
columnDefinition
    : columnIdentifier type genericOptions? columnConstraint* 
    ;

/**
 * Constraints
 *
 * https://www.postgresql.org/docs/current/ddl-constraints.html
 */

// Constraint for a table.
tableConstraint
    : CONSTRAINT columnIdentifier tableConstraintElement constraintAttributeList?
    | tableConstraintElement constraintAttributeList?
    ;

tableConstraintElement
    : CHECK '(' expression ')'
    | (UNIQUE | PRIMARY KEY) '(' columnIdentifierList ')' 
        includeClause?
        definitionListClause?
        usingIndexTablespaceClause?
    | (UNIQUE | PRIMARY KEY) usingIndexClause
    | EXCLUDE usingClause? '(' excludeIndexList ')'
        includeClause?
        definitionListClause?
        usingIndexTablespaceClause?
        exclusionWhereClause?
    | FOREIGN KEY '(' columnIdentifierList ')' REFERENCES qualifiedIdentifier
        ('(' columnIdentifierList ')')?
        keyMatch?
        keyActions?
    ;

excludeIndexList
    : excludeIndex (COMMA excludeIndex)*
    ;

excludeIndex
    : index WITH (operator | OPERATOR '(' operator ')')
    ;

exclusionWhereClause
    : WHERE '(' expression ')'
    ;
 
columnConstraint
    : CONSTRAINT columnIdentifier columnConstraintElement
    | columnConstraintElement
    | constraintAttribute
    | COLLATE qualifiedIdentifier
    ;

columnConstraintElement
    : NOT? NULL_P
    | (UNIQUE | PRIMARY KEY) definitionListClause?
    | CHECK '(' expression ')' (NO INHERIT)?
    | DEFAULT expression
    | GENERATED generatedOption AS (IDENTITY_P ('(' sequenceOptionList ')')? | '(' expression ')' STORED)
    | REFERENCES qualifiedIdentifier ('(' columnIdentifierList ')')? keyMatch? keyActions?
    ;

constraintAttributeList
    : constraintAttribute+
    ;

constraintAttribute
    : NOT? DEFERRABLE
    | INITIALLY (IMMEDIATE | DEFERRED)
    | NOT VALID
    | NO INHERIT
    ;

generatedOption
    : ALWAYS
    | BY DEFAULT
    ;

sequenceOptionList
    : sequenceOptionElement+
    ;

sequenceOptionElement
    : AS simpleType
    | (CACHE | MAXVALUE | MINVALUE) numericOnly
    | CYCLE
    | INCREMENT BY? numericOnly
    | NO (MAXVALUE | MINVALUE | CYCLE)
    | OWNED BY qualifiedIdentifier
    | SEQUENCE NAME_P qualifiedIdentifier
    | START WITH? numericOnly
    | RESTART WITH? numericOnly?
    ;

keyMatch
    : MATCH (FULL | PARTIAL | SIMPLE)
    ;

keyActions
    : keyAction+
    ;

keyAction
    : ON (UPDATE | DELETE_P) keyActionOptions
    ;

keyActionOptions
    : NO ACTION
    | RESTRICT
    | CASCADE
    | SET (NULL_P | DEFAULT)
    ;

/**
 * Definitions - Clauses such as foo = bar.
 *
 * TODO: I think this node is duplicated - we have Expression node!
 *       Guess open-source one made this because of the execution speed; its expression node is DAMN slow.
 *       Maybe I could replace definition as expression and check the time difference.
 */

definitionList
    : definition (COMMA definition)*
    ;

definition
    : columnLabelIdentifier (EQUAL definitionArgument)?
    ;

definitionArgument
    : functionType
    | reservedKeyword
    | qualifiedOperator
    | numericOnly
    | string
    | NONE
    ;

numericOnly
    : (PLUS | MINUS) float      #withSignFloat
    | float                     #noSignFloat
    | signedInt                       #withSignInt
    ;

/**
 * Function Definition
 */
functionDefinitionList
    : functionDefinition (',' functionDefinition)*
    ;
 
functionDefinition
    : functionName '(' argumentDefinitionList? ')'
    | typeFunctionKeyword
    | qualifiedIdentifier
    ;

argumentDefinitionWithDefaultList
    : argumentDefinitionWithDefault (COMMA argumentDefinitionWithDefault)*
    ;

argumentDefinitionWithDefault
    : argumentDefinition ((DEFAULT | EQUAL) expression)?
    ;

argumentDefinitionList
    : argumentDefinition (COMMA argumentDefinition)*
    ;

argumentDefinition
    : argumentClass typeFunctionIdentifier? functionType
    | typeFunctionIdentifier argumentClass? functionType
    | functionType
    ;

argumentClass
    : IN_P OUT_P?
    | OUT_P
    | INOUT
    | VARIADIC
    ;

functionType
    : type
    | SETOF? typeFunctionIdentifier (DOT columnLabelIdentifier)* PERCENT TYPE_P
    ;

/**
 * General options for the statement
 */
genericOptions
    : OPTIONS '(' genericOption (COMMA genericOption)* ')'
    ;

genericOption
    : columnLabelIdentifier string
    ;

alterGenericOptions
    : OPTIONS '(' alterGenericOption (',' alterGenericOption)* ')'
    ;

alterGenericOption
    : (SET | ADD_P | DROP)? genericOption
    ;

/**
 * Grantee
 */
granteeList
    : grantee (',' grantee)*
    ;

grantee
    : GROUP_P? role
    ;

/**
 * Index Parameters
 */
indexList
    : index (COMMA index)*
    ;

index
    : columnIdentifier indexOptions
    | windowlessFunctionExpression indexOptions
    | '(' expression ')' indexOptions
    ;

indexOptions
    : collateClause? qualifiedIdentifier? ('(' relOptionList ')')? (ASC | DESC)? (NULLS_P (FIRST_P | LAST_P))?
    ;

/**
 * Names
 */
// TODO: Find out what are those for.
object_type_any_name
   : TABLE
   | SEQUENCE
   | VIEW
   | MATERIALIZED VIEW
   | INDEX
   | FOREIGN TABLE
   | COLLATION
   | CONVERSION_P
   | STATISTICS
   | TEXT_P SEARCH PARSER
   | TEXT_P SEARCH DICTIONARY
   | TEXT_P SEARCH TEMPLATE
   | TEXT_P SEARCH CONFIGURATION
   ;

object_type_name
   : drop_type_name
   | DATABASE
   | ROLE
   | SUBSCRIPTION
   | TABLESPACE
   ;

drop_type_name
   : ACCESS METHOD
   | EVENT TRIGGER
   | EXTENSION
   | FOREIGN DATA_P WRAPPER
   | PROCEDURAL? LANGUAGE
   | PUBLICATION
   | SCHEMA
   | SERVER
   ;

/**
 * Privileges
 */
privileges
    : privilegeList
    | ALL (PRIVILEGES)? ('(' columnIdentifierList ')')?
    ;

privilegeList
    : privilege (',' privilege)*
    ;

privilege
    : (SELECT | REFERENCES | CREATE | columnIdentifier) ('(' columnIdentifierList ')')?
    ;

/**
 * relOption
 */
// TODO: Find out what is relOptionList and implement it.
//       Still dunno what the hell it is ('rel' stand for wut) but needs to be implemented, so I did.
//       Probably for option definition.
relOptionList
    : relOption (COMMA relOption)*
    ;

relOption
    : columnLabelIdentifier (EQUAL definitionArgument | DOT columnLabelIdentifier (EQUAL definitionArgument)?)?
    ;

/**
 * Roles
 */
roleList
    : role (',' role)*
    ;

role
    : noReservedKeywords
    | CURRENT_USER
    | SESSION_USER
    ;

//----------------- KETWORDS -------------------------------------------------------------------------------------------
// In PostgreSQL, reserved keywords are keywords that are being used by sql itself; cannot be used for identifier.
// Non-reserved keywords are the opposite; can be used for identifier(column name, function name, type name).
// But in some case, reserved words are able to be used for function name or type name, vise versa.
//
// Therefore we split keywords by 4 groups:
// 1. Reserved keywords
// 2. Reserved keywords (can be function or type)
// 3. Non-reserved Keywords
// 4. Non-reserved Keywords (cannot be function or type)
// 
// see: https://www.postgresql.org/docs/current/sql-keywords-appendix.html

reservedKeyword
    : ALL
    | ANALYSE
    | ANALYZE
    | AND
    | ANY
    | ARRAY
    | AS
    | ASC
    | ASYMMETRIC
    | BOTH
    | CASE
    | CAST
    | CHECK
    | COLLATE
    | COLUMN
    | CONSTRAINT
    | CREATE
    | CURRENT_CATALOG
    | CURRENT_DATE
    | CURRENT_ROLE
    | CURRENT_TIME
    | CURRENT_TIMESTAMP
    | CURRENT_USER
//  | DEFAULT
    | DEFERRABLE
    | DESC
    | DISTINCT
    | DO
    | ELSE
    | END_P
    | EXCEPT
    | FALSE_P
    | FETCH
    | FOR
    | FOREIGN
    | FROM
    | GRANT
    | GROUP_P
    | HAVING
    | IN_P
    | INITIALLY
    | INTERSECT
/**
 * Copied from the open-source PG parser:
 *
 * from pl_gram.y, line ~2982
 * Fortunately, INTO is a fully reserved word in the main grammar, so
 * at least we need not worry about it appearing as an identifier.
 */
//  | INTO
    | LATERAL_P
    | LEADING
    | LIMIT
    | LOCALTIME
    | LOCALTIMESTAMP
    | NOT
    | NULL_P
    | OFFSET
    | ON
    | ONLY
    | OR
    | ORDER
    | PLACING
    | PRIMARY
    | REFERENCES
    | RETURNING
    | SELECT
    | SESSION_USER
    | SOME
    | SYMMETRIC
    | TABLE
    | THEN
    | TO
    | TRAILING
    | TRUE_P
    | UNION
    | UNIQUE
    | USER
    | USING
    | VARIADIC
    | WHEN
    | WHERE
    | WINDOW
    | WITH
    ;

typeFunctionKeyword
    : AUTHORIZATION
    | BINARY
    | COLLATION
    | CONCURRENTLY
    | CROSS
    | CURRENT_SCHEMA
    | FREEZE
    | FULL
    | ILIKE
    | INNER_P
    | IS
    | ISNULL
    | JOIN
    | LEFT
    | LIKE
    | NATURAL
    | NOTNULL
    | OUTER_P
    | OVERLAPS
    | RIGHT
    | SIMILAR
    | TABLESAMPLE
    | VERBOSE
    ;

nonReservedKeyword
    : ABORT_P
    | ABSOLUTE_P
    | ACCESS
    | ACTION
    | ADD_P
    | ADMIN
    | AFTER
    | AGGREGATE
    | ALSO
    | ALTER
    | ALWAYS
    | ASSERTION
    | ASSIGNMENT
    | AT
    | ATTACH
    | ATTRIBUTE
    | BACKWARD
    | BEFORE
    | BEGIN_P
    | BY
    | CACHE
    | CALL
    | CALLED
    | CASCADE
    | CASCADED
    | CATALOG_P
    | CHAIN
    | CHARACTERISTICS
    | CHECKPOINT
    | CLASS
    | CLOSE
    | CLUSTER
    | COLUMNS
    | COMMENT
    | COMMENTS
    | COMMIT
    | COMMITTED
    | CONFIGURATION
    | CONFLICT
    | CONNECTION
    | CONSTRAINTS
    | CONTENT_P
    | CONTINUE_P
    | CONVERSION_P
    | COPY
    | COST
    | CSV
    | CUBE
    | CURRENT_P
    | CURSOR
    | CYCLE
    | DATA_P
    | DATABASE
    | DAY_P
    | DEALLOCATE
    | DECLARE
    | DEFAULTS
    | DEFERRED
    | DEFINER
    | DELETE_P
    | DELIMITER
    | DELIMITERS
    | DEPENDS
    | DETACH
    | DICTIONARY
    | DISABLE_P
    | DISCARD
    | DOCUMENT_P
    | DOMAIN_P
    | DOUBLE_P
    | DROP
    | EACH
    | ENABLE_P
    | ENCODING
    | ENCRYPTED
    | ENUM_P
    | ESCAPE
    | EVENT
    | EXCLUDE
    | EXCLUDING
    | EXCLUSIVE
    | EXECUTE
    | EXPLAIN
    | EXPRESSION
    | EXTENSION
    | EXTERNAL
    | FAMILY
    | FILTER
    | FIRST_P
    | FOLLOWING
    | FORCE
    | FORWARD
    | FUNCTION
    | FUNCTIONS
    | GENERATED
    | GLOBAL
    | GRANTED
    | GROUPS
    | HANDLER
    | HEADER_P
    | HOLD
    | HOUR_P
    | IDENTITY_P
    | IF_P
    | IMMEDIATE
    | IMMUTABLE
    | IMPLICIT_P
    | IMPORT_P
    | INCLUDE
    | INCLUDING
    | INCREMENT
    | INDEX
    | INDEXES
    | INHERIT
    | INHERITS
    | INLINE_P
    | INPUT_P
    | INSENSITIVE
    | INSERT
    | INSTEAD
    | INVOKER
    | ISOLATION
    | KEY
    | LABEL
    | LANGUAGE
    | LARGE_P
    | LAST_P
    | LEAKPROOF
    | LEVEL
    | LISTEN
    | LOAD
    | LOCAL
    | LOCATION
    | LOCK_P
    | LOCKED
    | LOGGED
    | MAPPING
    | MATCH
    | MATERIALIZED
    | MAXVALUE
    | METHOD
    | MINUTE_P
    | MINVALUE
    | MODE
    | MONTH_P
    | MOVE
    | NAME_P
    | NAMES
    | NEW
    | NEXT
    | NFC
    | NFD
    | NFKC
    | NFKD
    | NO
    | NORMALIZED
    | NOTHING
    | NOTIFY
    | NOWAIT
    | NULLS_P
    | OBJECT_P
    | OF
    | OFF
    | OIDS
    | OLD
    | OPERATOR
    | OPTION
    | OPTIONS
    | ORDINALITY
    | OTHERS
    | OVER
    | OVERRIDING
    | OWNED
    | OWNER
    | PARALLEL
    | PARSER
    | PARTIAL
    | PARTITION
    | PASSING
    | PASSWORD
    | PLANS
    | POLICY
    | PRECEDING
    | PREPARE
    | PREPARED
    | PRESERVE
    | PRIOR
    | PRIVILEGES
    | PROCEDURAL
    | PROCEDURE
    | PROCEDURES
    | PROGRAM
    | PUBLICATION
    | QUOTE
    | RANGE
    | READ
    | REASSIGN
    | RECHECK
    | RECURSIVE
    | REF
    | REFERENCING
    | REFRESH
    | REINDEX
    | RELATIVE_P
    | RELEASE
    | RENAME
    | REPEATABLE
    | REPLACE
    | REPLICA
    | RESET
    | RESTART
    | RESTRICT
    | RETURNS
    | REVOKE
    | ROLE
    | ROLLBACK
    | ROLLUP
    | ROUTINE
    | ROUTINES
    | ROWS
    | RULE
    | SAVEPOINT
    | SCHEMA
    | SCHEMAS
    | SCROLL
    | SEARCH
    | SECOND_P
    | SECURITY
    | SEQUENCE
    | SEQUENCES
    | SERIALIZABLE
    | SERVER
    | SESSION
    | SET
    | SETS
    | SHARE
    | SHOW
    | SIMPLE
    | SKIP_P
    | SNAPSHOT
    | SQL_P
    | STABLE
    | STANDALONE_P
    | START
    | STATEMENT
    | STATISTICS
    | STDIN
    | STDOUT
    | STORAGE
    | STORED
    | STRICT_P
    | STRIP_P
    | SUBSCRIPTION
    | SUPPORT
    | SYSID
    | SYSTEM_P
    | TABLES
    | TABLESPACE
    | TEMP
    | TEMPLATE
    | TEMPORARY
    | TEXT_P
    | TIES
    | TRANSACTION
    | TRANSFORM
    | TRIGGER
    | TRUNCATE
    | TRUSTED
    | TYPE_P
    | TYPES_P
    | UESCAPE
    | UNBOUNDED
    | UNCOMMITTED
    | UNENCRYPTED
    | UNKNOWN
    | UNLISTEN
    | UNLOGGED
    | UNTIL
    | UPDATE
    | VACUUM
    | VALID
    | VALIDATE
    | VALIDATOR
    | VALUE_P
    | VARYING
    | VERSION_P
    | VIEW
    | VIEWS
    | VOLATILE
    | WHITESPACE_P
    | WITHIN
    | WITHOUT
    | WORK
    | WRAPPER
    | WRITE
    | XML_P
    | YEAR_P
    | YES_P
    | ZONE
    ;

columnKeyword
    : BETWEEN
    | BIGINT
    | BIT
    | BOOLEAN_P
    | CHARACTER
    | CHAR_P
    | COALESCE
    | DEC
    | DECIMAL_P
    | EXISTS
    | EXTRACT
    | FLOAT_P
    | GREATEST
    | GROUPING
    | INOUT
    | INTEGER
    | INTERVAL
    | INT_P
    | LEAST
    | NATIONAL
    | NCHAR
    | NONE
    | NORMALIZE
    | NULLIF
    | NUMERIC
    | OUT_P
    | OVERLAY
    | POSITION
    | PRECISION
    | REAL
    | ROW
    | SETOF
    | SMALLINT
    | SUBSTRING
    | TIME
    | TIMESTAMP
    | TREAT
    | TRIM
    | VALUES
    | VARCHAR
    | XMLATTRIBUTES
    | XMLCONCAT
    | XMLELEMENT
    | XMLEXISTS
    | XMLFOREST
    | XMLNAMESPACES
    | XMLPARSE
    | XMLPI
    | XMLROOT
    | XMLSERIALIZE
    | XMLTABLE
    ;

/**
 * PL/SQL Keywords
 *
 * NOTE: These keywords are treated as identiifer in non PL/SQL.
 *       Nonetheless they are seperated because the lexer does.
 */
plsqlNonreservedKeyword
    : ABSOLUTE_P
    | ALIAS
    | AND
    | ARRAY
    | ASSERT
    | BACKWARD
    | CALL
    | CHAIN
    | CLOSE
    | COLLATE
    | COLUMN
    //| COLUMN_NAME
    | COMMIT
    | CONSTANT
    | CONSTRAINT
    //| CONSTRAINT_NAME
    | CONTINUE_P
    | CURRENT_P
    | CURSOR
    //| DATATYPE
    | DEBUG
    | DEFAULT
    //| DETAIL
    | DIAGNOSTICS
    | DO
    | DUMP
    | ELSIF
    //| ERRCODE
    | ERROR
    | EXCEPTION
    | EXIT
    | FETCH
    | FIRST_P
    | FORWARD
    | GET
    //| HINT
    
    //| IMPORT
    | INFO
    | INSERT
    | IS
    | LAST_P
    | LOG
    //| MESSAGE
    
    //| MESSAGE_TEXT
    | MOVE
    | NEXT
    | NO
    | NOTICE
    | OPEN
    | OPTION
    | PERFORM
    //| PG_CONTEXT
    
    //| PG_DATATYPE_NAME
    
    //| PG_EXCEPTION_CONTEXT
    
    //| PG_EXCEPTION_DETAIL
    
    //| PG_EXCEPTION_HINT
    | PRINT_STRICT_PARAMS
    | PRIOR
    | QUERY
    | RAISE
    | RELATIVE_P
    | RESET
    | RETURN
    //| RETURNED_SQLSTATE
    | REVERSE
    | ROLLBACK
    //| ROW_COUNT
    | ROWTYPE
    | SCHEMA
    //| SCHEMA_NAME
    | SCROLL
    | SET
    | SLICE
    | SQLSTATE
    | STACKED
    | TABLE
    //| TABLE_NAME
    | TYPE_P
    | USE_COLUMN
    | USE_VARIABLE
    | VARIABLE_CONFLICT
    | WARNING
    | OUTER_P
    ;

// Temp PL/SQL Keywords

//rk
//    : AND
//    | ARRAY
//    | COLLATE
//    | COLUMN
//    | CONSTRAINT
//    | DEFAULT
//    | DO
//    | FETCH
//    ;
//
//rk_cbfot
//    : IS
//    ;
//
//nrk
//    : ABSOLUTE_P
//    | BACKWARD
//    | CHAIN
//    | CLOSE
//    | COMMIT
//    | CURRENT_P
//    | CURSOR
//    | FIRST_P
//    | FORWARD
//    | INSERT
//    | LAST_P
//    | MOVE
//    | NEXT
//    | NO
//    | OPTION
//    | PRIOR
//    | RELATIVE_P
//    | RESET
//    | ROLLBACK
//    ;
//
//nrk_cnbfot
//    : ALIAS
//    | ASSERT
//    | CALL
//    | CONSTANT
//    | CONTINUE_P
//    | DEBUG
//    | DIAGNOSTICS
//    | DUMP
//    | ELSIF
//    | ERROR
//    | EXCEPTION
//    | EXIT
//    | GET
//    | INFO
//    | LOG
//    | NOTICE
//    | OPEN
//    | PERFORM
//    | PRINT_STRICT_PARAMS
//    | QUERY
//    | RAISE
//    | RETURN
//    | REVERSE
//    ;