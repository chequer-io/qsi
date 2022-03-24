parser grammar PostgreSqlParserInternal;


options {
    tokenVocab = PostgreSqlLexerInternal;
    superClass = PostgreSqlParserBase;
}


@header
{
}
@members
{
}

root
   : statement EOF
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

createAccessMethod
    :;

createAggregate
    :;

createCast
    :;

createCollation
    :;

createConversion
    :;

createDatabase
    :;

createDomain
    :;

createEventTrigger
    :;

createExtension
    :;

createForeign:
    FOREIGN (
        createForeignDataWrapper
        | createForeignTable
    );

createForeignDataWrapper
    :;

createForeignTable
    :;

createFunction
    :;

createGroup
    :;

createIndex
    :;

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

createTable
    :;

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
// TODO: Optimize; extremely slow when intake many brackets.
queryExpressionNoWith
    : (queryExpressionBody | queryExpressionParens)         // SELECT ...
    orderByClause?                                          // ORDER BY ...
    limitClause?                                            // LIMIT ... (OFFSET ...)
    forClause?                                              // FOR ... (OF ...)
    ;

// Expression with parantheses.
queryExpressionParens
    : OPEN_PAREN (queryExpressionParens | queryExpression) CLOSE_PAREN
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
    windowClause?                               # selectQueryPrimary
    | VALUES valueStatementItemList             # valuesQueryPrimary
    | TABLE tableName                           # tableQueryPrimary
    ;

valueStatementItemList
    : valueStatementItem (COMMA valueStatementItem)*
    ;

valueStatementItem
    : OPEN_PAREN expressionList CLOSE_PAREN
    ;

// Columns of the select statement.
// TODO: Create select item
selectItemList
    : (columnName | STAR) (COMMA columnName)*
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
    : insertStatementNoWith
    | withClause insertStatementNoWith
    ;

insertStatementNoWith
    : INSERT INTO tableName aliasClause? (OPEN_PAREN columnList CLOSE_PAREN)?
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
    : OPEN_PAREN conflictTargetItemList CLOSE_PAREN (WHERE indexPredicate)?
    | ON CONSTRAINT identifier
    ;
    
conflictTargetItemList
    : conflictTargetItem (COMMA conflictTargetItem)*
    ;

conflictTargetItem
    : (columnName | OPEN_PAREN expression CLOSE_PAREN) collate? opClass?
    ;

collate
    : COLLATE collation
    ;

// TODO: Implement collation.
// see: collation section at https://www.postgresql.org/docs/14/sql-insert.html
collation
    : TEMP
    ;

// TODO: Implement opClass.
// see: opclass section at https://www.postgresql.org/docs/14/sql-insert.html
opClass
    : TEMP
    ;

// TODO: Implement indexPredicate.
// see: index_predicate section at https://www.postgresql.org/docs/14/sql-insert.html
indexPredicate
    : TEMP
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
    : columnName EQUAL (expression | DEFAULT)                                                       #columnUpdateSet
    | OPEN_PAREN columnList CLOSE_PAREN EQUAL ROW? OPEN_PAREN updateSetExpressionList CLOSE_PAREN   #columnListUpdateSet
    | OPEN_PAREN columnList CLOSE_PAREN EQUAL OPEN_PAREN queryPrimary CLOSE_PAREN                   #subqueryUpdateSet
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
    (USING fromItemList)
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
    : identifier (OPEN_PAREN columnList CLOSE_PAREN)?
    ;

/**
 * FOR .. (OF ..);
 */
forClause
    // TODO: Implement table name expression.
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
    : fromItemPrimary
    | fromItemJoin
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
    : functionPrimary (WITH ORDINALITY)? aliasClause?                               # functionFromItemDefault
    | functionPrimary AS aliasName OPEN_PAREN columnDefinitionList CLOSE_PAREN      # functionFromItemWithAs
    | rowsFromFunctionPrimary (WITH ORDINALITY)? aliasClause?                       # functionFromItemWithRows
    ;

// Function item.
//
// NOTE: This node is kinda similar to functionExpression.
// Maybe using expression would be work too.
// Should use distinct node or existing expression?
functionPrimary
    : identifier OPEN_PAREN argumentList CLOSE_PAREN;

