parser grammar CqlParserInternal;

options { 
    tokenVocab=CqlLexerInternal;
}

@header {
    using Qsi.Data;
    using Qsi.Tree;
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
//    | deleteStatement
//    | createMaterializedViewStatement
    ;

/**
 * SELECT [JSON] [DISTINCT] <expression>
 * FROM <CF>
 * [ WHERE KEY = "key1" AND COL > 1 AND COL < 100 ]
 * [ PER PARTITION LIMIT <NUMBER> ]
 * [ LIMIT <NUMBER> ]
 * [ ALLOW FILTERING ]
 */
selectStatement
    :
        K_SELECT K_JSON? K_DISTINCT? selectors
        K_FROM columnFamilyName
        ( K_WHERE whereClause )?
        ( K_GROUP K_BY groupByClause ( ',' groupByClause )* )?
        ( K_ORDER K_BY orderByClause ( ',' orderByClause )* )?
        ( K_PER K_PARTITION K_LIMIT intValue )?
        ( K_LIMIT intValue )?
        ( K_ALLOW K_FILTERING )?
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
    : l=selectionMultiplication ( op=(PLUS | MINUS) r=selectionMultiplication )*
    ;

selectionMultiplication
    : l=selectionGroup ( op=(MULTIPLY | DIVIDE | MODULUS) r=selectionGroup )*
    ;

selectionGroup
    : selectionGroupWithField
    | selectionGroupWithoutField
    | '-' selectionGroup
    ;

selectionGroupWithField
    : selectionGroupWithoutField selectorModifier
    ;

selectorModifier
    : fieldSelectorModifier selectorModifier?
    | '[' collectionSubSelection ']' selectorModifier?
    ;

fieldSelectorModifier
    : '.' fident
    ;

collectionSubSelection
    : ( term ( RANGE term? )?
      | RANGE term
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
    : '{' unaliasedSelector ( selectionMap | selectionSet ) '}'
    | '{' '}'
    ;

selectionMap
      : ':' unaliasedSelector ( ',' unaliasedSelector ':' unaliasedSelector )*
      ;

selectionSet
      : ( ',' unaliasedSelector )*
      ;

selectionTupleOrNestedSelector
    : '(' unaliasedSelector (',' unaliasedSelector )* ')'
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
    : K_COUNT '(' '*' ')'
    | K_WRITETIME '(' c=sident ')'
    | K_TTL       '(' c=sident ')'
    | K_CAST      '(' sn=unaliasedSelector K_AS t=native_type ')'
    | functionName selectionFunctionArgs
    ;

selectionLiteral
    : constant
    | K_NULL
    | ':' noncol_ident
    | QMARK
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

orderByClause
    : cident (K_ASC | K_DESC)?
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

normalInsertStatement
    : '(' cidentList ')'
      K_VALUES
      '(' v1=term ( ',' vn=term )* ')'
      ( K_IF K_NOT K_EXISTS )?
      ( usingClause )?
    ;

jsonInsertStatement
    : val=jsonValue
      ( K_DEFAULT ( K_NULL | ( K_UNSET) ) )?
      ( K_IF K_NOT K_EXISTS )?
      ( usingClause )?
    ;

jsonValue
    : s=STRING_LITERAL
    | ':' id=noncol_ident
    | QMARK
    ;

usingClause
    : K_USING usingClauseObjective ( K_AND usingClauseObjective )*
    ;

usingClauseObjective
    : K_TIMESTAMP ts=intValue
    | K_TTL t=intValue
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

//deleteStatement
//    : // TODO
//    ;
//
//createMaterializedViewStatement
//    : // TODO
//    ;

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
    : '(' t1=term ( ',' tn=term )* ')'
    ;

value
    : c=constant
    | l=collectionLiteral
    | u=usertypeLiteral
    | t=tupleLiteral
    | K_NULL
    | ':' id=noncol_ident
    | QMARK
    ;

intValue
    : t=INTEGER
    | ':' id=noncol_ident
    | QMARK
    ;

functionName
    : (keyspaceName '.')? allowedFunctionName
    ;

allowedFunctionName
    : IDENT
    | QUOTED_NAME
    | unreserved_function_keyword
    | K_TOKEN
    | K_COUNT
    ;

cidentList returns [List<QsiIdentifier> list]
    @init {
        $list = new List<QsiIdentifier>();
    }
    : c1=cident { $list.Add($c1.id); } ( ',' cn=cident { $list.Add($cn.id); } )*
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

userTypeName returns [QsiQualifiedIdentifier id]
    @init { QsiIdentifier first = null; }
    : (f=noncol_ident DOT { first = $f.id; } )? second=non_type_ident
    {
        if (first == null)
            $id = new QsiQualifiedIdentifier($second.id);
        else
            $id = new QsiQualifiedIdentifier(first, $second.id);
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
    : STRING_LITERAL
    | INTEGER
    | FLOAT
    | BOOLEAN
    | DURATION
    | UUID
    | HEXNUMBER
    | K_POSITIVE_NAN
    | K_NEGATIVE_NAN
    | K_POSITIVE_INFINITY
    | K_NEGATIVE_INFINITY
    ;

function
    : f=functionName '(' ')'
    | f=functionName '(' args=functionArgs ')'
    ;

functionArgs
    : t1=term ( ',' tn=term )*
    ;

term
    : t=termAddition
    ;

termAddition
    : l=termMultiplication ( op=('+' | '-') r=termMultiplication )*
    ;

termMultiplication
    : l=termGroup ( op=('*' | '/' | '%') r=termGroup )*
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
            | marker=inMarker
            )
        | '[' element=term ']'
            ( op=relationType t=term
            | K_IN
                ( values=singleColumnInValues
                | marker=inMarker
                )
            )
        | '.' field=fident
            ( op=relationType t=term
            | K_IN
                ( values=singleColumnInValues
                | marker=inMarker
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
    | l=cident K_IN r=inMarker                             #inExpr1
    | l=cident K_IN r=singleColumnInValues                 #inExpr2
    | l=cident op=containsOperator r=term                  #constainsExpr
    | l=cident '[' key=term ']' op=relationType r=term     #logicalExpr2
    | l=tupleOfIdentifiers
      ( K_IN
          (
            '(' ')'
            | tupleInMarker=inMarkerForTuple /* (a, b, c) IN ? */
            | literals=tupleOfTupleLiterals /* (a, b, c) IN ((1, 2, 3), (4, 5, 6), ...) */
            | markers=tupleOfMarkersForTuples /* (a, b, c) IN (?, ?, ...) */
          )
          | op=relationType literal=tupleLiteral /* (a, b, c) > (1, 2, 3) or (a, b, c) > (?, ?, ?) */
          | op=relationType tupleMarker=markerForTuple /* (a, b, c) >= ? */
      )                                                    #inExpr3
    | '(' relation ')'                                     #groupExpr
    ;

containsOperator
    : K_CONTAINS (K_KEY)?
    ;

inMarker
    : QMARK
    | ':' name=noncol_ident
    ;

tupleOfIdentifiers
    : '(' cidentList ')'
    ;

singleColumnInValues
    : '(' ( t1 = term (',' ti=term)* )? ')'
    ;

tupleOfTupleLiterals
    : '(' t1=tupleLiteral (',' ti=tupleLiteral)* ')'
    ;

markerForTuple
    : QMARK
    | ':' name=noncol_ident
    ;

tupleOfMarkersForTuples
    : '(' m1=markerForTuple (',' mi=markerForTuple)* ')'
    ;

inMarkerForTuple
    : QMARK
    | ':' name=noncol_ident
    ;

comparatorType
    : native_type
    | collection_type
    | tuple_type
    | userTypeName
    | K_FROZEN '<' comparatorType '>'
    ;

native_type
    : K_ASCII
    | K_BIGINT
    | K_BLOB
    | K_BOOLEAN
    | K_COUNTER
    | K_DECIMAL
    | K_DOUBLE
    | K_DURATION
    | K_FLOAT
    | K_INET
    | K_INT
    | K_SMALLINT
    | K_TEXT
    | K_TIMESTAMP
    | K_TINYINT
    | K_UUID
    | K_VARCHAR
    | K_VARINT
    | K_TIMEUUID
    | K_DATE
    | K_TIME
    ;

collection_type
    : K_MAP '<' comparatorType ',' comparatorType '>'
    | K_LIST '<' comparatorType '>'
    | K_SET '<' comparatorType '>'
    ;

tuple_type
    : K_TUPLE '<' comparatorType ',' comparatorType '>'
    ;

username
    : IDENT
    | STRING_LITERAL
    | QUOTED_NAME { AddRecognitionError("Quoted strings are are not supported for user names and USER is deprecated, please use ROLE"); }
    ;

mbean
    : STRING_LITERAL
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