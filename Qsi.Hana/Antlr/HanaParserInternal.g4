parser grammar HanaParserInternal;

options { 
    tokenVocab=HanaLexerInternal;
}

@header {
    using Qsi.Data;
    using Qsi.Tree;
    using Qsi.Utilities;
}

root
    : EOF
    | (hanaStatement (SEMI EOF? | EOF))+
    ;

hanaStatement
    : dataManipulationStatement
    | dataDefinitionStatement
    | sessionManagementStatement
    ;

dataManipulationStatement
    : deleteStatement
    // | explainPlanStatement
    | insertStatement
    // | loadStatement
    | mergeDeltaStatement
    | mergeIntoStatement
    | replaceStatement
    | selectIntoStatement
    | selectStatement[true]
    // | truncateTableStatement
    // | unloadStatement
    | updateStatement
    ;

dataDefinitionStatement
    : createViewStatement
    ;

sessionManagementStatement
    : setSchemaStatement
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > SELECT Statement ------

selectIntoStatement
    : selectStatement[false] K_INTO (tableRef | variableNameList) columnListClause? hintClause?
    ;

selectStatement[bool allowParens]
    : withClause? subquery[$allowParens] (forClause | timeTravel)? hintClause?
    ;

subquery[bool allowParens]
    : select  = selectClause
      from    = fromClause
      where   = whereClause?
      groupBy = groupByClause?
      set     = setOperatorClause?
      orderBy = tableOrderByClause?
      limit   = limitClause?
    | {$allowParens}? '(' inner=selectStatement[true] ')' 
      set = setOperatorClause?
    ;

setSubquery
    : select  = selectClause
      from    = fromClause
      where   = whereClause?
      groupBy = groupByClause?
    | '(' inner=selectStatement[true] ')'
    ;

withClause
    : K_WITH elements+=withListElement (',' elements+=withListElement)*
    ;

withListElement
    : name=identifier[null] columnListClause? K_AS '(' subquery[true] ')'
    ;

columnList returns [IList<QsiQualifiedIdentifier> list]
    @init { $list = new List<QsiQualifiedIdentifier>(); }
    : n=fieldName { $list.Add($n.qqi); } (',' n=fieldName { $list.Add($n.qqi); })*
    ;

columnListClause returns [IList<QsiQualifiedIdentifier> list]
    : '(' cl=columnList { $list = $cl.list; } ')'
    ;

forClause
    : K_FOR K_SHARE K_LOCK                                                            #forShareLockClause
    | K_FOR K_UPDATE (K_OF columnListClause)? waitNowait? (K_IGNORE K_LOCKED)?        #forUpdateOfClause
    | K_FOR (K_JSON | K_XML) forJsonOrXmlOptionListClause? forJsonOrXmlReturnsClause? #forJsonXmlClause
    | forSystemTime                                                                   #forSystemTimeClause
    ;

forSystemTime
    : K_FOR K_SYSTEM_TIME K_AS K_OF value=STRING_LITERAL                            #forSystemTimeAsOf
    | K_FOR K_SYSTEM_TIME K_FROM from=STRING_LITERAL K_TO to=STRING_LITERAL         #forSystemTimeFrom
    | K_FOR K_SYSTEM_TIME K_BETWEEN lower=STRING_LITERAL K_AND upper=STRING_LITERAL #forSystemTimeBetween
    ;

waitNowait
    : K_WAIT time=unsignedIntegerOrBindParameter
    | K_NOWAIT
    ;

keyValuePair
    : key=STRING_LITERAL '=' value=STRING_LITERAL
    ;

forJsonOrXmlOptionListClause
    : '(' options+=keyValuePair (',' options+=keyValuePair)* ')'
    ;

forJsonOrXmlReturnsClause
    : K_RETURNS (
        K_VARCHAR '(' unsignedIntegerOrBindParameter ')'
        | K_NVARCHAR '(' unsignedIntegerOrBindParameter ')'
        | K_CLOB
        | K_NCLOB
     )
    ;

timeTravel
    : K_AS K_OF K_COMMIT K_ID unsignedIntegerOrBindParameter #commitId
    | K_AS K_OF K_UTCTIMESTAMP STRING_LITERAL  #timestamp
    ;

selectClause
   : K_SELECT topClause? (K_ALL | K_DISTINCT)? selectList
   ;

topClause
    : K_TOP top=unsignedIntegerOrBindParameter
    ;

selectList
    : items+=selectItem (',' items+=selectItem)*
    ;

selectItem
    : expression alias?             #exprItem
    | associationExpression alias?  #associationExprItem
    | (tableName '.')? '*'          #wildcardItem
    ;

