parser grammar HanaParserInternal;

options { 
    tokenVocab=HanaLexerInternal;
}

root
    : EOF
    | (hanaStatement (SEMI EOF? | EOF))+
    ;

hanaStatement
    : dataManipulationStatement
    ;

dataManipulationStatement
    : deleteStatement
    // | explainPlanStatement
    | insertStatement
    // | loadStatement
    | mergeDeltaStatement
    | mergeIntoStatement
    | replaceStatement
    | selectStatement
    // | truncateTableStatement
    // | unloadStatement
    | updateStatement
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > SELECT Statement ------

selectStatement
    : withClause? subquery (forUpdate | K_FOR K_SHARE K_LOCK | timeTravel | forSystemTime)? hintClause?
    | withClause? subquery (forUpdate | forJsonOrXmlClause | timeTravel)? hintClause?
    | subquery K_INTO (tableRef | variableNameList) columnListClause? hintClause? (K_TOTAL K_ROWCOUNT)?
    ;

subquery
    : '(' selectStatement ')'
    | selectClause
      fromClause
      whereClause?
      groupByClause?
      havingClause?
      setOperatorClause?
      tableOrderByClause?
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
   : K_SELECT topClause? (K_ALL | K_DISTINCT)? selectList
   ;

topClause
    : K_TOP top=UNSIGNED_INTEGER
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

seriesTable
    : K_SERIES K_TABLE tableName
    ;

tableExpression
    : tableRef (K_CROSS K_JOIN crossJoin=tableRef)?
//    | systemVersionedTableRef
    | subqueryTableExpression
    | tableExpression joinType? joinCardinality? K_JOIN tableExpression K_ON predicate
    | caseJoin
    | lateralTableExpression
    | collectionDerivedTable
    | tableFunctionExpression
    | variableTable
    | associationTableExpression
    ;

subqueryTableExpression
    : subquery alias?
    ;

tableFunctionExpression
    : (seriesExpression | functionExpression) alias?
    ;

tableRef
    : tableName (forSystemTime | forApplicationTimePeriod)? partitionRestriction? alias? tableSampleClause?
    ;

forApplicationTimePeriod
    : K_FOR K_APPLICATION_TIME K_AS K_OF { IsQuotedNumeric() }? STRING_LITERAL
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

fieldName
    : db=identifier '.' schema=identifier '.' table=identifier '.' column=identifier
    | schema=identifier '.' table=identifier '.' column=identifier
    | table=identifier '.' column=identifier
    | column=identifier
    ;

tableName
    : db=identifier '.' schema=identifier '.' table=identifier
    | schema=identifier '.' table=identifier
    | table=identifier
    ;

alias
    : K_AS? name=identifier
    ;

explicitAlias
    : K_AS name=identifier
    ;

associationTableExpression
    : tableName ('[' condition ']')? ':' associationExpression alias?
    ;

associationExpression
    : refs+=associationRef ('.' refs+=associationRef)*
    ;

associationRef
    : columnName ('[' (condition associationCardinality?)? ']')?
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
    | '(' '(' tables+=tableExpression (',' tables+=tableExpression)* ')' tableOrderByClause ')'
    ;

prefixTableName
    : '#' identifier
    ;

variableTable
    : ':' identifier alias?
    ;

tableOrderByClause
    : K_ORDER K_BY tableOrderByExpression (',' tableOrderByExpression)*
    ;

tableOrderByExpression
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

joinCardinality
    : K_MANY K_TO K_MANY               #manyToMany
    | K_MANY K_TO K_ONE                #manyToOne
    | K_MANY K_TO K_EXACT K_ONE        #manyToExactOne
    | K_ONE K_TO K_MANY                #oneToMany
    | K_EXACT K_ONE K_TO K_MANY        #exactOneToMany
    | K_ONE K_TO K_ONE                 #oneToOne
    | K_EXACT K_ONE K_TO K_ONE         #exactOneToOne
    | K_ONE K_TO K_EXACT K_ONE         #oneToExactOne
    | K_EXACT K_ONE K_TO K_EXACT K_ONE #exactOneToExactOne
    ;

