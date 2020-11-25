parser grammar CqlParserInternal;

options { 
    tokenVocab=CqlLexerInternal;
}

@header {
    using Qsi.Data;
    using Qsi.Tree;
    using Qsi.Cql.Tree;
    using Qsi.Cql.Schema;
}

root
    : cqlStatements? MINUSMINUS? EOF
    ;

cqlStatements
    : (cqlStatement MINUSMINUS? SEMI? | emptyStatement)*
    (cqlStatement (MINUSMINUS? SEMI)? | emptyStatement)
    ;

emptyStatement
    : SEMI
    ;

cqlStatement
    : selectStatement
    | insertStatement
    | updateStatement
    | deleteStatement
    | createMaterializedViewStatement
    ;

/**
 * SELECT [JSON] [DISTINCT] <expression>
 * FROM <CF>
 * [ WHERE KEY = "key1" AND COL > 1 AND COL < 100 ]
 * [ PER PARTITION LIMIT <NUMBER> ]
 * [ LIMIT <NUMBER> ]
 * [ ALLOW FILTERING ]
 */
selectStatement returns [
    bool json,
    bool distinct,
    bool allowFiltering
]
    :
        K_SELECT
        ( K_JSON { $json = true; } )? 
        ( K_DISTINCT { $distinct = true; } )? 
        selectors
        K_FROM columnFamilyName
        ( w=K_WHERE whereClause )?
        ( g=K_GROUP K_BY groupByClause ( ',' groupByClause )* )?
        ( o=K_ORDER K_BY orderByClause ( ',' orderByClause )* )?
        ( p=K_PER K_PARTITION K_LIMIT perLimit=intValue )?
        ( l=K_LIMIT limit=intValue )?
        ( K_ALLOW K_FILTERING { $allowFiltering = true; } )?
    ;

selectors
    : selector (',' selector)*
    | '*'
    ;

selector
    : unaliasedSelector alias?
    ;

unaliasedSelector
    : selectionAddition
    ;

selectionAddition
    : left=selectionMultiplication (additionOperator selectionMultiplication)*
    ;

selectionMultiplication
    : left=selectionGroup (multiplicationOperator selectionGroup)*
    ;

selectionGroup
    : selectionGroupWithField
    | selectionGroupWithoutField
    | unary='-' selectionGroup
    ;

selectionGroupWithField
    : selectionGroupWithoutField selectorModifier
    ;

selectorModifier
    : fieldSelectorModifier selectorModifier?          #fieldAccess
    | '[' collectionSubSelection ']' selectorModifier? #rangeAccess
    ;

fieldSelectorModifier
    : '.' fident
    ;

collectionSubSelection returns [bool range]
    : ( t1=term ( RANGE (t2=term)? { $range = true; } )?
      | RANGE t2=term { $range = true; }
      )
    ;

selectionGroupWithoutField
    : simpleUnaliasedSelector
    | selectionTypeHint
    | selectionTupleOrNestedSelector
    | selectionList
    | selectionMapOrSet
    // UDTs are equivalent to maps from the syntax point of view, so the final decision will be done in Selectable.WithMapOrUdt
    ;

selectionTypeHint
    : '(' comparatorType ')' selectionGroupWithoutField
    ;

selectionList
    : '[' ( unaliasedSelector ( ',' unaliasedSelector )* )? ']'
    ;

selectionMapOrSet
    : '{' unaliasedSelector ( m=selectionMap | s=selectionSet ) '}'
    | '{' '}'
    ;

selectionMap
      : ':' unaliasedSelector ( ',' unaliasedSelector ':' unaliasedSelector )*
      ;

selectionSet
      : ( ',' unaliasedSelector )*
      ;

selectionTupleOrNestedSelector
    : '(' list+=unaliasedSelector (',' list+=unaliasedSelector )* ')'
    ;

additionOperator
    : '-' 
    | '+'
    ;

multiplicationOperator
    : '*'
    | '/'
    | '%'
    ;

/*
 * A single selection. The core of it is selecting a column, but we also allow any term and function, as well as
 * sub-element selection for UDT.
 */
simpleUnaliasedSelector
    : sident
    | selectionLiteral
    | selectionFunction
    ;