columnName returns [QsiIdentifier qi]
    : i=identifier[null] { $qi = $i.qi; }
    ;

fromClause
    : K_FROM tables+=tableExpression (',' tables+=tableExpression)*
    ;

seriesTable
    : K_SERIES K_TABLE tableName
    ;

tableExpression
    : '(' inner=tableExpression ')'
    | tableRef (K_CROSS K_JOIN crossJoin=tableRef)?
//    | systemVersionedTableRef
    | subqueryTableExpression
    | left=tableExpression joinType? joinCardinality? K_JOIN right=tableExpression K_ON condition
    | caseJoin
    | lateralTableExpression
    | collectionDerivedTable
    | tableFunctionExpression
    | variableTable
    | associationTableExpression
    ;

subqueryTableExpression
    : subquery[true] alias?
    ;

tableFunctionExpression
    : (seriesExpression | functionExpression[true]) alias?
    ;

tableRef
    : tableName (forSystemTime | forApplicationTimePeriod)? partitionRestriction? alias? tableSampleClause?
    ;

forApplicationTimePeriod
    : K_FOR K_APPLICATION_TIME K_AS K_OF { IsQuotedNumeric() }? value=STRING_LITERAL
    ;

partitionRestriction
    : K_PARTITION '(' numbers+=unsignedIntegerOrBindParameter (',' numbers+=unsignedIntegerOrBindParameter)* ')'
    ;

tableSampleClause
    : K_TABLESAMPLE (K_BERNOULLI | K_SYSTEM)? '(' size=numericLiteral ')'
    ;

hintClause
    : K_WITH K_HINT '(' hints+=hintElement (',' hints+=hintElement)* ')'
    ;

hintElement
    : name=UNQUOTED_IDENTIFIER                                                                                                  #hintName
    | K_ROUTE_TO             '(' volumeIds+=unsignedIntegerOrBindParameter (',' volumeIds+=unsignedIntegerOrBindParameter)? ')' #routeTo
    | K_NO_ROUTE_TO          '(' volumeIds+=unsignedIntegerOrBindParameter (',' volumeIds+=unsignedIntegerOrBindParameter)? ')' #noRouteTo
    | K_ROUTE_BY             '(' tables+=tableName (',' tables+=tableName)* ')'                                                 #routeBy
    | K_ROUTE_BY_CARDINALITY '(' tables+=tableName (',' tables+=tableName)* ')'                                                 #routeByCardinality
    | K_DATA_TRANSFER_COST   '(' cost=unsignedIntegerOrBindParameter ')'                                                        #rdataTransferCost
    ;

fieldName returns [QsiQualifiedIdentifier qqi] locals [List<QsiIdentifier> buffer]
    @init { $buffer = new List<QsiIdentifier>(); }
    @after { $qqi = new QsiQualifiedIdentifier($buffer); }
    : identifier[$buffer] ('.' identifier[$buffer] ('.' identifier[$buffer] ('.' identifier[$buffer])?)?)?
    ;

tableName returns [QsiQualifiedIdentifier qqi] locals [List<QsiIdentifier> buffer]
    @init { $buffer = new List<QsiIdentifier>(); }
    @after { $qqi = new QsiQualifiedIdentifier($buffer); }
    : identifier[$buffer] ('.' identifier[$buffer] ('.' identifier[$buffer])?)?
    ;

alias returns [QsiAliasNode node]
    @after { $node = new QsiAliasNode { Name = $name.qi }; }
    : K_AS? name=identifier[null]
    ;

explicitAlias returns [QsiAliasNode node]
    @after { $node = new QsiAliasNode { Name = $name.qi }; }
    : K_AS name=identifier[null]
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
    : identifier[null] ('[' index=numericLiteral ']')?
    ;

variableNameList
    : variableName (',' variableName)*
    ;

whereClause
    : K_WHERE condition
    ;

groupByClause
    : K_GROUP K_BY groupByExpressionList (K_HAVING having=condition)?
    ;

groupByExpressionList
    : (expression | groupingSet) (',' (expression | groupingSet))*
    ;

groupingSet
    : (K_GROUPING K_SETS | K_ROLLUP | K_CUBE)
      (K_BEST best=numericLiteral)?
      (K_LIMIT limit=unsignedIntegerOrBindParameter (K_OFFSET offset=unsignedIntegerOrBindParameter)?)?
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
    : items+=groupingExpression (',' items+=groupingExpression)*
    ;

groupingExpression
    : fields+=fieldName
    | '(' fields+=fieldName (',' fields+=fieldName)* ')'
    | '(' '(' fields+=fieldName (',' fields+=fieldName)* ')' tableOrderByClause ')'
    ;