joinType
    : K_INNER
    | (K_LEFT | K_RIGHT | K_FULL) K_OUTER?
    ;

caseJoin
    : tableRef K_LEFT K_OUTER K_MANY K_TO K_ONE
        K_CASE K_JOIN
            (K_WHEN condition K_THEN K_RETURN columnListClause K_FROM tableRef K_ON predicate)+
            (K_ELSE K_RETURN columnListClause K_FROM tableRef K_ON predicate)?
        K_END alias?
    ;

lateralTableExpression
    : K_LATERAL '(' (subquery | functionExpression) ')' alias?
    ;

collectionDerivedTable
    : K_UNNEST '(' collectionValueExpression (',' collectionValueExpression)* ')' (K_WITH K_ORDINALITY)? explicitAlias columnListClause?
    ;

collectionValueExpression
    : K_ARRAY '(' (tableExpression (',' tableExpression)* | columnName) ')'
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > DELETE Statement ------

deleteStatement
    : K_DELETE K_HISTORY? K_FROM tableName partitionRestriction? whereClause? hintClause?
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > INESRT Statement ------

insertStatement
    : K_INSERT K_INTO tableName partitionRestriction? alias?
      columnListClause?
      (
        (valueListClause | overridingClause? subquery)
        | selectStatement
      )
      hintClause?
    ;

valueListClause
    : K_VALUES '(' expressionList ')' explicitAlias?
    ;

overridingClause
    : K_OVERRIDING (K_SYSTEM | K_USER) K_VALUE
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > UPDATE Statement ------

updateStatement
    : K_UPDATE topClause? tableName alias?
      portionOfApplicationTimeClause?
      partitionRestriction?
      setClause
      fromClause?
      whereClause?
      hintClause?
    ;

portionOfApplicationTimeClause
    : K_FOR K_PORTION K_OF K_APPLICATION_TIME K_FROM from=STRING_LITERAL K_TO to=STRING_LITERAL
    ;

setClause
    : K_SET elements+=setElement (',' elements+=setElement)*
    ;

setElement
    : fieldName '=' expression
    | '(' withClause subquery ')' // TODO: check to real db
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > REPLACE | UPSERT Statement ------

replaceStatement
    : (K_UPSERT | K_REPLACE) tableName partitionRestriction? columnListClause?
      (
        valueListClause (whereClause | K_WITH K_PRIMARY K_KEY)?
        | subquery
      )
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > MERGE DELTA Statement ------

mergeDeltaStatement
    : K_MERGE K_HISTORY? K_DELTA K_OF tableName (K_PART part=UNSIGNED_INTEGER)?
      (K_WITH K_PARAMETERS '(' params+=mergeDeltaParameter (',' params+=mergeDeltaParameter)* ')')?
      (K_FORCE K_REBUILD)?
    ;

mergeDeltaParameter
    : key=STRING_LITERAL '=' value=STRING_LITERAL
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > MERGE INTO Statement ------

mergeIntoStatement
    : K_MERGE K_INTO tableName partitionRestriction? alias?
      K_USING tableRef
      K_ON condition
      (operations+=mergeWhenClause)+
    ;

mergeWhenClause
    : K_WHEN K_MATCHED (K_AND condition)? K_THEN whenMatchedSpecification          #mergeWhenMatched
    | K_WHEN K_NOT K_MATCHED (K_AND condition)? K_THEN whenNotMatchedSpecification #mergeWhenNotMatched
    ;

whenMatchedSpecification
    : K_UPDATE setClause  #whenMatchedUpdate
    | K_DELETE            #whenMatchedDelete
    ;

whenNotMatchedSpecification
    : K_INSERT columnListClause? valueListClause
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
    | windowExpression                      #windowExpr
    | aggregateExpression                   #aggExpr
    | dataTypeConversionExpression          #conversionExpr
    | datetimeExpression                    #datetimeExpr
    | functionExpression                    #functionExpr
    | '(' expression ')'                    #parenthesisExpr
    | subquery                              #subqueryExpr
    | '-' expression                        #unaryExpr
    | l=expression op=operator r=expression #operationExpr
    | fieldName                             #fieldExpr
    | constant                              #constantExpr
    | identifier '=>' expression            #lambdaExpr
    | jsonObjectExpression                  #jsonObjectExpr
    | jsonArrayExpression                   #jsonArrayExpr
