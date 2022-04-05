parser grammar PostgreSqlParserInternal;


options {
    tokenVocab = PostgreSqlLexerInternal;
    superClass = PostgreSqlParserBase;
}


@header
{
    using Qsi.Data;
    using Qsi.Tree;
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
   // TODO: Only SELECT, INSERT, UPDATE, and DELETE statement have with clause.
   //       Should I group these 4 statements and use with in front of the group, not seperately?
   | selectStatement
   | insertStatement
   | updateStatement
   | deleteStatement
   | truncateStatement
   ;

//----------------- DDL statements -------------------------------------------------------------------------------------

/**
* ALTER STATEMENT
*/
alterStatement:
    ALTER (
        alterAggregate
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
        | alterUser
        | alterView
    );

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

alterRole
    :;

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

alterUser
    : USER (
        // TODO: Implement ALTER USER clause.
        alterUserMapping
    ); 
    
alterUserMapping
    :;

alterView
    :;

// TODO: Implement create statement.
createStatement:
    CREATE (
        createAccessMethod
        | createAggregate
        | createCast
        | createCollation
        | createConversion
        | createDatabase
        | createDomain
        | createEventTrigger
        | createExtension
        | createForeign
        | createFunction
        | createGroup
        | createIndex
        | createLanguage
        | createMaterializedView
        | createOperator
        | createPolicy
        | createProcedure
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
        | createUser
        | createView
    );

/**
 * CREATE ACCESS METHOD
 */
createAccessMethod
    : ACCESS METHOD columnIdentifier TYPE_P (INDEX | TABLE) HANDLER qualifiedIdentifier
    ;

/**
 * CREATE AGGREGATE METHOD
 *
 * See: https://www.postgresql.org/docs/14/sql-createaggregate.html
 */
createAggregate
    : (OR REPLACE)? AGGREGATE functionName createAggregateArgumentOption
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
    : CAST '(' type AS type ')' createCastOption castContext?
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
    : COLLATION (IF_P NOT EXISTS)? qualifiedIdentifier '(' definitionList ')'
    ;

/**
 * CREATE CONVERSION
 *
 * See: https://www.postgresql.org/docs/14/sql-createconversion.html
 */
createConversion
    : DEFAULT? CONVERSION_P qualifiedIdentifier FOR string TO string FROM qualifiedIdentifier
    ;

/**
 * CREATE DATABASE
 *
 * See: https://www.postgresql.org/docs/14/sql-createdatabase.html
 */
createDatabase
    : DATABASE columnIdentifier WITH? createDatabaseItem*
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
    : DOMAIN_P qualifiedIdentifier AS? type columnConstraint*
    ;

/**
 * CREATE EVENT TRIGGER
 *
 * See: https://www.postgresql.org/docs/14/sql-createeventtrigger.html
 */
createEventTrigger
    : EVENT TRIGGER columnIdentifier ON columnLabelIdentifier (WHEN eventTriggerWhenList)?
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
    : EXTENSION (IF_P NOT EXISTS)? columnIdentifier WITH? createExtensionOptionList?
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
 * CREATE FOREIGN ~
 */
createForeign:
    FOREIGN (
        createForeignDataWrapper
        | createForeignTable
    );

createForeignDataWrapper
    :;

/**
 * CREATE FOREIGN TABLE
 *
 * See: https://www.postgresql.org/docs/14/sql-createforeigntable.html
 */
// TODO: Cleanup codes
// TODO: Implement CREATE TABLE keyword first
createForeignTable
    : TABLE (IF_P NOT EXISTS)? qualifiedIdentifier createTableOptions SERVER columnIdentifier genericOptions?
    ;

createFunction
    :;

createGroup
    :;

/**
 * CREATE INDEX
 */
createIndex
    : UNIQUE? INDEX CONCURRENTLY? (IF_P NOT EXISTS)? columnIdentifier
        ON tableName (USING columnIdentifier)? '(' indexList ')'
        includeClause?
        withOptionsClause?
        tableSpaceClause?
        whereClause?
    ;

createLanguage
    :;

createMaterializedView
    :;

createOperator:
    OPERATOR (
        // TODO: Implement CREATE OPERATOR clause.
        | createOperatorClass
        | createOperatorFamily
    );
    
createOperatorClass
    :;

createOperatorFamily
    :;

createPolicy
    :;

createProcedure
    :;

createPublication
    :;

createRole
    :;

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

createUser:
    USER (
        // TODO: Implement CREATE USER clause.
        createUserMapping
    );
    
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
// TODO: Implement insert statement.
insertStatement
    : withClause insertStatementNoWith
    | insertStatementNoWith
    ;

insertStatementNoWith
    : INSERT INTO tableName aliasClause? ('(' qualifiedIdentifierList ')')?
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
    : '(' conflictTargetItemList ')' whereClause?
    | ON CONSTRAINT columnIdentifier
    ;
    
conflictTargetItemList
    : conflictTargetItem (COMMA conflictTargetItem)*
    ;

conflictTargetItem
    : (columnIdentifier | '(' expression ')') collateClause? opClass?
    ;

opClass
    : expression
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
    : COLLATE expression
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
    : '(' 
    windowName?
    (PARTITION BY expressionList)?
    orderByClause? 
    frameClause?
    ')';

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
    ;
// TODO: Activate statements after implementation.
//    | update_statement
//    | insert_statement
//    | delete_statement;

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
    : operator
    | OPERATOR '(' simpleOperator ')'
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
    | '(' expression ')'
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

TODO:
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
    : FROM FROM FROM //expression IN_P expression // TODO: Should we consider binary calculation only expression such as b_expr?
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
    | OPERATOR '(' simpleOperator ')'
    ;

qualifiedOperator
    : operator
    | OPERATOR '(' simpleOperator ')'
    ;

simpleOperator
    : (columnIdentifier DOT)* operator
    ;

operator
    : Operator
    | mathOperator
    ;

//----------------- IDENTIFIERS ----------------------------------------------------------------------------------------

/**
 * Identifier
 */
identifier returns [QsiIdentifier id]
    : t=Identifier              { $id = new QsiIdentifier($t.text, false); }
    | t=QuotedIdentifier        { $id = new QsiIdentifier($t.text, true); }
    | t=UnicodeQuotedIdentifier { $id = new QsiIdentifier($t.text, true); }
    ;

columnLabelIdentifier returns [QsiIdentifier id]
    : i=identifier { $id = $i.id; }
    | rKey=reservedKeyword { $id = new QsiIdentifier($rKey.text, false); }
    | nrKey=nonReservedKeyword { $id = new QsiIdentifier($nrKey.text, false); }
    | cKey=columnKeyword { $id = new QsiIdentifier($cKey.text, false); }
    | tKey=typeFunctionKeyword { $id = new QsiIdentifier($tKey.text, false); }
    ;

columnIdentifier returns [QsiIdentifier id]
    : i=identifier { $id = $i.id; }
    | nrKey=nonReservedKeyword { $id = new QsiIdentifier($nrKey.text, false); }
    | cKey=columnKeyword { $id = new QsiIdentifier($cKey.text, false); }
    ;

typeFunctionIdentifier returns [QsiIdentifier id]
    : i=identifier { $id = $i.id; }
    | nrKey=nonReservedKeyword { $id = new QsiIdentifier($nrKey.text, false); }
    | tKey=typeFunctionKeyword { $id = new QsiIdentifier($tKey.text, false); }
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
    | DECIMAL_P typeModifier?
    | DEC typeModifier?
    | NUMERIC typeModifier?
    | REAL
    | DOUBLE_P PRECISION
    | FLOAT_P ('(' unsignedInt ')')?
    | BOOLEAN_P
    ;

typeModifier
    : '(' expressionList ')'
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
 * TODO with_la was used
 */ 
intervalType
    : INTERVAL (intervalOption | '(' int ')')
    ;

timezoneOption
    : (WITH | WITHOUT) TIME ZONE
    ;

//----------------- CONSTANTS ------------------------------------------------------------------------------------------

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

unsignedInt
    : Integral
    ;

int
    : PLUS? unsignedInt
    | MINUS unsignedInt
    ;

float
    : Numeric
    ;

hex
    : HexadecimalStringConstant
    ;

bin
    : BinaryStringConstant
    ;

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
    : index WITH (simpleOperator | OPERATOR '(' simpleOperator ')')
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

// TODO: Move to proper place; this is related to column.
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
    : (COLLATE qualifiedIdentifier)? qualifiedIdentifier? (ASC | DESC)? (NULLS_P (FIRST_P | LAST_P))?
    | (COLLATE qualifiedIdentifier)? qualifiedIdentifier 
    ;

// TODO: Find out what is relOptionList and implement it.
//       Still dunno what the hell it is but needs to be implemented (What does 'rel' stand for?), so implemented it.
//       Probably for option definition.
relOptionList
    : relOption (COMMA relOption)*
    ;

relOption
    : columnLabelIdentifier (EQUAL definitionArgument | DOT columnLabelIdentifier (EQUAL definitionArgument)?)?
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
   | bitType
   | BOOLEAN_P
   | CHAR_P
   | characterType
   | COALESCE
   | DEC
   | DECIMAL_P
   | EXISTS
   | EXTRACT
   | GREATEST
   | GROUPING
   | INOUT
   | INTERVAL
   | LEAST
   | NATIONAL
   | NCHAR
   | NONE
   | NORMALIZE
   | NULLIF
   | numericType
   | OUT_P
   | OVERLAY
   | POSITION
   | PRECISION
   | REAL
   | ROW
   | SETOF
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
