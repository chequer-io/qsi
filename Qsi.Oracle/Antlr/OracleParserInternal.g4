parser grammar OracleParserInternal;

options { 
    tokenVocab=OracleLexerInternal;
}

root
    : select
    | delete
    | create
//    | savepoint
//    | rollback
    ;

select
    : subquery forUpdateClause?
    ;

delete
    : DELETE hint? FROM? (dmlTableExpressionClause | ONLY '(' dmlTableExpressionClause ')')
      tAlias? whereClause? returningClause? errorLoggingClause?
    ;

create
    : createAnalyticView
    | createAttributeDimension
    ;

createAnalyticView
    : CREATE (OR REPLACE)? (FORCE | NOFORCE)? ANALYTIC VIEW 
      analyticViewName=identifier
      sharingClause? 
      classificationClause* 
      usingClause? 
      dimByClause? 
      measuresClause? 
      defaultMeasureClause? 
      defaultAggregateClause? 
      cacheClause? 
      factColumnsClause? 
      qryTransformClause?
    ;

createAttributeDimension
    : CREATE (OR REPLACE)? (FORCE | NOFORCE)? ATTRIBUTE DIMENSION
      (schema '.')? attrDimension=identifier
      sharingClause?
      classificationClause*
      (DIMENSION TYPE (STANDARD | TIME))?
      attrDimUsingClause
      attributesClause
      attrDimLevelClause*
      allClause?
    ;

sharingClause
    : SHARING '=' (METADATA | DATA | NONE)
    ;

classificationClause
    : CAPTION caption=stringLiteral
    | DESCRIPTION description=stringLiteral
    | CLASSIFICATION classificationName=identifier 
      (VALUE classificationValue=stringLiteral)? 
      (LANGUAGE language=stringLiteral)?
    ;

usingClause
    : USING sourceClause
    ;

sourceClause
    : (schema '.')? factTableOrView=table REMOTE? (AS? alias)?
    ;

dimByClause
    : DIMENSION BY '(' dimKey (',' dimKey)* ')'
    ;

dimKey
    : dimRef classificationClause* KEY
      (
          '('? (alias '.')? factColumn=column ')'? 
        | '(' (alias '.')? factColumn=column (',' (alias '.')? factColumn=column)* ')'
      ) 
      REFERENCES (DISTINCT? '('? attribute ')'? | '(' attribute (',' attribute)* ')')
      HIERARCHIES '(' hierRef (',' hierRef)* ')'
    ;

dimRef
    : (schema '.')? attrDimName=identifier (AS? dimAlias=alias)?
    ;

hierRef
    : (schema '.')? hierName=identifier (AS? hierAlias=alias)? DEFAULT?
    ;

measuresClause
    : MEASURES '(' avMeasure (',' avMeasure)* ')'
    ;

avMeasure
    : measName=identifier (baseMeasureClause | calcMeasureClause)? classificationClause*
    ;

baseMeasureClause
    : (FACT (alias '.')?)? column measAggregateClause?
    ;

// TODO: Impl avExpression
calcMeasureClause
    : AS '(' 
//      calcMeasExpression=avExpression
     ')'
    ;

defaultMeasureClause
    : DEFAULT MEASURE measure=identifier
    ;

defaultAggregateClause
    : DEFAULT AGGREGATE BY 
//    aggrFunction
    ;

cacheClause
    : CACHE cacheSpecification (',' cacheSpecification)*
    ;

cacheSpecification
    : MEASURE GROUP (ALL | '(' measureName=identifier (',' measureName=identifier)* ')' (levelsClause MATERIALIZED)+)
    ;

levelsClause
    : LEVELS '(' levelSpecification (',' levelSpecification)* ')'
    ;

levelSpecification
    : '(' ((dimName=identifier '.')? hierName=identifier '.')? levelName=identifier ')'
    ;

factColumnsClause
    : FACT COLUMNS factColumn=column (AS factAlias=alias)? (',' AS factAlias=alias)*
    ;

qryTransformClause
    : ENABLE QUERY TRANSFORM (RELY | NORELY)?
    ;

attrDimUsingClause
    : USING sourceClause (',' sourceClause)* joinPathClause*
    ;

joinPathClause
    : JOIN PATH joinPathName=identifier ON joinCondition
    ;

joinCondition
    : joinConditionElem (AND joinConditionElem)*
    ;

joinConditionElem
    : CODE (alias '.')? column '=' (alias '.')? column
    ;

attributesClause
    : ATTRIBUTES '(' attrDimAttributeClause (',' attrDimAttributeClause)* ')'
    ;

attrDimAttributeClause
    : (alias '.')? column (AS? attributeName=identifier)? classificationClause*
    ;

