parser grammar CqlParser;

options { 
    tokenVocab=CqlLexer;
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
//    | insertStatement
//    | updateStatement
//    | deleteStatement
//    | createMaterializedViewStatement
    ;

selectStatement
    :
        K_SELECT K_JSON? selectClause
        K_FROM columnFamilyName
        ( K_WHERE whereClause )?
        ( K_GROUP K_BY groupByClause ( ',' groupByClause )* )?
        ( K_ORDER K_BY orderByClause ( ',' orderByClause )* )?
        ( K_PER K_PARTITION K_LIMIT intValue )?
        ( K_LIMIT intValue )?
        ( K_ALLOW K_FILTERING )?
    ;

selectClause
    : K_DISTINCT? selectors
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

alias
    : K_AS noncol_ident
    ;

sident
    : IDENT
    | QUOTED_NAME
    | unreserved_keyword
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

//insertStatement
//    : // TODO
//    ;
//
//updateStatement
//    : // TODO
//    ;
//
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

/** DEFINITIONS **/

// Like ident, but for case where we take a column name that can be the legacy super column empty name. Importantly,
// this should not be used in DDL statements, as we don't want to let users create such column.
cident
    : EMPTY_QUOTED_NAME
    | ident
    ;

ident
    : IDENT
    | QUOTED_NAME
    | unreserved_keyword
    ;

fident
    : IDENT
    | QUOTED_NAME
    | unreserved_keyword
    ;

// Identifiers that do not refer to columns
noncol_ident
    : IDENT
    | QUOTED_NAME
    | unreserved_keyword
    ;

// Keyspace & Column family names
keyspaceName
    : ksName |
    ;

indexName
    : (ksName DOT)? idxName
    ;

columnFamilyName
    : (ksName DOT)? cfName
    ;

userTypeName
    : (noncol_ident DOT)? non_type_ident
    ;

userOrRoleName
    : roleName
    ;

ksName
    : IDENT
    | QUOTED_NAME
    | unreserved_keyword
    | QMARK// {AddRecognitionError("Bind variables cannot be used for keyspace names");}
    ;

cfName
    : IDENT
    | QUOTED_NAME
    | unreserved_keyword
    | QMARK// {AddRecognitionError("Bind variables cannot be used for table names");}
    ;

idxName
    : IDENT
    | QUOTED_NAME
    | unreserved_keyword
    | QMARK// {AddRecognitionError("Bind variables cannot be used for index names");}
    ;

roleName
    : IDENT
    | QUOTED_NAME
    | unreserved_keyword
    | QMARK// {AddRecognitionError("Bind variables cannot be used for role names");}
    ;

constant
    : STRING_LITERAL
    | INTEGER
    | FLOAT
    | BOOLEAN
    | DURATION
    | UUID
    | HEXNUMBER
    | (
        (K_POSITIVE_NAN | K_NEGATIVE_NAN)
        | K_POSITIVE_INFINITY
        | K_NEGATIVE_INFINITY
      )
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
    : l=termMultiplication ( op=(PLUS | MINUS) r=termMultiplication )*
    ;

termMultiplication
    : l=termGroup ( op=(MULTIPLY | DIVIDE | MODULUS) r=termGroup )*
    ;

termGroup
    : t=simpleTerm
    | '-' t=simpleTerm
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
      /*{
          if (c == null)
          {
              addRawUpdate(operations, key, new Operation.SetValue(t));
          }
          else
          {
              if (!key.equals(c))
                  addRecognitionError("Only expressions of the form X = <value> + X are supported.");
              addRawUpdate(operations, key, new Operation.Prepend(t));
          }
      }*/
    | c=cident sig=('+' | '-') t=term
      /*{
          if (!key.equals(c))
              addRecognitionError("Only expressions of the form X = X " + $sig.text + "<value> are supported.");
          addRawUpdate(operations, key, $sig.text.equals("+") ? new Operation.Addition(t) : new Operation.Substraction(t));
      }*/
    | c=cident i=INTEGER
      /*{
          // Note that this production *is* necessary because X = X - 3 will in fact be lexed as [ X, '=', X, INTEGER].
          if (!key.equals(c))
              // We don't yet allow a '+' in front of an integer, but we could in the future really, so let's be future-proof in our error message
              addRecognitionError("Only expressions of the form X = X " + ($i.text.charAt(0) == '-' ? '-' : '+') + " <value> are supported.");
          addRawUpdate(operations, key, new Operation.Addition(Constants.Literal.integer($i.text)));
      }*/
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
    : name=cident type=relationType t=term                   #logicalExpr1
    | name=cident K_LIKE t=term                              #likeExpr
    | name=cident K_IS K_NOT K_NULL                          #isNotNulExpr
    | K_TOKEN l=tupleOfIdentifiers type=relationType t=term  #tokenExpr
    | name=cident K_IN marker=inMarker                       #inExpr1
    | name=cident K_IN inValues=singleColumnInValues         #inExpr2
    | name=cident rt=containsOperator t=term                 #constainsExpr
    | name=cident '[' key=term ']' type=relationType t=term  #logicalExpr2
    | ids=tupleOfIdentifiers
      ( K_IN
          ( '(' ')'
          | tupleInMarker=inMarkerForTuple /* (a, b, c) IN ? */
          | literals=tupleOfTupleLiterals /* (a, b, c) IN ((1, 2, 3), (4, 5, 6), ...) */
          | markers=tupleOfMarkersForTuples /* (a, b, c) IN (?, ?, ...) */
          )
      | type=relationType literal=tupleLiteral /* (a, b, c) > (1, 2, 3) or (a, b, c) > (?, ?, ?) */
      | type=relationType tupleMarker=markerForTuple /* (a, b, c) >= ? */
      )                                                      #inExpr3
    | '(' relation ')'                                       #groupExpr
    ;

containsOperator
    : K_CONTAINS (K_KEY)?
    ;

inMarker
    : QMARK
    | ':' name=noncol_ident
    ;

tupleOfIdentifiers
    : '(' n1=cident (',' ni=cident)* ')'
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
    | K_FROZEN L_BRACKET comparatorType R_BRACKET
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
    : K_MAP L_BRACKET comparatorType COMMA comparatorType R_BRACKET
    | K_LIST L_BRACKET comparatorType R_BRACKET
    | K_SET L_BRACKET comparatorType R_BRACKET
    ;

tuple_type
    : K_TUPLE L_BRACKET comparatorType COMMA comparatorType R_BRACKET
    ;

username
    : IDENT
    | STRING_LITERAL
    | QUOTED_NAME// { AddRecognitionError("Quoted strings are are not supported for user names and USER is deprecated, please use ROLE"); }
    ;

mbean
    : STRING_LITERAL
    ;

non_type_ident
    : t=IDENT// { VerifyReservedTypeName($t.Text); } 
    | QUOTED_NAME
    | basic_unreserved_keyword
    | K_KEY
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