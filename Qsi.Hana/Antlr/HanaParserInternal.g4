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

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > SELECT Statement ------

selectStatement
    : withClause? subquery (forUpdate | K_FOR K_SHARE K_LOCK | timeTravel | forSystemTime)? hintClause?
    | withClause? subquery (forUpdate | forJsonOrXmlClause | timeTravel)? hintClause?
    | subquery K_INTO (tableRef | variableNameList) columnListClause? hintClause? (K_TOTAL K_ROWCOUNT)?
    ;

subquery
    : '(' selectStatement ')' alias?
    | selectClause fromClause
      whereClause?
      groupByClause?
      havingClause?
      setOperatorClause?
      orderByClause?
      limitClause?
    | '(' inner=subquery ')'
    ;

withClause
    : K_WITH withListElement (',' withListElement)*
    ;

withListElement
    : name=identifier columnListClause? K_AS '(' subquery ')'
    ;

columnList
    : columns+=columnName (',' columns+=columnName)*
    ;

columnListClause
    : '(' list=columnList ')'
    ;

forUpdate
    : K_FOR K_UPDATE (K_OF columnListClause)? waitNowait? (K_IGNORE K_LOCKED)?
    ;

waitNowait
    : K_WAIT UNSIGNED_INTEGER
    | K_NOWAIT
    ;

forJsonOrXmlClause
    : K_FOR (K_JSON | K_XML) ('(' options+=forJsonOrXmlOption (',' options+=forJsonOrXmlOption)* ')')? forJsonOrXmlReturnsClause?
    ;

forJsonOrXmlOption
    : key=STRING_LITERAL '=' value=STRING_LITERAL
    ;

forJsonOrXmlReturnsClause
    : K_RETURNS (
        K_VARCHAR '(' numericLiteral ')'
        | K_NVARCHAR '(' numericLiteral ')'
        | K_CLOB
        | K_NCLOB
     )
    ;

timeTravel
    : UNSIGNED_INTEGER              #commtId
    | K_UTCTIMESTAMP STRING_LITERAL #timestamp
    ;

forSystemTime
    : K_FOR K_SYSTEM_TIME K_AS K_OF STRING_LITERAL                      #asOf
    | K_FOR K_SYSTEM_TIME K_FROM STRING_LITERAL K_TO STRING_LITERAL     #fromTo
    | K_FOR K_SYSTEM_TIME K_BETWEEN STRING_LITERAL K_AND STRING_LITERAL #between
    ;

selectClause
   : K_SELECT (K_TOP UNSIGNED_INTEGER)? (K_ALL | K_DISTINCT)? selectList
   ;

selectList
    : selectItem (',' selectItem)*
    ;

selectItem
    : expression alias?             #expr 
    | associationExpression alias?  #associationExpr
    | (tableName '.')? '*'          #wildcard
    ;

columnName
    : identifier
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
    : tableName (forSystemTime | forApplicationTimePeriod)? partitionRestriction? alias? tableSampleClause?
    ;

forApplicationTimePeriod
    : K_FOR K_APPLICATION_TIME K_AS K_OF '\'' UNSIGNED_INTEGER '\''
    ;

partitionRestriction
    : K_PARTITION '(' numbers+=UNSIGNED_INTEGER (',' numbers+=UNSIGNED_INTEGER)* ')'
    ;

tableSampleClause
    : K_TABLESAMPLE (K_BERNOULLI | K_SYSTEM)? '(' size=numericLiteral ')'
    ;

hintClause
    : K_WITH K_HINT '(' hints+=hintElement (',' hints+=hintElement)* ')'
    ;

hintElement
    : name=UNQUOTED_IDENTIFIER                                                                      #hintName
    | K_ROUTE_TO             '(' volumeIds+=UNSIGNED_INTEGER (',' volumeIds+=UNSIGNED_INTEGER)? ')' #routeTo
    | K_NO_ROUTE_TO          '(' volumeIds+=UNSIGNED_INTEGER (',' volumeIds+=UNSIGNED_INTEGER)? ')' #noRouteTo
    | K_ROUTE_BY             '(' tables+=tableName (',' tables+=tableName)* ')'                     #routeBy
    | K_ROUTE_BY_CARDINALITY '(' tables+=tableName (',' tables+=tableName)* ')'                     #routeByCardinality
    | K_DATA_TRANSFER_COST   '(' cost=UNSIGNED_INTEGER ')'                                          #rdataTransferCost
    ;

