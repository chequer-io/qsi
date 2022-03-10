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
 * DELETE
 */
// TODO: Implement delete statement.
deleteStatement
    :;

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
    : HAVING condition;

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
    : LIMIT value | ALL
    | FETCH (FIRST_P | NEXT) value (ROW | ROWS) (ONLY | WITH TIES);

offset
    : OFFSET (value (ROW | ROWS)? | ALL);

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

/**
 * WHERE
 */
whereClause
    : WHERE condition;

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

//----------------- OPERATORS ------------------------------------------------------------------------------------------

setOperator
    : UNION
    | INTERSECT
    | EXCEPT;

setOperatorOption
    : ALL
    | DISTINCT;
    
//----------------- TEMPORARY NODES ------------------------------------------------------------------------------------
// Nodes that are not implemented yet, but required to implement other nodes.

/**
 * Identifier
 */
identifier
    : TEMP
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
 * Expressions
 */
expressionList
    : expression (COMMA expression)*
    ;

expression
    : TEMP
    ;

condition
    : expression
    ;

/**
 * value
 */
value
    : TEMP
    ;

offsetValue
    : value
    ;

seed
    : value
    ;

operator
    : TEMP;

columnDefinitionList
    : columnDefinition (COMMA columnDefinition)*;
    
columnDefinition
    : columnName dataType;

dataType
    : TEMP;