//    | variableName
    ;

expressionList
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
    | K_CURRENT K_OF identifier #currentOfCondition
    ;

functionExpression
    : jsonExpression                      #jsonExpr
    | stringExpression                    #stringExpr
    | functionName '(' expressionList ')' #scalarExpr
    ;

functionName
    : ((db=identifier '.')? schema=identifier '.')? function=identifier
    ;

aggregateExpression
    : K_COUNT '(' '*' ')'                                                                      #aggCountExpr
    | K_COUNT '(' K_DISTINCT expressionList ')'                                                #aggCountDistinctExpr
    | K_STRING_AGG '(' expression (',' delimiter=STRING_LITERAL)? aggregateOrderByClause? ')'  #aggStringExpr
    | K_CROSS_CORR '('
        expression ',' 
        expression ','
        UNSIGNED_INTEGER (seriesOrderBy | aggregateOrderByClause) 
      ')' ('.' (K_POSITIVE_LAGS | K_NEGATIVE_LAGS | K_ZERO_LAG))?                              #aggCrossCorrExpr
    | K_DFT '('
        expression ','
        UNSIGNED_INTEGER (seriesOrderBy | aggregateOrderByClause)
      ')' '.' (K_REAL | K_IMAGINARY | K_AMPLITUDE | K_PHASE)                                   #aggDftExpr
    | aggName '(' (K_ALL | K_DISTINCT)? expression ')'                                         #aggFuncExpr
    ;

aggName
    : K_VAR
    | K_VAR_POP
    | K_VAR_SAMP
    | K_STDDEV
    | K_STDDEV_POP
    | K_STDDEV_SAMP
    | K_STRING_AGG
    | K_NTH_VALUE
    | K_MIN
    | K_MAX
    | K_MEDIAN
    | K_LAST_VALUE
    | K_FIRST_VALUE
    | K_COUNT
    | K_CORR
    | K_CORR_SPEARMAN
    | K_AUTO_CORR
    | K_AVG
    | K_SUM
    ;

aggregateOrderByClause
    : K_ORDER K_BY expression (K_ASC | K_DESC)? (K_NULLS K_FIRST | K_NULLS K_LAST)? collateClause?
    ;

windowSpecification
    : K_OVER '(' windowPartitionByClause? windowOrderByClause? windowFrameClause? ')'
    ;

windowWithSeriesSpecification
    : K_OVER '(' seriesSepcification? windowPartitionByClause? windowOrderByClause? ')'
    ;

seriesSepcification
    : seriesTable
    | seriesClause /* TODO: == SERIES ( .. ) right? */
    ;

windowPartitionByClause
    : K_PARTITION K_BY expression (',' expression)*
    ;

windowOrderByClause
    : K_ORDER K_BY windowOrderByExpression (',' windowOrderByExpression)*
    ;

windowOrderByExpression
    : columnName (K_ASC | K_DESC)? (K_NULLS (K_FIRST | K_LAST))? collateClause?
    ;

windowFrameClause
    : K_ROWS (windowFrameStart | windowFrameBetween)
    ;

windowFrameStart
    : K_UNBOUNDED K_PRECEDING
    | UNSIGNED_INTEGER K_PRECEDING
    | K_CURRENT K_ROW
    ;

windowFrameBetween
    : K_BETWEEN lower=windowFrameBound K_AND upper=windowFrameBound
    ;

windowFrameBound
    : windowFrameStart
    | K_UNBOUNDED K_FOLLOWING
    | UNSIGNED_INTEGER K_FOLLOWING
    ;