tableName
    : ((db=identifier '.')? schema=identifier '.')? table=identifier
    ;

alias
    : K_AS? name=identifier
    ;

// TODO: <association_expression> ::= <association_ref>[.<association_ref>[...] ]
associationExpression
    : refs+=associationRef ('.' refs+=associationRef*)?
    ;

associationRef
    : columnName '[' (condition associationCardinality?)? ']'
    ;

associationCardinality
    : K_USING (K_ONE | K_MANY) K_TO (K_ONE | K_MANY) K_JOIN
    ;

variableName
    : identifier ('[' index=numericLiteral ']')?
    ;

variableNameList
    : variableName (',' variableName)*
    ;

whereClause
    : K_WHERE condition
    ;

groupByClause
    : K_GROUP K_BY groupByExpressionList
    ;

groupByExpressionList
    : (tableExpression | groupingSet) (',' tableExpression | groupingSet)*
    ;

groupingSet
    : (K_GROUPING K_SETS | K_ROLLUP | K_CUBE)
      (K_BEST best=numericLiteral)?
      (K_LIMIT limit=UNSIGNED_INTEGER (K_OFFSET offset=UNSIGNED_INTEGER)?)?
      (K_WITH K_SUBTOTAL)?
      (K_WITH K_BALANCE)?
      (K_WITH K_TOTAL)?
      (K_TEXT_FILTER filter=STRING_LITERAL (K_FILL K_UP (K_SORT K_MATCHES K_TO K_TOP)?)?)?
      (
        K_STRUCTURED K_RESULT (K_WITH K_OVERVIEW)? (K_PREFIX prefixTableName)?
        | K_MULTIPLE K_RESULTSETS
      )?
      '(' groupingExpressionList ')'
    ;

groupingExpressionList
    : groupingExpression (',' groupingExpression)*
    ;

groupingExpression
    : tables+=tableExpression
    | '(' tables+=tableExpression (',' tables+=tableExpression)* ')'
    | '(' '(' tables+=tableExpression (',' tables+=tableExpression)* ')' orderByClause ')'
    ;

prefixTableName
    : '#' identifier
    ;

orderByClause
    : K_ORDER K_BY orderByExpression (',' orderByExpression)*
    ;

orderByExpression
    : (table=tableExpression | position=UNSIGNED_INTEGER) collateClause? (K_ASC | K_DESC)? (K_NULLS K_FIRST | K_NULLS K_LAST)?
    ;

collateClause
    : K_COLLATE name=UNICODE_IDENTIFIER
    ;

havingClause
    : K_HAVING condition
    ;

setOperator
    : K_UNION (K_ALL | K_DISTINCT)?
    | K_INTERSECT K_DISTINCT?
    | K_EXCEPT K_DISTINCT?
    ;

setOperatorClause
    : setOperator subquery (',' setOperator subquery)*
    ;

limitClause
    : K_LIMIT limit=UNSIGNED_INTEGER (K_OFFSET offset=UNSIGNED_INTEGER)? (K_TOTAL K_ROWCOUNT)?
    ;

// ------ SQL Reference > Operators ------

operator
    : arithmeticOperator
    | comparisonOperator
    | concatenationOperator
    | logicalOperator
    ;

arithmeticOperator
    : '+' | '-' | '*' | '/'
    ;

comparisonOperator
    : '=' | '>' | '<' | '>=' | '<=' | '!=' | '<>'
    ;

concatenationOperator
    : '||'
    ;

logicalOperator
    : K_AND | K_OR | K_NOT
    ;

// ------ SQL Reference > Expressions ------

expression
    : caseExpression                        #caseExpr
    | functionExpression                    #functionExpr
    | aggregateExpression                   #aggExpr
    | '(' expression ')'                    #parenthesisExpr
    | subquery                              #subqueryExpr
    | '-' expression                        #unaryExpr
    | l=expression op=operator r=expression #operationExpr
    | (t=tableName '.')? c=columnName       #fieldExpr
    | constant                              #constantExpr
    | jsonObjectExpression                  #jsonObjectExpr
    | jsonArrayExpression                   #jsonArrayExpr
