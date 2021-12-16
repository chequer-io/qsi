/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

grammar SqlBase;

tokens {
    DELIMITER
}

@header {
    using Qsi.Data;
    using Qsi.Tree;
    using Qsi.Utilities;
}

singleStatement
    : statement EOF
    ;

standaloneExpression
    : expression EOF
    ;

standaloneRoutineBody
    : routineBody EOF
    ;

statement
    : query                                                            #statementDefault
    | USE schema=identifier[null]                                      #use
    | USE catalog=identifier[null] '.' schema=identifier[null]         #use
    | CREATE SCHEMA (IF NOT EXISTS)? qualifiedName
        (WITH properties)?                                             #createSchema
    | DROP SCHEMA (IF EXISTS)? qualifiedName (CASCADE | RESTRICT)?     #dropSchema
    | ALTER SCHEMA qualifiedName RENAME TO identifier[null]            #renameSchema
    | CREATE TABLE (IF NOT EXISTS)? qualifiedName columnAliases?
        (COMMENT string)?
        (WITH properties)? AS (query | '('query')')
        (WITH (NO)? DATA)?                                             #createTableAsSelect
    | CREATE TABLE (IF NOT EXISTS)? qualifiedName
        '(' tableElement (',' tableElement)* ')'
         (COMMENT string)?
         (WITH properties)?                                            #createTable
    | DROP TABLE (IF EXISTS)? qualifiedName                            #dropTable
    | INSERT INTO qualifiedName columnAliases? query                   #insertInto
    | UNLOAD querySpecification 
        TO string WITH properties                                      #unload
    | DELETE FROM qualifiedName (WHERE booleanExpression)?             #delete
    
    
    //region ALTER DATABASE
        
    // SET OWNER: Not supported in Athena
    // SET LOCATION: Not supported in Athena
    // SET MANAGEDLOCATION: Not supported in Athena
    
    // SET DBPROPERTIES
    // Reference1: https://docs.aws.amazon.com/athena/latest/ug/alter-database-set-dbproperties.html
    // Reference2: https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AlterDatabase
    | ALTER (DATABASE | SCHEMA) databaseName=qualifiedName
        SET DBPROPERTIES dbProperties=stringProperties                 #setDbProperties

    //endregion  
    
    //region ALTER TABLE
  
    // SET SERDE: Not supported in Athena, tested at 2021. 12. 14.
    // SET SERDEPROPERTIES: Not supported in Athena, tested at 2021. 12. 14.
    // UNSET SERDEPROPERTIES: Not supported in Athena, tested at 2021. 12. 14.
    // ADD CONSTRAINT: Not supported in Athena, tested at 2021. 12. 14.
    
    // RENAME TO: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    // CLUSTERED BY: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    // NOT CLUSTERED: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    // SKEWED BY: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    // NOT SKEWED: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    // NOT STORED: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    // NOT STORED AS DIRECTORIES: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    // SET SKEWED LOCATION : Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)

    // SET TBLPROPERTIES
    // Reference: https://docs.aws.amazon.com/athena/latest/ug/alter-table-set-tblproperties.html 
    // Reference: https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AlterTableProperties
    | ALTER TABLE tableName=qualifiedName
        SET TBLPROPERTIES tblProperties=stringProperties               #setTblProperties
    
    //endregion
    
    //region ALTER TABLE/PARTITION

    // SET FILEFORMAT: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    // TOUCH: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    // COMPACT: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    // CONCATENATE: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    
    // ENABLE/DISABLE NO_DROP: Not supported in Athena, tested at 2021. 12. 14.
    // ENABLE/DISABLE OFFLINE: Not supported in Athena, tested at 2021. 12. 14.
    // UPDATE COLUMNS: Not supprted in Athena, test at 2021. 12. 14.
    
    // SET LOCATION
    // Reference: https://docs.aws.amazon.com/athena/latest/ug/alter-table-set-location.html
    // Reference: https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=82706445#LanguageManualDDL-AlterTable/PartitionLocation
    | ALTER TABLE tableName=qualifiedName
        (PARTITION partitionSpec=properties)?
        SET LOCATION location=string                                   #setLocation
        
    //endregion
    
    //region ALTER PARTITION
    
    // ARCHIVE PARTITION: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    // UNARCHIVE PARTITION: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)
    
    // ADD PARTITIONS
    // Reference1: https://docs.aws.amazon.com/athena/latest/ug/alter-table-add-partition.html
    // Reference2: https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AddPartitions
    | ALTER TABLE tableName=qualifiedName ADD (IF NOT EXISTS)?
        (
            PARTITION partitionSpec=properties
            (LOCATION location=string)?
        )+                                                             #addPartitions
    
    // RENAME PARTITION
    // Reference1: https://docs.aws.amazon.com/athena/latest/ug/alter-table-rename-partition.html
    // Reference2: https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-RenamePartition
    | ALTER TABLE tableName=qualifiedName
        PARTITION from=properties
        RENAME TO PARTITION to=properties                              #renamePartition

    // EXCHANGE PARTITION: Not supported in Athena (https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html)

    // MSCK REPAIR TABLE
    // Reference1: https://docs.aws.amazon.com/athena/latest/ug/msck-repair-table.html
    // Reference2: https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-RecoverPartitions(MSCKREPAIRTABLE)
    | MSCK (REPAIR)? TABLE tableName=qualifiedName
        (ADD | DROP | SYNC PARTITIONS)                                 #repairTable

    // DROP PARTITION
    // Reference1: https://docs.aws.amazon.com/athena/latest/ug/alter-table-drop-partition.html
    // Reference2: https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-DropPartitions
    | ALTER TABLE tableName=qualifiedName DROP (IF EXISTS)?
        PARTITION partitionSpec=properties (',' PARTITION partitionSpec=properties)*
        (IGNORE PROTECTION)?
        (PURGE)?                                                       #dropPartition
    
    // endregion

    // region ALTER COLUMNS
    
    // CHANGE COLUMN
    // According to Reference1(AWS), CHANGE COLUMN is not supported in Athena, but works in test (tested at 2021. 12. 14.)
    //      test query: "ALTER TABLE table_name CHANGE original_column changed_column int"
    // Reference1: https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
    // Reference2: https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ChangeColumnName/Type/Position/Comment
    | ALTER TABLE tableName=qualifiedName
        (PARTITION properties)?
        CHANGE (COLUMN)? identifier[null] columnDefinition
        (FIRST | AFTER identifier[null])?
        (CASCADE | RESTRICT)?                                            #changeColumn

    // ADD COLUMNS
    // Reference1: https://docs.aws.amazon.com/athena/latest/ug/alter-table-add-columns.html
    // Reference2: https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-Add/ReplaceColumns
    | ALTER TABLE tableName=qualifiedName
        (PARTITION partitionSpec=properties)?
        ADD COLUMNS
        '(' columnDefinition (',' columnDefinition)* ')'
        (CASCADE|RESTRICT)?                                              #addColumns

    // REPLACE COLUMNS
    // Reference1: https://docs.aws.amazon.com/athena/latest/ug/alter-table-replace-columns.html
    // Reference2: https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-Add/ReplaceColumns
    | ALTER TABLE tableName=qualifiedName
        (PARTITION partitionSpec=properties)?
        REPLACE COLUMNS
        '(' columnDefinition (',' columnDefinition)* ')'
        (CASCADE|RESTRICT)?                                              #replaceColumns
    
    // endregion

    | ANALYZE qualifiedName (WITH properties)?                         #analyze
    | CREATE TYPE qualifiedName AS (
        '(' sqlParameterDeclaration (',' sqlParameterDeclaration)* ')'
        | type)                                                        #createType
    | CREATE (OR REPLACE)? VIEW qualifiedName AS query                 #createView
    | DROP (DATABASE | SCHEMA) (IF EXISTS)?
        identifier[null] (RESTRICT | CASCADE)?                         #dropDatabase
    | DROP VIEW (IF EXISTS)? qualifiedName                             #dropView
    | DROP TABLE (IF EXISTS)? qualifiedName                            #dropTable
    | CREATE MATERIALIZED VIEW (IF NOT EXISTS)? qualifiedName
        (COMMENT string)?
        (WITH properties)? AS (query | '('query')')                    #createMaterializedView
    | DROP MATERIALIZED VIEW (IF EXISTS)? qualifiedName                #dropMaterializedView
    | REFRESH MATERIALIZED VIEW qualifiedName WHERE booleanExpression  #refreshMaterializedView
    | CREATE (OR REPLACE)? TEMPORARY? FUNCTION functionName=qualifiedName
        '(' (sqlParameterDeclaration (',' sqlParameterDeclaration)*)? ')'
        RETURNS returnType=type
        (COMMENT string)?
        routineCharacteristics routineBody                             #createFunction
    | CREATE (DATABASE | SCHEMA) (IF NOT EXISTS)? identifier[null]
        (COMMENT comment=string)?
        (LOCATION location=string)?
        (WITH DBPROPERTIES stringProperties)?                          #createDatabase
    | CREATE EXTERNAL TABLE (IF NOT EXISTS)? qualifiedName
        ('(' columnDefinition (',' columnDefinition)* ')')?
        (COMMENT comment=string)?
        (PARTITIONED BY '(' columnDefinition (',' columnDefinition)* ')')?
        (CLUSTERED BY '(' identifier[null] (',' identifier[null])* ')' INTO number BUCKETS)?
        (ROW FORMAT rowFormat)?
        (STORED AS fileFormat)?
        (WITH SERDEPROPERTIES stringProperties)?
        (LOCATION location=string)?
        (TBLPROPERTIES stringProperties)?                              #createExternalTable
    | CREATE TABLE qualifiedName
        (WITH properties)?
        AS query
        (WITH NO? DATA)?                                               #createTableAs
    | ALTER FUNCTION qualifiedName types?
      alterRoutineCharacteristics                                      #alterFunction
    | DROP TEMPORARY? FUNCTION (IF EXISTS)? qualifiedName types?       #dropFunction
    | CALL qualifiedName '(' (callArgument (',' callArgument)*)? ')'   #call
    | CREATE ROLE name=identifier[null]
        (WITH ADMIN grantor)?                                          #createRole
    | DROP ROLE name=identifier[null]                                  #dropRole
    | GRANT
        roles
        TO principal (',' principal)*
        (WITH ADMIN OPTION)?
        (GRANTED BY grantor)?                                          #grantRoles
    | REVOKE
        (ADMIN OPTION FOR)?
        roles
        FROM principal (',' principal)*
        (GRANTED BY grantor)?                                          #revokeRoles
    | SET ROLE (ALL | NONE | role=identifier[null])                    #setRole
    | GRANT
        (privilege (',' privilege)* | ALL PRIVILEGES)
        ON TABLE? qualifiedName TO grantee=principal
        (WITH GRANT OPTION)?                                           #grant
    | REVOKE
        (GRANT OPTION FOR)?
        (privilege (',' privilege)* | ALL PRIVILEGES)
        ON TABLE? qualifiedName FROM grantee=principal                 #revoke
    | SHOW GRANTS
        (ON TABLE? qualifiedName)?                                     #showGrants
    | EXPLAIN VERBOSE?
        ('(' explainOption (',' explainOption)* ')')? statement        #explain
    | SHOW TBLPROPERTIES qualifiedName ('(' string ')')?               #showTblproperties
    | SHOW (DATABASES | SCHEMAS) (LIKE string)?                        #showDatabases
    | SHOW PARTITIONS qualifiedName                                    #showPartitions
    | SHOW CREATE TABLE qualifiedName                                  #showCreateTable
    | SHOW CREATE VIEW qualifiedName                                   #showCreateView
    | SHOW CREATE MATERIALIZED VIEW qualifiedName                      #showCreateMaterializedView
    | SHOW CREATE FUNCTION qualifiedName types?                        #showCreateFunction
    | SHOW TABLES ((FROM | IN) qualifiedName)? (pattern=string)?       #showTables
    | SHOW VIEWS 
        (IN database=identifier[null])? (LIKE pattern=string)?         #showViews
    | SHOW CATALOGS (LIKE pattern=string)?                             #showCatalogs
    | SHOW COLUMNS (FROM | IN) 
      ( qualifiedName 
      | table=identifier[null] (FROM | IN) database=identifier[null])  #showColumns
    | SHOW STATS FOR qualifiedName                                     #showStats
    | SHOW STATS FOR '(' querySpecification ')'                        #showStatsForQuery
    | SHOW CURRENT? ROLES ((FROM | IN) identifier[null])?              #showRoles
    | SHOW ROLE GRANTS ((FROM | IN) identifier[null])?                 #showRoleGrants
    | (DESC | DESCRIBE) (EXTENDED | FORMATTED)? qualifiedName
        (PARTITION properties)?
        (qualifiedName)?                                               #describeTable
    | (DESC | DESCRIBE) qualifiedName                                  #describeView
    | SHOW FUNCTIONS
        (LIKE pattern=string (ESCAPE escape=string)?)?                 #showFunctions
    | SHOW SESSION                                                     #showSession
    | SET SESSION qualifiedName EQ expression                          #setSession
    | RESET SESSION qualifiedName                                      #resetSession
    | START TRANSACTION (transactionMode (',' transactionMode)*)?      #startTransaction
    | COMMIT WORK?                                                     #commit
    | ROLLBACK WORK?                                                   #rollback
    | PREPARE identifier[null] FROM statement                          #prepare
    | DEALLOCATE PREPARE identifier[null]                              #deallocate
    | EXECUTE identifier[null] (USING expression (',' expression)*)?   #execute
    | DESCRIBE INPUT identifier[null]                                  #describeInput
    | DESCRIBE OUTPUT identifier[null]                                 #describeOutput
    | MSCK REPAIR TABLE qualifiedName                                  #msckRepairTable
    ;