windowExpression
    : K_BINNING             '(' expressionList ')' windowSpecification               #windowBinningExpr
    | K_CUBIC_SPLINE_APPROX '(' expressionList ')' windowWithSeriesSpecification?    #windowCubicSplineApproxExpr
    | K_CUME_DIST           '(' ')' windowSpecification                              #windowCumeDistExpr
    | K_DENSE_RANK          '(' ')' windowSpecification                              #windowDenseRankExpr
    | K_LAG                 '(' expressionList ')' windowSpecification               #windowLagExpr
    | K_LEAD                '(' expressionList ')' windowSpecification               #windowLeadExpr
    | K_LINEAR_APPROX       '(' expressionList ')' windowWithSeriesSpecification     #windowLinearApproxExpr
    | K_NTILE               '(' UNSIGNED_INTEGER ')' windowSpecification             #windowNtileExpr
    | K_PERCENT_RANK        '(' ')' windowSpecification                              #windowPercentRankExpr
    | K_PERCENTILE_CONT     '(' expression ')' withinGroupClause windowSpecification #windowPercentileContExpr
    | K_PERCENTILE_DISC     '(' expression ')' withinGroupClause windowSpecification #windowPercentileDiscExpr
    | K_RANDOM_PARTITION    '(' expressionList ')' windowSpecification               #windowRandomPartitionExpr
    | K_RANK                '(' ')' windowSpecification                              #windowRankExpr
    | K_ROW_NUMBER          '(' ')' windowSpecification                              #windowRowNumberExpr
    | K_SERIES_FILTER       '(' expressionList ')' windowWithSeriesSpecification     #windowSeriesFilterExpr
    | K_WEIGHTED_AVG        '(' expression ')' windowSpecification                   #windowWeightedAvgExpr
    | aggregateExpression windowSpecification                                        #windowAggExpr
    | seriesExpression windowSpecification?                                          #windowSeriesExpr
    ;

withinGroupClause
    : K_WITHIN K_GROUP '(' aggregateOrderByClause ')'
    ;

seriesOrderBy
    : K_SERIES '(' seriesPeriod seriesEquidistantDefinition ')'
    ;

seriesExpression
    : seriesDisaggregate
    | seriesElementToPeriod
    | seriesGenerate
    | seriesPeriodToElement
    | seriesRound
    ;

seriesDisaggregate
    : K_SERIES_DISAGGREGATE '(' (seriesTable | expression) ',' (seriesTable | expression) (',' expression (',' expression)?)? ')'
    | (
        K_SERIES_DISAGGREGATE_TINYINT
        | K_SERIES_DISAGGREGATE_SMALLINT
        | K_SERIES_DISAGGREGATE_INTEGER
        | K_SERIES_DISAGGREGATE_BIGINT
        | K_SERIES_DISAGGREGATE_SMALLDECIMAL
        | K_SERIES_DISAGGREGATE_DECIMAL
        | K_SERIES_DISAGGREGATE_TIME
        | K_SERIES_DISAGGREGATE_DATE
        | K_SERIES_DISAGGREGATE_SECONDDATE
        | K_SERIES_DISAGGREGATE_TIMESTAMP
      ) '(' expressionList ')'
    ;

seriesElementToPeriod
    : K_SERIES_ELEMENT_TO_PERIOD '(' UNSIGNED_INTEGER ',' (expression ',' expression ',' expression | seriesTable) ')'
    ;

seriesGenerate
    : K_SERIES_GENERATE '(' seriesTable (',' expression (',' expression)?)? ')'
    | (
        K_SERIES_GENERATE_TINYINT
        | K_SERIES_GENERATE_SMALLINT
        | K_SERIES_GENERATE_INTEGER
        | K_SERIES_GENERATE_BIGINT
        | K_SERIES_GENERATE_SMALLDECIMAL
        | K_SERIES_GENERATE_DECIMAL
        | K_SERIES_GENERATE_TIME
        | K_SERIES_GENERATE_DATE
        | K_SERIES_GENERATE_SECONDDATE
        | K_SERIES_GENERATE_TIMESTAMP
    ) '(' expressionList ')'
    ;

seriesPeriodToElement
    : K_SERIES_PERIOD_TO_ELEMENT '(' 
        expression ',' (expression ',' expression ',' expression | seriesTable)
        (',' roundingMode)?
     ')'
    ;

seriesRound
    : K_SERIES_ROUND '(' expression ',' (expression | seriesTable) (',' roundingMode (',' expression)?)? ')'
    ;