//    | variableName
    ;

expressionList
    : (items+=expression)+
    ;

expressionListWithComma
    : expression (',' expression)*
    ;

caseExpression
    : simpleCaseExpression
    | searchCaseExpression
    ;

simpleCaseExpression
    : K_CASE expression
          (K_WHEN when+=expression K_THEN then+=expression)+
          (K_ELSE else=expression)?
      K_END
    ;

searchCaseExpression
    : K_CASE 
          (K_WHEN when+=condition K_THEN then+=expression)+
          (K_ELSE else=expression)?
      K_END
    ;

condition
    : predicate                 #predicateCondition
    | condition K_OR condition  #orCondition
    | condition K_AND condition #andCondition
    | K_NOT condition           #notCondition
    | '(' condition ')'         #parenthesisCondition
    ;

functionExpression
    : functionName '(' expressionListWithComma ')'
    ;

functionName
    : (identifier '.')? identifier
    ;

aggregateExpression
    : K_COUNT '(' '*' ')'                                                                      #countAggExpr
    | K_COUNT '(' K_DISTINCT expressionList ')'                                                #countDistinctAggExpr
    | aggName '(' (K_ALL | K_DISTINCT)? expression ')'                                         #funcAggExpr
    | K_STRING_AGG '(' expression (',' delimiter=STRING_LITERAL)? aggregateOrderByClause? ')'  #stringAggExpr
    ;

aggName
    : K_CORR
    | K_CORR_SPEARMAN
    | K_COUNT
    | K_MIN
    | K_MEDIAN
    | K_MAX
    | K_SUM
    | K_AVG
    | K_STDDEV
    | K_VAR
    | K_STDDEV_POP
    | K_VAR_POP
    | K_STDDEV_SAMP
    | K_VAR_SAMP
    ;

aggregateOrderByClause
    : K_ORDER K_BY expression (K_ASC | K_DESC)? (K_NULLS K_FIRST | K_NULLS K_LAST)?
    ;

constant
    : STRING_LITERAL  #string
    | numericLiteral  #number
    | booleanLiteral  #boolean
    | K_NULL          #null
    ;

jsonObjectExpression
    : '{' key=QUOTED_IDENTIFIER ':' value=jsonValueExpression '}'
    ;

jsonValueExpression
    : STRING_LITERAL
    | numericLiteral
    | booleanLiteral
    | K_NULL
//    | <path_expression>
    | jsonObjectExpression
    | jsonArrayExpression
    ;

jsonArrayExpression
    : '[' ']'                                                                        #empty
    | '[' items+=jsonArrayValueExpression (',' items+=jsonArrayValueExpression)* ']' #noEmpty
    ;

jsonArrayValueExpression
    : STRING_LITERAL
    | numericLiteral
    | booleanLiteral
    | K_NULL
//    | <path_expression>
    | jsonObjectExpression
    ;

// ------ SQL Reference > Predicates ------

predicate
    : comparisonPredicate
    | betweenPredicate
    | containsPredicate
    | inPredicate
    | likePredicate
    | existsPredicate
    | likeRegexPredicate
    | memberOfPredicate
    | nullPredicate
    ;

comparisonPredicate
    : left=expression op=comparisonOperator (K_ANY | K_SOME | K_ALL)? '(' (right1=expressionListWithComma | right2=subquery) ')'
    | left=expression op=comparisonOperator right=expression
    ;

betweenPredicate
    : source=expression K_NOT? K_BETWEEN lower=expression K_AND upper=expression
    ;

containsPredicate
    : K_CONTAINS '(' columns=containsColumns ',' search=STRING_LITERAL (',' specifier=searchSpecifier)? ')'
    ;

containsColumns
    : '*'
    | columnList
    | columnListClause
    ;

searchSpecifier
    : searchType? optSearchSpecifier2List?
    | searchSpecifier2List
    ;

optSearchSpecifier2List
    : '(' K_EMPTY ',' K_NOTHING K_SPECIFIED ')'
    | searchSpecifier2List
    ;

searchType
    : exactSearch
    | fuzzySearch
    | linguisticSearch
    ;

searchSpecifier2List
    : searchSpecifier2
    | searchSpecifier2List ',' searchSpecifier2
    ;

searchSpecifier2
    : weights
    | language
    | fulltext
    ;