selectionFunction
    : K_COUNT '(' '*' ')'                                         #countFunction
    | K_WRITETIME '(' c=sident ')'                                #writetimeFunction
    | K_TTL       '(' c=sident ')'                                #ttlFunction
    | K_CAST      '(' sn=unaliasedSelector K_AS t=native_type ')' #castFunction
    | n=functionName args=selectionFunctionArgs                   #userFunction
    ;

selectionLiteral
    : c=constant
    | n=K_NULL
    | b=bindParameter
    ;

selectionFunctionArgs
    : '(' (s1=unaliasedSelector
          ( ',' sn=unaliasedSelector )*)?
      ')'
    ;

alias returns [QsiAliasNode node]
    : K_AS n=noncol_ident
    {
        $node = new QsiAliasNode
        {
            Name = $n.id
        };
    }
    ;

sident returns [QsiIdentifier id]
    : t=IDENT              { $id = new QsiIdentifier($t.text, false); }
    | t=QUOTED_NAME        { $id = new QsiIdentifier($t.text, true); }
    | k=unreserved_keyword { $id = new QsiIdentifier($k.text, false); }
    ;

whereClause
    : relationOrExpression (K_AND relationOrExpression)*
    ;

relationOrExpression
    : relation
    | customIndexExpression
    ;

customIndexExpression
    : K_EXPR '(' idxName ',' t=term ')'
    ;

orderByClause returns [bool isDescending]
    : cident (K_ASC | K_DESC { $isDescending = true; })?
    ;

groupByClause
    : cident
    ;

/**
 * INSERT INTO <CF> (<column>, <column>, <column>, ...)
 * VALUES (<value>, <value>, <value>, ...)
 * USING TIMESTAMP <long>;
 *
 */
insertStatement
    : K_INSERT K_INTO cf=columnFamilyName
        ( st1=normalInsertStatement
        | K_JSON st2=jsonInsertStatement)
    ;

normalInsertStatement returns [bool ifNotExists]
    : '(' cidentList ')'
      K_VALUES
      '(' values+=term ( ',' values+=term )* ')'
      ( K_IF K_NOT K_EXISTS { $ifNotExists = true; })?
      ( usingClause )?
    ;

jsonInsertStatement returns [string defaultValue, bool ifNotExists]
    : val=jsonValue
      ( K_DEFAULT ( k=K_NULL { $defaultValue = $k.text; } | ( k=K_UNSET { $defaultValue = $k.text; }) ) )?
      ( K_IF K_NOT K_EXISTS { $ifNotExists = true; })?
      ( usingClause )?
    ;

jsonValue
    : s=stringLiteral
    | bindParameter
    ;

usingClause
    : K_USING usingClauseObjective ( K_AND usingClauseObjective )*
    ;

usingClauseObjective returns [CqlUsingType type, int time]
    @after { $time = int.Parse($t.text); }
    : K_TIMESTAMP t=intValue { $type = CqlUsingType.Timestamp; }
    | K_TTL t=intValue       { $type = CqlUsingType.Ttl; }
    ;

/**
 * UPDATE <CF>
 * USING TIMESTAMP <long>
 * SET name1 = value1, name2 = value2
 * WHERE key = value;
 * [IF (EXISTS | name = value, ...)];
 */
updateStatement
    : K_UPDATE cf=columnFamilyName
      ( usingClause )?
      K_SET columnOperation (',' columnOperation)*
      K_WHERE wclause=whereClause
      ( K_IF ( K_EXISTS | conditions=updateConditions ))?
    ;

updateConditions
    : columnCondition ( K_AND columnCondition )*
    ;

/**
 * DELETE name1, name2
 * FROM <CF>
 * USING TIMESTAMP <long>
 * WHERE KEY = keyname
   [IF (EXISTS | name = value, ...)];
 */
deleteStatement
    : K_DELETE ( dels=deleteSelection )?
      K_FROM cf=columnFamilyName
      ( usingClauseDelete )?
      K_WHERE wclause=whereClause
      ( K_IF ( K_EXISTS | conditions=updateConditions ))?
    ;

deleteSelection
    : deleteOp (',' deleteOp)*
    ;

deleteOp
    : c=cident
    | c=cident '[' t=term ']'
    | c=cident '.' field=fident
    ;

usingClauseDelete
    : K_USING K_TIMESTAMP ts=intValue
    ;

/**
 * CREATE MATERIALIZED VIEW <viewName> AS
 *  SELECT <columns>
 *  FROM <CF>
 *  WHERE <pkColumns> IS NOT NULL
 *  PRIMARY KEY (<pkColumns>)
 *  WITH <property> = <value> AND ...;
 */