roundingMode
    : K_ROUND_HALF_UP
    | K_ROUND_HALF_DOWN
    | K_ROUND_HALF_EVEN
    | K_ROUND_UP
    | K_ROUND_DOWN
    | K_ROUND_CEILING
    | K_ROUND_FLOOR
    ;

dataTypeConversionExpression
    : K_CAST '(' expression K_AS dataType ')'
    ;

dataType
    // Numeric types
    : K_TINYINT
    | K_SMALLINT
    | K_INTEGER | K_INT
    | K_BIGINT
    | (K_DECIMAL | K_DEC) ('(' precision=UNSIGNED_INTEGER (',' scale=UNSIGNED_INTEGER)? ')')?
    | K_SMALLDECIMAL
    | K_REAL
    | K_DOUBLE
    | K_FLOAT ('(' length=UNSIGNED_INTEGER ')')?
    // Characters string types
    | K_VARCHAR ('(' length=UNSIGNED_INTEGER ')')?
    | K_NVARCHAR ('(' length=UNSIGNED_INTEGER ')')?
    | K_ALPHANUM ('(' length=UNSIGNED_INTEGER ')')?
    | K_SHORTTEXT ('(' length=UNSIGNED_INTEGER ')')?
    // Binary types
    | K_VARBINARY ('(' length=UNSIGNED_INTEGER ')')?
    // Large Object types
    | K_BLOB
    | K_CLOB
    | K_NCLOB
    | K_TEXT
    // Datetime types
    | K_DATE
    | K_TIME
    | K_SECONDDATE
    | K_TIMESTAMP
    | K_DAYDATE
    ;

datetimeExpression
    : K_EXTRACT '(' (K_YEAR | K_MONTH | K_DAY | K_HOUR | K_MINUTE | K_SECOND) K_FROM expression ')'
    ;

jsonExpression
    : K_JSON_QUERY '('
        jsonApiCommonSyntax
        (K_RETURNING dataType)?
        jsonWrapperBehavior?
        (jsonBehavior K_ON K_EMPTY)?
        (jsonBehavior K_ON K_ERROR)?
     ')'                                        #jsonQueryExpr
    | K_JSON_TABLE '('
        jsonApiCommonSyntax
        jsonTableColumnsClause
        ((K_ERROR | K_EMPTY) K_ON K_ERROR)?
     ')'                                        #jsonTableExpr
    | K_JSON_VALUE '('
        jsonApiCommonSyntax
        (K_RETURNING dataType)?
        (jsonValueBehavior K_ON K_EMPTY)?
        (jsonValueBehavior K_ON K_ERROR)?
      ')'                                       #jsonValueExpr
    ;

jsonWrapperBehavior
    : K_WITHOUT K_ARRAY? K_WRAPPER
    | K_WITH (K_CONDITIONAL | K_UNCONDITIONAL)? K_ARRAY? K_WRAPPER
    ;

jsonBehavior
    : K_ERROR
    | K_NULL
    | K_EMPTY K_ARRAY
    | K_EMPTY K_OBJECT
    ;

jsonValueBehavior
    : K_ERROR
    | K_NULL
    | K_DEFAULT expression
    ;

jsonApiCommonSyntax
    : expression ',' jsonPathSpecification
    ;

jsonPathSpecification
    : (K_STRICT | K_LAX)? path=STRING_LITERAL
    ;

jsonTableColumnsClause
    : K_COLUMNS '(' jsonTableColumnDefinition (',' jsonTableColumnDefinition)* ')'
    ;

jsonTableColumnDefinition
    : jsonTableOrdinalityColumnDefinition
    | jsonTableRegularColumnDefinition
    | jsonTableFormattedColumnDefinition
    | jsonTableNestedColumns
    ;

jsonTableOrdinalityColumnDefinition
    : columnName K_FOR K_ORDINALITY
    ;

jsonTableRegularColumnDefinition
    : columnName dataType
      K_PATH jsonPathSpecification
      (jsonValueBehavior K_ON K_EMPTY)?
      (jsonValueBehavior K_ON K_ERROR)?
    ;