query
    :  with? queryNoWith
    ;

with
    : WITH namedQuery (',' namedQuery)*
    ;

tableElement
    : columnDefinition
    | likeClause
    ;

columnDefinition
    : identifier[null] type (COMMENT string)?
    ;

likeClause
    : LIKE qualifiedName (optionType=(INCLUDING | EXCLUDING) PROPERTIES)?
    ;

rowFormat
    : DELIMITED FIELDS TERMINATED BY string (ESCAPED BY string)?
    | DELIMITED COLLECTION ITEMS TERMINATED BY string
    | MAP KEYS TERMINATED BY string
    | LINES TERMINATED BY string
    | NULL DEFINED AS string
    | SERDE string (WITH SERDEPROPERTIES stringProperties)?
    ;

fileFormat
    : SEQUENCEFILE
    | TEXTFILE
    | RCFILE
    | ORC
    | PARQUET
    | AVRO
    | INPUTFORMAT string OUTPUTFORMAT string
    ;

stringProperties
    : '(' stringProperty (',' stringProperty)* ')'
    ;

stringProperty
    : string EQ string
    ;

properties
    : '(' property (',' property)* ')'
    ;

property
    : identifier[null] EQ expression
    ;

sqlParameterDeclaration
    : identifier[null] type
    ;

