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
   : alter_statement
   | create_statement
   | drop_statement
   
   // DML
   | select_statement
   | insert_statement
   | update_statement
   | delete_statement
   | values_statement
   ;

//----------------- DDL statements -------------------------------------------------------------------------------------

/**
* ALTER STATEMENT
*/
alter_statement:
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
create_statement:
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
    
drop_statement:
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
select_statement
    : query_expression                                      // SELECT ...
    | query_expression_parens;                              // (SELECT ...)

// Expression without parantheses.
query_expression
    : query_expression_without_with                         // SELECT ...
    | with_clause query_expression_without_with;            // WITH ... SELECT ...

// Expression without with clause.
query_expression_without_with
    : (query_expression_body | query_expression_parens)     // SELECT ...
    order_by_clause?                                        // ORDER BY ...
    limit_clause?                                           // LIMIT ... (OFFSET ...)
    for_clause?;                                            // FOR ... (OF ...)

// Expression with parantheses.
query_expression_parens:
    OPEN_PAREN (
        query_expression
        | query_expression_parens
    ) CLOSE_PAREN;

// Simpler query expression.
query_expression_body
    : (query_primary | query_expression_parens) query_expression_set*;

// Query expression with set expressions(union, intersect, except).
query_expression_set
    : set_operator set_operator_option? (query_primary | query_expression_parens);

// Primary query.
query_primary:
    SELECT select_option* select_item_list
    into_clause?
    from_clause?
    where_clause?
    group_by_clause?
    having_clause?
    window_clause?
    | values_statement
    | explicit_table_statement;

// Columns of the select statement.
select_item_list:
    // TODO: Create select item
    (select_item | STAR) (COMMA select_item)*;

// Options for select statement.
select_option
    : ALL
    | DISTINCT;

/**
 * UPDATE
 */
// TODO: Implement update statement.
update_statement
    :;
    
/**
 * INSERT
 */
// TODO: Implement insert statement.
insert_statement
    :;

/**
 * DELETE
 */
// TODO: Implement delete statement.
delete_statement
    :;

/**
 * VALUES
 */
values_statement:
    // TODO: Create expression list
    VALUES OPEN_PAREN expression_list CLOSE_PAREN (COMMA OPEN_PAREN expression_list CLOSE_PAREN)*;

/**
 * TABLES (explicitly)
 */
explicit_table_statement:
    TABLE table_ref; // TODO: Create table reference identifier
    
table_ref
    :;

//----------------- CLAUSES --------------------------------------------------------------------------------------------

/**
 * ALIAS
 */
alias_clause
    : alias_clause_body
    | AS alias_clause_body;
    
alias_clause_body
    : column_name (OPEN_PAREN name_list CLOSE_PAREN)?;  // TODO: Create column name.

name_list
    : name (COMMA name)*;   // TODO: Create name.

/**
 * FOR .. (OF ..);
 */
for_clause
    // TODO: Implement table name expression.
    : FOR lock_strength_option (OF table_ref (COMMA table_ref)*)? (NOWAIT | SKIP_P LOCKED)?;

lock_strength_option
    : UPDATE
    | NO KEY UPDATE
    | SHARE
    | KEY SHARE;

/**
 * FROM
 */
from_clause
    : FROM from_item_list;

// List of from item.
from_item_list
    : from_item (COMMA from_item)*;

from_item
    : from_item_inner
    | join_from_item;

// Item that can be an element of the from clause.
from_item_inner
    : table_from_item                       // FROM schema.table
    | ONLY table_from_item                  // FROM ONLY schema.table
    // TODO: Create with query name.
    | with_query_name alias_clause?         // FROM query (AS foo)
    | function_from_item                    // FROM function()
    | subquery_from_item                    // FROM (SELECT * FROM schema.table)
    | LATERAL_P (
        function_from_item                  // FROM LATERAL function()
        | subquery_from_item                // FROM LATERAL (SELECT * FROM schema.table)
    );

// Table item.
table_from_item
    // TODO: Create table name.
    : table_name STAR? alias_clause? tablesample_clause?;

// Function item.
function_from_item
    : function_primary                                                  // function()
    | function_primary (
        alias_clause                                                    // function() AS foo
        | WITH ORDINALITY alias_clause?                                 // function() WITH ORDINALITY AS foo
        // TODO: Create alias and column definition list.
        | AS alias OPEN_PAREN column_definition_list CLOSE_PAREN        // function() AS foo (bar, baz)
    )
    | rows_from_function_primary                                        // ROWS FROM function()
    | rows_from_function_primary (
        alias_clause                                                    // ROWS FROM function() AS foo
        | WITH ORDINALITY alias_clause?                                 // ROWS FROM function() WITH ORDINALITY AS foo
    );

// Function item.
function_primary
    // TODO: Create function.
    : function OPEN_PAREN argument_list? CLOSE_PAREN;

// Function item that has a ROWS FROM block.
rows_from_function_primary
    : ROWS FROM OPEN_PAREN function_primary CLOSE_PAREN;

// Subquery item.
subquery_from_item
    : query_expression_parens alias_clause?;