jsonTableFormattedColumnDefinition
    : columnName dataType
      K_FORMAT K_JSON (K_ENCODING (K_UTF8 | K_UTF16 | K_UTF32))?
      K_PATH jsonPathSpecification
      jsonWrapperBehavior?
      (jsonBehavior K_ON K_EMPTY)?
      (jsonBehavior K_ON K_ERROR)?
    ;

jsonTableNestedColumns
    : K_NESTED K_PATH? jsonPathSpecification jsonTableColumnsClause
    ;

stringExpression
    : (K_LOCATE_REGEXPR | K_SUBSTR_REGEXPR | K_SUBSTRING_REGEXPR)
     '('
        regexprClause
        (K_FROM start=UNSIGNED_INTEGER)?
        (K_OCCURRENCE occurrence=UNSIGNED_INTEGER)?
        (K_GROUP group=UNSIGNED_INTEGER)?
     ')'                                                      #regexprExpr
    | K_OCCURRENCES_REGEXPR '('
        regexprClause
        (K_FROM start=UNSIGNED_INTEGER)?
     ')'                                                      #occurrencesRegexprExpr
    | K_REPLACE_REGEXPR '('
        regexprClause
        (K_WITH replacement=STRING_LITERAL)?
        (K_FROM start=UNSIGNED_INTEGER)?
        (K_OCCURRENCE occurrence=(UNSIGNED_INTEGER | K_ALL))?
     ')'                                                      #replaceRegexprExpr
    |  K_TRIM '('
        ((K_LEADING | K_TRAILING | K_BOTH)? char=STRING_LITERAL K_FROM)?
        input=expression
     ')'                                                      #trimExpr
    | K_XMLTABLE '('
        (xmlNamespaceClause ',')?
        STRING_LITERAL K_PASSING expression K_COLUMNS xmlColumnDefinition (',' xmlColumnDefinition)*
        (K_ERROR K_ON K_ERROR)?
     ')'                                                      #xmlTableExpr 
    ;

regexprClause
    : (K_START | K_AFTER)? pattern=STRING_LITERAL regexFlagClause? K_IN subject=expression
    ;

xmlNamespaceClause
    : K_XMLNAMESPACE '(' xmlNamespace (',' xmlNamespace)* (K_DEFAULT url=STRING_LITERAL)? ')'
    ;

xmlNamespace
    : url=STRING_LITERAL K_AS alas=STRING_LITERAL
    ;

xmlColumnDefinition
    : columnName xmlColumnType
    ;

xmlColumnType
    : K_FOR K_ORDINALITY
    | dataType (K_FORMAT K_XML)? K_PATH STRING_LITERAL (K_DEFAULT STRING_LITERAL)?
    ;

constant
    : STRING_LITERAL       #constantString
    | numericLiteral       #constantNumber
    | booleanLiteral       #constantBoolean
    | intervalLiteral      #constantInterval
    | K_NULL               #constantNull
    ;

intervalLiteral
    : K_INTERVAL UNSIGNED_INTEGER (K_YEAR | K_MONTH | K_DAY | K_HOUR | K_MINUTE | K_SECOND)
    ;

jsonObjectExpression
    : '{' '}'                                                                              #emptyJsonObject
    | '{' properties+=jsonPropertyExpression (',' properties+=jsonPropertyExpression)* '}' #noEmptyJsonObject
    ;

jsonValueExpression
    : jsonStringLiteral
    | numericLiteral
    | booleanLiteral
    | K_NULL
//    | <path_expression>
    | jsonObjectExpression
    | jsonArrayExpression
    ;

jsonArrayExpression
    : '[' ']'                                                                        #emptyJsonArray
    | '[' items+=jsonArrayValueExpression (',' items+=jsonArrayValueExpression)* ']' #noEmptyJsonArray
    ;

jsonPropertyExpression
    : key=jsonStringLiteral ':' value=jsonValueExpression
    ;

jsonArrayValueExpression
    : jsonValueExpression
//    | <path_expression>
    ;

jsonStringLiteral
    : QUOTED_IDENTIFIER
    | STRING_LITERAL
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
    : left=expression op=comparisonOperator (K_ANY | K_SOME | K_ALL)? '(' (right1=expressionList | right2=subquery) ')'
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
    : source=expression K_NOT? K_IN (expressionList | subquery)
    ;