createMaterializedViewStatement
    : K_CREATE K_MATERIALIZED K_VIEW (K_IF K_NOT K_EXISTS)? cf=columnFamilyName K_AS
        K_SELECT sclause=selectors K_FROM basecf=columnFamilyName
        (w=K_WHERE wclause=whereClause)?
        viewPrimaryKey
        ( K_WITH viewProperty ( K_AND viewProperty )*)?
    ;

viewPrimaryKey
    : K_PRIMARY K_KEY '(' viewPartitionKey (',' c=ident )* ')'
    ;

viewPartitionKey
    : keys+=ident 
    | '(' 
        keys+=ident  
        ( ',' keys+=ident )* 
      ')'
    ;

viewProperty
    : property
    | K_COMPACT K_STORAGE
    | K_CLUSTERING K_ORDER K_BY '(' viewClusteringOrder (',' viewClusteringOrder)* ')'
    ;

viewClusteringOrder
    : k=ident (K_ASC | K_DESC)
    ;

fullMapLiteral
    : '{' ( k1=term ':' v1=term ( ',' kn=term ':' vn=term )* )?
      '}'
    ;

setOrMapLiteral
    : m=mapLiteral
    | s=setLiteral
    ;

setLiteral
    : ( ',' tn=term )*
    ;

mapLiteral
    : ':' v=term ( ',' kn=term ':' vn=term )*
    ;

collectionLiteral
    : l=listLiteral
    | '{' t=term v=setOrMapLiteral '}'
    // Note that we have an ambiguity between maps and set for "{}". So we force it to a set literal,
    // and deal with it later based on the type of the column (SetLiteral.java).
    | '{' '}'
    ;

listLiteral
    : '[' ( t1=term ( ',' tn=term )* )? ']'
    ;

usertypeLiteral
    // We don't allow empty literals because that conflicts with sets/maps and is currently useless since we don't allow empty user types
    : '{' k1=fident ':' v1=term ( ',' kn=fident ':' vn=term )* '}'
    ;

tupleLiteral
    : '(' list+=term ( ',' list+=term )* ')'
    ;

value
    : c=constant
    | l=collectionLiteral
    | u=usertypeLiteral
    | t=tupleLiteral
    | n=K_NULL
    | bindParameter
    ;

intValue
    : t=INTEGER
    | bindParameter
    ;

bindParameter
    @after { throw new QsiException(QsiError.NotSupportedFeature, "BindParameter"); }
    : ':' id=noncol_ident
    | QMARK
    ;

functionName returns [QsiQualifiedIdentifier id]
    @init { QsiIdentifier first = null; }
    : (f=keyspaceName '.' { first = $f.id; } )? second=allowedFunctionName
    {
        if (first == null)
            $id = new QsiQualifiedIdentifier($second.id);
        else
            $id = new QsiQualifiedIdentifier(first, $second.id);
    }
    ;

allowedFunctionName returns [QsiIdentifier id]
    : t=IDENT                       { $id = new QsiIdentifier($t.text, false); }
    | t=QUOTED_NAME                 { $id = new QsiIdentifier($t.text, true); }
    | k=unreserved_function_keyword { $id = new QsiIdentifier($k.text, false); }
    | t=K_TOKEN                     { $id = new QsiIdentifier($t.text, false); }
    | t=K_COUNT                     { $id = new QsiIdentifier($t.text, false); }
    ;

cidentList returns [List<QsiIdentifier> list]
    @init { $list = new List<QsiIdentifier>(); }
    : i=cident { $list.Add($i.id); } ( ',' i=cident { $list.Add($i.id); })*
    ;

/** DEFINITIONS **/

// Like ident, but for case where we take a column name that can be the legacy super column empty name. Importantly,
// this should not be used in DDL statements, as we don't want to let users create such column.
cident returns [QsiIdentifier id]
    : e=EMPTY_QUOTED_NAME { $id = new QsiIdentifier(string.Empty, false); }
    | i=ident             { $id = $i.id; }
    ;

ident returns [QsiIdentifier id]
    : t=IDENT              { $id = new QsiIdentifier($t.text, false); }
    | t=QUOTED_NAME        { $id = new QsiIdentifier($t.text, true); }
    | k=unreserved_keyword { $id = new QsiIdentifier($k.text, false); }
    ;

