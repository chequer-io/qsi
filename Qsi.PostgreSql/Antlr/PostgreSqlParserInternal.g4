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
   : statement (SEMI EOF? | EOF)
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
   | setStatement
   | resetStatement
   ;

//----------------- DDL statements -------------------------------------------------------------------------------------

/**
* ALTER STATEMENT
*/
alterStatement
    : alterAggregate
    | alterCollation
    | alterConversion
    | alterDatabase
    | alterDefaultPrivileges
    | alterDomain
    | alterEventTrigger
    | alterExtension
    | alterForeign
    | alterFunction
    | alterGroup
    | alterIndex
    | alterLanguage
    | alterLargeObject
    | alterMaterializedView
    | alterOperator
    | alterPolicy
    | alterProcedure
    | alterPublication
    | alterRole
    | alterRoutine
    | alterRule
    | alterSchema
    | alterSequence
    | alterServer
    | alterStatistics
    | alterSubscription
    | alterTable
    | alterTableSpace
    | alterTextSearch
    | alterTrigger
    | alterType
    | alterUserMapping
    | alterView
    ;

// TODO: Implement alter statement
alterAggregate
    :;
    
alterCollation
    :;

alterConversion
    :;

alterDatabase
    :;

alterDefaultPrivileges
    :;
    
alterDomain
    :;

alterEventTrigger
    :;

alterExtension
    :;

alterForeign
    : FOREIGN (
        alterForeignDataWrapper
        | alterForeignTable
    );
    
alterForeignDataWrapper
    :;

alterForeignTable
    :;

alterFunction
    :;

alterGroup
    :;

alterIndex
    :;

alterLanguage
    :;

alterLargeObject
    :;

alterMaterializedView
    :;

alterOperator
    : OPERATOR (
        // TODO: Implement ALTER OPERATOR clause.
        alterOperatorClass
        | alterOperatorFamily
    );
    
alterOperatorClass
    :;

alterOperatorFamily
    :;

alterPolicy
    :;

alterProcedure
    :;

alterPublication
    :;

/**
 * ALTER ROLE, ALTER USER
 *
 * ALTER USER is an alias for ALTER ROLE.
 *
 * See: https://www.postgresql.org/docs/14/sql-alterrole.html
 * See also: https://www.postgresql.org/docs/14/sql-alteruser.html
 */
alterRole
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
    | CONNECTION LIMIT int
    | VALID UNTIL string
    | USER role (',' role)*
    | identifier
    ;

alterRoutine
    :;

alterRule
    :;

alterSchema
    :;

alterSequence
    :;

alterServer
    :;

alterStatistics
    :;

alterSubscription
    :;

alterTable
    :;

alterTableSpace
    :;

alterTextSearch:
    TEXT_P SEARCH (
        alterTextSearchConfiguration
        | alterTextSearchDictionary
        | alterTextSearchParser
        | alterTextSearchTemplate
    );

alterTextSearchConfiguration
    :;

alterTextSearchDictionary
    :;

alterTextSearchParser
    :;

alterTextSearchTemplate
    :;


alterTrigger
    :;

alterType
    :;
    
alterUserMapping
    :;

alterView
    :;

// TODO: Implement create statement.
createStatement
    : createAccessMethod
    | createAggregate
    | createCast
    | createCollation
    | createConversion
    | createDatabase
    | createDomain
    | createEventTrigger
    | createExtension
    | createForeignDataWrapper
    | createForeignTable
    | createFunction
    | createIndex
    | createLanguage
    | createMaterializedView
    | createOperator
    | createOperatorClass
    | createOperatorFamily
    | createPolicy
    | createPublication
    | createRole
    | createRule
    | createSchema
    | createSequence
    | createServer
    | createTable
    | createTablespace
    | createTextSearch
    | createTransform
    | createTrigger
    | createType
    | createUserMapping
    | createView
    ;