attrDimLevelClause
    : LEVEL level=identifier
      (NOT NULL | K_SKIP WHEN NULL)?
      (classificationClause* 
          (
              LEVEL TYPE 
              (
                  STANDARD 
                | YEARS 
                | HALF_YEARS 
                | QUARTERS 
                | MONTHS 
                | WEEKS 
                | DAYS 
                | HOURS 
                | MINUTES 
                | SECONDS
              )
          )?
       keyClause
       alternateKeyClause? 
       (MEMBER NAME expr)? 
       (MEMBER CAPTION expr)?
       (MEMBER DESCRIPTION expr)?
       (ORDER BY (MIN | MAX)? dimOrderClause (',' (MIN | MAX)? dimOrderClause)*)?
      )?
      (DETERMINES '(' attribute (',' attribute)* ')')?
    ;

keyClause
    : KEY ('('? attribute ')'? | '(' attribute (',' attribute)* ')')
    ;

alternateKeyClause
    : ALTERNATE KEY ('('? attribute ')'? | '(' attribute (',' attribute)* ')')
    ;

dimOrderClause
    : attribute (ASC | DESC)? (NULLS (FIRST | LAST))?
    ;

allClause
    : ALL MEMBER
      ( 
        NAME expr (MEMBER CAPTION expr)? 
      | CAPTION expr (MEMBER DESCRIPTION expr)? 
      | DESCRIPTION expr
      )
    ;

//savepoint
//    : SAVEPOINT savepointName
//    ;
//
//rollback
//    : ROLLBACK WORK? (TO SAVEPOINT savepointName | FORCE string)?
//    ;

subquery
    : queryBlock orderByClause? rowOffset? rowFetchOption?
    | subquery ((UNION ALL? | INTERSECT | MINUS) subquery)+ orderByClause? rowOffset? rowFetchOption?
    | '(' subquery ')' orderByClause? rowOffset? rowFetchOption?
    ;

orderByClause
    : ORDER SIBLINGS? BY items+=orderByItem (',' items+=orderByItem)*
    ;

orderByItem
    : (expr | position=integer | cAlias) (ASC | DESC)? (NULLS FIRST | NULLS LAST)?
    ;

dmlTableExpressionClause
    : (table ('.' schema)? (partitionExtensionClause | '@' dblink)? 
    | (view | materializedView) ('@' dblink)?) 
    | '(' subquery subqueryRestrictionClause? ')' 
    | tableCollectionExpression
    ;

returningClause
    : (RETURN | RETURNING) expr (',' expr)* INTO dataItem (',' dataItem)*
    ;

dataItem
    : variableName
    | ':' variableName
    ;

errorLoggingClause
    : LOG ERRORS 
      (INTO (schema '.')? table)?
      ('(' simpleExpression ')')?
      (REJECT LIMIT (integer | UNLIMITED))?
    ;


rowOffset
    : OFFSET offset=expr (ROW | ROWS)
    ;

rowFetchOption
    : FETCH (FIRST | NEXT)
      (rowcount=expr | percent=expr PERCENT)?
      (ROW | ROWS)
      (ONLY | WITH TIES)
    ;

forUpdateClause
    : FOR UPDATE (OF fullColumnPath (',' fullColumnPath)*)? (NOWAIT | WAIT S_INTEGER_WITHOUT_SIGN | K_SKIP LOCKED)?
    ;

queryBlock
    : withClause?
      SELECT hint? queryBehavior? selectList FROM
      tables+=tableSource (',' tables+=tableSource)*
      whereClause?
      hierarchicalQueryClause?
      groupByClause?
      modelClause?
      windowClause?
    ;

withClause
    : WITH
//      plsqlDeclarations?
      clauses+=factoringClause (',' clauses+=factoringClause)?
    ;

factoringClause
    : subqueryFactoringClause 
    | subavFactoringClause
    ;

subqueryFactoringClause
    : identifier columnList? AS '(' subquery ')' searchClause? cycleClause?
    ;

columnList
    : '(' (columns+=identifier (',' columns+=identifier)*)? ')'
    ;

searchClause
    : SEARCH (DEPTH | BREADTH)
      FIRST BY identifier (',' identifier)* (ASC | DESC)? (NULLS FIRST | NULLS LAST)?
      SET orderingColumn=identifier
    ;

cycleClause
    : CYCLE cAlias (',' cAlias)*
      SET cycleMarkCAlias=identifier
      TO cycleValue=SINGLE_QUOTED_STRING {isCycleValue()}?
      DEFAULT noCycleValue=SINGLE_QUOTED_STRING {isCycleValue()}?
    ;

subavFactoringClause
    : subavName=fullObjectPath ANALYTIC VIEW AS '(' subAvClause ')'
    ;

subAvClause
    : USING identifier ('.' identifier)? hierarchiesClause? filterClauses? addMeasClause?
    ;

filterClauses
    : FILTER FACT '(' filterClause (',' filterClause)* ')'
    ;