prefixTableName
    : '#' identifier[null]
    ;

variableTable
    : ':' identifier[null] alias?
    ;

tableOrderByClause
    : K_ORDER K_BY orders+=tableOrderByExpression (',' orders+=tableOrderByExpression)*
    ;

tableOrderByExpression
    : (field=fieldName | position=unsignedIntegerOrBindParameter) collateClause? (K_ASC | K_DESC)? (K_NULLS K_FIRST | K_NULLS K_LAST)?
    ;

collateClause
    : K_COLLATE name=UNICODE_IDENTIFIER
    ;

setOperator
    : K_UNION (K_ALL | K_DISTINCT)?
    | K_INTERSECT K_DISTINCT?
    | K_EXCEPT K_DISTINCT?
    ;

setOperatorClause
    : (setOperator setSubquery)+
      orderBy = tableOrderByClause?
      limit   = limitClause? 
    ;

limitClause
    : K_LIMIT limit=unsignedIntegerOrBindParameter (K_OFFSET offset=unsignedIntegerOrBindParameter)? (K_TOTAL K_ROWCOUNT)?
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
            caseJoinWhenClause+
            caseJoinElseClause?
        K_END alias?
    ;

caseJoinWhenClause
    : K_WHEN condition K_THEN K_RETURN columnListClause K_FROM tableRef K_ON predicate
    ;

caseJoinElseClause
    : K_ELSE K_RETURN columnListClause K_FROM tableRef K_ON predicate
    ;

lateralTableExpression
    : K_LATERAL '(' (subquery[true] | functionExpression[true]) ')' alias?
    ;

collectionDerivedTable
    : K_UNNEST '(' collectionValueExpression (',' collectionValueExpression)* ')' (K_WITH K_ORDINALITY)? explicitAlias columnListClause?
    ;

collectionValueExpression
    : K_ARRAY '(' (expression (',' expression)* | columnName) ')'
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
        valueListClause
        | overridingClause? selectStatement[true]
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
    | '(' withClause subquery[true] ')' // TODO: check to real db
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > REPLACE | UPSERT Statement ------

replaceStatement
    : (K_UPSERT | K_REPLACE) tableName partitionRestriction? columnListClause?
      (
        valueListClause (whereClause | K_WITH K_PRIMARY K_KEY)?
        | selectStatement[true]
      )
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > MERGE DELTA Statement ------

mergeDeltaStatement
    : K_MERGE K_HISTORY? K_DELTA K_OF tableName (K_PART part=unsignedIntegerOrBindParameter)?
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
    : caseExpression                         #caseExpr
    | windowExpression                       #windowExpr
    | aggregateExpression                    #aggExpr
    | dataTypeConversionExpression           #conversionExpr
    | dateTimeExpression                     #dateTimeExpr
    | functionExpression[false]              #functionExpr
    | '(' expression (',' expression)* ')'   #setExpr
    | '(' subquery[true] ')'                 #subqueryExpr
    | '-' expression                         #unaryExpr
    | l=expression op=operator r=expression  #operationExpr
    | fieldName                              #fieldExpr
    | constant                               #constantExpr
    | identifier[null] '=>' expression       #lambdaExpr
    | jsonObjectExpression                   #jsonObjectExpr
    | jsonArrayExpression                    #jsonArrayExpr
    | bindParameterExpression                #bindParamExpr
    ;

expressionList
    : list+=expression (',' list+=expression)*
    ;

expressionOrSubqueryList
    : list+=expressionOrSubquery (',' list+=expressionOrSubquery)*
    ;

expressionOrSubquery
    : subquery[true]
    | expression
    ;

caseExpression
    : simpleCaseExpression
    | searchCaseExpression
    ;

simpleCaseExpression
    : K_CASE case=expression
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
    : predicate                       #predicateCondition
    | condition K_OR condition        #orCondition
    | condition K_AND condition       #andCondition
    | K_NOT condition                 #notCondition
    | '(' condition ')'               #parenthesisCondition
    | K_CURRENT K_OF identifier[null] #currentOfCondition
    ;

functionExpression[bool table]
    : jsonExpression[$table]                         #jsonExpr
    | stringExpression[$table]                       #stringExpr
    | { $table == false }? inlineFunctionName        #inlineExpr
    | functionName '(' expressionOrSubqueryList? ')' #scalarExpr
    ;

functionName returns [QsiQualifiedIdentifier qqi] locals [List<QsiIdentifier> buffer]
    @init { $buffer = new List<QsiIdentifier>(); }
    @after { $qqi = new QsiQualifiedIdentifier($buffer); }
    : identifier[$buffer] ('.' identifier[$buffer] ('.' identifier[$buffer] ('.' identifier[$buffer])?)?)?
    ;