likePredicate
    : source=expression K_NOT? K_LIKE value=expression (K_ESCAPE escape=expression)?
    ;

likeRegexPredicate
    : source=expression K_LIKE_REGEXPR pattern=STRING_LITERAL regexFlagClause?
    ;

regexFlagClause
    : K_FLAG flag=STRING_LITERAL
    ;

memberOfPredicate
    : source=expression K_NOT? K_MEMBER K_OF member=expression
    ;

nullPredicate
    : source=expression K_IS K_NOT? K_NULL
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > CRETE TABLE Statement

seriesClause
    : K_SERIES '(' seriesKey? seriesEquidistantDefinition? seriesMinvalue? seriesMaxvalue? seriesPeriod alternateSeries? ')'
    ;

seriesKey
    : K_SERIES K_KEY columnListClause
    ;

seriesEquidistantDefinition
    : K_NOT K_EQUIDISTANT
    | K_EQUIDISTANT K_INCREMENT K_BY expression (K_MISSING K_ELEMENTS K_NOT? K_ALLOWED)?
    ;

seriesMinvalue
    : K_NO K_MINVALUE
    | K_MINVALUE STRING_LITERAL
    ;

seriesMaxvalue
    : K_NO K_MAXVALUE
    | K_MAXVALUE STRING_LITERAL
    ;

seriesPeriod
    : K_PERIOD K_FOR K_SERIES columnListClause
    ;