routineCharacteristics
    : routineCharacteristic*
    ;

routineCharacteristic
    : LANGUAGE language
    | determinism
    | nullCallClause
    ;

alterRoutineCharacteristics
    : alterRoutineCharacteristic*
    ;

alterRoutineCharacteristic
    : nullCallClause
    ;

routineBody
    : returnStatement
    | externalBodyReference
    ;

returnStatement
    : RETURN expression
    ;

externalBodyReference
    : EXTERNAL (NAME externalRoutineName)?
    ;

language
    : SQL
    | identifier[null]
    ;

determinism
    : DETERMINISTIC
    | NOT DETERMINISTIC;

nullCallClause
    : RETURNS NULL ON NULL INPUT
    | CALLED ON NULL INPUT
    ;

externalRoutineName
    : identifier[null]
    ;

queryNoWith:
      queryTerm
      (ORDER BY sortItem (',' sortItem)*)?
      (OFFSET offset=INTEGER_VALUE (ROW | ROWS)?)?
      (LIMIT limit=(INTEGER_VALUE | ALL))?
    ;

queryTerm
    : queryPrimary                                                             #queryTermDefault
    | left=queryTerm operator=INTERSECT setQuantifier? right=queryTerm         #setOperation
    | left=queryTerm operator=(UNION | EXCEPT) setQuantifier? right=queryTerm  #setOperation
    ;