fident returns [QsiIdentifier id]
    : t=IDENT              { $id = new QsiIdentifier($t.text, false); }
    | t=QUOTED_NAME        { $id = new QsiIdentifier($t.text, true); }
    | k=unreserved_keyword { $id = new QsiIdentifier($k.text, false); }
    ;

// Identifiers that do not refer to columns
noncol_ident returns [QsiIdentifier id]
    : t=IDENT              { $id = new QsiIdentifier($t.text, false); }
    | t=QUOTED_NAME        { $id = new QsiIdentifier($t.text, true); }
    | k=unreserved_keyword { $id = new QsiIdentifier($k.text, false); }
    ;

// Keyspace & Column family names
keyspaceName returns [QsiIdentifier id]
    : n=ksName { $id = $n.id; }
    ;

indexName returns [QsiQualifiedIdentifier id]
    @init { QsiIdentifier first = null; }
    : (f=ksName DOT { first = $f.id; } )? second=idxName
    {
        if (first == null)
            $id = new QsiQualifiedIdentifier($second.id);
        else
            $id = new QsiQualifiedIdentifier(first, $second.id);
    }
    ;

columnFamilyName returns [QsiQualifiedIdentifier id]
    @init { QsiIdentifier first = null; }
    : (f=ksName DOT { first = $f.id; } )? second=cfName
    {
        if (first == null)
            $id = new QsiQualifiedIdentifier($second.id);
        else
            $id = new QsiQualifiedIdentifier(first, $second.id);
    }
    ;

userTypeName returns [CqlUserType type]
    @init { QsiIdentifier first = null; }
    : (f=noncol_ident DOT { first = $f.id; } )? second=non_type_ident
    {
        if (first == null)
            $type = new CqlUserType(new QsiQualifiedIdentifier($second.id));
        else
            $type = new CqlUserType(new QsiQualifiedIdentifier(first, $second.id));
    }
    ;

userOrRoleName
    : roleName
    ;

ksName returns [QsiIdentifier id]
    : t=IDENT              { $id = new QsiIdentifier($t.text, false); }
    | t=QUOTED_NAME        { $id = new QsiIdentifier($t.text, true); }
    | k=unreserved_keyword { $id = new QsiIdentifier($k.text, false); }
    | QMARK                { AddRecognitionError("Bind variables cannot be used for keyspace names"); }
    ;

cfName returns [QsiIdentifier id]
    : t=IDENT              { $id = new QsiIdentifier($t.text, false); }
    | t=QUOTED_NAME        { $id = new QsiIdentifier($t.text, true); }
    | k=unreserved_keyword { $id = new QsiIdentifier($k.text, false); }
    | QMARK                { AddRecognitionError("Bind variables cannot be used for table names"); }
    ;

idxName returns [QsiIdentifier id]
    : t=IDENT              { $id = new QsiIdentifier($t.text, false); }
    | t=QUOTED_NAME        { $id = new QsiIdentifier($t.text, true); }
    | k=unreserved_keyword { $id = new QsiIdentifier($k.text, false); }
    | QMARK                { AddRecognitionError("Bind variables cannot be used for index names"); }
    ;

roleName returns [QsiIdentifier id]
    : t=IDENT              { $id = new QsiIdentifier($t.text, false); }
    | t=QUOTED_NAME        { $id = new QsiIdentifier($t.text, true); }
    | k=unreserved_keyword { $id = new QsiIdentifier($k.text, false); }
    | QMARK                { AddRecognitionError("Bind variables cannot be used for role names"); }
    ;

constant
    : s=stringLiteral     #literalString
    | INTEGER             #literalInteger
    | FLOAT               #literalFloat
    | BOOLEAN             #literalBoolean
    | DURATION            #literalDuration
    | UUID                #literalUuid
    | HEXNUMBER           #literalHexnumber
    | K_POSITIVE_NAN      #literalPositiveNan
    | K_NEGATIVE_NAN      #literalNegativeNan
    | K_POSITIVE_INFINITY #literalPositiveInfinity
    | K_NEGATIVE_INFINITY #literalNegativeInfinity
    ;

function
    : n=functionName '(' ')'
    | n=functionName '(' args=functionArgs ')'
    ;

functionArgs
    : t1=term ( ',' tn=term )*
    ;

term
    : t=termAddition
    ;

termAddition
    : left=termMultiplication (additionOperator termMultiplication)*
    ;