fulltext
    : K_FULLTEXT '(' param=(K_ON | K_OFF | K_AUTOMATIC) ')'
    ;

exactSearch
    : K_EXACT ('(' param=STRING_LITERAL ')')?
    ;

fuzzySearch
    : K_FUZZY ('(' (fuzzyParams | fuzzyParamsList) ')')?
    ;

fuzzyParamsList
    : '(' fuzzyParams ')' ',' fuzzyParamsList2
    ;

fuzzyParamsList2
    : '(' fuzzyParams ')'
    | fuzzyParamsList2 ',' '(' fuzzyParams ')'
    ;

fuzzyParams
    : numericLiteral (',' STRING_LITERAL)?
    | K_NULL ',' STRING_LITERAL
    ;

linguisticSearch
    : K_LINGUISTIC '(' param=STRING_LITERAL ')'
    ;

weights
    : K_WEIGHT '(' param=numericLiteral ')'
    ;

language
    : K_LANGUAGE '(' param=STRING_LITERAL ')'
    ;

existsPredicate
    : K_NOT? K_EXISTS '(' subquery ')'
    ;

inPredicate
    : source=expression K_NOT? K_IN (expressionListWithComma | subquery)
    ;

likePredicate
    : source=expression K_NOT? K_LIKE value=expression (K_ESCAPE escape=expression)?
    ;

likeRegexPredicate
    : source=expression K_LIKE_REGEXPR pattern=STRING_LITERAL (K_FLAG {IsRegexFlag()}? flag=.)?
    ;

memberOfPredicate
    : source=expression K_NOT? K_MEMBER K_OF member=expression
    ;

nullPredicate
    : source=expression K_IS K_NOT? K_NULL
    ;

// ------ ETC ------

booleanLiteral
    : K_TRUE
    | K_FALSE
    ;

numericLiteral
    : ('+' | '-')? (EXACT_NUMERIC_LITERAL | APPROXIMATE_NUMERIC_LITERAL)
    | SIGNED_INTEGER
    | UNSIGNED_INTEGER
    ;

identifier
    : UNQUOTED_IDENTIFIER
    | QUOTED_IDENTIFIER
    | UNICODE_IDENTIFIER
    | keywodIdentifier
    ;

keywodIdentifier
    : K_AND | K_ANY | K_APPLICATION_TIME | K_ASC | K_AUTOMATIC | K_AVG | K_BALANCE | K_BERNOULLI | K_BEST | K_BETWEEN
    | K_BY | K_CLOB | K_COLLATE | K_CONTAINS | K_CORR | K_CORR_SPEARMAN | K_COUNT | K_DATA_TRANSFER_COST | K_DESC
    | K_EMPTY | K_ESCAPE | K_EXACT | K_EXISTS | K_FILL | K_FIRST | K_FLAG | K_FULLTEXT | K_FUZZY | K_GROUPING | K_HINT
    | K_IGNORE | K_JSON | K_LANGUAGE | K_LAST | K_LIKE | K_LIKE_REGEXPR | K_LINGUISTIC | K_LOCK | K_LOCKED | K_MANY
    | K_MATCHES | K_MAX | K_MEDIAN | K_MEMBER | K_MIN | K_MULTIPLE | K_NCLOB | K_NO_ROUTE_TO | K_NOT | K_NOTHING
    | K_NOWAIT | K_NULLS | K_NVARCHAR | K_OF | K_OFF | K_OFFSET | K_ONE | K_OR | K_OVERVIEW | K_PARTITION | K_PREFIX
    | K_RESULT | K_RESULTSETS | K_ROUTE_BY | K_ROUTE_BY_CARDINALITY | K_ROUTE_TO | K_ROWCOUNT | K_SETS | K_SHARE
    | K_SOME | K_SORT | K_SPECIFIED | K_STDDEV | K_STDDEV_POP | K_STDDEV_SAMP | K_STRING_AGG | K_STRUCTURED | K_SUBTOTAL
    | K_SUM | K_SYSTEM | K_SYSTEM_TIME | K_TEXT_FILTER | K_THEN | K_TO | K_TOTAL | K_UP | K_UPDATE | K_VAR | K_VAR_POP
    | K_VAR_SAMP | K_VARCHAR | K_WAIT | K_WEIGHT | K_XML
    ;