queryPrimary
    : querySpecification                   #queryPrimaryDefault
    | TABLE qualifiedName                  #table
    | VALUES expression (',' expression)*  #inlineTable
    | '(' queryNoWith  ')'                 #subquery
    ;

sortItem
    : expression ordering=(ASC | DESC)? (NULLS nullOrdering=(FIRST | LAST))?
    ;

querySpecification
    : SELECT setQuantifier? selectItem (',' selectItem)*
      (FROM relation (',' relation)*)?
      (WHERE where=booleanExpression)?
      (GROUP BY groupBy)?
      (HAVING having=booleanExpression)?
    ;

groupBy
    : setQuantifier? groupingElement (',' groupingElement)*
    ;

groupingElement
    : groupingSet                                            #singleGroupingSet
    | ROLLUP '(' (expression (',' expression)*)? ')'         #rollup
    | CUBE '(' (expression (',' expression)*)? ')'           #cube
    | GROUPING SETS '(' groupingSet (',' groupingSet)* ')'   #multipleGroupingSets
    ;

groupingSet
    : '(' (expression (',' expression)*)? ')'
    | expression
    ;

namedQuery
    : name=identifier[null] (columnAliases)? AS '(' query ')'
    ;

setQuantifier
    : DISTINCT
    | ALL
    ;

selectItem
    : expression (AS? identifier[null])?  #selectSingle
    | qualifiedName '.' ASTERISK    #selectAll
    | ASTERISK                      #selectAll
    ;

relation
    : left=relation
      ( CROSS JOIN right=sampledRelation
      | joinType JOIN rightRelation=relation joinCriteria
      | NATURAL joinType JOIN right=sampledRelation
      )                                           #joinRelation
    | sampledRelation                             #relationDefault
    ;

joinType
    : INNER?
    | LEFT OUTER?
    | RIGHT OUTER?
    | FULL OUTER?
    ;

joinCriteria
    : ON booleanExpression
    | USING '(' identifier[null] (',' identifier[null])* ')'
    ;

sampledRelation
    : aliasedRelation (
        TABLESAMPLE sampleType '(' percentage=expression ')'
      )?
    ;

sampleType
    : BERNOULLI
    | SYSTEM
    ;

aliasedRelation
    : relationPrimary (AS? identifier[null] columnAliases?)?
    ;

columnAliases
    : '(' identifier[null] (',' identifier[null])* ')'
    ;

relationPrimary
    : qualifiedName                                                   #tableName
    | '(' query ')'                                                   #subqueryRelation
    | UNNEST '(' expression (',' expression)* ')' (WITH ORDINALITY)?  #unnest
    | LATERAL '(' query ')'                                           #lateral
    | '(' relation ')'                                                #parenthesizedRelation
    ;

expression
    : booleanExpression
    ;

booleanExpression
    : valueExpression predicate[$valueExpression.ctx]?             #predicated
    | NOT booleanExpression                                        #logicalNot
    | left=booleanExpression operator=AND right=booleanExpression  #logicalBinary
    | left=booleanExpression operator=OR right=booleanExpression   #logicalBinary
    ;

// workaround for https://github.com/antlr/antlr4/issues/780
predicate[ParserRuleContext value]
    : comparisonOperator right=valueExpression                            #comparison
    | comparisonOperator comparisonQuantifier '(' query ')'               #quantifiedComparison
    | NOT? BETWEEN lower=valueExpression AND upper=valueExpression        #between
    | NOT? IN '(' expression (',' expression)* ')'                        #inList
    | NOT? IN '(' query ')'                                               #inSubquery
    | NOT? LIKE pattern=valueExpression (ESCAPE escape=valueExpression)?  #like
    | IS NOT? NULL                                                        #nullPredicate
    | IS NOT? DISTINCT FROM right=valueExpression                         #distinctFrom
    ;

valueExpression
    : primaryExpression                                                                 #valueExpressionDefault
    | valueExpression AT timeZoneSpecifier                                              #atTimeZone
    | operator=(MINUS | PLUS) valueExpression                                           #arithmeticUnary
    | left=valueExpression operator=(ASTERISK | SLASH | PERCENT) right=valueExpression  #arithmeticBinary
    | left=valueExpression operator=(PLUS | MINUS) right=valueExpression                #arithmeticBinary
    | left=valueExpression CONCAT right=valueExpression                                 #concatenation
    ;