/**
 * CREATE ACCESS METHOD
 *
 * See: https://www.postgresql.org/docs/14/sql-create-access-method.html
 */
createAccessMethod
    : CREATE ACCESS METHOD columnIdentifier TYPE_P (INDEX | TABLE) HANDLER qualifiedIdentifier
    ;

/**
 * CREATE AGGREGATE
 *
 * See: https://www.postgresql.org/docs/14/sql-createaggregate.html
 */
createAggregate
    : CREATE (OR REPLACE)? AGGREGATE functionName createAggregateArgumentOption
    ;

createAggregateArgumentOption
    : '(' aggregateArgumentDefinitions ')' '(' definitionList ')'
    | '(' aggregateArgumentListOldSyntax ')'
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
 * CREATE CAST
 *
 * See: https://www.postgresql.org/docs/14/sql-createcast.html
 */
createCast
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
createCollation
    : CREATE COLLATION (IF_P NOT EXISTS)? qualifiedIdentifier '(' definitionList ')'
    ;

/**
 * CREATE CONVERSION
 *
 * See: https://www.postgresql.org/docs/14/sql-createconversion.html
 */
createConversion
    : CREATE DEFAULT? CONVERSION_P qualifiedIdentifier FOR string TO string FROM qualifiedIdentifier
    ;

/**
 * CREATE DATABASE
 *
 * See: https://www.postgresql.org/docs/14/sql-createdatabase.html
 */
createDatabase
    : CREATE DATABASE columnIdentifier WITH? createDatabaseItem*
    ;

createDatabaseItem
    : createDatabaseItemName EQUAL? (int | booleanOrString | DEFAULT)
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
createDomain
    : CREATE DOMAIN_P qualifiedIdentifier AS? type columnConstraint*
    ;

/**
 * CREATE EVENT TRIGGER
 *
 * See: https://www.postgresql.org/docs/14/sql-createeventtrigger.html
 */
createEventTrigger
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
createExtension
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
createForeignDataWrapper
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
createForeignTable
    : CREATE FOREIGN TABLE (IF_P NOT EXISTS)? qualifiedIdentifier createTableOptions SERVER columnIdentifier genericOptions?
    ;

/**
 * CREATE FUNCTION, CREATE PROCEDURE
 *
 * See: https://www.postgresql.org/docs/14/sql-createfunction.html
 * See also: https://www.postgresql.org/docs/14/sql-createprocedure.html
 */
createFunction
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
createIndex
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
createLanguage
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
createMaterializedView
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
createOperator
    : CREATE OPERATOR operator '(' definitionList ')'
    ;

/**
 * CREATE OPERATOR CLASS
 *
 * See: https://www.postgresql.org/docs/14/sql-createopclass.html
 */
createOperatorClass
    : CREATE OPERATOR CLASS qualifiedIdentifier DEFAULT?
        FOR TYPE_P type USING columnIdentifier
        (FAMILY qualifiedIdentifier)? AS createOperatorClassItemList
    ;

createOperatorClassItemList
    : createOperatorClassItem (',' createOperatorClassItem)*
    ;

createOperatorClassItem
    : OPERATOR unsignedInt operator operatorArgumentTypes? createOperatorClassPurpose? RECHECK?
    | FUNCTION unsignedInt ('(' type (',' type)* ')')? argumentDefinitionList
    | STORAGE type
    ;

createOperatorClassPurpose
    : FOR (SEARCH | ORDER BY qualifiedIdentifier)
    ;

operatorArgumentTypes
    : '(' type ')'
    | '(' type ',' type ')'
    | '(' NONE ',' type ')'
    | '(' type ',' NONE ')'
    ;

createOperatorFamily
    :;

createPolicy
    :;

createPublication
    :;

/**
 * CREATE ROLE, CREATE USER, CREATE GROUP
 *
 * CREATE USER, CREATE GROUP is an alias for CREATE ROLE.
 *
 * See: https://www.postgresql.org/docs/14/sql-createrole.html
 * See also: https://www.postgresql.org/docs/14/sql-createuser.html
 * See also: https://www.postgresql.org/docs/14/sql-creategroup.html
 */