// Function item that has a ROWS FROM block.
rowsFromFunctionPrimary
    : ROWS FROM OPEN_PAREN functionPrimary CLOSE_PAREN;

// Subquery item.
subqueryFromItem
    : queryExpressionParens aliasClause?;

// Join item.
fromItemJoin
    : fromItemPrimary join+;

// Join
join
    : CROSS JOIN fromItemPrimary
    | NATURAL? joinType? JOIN fromItemPrimary;
    
joinType
    : (FULL | LEFT | RIGHT | INNER_P) OUTER_P?;

/**
 * GROUP BY
 */
groupByClause
    : GROUP_P BY (ALL | DISTINCT)? groupByItemList;

/**
 * HAVING
 */
havingClause
    : HAVING expression;

/**
 * INTO
 */
intoClause
    : INTO intoClauseOptions? TABLE? tableName;

// Table options for into clause.
intoClauseOptions
    : TEMPORARY
    | TEMP
    | UNLOGGED;

/**
 * LIMIT, FETCH, OFFSET
 */
limitClause
    : limit (offset)?
    | offset (limit)?;
 
limit
    : LIMIT expression | ALL
    | FETCH (FIRST_P | NEXT) expression (ROW | ROWS) (ONLY | WITH TIES);

offset
    : OFFSET (expression (ROW | ROWS)? | ALL);

/**
 * ORDER BY
 */
orderByClause
    : ORDER BY orderList;

orderList
    : orderExpression (COMMA orderExpression)*
    ;

orderExpression
    : expression (ASC | DESC)?
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
    : TABLESAMPLE samplingMethodName OPEN_PAREN argumentList CLOSE_PAREN (REPEATABLE OPEN_PAREN seed CLOSE_PAREN)?
    ;

seed
    : expression
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
    : columnName AS windowSpecification;

windowSpecification
    : OPEN_PAREN 
    windowName?
    (PARTITION BY expressionList)?
    (ORDER BY windowOrderByExpressionList)? 
    frameClause?
    CLOSE_PAREN;

windowOrderByExpressionList
    : windowOrderByExpression (COMMA windowOrderByExpression)*;

windowOrderByExpression
    : expression (ASC | DESC | USING operator) (NULLS_P (FIRST_P | LAST_P))?;

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
 * WITH
 */
withClause
    : WITH RECURSIVE? commonTableExpression (COMMA commonTableExpression)*;

// CTE(Common Table Expression).
commonTableExpression
    : subqueryName (OPEN_PAREN argumentList CLOSE_PAREN)? 
    AS commonTableExpressionOption? 
    OPEN_PAREN commonTableExpressionStatements CLOSE_PAREN;

commonTableExpressionOption
    : MATERIALIZED
    | NOT MATERIALIZED;

commonTableExpressionStatements
    : selectStatement;
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
    : booleanExpression
    | NOT expression
    | expression AND expression
    | expression OR expression
    ;

/**
 * Boolean Expression - with IS keyword
 */
booleanExpression
    : comparisonExpression
    | booleanExpression IS NOT? NULL_P
    | booleanExpression (ISNULL | NOTNULL)
    | booleanExpression IS NOT? TRUE_P
    | booleanExpression IS NOT? FALSE_P
    | booleanExpression IS NOT? UNKNOWN
    | booleanExpression IS NOT? DISTINCT FROM
    ;

/**
 * Comparison Expression - with comparison operators, subqueries
 */
comparisonExpression
    : likeExpression
    | comparisonExpression comparisonOperator comparisonExpression
    | comparisonExpression subqueryOperator subqueryType (queryExpressionParens | OPEN_PAREN expression CLOSE_PAREN)
    ;

subqueryType
    : ANY
    | SOME
    | ALL
    ;

/**
 * Like expression - with LIKE, ILIKE, SIMILAR TO, BETWEEN keywords
 */
likeExpression
    : qualifiedOperatorExpression likeExpressionOptions qualifiedOperatorExpression (ESCAPE expression)?
    | qualifiedOperatorExpression
    ;