inlineFunctionName
    : K_CURRENT_CONNECTION
    | K_CURRENT_SCHEMA
    | K_CURRENT_DATE
    | K_CURRENT_TIME
    | K_CURRENT_TRANSACTION_ISOLATION_LEVEL
    | K_CURRENT_UTCTIME
    | K_CURRENT_TIMESTAMP
    | K_CURRENT_UTCDATE
    | K_CURRENT_USER
    | K_CURRENT_UTCTIMESTAMP
    | K_SYSUUID
    | K_SESSION_USER
    ;

aggregateExpression
    : K_COUNT '(' '*' ')'                                                                      #aggCountExpr
    | K_COUNT '(' K_DISTINCT expressionList ')'                                                #aggCountDistinctExpr
    | K_STRING_AGG '(' expression (',' delimiter=expression)? aggregateOrderByClause? ')'      #aggStringExpr
    | K_CROSS_CORR '('
        expression ',' 
        expression ','
        unsignedIntegerOrBindParameter (seriesOrderBy | aggregateOrderByClause) 
      ')' ('.' (K_POSITIVE_LAGS | K_NEGATIVE_LAGS | K_ZERO_LAG))?                              #aggCrossCorrExpr
    | K_DFT '('
        expression ','
        unsignedIntegerOrBindParameter (seriesOrderBy | aggregateOrderByClause)
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
    : fieldName (K_ASC | K_DESC)? (K_NULLS (K_FIRST | K_LAST))? collateClause?
    ;

windowFrameClause
    : K_ROWS (windowFrameStart | windowFrameBetween)
    ;

windowFrameStart
    : K_UNBOUNDED K_PRECEDING
    | unsignedIntegerOrBindParameter K_PRECEDING
    | K_CURRENT K_ROW
    ;

windowFrameBetween
    : K_BETWEEN lower=windowFrameBound K_AND upper=windowFrameBound
    ;

windowFrameBound
    : windowFrameStart
    | K_UNBOUNDED K_FOLLOWING
    | unsignedIntegerOrBindParameter K_FOLLOWING
    ;

windowExpression
    : K_BINNING             '(' expressionList ')' windowSpecification                 #windowBinningExpr
    | K_CUBIC_SPLINE_APPROX '(' expressionList ')' windowWithSeriesSpecification?      #windowCubicSplineApproxExpr
    | K_CUME_DIST           '(' ')' windowSpecification                                #windowCumeDistExpr
    | K_DENSE_RANK          '(' ')' windowSpecification                                #windowDenseRankExpr
    | K_LAG                 '(' expressionList ')' windowSpecification                 #windowLagExpr
    | K_LEAD                '(' expressionList ')' windowSpecification                 #windowLeadExpr
    | K_LINEAR_APPROX       '(' expressionList ')' windowWithSeriesSpecification       #windowLinearApproxExpr
    | K_NTILE               '(' unsignedIntegerOrBindParameter ')' windowSpecification #windowNtileExpr
    | K_PERCENT_RANK        '(' ')' windowSpecification                                #windowPercentRankExpr
    | K_PERCENTILE_CONT     '(' expression ')' withinGroupClause windowSpecification   #windowPercentileContExpr
    | K_PERCENTILE_DISC     '(' expression ')' withinGroupClause windowSpecification   #windowPercentileDiscExpr
    | K_RANDOM_PARTITION    '(' expressionList ')' windowSpecification                 #windowRandomPartitionExpr
    | K_RANK                '(' ')' windowSpecification                                #windowRankExpr
    | K_ROW_NUMBER          '(' ')' windowSpecification                                #windowRowNumberExpr
    | K_SERIES_FILTER       '(' expressionList ')' windowWithSeriesSpecification       #windowSeriesFilterExpr
    | K_WEIGHTED_AVG        '(' expression ')' windowSpecification                     #windowWeightedAvgExpr
    | aggregateExpression windowSpecification                                          #windowAggExpr
    | seriesExpression windowSpecification?                                            #windowSeriesExpr
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
    : K_SERIES_ELEMENT_TO_PERIOD '(' unsignedIntegerOrBindParameter ',' (expression ',' expression ',' expression | seriesTable) ')'
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
    | (K_DECIMAL | K_DEC) ('(' precision=unsignedIntegerOrBindParameter (',' scale=unsignedIntegerOrBindParameter)? ')')?
    | K_SMALLDECIMAL
    | K_REAL
    | K_DOUBLE
    | K_FLOAT ('(' length=unsignedIntegerOrBindParameter ')')?
    // Boolean type
    | K_BOOLEAN
    // Characters string types
    | K_VARCHAR ('(' length=unsignedIntegerOrBindParameter ')')?
    | K_NVARCHAR ('(' length=unsignedIntegerOrBindParameter ')')?
    | K_ALPHANUM ('(' length=unsignedIntegerOrBindParameter ')')?
    | K_SHORTTEXT ('(' length=unsignedIntegerOrBindParameter ')')?
    // Binary types
    | K_VARBINARY ('(' length=unsignedIntegerOrBindParameter ')')?
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