createRole
    : CREATE (ROLE | USER | GROUP_P) role WITH? createRoleOption*
    ;

createRoleOption
    : alterRoleOption
    | SYSID unsignedInt
    | (ADMIN | ROLE | IN_P (ROLE | GROUP_P)) role (',' role)*
    ;

createRule
    :;

createSchema
    :;

createSequence
    :;

createServer
    :;

/**
 * CREATE TABLE
 *
 * See: https://www.postgresql.org/docs/14/sql-createtable.html
 */
createTable
    : createTablePrefix? TABLE (IF_P NOT EXISTS)? qualifiedIdentifier createTableOptions
        partitionByClause?
        usingClause?
        withOptionsClause?
        onCommitClause?
        tableSpaceClause?
    ;

createTablePrefix
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

createTablespace
    :;

createTextSearch:
    TEXT_P SEARCH (
        createTextSearchConfiguration
        | createTextSearchDictionary
        | createTextSearchParser
        | createTextSearchTemplate
    );

createTextSearchConfiguration
    :;

createTextSearchDictionary
    :;

createTextSearchParser
    :;

createTextSearchTemplate
    :;


createTransform
    :;

createTrigger
    :;

createType
    :;
    
createUserMapping
    :;

createView
    :;
    
dropStatement:
    DROP (
        dropAccessMethod
        | dropAggregate
        | dropCast
        | dropCollation
        | dropConversion
        | dropDatabase
        | dropDomain
        | dropEventTrigger
        | dropExtension
        | dropForeign
        | dropFunction
        | dropGroup
        | dropIndex
        | dropLanguage
        | dropMaterializedView
        | dropOperator
        | dropOwned
        | dropPolicy
        | dropProcedure
        | dropPublication
        | dropRole
        | dropRoutine
        | dropRule
        | dropSchema
        | dropSequence
        | dropServer
        | dropStatistics
        | dropSubscription
        | dropTable
        | dropTablespace
        | dropTextSearch
        | dropTransform
        | dropTrigger
        | dropType
        | dropUser
        | dropView
    );

dropAccessMethod
    :;

dropAggregate
    :;

dropCast
    :;

dropCollation
    :;

dropConversion
    :;

dropDatabase
    :;

dropDomain
    :;

dropEventTrigger
    :;

dropExtension
    :;

dropForeign:
    FOREIGN (
        dropForeignDataWrapper
        | dropForeignTable
    );
    
dropForeignDataWrapper
    :;
    
dropForeignTable
    :;

dropFunction
    :;

dropGroup
    :;

dropIndex
    :;

dropLanguage
    :;

dropMaterializedView
    :;

dropOperator:
    OPERATOR (
        // TODO: Implement CREATE OPERATOR clause.
        dropOperatorClass
        | dropOperatorFamily
    );
    
dropOperatorClass
    :;

dropOperatorFamily
    :;

dropOwned
    :;

dropPolicy
    :;

dropProcedure
    :;

dropPublication
    :;

dropRole
    :;

dropRoutine
    :;

dropRule
    :;

dropSchema
    :;

dropSequence
    :;

dropServer
    :;

dropStatistics
    :;

dropSubscription
    :;

dropTable
    :;

dropTablespace
    :;

dropTextSearch:
    TEXT_P SEARCH (
        dropTextSearchConfiguration
        | dropTextSearchDictionary
        | dropTextSearchParser
        | dropTextSearchTemplate
    );

dropTextSearchConfiguration
    :;

dropTextSearchDictionary
    :;

dropTextSearchParser
    :;

dropTextSearchTemplate
    :;


dropTransform
    :;

dropTrigger
    :;

dropType
    :;

dropUser:
    USER (
        // TODO: Implement DROP USER clause.
        dropUserMappings
    );
    