// TODO: predicate is expr?
filterClause
    : hierIds TO predicate=expr
    ;

addMeasClause
    : ADD MEASURES '(' cubeMeas (',' cubeMeas)*  ')'
    ;

// TODO: Check
cubeMeas
    : measName=identifier ( baseMeasClause | calcMeasClause )
    ;

// TODO: Check
baseMeasClause
    : FACT FOR MEASURE baseMeas=identifier measAggregateClause
    ;

measAggregateClause
    : AGGREGATE BY 
//    aggrFunction
    ;

// TODO: Check
calcMeasClause
    : measName=identifier AS '(' expr ')'
    ;


// ** HINT **
// /*+ ALL_ROWS */
// /*+ APPEND */

hint
    : '/*+' (hintItem stringLiteral?|.+?)+ '*/'
    | '--+' (hintItem stringLiteral?)+
    ;

hintItem
    : ALL_ROWS                                                                      #allRowsHint
    | APPEND                                                                        #appendHint
    | APPEND_VALUES                                                                 #appendValuesHint
    | CACHE '(' hintQueryBlockName tablespec? ')'                                   #cacheHint
    | CHANGE_DUPKEY_ERROR_INDEX 
      '(' identifier ( '.' identifier | '(' identifier ('.' identifier)* ')' ) ')'  #changeDupkeyErrorIndexHint
    | CLUSTER '(' hintQueryBlockName? tablespec ')'                                 #clusterHint
    | CLUSTERING                                                                    #clusteringHint
    | CONTAINERS '(' DEFAULT_PDB_HINT '=' SINGLE_QUOTED_STRING ')'                  #containersHint
    | CURSOR_SHARING_EXACT                                                          #cursorSharingExactHint
    | DISABLE_PARALLEL_DML                                                          #disableParallelDmlHint
    | DRIVING_SITE                                                                  #drivingSiteHint
    | DYNAMIC_SAMPLING '(' hintQueryBlockName? tablespec? integer ')'               #dynamicSamplingHint
    | ENABLE_PARALLEL_DML                                                           #enableParallelDmlHint
    | FACT '(' hintQueryBlockName? tablespec ')'                                    #factHint
    | FIRST_ROWS '(' integer ')'                                                    #firstRowsHint
    | FRESH_MV                                                                      #freshMvHint
    | FULL '(' hintQueryBlockName? tablespec ')'                                    #fullHint
    | GATHER_OPTIMIZER_STATISTICS                                                   #gatherOptimizerStatisticsHint
    | GROUPING                                                                      #groupingHint
    | HASH '(' hintQueryBlockName? tablespec ')'                                    #hashHint
    | IGNORE_ROW_ON_DUPKEY_INDEX 
      '(' (table '.' index| table '(' column (',' column)* ')') ')'                 #ignoreRowOnDupkeyIndexHint
    | INDEX '(' hintQueryBlockName? tablespec indexspec* ')'                        #indexHint
    | INDEX_ASC '(' hintQueryBlockName? tablespec indexspec* ')'                    #indexAscHint
    | INDEX_COMBINE '(' hintQueryBlockName? tablespec indexspec* ')'                #indexCombineHint
    | INDEX_DESC '(' hintQueryBlockName? tablespec indexspec* ')'                   #indexDescHint
    | INDEX_FFS '(' hintQueryBlockName? tablespec indexspec* ')'                    #indexFfsHint
    | INDEX_JOIN '(' hintQueryBlockName? tablespec indexspec* ')'                   #indexJoinHint
    | INDEX_SS '(' hintQueryBlockName? tablespec indexspec* ')'                     #indexSsHint
    | INDEX_SS_ASC '(' hintQueryBlockName? tablespec indexspec* ')'                 #indexSsAscHint
    | INDEX_SS_DESC '(' hintQueryBlockName? tablespec indexspec* ')'                #indexSsDescHint
    | INMEMORY '(' hintQueryBlockName? tablespec ')'                                #inmemoryHint
    | INMEMORY_PRUNING '(' hintQueryBlockName? tablespec ')'                        #inmemoryPruningHint
    | LEADING '(' hintQueryBlockName? tablespec* ')'                                #leadingHint
    | MERGE ('(' (hintQueryBlockName |  hintQueryBlockName? tablespec) ')')?        #mergeHint
    | MODEL_MIN_ANALYSIS                                                            #modelMinAnalysisHint
    | MONITOR                                                                       #monitorHint
    | NATIVE_FULL_OUTER_JOIN                                                        #nativeFullOuterJoinHint
    | NOAPPEND                                                                      #noappendHint
    | NOCACHE '(' hintQueryBlockName? tablespec ')'                                 #nocacheHint
    | NO_CLUSTERING                                                                 #noClusteringHint
    | NO_EXPAND ('(' hintQueryBlockName ')')?                                       #noExpandHint
    | NO_FACT '(' hintQueryBlockName? tablespec ')'                                 #noFactHint
    | NO_GATHER_OPTIMIZER_STATISTICS                                                #noGatherOptStatsHint
    | NO_INDEX '(' hintQueryBlockName? tablespec indexspec* ')'                     #noIndexHint
    | NO_INDEX_FFS '(' hintQueryBlockName? tablespec indexspec* ')'                 #noIndexFfsHint
    | NO_INDEX_SS '(' hintQueryBlockName? tablespec indexspec* ')'                  #noIndexSsHint
    | NO_INMEMORY '(' hintQueryBlockName? tablespec ')'                             #noInmemoryHint
    | NO_INMEMORY_PRUNING '(' hintQueryBlockName? tablespec ')'                     #noInmemoryPruningHint
    | NO_MERGE 
      ('(' hintQueryBlockName ')' | '(' hintQueryBlockName? tablespec ')')?         #noMergeHint
    | NO_MONITOR                                                                    #noMonitorHint
    | NO_NATIVE_FULL_OUTER_JOIN                                                     #noNativeFullOuterJoinHint
    | NO_PARALLEL '(' hintQueryBlockName? tablespec ')'                             #noParallelHint
    | NO_PARALLEL_INDEX '(' hintQueryBlockName? tablespec indexspec* ')'            #noParallelIndexHint
    | NO_PQ_CONCURRENT_UNION ('(' hintQueryBlockName ')')?                          #noPqConcurrentUnionHint
    | NO_PQ_SKEW '(' hintQueryBlockName? tablespec ')'                              #noPqSkewHint
    | NO_PUSH_PRED 
      ('(' hintQueryBlockName ')' | '(' hintQueryBlockName? tablespec ')')?         #noPushPredHint
    | NO_PUSH_SUBQ ('(' hintQueryBlockName ')')?                                    #noPushSubqHint
    | NO_PX_JOIN_FILTER '(' tablespec ')'                                           #noPxJoinFilterHint
    | NO_QUERY_TRANSFORMATION                                                       #noQueryTransformatnHint
    | NO_RESULT_CACHE                                                               #noResultCacheHint
    | NO_REWRITE ('(' hintQueryBlockName ')')?                                      #noRewriteHint
    | NO_STAR_TRANSFORMATION ('(' hintQueryBlockName ')')?                          #noStarTransformationHint
    | NO_STATEMENT_QUEUING                                                          #noStatementQueuingHint
    | NO_UNNEST ('(' hintQueryBlockName ')')?                                       #noUnnestHint
    | NO_USE_BAND '(' hintQueryBlockName? tablespec* ')'                            #noUseBandHint
    | NO_USE_CUBE '(' hintQueryBlockName? tablespec* ')'                            #noUseCubeHint
    | NO_USE_HASH '(' hintQueryBlockName? tablespec* ')'                            #noUseHashHint
    | NO_USE_MERGE '(' hintQueryBlockName? tablespec* ')'                           #noUseMergeHint
    | NO_USE_NL '(' hintQueryBlockName? tablespec* ')'                              #noUseNlHint
    | NO_XML_QUERY_REWRITE                                                          #noXmlQueryRewriteHint
    | NO_XMLINDEX_REWRITE                                                           #noXmlindexRewriteHint
    | NO_ZONEMAP '(' hintQueryBlockName? tablespec (SCAN | JOIN | PARTITION) ')'    #noZonemapHint