dateTimeExpression
    : K_EXTRACT '(' dateTimeKind K_FROM expression ')'
    ;

dateTimeKind
    : K_YEAR
    | K_MONTH
    | K_DAY
    | K_HOUR
    | K_MINUTE
    | K_SECOND
    ;

jsonExpression[bool table]
    : { $table == false }?
      K_JSON_QUERY '('
        jsonApiCommonSyntax
        (K_RETURNING dataType)?
        jsonWrapperBehavior?
        (jsonBehavior K_ON K_EMPTY)?
        (jsonBehavior K_ON K_ERROR)?
     ')'                                        #jsonQueryExpr
    | { $table }?
      K_JSON_TABLE '('
        jsonApiCommonSyntax
        jsonTableColumnsClause
        ((K_ERROR | K_EMPTY) K_ON K_ERROR)?
     ')'                                        #jsonTableExpr
    | { $table == false }?
      K_JSON_VALUE '('
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
    : (data=STRING_LITERAL | dataColumn=fieldName) ',' jsonPathSpecification
    ;

jsonPathSpecification
    : (K_STRICT | K_LAX)? path=STRING_LITERAL
    ;

jsonTableColumnsClause
    : K_COLUMNS '(' defs+=jsonTableColumnDefinition (',' defs+=jsonTableColumnDefinition)* ')'
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
      (empty=jsonValueBehavior K_ON K_EMPTY)?
      (error=jsonValueBehavior K_ON K_ERROR)?
    ;

jsonTableFormattedColumnDefinition
    : columnName dataType
      K_FORMAT K_JSON (K_ENCODING (enc=K_UTF8 | enc=K_UTF16 | enc=K_UTF32))?
      K_PATH jsonPathSpecification
      wrapper=jsonWrapperBehavior?
      (empty=jsonBehavior K_ON K_EMPTY)?
      (error=jsonBehavior K_ON K_ERROR)?
    ;

jsonTableNestedColumns
    : K_NESTED K_PATH? jsonPathSpecification jsonTableColumnsClause
    ;

stringExpression[bool table]
    : { $table == false }?
     (K_LOCATE_REGEXPR | K_SUBSTR_REGEXPR | K_SUBSTRING_REGEXPR)
     '('
        regexprClause
        (K_FROM start=unsignedIntegerOrBindParameter)?
        (K_OCCURRENCE occurrence=unsignedIntegerOrBindParameter)?
        (K_GROUP group=unsignedIntegerOrBindParameter)?
     ')'                                                      #regexprExpr
    | { $table == false }?
      K_OCCURRENCES_REGEXPR '('
        regexprClause
        (K_FROM start=unsignedIntegerOrBindParameter)?
     ')'                                                      #occurrencesRegexprExpr
    | { $table == false }?
      K_REPLACE_REGEXPR '('
        regexprClause
        (K_WITH replacement=STRING_LITERAL)?
        (K_FROM start=unsignedIntegerOrBindParameter)?
        (K_OCCURRENCE (occurrence1=unsignedIntegerOrBindParameter | occurrence2=K_ALL))?
     ')'                                                      #replaceRegexprExpr
    | { $table == false }?
      K_TRIM '('
        ((K_LEADING | K_TRAILING | K_BOTH)? char=STRING_LITERAL K_FROM)?
        input=expression
     ')'                                                      #trimExpr
    | { $table }?
      K_XMLTABLE '('
        (xmlNamespaceClause ',')?
        pattern=STRING_LITERAL
        K_PASSING (data=STRING_LITERAL | dataColumn=fieldName)
        K_COLUMNS columns+=xmlColumnDefinition (',' columns+=xmlColumnDefinition)*
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
    | binaryLiteral        #constantBinary
    | numericLiteral       #constantNumber
    | booleanLiteral       #constantBoolean
    | intervalLiteral      #constantInterval
    | K_NULL               #constantNull
    ;

intervalLiteral
    : K_INTERVAL unsignedIntegerOrBindParameter (K_YEAR | K_MONTH | K_DAY | K_HOUR | K_MINUTE | K_SECOND)
    ;