primaryExpression
    : NULL                                                                                #nullLiteral
    | interval                                                                            #intervalLiteral
    | identifier[null] string                                                             #typeConstructor
    | DOUBLE_PRECISION string                                                             #typeConstructor
    | number                                                                              #numericLiteral
    | booleanValue                                                                        #booleanLiteral
    | string                                                                              #stringLiteral
    | BINARY_LITERAL                                                                      #binaryLiteral
    | '?'                                                                                 #parameter
    | POSITION '(' valueExpression IN valueExpression ')'                                 #position
    | '(' expression (',' expression)+ ')'                                                #rowConstructor
    | ROW '(' expression (',' expression)* ')'                                            #rowConstructor
    | qualifiedName '(' ASTERISK ')' filter? over?                                        #functionCall
    | qualifiedName '(' (setQuantifier? expression (',' expression)*)?
        (ORDER BY sortItem (',' sortItem)*)? ')' filter? (nullTreatment? over)?           #functionCall
    | identifier[null] '->' expression                                                    #lambda
    | '(' (identifier[null] (',' identifier[null])*)? ')' '->' expression                 #lambda
    | '(' query ')'                                                                       #subqueryExpression
    // This is an extension to ANSI SQL, which considers EXISTS to be a <boolean expression>
    | EXISTS '(' query ')'                                                                #exists
    | CASE valueExpression whenClause+ (ELSE elseExpression=expression)? END              #simpleCase
    | CASE whenClause+ (ELSE elseExpression=expression)? END                              #searchedCase
    | CAST '(' expression AS type ')'                                                     #cast
    | TRY_CAST '(' expression AS type ')'                                                 #cast
    | ARRAY '[' (expression (',' expression)*)? ']'                                       #arrayConstructor
    | value=primaryExpression '[' index=valueExpression ']'                               #subscript
    | identifier[null]                                                                    #columnReference
    | expr=primaryExpression '.' fieldName=identifier[null]                               #dereference
    | name=CURRENT_DATE                                                                   #specialDateTimeFunction
    | name=CURRENT_TIME ('(' precision=INTEGER_VALUE ')')?                                #specialDateTimeFunction
    | name=CURRENT_TIMESTAMP ('(' precision=INTEGER_VALUE ')')?                           #specialDateTimeFunction
    | name=LOCALTIME ('(' precision=INTEGER_VALUE ')')?                                   #specialDateTimeFunction
    | name=LOCALTIMESTAMP ('(' precision=INTEGER_VALUE ')')?                              #specialDateTimeFunction
    | name=CURRENT_USER                                                                   #currentUser
    | SUBSTRING '(' valueExpression FROM valueExpression (FOR valueExpression)? ')'       #substring
    | NORMALIZE '(' valueExpression (',' normalForm)? ')'                                 #normalize
    | EXTRACT '(' identifier[null] FROM valueExpression ')'                               #extract
    | '(' expression ')'                                                                  #parenthesizedExpression
    | GROUPING '(' (qualifiedName (',' qualifiedName)*)? ')'                              #groupingOperation
    ;

string
    : STRING                                #basicStringLiteral
    | UNICODE_STRING (UESCAPE STRING)?      #unicodeStringLiteral
    ;

nullTreatment
    : IGNORE NULLS
    | RESPECT NULLS
    ;

timeZoneSpecifier
    : TIME ZONE interval  #timeZoneInterval
    | TIME ZONE string    #timeZoneString
    ;

comparisonOperator
    : EQ | NEQ | LT | LTE | GT | GTE
    ;

comparisonQuantifier
    : ALL | SOME | ANY
    ;

booleanValue
    : TRUE | FALSE
    ;

interval
    : INTERVAL sign=(PLUS | MINUS)? string from=intervalField (TO to=intervalField)?
    ;

intervalField
    : YEAR | MONTH | DAY | HOUR | MINUTE | SECOND
    ;

normalForm
    : NFD | NFC | NFKD | NFKC
    ;

types
    : '(' (type (',' type)*)? ')'
    ;

type
    : type ARRAY
    | ARRAY '<' type '>'
    | MAP '<' type ',' type '>'
    | ROW '(' identifier[null] type (',' identifier[null] type)* ')'
    | baseType ('(' typeParameter (',' typeParameter)* ')')?
    | INTERVAL from=intervalField TO to=intervalField
    ;

typeParameter
    : INTEGER_VALUE | type
    ;

baseType
    : TIME_WITH_TIME_ZONE
    | TIMESTAMP_WITH_TIME_ZONE
    | DOUBLE_PRECISION
    | qualifiedName
    ;

whenClause
    : WHEN condition=expression THEN result=expression
    ;

filter
    : FILTER '(' WHERE booleanExpression ')'
    ;

over
    : OVER '('
        (PARTITION BY partition+=expression (',' partition+=expression)*)?
        (ORDER BY sortItem (',' sortItem)*)?
        windowFrame?
      ')'
    ;

windowFrame
    : frameType=RANGE start=frameBound
    | frameType=ROWS start=frameBound
    | frameType=RANGE BETWEEN start=frameBound AND end=frameBound
    | frameType=ROWS BETWEEN start=frameBound AND end=frameBound
    ;

frameBound
    : UNBOUNDED boundType=PRECEDING                 #unboundedFrame
    | UNBOUNDED boundType=FOLLOWING                 #unboundedFrame
    | CURRENT ROW                                   #currentRowBound
    | expression boundType=(PRECEDING | FOLLOWING)  #boundedFrame // expression should be unsignedLiteral
    ;


explainOption
    : FORMAT value=(TEXT | GRAPHVIZ | JSON)                 #explainFormat
    | TYPE value=(LOGICAL | DISTRIBUTED | VALIDATE | IO)    #explainType
    ;

transactionMode
    : ISOLATION LEVEL levelOfIsolation    #isolationLevel
    | READ accessMode=(ONLY | WRITE)      #transactionAccessMode
    ;