//    | optimizerFeaturesEnableHint
//    | OPT_PARAM '(' parameterName ','? parameterValue ')'                           #optParamHint
    | ORDERED                                                                       #orderedHint
    // TODO: Impl
//    | parallelHint
    | PARALLEL_INDEX 
      '(' hintQueryBlockName? tablespec indexspec* (integer | DEFAULT)? ')'         #parallelIndexHint
    | PQ_CONCURRENT_UNION ('(' hintQueryBlockName ')')?                             #pqConcurrentUnionHint
    // TODO: Impl
//    | PQ_DISTRIBUTE 
//      '(' hintQueryBlockName? tablespec
//      (distribution | outerDistribution innerDistribution) ')'                      #pqDistributeHint
    | PQ_FILTER '(' (SERIAL | NONE | HASH | RANDOM) ')'                             #pqFilterHint
    | PQ_SKEW '(' hintQueryBlockName? tablespec ')'                                 #pqSkewHint
    | PUSH_PRED 
      ('(' hintQueryBlockName ')' | '(' hintQueryBlockName? tablespec ')')?         #pushPredHint
    | PUSH_SUBQ ('(' hintQueryBlockName ')')?                                       #pushSubqHint
    | PX_JOIN_FILTER '(' tablespec ')'                                              #pxJoinFilterHint
    | QB_NAME '(' UNQUOTED_OBJECT_NAME ')'                                          #qbNameHint
    | RESULT_CACHE (TEMP '=' (TRUE | FALSE))?                                       #resultCacheHint
    | RETRY_ON_ROW_CHANGE                                                           #retryOnRowChangeHint
    | REWRITE ('(' hintQueryBlockName? view* ')')?                                  #rewriteHint
    | STAR_TRANSFORMATION ('(' hintQueryBlockName ')')?                             #starTransformationHint
    | STATEMENT_QUEUING                                                             #statementQueuingHint
    | UNNEST ('(' hintQueryBlockName ')')?                                          #unnestHint
    | USE_BAND '(' hintQueryBlockName? tablespec* ')'                               #useBandHint
    | USE_CONCAT ('(' hintQueryBlockName ')')?                                      #useConcatHint
    | USE_CUBE '(' hintQueryBlockName? tablespec* ')'                               #useCubeHint
    | USE_HASH '(' hintQueryBlockName? tablespec* ')'                               #useHashHint
    | USE_MERGE '(' hintQueryBlockName? tablespec* ')'                              #useMergeHint
    | USE_NL '(' hintQueryBlockName? tablespec* ')'                                 #useNlHint
    | USE_NL_WITH_INDEX '(' hintQueryBlockName? tablespec indexspec* ')'            #useNlWithIndexHint
    ;