alternateSeries
    : K_ALTERNATE K_PERIOD K_FOR K_SERIES columnListClause
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
    : K_AFTER | K_ALLOWED | K_ALPHANUM | K_ALTERNATE | K_AMPLITUDE | K_AND | K_ANY | K_APPLICATION_TIME | K_ARRAY
    | K_ASC | K_AUTO_CORR | K_AUTOMATIC | K_AVG | K_BALANCE | K_BERNOULLI | K_BEST | K_BETWEEN | K_BIGINT | K_BINNING
    | K_BLOB | K_BY | K_CAST | K_CLOB | K_COLLATE | K_COLUMNS | K_CONDITIONAL | K_CONTAINS | K_CORR | K_CORR_SPEARMAN
    | K_COUNT | K_CROSS_CORR | K_CUBIC_SPLINE_APPROX | K_CUME_DIST | K_CURRENT | K_DATA_TRANSFER_COST | K_DATE | K_DAY
    | K_DAYDATE | K_DEC | K_DECIMAL | K_DEFAULT | K_DELETE | K_DELTA | K_DENSE_RANK | K_DESC | K_DFT | K_DOUBLE
    | K_ELEMENTS | K_EMPTY | K_ENCODING | K_EQUIDISTANT | K_ERROR | K_ESCAPE | K_EXACT | K_EXISTS | K_EXTRACT | K_FILL
    | K_FIRST | K_FIRST_VALUE | K_FLAG | K_FLOAT | K_FOLLOWING | K_FORCE | K_FORMAT | K_FULLTEXT | K_FUZZY | K_GROUPING
    | K_HINT | K_HISTORY | K_HOUR | K_IGNORE | K_IMAGINARY | K_INCREMENT | K_INSERT | K_INT | K_INTEGER | K_INTERVAL
    | K_JSON | K_JSON_QUERY | K_JSON_TABLE | K_JSON_VALUE | K_KEY | K_LAG | K_LANGUAGE | K_LAST | K_LAST_VALUE | K_LAX
    | K_LEAD | K_LIKE | K_LIKE_REGEXPR | K_LINEAR_APPROX | K_LINGUISTIC | K_LOCATE_REGEXPR | K_LOCK | K_LOCKED | K_MANY
    | K_MATCHED | K_MATCHES | K_MAX | K_MAXVALUE | K_MEDIAN | K_MEMBER | K_MERGE | K_MIN | K_MINUTE | K_MINVALUE
    | K_MISSING | K_MONTH | K_MULTIPLE | K_NCLOB | K_NEGATIVE_LAGS | K_NESTED | K_NO | K_NO_ROUTE_TO | K_NOT | K_NOTHING
    | K_NOWAIT | K_NTH_VALUE | K_NTILE | K_NULLS | K_NVARCHAR | K_OBJECT | K_OCCURRENCE | K_OCCURRENCES_REGEXPR | K_OF
    | K_OFF | K_OFFSET | K_ONE | K_OR | K_ORDINALITY | K_OUTER | K_OVER | K_OVERRIDING | K_OVERVIEW | K_PARAMETERS
    | K_PART | K_PARTITION | K_PASSING | K_PATH | K_PERCENT_RANK | K_PERCENTILE_CONT | K_PERCENTILE_DISC | K_PERIOD
    | K_PHASE | K_PORTION | K_POSITIVE_LAGS | K_PRECEDING | K_PREFIX | K_PRIMARY | K_RANDOM_PARTITION | K_RANK | K_REAL
    | K_REBUILD | K_REPLACE | K_REPLACE_REGEXPR | K_RESULT | K_RESULTSETS | K_RETURNING | K_ROUND_CEILING | K_ROUND_DOWN
    | K_ROUND_FLOOR | K_ROUND_HALF_DOWN | K_ROUND_HALF_EVEN | K_ROUND_HALF_UP | K_ROUND_UP | K_ROUTE_BY
    | K_ROUTE_BY_CARDINALITY | K_ROUTE_TO | K_ROW | K_ROW_NUMBER | K_ROWCOUNT | K_ROWS | K_SECOND | K_SECONDDATE
    | K_SERIES | K_SERIES_DISAGGREGATE | K_SERIES_DISAGGREGATE_BIGINT | K_SERIES_DISAGGREGATE_DATE
    | K_SERIES_DISAGGREGATE_DECIMAL | K_SERIES_DISAGGREGATE_INTEGER | K_SERIES_DISAGGREGATE_SECONDDATE
    | K_SERIES_DISAGGREGATE_SMALLDECIMAL | K_SERIES_DISAGGREGATE_SMALLINT | K_SERIES_DISAGGREGATE_TIME
    | K_SERIES_DISAGGREGATE_TIMESTAMP | K_SERIES_DISAGGREGATE_TINYINT | K_SERIES_ELEMENT_TO_PERIOD | K_SERIES_FILTER
    | K_SERIES_GENERATE | K_SERIES_GENERATE_BIGINT | K_SERIES_GENERATE_DATE | K_SERIES_GENERATE_DECIMAL
    | K_SERIES_GENERATE_INTEGER | K_SERIES_GENERATE_SECONDDATE | K_SERIES_GENERATE_SMALLDECIMAL
    | K_SERIES_GENERATE_SMALLINT | K_SERIES_GENERATE_TIME | K_SERIES_GENERATE_TIMESTAMP | K_SERIES_GENERATE_TINYINT
    | K_SERIES_PERIOD_TO_ELEMENT | K_SERIES_ROUND | K_SETS | K_SHARE | K_SHORTTEXT | K_SMALLDECIMAL | K_SMALLINT
    | K_SOME | K_SORT | K_SPECIFIED | K_STDDEV | K_STDDEV_POP | K_STDDEV_SAMP | K_STRICT | K_STRING_AGG | K_STRUCTURED
    | K_SUBSTR_REGEXPR | K_SUBSTRING_REGEXPR | K_SUBTOTAL | K_SUM | K_SYSTEM | K_SYSTEM_TIME | K_TABLE | K_TEXT
    | K_TEXT_FILTER | K_THEN | K_TIME | K_TIMESTAMP | K_TINYINT | K_TO | K_TOTAL | K_TRIM | K_UNBOUNDED
    | K_UNCONDITIONAL | K_UNNEST | K_UP | K_UPDATE | K_UPSERT | K_USER | K_UTF16 | K_UTF32 | K_UTF8 | K_VALUE | K_VAR
    | K_VAR_POP | K_VAR_SAMP | K_VARBINARY | K_VARCHAR | K_WAIT | K_WEIGHT | K_WEIGHTED_AVG | K_WITHIN | K_WITHOUT
    | K_WRAPPER | K_XML | K_XMLNAMESPACE | K_XMLTABLE | K_YEAR | K_ZERO_LAG
    ;