unsignedIntegerOrBindParameter
    : v=UNSIGNED_INTEGER
    | b=bindParameterExpression
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

bindParameterExpression returns [int index]
    : '?'                    { $index = NextBindParameterIndex(); } #bindParam1
    | ':' n=UNSIGNED_INTEGER { $index = int.Parse($n.text); }       #bindParam2
    | ':' identifier[null]                                          #bindParam3
    ;

// ------ SQL Reference > Predicates ------

predicate
    : '(' inner=predicate ')'
    | comparisonPredicate
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
    : left=expression op=comparisonOperator (K_ANY | K_SOME | K_ALL)? right=expression
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
    : K_NOT? K_EXISTS '(' subquery[true] ')'
    ;

inPredicate
    : ('(' left1=expressionList ')' | left2=expression) K_NOT? K_IN '(' (right1=expressionList | right2=subquery[true]) ')'
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

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > CREATE VIEW Statement ------

createViewStatement returns [
    QsiQualifiedIdentifier name,
    string comment,
    bool structuredPrivilegeCheck,
    bool force,
    bool checkOption,
    bool ddlOnly,
    bool readOnly
]
    : K_CREATE K_VIEW n=viewName               { $name = $n.qqi; }
      (K_COMMENT cmt=STRING_LITERAL            { $comment = IdentifierUtility.Unescape($cmt.text); })?
      columnListClause?
      parameterizedViewClause?
      K_AS selectStatement[true]
      withAssociationClause?
      withMaskClause?
      withExpressionMacroClause?
      withAnnotationClause?
      (K_WITH K_STRUCTURED K_PRIVILEGE K_CHECK { $structuredPrivilegeCheck = true; })?
      withCacheClause?
      (K_FORCE                                 { $force = true; })?
      (K_WITH K_CHECK K_OPTION                 { $checkOption = true; })?
      (K_WITH K_DDL K_ONLY                     { $ddlOnly = true; })?
      (K_WITH K_READ K_ONLY                    { $readOnly = true; })?
      withAnonymizationClause?
    ;

viewName returns [QsiQualifiedIdentifier qqi] locals [List<QsiIdentifier> buffer]
    @init { $buffer = new List<QsiIdentifier>(); }
    @after { $qqi = new QsiQualifiedIdentifier($buffer); }
    : identifier[$buffer] ('.' identifier[$buffer] )?
    ;

withAssociationClause
    : K_WITH K_ASSOCIATIONS '(' defs+=associationDef (',' defs+=associationDef)* ')'
    ;

associationDef
    : forwardJoinDef
    | propagationDef
    ;

forwardJoinDef
    : joinCardinalityClass
    ;

joinCardinalityClass
    : joinCardinality? K_JOIN tableName explicitAlias? K_ON condition
    | propagationDef
    ;

propagationDef
    : tableName explicitAlias?
    ;

parameterizedViewClause
    : '(' defs+=parameterDef (',' defs+=parameterDef)* ')'
    ;

parameterDef
    : K_IN identifier[null] dataType expression?
    ;

withMaskClause
    : K_WITH K_MASK '(' defs+=maskDef (',' defs+=maskDef)* ')'
    ;

maskDef
    : columnName K_USING expression
    ;

withExpressionMacroClause
    : K_WITH K_EXPRESSION K_MACROS '(' defs+=expressionMacroDef (',' defs+=expressionMacroDef)* ')'
    ;

expressionMacroDef
    : expression explicitAlias
    ;

withAnnotationClause
    : K_WITH K_ANNOTATIONS '(' (setViewAnnotations? columnAnnotation* parameterAnnotation*) ')'
    ;

setViewAnnotations
    : keySetOperation
    ;

columnAnnotation
    : K_COLUMN columnName keySetOperation
    ;

parameterAnnotation
    : K_PARAMETER columnName keySetOperation
    ;

keySetOperation
    : K_SET keyValuePair (',' keyValuePair)*
    ;

withCacheClause
    : K_WITH (K_STATIC | K_DYNAMIC)? K_CACHE (K_NAME identifier[null])
      (K_RETENTION unsignedIntegerOrBindParameter)?
      (K_OF projectionClause)?
      (K_FILTER condition)?
      locationClause?
    ;

projectionClause
    : defs+=projectionDef (',' defs+=projectionDef)*
    ;

projectionDef
    : (K_SUM | K_MIN | K_MAX | K_COUNT) '(' columnName ')' #projectionAggrDef
    | columnName                                           #projectionColumnDef
    ;