hintQueryBlockName
    : '@' UNQUOTED_OBJECT_NAME
    ;

queryBehavior
    : DISTINCT
    | UNIQUE
    | ALL
    ;

selectList
    : '*'
    | selectListItem (',' selectListItem)*
    ;

selectListItem
    : identifier ('.' identifier)? '.' '*'                   #objectSelectListItem
    | expr (AS? alias)?                                      #exprSelectListItem
    ;

tableSource
    : tableReference
    | joinClause
    | '(' joinClause ')'
    | inlineAnalyticView 
    ;

tableReference
    : (
          (
              ( ONLY '(' queryTableExpression ')' | queryTableExpression ) 
              flashbackQueryClause?
              (pivotClause | unpivotClause | rowPatternClause)?
          )
        | containersClause
        | shardsClause
      ) 
      tAlias?
    ;

joinClause
    : tableReference 
        (
            innerCrossJoinClause
          | outerJoinClause
          | crossOuterApplyClause
        )*
    ;

innerCrossJoinClause
    : INNER? JOIN tableReference (ON condition | USING '(' column (',' column)* ')') 
    | (CROSS | NATURAL INNER?) JOIN tableReference
    ;

outerJoinClause
    : queryPartitionClause? 
      NATURAL? 
      outerJoinType JOIN tableReference queryPartitionClause? 
      (ON condition | USING '(' column (',' column)* ')')?
    ;

queryPartitionClause
    : PARTITION BY queryPartitionExpressions
    ;

queryPartitionExpressions
    : expr (',' expr)* 
    | '(' expr (',' expr)* ')'
    ;

outerJoinType
    : (FULL | LEFT | RIGHT) OUTER?
    ;

crossOuterApplyClause
    : (CROSS | OUTER) APPLY (tableReference | collectionExpression)
    ;

inlineAnalyticView
    : ANALYTIC VIEW subAvClause (AS? inlineAvAlias=identifier)?
    ;

queryTableExpression
    : queryName=fullObjectPath
    | identifier? 
        ( identifier (partitionExtensionClause | '@' dblink)?
          analyticView=identifier hierarchiesClause?
          hierarchy=identifier
        ) sampleClause?
    | LATERAL? '(' subquery 
    subqueryRestrictionClause? 
    ')'
    | tableCollectionExpression
    ;

flashbackQueryClause
    : VERSIONS 
        (BETWEEN (SCN | TIMESTAMP) (expr | MINVALUE) AND (expr | MAXVALUE) 
        | PERIOD FOR validTimeColumn=column BETWEEN (expr | MINVALUE) AND (expr | MAXVALUE)) 
    | AS OF 
        ((SCN | TIMESTAMP) expr 
        | AS OF PERIOD FOR validTimeColumn=column expr)
    ;

pivotClause
    : PIVOT XML? 
      '(' pivotItem (',' pivotItem)* pivotForClause pivotInClause ')'
    ;

pivotItem
    : 
//    aggregateFunction
     '(' expr ')' (AS? alias)?
    ;

unpivotClause
    : UNPIVOT ((INCLUDE | EXCLUDE) NULLS)?
      '(' (column | '(' column (',' column)* ')') pivotForClause unpivotInClause ')'
    ;

unpivotInClause
    : IN '(' unpivotInItem (',' unpivotInItem)* ')'
    ;

unpivotInItem
    : (column | '(' column (',' column)* ')') 
      (AS (literal | '(' literal (',' literal)* ')'))?
    ;