likeExpressionOptions
    : NOT? LIKE
    | NOT? ILIKE
    | SIMILAR TO
    | BETWEEN SYMMETRIC?
    ;

/**
 * Qualified Operator Expression - with user defined operators
 */
qualifiedOperatorExpression
    : unaryQualifiedOperatorExpression (qualifiedOperator unaryQualifiedOperatorExpression)*
    ;

unaryQualifiedOperatorExpression
    : qualifiedOperator? arithmeticExpression
    ;

/**
 * Arithmetic Expression - with basic mathematical operators
 */
arithmeticExpression
    : fooExpression
    | (PLUS | MINUS) arithmeticExpression
    | arithmeticExpression CARET arithmeticExpression
    | arithmeticExpression (STAR | SLASH | PERCENT) arithmeticExpression
    | arithmeticExpression (PLUS | MINUS) arithmeticExpression
    ;

/**
 * UNNAMED Expression.
 * TODO: Should be named; though not yet grouped by sth
 */
fooExpression
    // NOTE: Implementation of [] operator?
    : typecastExpression (COLLATE identifier)?
    ;

/**
 * Typecast Expression - with :: keyword.
 */
typecastExpression
    : valueExpression (TYPECAST identifier)*
    ;

/**
 * Value Expression
 * See: https://www.postgresql.org/docs/14/sql-expressions.html
 */
valueExpression
    : EXISTS queryExpressionParens
    | ARRAY (queryExpressionParens | arrayExpression)
    | PARAM indirection?
    | GROUPING OPEN_PAREN expressionList CLOSE_PAREN
    | UNIQUE queryExpressionParens
    | columnReference
    | constant
    | OPEN_PAREN expression CLOSE_PAREN indirection?
    | caseExpression
    | functionExpression
    | queryExpressionParens indirection?
    | explicitRow
    | implicitRow
    | row OVERLAPS row
    ;

/**
 * Array Expression
 */
arrayExpression
    : OPEN_BRACKET expressionList CLOSE_BRACKET
    ;

/**
 * Indirection
 * TODO: Get the definition of Indirection.
 */
indirection
    : indirectionElement+
    ;

indirectionElement
    : DOT (identifier | STAR)
    | OPEN_PAREN (expression | expression? COLON expression?) CLOSE_PAREN
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
 * ROW clause
 */
row
    : explicitRow
    | implicitRow
    ;

explicitRow
    : ROW OPEN_PAREN expressionList? CLOSE_PAREN
    ;

/*
Copied from the open-source PG parser:

TODO:
for some reason v1
implicit_row: OPEN_PAREN expr_list COMMA a_expr CLOSE_PAREN;
works better than v2
implicit_row: OPEN_PAREN expr_list  CLOSE_PAREN;
while looks like they are almost the same, except v2 requieres at least 2 items in list
while v1 allows single item in list
*/
implicitRow
    : OPEN_PAREN expressionList COMMA expression CLOSE_PAREN
    ;

/**
 * Function Expression
 *
 * See 4.2.6, 4.2.7, 4.2.8 of: 
 * https://www.postgresql.org/docs/14/sql-expressions.html#SQL-EXPRESSIONS-FUNCTION-CALLS
 */
functionExpression
    : functionApplication withinGroupClause? filterClause? overClause?
// NOTE: func_expr_common_subexpr has been used in the open-source version of PG parser.
// Guess it's been used for commonly used function format.
// Ignoring it since creating a generic grammar of the function expression would cover it's definition.
//    | func_expr_common_subexpr
    ;

functionApplication
    : identifier OPEN_PAREN functionApplicationArgument? CLOSE_PAREN
    ;

functionApplicationArgument
    : complexArgumentList (COMMA VARIADIC argumentExpression)? orderByClause?
    | VARIADIC argumentExpression orderByClause?
    | (ALL | DISTINCT) complexArgumentList orderByClause?
    | STAR
    ;

/**
 * WITHIN GROUP Clause
 */
withinGroupClause
    : WITHIN GROUP_P OPEN_PAREN orderByClause CLOSE_PAREN
    ;

/**
 * FILTER clause
 */