locationClause
    : K_AT K_LOCATION? locs+=STRING_LITERAL (',' locs+=STRING_LITERAL)*
    ;

withAnonymizationClause
    : K_WITH K_ANONYMIZATION '(' K_ALGORITHM STRING_LITERAL (viewLevelParameter? columnLevelParameter*) ')'
    ;

viewLevelParameter
    : K_PARAMETERS STRING_LITERAL
    ;

columnLevelParameter
    : K_COLUMN columnName K_PARAMETERS STRING_LITERAL
    ;

// ------ SQL Reference > SQL Statements > Alpabetical List of Statements > SET SCHEMA Statement ------

setSchemaStatement
    : K_SET K_SCHEMA identifier[null]
    ;

// ------ ETC ------

binaryLiteral
    : HEX_NUMBER
    ;

booleanLiteral
    : K_TRUE
    | K_FALSE
    ;

numericLiteral returns [bool negative]
    : ('+' | '-' { $negative = !$negative; })+ numericLiteral #signedNumericLiteral
    | EXACT_NUMERIC_LITERAL             #exactNumericLiteral
    | APPROXIMATE_NUMERIC_LITERAL       #approximateNumericLiteral
    | unsignedIntegerOrBindParameter    #unsignedIntegerOrBindParameter_
    ;

identifier[List<QsiIdentifier> buffer] returns [QsiIdentifier qi]
    @after { $buffer?.Add($qi); }
    : i=UNQUOTED_IDENTIFIER { $qi = new QsiIdentifier($i.text.ToUpper(), false); }
    | i=QUOTED_IDENTIFIER   { $qi = new QsiIdentifier($i.text, true); }
    | i=UNICODE_IDENTIFIER  { $qi = new QsiIdentifier($i.text.ToUpper(), false); }
    | ki=keywodIdentifier   { $qi = new QsiIdentifier($ki.text.ToUpper(), false); }
    ;