rowPatternClause
    : MATCH_RECOGNIZE '(' 
      rowPatternPartitionBy? 
      rowPatternOrderBy? 
      rowPatternMeasures? 
      rowPatternRowsPerMatch? 
      rowPatternSkipTo? 
      PATTERN '(' rowPattern ')' 
      rowPatternSubsetClause? DEFINE rowPatternDefinitionList ')'
    ;

rowPatternPartitionBy
    : PARTITION BY column (',' column)*
    ;

rowPatternOrderBy
    : ORDER BY column (',' column)*
    ;

rowPatternMeasures
    : MEASURES rowPatternMeasureColumn (',' rowPatternMeasureColumn)*
    ;

rowPatternMeasureColumn
    : expr AS cAlias
    ;

rowPatternRowsPerMatch
    : ONE ROW PER MATCH | ALL ROWS PER MATCH
    ;

rowPatternSkipTo
    : AFTER MATCH 
       ( 
           K_SKIP TO NEXT ROW
         | K_SKIP PAST LAST ROW 
         | K_SKIP TO FIRST variableName 
         | K_SKIP TO LAST variableName 
         | K_SKIP TO variableName
       )
    ;

rowPattern
    : rowPatternTerm ('|' rowPatternTerm)*
    ;

rowPatternTerm
    : rowPatternFactor+
    ;

rowPatternFactor
    : rowPatternPrimary rowPatternQuantifier?
    ;

rowPatternPrimary
    : variableName 
    | '$' 
    | '^' 
    | '(' rowPattern? ')' 
    | '{-' rowPattern '-}' 
    | rowPatternPermute
    ;

rowPatternPermute
    : PERMUTE '(' rowPattern (',' rowPattern)* ')'
    ;

rowPatternQuantifier
    : '*' '?'? 
    | '+' '?'? 
    | '?' '?'? 
    | '{' S_INTEGER_WITHOUT_SIGN? ',' S_INTEGER_WITHOUT_SIGN? '}' '?'? 
    | '{' S_INTEGER_WITHOUT_SIGN '}'
    ;

rowPatternSubsetClause
    : SUBSET rowPatternSubsetItem (',' rowPatternSubsetItem)*
    ;

rowPatternSubsetItem
    : variableName '=' '(' variableName (',' variableName)? ')'
    ;

rowPatternDefinitionList
    : rowPatternDefinition (',' rowPatternDefinition)*
    ;

rowPatternDefinition
    : variableName AS condition
    ;

rowPatternRecFunc
    : rowPatternClassifierFunc 
    | rowPatternMatchNumFunc 
    | rowPatternNavigationFunc 
    | rowPatternAggregateFunc
    ;

rowPatternClassifierFunc
    : CLASSIFIER '(' ')'
    ;

rowPatternMatchNumFunc
    : MATCH_NUMBER '(' ')'
    ;

rowPatternNavigationFunc
    : rowPatternNavLogical 
    | rowPatternNavPhysical 
    | rowPatternNavCompound
    ;

rowPatternNavLogical
    : (RUNNING | FINAL)? (FIRST | LAST) '(' expr (',' offset=expr)? ')'
    ;

rowPatternNavPhysical
    : (PREV | NEXT) '(' expr (',' offset=expr)? ')'
    ;

rowPatternNavCompound
    : (PREV | NEXT) '(' (RUNNING | FINAL)? (FIRST | LAST) '(' expr (',' offset=expr)? ')' (',' offset=expr)? ')'
    ;

rowPatternAggregateFunc
    : (RUNNING | FINAL)? 
//    aggregateFunction
    ;

containersClause
    : CONTAINERS '(' (schema '.')? (table | view) ')'
    ;

shardsClause
    : SHARDS '(' (schema '.')? (table | view) ')'
    ;

pivotForClause
    : FOR (column | '(' column (',' column)* ')')
    ;

pivotInClause
    : IN '(' 
          (
            ((expr | '(' expr (',' expr)* ')') (AS? alias)?)* 
          | subquery 
          | ANY (',' ANY)*
          )
      ')'
    ;

partitionExtensionClause
    : PARTITION 
          ( '(' partition=identifier ')' 
          | FOR '(' partitionKeyValue=expr (',' partitionKeyValue=expr)* ')') 
    | SUBPARTITION
          ('(' subpartition=identifier ')' 
          | FOR '(' subpartitionKeyValue=expr (',' subpartitionKeyValue=expr)* ')')
    ;

sampleClause
    : SAMPLE BLOCK? '(' samplePercent=integer ')' (SEED '(' seedValue=integer ')')?
    ;

subqueryRestrictionClause
    : WITH (READ ONLY | CHECK OPTION) (CONSTRAINT constraint)?
    ;

tableCollectionExpression
    : TABLE '(' collectionExpression ')' ('(' '+' ')')?
    ;

collectionExpression
    : subquery
    | column
//    | function
//    | collectionConstructor
    ;

hierarchiesClause
    : HIERARCHIES '(' (hierIds)? ')'
    ;