levelOfIsolation
    : READ UNCOMMITTED                    #readUncommitted
    | READ COMMITTED                      #readCommitted
    | REPEATABLE READ                     #repeatableRead
    | SERIALIZABLE                        #serializable
    ;

callArgument
    : expression                          #positionalArgument
    | identifier[null] '=>' expression    #namedArgument
    ;

privilege
    : SELECT | DELETE | INSERT | identifier[null]
    ;

qualifiedName returns [QsiQualifiedIdentifier qqi] locals [List<QsiIdentifier> buffer]
    @init { $buffer = new List<QsiIdentifier>(); }
    @after { $qqi = new QsiQualifiedIdentifier($buffer); }
    : identifier[$buffer] ('.' identifier[$buffer] )*
    ;

grantor
    : CURRENT_USER          #currentUserGrantor
    | CURRENT_ROLE          #currentRoleGrantor
    | principal             #specifiedPrincipal
    ;

principal
    : USER identifier[null]       #userPrincipal
    | ROLE identifier[null]       #rolePrincipal
    | identifier[null]            #unspecifiedPrincipal
    ;

roles
    : identifier[null] (',' identifier[null])*
    ;

identifier[List<QsiIdentifier> buffer] returns [QsiIdentifier qi]
    @after { $buffer?.Add($qi); }
    : i=IDENTIFIER { $qi = new QsiIdentifier($i.text.ToUpper(), false); }             #unquotedIdentifier
    | i=QUOTED_IDENTIFIER { $qi = new QsiIdentifier($i.text, true); }                 #quotedIdentifier
    | ki=nonReserved { $qi = new QsiIdentifier($ki.text.ToUpper(), false); }          #unquotedIdentifier
    | i=BACKQUOTED_IDENTIFIER { $qi = new QsiIdentifier($i.text, true); }             #backQuotedIdentifier
    | i=DIGIT_IDENTIFIER { $qi = new QsiIdentifier($i.text.ToUpper(), false); }       #digitIdentifier
    ;

number
    : DECIMAL_VALUE  #decimalLiteral
    | DOUBLE_VALUE   #doubleLiteral
    | INTEGER_VALUE  #integerLiteral
    ;

nonReserved
    // IMPORTANT: this rule must only contain tokens. Nested rules are not supported. See SqlParser.exitNonReserved
    : ADD | AFTER | ADMIN | ALL | ANALYZE | ANY | ARRAY | ASC | AT
    | BERNOULLI
    | CALL | CALLED | CASCADE | CATALOGS | CHANGE | COLUMN | COLUMNS | COMMENT | COMMIT | COMMITTED | CURRENT | CURRENT_ROLE
    | DATA | DATE | DAY | DEFINER | DESC | DETERMINISTIC | DISTRIBUTED
    | EXCLUDING | EXPLAIN | EXTERNAL
    | FILTER | FIRST | FOLLOWING | FORMAT | FUNCTION | FUNCTIONS
    | GRANT | GRANTED | GRANTS | GRAPHVIZ
    | HOUR
    | IF | IGNORE | INCLUDING | INPUT | INTERVAL | INVOKER | IO | ISOLATION
    | JSON
    | LANGUAGE | LAST | LATERAL | LEVEL | LIMIT | LOGICAL
    | MAP | MATERIALIZED | MINUTE | MONTH
    | NAME | NFC | NFD | NFKC | NFKD | NO | NONE | NULLIF | NULLS
    | OFFSET | ONLY | OPTION | ORDINALITY | OUTPUT | OVER
    | PARTITION | PARTITIONS | POSITION | PRECEDING | PRIVILEGES | PROPERTIES
    | RANGE | READ | REFRESH | RENAME | REPEATABLE | REPLACE | RESET | RESPECT | RESTRICT | RETURN | RETURNS | REVOKE | ROLE | ROLES | ROLLBACK | ROW | ROWS
    | SCHEMA | SCHEMAS | SECOND | SECURITY | SERIALIZABLE | SESSION | SET | SETS | SQL
    | SHOW | SOME | START | STATS | SUBSTRING | SYSTEM
    | TABLES | TABLESAMPLE | TEMPORARY | TEXT | TIME | TIMESTAMP | TO | TRANSACTION | TRY_CAST | TYPE
    | UNBOUNDED | UNCOMMITTED | USE | USER
    | VALIDATE | VERBOSE | VIEW
    | WORK | WRITE
    | YEAR
    | ZONE
    | SYNC | PROTECTION | PURGE;