dropUserMappings
    :;

dropView
    :;

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
    : (expression | STAR) aliasClause?
    ;

// Options for select statement.
selectOption
    : ALL
    | DISTINCT
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
    : DELETE_P FROM ONLY? tableName STAR? aliasClause? USING fromItemList
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
    : columnIdentifier ('(' qualifiedIdentifierList ')')?
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
    : join (ON expression | USING '(' columnIdentifier (COMMA columnIdentifier)* ')')? aliasClause?
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
expression
    : expressionParens
    | expressionNoParens
    ;

expressionParens
    : '(' (expressionParens | expression) ')'
    ;

expressionNoParens
    : andExpression
    | expressionNoParens OR expressionNoParens
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
    | booleanExpression IS NOT? DISTINCT FROM
    ;

/**
 * Comparison Expression - with comparison operators, subqueries
 */
// TODO: likeExpressionOptions and subqueryOperator have same operators.
//       Lookahead increases because of that (k = 5). Should we fix this?
comparisonExpression
    : qualifiedOperatorExpression (likeExpressionOptions qualifiedOperatorExpression (ESCAPE expression)?)?
    | comparisonExpression comparisonOperator comparisonExpression
    | comparisonExpression subqueryOperator subqueryType (queryExpressionParens | '(' expression ')')
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
    : typecastExpression (COLLATE indirectionExpression)?
    ;

/**
 * Typecast Expression - with :: keyword.
 */
typecastExpression
    : indirectionExpression (TYPECAST type)*
    ;

/**
 * Array Expression - with [](brackets).
 */
indirectionExpression
    : valueExpression indirection*
    ;

/**
 * Value Expression
 * See: https://www.postgresql.org/docs/14/sql-expressions.html
 */
// TODO: Check lookahead
valueExpression
    : (EXISTS | UNIQUE | ARRAY)? queryExpressionParens                              // Subquery
    | ARRAY OPEN_BRACKET expressionList CLOSE_BRACKET                               // ARRAY Constructor
    | GROUPING '(' expressionList ')'                  // GROUPING
    | columnIdentifier                                                              // identifier TODO: reduce max k
    | constant
    | caseExpression                                                                // CASE ~ END
    | functionExpression                                                            // function call TODO: reduce max k
    | row
    | row OVERLAPS row // TODO: reduce max k
    | PARAM                                                                         // PARAM
    ;

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
    | columnIdentifier indirection
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
    | CURRENT_TIME ('(' int ')')?
    | CURRENT_TIMESTAMP ('(' int ')')?
    | LOCALTIME ('(' int ')')?
    | LOCALTIMESTAMP ('(' int ')')?
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
    : columnIdentifier (DOT (columnLabelIdentifier | STAR))*
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
indirection
    : (DOT columnLabelIdentifier | STAR)
    | arraySubscript
    ;

arraySubscript
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
    : identifierList
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
    : (OPEN_BRACKET int? CLOSE_BRACKET)+
    | ARRAY (OPEN_BRACKET int CLOSE_BRACKET)*
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
    : (TIMESTAMP | TIME) ('(' int ')')? timezoneOption
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
    : INTERVAL (intervalOption | '(' int ')')
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
    : int
    | float
    | hex
    | bin
    | string
//    | functionName (string | '(' /* TODO: Implement functionArgumentList on constants */ orderByClause ')' string)
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
    : INTERVAL (string intervalOption | '(' int ')' string)
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
    : SECOND_P ('(' int ')')?
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
int
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
    : stringBody (UESCAPE stringBody)?
    ;

stringBody
    : StringConstant
    | UnicodeEscapeStringConstant
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
// Nodes that are not implemented yet, but required to implement other nodes.

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
    | int                       #withSignInt
    ;

/**
 * Function Definition
 */
functionDefinition
    : functionName '(' argumentDefinitionList? ')'
    | typeFunctionKeyword
    | columnIdentifier indirection*
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