hierIds
    : hierId (',' hierId)*
    ;

hierId
    : MEASURES
    | identifier ('.' identifier)?
    ;

whereClause
    : WHERE condition
    ;

hierarchicalQueryClause
    : CONNECT BY NOCYCLE? condition (START WITH condition)? #connectByHierarchicalQueryClause
    | START WITH condition CONNECT BY NOCYCLE? condition    #startWithHierarchicalQueryClause
    ;

groupByClause
    : GROUP BY groupByItem (',' groupByItem)* (HAVING condition)?
    ;

groupByItem
    : (expr | rollupCubeClause | groupingSetsClause)
    ;

rollupCubeClause
    : (ROLLUP | CUBE) '(' groupingExpressionList ')'
    ;

groupingSetsClause
    : GROUPING SETS '(' (rollupCubeClause | groupingExpressionList) ')'
    ;

groupingExpressionList
    : expressionList (',' expressionList)*
    ;

expressionList
    : (expr (',' expr)* | '(' (expr (',' expr)?)* ')')
    ;

modelClause
    : MODEL cellReferenceOptions returnRowsClause? referenceModel* mainModel
    ;

cellReferenceOptions
    : ((IGNORE | KEEP) NAV)? (UNIQUE (DIMENSION | SINGLE REFERENCE))?
    ;

returnRowsClause
    : RETURN (UPDATED | ALL) ROWS
    ;

referenceModel
    : REFERENCE referenceModelName=identifier ON '(' subquery ')' modelColumnClauses cellReferenceOptions
    ;

modelColumnClauses
    : (PARTITION BY '(' modelColumnItems ')')? 
      DIMENSION BY '(' modelColumnItems ')' 
      MEASURES '(' modelColumnItems ')'
    ;

modelColumnItem
    : expr cAlias?
    ;

modelColumnItems
    : modelColumnItem (',' modelColumnItem)*
    ;

mainModel
    : (MAIN mainModelName=identifier)? modelColumnClauses cellReferenceOptions modelRulesClause
    ;

modelRulesClause
    : (RULES 
          (UPDATE | UPSERT ALL?)? 
          ((AUTOMATIC | SEQUENTIAL) ORDER)? 
          modelIterateClause?
      )? 
      '(' modelRulesItem (',' modelRulesItem)* ')'
    ;

modelRulesItem
    : (UPDATE | UPSERT ALL?)? cellAssignment orderByClause? '=' expr
    ;

modelIterateClause
    : ITERATE '(' numberLiteral ')' (UNTIL '(' condition ')')?
    ;

cellAssignment
    : measureColumn=cAlias '[' (cellAssignmentItem (',' cellAssignmentItem)* | multiColumnForLoop) ']'
    ;

cellAssignmentItem
    : condition
    | expr 
    | singleColumnForLoop
    ;

singleColumnForLoop
    : FOR dimensionColumn=cAlias (IN '(' (literal (',' literal)* | subquery) ')' | (LIKE pattern=stringLiteral)? FROM literal TO literal (INCREMENT | DECREMENT) literal)
    ;

multiColumnForLoop
    : FOR '(' dimensionColumn=cAlias (',' dimensionColumn=cAlias)* ')' IN '(' ('(' literal (',' literal)* ')' ('(' literal (',' literal)* ')')* | subquery) ')'
    ;

windowClause
    : WINDOW windowClauseItem (',' windowClauseItem)*
    ;

windowClauseItem
    : windowName=identifier AS existingWindowName=identifier? queryPartitionClause? orderByClause? windowingClause?
    ;

windowingClause
    : ( ROWS | RANGE )
      ( BETWEEN
        ( UNBOUNDED PRECEDING
        | CURRENT ROW
        | value_expr=expr ( PRECEDING | FOLLOWING )
        ) 
        AND
        ( UNBOUNDED FOLLOWING
        | CURRENT ROW
        | value_expr=expr ( PRECEDING | FOLLOWING )
        )
      | ( UNBOUNDED PRECEDING
        | CURRENT ROW
        | value_expr=expr PRECEDING
        )
      )
    ;

constraint
    :
//    : inlineConstraint
//    | outOfLineConstraint
//    | inlineRefConstraint
//    | outOfLineRefConstraint
    ;

condition
    : comparisonCondition
//    | floatingPointCondition
//    | logicalCondition
//    | modelCondition
//    | multisetCondition
//    | patternMatchingCondition
//    | rangeCondition
//    | nullCondition
//    | xmlCondition
//    | jsonCondition
//    | compoundCondition
//    | existsCondition
//    | inCondition
//    | isOfTypeCondition
    ;

comparisonCondition
    : simpleComparisonCondition
    ;

simpleComparisonCondition
    : expr ( '=' | '!=' | '^=' | '<>' | '>' | '<' | '>=' | '<=' ) expr
    ;