ADD: 'ADD';
ADMIN: 'ADMIN';
AFTER: 'AFTER';
ALL: 'ALL';
ALTER: 'ALTER';
ANALYZE: 'ANALYZE';
AND: 'AND';
ANY: 'ANY';
ARRAY: 'ARRAY';
AS: 'AS';
ASC: 'ASC';
AT: 'AT';
BERNOULLI: 'BERNOULLI';
BETWEEN: 'BETWEEN';
BY: 'BY';
CALL: 'CALL';
CALLED: 'CALLED';
CASCADE: 'CASCADE';
CASE: 'CASE';
CAST: 'CAST';
CATALOGS: 'CATALOGS';
CHANGE: 'CHANGE';
COLUMN: 'COLUMN';
COLUMNS: 'COLUMNS';
COMMENT: 'COMMENT';
COMMIT: 'COMMIT';
COMMITTED: 'COMMITTED';
CONSTRAINT: 'CONSTRAINT';
CREATE: 'CREATE';
CROSS: 'CROSS';
CUBE: 'CUBE';
CURRENT: 'CURRENT';
CURRENT_DATE: 'CURRENT_DATE';
CURRENT_ROLE: 'CURRENT_ROLE';
CURRENT_TIME: 'CURRENT_TIME';
CURRENT_TIMESTAMP: 'CURRENT_TIMESTAMP';
CURRENT_USER: 'CURRENT_USER';
DATA: 'DATA';
DATABASE: 'DATABASE';
DATE: 'DATE';
DAY: 'DAY';
DBPROPERTIES: 'DBPROPERTIES';
DEALLOCATE: 'DEALLOCATE';
DEFINER: 'DEFINER';
DELETE: 'DELETE';
DESC: 'DESC';
DESCRIBE: 'DESCRIBE';
DETERMINISTIC: 'DETERMINISTIC';
DISTINCT: 'DISTINCT';
DISTRIBUTED: 'DISTRIBUTED';
DROP: 'DROP';
ELSE: 'ELSE';
END: 'END';
ESCAPE: 'ESCAPE';
EXCEPT: 'EXCEPT';
EXCLUDING: 'EXCLUDING';
EXECUTE: 'EXECUTE';
EXISTS: 'EXISTS';
EXPLAIN: 'EXPLAIN';
EXTRACT: 'EXTRACT';
EXTERNAL: 'EXTERNAL';
FALSE: 'FALSE';
FILTER: 'FILTER';
FIRST: 'FIRST';
FOLLOWING: 'FOLLOWING';
FOR: 'FOR';
FORMAT: 'FORMAT';
FROM: 'FROM';
FULL: 'FULL';
FUNCTION: 'FUNCTION';
FUNCTIONS: 'FUNCTIONS';
GRANT: 'GRANT';
GRANTED: 'GRANTED';
GRANTS: 'GRANTS';
GRAPHVIZ: 'GRAPHVIZ';
GROUP: 'GROUP';
GROUPING: 'GROUPING';
HAVING: 'HAVING';
HOUR: 'HOUR';
IF: 'IF';
IGNORE: 'IGNORE';
IN: 'IN';
INCLUDING: 'INCLUDING';
INNER: 'INNER';
INPUT: 'INPUT';
INSERT: 'INSERT';
INTERSECT: 'INTERSECT';
INTERVAL: 'INTERVAL';
INTO: 'INTO';
INVOKER: 'INVOKER';
IO: 'IO';
IS: 'IS';
ISOLATION: 'ISOLATION';
JSON: 'JSON';
JOIN: 'JOIN';
LANGUAGE: 'LANGUAGE';
LAST: 'LAST';
LATERAL: 'LATERAL';
LEFT: 'LEFT';
LEVEL: 'LEVEL';
LIKE: 'LIKE';
LIMIT: 'LIMIT';
LOCATION: 'LOCATION';
LOCALTIME: 'LOCALTIME';
LOCALTIMESTAMP: 'LOCALTIMESTAMP';
LOGICAL: 'LOGICAL';
MAP: 'MAP';
MATERIALIZED: 'MATERIALIZED';
MINUTE: 'MINUTE';
MONTH: 'MONTH';
NAME: 'NAME';
NATURAL: 'NATURAL';
NFC : 'NFC';
NFD : 'NFD';
NFKC : 'NFKC';
NFKD : 'NFKD';
NO: 'NO';
NONE: 'NONE';
NORMALIZE: 'NORMALIZE';
NOT: 'NOT';
NULL: 'NULL';
NULLIF: 'NULLIF';
NULLS: 'NULLS';
OFFSET: 'OFFSET';
ON: 'ON';
ONLY: 'ONLY';
OPTION: 'OPTION';
OR: 'OR';
ORDER: 'ORDER';
ORDINALITY: 'ORDINALITY';
OUTER: 'OUTER';
OUTPUT: 'OUTPUT';
OVER: 'OVER';
PARTITION: 'PARTITION';
PARTITIONS: 'PARTITIONS';
POSITION: 'POSITION';
PRECEDING: 'PRECEDING';
PREPARE: 'PREPARE';
PRIVILEGES: 'PRIVILEGES';
PROPERTIES: 'PROPERTIES';
RANGE: 'RANGE';
READ: 'READ';
RECURSIVE: 'RECURSIVE';
REFRESH: 'REFRESH';
RENAME: 'RENAME';
REPEATABLE: 'REPEATABLE';
REPLACE: 'REPLACE';
RESET: 'RESET';
RESPECT: 'RESPECT';
RESTRICT: 'RESTRICT';
RETURN: 'RETURN';
RETURNS: 'RETURNS';
REVOKE: 'REVOKE';
RIGHT: 'RIGHT';
ROLE: 'ROLE';
ROLES: 'ROLES';
ROLLBACK: 'ROLLBACK';
ROLLUP: 'ROLLUP';
ROW: 'ROW';
ROWS: 'ROWS';
SCHEMA: 'SCHEMA';
SCHEMAS: 'SCHEMAS';
SECOND: 'SECOND';
SECURITY: 'SECURITY';
SELECT: 'SELECT';
SERIALIZABLE: 'SERIALIZABLE';
SESSION: 'SESSION';
SET: 'SET';
SETS: 'SETS';
SHOW: 'SHOW';
SOME: 'SOME';
SQL: 'SQL';
START: 'START';
STATS: 'STATS';
SUBSTRING: 'SUBSTRING';
SYSTEM: 'SYSTEM';
TABLE: 'TABLE';
TABLES: 'TABLES';
TABLESAMPLE: 'TABLESAMPLE';
TEMPORARY: 'TEMPORARY';
TEXT: 'TEXT';
THEN: 'THEN';
TIME: 'TIME';
TIMESTAMP: 'TIMESTAMP';
TO: 'TO';
TRANSACTION: 'TRANSACTION';
TRUE: 'TRUE';
TRY_CAST: 'TRY_CAST';
TYPE: 'TYPE';
UESCAPE: 'UESCAPE';
UNBOUNDED: 'UNBOUNDED';
UNCOMMITTED: 'UNCOMMITTED';
UNION: 'UNION';
UNNEST: 'UNNEST';
UNLOAD: 'UNLOAD';
USE: 'USE';
USER: 'USER';
USING: 'USING';
VALIDATE: 'VALIDATE';
VALUES: 'VALUES';
VERBOSE: 'VERBOSE';
VIEW: 'VIEW';
WHEN: 'WHEN';
WHERE: 'WHERE';
WITH: 'WITH';
WORK: 'WORK';
WRITE: 'WRITE';
YEAR: 'YEAR';
ZONE: 'ZONE';
PARTITIONED: 'PARTITIONED';
CLUSTERED: 'CLUSTERED';
BUCKETS: 'BUCKETS';
STORED: 'STORED';
SERDEPROPERTIES: 'SERDEPROPERTIES';
TBLPROPERTIES: 'TBLPROPERTIES';
DELIMITED: 'DELIMITED';
FIELDS: 'FIELDS';
TERMINATED: 'TERMINATED';
ESCAPED: 'ESCAPED';
COLLECTION: 'COLLECTION';
ITEMS: 'ITEMS';
KEYS: 'KEYS';
LINES: 'LINES';
DEFINED: 'DEFINED';
SERDE: 'SERDE';
SEQUENCEFILE: 'SEQUENCEFILE';
TEXTFILE: 'TEXTFILE';
RCFILE: 'RCFILE';
ORC: 'ORC';
PARQUET: 'PARQUET';
AVRO: 'AVRO';
INPUTFORMAT: 'INPUTFORMAT';
OUTPUTFORMAT: 'OUTPUTFORMAT';
EXTENDED: 'EXTENDED';
FORMATTED: 'FORMATTED';
MSCK: 'MSCK';
REPAIR: 'REPAIR';
DATABASES: 'DATABASES';
VIEWS: 'VIEWS';
SYNC: 'SYNC';
PROTECTION: 'PROTECTION';
PURGE: 'PURGE';