filterClause
    : FILTER OPEN_PAREN WHERE expression CLOSE_PAREN
    ;

/**
 * OVER clause
 */
overClause
    : OVER (windowSpecification | identifier)
    ;

//----------------- OPERATORS ------------------------------------------------------------------------------------------

operator
    : TEMP;

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

subqueryOperator
    : allOperator
    | OPERATOR OPEN_PAREN anyOperator CLOSE_PAREN
    | NOT? LIKE
    | NOT? ILIKE
    ;

qualifiedOperator
    : Operator
    | OPERATOR OPEN_PAREN anyOperator CLOSE_PAREN
    ;

anyOperator
    : (columnName DOT)* allOperator
    ;

allOperator
    : Operator
    | mathOperator
    ;
    
//----------------- TEMPORARY NODES ------------------------------------------------------------------------------------
// Nodes that are not implemented yet, but required to implement other nodes.

/**
 * Identifier
 */
identifier
    : pureIdentifier
    | nonReservedKeyword
    // Added temporarily
    | noTypeOrFunctionNonReservedKeyword
    | reservedKeyword
    | typeOrFunctionReservedKeyword
    ;

pureIdentifier
    : Identifier
    | QuotedIdentifier
    ;

aliasName
    : identifier
    ;

columnName
    : identifier
    ;

cursorName
    : identifier;

samplingMethodName
    : identifier
    ;

subqueryName
    : identifier
    ;

tableName
    : identifier
    ;

windowName
    : identifier
    ;

/**
 * Identifier List
 */
identifierList
    : identifier (COMMA identifier)*
    ;

argumentList
    : identifierList
    ;

// NOTE: Is argument above and below same?

complexArgumentList
    : argumentExpression (COMMA argumentExpression)*
    ;

argumentExpression
    : expression
    | identifier (COLON_EQUALS | EQUALS_GREATER) expression
    ;

columnList
    : identifierList
    ;

groupByItemList
    : identifierList
    ;

tableList
    : identifierList
    ;

/**
 * value
 */
columnReference
    : identifier (DOT identifier)*
    ;

columnDefinitionList
    : columnDefinition (COMMA columnDefinition)*;
    
columnDefinition
    : columnName dataType;

dataType
    : TEMP;

bitType
    : BIT VARYING? (OPEN_PAREN unsignedInt CLOSE_PAREN)?
    ;

characterType
    : characterPrefix (OPEN_PAREN unsignedInt CLOSE_PAREN)?
    ;

characterPrefix
    : (CHARACTER | CHAR_P | NCHAR) VARYING?
    | VARCHAR
    | NATIONAL (CHARACTER | CHAR_P) VARYING?
    ;

numericType
    : INT_P
    | INTEGER
    | SMALLINT
    | BIGINT
    | REAL
    | FLOAT_P (OPEN_PAREN unsignedInt CLOSE_PAREN)?
    | DOUBLE_P PRECISION
    | DECIMAL_P typeModifier
    | DEC typeModifier
    | NUMERIC typeModifier
    | BOOLEAN_P
    ;

typeModifier
    : OPEN_PAREN expressionList CLOSE_PAREN
    ;

dateTimeType
    : (TIMESTAMP | TIME) (OPEN_PAREN int CLOSE_PAREN)? timezoneOption
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
    | identifier indirection? (OPEN_PAREN complexArgumentList orderByClause CLOSE_PAREN) string
    | constType string
    | INTERVAL (string intervalOption? | OPEN_PAREN int CLOSE_PAREN string)
    | TRUE_P
    | FALSE_P
    | NULL_P
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
    : SECOND_P (OPEN_PAREN int CLOSE_PAREN)?
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

typeOrFunctionReservedKeyword
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

noTypeOrFunctionNonReservedKeyword
   : BETWEEN
   | BIGINT
   | bitType
   | BOOLEAN_P
   | CHAR_P
   | characterType
   | COALESCE
   | DEC
   | DECIMAL_P
   | EXISTS
   | EXTRACT
   | FLOAT_P
   | GREATEST
   | GROUPING
   | INOUT
   | INT_P
   | INTEGER
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