termMultiplication
    : left=termGroup (multiplicationOperator termGroup)*
    ;

termGroup
    : t=simpleTerm
    | u='-' t=simpleTerm
    ;

simpleTerm
    : v=value
    | f=function
    | '(' c=comparatorType ')' t=simpleTerm
    ;

columnOperation
    : key=cident columnOperationDifferentiator
    ;

columnOperationDifferentiator
    : '=' normalColumnOperation
    | shorthandColumnOperation
    | '[' k=term ']' collectionColumnOperation
    | '.' field=fident udtColumnOperation
    ;

normalColumnOperation
    : t=term ('+' c=cident )?
    | c=cident sig=('+' | '-') t=term
    | c=cident i=INTEGER
    ;

shorthandColumnOperation
    : sig=('+=' | '-=') t=term
    ;

collectionColumnOperation
    : '=' t=term
    ;

udtColumnOperation
    : '=' t=term
    ;

columnCondition
    // Note: we'll reject duplicates later
    : key=cident
        ( op=relationType t=term
        | K_IN
            ( values=singleColumnInValues
            | marker=bindParameter
            )
        | '[' element=term ']'
            ( op=relationType t=term
            | K_IN
                ( values=singleColumnInValues
                | marker=bindParameter
                )
            )
        | '.' field=fident
            ( op=relationType t=term
            | K_IN
                ( values=singleColumnInValues
                | marker=bindParameter
                )
            )
        )
    ;

properties
    : property (K_AND property)*
    ;

property
    : k=noncol_ident '=' simple=propertyValue
    | k=noncol_ident '=' map=fullMapLiteral
    ;

propertyValue
    : c=constant
    | u=unreserved_keyword
    ;

relationType
    : '='
    | '<'
    | '<='
    | '>'
    | '>='
    | '!='
    ;

relation
    : l=cident op=relationType r=term                      #logicalExpr1
    | l=cident K_LIKE r=term                               #likeExpr
    | l=cident K_IS K_NOT K_NULL                           #isNotNulExpr
    | K_TOKEN l=tupleOfIdentifiers op=relationType r=term  #tokenExpr
    | l=cident K_IN r=bindParameter                        #inExpr1
    | l=cident K_IN r=singleColumnInValues                 #inExpr2
    | l=cident op=containsOperator r=term                  #containsExpr
    | l=cident '[' key=term ']' op=relationType r=term     #logicalExpr2
    | l=tupleOfIdentifiers
      ( 
          K_IN
          (
            /* (a, b, c) IN () */
            '(' ')'
            /* (a, b, c) IN ? */
            | tupleInMarker=bindParameter
            /* (a, b, c) IN ((1, 2, 3), (4, 5, 6), ...) */
            | literals=tupleOfTupleLiterals
            /* (a, b, c) IN (?, ?, ...) */
            | markers=tupleOfMarkersForTuples
          )
          /* (a, b, c) > (1, 2, 3) or (a, b, c) > (?, ?, ?) */
          | op=relationType literal=tupleLiteral
          /* (a, b, c) >= ? */
          | op=relationType tupleMarker=bindParameter
      )                                                    #tupleExpr
    | '(' relation ')'                                     #groupExpr
    ;

containsOperator returns [bool key]
    : K_CONTAINS (K_KEY { $key = true; } )?
    ;

tupleOfIdentifiers
    : '(' cidentList ')'
    ;

singleColumnInValues
    : '(' ( list+=term (',' list+=term)* )? ')'
    ;

tupleOfTupleLiterals
    : '(' list+=tupleLiteral (',' list+=tupleLiteral)* ')'
    ;

tupleOfMarkersForTuples
    : '(' list+=bindParameter (',' list+=bindParameter)* ')'
    ;

comparatorType returns [CqlType type]
    : nt=native_type                     { $type = $nt.type; }
    | ct=collection_type                 { $type = $ct.type; }
    | tt=tuple_type                      { $type = $tt.type; }
    | ut=userTypeName                    { $type = $ut.type; }
    | K_FROZEN '<' ft=comparatorType '>' { $type = new CqlFrozenType($ft.type); }
    ;

