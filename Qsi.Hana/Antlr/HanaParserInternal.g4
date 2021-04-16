parser grammar HanaParserInternal;

options { 
    tokenVocab=HanaLexerInternal;
}

root
    : EOF
    | (hanaStatement (SEMI EOF? | EOF))+
    ;

hanaStatement
    : selectStatement
    ;

selectStatement
    : withClause? subquery /*(forUpdate | K_FORSHARELOCK | timeTravel | forSystemTime)?*/ hintClause?
    | withClause? '(' subquery ')' /*(forUpdate | forJsonClause | forXmlClause | timeTravel)?*/ hintClause?
    /*| (subquery | '(' subquery ')') K_INTO (tableRef | variableNameList) ('(' columnNameList ')')? hintClause? K_TOTALROWCOUNT?*/
    ;

subquery
    : '(' selectStatement ')'
    | selectClause fromClause
      /* 
      whereClause?
      groupByClause?
      havingClause?
      (setOperator subquery (',' subquery)*)?
      (orderByClause limitClause)?
      */
    ;

withClause
    : K_WITH withListElement (',' withListElement)*
    ;

withListElement
    : queryName columnListClause? K_AS '(' subquery ')'
    ;

columnListClause
    : '(' columnName (',' columnName)* ')'
    ;

queryName
    : IDENTIFIER
    ;

selectClause
   : K_SELECT (K_TOP UNSIGNED_INTEGER)? (K_ALL | K_DISTINCT)? selectList
   ;

selectList
    : selectItem (',' selectItem)*
    ;

selectItem
    : columnName/*(columnName | EXPRESSION | ASSOCIATION_EXPRESSION)*/ (K_AS? alias)? | (tableName '.')? '*'
    ;

columnName
    : IDENTIFIER
    ;

fromClause
    : K_FROM tableExpression (',' tableExpression)*
    ;

tableExpression
    : tableRef
//    | systemVersionedTableRef 
    | subquery
//    | joinedTable 
//    | lateralTableExpression 
//    | collectionDerivedTable 
//    | functionReference 
//    | jSONCollectionTable 
//    | ':' tableVariable 
//    | associatedTableExpression
    ;

tableRef
    : tableName /*(forSystemTime | forApplicationTimePeriod)? partitionRestriction?*/ (K_AS? alias)? /*tablesampleClause?*/
    ;

hintClause
    : K_WITHHINT '(' hintElement (',' hintElement*)? ')'
    ;

hintElement
    : (hint | hintWithParameters)
    ;

hint
    : SIMPLE_IDENTIFIER
    ;

hintWithParameters
    : K_ROUTE_TO '(' volumeId (',' volumeId)? ')'
    | K_NO_ROUTE_TO '(' volumeId (',' volumeId)? ')'
    | K_ROUTE_BY '(' tableName (',' tableName)? ')'
    | K_ROUTE_BY_CARDINALITY '(' tableName (',' tableName)? ')'
    | K_DATA_TRANSFER_COST '(' UNSIGNED_INTEGER ')'
    ;

tableName
    : ((db=IDENTIFIER '.')? schema=SCHEMA_NAME '.')? table=IDENTIFIER
    ;

volumeId
    : IDENTIFIER
    ;

alias
    : IDENTIFIER
    ;