keywodIdentifier
    : K_AFTER | K_ALGORITHM | K_ALLOWED | K_ALPHANUM | K_ALTERNATE | K_AMPLITUDE | K_AND | K_ANNOTATIONS
    | K_ANONYMIZATION | K_ANY | K_APPLICATION_TIME | K_ARRAY | K_ASC | K_ASSOCIATIONS | K_AT | K_AUTO_CORR | K_AUTOMATIC
    | K_AVG | K_BALANCE | K_BERNOULLI | K_BEST | K_BETWEEN | K_BIGINT | K_BINNING | K_BLOB | K_BOOLEAN | K_BY | K_CACHE
    | K_CAST | K_CHECK | K_CLOB | K_COLLATE | K_COLUMN | K_COLUMNS | K_COMMENT | K_COMMIT | K_CONDITIONAL | K_CONTAINS
    | K_CORR | K_CORR_SPEARMAN | K_COUNT | K_CREATE | K_CROSS_CORR | K_CUBIC_SPLINE_APPROX | K_CUME_DIST | K_CURRENT
    | K_DATA_TRANSFER_COST | K_DATE | K_DAY | K_DAYDATE | K_DDL | K_DEC | K_DECIMAL | K_DEFAULT | K_DELETE | K_DELTA
    | K_DENSE_RANK | K_DESC | K_DFT | K_DOUBLE | K_DYNAMIC | K_ELEMENTS | K_EMPTY | K_ENCODING | K_EQUIDISTANT | K_ERROR
    | K_ESCAPE | K_EXACT | K_EXISTS | K_EXPRESSION | K_EXTRACT | K_FILL | K_FILTER | K_FIRST | K_FIRST_VALUE | K_FLAG
    | K_FLOAT | K_FOLLOWING | K_FORCE | K_FORMAT | K_FULLTEXT | K_FUZZY | K_GROUPING | K_HINT | K_HISTORY | K_HOUR
    | K_ID | K_IGNORE | K_IMAGINARY | K_INCREMENT | K_INSERT | K_INT | K_INTEGER | K_INTERVAL | K_JSON | K_JSON_QUERY
    | K_JSON_TABLE | K_JSON_VALUE | K_KEY | K_LAG | K_LANGUAGE | K_LAST | K_LAST_VALUE | K_LAX | K_LEAD | K_LIKE
    | K_LIKE_REGEXPR | K_LINEAR_APPROX | K_LINGUISTIC | K_LOCATE_REGEXPR | K_LOCATION | K_LOCK | K_LOCKED | K_MACROS
    | K_MANY | K_MASK | K_MATCHED | K_MATCHES | K_MAX | K_MAXVALUE | K_MEDIAN | K_MEMBER | K_MERGE | K_MIN | K_MINUTE
    | K_MINVALUE | K_MISSING | K_MONTH | K_MULTIPLE | K_NAME | K_NCLOB | K_NEGATIVE_LAGS | K_NESTED | K_NO
    | K_NO_ROUTE_TO | K_NOT | K_NOTHING | K_NOWAIT | K_NTH_VALUE | K_NTILE | K_NULLS | K_NVARCHAR | K_OBJECT
    | K_OCCURRENCE | K_OCCURRENCES_REGEXPR | K_OF | K_OFF | K_OFFSET | K_ONE | K_ONLY | K_OPTION | K_OR | K_ORDINALITY
    | K_OUTER | K_OVER | K_OVERRIDING | K_OVERVIEW | K_PARAMETER | K_PARAMETERS | K_PART | K_PARTITION | K_PASSING
    | K_PATH | K_PERCENT_RANK | K_PERCENTILE_CONT | K_PERCENTILE_DISC | K_PERIOD | K_PHASE | K_PORTION | K_POSITIVE_LAGS
    | K_PRECEDING | K_PREFIX | K_PRIMARY | K_PRIVILEGE | K_RANDOM_PARTITION | K_RANK | K_READ | K_REAL | K_REBUILD
    | K_REPLACE | K_REPLACE_REGEXPR | K_RESULT | K_RESULTSETS | K_RETENTION | K_RETURNING | K_ROUND_CEILING
    | K_ROUND_DOWN | K_ROUND_FLOOR | K_ROUND_HALF_DOWN | K_ROUND_HALF_EVEN | K_ROUND_HALF_UP | K_ROUND_UP | K_ROUTE_BY
    | K_ROUTE_BY_CARDINALITY | K_ROUTE_TO | K_ROW | K_ROW_NUMBER | K_ROWCOUNT | K_ROWS | K_SCHEMA | K_SECOND
    | K_SECONDDATE | K_SERIES | K_SERIES_DISAGGREGATE | K_SERIES_DISAGGREGATE_BIGINT | K_SERIES_DISAGGREGATE_DATE
    | K_SERIES_DISAGGREGATE_DECIMAL | K_SERIES_DISAGGREGATE_INTEGER | K_SERIES_DISAGGREGATE_SECONDDATE
    | K_SERIES_DISAGGREGATE_SMALLDECIMAL | K_SERIES_DISAGGREGATE_SMALLINT | K_SERIES_DISAGGREGATE_TIME
    | K_SERIES_DISAGGREGATE_TIMESTAMP | K_SERIES_DISAGGREGATE_TINYINT | K_SERIES_ELEMENT_TO_PERIOD | K_SERIES_FILTER
    | K_SERIES_GENERATE | K_SERIES_GENERATE_BIGINT | K_SERIES_GENERATE_DATE | K_SERIES_GENERATE_DECIMAL
    | K_SERIES_GENERATE_INTEGER | K_SERIES_GENERATE_SECONDDATE | K_SERIES_GENERATE_SMALLDECIMAL
    | K_SERIES_GENERATE_SMALLINT | K_SERIES_GENERATE_TIME | K_SERIES_GENERATE_TIMESTAMP | K_SERIES_GENERATE_TINYINT
    | K_SERIES_PERIOD_TO_ELEMENT | K_SERIES_ROUND | K_SETS | K_SHARE | K_SHORTTEXT | K_SMALLDECIMAL | K_SMALLINT
    | K_SOME | K_SORT | K_SPECIFIED | K_STATIC | K_STDDEV | K_STDDEV_POP | K_STDDEV_SAMP | K_STRICT | K_STRING_AGG
    | K_STRUCTURED | K_SUBSTR_REGEXPR | K_SUBSTRING_REGEXPR | K_SUBTOTAL | K_SUM | K_SYSTEM | K_SYSTEM_TIME | K_TABLE
    | K_TEXT | K_TEXT_FILTER | K_THEN | K_TIME | K_TIMESTAMP | K_TINYINT | K_TO | K_TOTAL | K_TRIM | K_UNBOUNDED
    | K_UNCONDITIONAL | K_UNNEST | K_UP | K_UPDATE | K_UPSERT | K_USER | K_UTF16 | K_UTF32 | K_UTF8 | K_VALUE | K_VAR
    | K_VAR_POP | K_VAR_SAMP | K_VARBINARY | K_VARCHAR | K_VIEW | K_WAIT | K_WEIGHT | K_WEIGHTED_AVG | K_WITHIN
    | K_WITHOUT | K_WRAPPER | K_XML | K_XMLNAMESPACE | K_XMLTABLE | K_YEAR | K_ZERO_LAG
    ;