native_type returns [CqlNativeType type]
    : K_ASCII     { $type = CqlAsciiType.Default; }
    | K_BIGINT    { $type = CqlBigIntType.Default; }
    | K_BLOB      { $type = CqlBlobType.Default; }
    | K_BOOLEAN   { $type = CqlBooleanType.Default; }
    | K_COUNTER   { $type = CqlCounterType.Default; }
    | K_DECIMAL   { $type = CqlDecimalType.Default; }
    | K_DOUBLE    { $type = CqlDoubleType.Default; }
    | K_DURATION  { $type = CqlDurationType.Default; }
    | K_FLOAT     { $type = CqlFloatType.Default; }
    | K_INET      { $type = CqlInetType.Default; }
    | K_INT       { $type = CqlIntType.Default; }
    | K_SMALLINT  { $type = CqlSmallIntType.Default; }
    | K_TEXT      { $type = CqlTextType.Default; }
    | K_TIMESTAMP { $type = CqlTimestampType.Default; }
    | K_TINYINT   { $type = CqlTinyintType.Default; }
    | K_UUID      { $type = CqlUuidType.Default; }
    | K_VARCHAR   { $type = CqlVarcharType.Default; }
    | K_VARINT    { $type = CqlVarintType.Default; }
    | K_TIMEUUID  { $type = CqlTimeUuidType.Default; }
    | K_DATE      { $type = CqlDateType.Default; }
    | K_TIME      { $type = CqlTimeType.Default; }
    ;

collection_type returns [CqlCollectionType type]
    : K_MAP '<' g1=comparatorType ',' g2=comparatorType '>' { $type = new CqlMapType($g1.type, $g2.type); }
    | K_LIST '<' g1=comparatorType '>'                      { $type = new CqlListType($g1.type); }
    | K_SET '<' g1=comparatorType '>'                       { $type = new CqlSetType($g1.type); }
    ;

tuple_type returns [CqlTupleType type]
    : K_TUPLE '<' f=comparatorType ',' s=comparatorType '>' { $type = new CqlTupleType($f.type, $s.type); }
    ;

username
    : IDENT
    | stringLiteral
    | QUOTED_NAME { AddRecognitionError("Quoted strings are are not supported for user names and USER is deprecated, please use ROLE"); }
    ;

stringLiteral returns [string raw]
    : t=STRING_LITERAL        { $raw = $t.text[1..^1].Replace("''", "'"); }
    | t=DOLLAR_STRING_LITERAL { $raw = $t.text[2..^2]; }
    ;

non_type_ident returns [QsiIdentifier id]
    : t=IDENT                    { VerifyReservedTypeName($t.text); 
                                   $id = new QsiIdentifier($t.text, false); } 
    | t=QUOTED_NAME              { $id = new QsiIdentifier($t.text, true); }
    | k=basic_unreserved_keyword { $id = new QsiIdentifier($k.text, false); }
    | t=K_KEY                    { $id = new QsiIdentifier($t.text, false); }
    ;

unreserved_keyword
    : unreserved_function_keyword
    | (K_TTL | K_COUNT | K_WRITETIME | K_KEY | K_CAST | K_JSON | K_DISTINCT)
    ;

unreserved_function_keyword
    : basic_unreserved_keyword
    | native_type
    ;

basic_unreserved_keyword
    : K_KEYS
    | K_AS
    | K_CLUSTER
    | K_CLUSTERING
    | K_COMPACT
    | K_STORAGE
    | K_TABLES
    | K_TYPE
    | K_TYPES
    | K_VALUES
    | K_MAP
    | K_LIST
    | K_FILTERING
    | K_PERMISSION
    | K_PERMISSIONS
    | K_KEYSPACES
    | K_ALL
    | K_USER
    | K_USERS
    | K_ROLE
    | K_ROLES
    | K_SUPERUSER
    | K_NOSUPERUSER
    | K_LOGIN
    | K_NOLOGIN
    | K_OPTIONS
    | K_PASSWORD
    | K_EXISTS
    | K_CUSTOM
    | K_TRIGGER
    | K_CONTAINS
    | K_INTERNALS
    | K_ONLY
    | K_STATIC
    | K_FROZEN
    | K_TUPLE
    | K_FUNCTION
    | K_FUNCTIONS
    | K_AGGREGATE
    | K_AGGREGATES
    | K_SFUNC
    | K_STYPE
    | K_FINALFUNC
    | K_INITCOND
    | K_RETURNS
    | K_LANGUAGE
    | K_CALLED
    | K_INPUT
    | K_LIKE
    | K_PER
    | K_PARTITION
    | K_GROUP
    ;