expr
    : simpleExpression                          #simpleExpr
    | '(' expr ')'                              #parenthesisExpr
    | ('+' | '-'| PRIOR) expr                   #signExpr
    | expr ( '*' | '/' | '+' | '-' | '||') expr #binaryExpr
    | expr COLLATE collationName=identifier     #collateExpr
//    | calcMeasExpression 
    | caseExpression                            #caseExpr
//    | cursorExpression 
//    | datetimeExpression 
//    | functionExpression 
//    | intervalExpression 
//    | jsonObjectAccessExpr 
//    | modelExpression 
//    | objectAccessExpression 
//    | scalarSubqueryExpression 
//    | typeConstructorExpression 
//    | variableExpression
    ;

simpleExpression
    : ((schema '.')? table '.')? (column | ROWID)
    | ROWNUM
    | stringLiteral 
    | numberLiteral 
    | sequence '.' (CURRVAL | NEXTVAL) 
    | NULL
    ;

//cursorExpression
//    : CURSOR '(' subquery ')'
//    ;

caseExpression
    : CASE (simpleCaseExpression | searchedCaseExpression) elseClause? END
    ;

//datetimeExpression
//    : expr AT ( LOCAL | TIME ZONE
//        ( SINGLE_QUOTE ('+'|'-') hh ':' mi SINGLE_QUOTE
//        | DBTIMEZONE
//        | SESSIONTIMEZONE
//        | SINGLE_QUOTE timeZoneName SINGLE_QUOTE
//        | expr
//        )
//      )
//    ;
//
//functionExpression
//    : function
//    ;
//
//intervalExpression
//    : '(' expr '-' expr ')' 
//      ( DAY ('(' leadingFieldPrecision ')')? TO SECOND ('(' fractionalSecondPrecision ')')? 
//      | YEAR ('(' leadingFieldPrecision ')')? TO MONTH
//      )
//    ;
//
//jsonObjectAccessExpr
//    : tableAlias '.' jsonColumn ('.' jsonObjectKey arrayStep*)+? 
//    ;
//
//arrayStep
//    : '[' (arrayStepItem (',' arrayStepItem)* | '*') ']'
//    ;
//
//arrayStepItem
//    : ( integer (TO integer)? )
//    ;
//
//function
//    : aggregateFunction
//    | analyticFunction
//    | objectReferenceFunction
//    | modelFunction
//    | userDefinedFunction
//    | olapFunction
//    | dataCartridgeFunction
//    ;
//
//aggregateFunction
//    : 
//    ;
//
simpleCaseExpression
    : expr (WHEN comparisonExpr=expr THEN returnExpr=expr)+
    ;

searchedCaseExpression
    : (WHEN condition THEN returnExpr=expr)+
    ;

elseClause
    : ELSE elseExpr=expr
    ;

fullColumnPath
    : identifier ('.' identifier ('.' identifier)?)?
    ;

fullObjectPath
    : identifier ('.' identifier)?
    ;

variableName
    : identifier
    ;

dblink
    : identifier
    ;

attribute
    : identifier
    ;

alias
    : identifier
    ;

tAlias // table alias
    : identifier
    ;

cAlias // column alias
    : identifier
    ;

sequence
    : identifier
    ;

table
    : identifier
    ;

schema
    : identifier
    ;

materializedView
    : identifier
    ;

view
    : identifier
    ;

index
    : identifier
    ;

column
    : identifier
    ;

tablespec
    : identifier ('.' identifier)*
    ;
    
indexspec
    : identifier
    | '(' ((identifier '.')* identifier)+ ')'
    ;

integer
    : S_INTEGER_WITH_SIGN
    | S_INTEGER_WITHOUT_SIGN
    ;

literal
    : intervalLiteral
    | numberLiteral
    | stringLiteral
    | dateTimeLiteral
    ;

numberLiteral
    : S_NUMBER_WITH_SIGN
    | S_INTEGER_WITH_SIGN
    | S_INTEGER_WITHOUT_SIGN
    | S_NUMBER_WITHOUT_SIGN
    ;

// '…'
// Q'…'
// N'…'
// NQ'…'
stringLiteral
    : SINGLE_QUOTED_STRING
    | v=QUOTED_STRING     { validateStringLiteral($v.text) }?
    | v=NATIONAL_STRING   { validateStringLiteral($v.text) }?
    ;

dateTimeLiteral
    : DATE SINGLE_QUOTED_STRING         #dateLiteral
    | TIMESTAMP SINGLE_QUOTED_STRING    #timestampLiteral
    ;

intervalLiteral
    : INTERVAL SINGLE_QUOTED_STRING (YEAR|MONTH) ('(' precision=expr ')')? (TO (YEAR|MONTH))?
    ;

identifier
    : UNQUOTED_OBJECT_NAME
    | QUOTED_OBJECT_NAME
    ;