EQ  : '=';
NEQ : '<>' | '!=';
LT  : '<';
LTE : '<=';
GT  : '>';
GTE : '>=';

PLUS: '+';
MINUS: '-';
ASTERISK: '*';
SLASH: '/';
PERCENT: '%';
CONCAT: '||';

STRING
    : '\'' ( ~'\'' | '\'\'' )* '\''
    ;

UNICODE_STRING
    : 'U&\'' ( ~'\'' | '\'\'' )* '\''
    ;

// Note: we allow any character inside the binary literal and validate
// its a correct literal when the AST is being constructed. This
// allows us to provide more meaningful error messages to the user
BINARY_LITERAL
    :  'X\'' (~'\'')* '\''
    ;

INTEGER_VALUE
    : DIGIT+
    ;

DECIMAL_VALUE
    : DIGIT+ '.' DIGIT*
    | '.' DIGIT+
    ;

DOUBLE_VALUE
    : DIGIT+ ('.' DIGIT*)? EXPONENT
    | '.' DIGIT+ EXPONENT
    ;

IDENTIFIER
    : (LETTER | '_') (LETTER | DIGIT | '_' | '@' | ':')*
    ;

DIGIT_IDENTIFIER
    : DIGIT (LETTER | DIGIT | '_' | '@' | ':')+
    ;

QUOTED_IDENTIFIER
    : '"' ( ~'"' | '""' )* '"'
    ;

BACKQUOTED_IDENTIFIER
    : '`' ( ~'`' | '``' )* '`'
    ;

TIME_WITH_TIME_ZONE
    : 'TIME' WS 'WITH' WS 'TIME' WS 'ZONE'
    ;

TIMESTAMP_WITH_TIME_ZONE
    : 'TIMESTAMP' WS 'WITH' WS 'TIME' WS 'ZONE'
    ;

DOUBLE_PRECISION
    : 'DOUBLE' WS 'PRECISION'
    ;

fragment EXPONENT
    : 'E' [+-]? DIGIT+
    ;

fragment DIGIT
    : [0-9]
    ;

fragment LETTER
    : [A-Z]
    ;

SIMPLE_COMMENT
    : '--' ~[\r\n]* '\r'? '\n'? -> channel(HIDDEN)
    ;

BRACKETED_COMMENT
    : '/*' .*? '*/' -> channel(HIDDEN)
    ;

WS
    : [ \r\n\t]+ -> channel(HIDDEN)
    ;

// Catch-all for anything we can't recognize.
// We use this to be able to ignore and recover all the text
// when splitting statements with DelimiterLexer
UNRECOGNIZED
    : .
    ;