// List of columns.
column_list
    // TODO: Create column.
    : column (COMMA column)*;

// List of arguments.
argument_list
    // TODO: Create argument.
    : argument (COMMA argument)*;

// Join item.
join_from_item
    : from_item_inner join (join)*;

// Join
join
    : CROSS JOIN from_item_inner
    | natural_joinable
    | NATURAL natural_joinable;

// Natural joinable joins.
natural_joinable
    : join_type? JOIN from_item_inner;
    
join_type
    : (FULL | LEFT | RIGHT | INNER_P) OUTER_P?;

/**
 * GROUP BY
 */
group_by_clause
    : GROUP_P BY (ALL | DISTINCT)? grouping_element_list;

grouping_element_list
    // TODO: Implement grouping element expression.
    : grouping_element (COMMA grouping_element)*;

/**
 * HAVING
 */
having_clause
    // TODO: Create condition expression.
    : HAVING condition_expression;

/**
 * INTO
 */
into_clause
    // TODO: Create table name
    : INTO into_clause_options? TABLE? table_name;

// Table options for into clause.
into_clause_options
    : TEMPORARY
    | TEMP
    | UNLOGGED;

/**
 * LIMIT, FETCH, OFFSET
 */
limit_clause
    : limit (offset)?
    | offset (limit)?;
 
limit
    : LIMIT limit_value
    // TODO: Create value expression.
    | FETCH (FIRST_P | NEXT) value (ROW | ROWS) (ONLY | WITH TIES);

offset
    : OFFSET (
        limit_value
        // TODO: Create value expression.
        | value (ROW | ROWS)
    );
    
limit_value
    // TODO: Create value expression.
    : value
    | ALL;

/**
 * ORDER BY
 */
order_by_clause
    : ORDER BY order_list;

order_list
    : // TODO: Implement orderExpression.
    order_expression (COMMA order_expression)*;

order_expression
    : expression (ASC | DESC)?;

/**
 * TABLESAMPLE
 */
tablesample_clause
    // TODO: Create sampling_method and seed.
    : TABLESAMPLE sampling_method OPEN_PAREN argument_list CLOSE_PAREN (REPEATABLE OPEN_PAREN seed CLOSE_PAREN)?;

/**
 * WHERE
 */
where_clause
    // TODO: Create condition_expression.
    : WHERE condition_expression;

/**
 * WINDOW
 */
window_clause
    : WINDOW window_definition_list;
    
window_definition_list
    : window_definition (COMMA window_definition)*;

// NOTE: Same structure as the OVER clause.    
window_definition
    : column AS window_specification;

window_specification
    : OPEN_PAREN (
        window_name?
        (PARTITION BY expression_list)?
        (ORDER BY window_order_by_expression_list)?
        frame_clause?
    ) CLOSE_PAREN;

window_order_by_expression_list
    : window_order_by_expression (COMMA window_order_by_expression)*;

window_order_by_expression
    : expression (ASC | DESC | USING operator) (NULLS_P (FIRST_P | LAST_P))?;

frame_clause
    : (RANGE | ROWS | GROUPS) (
        frame_bound
        | BETWEEN frame_bound AND frame_bound
    ) (frame_exclusion)?;
    
frame_bound
    : UNBOUNDED frame_bound_option
    | offset frame_bound_option
    | CURRENT_P ROW;
    
frame_bound_option
    : PRECEDING
    | FOLLOWING;

frame_exclusion
    : EXCLUDE (CURRENT_P ROW | GROUP_P | TIES | NO OTHERS);

/**
 * WITH
 */
with_clause
    : WITH RECURSIVE? common_table_expression (COMMA common_table_expression)*;

// CTE(Common Table Expression).
common_table_expression
    // TODO: Create identifier.
    : identifier OPEN_PAREN name_list CLOSE_PAREN 
    AS common_table_expression_option 
    OPEN_PAREN common_table_expression_statements CLOSE_PAREN;

common_table_expression_option
    : MATERIALIZED
    | NOT MATERIALIZED;

common_table_expression_statements
    : select_statement
    | update_statement
    | insert_statement
    | delete_statement;

//----------------- OPERATORS ------------------------------------------------------------------------------------------

set_operator
    : UNION
    | INTERSECT
    | EXCEPT;

set_operator_option
    : ALL
    | DISTINCT;
    
//----------------- TEMPORARY NODES ------------------------------------------------------------------------------------
// Nodes that are not implemented yet, but required to implement other nodes.

identifier
    : TEMP;

window_name
    : TEMP;

expression_list
    : TEMP;

expression
    : TEMP;

operator
    : TEMP;

with_query_name
    : TEMP;

sampling_method
    : TEMP;

seed
    : TEMP;

table_name
    : TEMP;

alias
    : TEMP;

column_definition_list
    : TEMP;
    
column_name
    : TEMP;

name
    : TEMP;

function
    : TEMP;

column
    : TEMP;

argument
    : TEMP;

grouping_element
    : TEMP;
    
condition_expression
    : TEMP;

value
    : TEMP;
    
select_item
    : TEMP;