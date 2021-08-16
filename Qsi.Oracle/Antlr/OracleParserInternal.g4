parser grammar OracleParserInternal;

options {
    tokenVocab=OracleLexerInternal;
}

root
    : EOF
    | (block (SEMICOLON_SYMBOL EOF? | EOF))+
    ;

block
    : plsqlBlock
    | sqlplusCommand
    | statement
    | oracleStatement
    ;

plsqlBlock
    : ('<' '<' label '>' '>')* (DECLARE declareSection)? body
    ;

sqlplusCommand
    : '/'
    | sqlplusAtSignCommand
    | sqlplusDoubleAtSignCommand
    | sqlplusAcceptCommand
    | sqlplusAppendCommand
    | sqlplusArchiveLogCommand
    | sqlplusAttributeCommand
    | sqlplusBreakCommand
    | sqlplusBtitleCommand
    | sqlplusChangeCommand
    | sqlplusClearCommand
    | sqlplusColumnCommand
    | sqlplusComputeCommand
    | sqlplusConnectCommand
    | sqlplusCopyCommand
    | sqlplusDefineCommand
    | sqlplusDelCommand
    | sqlplusDescribeCommand
    | sqlplusDisconnectCommand
    | sqlplusEditCommand
    | sqlplusExecuteCommand
    | sqlplusExitCommand
    | sqlplusHelpCommand
    | sqlplusGetCommand
    | sqlplusHistoryCommand
    | sqlplusHostCommand
    | sqlplusInputCommand
    | sqlplusListCommand
    | sqlplusPasswordCommand
    | sqlplusPauseCommand
    | sqlplusPrintCommand
    | sqlplusPromptCommand
    | sqlplusRecoverCommand
    | sqlplusRepfooterCommand
    | sqlplusRepheaderCommand
    | sqlplusRunCommand
    | sqlplusSaveCommand
    | sqlplusSetCommand
    | sqlplusShowCommand
    | sqlplusShutdownCommand
    | sqlplusSpoolCommand
    | sqlplusStartCommand
    | sqlplusStartupCommand
    | sqlplusStoreCommand
    | sqlplusTimingCommand
    | sqlplusTtitleCommand
    ;

sqlplusAtSignCommand
    : '@' sqlplusLegalfilename
    ;

sqlplusDoubleAtSignCommand
    : '@' '@' sqlplusLegalfilename
    ;

sqlplusAcceptCommand
    : (ACC | ACCEPT) identifier (NUM | NUMBER | CHAR | DATE | BINARY_FORMAT | BINARY_DOUBLE)?
      ((FOR | FORMAT) stringLiteral)? ((DEF | DEFAULT) stringLiteral)? (PROMPT (stringLiteral | (NOPR | NOPROMPT))) HIDE?
    ;

sqlplusAppendCommand
    : (KW_A | APPEND) .+?
    ;

sqlplusArchiveLogCommand
    : ARCHIVE LOG LIST
    ;

sqlplusAttributeCommand
    : (ATTRIBUTE | ATTR) sqlplusLegalfilename sqlplusAttributeOptions*
    ;

sqlplusBreakCommand
    : (BRE | BREAK) sqlplusBreakOnSomething+
    ;

sqlplusBtitleCommand
    : (BTI | BTITLE) (sqlplusBtitlePrintSpec stringLiteral?)+ (ON | OFF)?
    ;

sqlplusChangeCommand
    : (KW_C | CHANGE) .+?
    ;

sqlplusClearCommand
    : (CL | CLEAR) sqlplusClearOptions+
    ;

sqlplusColumnCommand
    : (COL | COLUMN) .+?
    ;

sqlplusComputeCommand
    : COMPUTE identifier (LAB | LABEL) stringLiteral OF identifier ON identifier
    ;

sqlplusConnectCommand
    : (CONN | CONNECT) .+?
    ;

sqlplusCopyCommand
    : COPY (FROM sqlplusDatabase | TO sqlplusDatabase | FROM sqlplusDatabase TO sqlplusDatabase)
      (APPEND | CREATE | INSERT | REPLACE) identifier ('(' identifier (',' identifier)* ')')?
      USING oracleStatement
    ;

sqlplusDefineCommand
    : (DEF | DEFINE) (identifier | identifier '=' stringLiteral)
    ;

sqlplusDelCommand
    : DEL ( numberLiteral (numberLiteral | '*')
          | '*'? LAST
          | '*' numberLiteral
          | '*'
          )
    ;

sqlplusDescribeCommand
    : (DESC | DESCRIBE) (identifierFragment '.')? identifierFragment ('@' identifierFragment)?
    ;

sqlplusDisconnectCommand
    : DISC 
    | DISCONNECT
    ;

sqlplusEditCommand
    : (ED | EDIT) sqlplusLegalfilename
    ;

sqlplusExecuteCommand
    : (EXEC | EXECUTE) statement
    ;

sqlplusExitCommand
    : (EXIT | QUIT) (identifierFragment | numberLiteral | bindVariable)
    ;

sqlplusHelpCommand
    : (HELP | '?') identifierFragment
    ;

sqlplusGetCommand
    : GET sqlplusLegalfilename (LIST | NOLIST)?
    ;

sqlplusHistoryCommand
    : (HIST | HISTORY) ( numberLiteral? ((KW_R| RUN) | (KW_E | EDIT) | (KW_D | DELETE))
                       | CLEAR
                       | LIST
                       )
    ;

sqlplusHostCommand
    : (HO | HOST) .+?
    ;

sqlplusInputCommand
    : (KW_I | INPUT) .+?
    ;

sqlplusListCommand
    : (KW_L | LIST) ( numberLiteral (numberLiteral | '*')
                    | '*'? LAST
                    | '*' numberLiteral
                    | '*'
                    )
    ;

sqlplusPasswordCommand
    : (PASSW | PASSWORD) identifierFragment
    ;

sqlplusPauseCommand
    : (PAU | PAUSE) .+?
    ;

sqlplusPrintCommand
    : PRINT identifierFragment
    ;

sqlplusPromptCommand
    : (PRO | PROMPT) .+?
    ;

sqlplusRecoverCommand
    : RECOVER (sqlplusRecoverGeneralClause | sqlplusRecoverManagedClause | BEGIN BACKUP | END BACKUP)
    ;

// TODO: REM | REMARK ...

sqlplusRepfooterCommand
    : (REPF | REPFOOTER) PAGE? sqlplusRepFooterOrHeaderCommandPrintspec*
    ;

sqlplusRepheaderCommand
    : (REPH | REPHEADER) PAGE? sqlplusRepFooterOrHeaderCommandPrintspec*
    ;

sqlplusRunCommand
    : KW_R
    | RUN
    ;

sqlplusSaveCommand
    : (SAV | SAVE) sqlplusLegalfilename (CRE | CREATE | REP | REPLACE | APP | APPEND)?
    ;

sqlplusSetCommand
    : SET identifierFragment (literal | identifierFragment)?
    ;

sqlplusShowCommand
    : (SHO | SHOW) sqlplusShowCommandOption
    ;

sqlplusShutdownCommand
    : SHUTDOWN (ABORT | IMMEDIATE | NORMAL | TRANSACTIONAL LOCAL?)
    ;

sqlplusSpoolCommand
    : (SPO | SPOOL) sqlplusLegalfilename (CRE | CREATE | REP | REPLACE | APP | APPEND | OFF)?
    ;

sqlplusStartCommand
    : (STA | START) sqlplusLegalfilename literal*
    ;

sqlplusStartupCommand
    : STARTUP (sqlplusDbOptions | sqlplusCdbOptions | sqlplusUpgradeOptions)
    ;

sqlplusStoreCommand
    : STORE SET sqlplusLegalfilename (CRE | CREATE | REP | REPLACE | APP | APPEND)?
    ;

sqlplusTimingCommand
    : (TIMI | TIMING) (START stringLiteral | SHOW | STOP)
    ;

sqlplusTtitleCommand
    : (TTI | TTITLE) (sqlplusTtitlePrintspec identifierFragment?)* (ON | OFF)?
    ;

sqlplusUndefineCommand
    : (UNDEF | UNDEFINE) identifierFragment
    ;

sqlplusVariableCommand
    : (VAR | VARIABLE) (identifierFragment (datatype ('=' literal)?)?)?
    ;

sqlplusWheneverCommand
    : WHENEVER (OSERROR | SQLERROR) ( EXIT (SUCCESS | FAILURE | numberLiteral | identifierFragment | bindVariable)? (COMMIT | ROLLBACK)?
                                    | CONTINUE (COMMIT | ROLLBACK | NONE)?)
    ;

sqlplusXqueryCommand
    : XQUERY .+?
    ;

sqlplusTtitlePrintspec
    : BOLD
    | CE | CENTER
    | COL numberLiteral
    | FORMAT stringLiteral
    | LE | LEFT
    | KW_R | RIGHT
    | (KW_S | KW_SKIP) numberLiteral
    | TAB numberLiteral
    ;

sqlplusAttributeOptions
    : (ALI | ALIAS) identifier
    | (CLE | CLEAR)
    | (FOR | FORMAT) identifier
    | LIKE sqlplusLegalfilename
    | ON
    | OFF
    ;

sqlplusDbOptions
    : FORCE? RESTRICT? (PFILE '=' stringLiteral)? QUIET? 
      (MOUNT identifierFragment? | OPEN sqlplusOpenDbOptions? identifierFragment? | NOMOUNT)?
    ;

sqlplusOpenDbOptions
    : READ (ONLY | WRITE RECOVER?) RECOVER
    ;

sqlplusCdbOptions
    : sqlplusRootConnectionOptions
    | sqlplusPdbConnectionOptions
    ;

sqlplusRootConnectionOptions
    : PLUGGABLE DATABASE identifierFragment (FORCE | UPGRADE | RESTRICT | OPEN sqlplusOpenPdbOptions?)?
    ;

sqlplusPdbConnectionOptions
    : FORCE
    | UPGRADE
    | RESTRICT
    | OPEN sqlplusOpenPdbOptions
    ;

sqlplusOpenPdbOptions
    : READ (WRITE | ONLY)
    ;

sqlplusUpgradeOptions
    : (PFILE '=' identifierFragment)? (UPGRADE | DOWNGRADE) QUIET?
    ;

sqlplusShowCommandOption
    : identifierFragment
    | ALL
    | BTI | BTITLE
    | CON_ID
    | CON_NAME
    | EDITION
    | (ERR | ERRORS) (ANALYTIC VIEW 
                     | ATTRIBUTE DIMENSION 
                     | HIERARCHY 
                     | FUNCTION 
                     | PROCEDURE 
                     | PACKAGE 
                     | PACKAGE BODY 
                     | TRIGGER 
                     | VIEW 
                     | TYPE 
                     | TYPE BODY 
                     | DIMENSION 
                     | JAVA CLASS 
                     )? (identifierFragment '.')? identifierFragment
    ;

sqlplusRepFooterOrHeaderCommandPrintspec
    : COL numberLiteral
    | (KW_S | KW_SKIP) numberLiteral
    | TAB numberLiteral
    | LE | LEFT
    | CE | CENTER
    | KW_R | RIGHT
    | BOLD
    | FORMAT stringLiteral
    ;

sqlplusRecoverGeneralClause
    : AUTOMATIC? (FROM stringLiteral)?
      (fullDatabaseRecovery | partialDatabaseRecovery | LOGFILE stringLiteral)
      (TEST | ALLOW integer CORRUPTION | parallelClause)*
    | CONTINUE DEFAULT?
    | CANCEL
    ;

sqlplusRecoverManagedClause
    : MANAGED STANDBY DATABASE (sqlplusRecoverRecoverClause | sqlplusRecoverCancelClause | sqlplusRecoverFinishClause)
    ;

sqlplusRecoverRecoverClause
    : (( DISCONNECT (FROM SESSION)?  | ( TIMEOUT integer | NOTIMEOUT ))
      | (NODELAY | DEFAULT DELAY | DELAY integer )
      | NEXT integer  
      | (EXPIRE integer | NO EXPIRE )
      | parallelClause 
      | USING CURRENT LOGFILE 
      | UNTIL CHANGE integer
      | THROUGH ( (THREAD integer)? SEQUENCE integer 
                | ALL ARCHIVELOG
                | (ALL | LAST | NEXT) SWITCHOVER)
      )+
    ;

sqlplusRecoverCancelClause
    : CANCEL IMMEDIATE? (WAIT | NOWAIT)
    ;

sqlplusRecoverFinishClause
    : (DISCONNECT (FROM SESSION)?)? (parallelClause)?
      FINISH (KW_SKIP (STANDBY LOGFILE)?)? (WAIT | NOWAIT)?
    ;

sqlplusDatabase
    : identifier ('/' identifier)? '@' identifier
    ;

sqlplusBreakOnSomething
    : ON identifier identifier?
    ;

sqlplusClearOptions
    : BRE | BREAKS
    | BUFF | BUFFER
    | COL | COLUMNS
    | COMP | COMPUTES
    | SCR | SCREEN
    | SQL
    | TIMI | TIMING
    ;

sqlplusBtitlePrintSpec
    : BOLD
    | CE
    | CENTER
    | COL numberLiteral
    | FORMAT stringLiteral
    | LE
    | LEFT
    | KW_R
    | RIGHT
    | (KW_S | KW_SKIP) numberLiteral
    | TAB numberLiteral
    ;

sqlplusLegalfilename
    : sqlplusFileNameFragment+
    ;

sqlplusFileNameFragment
    : identifier
    | '.'
    | numberLiteral
    ;

oracleStatement
    : select
    | delete
    | commit
    | create
    | alter
    | drop
    | grant
    | insert
    | update
    | merge
    | noaudit
    | purge
    | rename
    | revoke
    | rollback
    | savepoint
    | set
    | truncate
    | lock
    | flashback
    | explain
    | administerKeyManagement
    | analyze
    | audit
    | call
    | associateStatistics
    | disassociateStatistics
    | comment
    | deallocateUnused
    ;

select
    : subquery forUpdateClause?
    ;

delete
    : DELETE hint? FROM? (dmlTableExpressionClause | ONLY '(' dmlTableExpressionClause ')')
      tAlias? whereClause? returningClause? errorLoggingClause?
    ;

commit
    : COMMIT WORK? ( (COMMENT stringLiteral)? (WRITE (WAIT | NOWAIT)? (IMMEDIATE | BATCH)?)?
                   | FORCE stringLiteral ( ',' integer )?
                   )
    ;

grant
    : GRANT ( (grantSystemPrivileges | grantObjectPrivileges) (CONTAINER '=' (CURRENT | ALL))?
            | grantRolesToPrograms
            )
    ;

create
    : createAnalyticView
    | createAttributeDimension
    | createAuditPolicy
    | createCluster
    | createContext
    | createControlfile
    | createDatabase
    | createDatabaseLink
    | createDimension
    | createDirectory
    | createDiskgroup
    | createEdition
    | createFlashbackArchive
    | createFunction
    | createHierarchy
    | createIndex
    | createIndextype
    | createInmemoryJoinGroup
    | createJava
    | createLibrary
    | createLockdownProfile
    | createMaterializedView
    | createMaterializedViewLog
    | createMaterializedZonemap
    | createOperator
    | createOutLine
    | createPackage
    | createPackageBody
    | createPfile
    | createPluggableDatabase
    | createPmemFileStore
    | createProcedure
    | createProfile
    | createRestorePoint
    | createRole
    | createRollbackSegment
    | createSchema
    | createSequence
    | createSpfile
    | createSynonym
    | createTable
    | createTablespace
    | createTablespaceSet
    | createTrigger
    | createType
    | createTypeBody
    | createUser
    | createView
    ;

alter
    : alterAnalyticView
    | alterAttributeDimension
    | alterAuditPolicy
    | alterCluster
    | alterDatabase
    | alterDatabaseDictionary
    | alterDatabaseLink
    | alterDimension
    | alterDiskgroup
    | alterFlashbackArchive
    | alterFunction
    | alterHierarchy
    | alterIndex
    | alterIndextype
    | alterInmemoryJoinGroup
    | alterJava
    | alterLibrary
    | alterLockdownProfile
    | alterMaterializedView
    | alterMaterializedViewLog
    | alterMaterializedZonemap
    | alterOperator
    | alterOutline
    | alterPackage
    | alterPluggableDatabase
    | alterPmemFilestore
    | alterProcedure
    | alterProfile
    | alterResourceCost
    | alterRole
    | alterRollbackSegment
    | alterSequence
    | alterSession
    | alterSynonym
    | alterSystem
    | alterTable
    | alterTablespace
    | alterTablespaceSet
    | alterTrigger
    | alterType
    | alterUser
    | alterView
    ;

drop
    : dropAnalyticView
    | dropAttributeDimension
    | dropAuditPolicy
    | dropCluster
    | dropContext
    | dropDatabase
    | dropDatabaseLink
    | dropDimension
    | dropDirectory
    | dropDiskgroup
    | dropEdition
    | dropFlashbackArchive
    | dropFunction
    | dropHierarchy
    | dropIndex
    | dropIndextype
    | dropInmemoryJoinGroup
    | dropJava
    | dropLibrary
    | dropLockdownProfile
    | dropMaterializedView
    | dropMaterializedViewLog
    | dropMaterializedZonemap
    | dropOperator
    | dropOutline
    | dropPackage
    | dropPluggableDatabase
    | dropPmemFilestore
    | dropProcedure
    | dropProfile
    | dropRestorePoint
    | dropRole
    | dropRollbackSegment
    | dropSequence
    | dropSynonym
    | dropTable
    | dropTablespace
    | dropTablespaceSet
    | dropTrigger
    | dropType
    | dropTypeBody
    | dropUser
    | dropView
    ;

createCluster
    : CREATE CLUSTER ( schema '.' )? cluster
        '(' column datatype ( COLLATE columnCollationName )? SORT?
          ( ',' column datatype ( COLLATE columnCollationName )? SORT? )*
        ')'
        ( physicalAttributesClause
          | SIZE sizeClause
          | TABLESPACE tablespace
          | ( INDEX
            | ( SINGLE TABLE )?
              HASHKEYS integer ( HASH IS expr )?
            )
        )*
        parallelClause?
        ( NOROWDEPENDENCIES | ROWDEPENDENCIES )?
        ( CACHE | NOCACHE )? clusterRangePartitions?
    ;

clusterRangePartitions
    : PARTITION BY RANGE '(' column ( ',' column )* ')'
      '(' PARTITION partition?
          rangeValuesClause tablePartitionDescription
            ( ',' PARTITION partition?
              rangeValuesClause tablePartitionDescription
            )*
      ')'
    ;

createContext
    : CREATE ( OR REPLACE )? CONTEXT namespace
        USING ( schema '.' )? packageName
        ( INITIALIZED ( EXTERNALLY | GLOBALLY )
        | ACCESSED GLOBALLY
        )?
    ;

createControlfile
    : CREATE CONTROLFILE
        REUSE? SET? DATABASE database
        logfileClause?
        ( RESETLOGS | NORESETLOGS )
        ( DATAFILE fileSpecification
                   ( ',' fileSpecification )* )?
        ( MAXLOGFILES integer
        | MAXLOGMEMBERS integer
        | MAXLOGHISTORY integer
        | MAXDATAFILES integer
        | MAXINSTANCES integer
        | ( ARCHIVELOG | NOARCHIVELOG )
        | FORCE LOGGING
        | SET STANDBY NOLOGGING FOR ( DATA AVAILABILITY | LOAD PERFORMANCE )
        )*
        characterSetClause?
    ;

logfileClause
    : LOGFILE ( GROUP integer )? fileSpecification ( ',' ( GROUP integer)? fileSpecification )*
    ;

characterSetClause
    : CHARACTER SET characterSet
    ;

createDatabaseLink
    : CREATE SHARED? PUBLIC? DATABASE LINK dblink
        ( CONNECT TO
          ( CURRENT_USER
          | user IDENTIFIED BY password dblinkAuthentication?
          )
        | dblinkAuthentication
        )*
        ( USING connectString )?
    ;

createDimension
    : CREATE DIMENSION ( schema '.' )? dimensionName
        levelClause+
        ( hierarchyClause
        | attributeClause
        | extendedAttributeClause
        )*
    ;

createDirectory
    : CREATE ( OR REPLACE )? DIRECTORY directoryName
        ( SHARING '=' ( METADATA | NONE ) )?
        AS pathName
    ;

createDiskgroup
    : CREATE DISKGROUP diskgroupName
        ( ( HIGH | NORMAL | FLEX | EXTENDED ( SITE siteName ) | EXTERNAL ) REDUNDANCY )?
        ( ( QUORUM | REGULAR )? ( FAILGROUP failgroupName )?
          DISK qualifiedDiskClause ( ',' qualifiedDiskClause )*
        )*
        ( ATTRIBUTE ( stringLiteral '=' stringLiteral )
                    ( ',' stringLiteral '=' stringLiteral )* )?
    ;

createEdition
    : CREATE EDITION editionName
        ( AS CHILD OF parentEdition )?
    ;

createFlashbackArchive
    : CREATE FLASHBACK ARCHIVE DEFAULT? flashbackArchiveName
        TABLESPACE tablespace
        flashbackArchiveQuota?
        ( NO? OPTIMIZE DATA )?
        flashbackArchiveRetention
    ;

createFunction
    : CREATE ( OR REPLACE )?
      ( EDITIONABLE | NONEDITIONABLE )?
      FUNCTION plsqlFunctionSource
    ;

plsqlFunctionSource
    : ( schema '.' )? functionName
        ( '(' parameterDeclaration ( ',' parameterDeclaration )* ')' )? RETURN datatype 
      sharingClause?
        ( invokerRightsClause
          | accessibleByClause
          | defaultCollationClause    
          | deterministicClause
          | parallelEnableClause
          | resultCacheClause 
          | aggregateClause
          | pipelinedClause
          | sqlMacroClause
        )*
      ( IS | AS ) ( declareSection? body 
                    | callSpec
                  )
    ;

parameterDeclaration
    : parameter ( IN? datatype ( ( ':''=' | DEFAULT ) expr )?
                | ( OUT | IN OUT ) NOCOPY? datatype
                )
    ;

defaultCollationClause
    : DEFAULT COLLATION collationOption
    ;

collationOption
    : USING_NLS_COMP
    ;

deterministicClause
    : DETERMINISTIC
    ;

parallelEnableClause
    : PARALLEL_ENABLE ( '(' PARTITION argument BY ( ANY
                                                 | ( HASH | RANGE ) '(' column ( ',' column )* ')' streamingClause?
                                                 | VALUE '(' column ')'
                                                 ) ')' )?
    ;

streamingClause
    : ( ORDER | CLUSTER ) expr BY '(' column ( ',' column )* ')'
    ;

aggregateClause
    : AGGREGATE USING ( schema '.' )? implementationType
    ;

pipelinedClause
    : PIPELINED
      ( ( USING ( schema '.' )? implementationType )?
      | ( ROW | TABLE ) POLYMORPHIC ( USING ( schema '.' )? implementationPackage )?
      )
    ;

sqlMacroClause
    : SQL_MACRO ( '(' ( TYPE '=' '>' )? ( SCALAR | TABLE ) ')' )?
    ;

declareSection
    : itemList1 itemList2?
    | itemList2
    ;

itemList1
    : ( typeDefinition
      | cursorDeclaration
      | itemDeclaration
      | functionDeclaration
      | procedureDeclaration
      )+
    ;

itemList2
    : ( cursorDeclaration
      | cursorDefinition
      | functionDeclaration
      | functionDefinition
      | procedureDeclaration
      | procedureDefinition
      )+
    ;

procedureDefinition
    : procedureDeclaration ( IS | AS ) ( declareSection? body | callSpec ) ';'
    ;

functionDeclaration
    : functionHeading ( DETERMINISTIC | PIPELINED | PARALLEL_ENABLE | RESULT_CACHE resultCacheClause? )* ';'
    ;

functionHeading
    : FUNCTION functionName '(' parameterDeclaration ( ',' parameterDeclaration )* ')' RETURN datatype
    ;

functionDefinition
    : functionHeading ( DETERMINISTIC | PIPELINED | PARALLEL_ENABLE | RESULT_CACHE resultCacheClause? )*
        ( IS | AS ) ( declareSection? body | callSpec )
    ;

procedureDeclaration
    : procedureHeading procedureProperties
    ;

body
    : BEGIN statement* ( EXCEPTION exceptionHandler+ )? END name? ';'
    ;

exceptionHandler
    : WHEN ( exceptionMessage ( OR exceptionMessage )* | OTHERS ) THEN statement+
    ;

procedureHeading
    : PROCEDURE procedureName '(' parameterDeclaration ( ',' parameterDeclaration )* ')'
    ;

procedureProperties
    : ( accessibleByClause
      | defaultCollationClause
      | invokerRightsClause
      )*
    ;

itemDeclaration
    : collectionVariableDecl
    | constantDeclaration
    | cursorVariableDeclaration
    | exceptionDeclaration
    | recordVariableDeclaration
    | variableDeclaration
    ;

plsqlDatatype
    : collectionOrRecordOrRefCursorType=identifier
    | REF? objectType
    | rowTypeAttribute
// WARNING: no documentation
    | scalarDatatype=datatype
    | typeAttribute
    ;

typeAttribute
    : identifier ('.' identifier)? MOD_SYMBOL TYPE
    ;

plsqlExpression
    : column                                                                                            #constantOrVariableExpression
    | '(' plsqlExpression ')'                                                                           #subqueryExpression
    | numberLiteral                                                                                     #numberLiteralExpression
    | booleanLiteral                                                                                    #booleanLiteralExpression
    | stringLiteral                                                                                     #stringLiteralExpression
    | functionCall                                                                                      #functionCallExpression
    | conditionalPredicate                                                                              #conditionalPredicateExpression
    | identifier '.' (COUNT | FIRST | LAST | LIMIT | (NEXT | PRIOR | EXISTS) '(' index ')')             #collectionFunctionsExpression
    | plsqlExpression IS NOT? NULL                                                                      #expressionIsNullExpression
    | <assoc=right> plsqlExpression NOT? BETWEEN plsqlExpression AND plsqlExpression                    #expressionBetweenExpression
    | <assoc=right> plsqlExpression NOT? IN '(' plsqlExpression (',' plsqlExpression)* ')'              #expressionInExpression
    | plsqlExpression NOT? LIKE stringLiteral                                                           #expressionLikeExpression
    | (plsqlNamedCursor | SQL) '%' (FOUND | ISOPEN | NOTFOUND | ROWCOUNT | BULK_ROWCOUNT '(' index ')') #cursorStateExpression
    | NOT plsqlExpression                                                                               #notExpression
    | ('-' | '+') plsqlExpression                                                                       #unarySignExpression
    | <assoc=right> plsqlExpression (AND | OR) plsqlExpression                                          #andOrExpression
    | <assoc=right> plsqlExpression relationalOperator plsqlExpression                                  #relationalOperatorExpression
    | <assoc=right> plsqlExpression '||' plsqlExpression                                                #characterConnectExpression
    | <assoc=right> plsqlExpression ('+' | '-' | '*' | '/' | '*' '*') plsqlExpression                   #signExpression
    | placeholder                                                                                       #plsqlPlaceholderExpression
    | <assoc=right> collectionType '(' (plsqlExpression (',' plsqlExpression)*)? ')'                    #plsqlCollectionConstructor
    | <assoc=right> CASE WHEN plsqlExpression THEN plsqlExpression
      (WHEN plsqlExpression THEN plsqlExpression)*
      (ELSE plsqlExpression)?
      END                                                                                               #plsqlSearchedCaseExpression
    | <assoc=right> CASE plsqlExpression WHEN plsqlExpression THEN plsqlExpression
      (WHEN plsqlExpression THEN plsqlExpression)*
      (ELSE plsqlExpression)?
      END                                                                                               #plsqlSimpleCaseExpression
    ;

booleanLiteral
    : TRUE 
    | FALSE 
    | NULL
    ;

conditionalPredicate
    : INSERTING 
    | UPDATING ('(' stringLiteral ')')? 
    | DELETING
    ;

relationalOperator
    : '='
    | '<>'
    | '!='
    | '~='
    | '^='
    | '<'
    | '>'
    | '<='
    | '>='
    ;

plsqlNamedCursorAttribute
    : plsqlNamedCursor '%' (ISOPEN | FOUND | NOTFOUND | ROWCOUNT)
    ;

plsqlNamedCursor
    : identifier
    | ':' identifier
    ;

variableDeclaration
    : variable plsqlDatatype ( ( NOT NULL )? ( ':' '=' | DEFAULT ) plsqlExpression )? ';'
    ;

recordVariableDeclaration
    : record1 ( recordType | rowTypeAttribute | record2 MOD_SYMBOL TYPE ) ';'
    ;

rowTypeAttribute
    : ( explicitCursorName | cursorVariableName | dbTableOrViewName ) MOD_SYMBOL ROWTYPE
    ;

collectionVariableDecl
    : newCollectionVar ( assocArrayType ( ':' '=' ( qualifiedExpression | functionCall | collectionVar1=collectionVar ) )?
                       | ( varrayType | nestedTableType ) ( ':' '=' ( collectionConstructor | collectionVar1=collectionVar ) )?
                       | collectionVar2=collectionVar MOD_SYMBOL TYPE
                       )
      ';'
    ;

cursorVariableDeclaration
    : cursorVariable type
    ;

constantDeclaration
    : constant CONSTANT plsqlDatatype ( NOT NULL )? ( ':' '=' | DEFAULT ) plsqlExpression ';'
    ;

exceptionDeclaration
    : exceptionMessage EXCEPTION
    ;

collectionConstructor
    : collectionType '(' value ( ',' value )* ')'
    ;

functionCall
    : functionName '(' ( plsqlParameter ( ',' plsqlParameter )* )? ')'
    ;

plsqlParameter
    : identifier
    | literal
    | plsqlExpression
    | identifier '=' '>' ( identifier | literal | plsqlExpression )
    ;

qualifiedExpression
    : typemark '(' aggregate ')'
    ;

typemark
    : typeName
    ;

typeName
    : identifier ( '.' identifier )*
    ;

aggregate
    : positionChoiceList? explicitChoiceList?
    ;

explicitChoiceList
    : ( ( namedChoiceList | indexChoiceList | basicIteratorChoice | indexIteratorChoice ) ','? )+
    ;

positionChoiceList
    : ( ( expr | sequenceIteratorChoice ) ','? )+
    ;

namedChoiceList
    : ( identifier '|'? )+ '=' '>' expr
    ;

indexChoiceList
    : ( expr '|'? )+ '=' '>' expr
    ;

basicIteratorChoice
    : FOR iterator '=' '>' expr
    ;

indexIteratorChoice
    : FOR iterator INDEX expr '=' '>' expr
    ;

sequenceIteratorChoice
    : FOR iterator SEQUENCE '=' '>' expr
    ;

cursorDeclaration
    : CURSOR cursor
        ( '(' cursorParameterDec ( ',' cursorParameterDec )* ')' )?
          RETURN rowtype
    ;

cursorDefinition
    : CURSOR cursor
       ( '(' cursorParameterDec ( ',' cursorParameterDec )* ')' )?
         ( RETURN rowtype )? IS select ';'
    ;

cursorParameterDec
    : parameter IN? datatype ( ( ':' '=' | DEFAULT ) plsqlExpression )?
    ;

rowtype
    : ( dbTableOrView | cursor | cursorVariable ) MOD_SYMBOL ROWTYPE
    | record MOD_SYMBOL TYPE
    | recordType
    ;

typeDefinition
    : collectionTypeDefinition
    | recordTypeDefinition
    | refCursorTypeDefinition
    | subtypeDefinition
    ;

collectionTypeDefinition
    : TYPE type IS 
        ( assocArrayTypeDef
        | varrayTypeDef
        | nestedTableTypeDef
        )
     ';'
    ;

assocArrayTypeDef
    : TABLE OF plsqlDatatype ( NOT NULL )?
      INDEX BY ( PLS_INTEGER | BINARY_INTEGER | ( VARCHAR | VARCHAR2 | STRING ) '(' integer ')' | plsqlDatatype )
    ;

varrayTypeDef
    : ( VARRAY | VARYING? ARRAY ) '(' integer ')'
        OF plsqlDatatype ( NOT NULL )?
    ;

nestedTableTypeDef
    : TABLE OF plsqlDatatype ( NOT NULL )?
    ;

recordTypeDefinition
    : TYPE recordType IS RECORD '(' fieldDefinition ( ',' fieldDefinition )* ')' ';'
    ;

fieldDefinition
    : field plsqlDatatype ( ( NOT NULL )? ( ':' '=' | DEFAULT ) plsqlExpression )?
    ;

refCursorTypeDefinition
    : TYPE type IS REF CURSOR
        ( RETURN
          ( ( dbTableOrView | cursor | cursorVariable ) MOD_SYMBOL ROWTYPE
          | record MOD_SYMBOL TYPE
          | recordType
          | refCursorType
          )
        )?
     ';'
    ;

subtypeDefinition
    : SUBTYPE subtype IS baseType ( plsqlConstraint | CHARACTER SET characterSet )?
        ( NOT NULL )?
      ';'
    ;

plsqlConstraint
    : precision (',' scale)?
    | RANGE numberLiteral '.' '.' numberLiteral
    ;

createHierarchy
    : CREATE ( OR REPLACE )? ( FORCE | NOFORCE )? 
        HIERARCHY ( schema '.' )? hierarchy 
        sharingClause?
        classificationClause*
        hierUsingClause
        levelHierClause
        hierAttrsClause?
    ;

hierUsingClause
    : USING ( schema '.' )? attributeDimension levelHierClause
    ;

levelHierClause
    : '(' ( level ( CHILD OF )? )+ ')'
    ;

hierAttrsClause
    : hierAttrName classificationClause*
    ;

hierAttrName
    : MEMBER_NAME
    | MEMBER_UNIQUE_NAME
    | MEMBER_CAPTION
    | MEMBER_DESCRIPTION
    | LEVEL_NAME
    | HIER_ORDER
    | DEPTH
    | IS_LEAF
    | PARENT_LEVEL_NAME
    | PARENT_UNIQUE_NAME
    ;

createIndextype
    : CREATE ( OR REPLACE )? INDEXTYPE ( schema '.' )? indextype
        FOR ( schema '.' )? operatorName '(' parameterType ( ',' parameterType )*')'
              ( ',' ( schema '.' )? operatorName '('parameterType ( ',' parameterType )*')'
              )*
        usingTypeClause
        ( WITH LOCAL RANGE? PARTITION )?
        storageTableClause?
    ;

createInmemoryJoinGroup
    : CREATE INMEMORY JOIN GROUP  ( schema '.' )? joinGroup
        '('  ( schema '.' )? table '(' column ')' ','  ( schema '.' )? table '(' column ')'
          ( ','  ( schema '.' )? table '(' column ')' )* ')'
    ;

createJava
    : CREATE ( OR REPLACE )? ( AND ( RESOLVE | COMPILE ) )? NOFORCE?
        JAVA ( ( SOURCE | RESOURCE ) NAMED ( schema '.' )? primaryName
             | CLASS ( SCHEMA schema )?
             )
        ( SHARING '=' ( METADATA | NONE ) )?
        invokerRightsClause?
        ( RESOLVER '(' ( '(' matchString ','? ( schemaName | '-' ) ')' )+ ')' )?
        ( USING ( BFILE '(' directoryObjectName ',' serverFileName ')'
                | ( CLOB | BLOB | BFILE ) subquery
                | stringLiteral
                )
        | AS sourceChar
        )
    ;

createLibrary
    : CREATE ( OR REPLACE )? ( EDITIONABLE | NOEDITIONABLE )? LIBRARY plsqlLibrarySource
    ;

plsqlLibrarySource
    : ( schema '.' )? libraryName sharingClause? ( IS | AS )
        ( stringLiteral | stringLiteral IN directoryObject )
        ( AGENT stringLiteral )? ( CREDENTIAL ( schema '.' )? credentialName )?
    ;

createLockdownProfile
    : CREATE LOCKDOWN PROFILE profileName ( staticBaseProfile | dynamicBaseProfile )?
    ;

staticBaseProfile
    : FROM baseProfile
    ;

dynamicBaseProfile
    : INCLUDING baseProfile
    ;

createMaterializedView
    : CREATE MATERIALIZED VIEW ( schema '.' )? materializedView
        ( OF ( schema '.' )? objectType )?
        ( '(' ( scopedTableRefConstraint
            | cAlias ( ENCRYPT encryptionSpec )?
            )
            ( ',' ( scopedTableRefConstraint
               | cAlias ( ENCRYPT encryptionSpec )?
               )
            )*
          ')'
        )?
        ( DEFAULT COLLATION collationName )?
        ( ON PREBUILT TABLE
          ( ( WITH | WITHOUT ) REDUCED PRECISION )?
        | physicalProperties? materializedViewProps
        )
        ( USING INDEX
          ( physicalAttributesClause
          | TABLESPACE tablespace
          )*
        | USING NO INDEX
        )?
        createMvRefresh?
        evaluationEditionClause?
        ( ( ENABLE | DISABLE ) ON QUERY COMPUTATION )?
        queryRewriteClause?
      AS subquery
    ;

createMvRefresh
    : REFRESH
        ( ( FAST | COMPLETE | FORCE )
        | ( ON DEMAND 
          | ON COMMIT 
          | ON STATEMENT
          )
        | ( START WITH expr |
            NEXT expr
          )+
        | WITH ( PRIMARY KEY | ROWID )
        | USING
           ( DEFAULT ( MASTER | LOCAL )? ROLLBACK SEGMENT
           | ( MASTER | LOCAL )? ROLLBACK SEGMENT rollbackSegment
           )+
        | USING
           ( ENFORCED | TRUSTED ) CONSTRAINTS
        )+
      | NEVER REFRESH
    ;

queryRewriteClause
    : ( ENABLE | DISABLE ) QUERY REWRITE unusableEditionsClause?
    ;

materializedViewProps
    : columnProperties?
      tablePartitioningClauses?
      ( CACHE | NOCACHE )?
      parallelClause?
      buildClause?
    ;

buildClause
    : BUILD ( IMMEDIATE | DEFERRED )
    ;

createMaterializedViewLog
    : CREATE MATERIALIZED VIEW LOG ON ( schema '.' )? table
        ( physicalAttributesClause
        | TABLESPACE tablespace
        | loggingClause
        | ( CACHE | NOCACHE )
        )*
        parallelClause?
        tablePartitioningClauses?
        ( WITH ( ( OBJECT ID
                     | PRIMARY KEY
                     | ROWID
                     | SEQUENCE '(' column ( ',' column )* ')'
                     | COMMIT SCN
                     )
                     ( ',' OBJECT ID
                     | ',' PRIMARY KEY
                     | ',' ROWID
                     | ',' SEQUENCE '(' column ( ',' column )* ')'
                     | ',' COMMIT SCN
                     )* 
                  )?
          ( '(' column ( ',' column )* ')' )?
          newValuesClause?
        )?
        mvLogPurgeClause?
        forRefreshClause?
    ;

createMaterializedZonemap
    : createZonemapOnTable
    | createZonemapAsSubquery
    ;

createZonemapOnTable
    : CREATE MATERIALIZED ZONEMAP
              ( schema '.' )? zonemapName
              zonemapAttributes?
              zonemapRefreshClause?
              ( ( ENABLE | DISABLE ) PRUNING )?
              ON ( schema '.' )? ( table | materializedView ) '(' column ( ',' column )* ')'
    ;

createZonemapAsSubquery
    : CREATE MATERIALIZED ZONEMAP
        ( schema '.' )? zonemapName
        zonemapAttributes?
        zonemapRefreshClause?
        ( ( ENABLE | DISABLE ) PRUNING )?
        AS queryBlock
    ;

zonemapAttributes
    : TABLESPACE tablespace
    | SCALE integer
    | ( CACHE | NOCACHE )
    ;

createOperator
    : CREATE ( OR REPLACE )? OPERATOR ( schema '.' )? operatorName bindingClause
    ;

bindingClause
    : BINDING '(' parameterTypes ')' RETURN returnType implementationClause? usingFunctionClause
        ( ',' '(' parameterTypes ')' RETURN returnType implementationClause? usingFunctionClause )*
    ;

createOutLine
    : CREATE ( OR REPLACE )?
        ( PUBLIC | PRIVATE )? OUTLINE outline?
        ( FROM ( PUBLIC | PRIVATE )? sourceOutline )?
        ( FOR CATEGORY categoryName )?
        ( ON statement )?
    ;

createPackage
    : CREATE ( OR REPLACE )?
        ( EDITIONABLE | NONEDITIONABLE )?
        PACKAGE plsqlPackageSource
    ;

plsqlPackageSource
    : ( schema '.' )? packageName sharingClause? ( defaultCollationClause | invokerRightsClause | accessibleByClause )*
         ( IS | AS ) packageItemList END packageName?
    ;

packageItemList
    : ( typeDefinition 
      | cursorDeclaration 
      | itemDeclaration 
      | packageFunctionDeclaration 
      | packageProcedureDeclaration 
      )+
    ;

packageFunctionDeclaration
    : functionHeading 
        ( accessibleByClause 
        | deterministicClause 
        | pipelinedClause 
        | parallelEnableClause 
        | resultCacheClause )?
    ;

packageProcedureDeclaration
    : procedureHeading accessibleByClause?
    ;

createPackageBody
    : CREATE ( OR REPLACE )? ( EDITIONABLE | NONEDITIONABLE )?
        PACKAGE BODY plsqlPackageBodySource
    ;

plsqlPackageBodySource
    : ( schema '.' )? packageName sharingClause?
        ( IS | AS ) declareSection initializeSection?
        END packageName?
    ;

initializeSection
    : BEGIN statement+ ( EXCEPTION exceptionHandler+ )?
    ;

createPfile
    : CREATE PFILE '=' stringLiteral FROM ( SPFILE '=' stringLiteral
                                          | MEMORY
                                          )
    ;

createPluggableDatabase
    : CREATE PLUGGABLE DATABASE
        ( pdbName ( AS APPLICATION CONTAINER )? | AS SEED )
        ( createPdbFromSeed
        | createPdbClone
        | createPdbFromXml
        | createPdbFromMirrorCopy
        | containerMapClause
        | pdbSnapshotClause
        )
    ;

containerMapClause
    : CONTAINER_MAP UPDATE '(' ( addTablePartition | splitTablePartition ) ')'
    ;

createPdbFromMirrorCopy
    : newPdbName FROM basePdbName '@' dblinkname
        USING MIRROR COPY mirrorName
    ;

createPdbFromXml
    :  ( AS CLONE )? USING filename
         ( sourceFileNameConvert | sourceFileDirectory )?
         ( ( COPY | MOVE )? fileNameConvert | NOCOPY )?
         ( serviceNameConvert )?
         ( defaultTablespace )?
         ( pdbStorageClause )?
         ( pathPrefixClause )?
         ( tempfileReuseClause )?
         ( userTablespacesClause )?
         ( standbysClause )?
         ( loggingClause )?
         ( createFileDestClause )?
         ( HOST '=' stringLiteral )?
         ( PORT '=' numberLiteral )?
         ( createPdbDecryptFromXml )?
    ;

sourceFileNameConvert
    : SOURCE_FILE_NAME_CONVERT '='
      ( '(' filenamePattern ','  filenamePattern ')' ( ',' '(' filenamePattern ',' filenamePattern ')' )* 
      | NONE
      )
    ;

sourceFileDirectory
    : SOURCE_FILE_DIRECTORY '=' ( stringLiteral | NONE )
    ;

createPdbDecryptFromXml
    : DECRYPT USING transportSecret
    ;

createPdbClone
    : ( FROM ( pdbName ( '@' dblink )? ) | ( NON '$' CDB '@' dblink )
      | AS PROXY FROM pdbName '@' dblink
      )
        parallelPdbCreationClause?
        defaultTablespace?
        pdbStorageClause?
        fileNameConvert?
        serviceNameConvert?
        pathPrefixClause?
        tempfileReuseClause?
        ( SNAPSHOT COPY )?
        usingSnapshotClause?
        userTablespacesClause?
        standbysClause?
        loggingClause?
        createFileDestClause?
        keystoreClause?
        pdbRefreshModeClause?
        ( RELOCATE AVAILABILITY  (MAX | NORMAL ) )?
        ( NO DATA )?
        ( HOST '=' stringLiteral )?
        ( PORT '=' numberLiteral )?
    ;

usingSnapshotClause
    : USING SNAPSHOT ( snapshotName
                     | AT SCN snapshotSCN
                     | AT snapshotTimestamp
                     )
    ;

keystoreClause
    : KEYSTORE IDENTIFIED BY keystorePassword
    ;

createPdbFromSeed
    : ADMIN USER adminUserName? IDENTIFIED BY password
        pdbDbaRoles?
        parallelPdbCreationClause?
        defaultTablespace?
        pdbStorageClause?
        fileNameConvert?
        serviceNameConvert?
        pathPrefixClause?
        tempfileReuseClause?
        containerMapClause?
        userTablespacesClause?
        standbysClause?
        loggingClause?
        createFileDestClause?
        ( HOST '=' stringLiteral )?
        ( PORT '=' numberLiteral )?
    ;

pdbDbaRoles
    : ROLES '=' '(' role ( ',' role )* ')'
    ;

parallelPdbCreationClause
    : PARALLEL integer?
    ;

 serviceNameConvert
    : SERVICE_NAME_CONVERT '='
      ( '(' serviceName ',' serviceName ')' ( ',' '(' serviceName ',' serviceName ')' )* 
      | NONE
      )
    ;

pathPrefixClause
    : PATH_PREFIX '=' ( stringLiteral | directoryObjectName | NONE )
    ;

tempfileReuseClause
    : TEMPFILE REUSE
    ;

userTablespacesClause
    : USER_TABLESPACES '='
        ( '(' tablespace ( ',' tablespace )* ')'
        | ALL ( EXCEPT '(' tablespace ( ',' tablespace )* ')' )?
        | NONE
        )
        ( SNAPSHOT COPY | NO DATA | COPY | MOVE | NOCOPY )?
    ;

standbysClause
    : STANDBYS '=' ( '(' cdbName ( ',' cdbName )* ')'
                   | ( ALL ( EXCEPT '(' cdbName ( ',' cdbName )* ')' )? )
                   | NONE
                   )
    ;

createFileDestClause
    : CREATE_FILE_DEST '=' ( NONE | stringLiteral | diskgroupName )
    ;

createPmemFileStore
    : CREATE PMEM FILESTORE filestoreName
        ( MOUNTPOINT filePath
        | BACKINGFILE fileName REUSE?
        | SIZE sizeClause
        | BLOCK SIZE sizeClause
        | autoextendClause
        )
    ;

createProcedure
    : CREATE ( OR REPLACE )?
        ( EDITIONABLE | NONEDITIONABLE )?
        PROCEDURE plsqlProcedureSource
    ;

plsqlProcedureSource
    : ( schema '.' )? procedureName
        ( '(' parameterDeclaration ( ',' parameterDeclaration )* ')' )? sharingClause?
        ( defaultCollationClause | invokerRightsClause | accessibleByClause)*
        ( IS | AS ) ( declareSection? body | callSpec )
    ;

createProfile
    : CREATE MANDATORY? PROFILE profile
        LIMIT ( resourceParameters
              | passwordParameters
              )+
        ( CONTAINER '=' ( CURRENT | ALL ) )?
    ;

createRestorePoint
    : CREATE CLEAN? RESTORE POINT restorePoint
        ( FOR PLUGGABLE DATABASE pdbName )?
        ( AS OF ( TIMESTAMP | SCN ) expr )?
        ( PRESERVE
        | GUARANTEE FLASHBACK DATABASE
        )?
    ;

createRole
    : CREATE ROLE role
        ( NOT IDENTIFIED
        | IDENTIFIED ( BY password
                     | USING ( schema '.' )? packageName
                     | EXTERNALLY
                     | GLOBALLY AS domainNameOfDirectoryGroup
                     )
        )? ( CONTAINER '=' ( CURRENT | ALL ) )?
    ;

createRollbackSegment
    : CREATE PUBLIC? ROLLBACK SEGMENT rollbackSegment
        ( TABLESPACE tablespace | storageClause )*
    ;

createSequence
    : CREATE SEQUENCE ( schema '.' )? sequence
        ( SHARING '=' ( METADATA | DATA | NONE ) )?
        ( ( INCREMENT BY | START WITH ) integer
        | ( MAXVALUE integer | NOMAXVALUE )
        | ( MINVALUE integer | NOMINVALUE )
        | ( CYCLE | NOCYCLE )
        | ( CACHE integer | NOCACHE )
        | ( ORDER | NOORDER )
        | ( KEEP | NOKEEP )
        | ( SCALE ( EXTEND | NOEXTEND ) | NOSCALE )
        | ( SESSION | GLOBAL )
        )*
    ;

createSpfile
    : CREATE SPFILE ( '=' stringLiteral )?
        FROM ( PFILE ( '=' stringLiteral )? ( AS COPY )?
             | MEMORY
             )
    ;

createTablespace
    : CREATE
        ( BIGFILE | SMALLFILE )?
        ( permanentTablespaceClause
        | temporaryTablespaceClause
        | undoTablespaceClause
        )
    ;

permanentTablespaceClause
    : TABLESPACE tablespace
        ( DATAFILE fileSpecification ( ',' fileSpecification )* )?
        permanentTablespaceAttrs?
        ( IN SHARDSPACE shardspaceName )?
    ;

permanentTablespaceAttrs
    : ( MINIMUM EXTENT sizeClause
      | BLOCKSIZE integer KW_K?
      | loggingClause
      | FORCE LOGGING
      | tablespaceEncryptionClause
      | defaultTablespaceParams
      | ( ONLINE | OFFLINE )
      | extentManagementClause
      | segmentManagementClause
      | flashbackModeClause
      )+
    ;

tablespaceEncryptionClause
    : ENCRYPTION ( tablespaceEncryptionSpec? ENCRYPT | DECRYPT )?
    ;

segmentManagementClause
    : SEGMENT SPACE MANAGEMENT ( AUTO | MANUAL )
    ;

temporaryTablespaceClause
    : ( TEMPORARY TABLESPACE
      | LOCAL TEMPORARY TABLESPACE FOR ( ALL | LEAF )
      ) tablespace
      ( TEMPFILE fileSpecification ( ',' fileSpecification )* )?
      tablespaceGroupClause?
      extentManagementClause?
    ;

undoTablespaceClause
    : UNDO TABLESPACE tablespace
        ( DATAFILE fileSpecification ( ',' fileSpecification )* )?
        extentManagementClause?
        tablespaceRetentionClause?
        tablespaceEncryptionClause?
    ;

createTablespaceSet
    : CREATE TABLESPACE SET tablespaceSet
        ( IN SHARDSPACE shardspaceName )?
        ( USING TEMPLATE 
          '(' ( DATAFILE fileSpecification ( ',' fileSpecification )* )? permanentTablespaceAttrs ')'
        )
    ;

createTrigger
    : CREATE ( OR REPLACE )?
        ( EDITIONABLE | NONEDITIONABLE )?
        TRIGGER plsqlTriggerSource
    ;

plsqlTriggerSource
    : ( schema '.' )? triggerName
         sharingClause? defaultCollationClause ?
         ( simpleDmlTrigger
         | insteadOfDmlTrigger
         | compoundDmlTrigger
         | systemTrigger
         )
    ;

systemTrigger
    : ( BEFORE | AFTER | INSTEAD OF )
         ( ddlEvent ( OR ddlEvent )* | databaseEvent ( OR databaseEvent )* )
         ON ( ( schema '.' )? SCHEMA | PLUGGABLE? DATABASE )
         triggerOrderingClause? 
         ( ENABLE | DISABLE )? triggerBody
    ;

compoundDmlTrigger
    : FOR dmlEventClause referencingClause?
        triggerEditionClause?
        triggerOrderingClause?
        ( ENABLE | DISABLE )?
        ( WHEN '(' condition ')' )?
        compoundTriggerBlock
    ;

compoundTriggerBlock
    : COMPOUND TRIGGER declareSection? timingPointSection+ END trigger?
    ;

timingPointSection
    : timingPoint IS BEGIN tpsBody END timingPoint
    ;

timingPoint
    : BEFORE STATEMENT
    | BEFORE EACH ROW
    | AFTER STATEMENT
    | AFTER EACH ROW
    | INSTEAD OF EACH ROW
    ;

tpsBody
    : statement+ ( EXCEPTION exceptionHandler+ )?
    ;

insteadOfDmlTrigger
    : INSTEAD OF ( DELETE | INSERT | UPDATE ) ( OR ( DELETE | INSERT | UPDATE ) )*
        ON ( NESTED TABLE nestedTableColumn OF )? ( schema '.' )? noneditioningView
        referencingClause?
        ( FOR EACH ROW )?
        triggerEditionClause?
        triggerOrderingClause?
        ( ENABLE | DISABLE )? triggerBody
    ;

simpleDmlTrigger
    : ( BEFORE | AFTER ) dmlEventClause referencingClause? ( FOR EACH ROW )?
        triggerEditionClause? triggerOrderingClause?
        ( ENABLE | DISABLE )? ( WHEN '(' condition ')' )? triggerBody
    ;

dmlEventClause
    : ( DELETE | INSERT | UPDATE ( OF column ( ',' column )* )? )
        ( OR ( DELETE | INSERT | UPDATE ( OF column ( ',' column )* )? ) )+
        ON ( schema '.' )? ( table | view )
    ;

referencingClause
    : REFERENCING
        ( OLD AS? oldName
        | NEW AS? newName
        | PARENT AS? parent
        )+
    ;

triggerEditionClause
    : ( FORWARD | REVERSE )? CROSSEDITION 
    ;

triggerOrderingClause
    : ( FOLLOWS | PRECEDES ) ( schema '.' )? trigger ( ',' ( schema '.' )? trigger )* 
    ;

triggerBody
    : plsqlBlock
    | CALL routineClause ( '.' routineClause )*
    ;

routineClause
    : ( schema '.' )? ( type '.' | packageName '.' )?
         ( functionName | procedureName | method )
         ( '@' dblinkname )?
         '(' ( argument ( ',' argument )* )? ')'
    ;

createType
    : CREATE ( OR REPLACE )?
        ( EDITIONABLE | NONEDITIONABLE )?
        TYPE plsqlTypeSource
    ;

plsqlTypeSource
    : ( schema '.' )? typeName FORCE? ( OID stringLiteral )?
          sharingClause? defaultCollationClause? ( invokerRightsClause |  accessibleByClause )*
            ( objectBaseTypeDef | objectSubtypeDef )
    ;

objectBaseTypeDef
    :  ( IS | AS ) ( objectTypeDef | varrayTypeSpec | nestedTableTypeSpec )
    ;

objectSubtypeDef
    : UNDER ( schema '.' )? supertype 
        '(' ( attribute plsqlDatatype ( ',' attribute plsqlDatatype )* ) ( ',' elementSpec )* ')'
        ( NOT? ( FINAL | INSTANTIABLE ) )*
    ;

objectTypeDef
    : OBJECT
        '(' ( attribute plsqlDatatype ( ',' attribute plsqlDatatype )* ) ( ',' elementSpec )* ')'
        ( NOT? ( FINAL | INSTANTIABLE | PERSISTABLE ) )*
    ;

varrayTypeSpec
    : ( VARRAY | VARYING ARRAY ) '(' sizeLimit ')' OF ( '('? plsqlDatatype ( NOT NULL )? ')'? | '(' plsqlDatatype ( NOT NULL )? ')' ( NOT? PERSISTABLE )? )
    ;

nestedTableTypeSpec
    : TABLE OF ( '('? plsqlDatatype ( NOT NULL )? ')'? | '(' plsqlDatatype ( NOT NULL )? ')'  ( NOT? PERSISTABLE )? )
    ;

createTypeBody
    : CREATE ( OR REPLACE )?
        ( EDITIONABLE | NONEDITIONABLE )?
        TYPE BODY plsqlTypeBodySource
    ;

plsqlTypeBodySource
    : ( schema '.' )? typeName  sharingClause?
         ( IS | AS )
            ( subprogDeclInType
            | mapOrderFuncDeclaration
            )
              ( ',' ( subprogDeclInType
                    | mapOrderFuncDeclaration
                    )
              )*
         END
    ;

subprogDeclInType
    : procDeclInType
    | funcDeclInType
    | constructorDeclaration
    ;

procDeclInType
    : STATIC? PROCEDURE name ( '(' parameterDeclaration ( ',' parameterDeclaration )* ')' )?
        ( IS | AS ) ( declareSection? body | callSpec )
    ;

funcDeclInType
    : STATIC? FUNCTION name ( '(' parameterDeclaration ( ',' parameterDeclaration )* ')' )?
        RETURN plsqlDatatype
        ( invokerRightsClause
        | accessibleByClause
        | DETERMINISTIC
        | parallelEnableClause
        | resultCacheClause
        )* PIPELINED?
        ( IS | AS ) ( declareSection? body | callSpec )
    ;

constructorDeclaration
    : FINAL?
      INSTANTIABLE?
      CONSTRUCTOR FUNCTION plsqlDatatype
      ( ( SELF IN OUT plsqlDatatype ',' )?
        parameter plsqlDatatype ( ',' parameter plsqlDatatype )*
      )?
      RETURN SELF AS RESULT
      ( IS | AS ) ( declareSection? body | callSpec )
    ;

mapOrderFuncDeclaration
    : ( MAP | ORDER )? MEMBER funcDeclInType
    ;

createUser
    : CREATE USER user
        ( (
            IDENTIFIED
            (
               BY password ( HTTP? DIGEST ( ENABLE | DISABLE ) )?
               | EXTERNALLY ( AS stringLiteral  |  AS stringLiteral )
               | GLOBALLY ( AS stringLiteral )?
            )
          )
        | NO AUTHENTICATION
        )
        ( DEFAULT COLLATION collationName )?
        ( DEFAULT TABLESPACE tablespace
        | LOCAL? TEMPORARY TABLESPACE ( tablespace | tablespaceGroupName )
        | ( QUOTA ( sizeClause | UNLIMITED ) ON tablespace )+
        | PROFILE profile
        | PASSWORD EXPIRE
        | ACCOUNT ( LOCK | UNLOCK )
          ( DEFAULT TABLESPACE tablespace
          | TEMPORARY TABLESPACE
               ( tablespace | tablespaceGroupName )
          | ( QUOTA ( sizeClause | UNLIMITED ) ON tablespace )+
          | PROFILE profile
          | PASSWORD EXPIRE
          | ACCOUNT ( LOCK | UNLOCK )
          | ENABLE EDITIONS
          | CONTAINER '=' ( CURRENT | ALL )
          )*
        )?
    ;

insert
    : INSERT hint? ( singleTableInsert | multiTableInsert )
    ;

singleTableInsert
    : insertIntoClause ( valuesClause returningClause? | subquery ) errorLoggingClause?
    ;

insertIntoClause
    : INTO dmlTableExpressionClause tAlias? ( '('column (',' column )* ')' )?
    ;

valuesClause
    : VALUES '(' ( expr | DEFAULT ) (',' ( expr | DEFAULT ) )* ')'
    ;

multiTableInsert
    : ( ALL ( insertIntoClause valuesClause? errorLoggingClause? )* | conditionalInsertClause ) subquery
    ;

conditionalInsertClause
    : ( ALL | FIRST )?
      WHEN condition THEN ( insertIntoClause valuesClause? errorLoggingClause? )+
      ( WHEN condition THEN ( insertIntoClause valuesClause? errorLoggingClause? )+ )*
      ( ELSE ( insertIntoClause valuesClause? errorLoggingClause? )+ )?
    ;

update
    : UPDATE hint?
      ( dmlTableExpressionClause | ONLY '(' dmlTableExpressionClause ')' ) tAlias?
      updateSetClause
      whereClause?
      returningClause?
      errorLoggingClause?
    ;

updateSetClause
    : SET
      (
        updateSetSubstituteClause ( ',' updateSetSubstituteClause )*
      | VALUE '(' tAlias ')' '=' ( expr | '(' subquery ')' )
      )
    ;

updateSetSubstituteClause
    : '(' column ( ',' column )* ')' '=' '(' subquery ')'
    | column '=' ( expr | '(' subquery ')' | DEFAULT )
    ;

merge
    : MERGE hint?
      INTO ( schema '.' )? ( table | view ) tAlias?
      USING ( 
              ( schema '.' )? ( table | view )
            | subquery 
            ) tAlias?
      ON ( condition )
      mergeUpdateClause?
      mergeInsertClause?
      errorLoggingClause?
    ;

mergeUpdateClause
    : WHEN MATCHED THEN
      UPDATE SET column '=' ( expr | DEFAULT )
                 (',' column '=' ( expr | DEFAULT ) )*
      whereClause?
      ( DELETE whereClause )?
    ;

mergeInsertClause
    : WHEN NOT MATCHED THEN
      INSERT ( '(' column ( ',' column )* ')' )?
      VALUES '(' ( expr | DEFAULT ) ( ',' ( expr | DEFAULT ) )* ')'
      whereClause?
    ;

call
    : CALL ( routineClause ( '.' routineClause )* | objectAccessExpression ) ( INTO ':' hostVariableName ( INDICATOR? ':' indicatorVariable=identifier )? )?
    ;

associateStatistics
    : ASSOCIATE STATISTICS WITH ( columnAssociation | functionAssociation ) storageTableClause?
    ;

disassociateStatistics
    : DISASSOCIATE STATISTICS FROM  ( FUNCTIONS ( schema '.' )? functionName ( ',' ( schema '.' )? functionName )*
                                    | PACKAGES ( schema '.' )? packageName ( ',' ( schema '.' )? packageName )*
                                    | TYPES ( schema '.' )? typeName ( ',' ( schema '.' )? typeName )*
                                    | INDEXES ( schema '.' )? index ( ',' ( schema '.' )? index )*
                                    | INDEXTYPES ( schema '.' )? indextype ( ',' ( schema '.' )? indextype )*
                                    | COLUMNS ( schema '.' )? table '.' column ( ',' ( schema '.' )? table '.' column )* 
                                    )
                                    FORCE?
    ;

comment
    : COMMENT ON
        ( AUDIT POLICY policy
        | COLUMN ( schema '.' )?
            ( table '.' | view '.' | materializedView '.' ) column
        | EDITION editionName
        | INDEXTYPE ( schema '.' )? indextype
        | MATERIALIZED VIEW materializedView
        | MINING MODEL ( schema '.' )? model
        | OPERATOR ( schema '.' )? operatorName
        | TABLE ( schema '.' )? ( table | view )
        )
        IS stringLiteral
    ;

columnAssociation
    : COLUMNS ( schema '.' )? table '.' column ( ',' ( schema '.' )? table '.' column )* usingStatisticsType
    ;

functionAssociation
    : ( FUNCTIONS ( schema '.' )? functionName ( ',' ( schema '.' )? functionName )*
      | PACKAGES ( schema '.' )? packageName ( ',' ( schema '.' )? packageName )*
      | TYPES ( schema '.' )? typeName ( ',' ( schema '.' )? typeName )*
      | INDEXES ( schema '.' )? index ( ',' ( schema '.' )? index )*
      | INDEXTYPES ( schema '.' )? indextype ( ',' ( schema '.' )? indextype )*
      )
      ( usingStatisticsType
      | defaultCostClause ( ',' defaultSelectivityClause )?
      | defaultSelectivityClause ( ',' defaultCostClause )?
      )
    ;

usingStatisticsType
    : USING ( NULL
            | ( schema '.' )? statisticsType=identifier
            )
    ;

defaultCostClause
    : DEFAULT COST '(' cpuCost=numberLiteral ',' ioCost=numberLiteral ',' networkCost=numberLiteral ')'
    ;

defaultSelectivityClause
    : DEFAULT SELECTIVITY defaultSelectivity=numberLiteral
    ;


audit
    : traditionalAudit
    | unifiedAudit
    ;

traditionalAudit
    :  AUDIT
         ( auditOperationClause ( auditingByClause | IN SESSION CURRENT )?
         | auditSchemaObjectClause
         | NETWORK
         | DIRECT_PATH LOAD auditingByClause?
         ) ( BY ( SESSION | ACCESS ) )?
           ( WHENEVER NOT? SUCCESSFUL )?
           ( CONTAINER '=' ( CURRENT | ALL ) )?
    ;

unifiedAudit
    : AUDIT
        ( ( POLICY policy
            ( ( BY user (',' user )* )
            | ( EXCEPT user (',' user )* )
            | byUsersWithRoles )?
            ( WHENEVER NOT? SUCCESSFUL )?
          )
        | ( CONTEXT NAMESPACE namespace ATTRIBUTES attribute ( ',' attribute )*
              ( ',' CONTEXT NAMESPACE namespace ATTRIBUTES attribute ( ',' attribute )* )*
            ( BY user (',' user )* )?
          )
        )
    ;

byUsersWithRoles
    : BY USERS WITH GRANTED ROLES role ( ',' role )*
    ;

noaudit
    : unifiedNoaudit
    | traditionalNoAudit
    ;

traditionalNoAudit
    : NOAUDIT 
         ( auditOperationClause auditingByClause?
         | auditSchemaObjectClause
         | NETWORK
         | DIRECT_PATH LOAD auditingByClause?
         )
         ( WHENEVER NOT? SUCCESSFUL )?
         ( CONTAINER '=' ( CURRENT | ALL ) )?
    ;

auditOperationClause
    : ( sqlStatementShortcut | ALL STATEMENTS? )
      ( ',' ( sqlStatementShortcut | ALL STATEMENTS? ) )*
    | ( systemPrivilege | ALL PRIVILEGES ) 
      (',' ( systemPrivilege | ALL PRIVILEGES ) )*
    ;

sqlStatementShortcut
    : ALTER SYSTEM
    | CLUSTER
    | CONTEXT
    | DATABASE LINK
    | DIMENSION
    | DIRECTORY
    | INDEX
    | MATERIALIZED VIEW
    | NOT EXISTS
    | OUTLINE
    | PLUGGABLE DATABASE
    | PROCEDURE
    | PROFILE
    | ROLE
    | ROLLBACK SEGMENT
    | SEQUENCE
    | SESSION
    | SYNONYM
    | SYSTEM AUDIT
    | SYSTEM GRANT
    | TABLE
    | TABLESPACE
    | TRIGGER
    | TYPE
    | USER
    | VIEW
    | ALTER SEQUENCE
    | ALTER TABLE
    | COMMENT TABLE
    | DELETE TABLE
    | EXECUTE DIRECTORY
    | EXECUTE PROCEDURE
    | GRANT DIRECTORY
    | GRANT PROCEDURE
    | GRANT SEQUENCE
    | GRANT TABLE
    | GRANT TYPE
    | INSERT TABLE
    | LOCK TABLE
    | READ DIRECTORY
    | SELECT SEQUENCE
    | SELECT TABLE
    | UPDATE TABLE
    | WRITE DIRECTORY
    ;

auditingByClause
    : BY user ( ',' user )*
    ;

auditSchemaObjectClause
    : ( sqlOperation ( ',' sqlOperation )* | ALL ) auditingOnClause
    ;

auditingOnClause
    : ON ( ( schema '.' )? object
         | DIRECTORY directoryName
         | MINING MODEL ( schema '.' )? model
         | SQL TRANSLATION PROFILE ( schema '.' )? profile
         | DEFAULT
         )
    ;

unifiedNoaudit
    : NOAUDIT
      ( POLICY policy ( ( BY user ( ',' user )* ) | byUsersWithRoles )?
      | CONTEXT NAMESPACE namespace ATTRIBUTES attribute ( ',' attribute )*
          ( ',' CONTEXT NAMESPACE namespace ATTRIBUTES attribute ( ',' attribute )* )*
          ( BY user ( ',' user )* )? ( WHENEVER NOT? SUCCESSFUL )?
      )
    ;

purge
    : PURGE
        ( TABLE table
        | INDEX index
        | TABLESPACE tablespace ( USER username )?
        | TABLESPACE SET tablespaceSet ( USER username )?
        | RECYCLEBIN
        | DBA_RECYCLEBIN
        )
    ;

rename
    : RENAME oldName TO containerName
    ;

revoke
    : REVOKE( ( revokeSystemPrivileges | revokeObjectPrivileges )
          ( CONTAINER '=' ( CURRENT | ALL ) )? )
    | revokeRolesFromPrograms
    ;

set
    : setConstraints
    | setRole
    | setTransaction
    // WARN: not definition in documentation
    | setVariable
    ;

setConstraints
    : SET 
      ( CONSTRAINT | CONSTRAINTS )
      ( constraint ( ',' constraint )* | ALL )
      ( IMMEDIATE | DEFERRED )
    ;

setRole
    : SET ROLE
      ( role ( IDENTIFIED BY password )?
        ( ',' role ( IDENTIFIED BY password )? )*
      | ALL ( EXCEPT role ( ',' role )* )?
      | NONE
      )
    ;

setTransaction
    : SET TRANSACTION
         ( ( READ ( ONLY | WRITE )
           | ISOLATION LEVEL
             ( SERIALIZABLE | READ COMMITTED )
           | USE ROLLBACK SEGMENT rollbackSegment
           ) ( NAME stringLiteral )?
         | NAME stringLiteral
         )
    ;

setVariable
    : SET identifier '=' expr
    ;

revokeSystemPrivileges
    : ( systemPrivilege | role | ALL PRIVILEGES )
        ( ',' ( systemPrivilege | role | ALL PRIVILEGES ) )*
      FROM revokeeClause
    ;

revokeObjectPrivileges
    : ( objectPrivilege | ALL PRIVILEGES? )
        ( ',' ( objectPrivilege | ALL PRIVILEGES? ) )*
      onObjectClause
      FROM revokeeClause
      ( CASCADE CONSTRAINTS | FORCE )?
    ;

revokeeClause
    : ( user | role | PUBLIC ) ( ',' ( user | role | PUBLIC ) )*
    ;

revokeRolesFromPrograms
    : ( role ( ',' role )* | ALL ) FROM programUnit ( ',' programUnit )*
    ;

rollback
    : ROLLBACK WORK? ( TO SAVEPOINT? savepointName | FORCE stringLiteral )?
    ;

savepoint
    : SAVEPOINT savepointName
    ;

truncate
    : truncateCluster
    | truncateTable
    ;

truncateCluster
    : TRUNCATE CLUSTER ( schema '.' )? cluster
        ( ( DROP | REUSE ) STORAGE )?
    ;

truncateTable
    : TRUNCATE TABLE ( schema '.' )? table
        ( ( PRESERVE | PURGE ) MATERIALIZED VIEW LOG )?
        ( ( DROP ALL? | REUSE ) STORAGE )? CASCADE?
    ;

lock
    : lockTable
    ;

lockTable
    : LOCK TABLE ( schema '.' )? ( table | view )
         ( partitionExtensionClause | '@' dblink )? 
         ( ',' ( schema '.' )? ( table | view ) ( partitionExtensionClause | '@' dblink )? )*
         IN lockmode MODE
         ( NOWAIT | WAIT integer )?
    ;

flashback
    : flashbackTable
    | flashbackDatabase
    ;

flashbackTable
    : FLASHBACK TABLE
         ( schema '.' )? table
         ( ',' ( schema '.' )? table )*
         TO 
         (
           ( 
             ( SCN | TIMESTAMP ) expr 
           | RESTORE POINT restorePointName 
           ) 
           ( ( ENABLE | DISABLE ) TRIGGERS )?
         | BEFORE DROP ( RENAME TO table )?
         )
    ;

flashbackDatabase
    : FLASHBACK STANDBY? PLUGGABLE? DATABASE database? TO
        (
          ( SCN | TIMESTAMP ) expr
        | RESTORE POINT restorePointName
        | BEFORE ( ( SCN | TIMESTAMP ) expr | RESETLOGS )
        )
    ;

explain
    : EXPLAIN PLAN
         ( SET STATEMENT_ID '=' stringLiteral )?
         ( INTO ( schema '.' )? table ( '@' dblink )? )?
      FOR oracleStatement
    ;

administerKeyManagement
    : ADMINISTER KEY MANAGEMENT ( keystoreManagementClauses
                                | keyManagementClauses
                                | secretManagementClauses
                                | zeroDowntimeSoftwarePatchingClauses
                                )
    ;

analyze
    : ANALYZE
        ( ( TABLE ( schema '.' )? table
          | INDEX ( schema '.' )? index
          ) partitionExtensionClause?
        | CLUSTER ( schema'.' )? cluster
        )
        ( validationClauses
        | LIST CHAINED ROWS intoClause?
        | DELETE SYSTEM? STATISTICS
        )
    ;

validationClauses
    : VALIDATE REF UPDATE ( SET DANGLING TO NULL )?
    | VALIDATE STRUCTURE CASCADE? ( FAST 
                                  | COMPLETE? ( ONLINE | OFFLINE ) intoClause? 
                                  )?
    ;

intoClause
    : INTO ( schema '.' )? table
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

alterAnalyticView
    : ALTER ANALYTIC VIEW (schema '.')? analyticViewName=identifier ( RENAME TO newAvName=identifier
                                                                    | COMPILE
                                                                    | alterAddCacheClause
                                                                    | alterDropCacheClause
                                                                    )
    ;

alterFlashbackArchive
    : ALTER FLASHBACK ARCHIVE flashbackArchiveName
        ( SET DEFAULT
        | ( ADD | MODIFY ) TABLESPACE tablespace flashbackArchiveQuota?
        | REMOVE TABLESPACE tablespace
        | MODIFY flashbackArchiveRetention
        | PURGE ( ALL | BEFORE ( SCN | TIMESTAMP ) expr )
        | NO? OPTIMIZE DATA
        )
    ;

flashbackArchiveQuota
    : QUOTA sizeClause
    ;

flashbackArchiveRetention
    : RETENTION integer ( YEAR | MONTH | DAY )
    ;

alterFunction
    : ALTER FUNCTION ( schema '.' )? functionName
      ( functionCompileClause | ( EDITIONABLE | NONEDITIONABLE ) )
    ;

functionCompileClause
    : COMPILE DEBUG? compilerParametersClause* ( REUSE SETTINGS )?
    ;

compilerParametersClause
    : parameterName '=' parameterValue
    ;

alterHierarchy
    : ALTER HIERARCHY ( schema '.' )? hierarchyName 
        ( RENAME TO newHierName | COMPILE )
    ;

alterIndextype
    : ALTER INDEXTYPE ( schema '.' )? indextype
        ( ( ADD | DROP ) ( schema '.' )? operator=operator1 '(' parameterTypes ')' 
            ( ',' ( ADD | DROP ) ( schema '.' )? operator=operator1 '(' parameterTypes ')' )* usingTypeClause?
        | COMPILE
        )
        ( WITH LOCAL RANGE? PARTITION )? storageTableClause?
    ;

usingTypeClause
    : USING ( schema '.' )? implementationType arrayDMLClause?
    ;

arrayDMLClause
    : ( WITH | WITHOUT )?
      ARRAY DML
      ( 
        '(' ( schema '.' )? type
        ( ',' ( schema '.' )? varrayType )? ')'
        ( ',' '(' ( schema '.' )? type
          ( ',' ( schema '.' )? varrayType )? ')'
        )
      )*
    ;

storageTableClause
    : WITH ( SYSTEM | USER ) MANAGED STORAGE TABLES
    ;

alterInmemoryJoinGroup
    : ALTER INMEMORY JOIN GROUP ( schema '.' )? joinGroup
      ( ADD | REMOVE ) '(' ( schema '.' )? table '(' column ')' ')'
    ;

alterJava
    : ALTER JAVA
      ( SOURCE | CLASS ) ( schema '.' )? objectName 
      ( RESOLVER
      '(' 
        ( 
          '(' matchString ','? ( schema | '-' ) ')' 
        )*
      ')' )?
      ( COMPILE | RESOLVE | invokerRightsClause )
    ;

invokerRightsClause
    : AUTHID ( CURRENT_USER | DEFINER )
    ;

alterLibrary
    : ALTER LIBRARY ( schema '.' )? libraryName
      ( libraryCompileClause | EDITIONABLE | NONEDITIONABLE )
    ;

libraryCompileClause
    : COMPILE DEBUG? compilerParametersClause* ( REUSE SETTINGS )?
    ;

alterLockdownProfile
    : ALTER LOCKDOWN PROFILE profileName
      ( lockdownFeatures
      | lockdownOptions
      | lockdownStatements
      )
    ;

lockdownFeatures
    : ( DISABLE | ENABLE ) FEATURE
      ( '=' '(' stringLiteral ( ',' stringLiteral )* ')'
      | ALL ( EXCEPT  '=' '(' stringLiteral ( ',' stringLiteral )* ')' )? 
      )
    ;

lockdownOptions
    : ( DISABLE | ENABLE ) OPTION
      ( '=' '(' stringLiteral ( ',' stringLiteral )* ')'
      | ALL ( EXCEPT  '=' '(' stringLiteral ( ',' stringLiteral )* ')' )? 
      )
    ;

lockdownStatements
    : ( DISABLE | ENABLE ) STATEMENT
      ( '=' '(' stringLiteral ( ',' stringLiteral )* ')'
      | ALL ( EXCEPT  '=' '(' stringLiteral ( ',' stringLiteral )* ')' )? 
      | '=' '(' stringLiteral statementClauses ')'
      )
    ;

statementClauses
    : CLAUSE
      ( '=' '(' stringLiteral ( ',' stringLiteral )* ')'
      | ALL ( EXCEPT  '=' '(' stringLiteral ( ',' stringLiteral )* ')' )? 
      | '=' '(' stringLiteral clauseOptions ')'
      )
    ;

clauseOptions
    : OPTION
      ( '=' '(' stringLiteral ( ',' stringLiteral )* ')'
      | ALL ( EXCEPT  '=' '(' stringLiteral ( ',' stringLiteral )* ')' )? 
      | '=' '(' stringLiteral optionValues ')'
      )
    ;

optionValues
    : ( 
        VALUE '=' '(' stringLiteral ( ',' stringLiteral )* ')'
      | MINVALUE '=' stringLiteral
      | MAXVALUE '=' stringLiteral
      )+
    ;

alterMaterializedView
    : ALTER MATERIALIZED VIEW
      ( schema'.' )? materializedView
      (
        ( physicalAttributesClause
          | modifyMvColumnClause
          | tableCompression
          | lobStorageClause ( ',' lobStorageClause )*
          | modifyLOBStorageClause ( ',' modifyLOBStorageClause )*
          | alterTablePartitioning
          | parallelClause
          | loggingClause
          | allocateExtentClause
          | deallocateUnused
          | shrinkClause
          | ( CACHE | NOCACHE )
          )?
        | inmemoryTableClause
      )
      alterIotClauses?
      ( USING INDEX physicalAttributesClause )?
      ( MODIFY scopedTableRefConstraint | alterMvRefresh )?
      evaluationEditionClause?
      ( ( ENABLE | DISABLE ) ON QUERY COMPUTATION )?
      ( alterQueryRewriteClause
      | COMPILE
      | CONSIDER FRESH
      )?
    ;

modifyMvColumnClause
    : MODIFY '(' column ( ENCRYPT encryptionSpec | DECRYPT )? ')'
    ;

modifyLOBStorageClause
    : MODIFY LOB '(' lobItem ')' '(' modifyLOBParameters ')'
    ;

modifyLOBParameters
    : ( storageClause
      | PCTVERSION integer
      | FREEPOOLS integer
      | REBUILD FREEPOOLS
      | lobRetentionClause
      | lobDeduplicateClause
      | lobCompressionClause
      | ( ENCRYPT encryptionSpec | DECRYPT )
      | ( CACHE | ( NOCACHE | CACHE READS ) loggingClause? )
      | allocateExtentClause
      | shrinkClause
      | deallocateUnused
      )+
    ;

scopedTableRefConstraint
    : SCOPE FOR '(' ( refColumn | refAttribute ) ')' IS ( schema '.' )? ( scopeTableName | cAlias)
    ;

alterMvRefresh
    : REFRESH
      ( ( FAST | COMPLETE | FORCE )
      | ON ( DEMAND | COMMIT )
      | ( START WITH | NEXT ) date=expr
      | WITH PRIMARY KEY
      | USING
           ( DEFAULT MASTER ROLLBACK SEGMENT
           | MASTER ROLLBACK SEGMENT rollbackSegment
           )
      | USING ( ENFORCED | TRUSTED ) CONSTRAINTS
      )+
    ;

alterIotClauses
    : indexOrgTableClause
    | alterOverflowClause
    | alterMappingTableClauses
    | COALESCE
    ;

alterOverflowClause
    : addOverflowClause
    | OVERFLOW
      ( segmentAttributesClause
      | allocateExtentClause
      | shrinkClause
      | deallocateUnused
      )+
    ;

alterMappingTableClauses
    : MAPPING TABLE ( allocateExtentClause | deallocateUnused )
    ;

addOverflowClause
    : ADD OVERFLOW 
      segmentAttributesClause?
      (
          '('
            PARTITION segmentAttributesClause?
            ( ',' PARTITION segmentAttributesClause? )*
          ')'
      )?
    ;

alterQueryRewriteClause
    : ( ENABLE | DISABLE )? QUERY REWRITE
        unusableEditionsClause?
    ;

alterMaterializedViewLog
    : ALTER MATERIALIZED VIEW LOG FORCE?
      ON ( schema '.' )? table
      ( physicalAttributesClause
      | addMvLogColumnClause
      | alterTablePartitioning
      | parallelClause
      | loggingClause
      | allocateExtentClause
      | shrinkClause
      | moveMvLogClause
      | CACHE
      | NOCACHE
      )? 
      mvLogAugmentation?
      mvLogPurgeClause?
      forRefreshClause?
    ;

addMvLogColumnClause
    : ADD '(' column ')'
    ;

moveMvLogClause
    : MOVE segmentAttributesClause parallelClause?
    ;

mvLogAugmentation
    : ADD 
      ( 
        ( OBJECT ID
        | PRIMARY KEY
        | ROWID
        | SEQUENCE
        ) ( '(' column ( ',' column )* ')' )?
        | '(' column ( ',' column )* ')'
      ) 
      (','
        ( 
          ( OBJECT ID
          | PRIMARY KEY
          | ROWID
          | SEQUENCE
          ) ( '(' column ( ',' column )* ')' )?
        | '(' column ( ',' column )* ')'
        )
      )*
      newValuesClause?
    ;

newValuesClause
    : ( INCLUDING | EXCLUDING ) NEW VALUES
    ;

mvLogPurgeClause
    : PURGE 
      ( IMMEDIATE ( SYNCHRONOUS | ASYNCHRONOUS )?
      | START WITH datetimeExpression 
        ( NEXT datetimeExpression 
        | REPEAT ( intervalLiteral | INTERVAL intervalExpression ) 
        )?
      | ( START WITH datetimeExpression )?
        ( NEXT datetimeExpression 
        | REPEAT ( intervalLiteral | INTERVAL intervalExpression ) 
        )
      )
    ;

forRefreshClause
    : FOR 
      (
        SYNCHRONOUS REFRESH USING stagingLogName
      | FAST REFRESH
      )
    ;

alterMaterializedZonemap
    : ALTER MATERIALIZED ZONEMAP ( schema '.' )? zonemapName
      ( alterZonemapAttributes
      | zonemapRefreshClause
      | ( ENABLE | DISABLE ) PRUNING
      | COMPILE
      | REBUILD
      | UNUSABLE
      )
    ;

alterZonemapAttributes
    : ( PCTFREE integer
      | PCTUSED integer
      | CACHE
      | NOCACHE
      )+
    ;

zonemapRefreshClause
    : REFRESH
      ( FAST | COMPLETE | FORCE )?
      ( ON ( DEMAND | COMMIT | LOAD | DATA MOVEMENT | LOAD DATA MOVEMENT ) )?
    ;

alterOperator
    : ALTER OPERATOR ( schema '.' )? operatorName
      ( addBindingClause
      | dropBindingClause
      | COMPILE
      )
    ;

addBindingClause
    : ADD BINDING
      '(' parameterTypes ')'
      RETURN '(' returnType ')'
      implementationClause?
      usingFunctionClause
    ;

implementationClause
    : ANCILLARY TO primaryOperator
        '(' parameterTypes ')'
          (',' primaryOperator
             '(' parameterTypes ')'
          )*
    | contextClause
    ;

contextClause
    : ( WITH COLUMN CONTEXT )?
        WITH INDEX CONTEXT ',' SCAN CONTEXT implementationType ( COMPUTE ANCILLARY DATA )?
    | WITH COLUMN CONTEXT
    | WITH INDEX CONTEXT ',' SCAN CONTEXT implementationType ( COMPUTE ANCILLARY DATA )? ( WITH COLUMN CONTEXT )? 
    ;

usingFunctionClause
    : USING ( schema '.' )? ( ( type | packageName ) '.' )? functionName
    ;

dropBindingClause
    : DROP BINDING '(' parameterTypes ')' FORCE?
    ;

alterOutline
    : ALTER OUTLINE ( PUBLIC | PRIVATE )? outline
      ( REBUILD
      | RENAME TO newOutlineName
      | CHANGE CATEGORY TO newCategoryName
      | ENABLE
      | DISABLE
      )+
    ;

alterPackage
    : ALTER PACKAGE ( schema '.' )? packageName 
      ( packageCompileClause |  EDITIONABLE | NONEDITIONABLE )
    ;

packageCompileClause
    : COMPILE DEBUG? compilerParametersClause* ( REUSE SETTINGS )?
    ;

alterPluggableDatabase
    : ALTER databaseClause
      ( pdbUnplugClause
      | pdbSettingsClauses
      | pdbDatafileClause
      | pdbRecoveryClauses
      | pdbChangeState
      | pdbChangeStateFromRoot
      | applicationClauses
      | snapshotClauses
      | prepareClause
      | dropMirrorCopy
      | lostWriteProtection
      | pdbManagedRecovery
      )
    ;

pdbUnplugClause
    : pdbName UNPLUG INTO stringLiteral pdbUnplugEncrypt?
    ;

pdbUnplugEncrypt
    : ENCRYPT USING transportSecret
    ;

pdbSettingsClauses
    : pdbName?
      ( DEFAULT EDITION '=' editionName
      | SET DEFAULT '(' BIGFILE | SMALLFILE ')' TABLESPACE
      | DEFAULT TABLESPACE tablespace
      | DEFAULT TEMPORARY TABLESPACE ( tablespace | tablespaceGroupName )
      | RENAME GLOBAL_NAME TO database '.' domain ( '.' domain )*
      | setTimeZoneClause
      | databaseFileClauses
      | supplementalDbLogging
      | pdbStorageClause
      | pdbLoggingClauses
      | pdbRefreshModeClause
      | REFRESH pdbRefreshSwitchoverClause?
      | SET CONTAINER_MAP '=' stringLiteral
      )
    | CONTAINERS ( DEFAULT TARGET '=' ( '(' containerName ')' | NONE )
                 | HOST '=' stringLiteral
                 | PORT '=' integer
                 )
    ;

pdbStorageClause
    : STORAGE ( '(' ( ( MAXSIZE | MAX_AUDIT_SIZE | MAX_DIAG_SIZE ) ( UNLIMITED | sizeClause ) )+ ')' 
              | UNLIMITED
              )
    ;

pdbLoggingClauses
    : loggingClause
    | pdbForceLoggingClause
    ;

pdbForceLoggingClause
    : ( ENABLE | DISABLE ) FORCE ( LOGGING | NOLOGGING )
    | SET STANDBY NOLOGGING FOR ( DATA AVAILABILITY | LOAD PERFORMANCE )
    ;

pdbRefreshModeClause
    : REFRESH MODE ( MANUAL
                   | EVERY numberLiteral ( HOURS | MINUTES )
                   | NONE
                   )
    ;

pdbRefreshSwitchoverClause
    : FROM pdbName '@' dblink SWITCHOVER
    ;

pdbDatafileClause
    : pdbName? DATAFILE 
      (
        ( integer | stringLiteral ) ( ',' ( integer | stringLiteral ) )*
      | ALL
      )
      ( ONLINE | OFFLINE )
    ;

pdbRecoveryClauses
    : pdbName? ( pdbGeneralRecovery
               | ( BEGIN | END ) BACKUP
               | ( ENABLE | DISABLE) RECOVERY
               )
    ;

pdbGeneralRecovery
    : RECOVER AUTOMATIC? ( FROM stringLiteral )
      ( DATABASE
      | TABLESPACE tablespace ( ',' tablespace )*
      | DATAFILE ( integer | stringLiteral ) ( ',' ( integer | stringLiteral ) )*
      | LOGFILE stringLiteral
      | CONTINUE DEFAULT?
      )
    ;

pdbChangeState
    : pdbName? ( pdbOpen | pdbClose | pdbSaveOrDiscardState )
    ;

pdbOpen
    : OPEN
      ( ( READ WRITE | READ ONLY )? RESTRICTED? FORCE?
      | ( READ WRITE ) UPGRADE RESTRICTED?
      | RESETLOGS
      )
      instancesClause?
    ;

instancesClause
    : INSTANCES
      ( '=' '(' stringLiteral ( ',' stringLiteral )* ')'
      | ALL ( EXCEPT  '=' '(' stringLiteral ( ',' stringLiteral )* ')' )? 
      )
    ;

pdbClose
    : CLOSE IMMEDIATE? ( instancesClause | relocateClause )?
    | CLOSE ABORT? instancesClause?
    ;

relocateClause
    : RELOCATE ( TO stringLiteral )?
    | NORELOCATE
    ;

pdbSaveOrDiscardState
    : ( SAVE | DISCARD ) STATE instancesClause
    ;

pdbChangeStateFromRoot
    : ( pdbName ( ',' pdbName )* | ALL ( EXCEPT pdbName ( ',' pdbName )* )? )
      ( pdbOpen | pdbClose | pdbSaveOrDiscardState )
    ;

applicationClauses
    : APPLICATION
      (
        ( appName
          ( BEGIN INSTALL stringLiteral ( COMMENT stringLiteral )
          | END INSTALL stringLiteral?
          | BEGIN PATCH numberLiteral ( MINIMUM VERSION stringLiteral )? ( COMMENT stringLiteral )?
          | END PATCH numberLiteral?
          | BEGIN UPGRADE stringLiteral? TO stringLiteral ( COMMENT stringLiteral )?
          | END UPGRADE ( TO stringLiteral )?
          | BEGIN UNINSTALL
          | END UNINSTALL
          | SET PATCH numberLiteral
          | SET VERSION stringLiteral
          | SET COMPATIBILITY VERSION ( stringLiteral | CURRENT )
          | SYNC TO  ( stringLiteral | PATCH numberLiteral )
          | ( appName ( ',' appName )* )? SYNC
          )
        )
        | ALL ( EXCEPT appName ( ',' appName )* )? SYNC
      )
    ;

snapshotClauses
    : pdbSnapshotClause
    | materializeClause
    | createSnapshotClause
    | dropSnapshotClause
    | setMaxPdbSnapshotsClause
    ;

pdbSnapshotClause
    : SNAPSHOT
      ( MANUAL
      | EVERY numberLiteral ( HOURS | MINUTES )
      | NONE
      )
    ;

materializeClause
    : MATERIALIZE
    ;

createSnapshotClause
    : SNAPSHOT snapshotName
    ;

dropSnapshotClause
    : DROP SNAPSHOT snapshotName
    ;

setMaxPdbSnapshotsClause
    : SET maxPdbSnapshots '=' numberLiteral
    ;

pdbManagedRecovery
    : RECOVER MANAGED STANDBY DATABASE CANCEL?
    ;

alterTablePartitioning
    : modifyTableDefaultAttrs
    | alterAutomaticPartitioning
    | alterIntervalPartitioning
    | setSubpartitionTemplate
    | modifyTablePartition
    | modifyTableSubpartition
    | moveTablePartition
    | moveTableSubpartition
    | addExternalPartitionAttrs
    | addTablePartition
    | coalesceTablePartition
    | dropExternalPartitionAttrs
    | dropTablePartition
    | dropTableSubpartition
    | renamePartitionSubpart
    | truncatePartitionSubpart
    | splitTablePartition
    | splitTableSubpartition
    | mergeTablePartitions
    | mergeTableSubpartitions
    | exchangePartitionSubpart
    ;


modifyTableDefaultAttrs
    : MODIFY DEFAULT ATTRIBUTES
      ( FOR partitionExtendedName )?
      ( DEFAULT DIRECTORY directoryName )?
      deferredSegmentCreation?
      readOnlyClause?
      indexingClause?
      segmentAttributesClause?
      tableCompression?
      inmemoryClause?
      ( PCTTHRESHOLD integer )?
      prefixCompression?
      alterOverflowClause?
      ( ( LOB '(' lobItem ')' | VARRAY varrayItem ) '(' lobParameters ')' )*
    ;

alterAutomaticPartitioning
    : SET PARTITIONING ( AUTOMATIC | MANUAL )
    | SET STORE IN ( tablespace ( ',' tablespace )* )
    ;

alterIntervalPartitioning
    : SET INTERVAL '(' expr? ')'
    | SET STORE IN '(' tablespace ( ',' tablespace )* ')'
    ;

setSubpartitionTemplate
    : SET SUBPARTITION TEMPLATE
        ( '(' rangeSubpartitionDesc ( ',' rangeSubpartitionDesc )* ')'
        | '(' listSubpartitionDesc ( ',' listSubpartitionDesc )* ')'
        | '(' individualHashSubparts ( ',' individualHashSubparts )* ')'
        | '(' ')'
        | numberLiteral
        )
    ;

modifyTablePartition
    : modifyRangePartition
    | modifyHashPartition
    | modifyListPartition
    ;

modifyRangePartition
    : MODIFY partitionExtendedName
       ( partitionAttributes
       | addRangeSubpartition
       | addHashSubpartition
       | addListSubpartition
       | coalesceTableSubpartition
       | alterMappingTableClause
       | REBUILD? UNUSABLE LOCAL INDEXES
       | readOnlyClause
       | indexingClause
       )
    ;

partitionAttributes
    : ( ( physicalAttributesClause
        | loggingClause
        | allocateExtentClause
        | deallocateUnused
        | shrinkClause
        )+
      )?
      ( OVERFLOW
        ( physicalAttributesClause
        | loggingClause
        | allocateExtentClause
        | deallocateUnused
        )+
      )?
      tableCompression?
      inmemoryClause?
      ( ( ( LOB lobItem | VARRAY varrayItem ) '(' modifyLOBParameters ')' )+ )?
    ;

addRangeSubpartition
    : ADD rangeSubpartitionDesc 
      ( ',' rangeSubpartitionDesc )*
      dependentTablesClause? 
      updateIndexClauses?
    ;

dependentTablesClause
    : DEPENDENT TABLES
          '(' 
            table '(' partitionSpec ( ',' partitionSpec)* ')'
            ( ',' table '(' partitionSpec ( ',' partitionSpec)* ')' )*
          ')'
    ;

partitionSpec
    : PARTITION partition? tablePartitionDescription
    ;

updateIndexClauses
    : updateGlobalIndexClause
    | updateAllIndexesClause
    ;

updateGlobalIndexClause
    : ( UPDATE | INVALIDATE ) GLOBAL INDEXES
    ;

updateAllIndexesClause
    : UPDATE INDEXES ( '(' 
                         index '(' ( updateIndexPartition | updateIndexSubpartition ) ')' 
                         ( ',' index '(' ( updateIndexPartition | updateIndexSubpartition ) ')' )* 
                       ')' )?
    ;

updateIndexPartition
    : indexPartitionDescription indexSubpartitionClause? ( ',' indexPartitionDescription indexSubpartitionClause? )*
    ;

updateIndexSubpartition
    : SUBPARTITION subpartition? ( TABLESPACE tablespace )? ( ',' SUBPARTITION subpartition? ( TABLESPACE tablespace )? )*
    ;

addHashSubpartition
    : ADD individualHashSubparts dependentTablesClause? updateIndexClauses? parallelClause?
    ;

addListSubpartition
    : ADD listSubpartitionDesc ( ',' listSubpartitionDesc )* dependentTablesClause? updateIndexClauses?
    ;

coalesceTableSubpartition
    : COALESCE PARTITION updateIndexClauses? parallelClause? allowDisallowClustering?
    ;

allowDisallowClustering
    : ( ALLOW | DISALLOW ) CLUSTERING
    ;

alterMappingTableClause
    : MAPPING TABLE ( allocateExtentClause | deallocateUnused )
    ;

modifyHashPartition
    : MODIFY partitionExtendedName
      ( partitionAttributes
      | coalesceTableSubpartition
      | alterMappingTableClause
      | REBUILD? UNUSABLE LOCAL INDEXES
      | readOnlyClause
      | indexingClause
      )
    ;

modifyListPartition
    : MODIFY partitionExtendedName
      ( partitionAttributes
      | ( ADD | DROP ) VALUES '(' listValues ')'
      | addRangeSubpartition
      | addListSubpartition
      | addHashSubpartition   
      | coalesceTableSubpartition
      | REBUILD? UNUSABLE LOCAL INDEXES
      | readOnlyClause
      | indexingClause
      )
    ;

modifyTableSubpartition
    : MODIFY subpartitionExtendedName
      ( allocateExtentClause
      | deallocateUnused
      | shrinkClause
      | ( ( LOB lobItem | VARRAY varrayItem ) '(' modifyLOBParameters ')' )+
      | REBUILD? UNUSABLE LOCAL INDEXES
      | ( ADD | DROP ) VALUES ( listValues )
      | readOnlyClause
      | indexingClause
      )
    ;

moveTablePartition
    : MOVE partitionExtendedName
        ( MAPPING TABLE )?
        tablePartitionDescription
        filterCondition?
        updateIndexClauses?
        parallelClause?
        allowDisallowClustering?
        ONLINE?
    ;

filterCondition
    : INCLUDING ROWS whereClause
    ;

moveTableSubpartition
    : MOVE subpartitionExtendedName
        indexingClause?
        partitioningStorageClause?
        updateIndexClauses?
        filterCondition?
        parallelClause?
        allowDisallowClustering?
        ONLINE?
    ;

addExternalPartitionAttrs
    : ADD EXTERNAL PARTITION ATTRIBUTES externalTableClause ( REJECT LIMIT )?
    ;

addTablePartition
    : ADD 
        (
          PARTITION partition? addRangePartitionClause
          ( ',' PARTITION partition? addRangePartitionClause )*
        | PARTITION partition? addListPartitionClause
          ( ',' PARTITION partition? addListPartitionClause )*
        | PARTITION partition? addSystemPartitionClause
          ( ',' PARTITION partition? addSystemPartitionClause )*
          ( BEFORE ( partitionName | partitionNumber ) )?
        | PARTITION partition? addHashPartitionClause
        ) 
        dependentTablesClause?
    ;

addRangePartitionClause
    : rangeValuesClause
        tablePartitionDescription
        externalPartSubpartDataProps?
        ( '(' ( rangeSubpartitionDesc ( ',' rangeSubpartitionDesc )*
              | listSubpartitionDesc ( ',' listSubpartitionDesc )*
              | individualHashSubparts ( ',' individualHashSubparts )*
              )
          ')' | hashSubpartsByQuantity )?
        updateIndexClauses?
    ;

addListPartitionClause
    : listValuesClause
        tablePartitionDescription
        externalPartSubpartDataProps?
        ( '(' ( rangeSubpartitionDesc ( ',' rangeSubpartitionDesc )*
              | listSubpartitionDesc ( ',' listSubpartitionDesc )*
              | individualHashSubparts ( ',' individualHashSubparts )*
              )
          ')' | hashSubpartsByQuantity )?
        updateIndexClauses?
    ;

addSystemPartitionClause
    : 
    tablePartitionDescription 
    updateIndexClauses?
    ;

addHashPartitionClause
    : partitioningStorageClause
        updateIndexClauses?
        parallelClause?
        readOnlyClause?
        indexingClause?
    ;

coalesceTablePartition
    : COALESCE PARTITION updateIndexClauses? parallelClause? allowDisallowClustering?
    ;

dropExternalPartitionAttrs
    : DROP EXTERNAL PARTITION ATTRIBUTES
    ;

dropTablePartition
    : DROP partitionExtendedName ( updateIndexClauses parallelClause? )?
    ;

dropTableSubpartition
    : DROP subpartitionExtendedName ( updateIndexClauses parallelClause? )?
    ;

renamePartitionSubpart
    : RENAME ( partitionExtendedName | subpartitionExtendedName ) TO newName
    ;

truncatePartitionSubpart
    : TRUNCATE ( partitionExtendedNames | subpartitionExtendedNames )
        ( ( DROP ALL? | REUSE ) STORAGE )?
        ( updateIndexClauses parallelClause? )? CASCADE?
    ;

splitTablePartition
    : SPLIT partitionExtendedName
        ( AT '(' expr ( ',' expr )* ')'
          ( INTO '(' rangePartitionDesc ',' rangePartitionDesc ')' )?
        | VALUES '(' listValues ')'
          ( INTO '(' listPartitionDesc ',' listPartitionDesc ')' )?
        | INTO '(' ( rangePartitionDesc ( ',' rangePartitionDesc )*
                   | listPartitionDesc ( ',' listPartitionDesc )* 
                   )
               ',' partitionSpec ')'
        ) splitNestedTablePart?
          filterCondition?
          dependentTablesClause?
          updateIndexClauses?
          parallelClause?
          allowDisallowClustering?
          ONLINE?
    ;

splitNestedTablePart
    : NESTED TABLE column INTO
       '(' nestedTablePartitionSpec ',' nestedTablePartitionSpec
         splitNestedTablePart?
       ')' splitNestedTablePart?
    ;

nestedTablePartitionSpec
    : PARTITION partition segmentAttributesClause?
    ;

splitTableSubpartition
    : SPLIT subpartitionExtendedName
       ( AT '(' literal ( ',' literal )* ')'
         ( INTO '(' rangeSubpartitionDesc',' rangeSubpartitionDesc ')' )?
       | VALUES '(' listValues ')'
         ( INTO '(' listSubpartitionDesc',' listSubpartitionDesc ')' )?
       | INTO '(' ( rangeSubpartitionDesc (',' rangeSubpartitionDesc )*
                  | listSubpartitionDesc (',' listSubpartitionDesc )* 
                  )
              ',' subpartitionSpec ')'
        ) filterCondition?
         dependentTablesClause?
         updateIndexClauses?
         parallelClause?
         allowDisallowClustering?
         ONLINE?
    ;

subpartitionSpec
    : SUBPARTITION subpartition? partitioningStorageClause?
    ;

mergeTablePartitions
    : MERGE PARTITIONS partitionOrKeyValue
       ( ',' partitionOrKeyValue ( ',' partitionOrKeyValue )*
       | TO partitionOrKeyValue )
       INTO partitionSpec?
       filterCondition?
       dependentTablesClause?
       updateIndexClauses?
       parallelClause?
       allowDisallowClustering?
    ;

mergeTableSubpartitions
    : MERGE SUBPARTITIONS subpartitionOrKeyValue
        ( ',' subpartitionOrKeyValue ( ',' subpartitionOrKeyValue )*
        | TO subpartitionOrKeyValue 
        )
        ( INTO ( rangeSubpartitionDesc 
               | listSubpartitionDesc
               )
        )?
        filterCondition?
        dependentTablesClause?
        updateIndexClauses?
        parallelClause?
        allowDisallowClustering?
    ;

exchangePartitionSubpart
    : EXCHANGE ( partitionExtendedName | subpartitionExtendedName )
        WITH TABLE ( schema '.' )? table
        ( ( INCLUDING | EXCLUDING ) INDEXES )?
        ( ( WITH | WITHOUT ) VALIDATION )?
        exceptionsClause?
        ( updateIndexClauses parallelClause? )?
        CASCADE?
    ;

alterPmemFilestore
    : ALTER PMEM FILESTORE filestoreName
        (
          ( RESIZE sizeClause )?
        | autoextendClause 
        | MOUNT ( ( MOUNTPOINT filePath | BACKINGFILE fileName ) )? FORCE?
        | DISMOUNT
        ) 
    ;

alterProcedure
    : ALTER PROCEDURE ( schema '.' )? procedureName
        ( procedureCompileClause |  EDITIONABLE | NONEDITIONABLE )
    ;

procedureCompileClause
    : COMPILE DEBUG? compilerParametersClause* ( REUSE SETTINGS )?
    ;

alterProfile
    : ALTER PROFILE profile LIMIT
        ( resourceParameters | passwordParameters )+
        ( CONTAINER '=' ( CURRENT | ALL ) )?
    ;

resourceParameters
    : ( SESSIONS_PER_USER
      | CPU_PER_SESSION
      | CPU_PER_CALL
      | CONNECT_TIME
      | IDLE_TIME
      | LOGICAL_READS_PER_SESSION
      | LOGICAL_READS_PER_CALL
      | COMPOSITE_LIMIT
      )
      ( integer | UNLIMITED | DEFAULT )
    | PRIVATE_SGA ( sizeClause | UNLIMITED | DEFAULT )
    ;

passwordParameters
    : ( FAILED_LOGIN_ATTEMPTS
      | PASSWORD_LIFE_TIME
      | PASSWORD_REUSE_TIME
      | PASSWORD_REUSE_MAX
      | PASSWORD_LOCK_TIME
      | PASSWORD_GRACE_TIME
      | INACTIVE_ACCOUNT_TIME
      )
      ( expr | UNLIMITED | DEFAULT )
    | PASSWORD_VERIFY_FUNCTION
      ( functionName | NULL | DEFAULT )
    | PASSWORD_ROLLOVER_TIME ( expr | DEFAULT )
    ;

alterResourceCost
    : ALTER RESOURCE COST
        ( ( CPU_PER_SESSION
          | CONNECT_TIME
          | LOGICAL_READS_PER_SESSION
          | PRIVATE_SGA
          ) integer
        )+
    ;

alterRole
    : ALTER ROLE role
        ( NOT IDENTIFIED
        | IDENTIFIED
            ( BY password
            | USING ( schema '.' ) packageName
            | EXTERNALLY
            | GLOBALLY AS domainNameOfDirectoryGroup    
            )
        )
        ( CONTAINER '=' ( CURRENT | ALL ) )?
    ;

alterRollbackSegment
    : ALTER ROLLBACK SEGMENT rollbackSegment
        ( ONLINE
        | OFFLINE
        | storageClause
        | SHRINK ( TO sizeClause )?
        )
    ;

alterSequence
    : ALTER SEQUENCE ( schema '.' )? sequence
        ( INCREMENT BY integer
        | ( MAXVALUE integer | NOMAXVALUE )
        | ( MINVALUE integer | NOMINVALUE )
        | RESTART 
        | ( CYCLE | NOCYCLE )
        | ( CACHE integer | NOCACHE )
        | ( ORDER | NOORDER )
        | ( KEEP | NOKEEP )
        | ( SCALE ( EXTEND | NOEXTEND ) | NOSCALE )
        | ( SHARD ( EXTEND | NOEXTEND ) | NOSHARD )
        | ( SESSION | GLOBAL )
        )+
    ;

alterSession
    : ALTER SESSION
        ( ADVISE ( COMMIT | ROLLBACK | NOTHING )
        | CLOSE DATABASE LINK dblink
        | ( ENABLE | DISABLE ) COMMIT IN PROCEDURE
        | ( ENABLE | DISABLE ) GUARD
        | ( ENABLE | DISABLE | FORCE ) PARALLEL
          ( DML | DDL | QUERY ) ( PARALLEL integer )?
        | ( ENABLE RESUMABLE ( TIMEOUT integer )? ( NAME stringLiteral )?
          | DISABLE RESUMABLE
          )
        | ( ENABLE | DISABLE ) SHARD DDL
        | SYNC WITH PRIMARY  
        | alterSessionSetClause
        )
    ;

alterSessionSetClause
    : SET ( ( parameterName '=' parameterValue )+
          | EDITION '=' editionName
          | CONTAINER '=' containerName ( SERVICE '=' serviceName )?
          | ROW ARCHIVAL VISIBILITY '=' ( ACTIVE | ALL )
          | DEFAULT COLLATION '=' ( collationName | NONE )
          )
    ;

alterSystem
    : ALTER SYSTEM
        ( archiveLogClause
        | checkpointClause
        | checkDatafilesClause
        | distributedRecovClauses
        | FLUSH ( SHARED_POOL | GLOBAL CONTEXT | BUFFER_CACHE | FLASH_CACHE          
                      | REDO TO targetDbName ( NO? CONFIRM APPLY )? )
        | endSessionClauses
        | SWITCH LOGFILE
        | ( SUSPEND | RESUME )
        | quiesceClauses
        | rollingMigrationClauses
        | rollingPatchClauses
        | securityClauses
        | affinityClauses
        | shutdownDispatcherClause
        | REGISTER
        | SET alterSystemSetClause
              ( alterSystemSetClause )*
        | RESET alterSystemResetClause
                ( alterSystemResetClause )*
        | RELOCATE CLIENT clientId
        | ALTER SYSTEM CANCEL SQL stringLiteral
        | FLUSH PASSWORDFILE_METADATA_CACHE
        )
    ;

archiveLogClause
    : ARCHIVE LOG
        ( INSTANCE stringLiteral )?
        ( ( SEQUENCE integer
          | CHANGE integer
          | CURRENT NOSWITCH?
          | GROUP integer
          | LOGFILE stringLiteral
               ( USING BACKUP CONTROLFILE )?
          | NEXT
          | ALL
        )
          ( TO stringLiteral )?
        )
    ;

checkpointClause
    : CHECKPOINT ( GLOBAL | LOCAL )?
    ;

checkDatafilesClause
    : CHECK DATAFILES ( GLOBAL | LOCAL )?
    ;

distributedRecovClauses
    : ( ENABLE | DISABLE ) DISTRIBUTED RECOVERY
    ;

endSessionClauses
    : ( DISCONNECT SESSION stringLiteral
           POST_TRANSACTION?
      | KILL SESSION stringLiteral
      )
      ( IMMEDIATE | NOREPLAY )?
    ;

quiesceClauses
    : QUIESCE RESTRICTED
    | UNQUIESCE
    ;

rollingMigrationClauses
    : START ROLLING MIGRATION TO stringLiteral
    | STOP ROLLING MIGRATION
    ;

rollingPatchClauses
    : START ROLLING PATCH
    | STOP ROLLING PATCH
    ;

securityClauses
    : ( ENABLE | DISABLE ) RESTRICTED SESSION
    | SET ENCRYPTION WALLET OPEN IDENTIFIED BY password
    | SET ENCRYPTION WALLET CLOSE ( IDENTIFIED BY password )?
    | setEncryptionKey
    ;

affinityClauses
    : ENABLE AFFINITY ( schema '.' )? table ( SERVICE serviceName )?
    | DISABLE AFFINITY ( schema '.' )? table
    ;

setEncryptionKey
    : SET ENCRYPTION KEY
        (
          stringLiteral? IDENTIFIED BY password
          | IDENTIFIED BY password ( MIGRATE USING password )?
        )
    ;

shutdownDispatcherClause
    : SHUTDOWN IMMEDIATE? dispatcherName?
    ;

alterSystemSetClause
    : setParameterClause
    | USE_STORED_OUTLINES '=' (TRUE | FALSE | categoryName)
    | GLOBAL_TOPIC_ENABLED '=' (TRUE | FALSE)
    ;

setParameterClause
    : parameterName '='
        parameterValue ( ',' parameterValue )*
        ( COMMENT '=' stringLiteral )?
        DEFERRED?
        ( CONTAINER '=' ( CURRENT | ALL ) )?
        ( SCOPE '=' ( MEMORY | SPFILE | BOTH )
        | SID '=' stringLiteral
        )*
    ;

alterSystemResetClause
    : parameterName ( SCOPE '=' ( MEMORY | SPFILE | BOTH )
                    | SID '=' stringLiteral
                    )*
    ;

alterTable
    : ALTER TABLE ( schema '.' )? table
        memoptimizeReadClause?
        memoptimizeWriteClause?
        ( 
          alterTableProperties
        | columnClauses
        | constraintClauses
        | alterTablePartitioning
        | alterExternalTable
        | moveTableClause
        | modifyToPartitioned
        | modifyOpaqueType
        | blockchainTableClauses
        )?
        ( enableDisableClause
        | ( ENABLE | DISABLE )
          ( TABLE LOCK | ALL TRIGGERS | CONTAINER_MAP | CONTAINERS_DEFAULT )
        )*
    ;

constraintClauses
    :  ADD ( outOfLineConstraint+
           | outOfLineRefConstraint
           | '(' outOfLineConstraint+ ')'
           | '(' outOfLineRefConstraint ')'
           )
    | MODIFY ( CONSTRAINT constraintName
             | PRIMARY KEY
             | UNIQUE '(' column ( ',' column )* ')'
             ) constraintState CASCADE?
    | RENAME CONSTRAINT oldName TO newName
    | dropConstraintClause+
    ;

dropConstraintClause
    : DROP 
        ( 
          ( PRIMARY KEY
          | UNIQUE '(' column ( ',' column )* ')'
          | CONSTRAINT constraintName 
          ) 
		  CASCADE? ( ( KEEP | DROP ) INDEX )?
	    ) 
	    ONLINE?
    ;

alterExternalTable
    : ( 
        addColumnClause
      | modifyColumnClauses
      | dropColumnClause
      | parallelClause
      | externalTableDataProps
      | REJECT LIMIT ( integer | UNLIMITED )
      | PROJECT COLUMN ( ALL | REFERENCED )
      )+
    ;

dropColumnClause
    : SET UNUSED ( COLUMN column
                 | '('column ( ',' column )*')'
                 )
      ( CASCADE CONSTRAINTS | INVALIDATE )*
      ONLINE?
    | DROP ( COLUMN column
           | '('column ( ',' column )*')'
           )
      ( CASCADE CONSTRAINTS | INVALIDATE )*
      ( CHECKPOINT integer )?
    | DROP ( UNUSED COLUMNS
           | COLUMNS CONTINUE
           )
      ( CHECKPOINT integer )?
    ;

moveTableClause
    : MOVE
        filterCondition?
        ONLINE?
        segmentAttributesClause?
        tableCompression?
        indexOrgTableClause
        ( lobStorageClause | varrayColProperties )*
        parallelClause?
        allowDisallowClustering?
        ( UPDATE INDEXES
          ( '(' index ( segmentAttributesClause
                      | updateIndexPartition
                      | GLOBAL 
                      )
              ( ',' index ( segmentAttributesClause
                          | updateIndexPartition
                          | GLOBAL  
                          ) 
              )*
            ')'
          )?
        )?
    ;

modifyToPartitioned
    : MODIFY tablePartitioningClauses
        filterCondition?
        ONLINE?
        ( UPDATE INDEXES ( '(' index ( localPartitionedIndex | globalPartitionedIndex | GLOBAL )
                           ( ',' index ( localPartitionedIndex | globalPartitionedIndex | GLOBAL ) )* ')'
                         )?
        )
    ;

modifyOpaqueType
    : MODIFY OPAQUE TYPE anydataColumn STORE '(' typeName ( ',' typeName )* ')' UNPACKED
    ;

columnClauses
    : ( addColumnClause
      | modifyColumnClauses
      | dropColumnClause
      | addPeriodClause
      | dropPeriodClause
      )+
    | renameColumnClause
    | modifyCollectionRetrieval+
    | modifyLobStorageClause+
    | alterVarrayColProperties+
    ;

addPeriodClause
    : ADD '(' periodDefinition ')'
    ;

dropPeriodClause
    : DROP '(' PERIOD FOR validTimeColumn ')'
    ;

renameColumnClause
    : RENAME COLUMN oldName TO newName
    ;

modifyCollectionRetrieval
    : MODIFY NESTED TABLE collectionItem RETURN AS ( LOCATOR | VALUE )
    ;

modifyLobStorageClause
    : MODIFY LOB '(' lobItem ')' '(' modifyLOBParameters ')'
    ;

alterVarrayColProperties
    : MODIFY VARRAY varrayItem '(' modifyLOBParameters ')'
    ;

addColumnClause
    : ADD ( '(' (columnDefinition | virtualColumnDefinition) ( ',' (columnDefinition | virtualColumnDefinition))* ')'
          | columnDefinition
          | virtualColumnDefinition 
          )
        columnProperties?
        ( '(' outOfLinePartStorage ( ',' outOfLinePartStorage )* ')' )?
    ;

outOfLinePartStorage
    : PARTITION partition
        ( nestedTableColProperties | lobStorageClause | varrayColProperties )
          ( nestedTableColProperties | lobStorageClause | varrayColProperties )*
      ( ( SUBPARTITION subpartition
         ( nestedTableColProperties | lobStorageClause | varrayColProperties )
           ( nestedTableColProperties | lobStorageClause | varrayColProperties )*
          ( ',' SUBPARTITION subpartition
           ( nestedTableColProperties | lobStorageClause | varrayColProperties )
             ( nestedTableColProperties | lobStorageClause | varrayColProperties )*
          )*
        )
      )?
    ;

modifyColumnClauses
    : MODIFY
        ( '(' ( modifyColProperties | modifyVirtcolProperties )
            ( ',' ( modifyColProperties | modifyVirtcolProperties ) )* ')'
        | '(' modifyColVisibility ( ',' modifyColVisibility )* ')'
        | modifyColSubstitutable
        | modifyColProperties
        | modifyVirtcolProperties
        )
    ;

modifyColProperties
    : column datatype?
       ( COLLATE columnCollationName )?
       ( DEFAULT ( ON NULL )? expr | identityClause | DROP IDENTITY )?
       ( ENCRYPT encryptionSpec | DECRYPT )?
       inlineConstraint*
       lobStorageClause?
       alterXMLSchemaClause?
    ;

modifyVirtcolProperties
    : column datatype?
        ( COLLATE columnCollationName )?
        ( GENERATED ALWAYS )? AS '(' expr ')' VIRTUAL?
        evaluationEditionClause
        unusableEditionsClause?
    ;

modifyColVisibility
    : column ( VISIBLE | INVISIBLE )
    ;

modifyColSubstitutable
    : COLUMN column NOT? SUBSTITUTABLE AT ALL LEVELS FORCE?
    ;

alterXMLSchemaClause
    : ALLOW ANYSCHEMA
    | ALLOW NONSCHEMA
    | DISALLOW NONSCHEMA
    ;

memoptimizeReadClause
    : MEMOPTIMIZE FOR READ
    | NO MEMOPTIMIZE FOR READ
    ;

memoptimizeWriteClause
    : MEMOPTIMIZE FOR WRITE
    | NO MEMOPTIMIZE FOR WRITE
    ;

alterTableProperties
    : (
        ( 
          physicalAttributesClause
        | loggingClause
        | tableCompression
        | inmemoryTableClause
        | ilmClause
        | supplementalTableLogging
        | allocateExtentClause
        | deallocateUnused
        | ( CACHE | NOCACHE )
        | resultCacheClause
        | upgradeTableClause
        | recordsPerBlockClause
        | parallelClause
        | rowMovementClause
        | logicalReplicationClause
        | flashbackArchiveClause
        )+
      | RENAME TO newTableName
      )
      alterIotClauses?
      alterXMLSchemaClause?
    | alterIotClauses
      alterXMLSchemaClause?
    | alterXMLSchemaClause
    | shrinkClause 
    | READ ONLY
    | READ WRITE 
    | REKEY encryptionSpec 
    | DEFAULT COLLATION collationName
    | NO? ROW ARCHIVAL
    | ADD attributeClusteringClause
    | MODIFY CLUSTERING clusteringWhen zonemapClause?
    | DROP CLUSTERING
    ;

supplementalTableLogging
    : ADD SUPPLEMENTAL LOG
        ( supplementalLogGrpClause | supplementalIdKeyClause )
          (',' SUPPLEMENTAL LOG
           ( supplementalLogGrpClause | supplementalIdKeyClause )
        )*
    | DROP SUPPLEMENTAL LOG
      ( supplementalIdKeyClause | GROUP logGroup )
        ( ',' SUPPLEMENTAL LOG
           ( supplementalIdKeyClause | GROUP logGroup )
        )*
    ;

upgradeTableClause
    : UPDATE ( NOT? INCLUDING DATA ) columnProperties?
    ;

recordsPerBlockClause
    : ( MINIMIZE | NOMINIMIZE ) RECORDS_PER_BLOCK
    ;

alterTablespace
    : ALTER TABLESPACE tablespace alterTablespaceAttrs
    ;

alterTablespaceAttrs
    : defaultTablespaceParams
    | MINIMUM EXTENT sizeClause
    | RESIZE sizeClause
    | COALESCE
    | SHRINK SPACE ( KEEP sizeClause )?
    | RENAME TO newTablespaceName
    | ( BEGIN | END ) BACKUP
    | datafileTempfileClauses
    | tablespaceLoggingClauses
    | tablespaceGroupClause
    | tablespaceStateClauses
    | autoextendClause
    | flashbackModeClause
    | tablespaceRetentionClause
    | alterTablespaceEncryption
    ;

defaultTablespaceParams
    : DEFAULT defaultTableCompression? defaultIndexCompression? inmemoryClause? ilmClause? storageClause?
    ;

defaultTableCompression
    : TABLE ( COMPRESS FOR OLTP      
            | COMPRESS FOR QUERY ( LOW | HIGH )
            | COMPRESS FOR ARCHIVE ( LOW | HIGH )
            | NOCOMPRESS
            )
    ;

defaultIndexCompression
    : INDEX ( COMPRESS ADVANCED ( LOW | HIGH )
            | NOCOMPRESS
            )
    ;

datafileTempfileClauses
    : ADD ( DATAFILE | TEMPFILE )
       fileSpecification ( ',' fileSpecification )*
    | DROP ( DATAFILE | TEMPFILE ) ( stringLiteral | numberLiteral )
    | SHRINK TEMPFILE ( stringLiteral | numberLiteral ) ( KEEP sizeClause )?
    | RENAME DATAFILE stringLiteral ( ',' stringLiteral )*
        TO stringLiteral ( ',' stringLiteral )*
    | ( DATAFILE | TEMPFILE ) ( ONLINE | OFFLINE )
    ;

tablespaceLoggingClauses
    : loggingClause
    | NO? FORCE LOGGING
    ;

tablespaceGroupClause
    : TABLESPACE GROUP tablespaceGroupName
    ;

tablespaceStateClauses
    : ( ONLINE
      | OFFLINE ( NORMAL | TEMPORARY | IMMEDIATE )?
      )
    | READ ( ONLY | WRITE )
    | ( PERMANENT | TEMPORARY )
    ;

tablespaceRetentionClause
    : RETENTION ( GUARANTEE | NOGUARANTEE )
    ;

alterTablespaceEncryption
    : ENCRYPTION
        ( ( OFFLINE ( ENCRYPT | DECRYPT ) )
        | ( ONLINE ( ( tablespaceEncryptionSpec? ( ENCRYPT | REKEY ) )
                     | DECRYPT )
                   tsFileNameConvert? )
        | ( FINISH ( ENCRYPT | REKEY | DECRYPT ) tsFileNameConvert? )
        )
    ;

tablespaceEncryptionSpec
    : USING stringLiteral
    ;

tsFileNameConvert
    : FILE_NAME_CONVERT '=' '(' stringLiteral ',' stringLiteral (',' stringLiteral ',' stringLiteral )* ')' KEEP?
    ;

alterTablespaceSet
    : ALTER TABLESPACE SET tablespaceSet alterTablespaceAttrs
    ;

alterTrigger
    : ALTER TRIGGER ( schema '.' )? triggerName
        ( triggerCompileClause
        | ( ENABLE | DISABLE )
        | RENAME TO newName
        | ( EDITIONABLE | NONEDITIONABLE )
        )
    ;

triggerCompileClause
    : COMPILE DEBUG? compilerParametersClause* ( REUSE SETTINGS )?
    ;

alterType
    : ALTER TYPE ( schema '.' )? typeName
        ( alterTypeClause | ( EDITIONABLE | NONEDITIONABLE ) )
    ;

alterTypeClause
    : typeCompileClause
    | typeReplaceClause
    | RESET
    | NOT? ( INSTANTIABLE | FINAL )
    | ( alterMethodSpec
      | alterAttributeDefinition
      | alterCollectionsClauses
      ) dependentHandlingClause?
    ;

typeCompileClause
    : COMPILE DEBUG? compilerParametersClause* ( REUSE SETTINGS )?
    ;

typeReplaceClause
    : REPLACE
        ( invokerRightsClause accessibleByClause? 
        | accessibleByClause invokerRightsClause? 
        )?
        AS OBJECT  
        '(' attribute datatype ( ',' attribute datatype )*
           ( ',' elementSpec )*
        ')'
    ;

elementSpec
    : inheritanceClauses?
        ( subprogramSpec
        | constructorSpec
        | mapOrderFunctionSpec
        )+ 
        ( ',' restrictReferencesPragma )?
    ;

inheritanceClauses
    :  ( NOT? ( OVERRIDING | FINAL | INSTANTIABLE ) )+
    ;

constructorSpec
    : FINAL? INSTANTIABLE?
        CONSTRUCTOR FUNCTION datatype
        ( '('( SELF IN OUT datatype',' )?
           parameter datatype ( ',' parameter datatype )*
          ')'
        )?
        RETURN SELF AS RESULT
        ( ( IS | AS ) callSpec )?
    ;

restrictReferencesPragma
    : PRAGMA RESTRICT_REFERENCES
        '(' ( subprogram | method | DEFAULT )
          ( ',' ( RNDS | WNDS | RNPS | WNPS | TRUST ) )+ ')'
    ;

accessibleByClause
    : ACCESSIBLE BY '(' accessor ( ',' accessor )* ')'
    ;

accessor
    : unitKind? ( schema '.' )? unitName
    ;

unitKind
    : FUNCTION 
    | PROCEDURE 
    | PACKAGE 
    | TRIGGER 
    | TYPE
    ;

alterMethodSpec
    : ( ADD | DROP ) ( mapOrderFunctionSpec | subprogramSpec )
        ( ',' ( ADD | DROP ) ( mapOrderFunctionSpec | subprogramSpec ) )*
    ;

mapOrderFunctionSpec
    : ( MAP | ORDER ) MEMBER functionSpec
    ;

subprogramSpec
    : ( MEMBER | STATIC ) ( procedureSpec | functionSpec )
    ;

procedureSpec
    : PROCEDURE procedureName '(' parameter datatype ( ',' parameter datatype )* ')' ( ( IS | AS ) callSpec )?
    ;

functionSpec
    : FUNCTION functionName
       ( '(' parameter datatype ( ',' parameter datatype )* ')' )?
        returnClause
    ;

returnClause
    : RETURN datatype ( DETERMINISTIC | PIPELINED | PARALLEL_ENABLE | RESULT_CACHE resultCacheClause? )* ( ( IS | AS ) callSpec )?
    ;

callSpec
    : javaDeclaration 
    | cDeclaration
    ;

javaDeclaration
    : LANGUAGE JAVA NAME stringLiteral
    ;

cDeclaration
    : (LANGUAGE KW_C | EXTERNAL ) ( ( NAME name )? LIBRARY libName | LIBRARY libName ( NAME name )? )
        ( AGENT IN '(' argument ( ',' argument )* ')' )?
        ( WITH CONTEXT )?
        ( PARAMETERS '(' externalParameter ( ',' externalParameter )* ')' )?
    ;

alterAttributeDefinition
    : ( ADD | MODIFY ) ATTRIBUTE
         ( attribute datatype?
         | '(' attribute datatype ( ',' attribute datatype )* ')'
         )
    | DROP ATTRIBUTE
         ( attribute
         | '(' attribute ( ',' attribute )* ')'
         )
    ;

alterCollectionsClauses
    : MODIFY ( LIMIT integer
             | ELEMENT TYPE datatype
             )
    ;

dependentHandlingClause
    : INVALIDATE
    | CASCADE ( NOT? INCLUDING TABLE DATA
              | CONVERT TO SUBSTITUTABLE
              )?
         ( FORCE? exceptionsClause )?
    ;

alterUser
    : ALTER USER
        ( user
          ( IDENTIFIED
            ( BY password ( REPLACE oldPassword )?
            | EXTERNALLY ( AS stringLiteral )?
            | GLOBALLY ( AS stringLiteral )?
            )
          | NO AUTHENTICATION 
          | DEFAULT COLLATION collationName
          | DEFAULT TABLESPACE tablespace
          | LOCAL? TEMPORARY TABLESPACE ( tablespace | tablespaceGroupName )
          | ( QUOTA ( sizeClause
                    | UNLIMITED
                    ) ON tablespace
            )+
          | PROFILE profile
          | DEFAULT ROLE ( role ( ',' role )*
                         | ALL ( EXCEPT role ( ',' role )* )?
                         | NONE
                         )
          | PASSWORD EXPIRE
          | ACCOUNT ( LOCK | UNLOCK )
          | ENABLE EDITIONS ( FOR objectType ( ',' objectType )* )? FORCE?
          | HTTP? DIGEST ( ENABLE | DISABLE )
          | CONTAINER '=' ( CURRENT | ALL )
          | containerDataClause
          )+
        | user ( ',' user )* proxyClause
        )
    ;

containerDataClause
    : ( SET CONTAINER_DATA '=' ( ALL | DEFAULT | '(' containerName ( ',' containerName )* ')' )
      | ADD CONTAINER_DATA '=' '(' containerName ( ',' containerName )* ')'
      | REMOVE CONTAINER_DATA '=' '(' containerName ( ',' containerName )* ')'
      )
      ( FOR ( schema '.' )? containerDataObject )?
    ;

proxyClause
    : GRANT CONNECT THROUGH ( ENTERPRISE USERS | dbUserProxy dbUserProxyClauses )
    | REVOKE CONNECT THROUGH ( ENTERPRISE USERS | dbUserProxy )
    ;

dbUserProxyClauses
    :  ( WITH
         ( ROLE ( roleName ( ',' roleName )*
                | ALL EXCEPT roleName ( ',' roleName )*
                )
         | NO ROLES
         )
       )?
       ( AUTHENTICATION REQUIRED )?
       ( AUTHENTICATED USING ( PASSWORD | CERTIFICATE | DISTINGUISHED NAME ) )?
    ;

dropAnalyticView
    : DROP ANALYTIC VIEW (schema '.')? analyticViewName=identifier
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

alterAttributeDimension
    : ALTER ATTRIBUTE DIMENSION (schema '.')? attrDimension=identifier
      ( RENAME TO newAttrDimension=identifier
      | COMPILE
      )
    ;

dropAttributeDimension
    : DROP ATTRIBUTE DIMENSION (schema '.')? attrDimension=identifier
    ;

createAuditPolicy
    : CREATE AUDIT POLICY policy
      privilegeAuditClause?
      actionAuditClause?
      roleAuditClause?
      (WHEN stringLiteral
       EVALUATE PER (STATEMENT | SESSION | INSTANCE)
      )?
      (ONLY TOPLEVEL)?
      (CONTAINER '=' (ALL | CURRENT))?
    ;

alterAuditPolicy
    : ALTER AUDIT POLICY policy
      ADD?
      ( (privilegeAuditClause? actionAuditClause? roleAuditClause?)
      | (ONLY TOPLEVEL)?
      )
      DROP?
      ( (privilegeAuditClause? actionAuditClause? roleAuditClause?)
      | (ONLY TOPLEVEL)?
      )
      (CONDITION ( DROP
                 | stringLiteral EVALUATE PER (STATEMENT | SESSION | INSTANCE)
                 )
      )?
    ;

dropAuditPolicy
    : DROP AUDIT POLICY policy
    ;

alterCluster
    : ALTER CLUSTER (schema '.')? cluster
      ( physicalAttributesClause 
      | SIZE sizeClause 
      | (MODIFY PARTITION partition)? allocateExtentClause 
      | deallocateUnused 
      | (CACHE | NOCACHE)
      )* parallelClause?
    ;

createDatabase
    : CREATE DATABASE database?
      createDatabaseOption+
    ;

alterDatabase
    : ALTER databaseClause
      ( startupClauses
      | recoveryClauses
      | databaseFileClauses
      | logfileClauses
      | controlfileClauses
      | standbyDatabaseClauses
      | defaultSettingsClauses
      | instanceClauses
      | securityClause
      | prepareClause
      | dropMirrorCopy
      | lostWriteProtection
      | cdbFleetClauses
      | propertyClause
      | replayUpgradeClause
      )
    ;

dropDatabase
    : DROP DATABASE
    ;

alterDatabaseDictionary
    : ALTER DATABASE DICTIONARY ( ENCRYPT CREDENTIALS 
                                | REKEY CREDENTIALS 
                                | DELETE CREDENTIALS KEY
                                )
    ;

alterDatabaseLink
    : ALTER SHARED? PUBLIC? DATABASE LINK dblink
      (CONNECT TO user IDENTIFIED BY password dblinkAuthentication? | dblinkAuthentication)
    ;

alterDimension
    : ALTER DIMENSION (schema '.')? dimension=identifier
      ( ( ADD (levelClause | hierarchyClause | attributeClause | extendedAttributeClause))+
      | (DROP (LEVEL level (RESTRICT | CASCADE)? | HIERARCHY hierarchy | ATTRIBUTE attribute (LEVEL level (COLUMN column)?)*))+
      | COMPILE
      )
    ;

alterDiskgroup
    : ALTER DISKGROUP (diskgroupName ( ((addDiskClause | dropDiskClause)+ | resizeDiskClause) rebalanceDiskgroupClause?
                                     | replaceDiskClause
                                     | renameDiskClause 
                                     | diskOnlineClause 
                                     | diskOfflineClause 
                                     | rebalanceDiskgroupClause 
                                     | checkDiskgroupClause 
                                     | diskgroupTemplateClauses 
                                     | diskgroupDirectoryClauses 
                                     | diskgroupAliasClauses 
                                     | diskgroupVolumeClauses 
                                     | diskgroupAttributes 
                                     | dropDiskgroupFileClause 
                                     | convertRedundancyClause 
                                     | usergroupClauses 
                                     | userClauses 
                                     | filePermissionsClause 
                                     | fileOwnerClause 
                                     | scrubClause 
                                     | quotagroupClauses 
                                     | filegroupClauses
                                     ) 
                      | (diskgroupName (',' diskgroupName)* | ALL) (undropDiskClause | diskgroupAvailability | enableDisableVolume))
    ;

createSchema
    : CREATE SCHEMA AUTHORIZATION schema
      (createTable | createView | grant)+
    ;

createTable
    : CREATE
      hint?
      ( (GLOBAL|PRIVATE) TEMPORARY
      | SHARDED
      | DUPLICATED
      | BLOCKCHAIN
      )?
      TABLE (schema '.')? table
      (SHARING '=' (METADATA | DATA | EXTENDED DATA | NONE))?
      (relationalTable | objectTable | xmlTypeTable)
      (MEMOPTIMIZE FOR READ)?
      (MEMOPTIMIZE FOR WRITE)?
      (PARENT (schema '.')? table)?
    ;

dropTable
    : DROP TABLE (schema '.')? table (CASCADE CONSTRAINTS)? PURGE?
    ;

createIndex
    : CREATE (UNIQUE | BITMAP | MULTIVALUE)? INDEX (schema '.')? index
      indexIlmClause? ON (clusterIndexClause | tableIndexClause | bitmapJoinIndexClause)
      (USABLE | UNUSABLE)? ((DEFERRED | IMMEDIATE) INVALIDATION)?
    ;

alterIndex
    : ALTER INDEX (schema '.')? indexName=identifier indexIlmClause?
      ( ( deallocateUnused
        | allocateExtentClause
        | shrinkClause
        | parallelClause
        | physicalAttributesClause
        | loggingClause
        | partialIndexClause
        )*
     | rebuildClause
     | PARAMETERS '(' stringLiteral ')'
     | COMPILE
     | ENABLE
     | DISABLE
     | UNUSABLE ONLINE? ((DEFERRED | IMMEDIATE) INVALIDATION)?
     | VISIBLE
     | INVISIBLE
     | RENAME TO containerName
     | COALESCE CLEANUP? ONLY? parallelClause?
     | (MONITORING | NOMONITORING) USAGE
     | UPDATE BLOCK REFERENCES
     | alterIndexPartitioning) ';'
    ;

dropIndex
    : DROP INDEX (schema '.')? index ONLINE? FORCE? ((DEFERRED | IMMEDIATE) INVALIDATION)?
    ;

createView
    : CREATE (OR REPLACE)? (NO? FORCE)? (EDITIONING | EDITIONABLE EDITIONING? | NONEDITIONABLE)? VIEW
      (schema '.')? view (SHARING '=' (METADATA | DATA | EXTENDED DATA | NONE))?
      ( '(' createViewConstraintItem (',' createViewConstraintItem)* ')'
      | objectViewClause
      | xmlTypeViewClause
      )?
      (DEFAULT COLLATION collationName)?
      (BEQUEATH (CURRENT_USER | DEFINER))?
      AS subquery subqueryRestrictionClause? (CONTAINER_MAP | CONTAINERS_DEFAULT)?
    ;

alterView
    : ALTER VIEW (schema '.')? view
      ( ADD outOfLineConstraint
      | MODIFY CONSTRAINT constraint (RELY | NORELY)
      | DROP (CONSTRAINT constraint
             | PRIMARY KEY
             | UNIQUE '(' column (',' column)* ')'
             )
      | COMPILE
      | READ (ONLY | WRITE)
      | EDITIONABLE
      | NONEDITIONABLE
      )
    ;

dropCluster
    : DROP CLUSTER ( schema '.' )? cluster
        ( INCLUDING TABLES ( CASCADE CONSTRAINTS )? )?
    ;

dropContext
    : DROP CONTEXT namespace
    ;

dropDatabaseLink
    : DROP PUBLIC? DATABASE LINK dblink
    ;

dropDimension
    : DROP DIMENSION ( schema '.' )? dimensionName
    ;

dropDirectory
    : DROP DIRECTORY directoryName
    ;

dropDiskgroup
    : DROP DISKGROUP diskgroupName
         (  FORCE INCLUDING CONTENTS
         | ( INCLUDING | EXCLUDING ) CONTENTS
         )
    ;

dropEdition
    : DROP EDITION editionName CASCADE?
    ;

dropFlashbackArchive
    : DROP FLASHBACK ARCHIVE flashbackArchiveName
    ;

dropFunction
    : DROP FUNCTION ( schema '.' )? functionName
    ;

dropHierarchy
    : DROP HIERARCHY ( schema '.' )? hierarchyName
    ;

dropIndextype
    : DROP INDEXTYPE ( schema '.' )? indextype FORCE?
    ;

dropInmemoryJoinGroup
    : DROP INMEMORY JOIN GROUP ( schema '.' )? joinGroup
    ;

dropJava
    : DROP JAVA ( SOURCE | CLASS | RESOURCE )
        ( schema '.' )? objectName
    ;

dropLibrary
    : DROP LIBRARY libraryName
    ;

dropLockdownProfile
    : DROP LOCKDOWN PROFILE profileName
    ;

dropMaterializedView
    : DROP MATERIALIZED VIEW ( schema '.' )? materializedView
         ( PRESERVE TABLE )?
    ;

dropMaterializedViewLog
    : DROP MATERIALIZED VIEW LOG ON ( schema '.' )? table
    ;

dropMaterializedZonemap
    : DROP MATERIALIZED ZONEMAP ( schema '.' )? zonemapMame
    ;

dropOperator
    : DROP OPERATOR ( schema '.' )? operatorName FORCE?
    ;

dropOutline
    : DROP OUTLINE outlineName
    ;

dropPackage
    : DROP PACKAGE BODY? ( schema '.' )? packageName
    ;

dropPluggableDatabase
    : DROP PLUGGABLE DATABASE pdbName
        ( ( KEEP | INCLUDING ) DATAFILES )?
    ;

dropPmemFilestore
    : DROP PMEM FILESTORE filestoreName
        ( FORCE INCLUDING CONTENTS
        | ( INCLUDING | EXCLUDING ) CONTENTS
        )?
    ;

dropProcedure
    : DROP PROCEDURE ( schema '.' )? procedureName
    ;

dropProfile
    : DROP PROFILE profile CASCADE?
    ;

dropRestorePoint
    : DROP RESTORE POINT restorePointName ( FOR PLUGGABLE DATABASE pdbName )?
    ;

dropRole
    : DROP ROLE role
    ;

dropRollbackSegment
    : DROP ROLLBACK SEGMENT rollbackSegment
    ;

dropSequence
    : DROP SEQUENCE ( schema '.' )? sequenceName
    ;

dropTablespace
    : DROP TABLESPACE tablespace
         ( ( DROP | KEEP ) QUOTA )?
         ( INCLUDING CONTENTS ( ( AND | KEEP ) DATAFILES )? ( CASCADE CONSTRAINTS )? )?
    ;

dropTablespaceSet
    : DROP TABLESPACE SET tablespaceSet
         ( INCLUDING CONTENTS ( ( AND | KEEP ) DATAFILES )? ( CASCADE CONSTRAINTS )? )?
    ;

dropTrigger
    : DROP TRIGGER ( schema '.' )? trigger
    ;

dropType
    : DROP TYPE ( schema '.' )? typeName ( FORCE | VALIDATE )?
    ;

dropTypeBody
    : DROP TYPE BODY ( schema '.' )? typeName
    ;

dropUser
    : DROP USER user CASCADE?
    ;

dropView
    : DROP VIEW (schema '.')? view (CASCADE CONSTRAINTS)?
    ;

createSynonym
    : CREATE (OR REPLACE)? (EDITIONABLE | NONEDITIONABLE)? (PUBLIC)? SYNONYM (schema '.')? synonym=identifier
      (SHARING '=' (METADATA | NONE))? FOR (schema '.')? object ('@' dblink)?
    ;

alterSynonym
    : ALTER PUBLIC? SYNONYM (schema '.')? synonym=identifier (EDITIONABLE | NONEDITIONABLE | COMPILE)
    ;

dropSynonym
    : DROP PUBLIC? SYNONYM (schema '.')? synonym=identifier FORCE?
    ;

// clauses

levelClause
    : LEVEL level IS (levelClauseItem | '(' levelClauseItem (',' levelClauseItem)* ')') (KW_SKIP WHEN NULL)?
    ;

hierarchyClause
    : HIERARCHY hierarchy '(' childLevel=identifier (CHILD OF parentLevel=identifier)* dimensionJoinClause ')'
    ;

dimensionJoinClause
    : dimensionJoinClauseItem*
    ;

dimensionJoinClauseItem
    : JOIN KEY ( childKeyColumn=column 
               | '(' childKeyColumn=column (',' childKeyColumn=column)* ')'
               )
      REFERENCES parentLevel=identifier
    ;

attributeClause
    : ATTRIBUTE level DETERMINES ( dependentColumn=column 
                                            | '(' dependentColumn=column (',' dependentColumn=column)* ')'
                                            )
    ;

extendedAttributeClause
    : ATTRIBUTE attribute (LEVEL level DETERMINES (dependentColumn=column | '(' dependentColumn=column (',' dependentColumn=column)* ')'))*
    ;

levelClauseItem
    : levelTable=identifier '.' levelColumn=identifier
    ;

dblinkAuthentication
    : AUTHENTICATED BY user IDENTIFIED BY password
    ;

deallocateUnused
    : DEALLOCATE UNUSED (KEEP sizeClause)?
    ;

allocateExtentClause
    : ALLOCATE EXTENT ('(' (SIZE sizeClause | DATAFILE stringLiteral | INSTANCE integer)* ')')?
    ;

shrinkClause
    : SHRINK SPACE COMPACT? CASCADE?
    ;

partialIndexClause
    : INDEXING (PARTIAL | FULL)
    ;

rebuildClause
    : REBUILD ( PARTITION partition
              | SUBPARTITION subpartition
              | REVERSE
              | NOREVERSE)?
      ( parallelClause
      | TABLESPACE tablespace
      | PARAMETERS '(' stringLiteral ')'
      | xmlIndexParametersClause
      | ONLINE
      | physicalAttributesClause
      | indexCompression
      | loggingClause
      | partialIndexClause
      )*
    ;

alterIndexPartitioning
    : modifyIndexDefaultAttrs
    | addHashIndexPartition
    | modifyIndexPartition
    | renameIndexPartition
    | dropIndexPartition
    | splitIndexPartition
    | coalesceIndexPartition
    | modifyIndexSubpartition
    ;

modifyIndexDefaultAttrs
    : MODIFY DEFAULT ATTRIBUTES (FOR PARTITION partition)? (physicalAttributesClause | TABLESPACE (tablespace | DEFAULT) | loggingClause)*
    ;

addHashIndexPartition
    : ADD PARTITION partitionName? (TABLESPACE tablespaceName=identifier)? indexCompression? parallelClause?
    ;

modifyIndexPartition
    : MODIFY PARTITION partition
      ( (deallocateUnused | allocateExtentClause | physicalAttributesClause | loggingClause | indexCompression)*
      | PARAMETERS '(' stringLiteral ')'
      | COALESCE CLEANUP? parallelClause?
      | UPDATE BLOCK REFERENCES
      | UNUSABLE
      )
    ;

renameIndexPartition
    : RENAME (PARTITION partition | SUBPARTITION subpartition) TO containerName
    ;

dropIndexPartition
    : DROP PARTITION partitionName
    ;

splitIndexPartition
    : SPLIT PARTITION partitionNameOld=identifier AT '(' literal (',' literal)* ')'
      (INTO '(' indexPartitionDescription ',' indexPartitionDescription ')')?
      parallelClause?
    ;

indexPartitionDescription
    : PARTITION ( partition ( (segmentAttributesClause | indexCompression)+
                                       | PARAMETERS '(' stringLiteral ')'
                                       )?
                                       (USABLE | UNUSABLE)?
                )?
    ;

coalesceIndexPartition
    : COALESCE PARTITION parallelClause?
    ;

modifyIndexSubpartition
    : MODIFY SUBPARTITION subpartition (UNUSABLE | allocateExtentClause | deallocateUnused)
    ;

databaseClause
    : DATABASE database?
    | PLUGGABLE DATABASE pdbName?
    ;

startupClauses
    : MOUNT ((STANDBY | CLONE) DATABASE)?
    | OPEN ( (READ WRITE)? (RESETLOGS | NORESETLOGS)? (UPGRADE | DOWNGRADE)?
           | READ ONLY
           )
    ;

recoveryClauses
    : generalRecovery
    | managedStandbyRecovery
    | (BEGIN | END) BACKUP
    ;

generalRecovery
    : RECOVER AUTOMATIC? (FROM stringLiteral)?
      ( ( fullDatabaseRecovery
        | partialDatabaseRecovery
        | LOGFILE stringLiteral
        ) ( TEST | ALLOW integer CORRUPTION | parallelClause )*
      | CONTINUE DEFAULT?
      | CANCEL
      )
    ;

fullDatabaseRecovery
    : STANDBY? DATABASE?
      ( UNTIL ( CANCEL
              | TIME date=stringLiteral
              | CHANGE integer
              | CONSISTENT
              )
      | USING BACKUP CONTROLFILE
      | SNAPSHOT TIME date=stringLiteral
      )*
    ;

partialDatabaseRecovery
    : TABLESPACE tablespace (',' tablespace)*
    | DATAFILE (filename | filenumber=integer)
    ;

managedStandbyRecovery
    : RECOVER ( MANAGED STANDBY DATABASE ( managedStandbyRecoveryItem+
                                         | FINISH
                                         | CANCEL
                                         )?
              | TO LOGICAL STANDBY (dbName=identifier | KEEP IDENTITY)
              )
    ;

managedStandbyRecoveryItem
    : USING ARCHIVED LOGFILE
    | DISCONNECT (FROM SESSION)?
    | NODELAY
    | UNTIL CHANGE integer
    | UNTIL CONSISTENT
    | USING INSTANCES (ALL | integer)
    | parallelClause
    ;

databaseFileClauses
    : (RENAME FILE stringLiteral (',' stringLiteral)* TO stringLiteral
      | createDatafileClause
      | alterDatafileClause
      | alterTempfileClause
      | moveDatafileClause
      )
    ;

createDatafileClause
    : CREATE DATAFILE (stringLiteral | filenumber=integer) ( ',' (stringLiteral | filenumber=integer)* )?
      (AS (fileSpecification (',' fileSpecification)* | NEW))?
    ;

alterDatafileClause
    : DATAFILE (stringLiteral | filenumber=integer) (',' (stringLiteral | filenumber=integer))*
      ( ONLINE
      | OFFLINE (FOR DROP)?
      | RESIZE sizeClause
      | autoextendClause
      | END BACKUP
      | ENCRYPT
      | DECRYPT
      )
    ;

alterTempfileClause
    : TEMPFILE (stringLiteral | filenumber=integer) (',' (stringLiteral | filenumber=integer))*
      (RESIZE sizeClause
      | autoextendClause
      | DROP (INCLUDING DATAFILES)?
      | ONLINE
      | OFFLINE
      )
    ;

moveDatafileClause
    : MOVE DATAFILE (stringLiteral | filenumber=integer) (TO stringLiteral)? REUSE? KEEP?
    ;

logfileClauses
    : ((ARCHIVELOG MANUAL? | NOARCHIVELOG)
      | NO? FORCE LOGGING
      | SET STANDBY NOLOGGING FOR (DATA AVAILABILITY | LOAD PERFORMANCE)
      | RENAME FILE stringLiteral (',' stringLiteral)* TO stringLiteral
      | CLEAR UNARCHIVED? LOGFILE logfileDescriptor (',' logfileDescriptor)* (UNRECOVERABLE DATAFILE)?
      | addLogfileClauses
      | dropLogfileClauses
      | switchLogfileClause
      | supplementalDbLogging
      )
    ;

addLogfileClauses
    : ADD STANDBY? LOGFILE 
        ( ( INSTANCE stringLiteral | THREAD integer )? ( GROUP integer )? redoLogFileSpec (',' ( GROUP integer )? redoLogFileSpec)*
        | MEMBER stringLiteral REUSE? (',' stringLiteral REUSE?)* TO logfileDescriptor (',' logfileDescriptor)*
        )
    ;

dropLogfileClauses
    : DROP STANDBY? LOGFILE ( logfileDescriptor (',' logfileDescriptor)*
                            | MEMBER stringLiteral (',' stringLiteral)*
                            )
    ;

logfileDescriptor
    : GROUP integer
    | '(' stringLiteral (',' stringLiteral) ')'
    | stringLiteral
    ;

switchLogfileClause
    : SWITCH ALL LOGFILES TO BLOCKSIZE integer
    ;

supplementalDbLogging
    : (ADD | DROP) SUPPLEMENTAL LOG ( DATA
                                    | supplementalIdKeyClause
                                    | supplementalPlsqlClause
                                    | supplementalSubsetReplicationClause
                                    )
    ;

supplementalPlsqlClause
    : DATA FOR PROCEDURAL REPLICATION
    ;

supplementalSubsetReplicationClause
    : DATA SUBSET DATABASE REPLICATION
    ;

controlfileClauses
    : CREATE ((LOGICAL | PHYSICAL)? STANDBY | FAR SYNC INSTANCE) CONTROLFILE AS stringLiteral REUSE?
    | BACKUP CONTROLFILE TO (stringLiteral REUSE? | traceFileClause)
    ;

traceFileClause
    : TRACE (AS stringLiteral REUSE?)? (RESETLOGS | NORESETLOGS)?
    ;

standbyDatabaseClauses
    : (( activateStandbyDbClause
       | maximizeStandbyDbClause
       | registerLogfileClause
       | commitSwitchoverClause
       | startStandbyClause
       | stopStandbyClause
       | convertDatabaseClause
       ) parallelClause?
      )
    | switchoverClause
    | failoverClause
    ;

activateStandbyDbClause
    : ACTIVATE (PHYSICAL | LOGICAL)? STANDBY DATABASE (FINISH APPLY)?
    ;

maximizeStandbyDbClause
    : SET STANDBY DATABASE TO MAXIMIZE (PROTECTION | AVAILABILITY | PERFORMANCE)
    ;

registerLogfileClause
    : REGISTER (OR REPLACE)? (PHYSICAL | LOGICAL)?
      LOGFILE fileSpecification (',' fileSpecification)*
      (FOR logminerSessionName=identifier)?
    ;

switchoverClause
    : SWITCHOVER TO targetDbName (VERIFY | FORCE)?
    ;

failoverClause
    : FAILOVER TO targetDbName FORCE?
    ;

commitSwitchoverClause
    : (PREPARE | COMMIT) TO SWITCHOVER
      ( TO (((PHYSICAL | LOGICAL)? PRIMARY | PHYSICAL? STANDBY) ((WITH | WITHOUT) SESSION SHUTDOWN (WAIT | NOWAIT))? | LOGICAL STANDBY)
      | CANCEL)?
    ;

// TODO: scnValue
startStandbyClause
    : START LOGICAL STANDBY APPLY IMMEDIATE? NODELAY? (NEW PRIMARY dblink | INITIAL scnValue=literal? | (KW_SKIP FAILED TRANSACTION | FINISH))?
    ;

stopStandbyClause
    : (STOP | ABORT) LOGICAL STANDBY APPLY
    ;

convertDatabaseClause
    : CONVERT TO (PHYSICAL | SNAPSHOT) STANDBY
    ;

defaultSettingsClauses
    : DEFAULT EDITION '=' editionName
    | SET DEFAULT (BIGFILE | SMALLFILE) TABLESPACE
    | DEFAULT TABLESPACE tablespace
    | DEFAULT LOCAL? TEMPORARY TABLESPACE (tablespace | tablespaceGroupName)
    | RENAME GLOBAL_NAME TO database '.' domain ('.' domain)*
    | ENABLE BLOCK CHANGE TRACKING (USING FILE stringLiteral REUSE?)?
    | DISABLE BLOCK CHANGE TRACKING
    | NO? FORCE FULL DATABASE CACHING
    | CONTAINERS DEFAULT TARGET '=' ('(' containerName ')' | NONE)
    | flashbackModeClause
    | undoModeClause
    | setTimeZoneClause
    ;

flashbackModeClause
    : FLASHBACK (ON | OFF)
    ;

undoModeClause
    : LOCAL UNDO (ON | OFF)
    ;

instanceClauses
    : (ENABLE | DISABLE) INSTANCE stringLiteral
    ;

securityClause
    : GUARD (ALL | STANDBY | NONE)
    ;

prepareClause
    : PREPARE MIRROR COPY copyName=identifier (WITH (UNPROTECTED | MIRROR | HIGH) REDUNDANCY)? (FOR DATABASE targetCdbName=identifier)?
    ;

dropMirrorCopy
    : DROP MIRROR COPY mirrorName
    ;

lostWriteProtection
    : (ENABLE | DISABLE | REMOVE | SUSPEND)? LOST WRITE PROTECTION
    ;

cdbFleetClauses
    : leadCdbClause
    | leadCdbUriClause
    ;

leadCdbClause
    : SET LEAD_CDB '=' (TRUE | FALSE)
    ;

leadCdbUriClause
    : SET LEAD_CDB_URI '=' uriString=stringLiteral
    ;

propertyClause
    : PROPERTY (SET | REMOVE) DEFAULT_CREDENTIAL '=' qualifiedCredentialName=identifier
    ;

replayUpgradeClause
    : UPGRADE SYNC (ON | OFF)
    ;

alterAddCacheClause
    : ADD CACHE MEASURE GROUP (ALL | '(' measName=identifier (',' measName=identifier)* ')' )?
      LEVELS '(' alterCacheClauseItem (',' alterCacheClauseItem)* ')'
    ;

alterDropCacheClause
    : DROP CACHE MEASURE GROUP (ALL | '(' measName=identifier (',' measName=identifier)* ')' )?
      LEVELS '(' alterCacheClauseItem (',' alterCacheClauseItem)* ')'
    ;

alterCacheClauseItem
    : ((dimName=identifier '.')? hierName=identifier '.')? levelName=identifier
    ;

createViewConstraintItem
    : alias (VISIBLE | INVISIBLE)? inlineConstraint*
    | outOfLineConstraint
    ;

objectViewClause
    : OF (schema '.')? typeName ( WITH OBJECT (IDENTIFIER | ID) (DEFAULT | '(' attribute (',' attribute)* ')')
                                           | UNDER (schema '.')? superview=identifier
                                           )
      ('(' objectViewClauseConstraintItem (',' objectViewClauseConstraintItem)* ')')?
    ;

objectViewClauseConstraintItem
    : outOfLineConstraint
    | attribute inlineConstraint+
    ;

xmlTypeViewClause
    : OF XMLTYPE xmlSchemaSpec? WITH OBJECT (IDENTIFIER | ID) (DEFAULT | '(' expr (',' expr)* ')')
    ;

indexIlmClause
    : ILM ( (ADD POLICY)? policyClause
          | DELETE POLICY policyName=identifier)
    ;

policyClause
    : OPTIMIZE conditionClause
    | tieringClause plSqlFunctionName=identifier?
    | // empty rule
    ;

tieringClause
    : TIER TO LOW_COST_TBS
    ;

conditionClause
    : trackingStatisticsClause
    | ON plSqlFunctionName=identifier
    ;

trackingStatisticsClause
    : AFTER timeInterval=integer (DAYS | MONTHS | YEARS) OF NO? (ACCESS | MODIFICATION | CREATION)
    ;

clusterIndexClause
    : CLUSTER (schema '.')? cluster tAlias? ( '(' indexExpr ( ASC | DESC )? ( ',' indexExpr ( ASC | DESC )? )* ')' )? indexProperties?
    ;

tableIndexClause
    : (schema '.')? table tAlias? '(' tableIndexClauseItem (',' tableIndexClauseItem)* ')' indexProperties?
    ;

tableIndexClauseItem
    : indexExpr (ASC | DESC)?
    ;

bitmapJoinIndexClause
    : (schema '.')? table '(' bitmapJoinIndexClauseColumnItem (',' bitmapJoinIndexClauseColumnItem)* ')'
      FROM bitmapJoinIndexClauseTableItem (',' bitmapJoinIndexClauseTableItem)*
      WHERE condition localPartitionedIndex? indexAttributes
    ;

bitmapJoinIndexClauseColumnItem
    : ((schema '.')? table '.' | tAlias '.')? column (ASC | DESC)?
    ;

bitmapJoinIndexClauseTableItem
    : (schema '.')? table tAlias?
    ;

indexExpr
    : column
    | columnExpression=expr
    ;

relationalTable
    : ('(' relationalProperties ')')? blockchainTableClauses?
      (DEFAULT COLLATION identifier)? (ON COMMIT (DROP|PRESERVE) DEFINITION)?
      (ON COMMIT (DELETE|PRESERVE)? ROWS)? physicalProperties? tableProperties
    ;

objectTable
    : OF (schema '.')? objectType objectTableSubstitution?
      ('(' objectProperties ')')? (ON COMMIT (DELETE | PRESERVE) ROWS)?
      oidClause? oidIndexClause? physicalProperties? tableProperties
    ;

xmlTypeTable
    : OF XMLTYPE ('(' objectProperties ')')? (XMLTYPE xmlTypeStorage)? xmlSchemaSpec?
      xmlTypeVirtualColumns? (ON COMMIT (DELETE | PRESERVE) ROWS)? oidClause?
      oidIndexClause? physicalProperties? tableProperties
    ;

relationalProperties
    : relationalProperty (',' relationalProperty)*
    ;

relationalProperty
    : columnDefinition
    | virtualColumnDefinition
    | periodDefinition
    | outOfLineConstraint
    | outOfLineRefConstraint
    | supplementalLoggingProps
    ;

columnDefinition
    : column ( datatype (COLLATE columnCollateName=identifier)? )? SORT? ( VISIBLE | INVISIBLE )?
      ( DEFAULT ( ON NULL )? expr | identityClause)?
      ( ENCRYPT encryptionSpec )?
      ( inlineConstraint+ | inlineRefConstraint )?
    ;

virtualColumnDefinition
    : column (datatype (COLLATE columnCollactionName=identifier)? )? (VISIBLE | INVISIBLE)?
      (GENERATED | ALWAYS)? AS '(' columnExpression=expr ')' VIRTUAL? evaluationEditionClause?
      unusableEditionsClause? inlineConstraint*
    ;

evaluationEditionClause
    : EVALUATE USING ( CURRENT EDITION
                     | EDITION edition=identifier
                     | NULL EDITION
                     )
    ;

unusableEditionsClause
    : UNUSABLE BEFORE ( CURRENT EDITION | EDITION edition=identifier)
    | ( UNUSABLE BEFORE ( CURRENT EDITION | EDITION edition=identifier) )? 
        UNUSABLE BEGINNING WITH ( CURRENT EDITION | EDITION edition=identifier | NULL EDITION )
    | UNUSABLE BEGINNING WITH ( CURRENT EDITION | EDITION edition=identifier | NULL EDITION )
        ( UNUSABLE BEFORE ( CURRENT EDITION | EDITION edition=identifier) )? 
    ;

periodDefinition
    : PERIOD FOR validTimeColumn ('(' startTimeColumn ',' endTimeColumn ')')?
    ;

supplementalLoggingProps
    : SUPPLEMENTAL LOG ( supplementalLogGrpClause
                       | supplementalIdKeyClause
                       )
    ;

supplementalLogGrpClause
    : GROUP logGroup '(' supplementalLogGrpClauseItem (',' supplementalLogGrpClauseItem)* ')' ALWAYS?
    ;

supplementalLogGrpClauseItem
    : column (NO LOG)?
    ;

supplementalIdKeyClause
    : DATA '(' supplementalIdKeyClauseOption (',' supplementalIdKeyClauseOption)* ')' COLUMNS
    ;

supplementalIdKeyClauseOption
    : ALL
    | PRIMARY KEY
    | UNIQUE
    | FOREIGN KEY
    ;

referencesClause
    : REFERENCES (schema '.')? identifier ('(' column (',' column)* ')')?
      (ON DELETE (CASCADE | SET NULL))?
    ;

constraintState
    : ( NOT? DEFERRABLE (INITIALLY (IMMEDIATE | DEFERRED))?
      | INITIALLY (IMMEDIATE | DEFERRED) NOT? DEFERRABLE?
      )?
      (RELY | NORELY)?
      usingIndexClause?
      (ENABLE | DISABLE)?
      (VALIDATE | NOVALIDATE)?
      exceptionsClause?
    ;

usingIndexClause
    : USING INDEX ((schema '.')? index
                  | '(' createIndex ')'
                  | indexProperties?
                  )
    ;

indexProperties
    : ( globalPartitionedIndex | localPartitionedIndex | indexAttributes )+
    | INDEXTYPE IS (domainIndexClause | xmlIndexClause)
    ;

domainIndexClause
    : indextype localDomainIndexClause? parallelClause? (PARAMETERS '(' stringLiteral ')')?
    ;

xmlIndexClause
    : (XDB '.')? XMLINDEX localXmlIndexClause? parallelClause? xmlIndexParametersClause?
    ;

localXmlIndexClause
    : LOCAL ('(' localXmlIndexClauseItem (',' localXmlIndexClauseItem) ')')?
    ;

localXmlIndexClauseItem
    : PARTITION partition xmlIndexParametersClause?
    ;

// cannot find definition copy from domainIndexClause
xmlIndexParametersClause
    : PARAMETERS '(' stringLiteral ')'
    ;

localDomainIndexClause
    : LOCAL ('(' localDomainIndexClauseItem (',' localDomainIndexClauseItem)* ')')?
    ;

localDomainIndexClauseItem
    : PARTITION partition (PARAMETERS '(' stringLiteral ')')?
    ;

globalPartitionedIndex
    : GLOBAL PARTITION BY ( RANGE columnList '(' indexPartitioningClause ( ',' indexPartitioningClause )* ')'
                          | HASH columnList (individualHashPartitions | hashPartitionsByQuantity)
                          )
    ;

localPartitionedIndex
    : LOCAL ( onRangePartitionedTable
            | onListPartitionedTable
            | onHashPartitionedTable
            | onCompPartitionedTable
            )?
    ;

onRangePartitionedTable
    : '(' onRangePartitionedTableItem (',' onRangePartitionedTableItem)* ')'
    ;

onRangePartitionedTableItem
    : PARTITION partition? (segmentAttributesClause|indexCompression)* (USABLE|UNUSABLE)?
    ;

onListPartitionedTable
    : '(' onListPartitionedTableItem (',' onListPartitionedTableItem)* ')'
    ;

onListPartitionedTableItem
    : PARTITION partition? (segmentAttributesClause|indexCompression)* (USABLE|UNUSABLE)?
    ;

onHashPartitionedTable
    : STORE IN '(' tablespace (',' tablespace)* ')'
    | '(' onHashPartitionedTableItem (',' onHashPartitionedTableItem)* ')'
    ;

onHashPartitionedTableItem
    : PARTITION partition? (TABLESPACE tablespace)? indexCompression? (USABLE|UNUSABLE)?
    ;

onCompPartitionedTable
    : (STORE IN '(' tablespace (',' tablespace) ')')
    | '(' onCompPartitionedTableItem (',' onCompPartitionedTableItem)* ')'
    ;

onCompPartitionedTableItem
    : PARTITION partition? (segmentAttributesClause|indexCompression)* (USABLE|UNUSABLE)? indexSubpartitionClause?
    ;

indexAttributes
    : indexAttributesItem+
    ;

indexAttributesItem
    : physicalAttributesClause
    | loggingClause
    | ONLINE
    | TABLESPACE (tablespace | DEFAULT)
    | indexCompression
    | SORT
    | NOSORT
    | REVERSE
    | VISIBLE
    | INVISIBLE
    | partitionIndexClause
    | parallelClause
    ;

partitionIndexClause
    : INDEXING (PARTIAL | FULL)
    ;

parallelClause
    : NOPARALLEL
    | PARALLEL integer?
    ;

indexSubpartitionClause
    : STORE IN '(' tablespace (',' tablespace) ')'
    | '(' indexSubpartitionClauseItem (',' indexSubpartitionClauseItem)* ')'
    ;

indexSubpartitionClauseItem
    : SUBPARTITION subpartition? (TABLESPACE tablespace)? indexCompression? (USABLE|UNUSABLE)?
    ;

indexCompression
    : prefixCompression
    | advancedIndexCompression
    ;

prefixCompression
    : COMPRESS integer?
    | NOCOMPRESS
    ;

advancedIndexCompression
    : COMPRESS ADVANCED (LOW | HIGH)?
    | NOCOMPRESS
    ;

indexPartitioningClause
    : PARTITION partition? VALUES LESS THAN '(' expr (',' expr )* ')' segmentAttributesClause?
    ;

exceptionsClause
    : EXCEPTIONS INTO (schema '.')? table
    ;

identityClause
    : GENERATED (ALWAYS|BY DEFAULT (ON NULL)?)? AS IDENTITY ('(' identityOptions ')')?
    ;

identityOptions
    : identityOption+
    ;

identityOption
    : START WITH (integer|LIMIT VALUE)
    | INCREMENT BY integer
    | MAXVALUE integer
    | NOMAXVALUE
    | MINALUE integer
    | NOMINVALUE
    | CYCLE
    | NOCYCLE
    | CACHE integer
    | NOCACHE
    | ORDER
    | NOORDER
    ;

encryptionSpec
    : (USING encryptAlgorithm=stringLiteral)? (IDENTIFIED BY password)? (integrityAlgorithm=stringLiteral)? (NO? SALT)?
    ;

blockchainTableClauses
    : blockchainDropTableClause blockchainRowRetentionClause blockchainHashAndDataFormatClause
    ;

blockchainDropTableClause
    : NO DROP (UNTIL numberLiteral DAYS IDLE)
    ;

blockchainRowRetentionClause
    : NO DELETE (UNTIL numberLiteral DAYS AFTER INSERT)? (LOCKED)?
    ;

blockchainHashAndDataFormatClause
    : HASHING USING SHA2_512 VERSION V1
    ;

physicalProperties
    : deferredSegmentCreation? segmentAttributesClause tableCompression? inmemoryTableClause? ilmClause?
    | deferredSegmentCreation segmentAttributesClause? tableCompression? inmemoryTableClause? ilmClause?
    | deferredSegmentCreation? ( ORGANIZATION ( HEAP segmentAttributesClause? heapOrgTableClause
                                              | INDEX segmentAttributesClause? indexOrgTableClause
                                              | EXTERNAL externalTableClause
                                              )
                               | EXTERNAL PARTITION ATTRIBUTES externalTableClause (REJECT LIMIT)?
                               )
    | CLUSTER cluster '(' column (',' column )* ')'
    ;

objectTableSubstitution
    : NOT? SUBSTITUTABLE AT ALL LEVELS
    ;

objectProperties
    : objectProperty (',' objectProperty)*
    ;

objectProperty
    : identifier (DEFAULT expr)? (inlineConstraint+|inlineRefConstraint)?
    | outOfLineConstraint
    | outOfLineRefConstraint
    | supplementalLoggingProps
    ;

oidClause
    : OBJECT IDENTIFIER IS (SYSTEM GENERATED | PRIMARY KEY)
    ;

oidIndexClause
    : OIDINDEX index? '(' (physicalAttributesClause | TABLESPACE tablespace)* ')'
    ;

deferredSegmentCreation
    : SEGMENT CREATION (IMMEDIATE | DEFERRED)
    ;

segmentAttributesClause
    : ( physicalAttributesClause
      | TABLESPACE tablespace
      | TABLESPACE SET tablespaceSet
      | loggingClause
      )+
    ;

heapOrgTableClause
    : tableCompression? inmemoryTableClause ilmClause?
    ;

indexOrgTableClause
    : indexOrgTableClauseItem+ indexOrgOverflowClause?
    | indexOrgOverflowClause
    ;

indexOrgTableClauseItem
    : mappingTableClause
    | PCTTHRESHOLD integer
    | prefixCompression
    ;

externalTableClause
    : '(' (TYPE accessDriverType)? externalTableDataProps* ')' (REJECT LIMIT (integer | UNLIMITED))?
    ;

externalTableDataProps
    : DEFAULT DIRECTORY directory=identifier
    | ACCESS PARAMETERS ( '(' opaqueFormatSpec=etRecordSpec ')'
                         | USING CLOB subquery
                         )
    | LOCATION '(' externalTableDataPropsLocation (',' externalTableDataPropsLocation)* ')'
    ;

etRecordSpec
    : RECORDS ( (FIXED | VARIABLE) integer
              | DELIMITED BY (DETECTED? NEWLINE | etString)
              | XMLTAG etString
              )
      etRecordSepcOptions
    ;

etRecordSepcOptions
    : etRecordSepcOptionsItem+
    ;

etRecordSepcOptionsItem
    : CHARACTERSET etString
    | PREPROCESSOR (directory=identifier ':')? etString
    | PREPROCESSOR_TIMEOUT integer
    | EXTERNAL VARIABLE DATA (LOGFILE | NOLOGFILE | READSIZE | PREPROCESSOR)?
    | (LANGUAGE | TERRITORY) etString
    | DATA IS (LITTLE | BIG) ENDIAN
    | BYTEORDERMARK (CHECK | NOCHECK)
    | STRING SIZES ARE IN (BYTES | CHARACTERS)
    | LOAD WHEN etConditionSpec
    | etOutputFiles
    | READSIZE integer
    | DISABLE_DIRECTORY_LINK_CHECK
    | (DATE_CACHE | KW_SKIP) integer
    | FIELD_NAMES ( FIEST FILE IGNORE?
                  | ALL FILES IGNORE?
                  | NONE
                  )
    | IO_OPTIONS '(' (DIRECTIO | NODIRECTIO) ')'
    | DNFS_ENABLE
    | DNFS_DISABLE
    | DNFS_READBUFFERS integer
    | etFieldsClause
    ;

etConditionSpec
    : condition
    | etConditionSpec (AND | OR ) etConditionSpec
    | '(' condition ')'
    | '(' etConditionSpec (AND | OR ) etConditionSpec ')'
    ;

etOutputFiles
    : etOutputFilesItem+
    ;

etOutputFilesItem
    : NOBADFILE
    | BADFILE (directory=identifier ':')? etString?
    | NODISCARDFILE
    | DISCARDFILE (directory=identifier ':')? etString?
    | NOLOGFILE
    | LOGFILE (directory=identifier ':')? etString?
    ;

externalTableDataPropsLocation
    : (directory=identifier ':')? etString
    ;

etFieldsClause
    : FIELDS ( IGNORE_CHARS_AFTER_EOR
             | CSV ((WITH | WITHOUT) EMBEDDED)?
             | etDelimSpec
             | etTrimSpec
             | ALL FIELDS OVERRIDE THESE FIELDS
             | MISSING FIELD VALUES ARE NULL
             | REJECT ROWS WITH ALL NULL FIELDS
             | (DATE_FORMAT (DATE | TIMESTAMP)* MASK stringLiteral)+
             | NULLIF
             | NONULLIF
             )?
      etFieldList?
    ;

etDelimSpec
    : ENCLOSED BY etString (AND etString)*
    | TERMINATED BY (etString | WHITESPACE) (OPTIONALLY? ENCLOSED BY etString (AND etString)?)?
    ;

etTrimSpec
    : LRTRIM
    | NOTRIM
    | LTRIM
    | RTRIM
    | LDRTRIM
    ;

etFieldList
    : '(' etFieldListItem (',' etFieldListItem)* ')'
    ;

etFieldListItem
    : fieldName=identifier etPositionSpec? etDataTypeSpec? etInitSpec? etLlsSpec?
    ;

etPositionSpec
    : POSITION? '(' (start=numberLiteral | '*' | ('+' | '-') increment=numberLiteral) (':' | '-') end=numberLiteral ')'
    ;

etDataTypeSpec
    : UNSIGNED? INTEGER EXTERNAL? ('(' len=numberLiteral ')')? etDelimSpec?
    | (DECIMAL | ZONED) (EXTERNAL ('(' len=numberLiteral ')')? etDelimSpec? | '(' precision ('.' scale)? ')')?
    | ORACLE_DATE
    | ORACLE_NUMBER COUNTED?
    | FLOAT EXTERNAL? ('(' len=numberLiteral ')')? etDelimSpec?
    | DOUBLE
    | BINARY_FLOAT EXTERNAL? ('(' len=numberLiteral ')')? etDelimSpec?
    | BINARY_DOUBLE
    | RAW ('(' len=numberLiteral ')')?
    | CHAR ('(' len=numberLiteral ')')? etDelimSpec? etTrimSpec? etDateFormatSpec?
    | (VARCHAR | VARRAW | VARCHARC | VARRAWC) '(' (lengthOfLength=numberLiteral '.')? maxLen=numberLiteral ')'
    ;

etDateFormatSpec
    : DATE_FORMAT?
    ( (DATE | TIMESTAMP (WITH LOCAL? TIME ZONE)?)+ MASK identifier
    | INTERVAL (YEAR_TO_MONTH | DAY_TO_SECOND)
    )
    ;

etInitSpec
    : (DEFAULTIF | NULLIF) etConditionSpec
    ;

etLlsSpec
    : LLS identifier?
    ;

etString
    : (TK_HEXA | KW_X)? (stringLiteral | identifier)
    ;

mappingTableClause
    : MAPPING TABLE
    | NOMAPPING
    ;

indexOrgOverflowClause
    : ( INCLUDING column )? OVERFLOW segmentAttributesClause?
    ;

tableCompression
    : COMPRESS
    | ROW STORE COMPRESS (BASIC | ADVANCED)?
    | COLUMN STORE COMPRESS (FOR (QUERY | ARCHIVE) (LOW | HIGH)? )? (NO? ROW LEVEL LOCKING)?
    | NOCOMPRESS
    ;

inmemoryTableClause
    : INMEMORY inmemoryAttributes inmemoryColumnClause? 
    | NO INMEMORY inmemoryColumnClause?
    ;

inmemoryAttributes
    : inmemoryMemcompress? inmemoryPriority? inmemoryDistribute? inmemoryDuplicate? inmemorySpatial?
    ;

inmemoryMemcompress
    : MEMCOMPRESS FOR (DML | (QUERY | CAPACITY) (LOW | HIGH)? )
    | NO MEMCOMPRESS
    | MEMCOMPRESS AUTO
    ;

inmemoryPriority
    : PRIORITY (NONE | LOW | MEDIUM | HIGH | CRITICAL)
    ;

inmemoryDistribute
    : DISTRIBUTE (AUTO | BY (ROWID RANGE | PARTITION | SUBPARTITION))? (FOR SERVICE (DEFAULT | ALL | serviceName | NONE))?
    ;

inmemoryDuplicate
    : DUPLICATE ALL?
    | NO DUPLICATE
    ;

inmemorySpatial
    : SPATIAL column
    ;

inmemoryColumnClause
    : inmemoryColumnClauseItem+
    ;

inmemoryColumnClauseItem
    : ( INMEMORY inmemoryMemcompress?
      | NO INMEMORY
      ) '(' column (',' column)* ')'
    ;

ilmClause
    : ILM ( ADD POLICY ilmPolicyClause
          | (DELETE | ENABLE | DISABLE) POLICY ilmPolicyName=identifier
          | (DELETE_ALL | ENABLE_ALL | DISABLE_ALL)
          )
    ;

ilmPolicyClause
    : ilmCompressionPolicy
    | ilmTieringPolicy
    | ilmInmemoryPolicy
    ;

ilmCompressionPolicy
    : tableCompression (SEGMENT | GROUP) (AFTER ilmTimePeriod OF (NO ACCESS | NO MODIFICATION | CREATION) | ON functionName)
    | (ROW STORE COMPRESS ADVANCED | COLUMN STORE COMPRESS FOR QUERY) ROW AFTER ilmTimePeriod OF NO MODIFICATION
    ;

ilmTieringPolicy
    : TIER TO tablespace (SEGMENT | GROUP)? (ON functionName)?
    | TIER TO tablespace READ ONLY (SEGMENT | GROUP)? (AFTER ilmTimePeriod OF (NO ACCESS | NO MODIFICATION | CREATION) | ON functionName)
    ;

ilmInmemoryPolicy
    : ( SET INMEMORY inmemoryAttributes
      | MODIFY INMEMORY inmemoryMemcompress
      | NO INMEMORY
      ) SEGMENT?
      ( AFTER ilmTimePeriod OF (NO ACCESS | NO MODIFICATION | CREATION)
      | ON functionName
      )
    ;

ilmTimePeriod
    : integer (DAY | DAYS | MONTH | MONTHS | YEAR | YEARS)
    ;

physicalAttributesClause
    : ( PCTFREE integer
      | PCTUSED integer
      | INITRANS integer
      | storageClause
      )+
    ;

loggingClause
    : LOGGING
    | NOLOGGING
    | FILESYSTEM_LIKE_LOGGING
    ;

storageClause
    : STORAGE '(' storageClauseOption+ ')'
    ;

storageClauseOption
    : INITIAL sizeClause
    | NEXT sizeClause
    | MINEXTENTS integer
    | MAXEXTENTS (integer | UNLIMITED)
    | maxsizeClause
    | PCTINCREASE integer
    | FREELISTS integer
    | FREELIST GROUPS integer
    | OPTIMAL (sizeClause|NULL)?
    | BUFFER_POOL (KEEP | RECYCLE | DEFAULT)
    | FLASH_CACHE (KEEP | NONE | DEFAULT)
    | CELL_FLASH_CACHE (KEEP | NONE | DEFAULT)
    | ENCRYPT
    ;

tableProperties
    : ( columnProperties
      | readOnlyClause
      | indexingClause
      | tablePartitioningClauses
      | attributeClusteringClause
      | (CACHE | NOCACHE)
      | resultCacheClause
      | parallelClause
      | (ROWDEPENDENCIES | NOROWDEPENDENCIES)
      | enableDisableClause
      | rowMovementClause
      | logicalReplicationClause
      | flashbackArchiveClause
      | (ROW ARCHIVAL)
      | (AS subquery | (FOR EXCHANGE WITH TABLE (schema '.')? table))
      )*
    ;

columnProperties
    : columnProperty+
    ;

columnProperty
    : objectTypeColProperties
    | nestedTableColProperties
    | (varrayColProperties|lobStorageClause) ('(' lobPartitionStorage (',' lobPartitionStorage)* ')')?
    | xmlTypeColumnProperties
    | jsonStorageClause
    ;

readOnlyClause
    : READ ONLY
    | READ WRITE
    ;

indexingClause
    : INDEXING (ON | OFF)
    ;

tablePartitioningClauses
    : rangePartitions
    | listPartitions
    | hashPartitions
    | compositeRangePartitions
    | compositeListPartitions
    | compositeHashPartitions
    | referencePartitioning
    | systemPartitioning
    | consistentHashPartitions
    | consistentHashWithSubpartitions
    | partitionsetClauses
    ;

rangePartitions
    : PARTITION BY RANGE '(' column (',' column)* ')'
      (INTERVAL '(' expr ')' (STORE IN '(' tablespace (',' tablespace)* ')')? )?
      '(' rangePartitionsItem (',' rangePartitionsItem)* ')'
    ;

rangePartitionsItem
    : (PARTITION partition rangeValuesClause tablePartitionDescription externalPartSubpartDataProps?)
    ;

externalPartSubpartDataProps
    : (DEFAULT DIRECTORY directory=identifier)? (LOCATION '(' externalPartSubpartDataPropsItem (',' externalPartSubpartDataPropsItem)* ')')
    ;

externalPartSubpartDataPropsItem
    : (directory=identifier ':')? stringLiteral
    ;

hashPartitions
    : PARTITION BY HASH '(' column (',' column)* ')' (individualHashPartitions | hashPartitionsByQuantity)
    ;

individualHashPartitions
    : '(' individualHashPartitionsItem (',' individualHashPartitionsItem)* ')'
    ;

individualHashPartitionsItem
    : PARTITION partition? readOnlyClause? indexingClause? partitioningStorageClause?
    ;

hashPartitionsByQuantity
    : PARTITIONS hashPartitionsQuantity=integer
      (STORE IN '(' tablespace (',' tablespace)* ')')?
      (tableCompression | indexCompression)?
      (OVERFLOW STORE IN '(' tablespace (',' tablespace)* ')')?
    ;

listPartitions
    : PARTITION BY LIST '(' column (',' column)* ')'
      (AUTOMATIC (STORE IN '(' tablespace (',' tablespace)* ')')?)?
      '(' listPartitionsItem (',' listPartitionsItem) ')'
    ;

listPartitionsItem
    : PARTITION partition? listValuesClause tablePartitionDescription externalPartSubpartDataProps?
    ;

compositeRangePartitions
    : PARTITION BY RANGE '(' column (',' column)* ')'
      (INTERVAL '(' expr ')' (STORE IN '(' tablespace (',' tablespace)* ')')?)?
      (subpartitionByRange | subpartitionByList | subpartitionByHash)
      '(' rangePartitionDesc (',' rangePartitionDesc)* ')'
    ;

compositeHashPartitions
    : PARTITION BY HASH '(' column (',' column)* ')'
      (subpartitionByRange | subpartitionByList | subpartitionByHash)
      (individualHashPartitions | hashPartitionsByQuantity)
    ;

compositeListPartitions
    : PARTITION BY LIST '(' column (',' column)* ')'
      (AUTOMATIC (STORE IN '(' tablespace (',' tablespace)* ')')?)?
      (subpartitionByRange | subpartitionByList | subpartitionByHash)
      '(' listPartitionDesc (',' listPartitionDesc)* ')'
    ;

referencePartitioning
    : PARTITION BY REFERENCE '(' constraint ')' ('(' referencePartitionDesc* ')')?
    ;

referencePartitionDesc
    : PARTITION partition? tablePartitionDescription ')'
    ;

systemPartitioning
    : PARTITION BY SYSTEM (PARTITIONS integer | referencePartitionDesc (',' referencePartitionDesc*)?)?
    ;

consistentHashPartitions
    : PARTITION BY CONSISTENT HASH '(' column (',' column)* ')' (PARTITIONS AUTO)? TABLESPACE SET tablespaceSet
    ;

consistentHashWithSubpartitions
    : PARTITION BY CONSISTENT HASH '(' column (',' column)* ')'
      (subpartitionByRange | subpartitionByList | subpartitionByHash)
      (PARTITIONS AUTO)?
    ;

partitionsetClauses
    : rangePartitionsetClause
    | listPartitionsetClause
    ;

rangePartitionsetClause
    : PARTITIONSET BY RANGE '(' column (',' column)* ')'
      PARTITION BY CONSISTENT HASH '(' column (',' column)* ')'
      (SUBPARTITION BY ((RANGE | HASH) '(' column (',' column)* ')' | LIST '(' column ')') subpartitionTemplate?)?
      PARTITIONS AUTO '(' rangePartitionsetDesc (',' rangePartitionsetDesc)* ')'
    ;

rangePartitionsetDesc
    : PARTITIONSET partitionSet=identifier rangeValuesClause (TABLESPACE SET tablespaceSet)?
      lobStorageClause?
      (SUBPARTITIONS STORE IN '(' tablespaceSet (',' tablespaceSet) ')')?
    ;

listPartitionsetClause
    : PARTITIONSET BY LIST '(' column ')'
      PARTITION BY CONSISTENT HASH '(' column (',' column)* ')'
      (SUBPARTITION BY ((RANGE | HASH) '(' column (',' column)* ')' | LIST '(' column ')') subpartitionTemplate?)?
      PARTITIONS AUTO '(' listPartitionsetDesc (',' listPartitionsetDesc)* ')'
    ;

listPartitionsetDesc
    : PARTITIONSET partitionSet=identifier listValuesClause? (TABLESPACE SET tablespaceSet)? lobStorageClause?
      (SUBPARTITIONS STORE IN '(' tablespaceSet (',' tablespaceSet) ')')?
    ;

rangePartitionDesc
    : PARTITION partition? rangeValuesClause? tablePartitionDescription
      ( '(' ( rangeSubpartitionDesc (',' rangeSubpartitionDesc)*
            | listSubpartitionDesc (',' listSubpartitionDesc)*
            | individualHashSubparts (',' individualHashSubparts)*
            )
        ')'
      | hashSubpartsByQuantity
      )?
    ;

listPartitionDesc
    : PARTITION partition? listValuesClause? tablePartitionDescription
      ( '(' ( rangeSubpartitionDesc (',' rangeSubpartitionDesc)*
            | listSubpartitionDesc (',' listSubpartitionDesc)*
            | individualHashSubparts (',' individualHashSubparts)*
            )
        ')'
      | hashSubpartsByQuantity
      )?
    ;

subpartitionTemplate
    : SUBPARTITION TEMPLATE
      ( '(' ( rangeSubpartitionDesc (',' rangeSubpartitionDesc)*
            | listSubpartitionDesc (',' listSubpartitionDesc)*
            | individualHashSubparts (',' individualHashSubparts)*
            )
        ')'
      | hashSubpartsByQuantity
      )
    ;

subpartitionByRange
    : SUBPARTITION BY RANGE '(' column (',' column)* ')' subpartitionTemplate?
    ;

subpartitionByList
    : SUBPARTITION BY LIST '(' column (',' column)* ')' subpartitionTemplate?
    ;

subpartitionByHash
    : SUBPARTITION BY HASH '(' column (',' column)* ')'
      (SUBPARTITIONS integer (STORE IN '(' tablespace (',' tablespace)* ')')? | subpartitionTemplate)?
    ;

rangeSubpartitionDesc
    : SUBPARTITION subpartition? rangeValuesClause readOnlyClause? indexingClause? partitioningStorageClause? externalPartSubpartDataProps?
    ;

listSubpartitionDesc
    : SUBPARTITION subpartition? listValuesClause readOnlyClause? indexingClause? partitioningStorageClause? externalPartSubpartDataProps?
    ;

individualHashSubparts
    : SUBPARTITION subpartition? readOnlyClause? indexingClause? partitioningStorageClause?
    ;

hashSubpartsByQuantity
    : SUBPARTITIONS integer (STORE IN '(' tablespace (',' tablespace)* ')')?
    ;

rangeValuesClause
    : VALUES LESS THAN '(' expr (',' expr )* ')'
    ;

listValuesClause
    : VALUES '(' listValues | DEFAULT ')'
    ;

listValues
    : ((literal | NULL) (',' (literal | NULL))*)
    | '(' (literal | NULL) (',' (literal | NULL))* ')' (',' '(' (literal | NULL) (',' (literal | NULL))* ')')?
    ;

tablePartitionDescription
    : (INTERNAL | EXTERNAL)? deferredSegmentCreation? readOnlyClause? indexingClause? segmentAttributesClause?
      (tableCompression | prefixCompression)? inmemoryClause? ilmClause? (OVERFLOW segmentAttributesClause?)?
      (lobStorageClause | varrayColProperties | nestedTableColProperties)*
    ;

partitioningStorageClause
    : partitioningStorageClauseItem+
    ;

partitioningStorageClauseItem
    : TABLESPACE tablespace
    | TABLESPACE SET tablespaceSet
    | OVERFLOW ( TABLESPACE tablespace
               | TABLESPACE SET tablespaceSet)?
    | tableCompression
    | indexCompression
    | inmemoryClause
    | ilmClause
    | lobPartitionStorage
    | VARRAY varrayItem STORE AS (SECUREFILE | BASICFILE)? LOB lobSegName
    | jsonStorageClause
    ;

inmemoryClause
    : INMEMORY inmemoryAttributes (TEXT ( identifier (',' identifier)*
                                         | identifier USING identifier (',' identifier USING identifier)*))?
    | NO INMEMORY
    ;

attributeClusteringClause
    : CLUSTERING clusteringJoin? clusterClause clusteringWhen zonemapClause?
    ;

resultCacheClause
    : RESULT_CACHE ( '(' ((MODE '(' DEFAULT | FORCE ')')? (',' STANDBY '(' ENABLE | DISABLE ')')?)
                   | '(' (STANDBY '(' ENABLE | DISABLE ')')? (',' MODE (DEFAULT | FORCE))? ')' ')')
    ;

clusteringJoin
    : (schema '.')? table clusteringJoinItem (',' clusteringJoinItem)*
    ;

clusteringJoinItem
    : JOIN (schema '.')? table ON '(' joinCondition ')'
    ;

clusterClause
    : BY (LINEAR | INTERLEAVED)? ORDER clusteringColumns
    ;

clusteringColumns
    : clusteringColumnGroup | '(' clusteringColumnGroup (',' clusteringColumnGroup)* ')'
    ;

clusteringColumnGroup
    : '(' column (',' column)* ')'
    ;

clusteringWhen
    : ((YES | NO) ON LOAD)?
      ((YES | NO) ON DATA MOVEMENT)?
    ;

zonemapClause
    : WITH MATERIALIZED ZONEMAP ('(' zonemapName ')')?
    | WITHOUT MATERIALIZED ZONEMAP
    ;

enableDisableClause
    : (ENABLE | DISABLE) (VALIDATE | NOVALIDATE)?
      (UNIQUE '(' column (',' column)* ')' | PRIMARY KEY | CONSTRAINT constraintName)
      usingIndexClause? exceptionsClause? CASCADE? ((KEEP | DROP) INDEX)?
    ;

objectTypeColProperties
    : COLUMN column substitutableColumnClause
    ;

substitutableColumnClause
    : ELEMENT? IS OF TYPE? '(' ONLY type ')'
    | NOT? SUBSTITUTABLE AT ALL LEVELS
    ;

nestedTableColProperties
    : NESTED TABLE (nestedItem=identifier | COLUMN_VALUE) substitutableColumnClause? (LOCAL | GLOBAL)?
      STORE AS storageTable=identifier ('(' ('(' objectProperties ')' | physicalProperties | columnProperties)+ ')')?
      (RETURN AS? (LOCATOR | VALUE))?
    ;

varrayColProperties
    : VARRAY varrayItem (substitutableColumnClause? varrayStorageClause | substitutableColumnClause)
    ;

xmlTypeColumnProperties
    : XMLTYPE COLUMN? column xmlTypeStorage? xmlSchemaSpec?
    ;

xmlTypeStorage
    : STORE
      (AS ( OBJECT RELATIONAL
          | (SECUREFILE | BASICFILE)? (CLOB | BINARY XML) (lobSegName ('(' lobStorageParameters ')')? | '(' lobStorageParameters ')')?)
      | (ALL VARRAYS AS (LOBS | TABLES))
      )
    ;

xmlSchemaSpec
    : (XMLSCHEMA xmlSchemaUrl=identifier)? ELEMENT (element=identifier | xmlSchemaUrl=identifier '#' element=identifier)
      (STORE ALL VARRAYS AS (LOBS|TABLES))?
      ((ALLOW | DISALLOW) NONSCHEMA)?
      ((ALLOW | DISALLOW) ANYSCHEMA)?
    ;

varrayStorageClause
    : STORE AS (SECUREFILE | BASICFILE)? LOB (lobSegName? '(' lobStorageParameters ')'| lobSegName)
    ;

lobStorageClause
    : LOB '(' lobItem (',' lobItem)* ')' STORE AS ( SECUREFILE | BASICFILE | lobSegName | '(' lobStorageParameters ')' )+
    ;

lobPartitionStorage
    : PARTITION partition (lobStorageClause | varrayColProperties)+
      ('(' SUBPARTITION subpartition (lobStorageClause | varrayColProperties)+ ')')?
    ;

lobStorageParameters
    : lobStorageParameter+
    ;

lobStorageParameter
    : ( TABLESPACE tablespace
      | TABLESPACE SET tablespaceSet
      ) storageClause?
    | lobParameters storageClause?
    ;

lobParameters
    : (ENABLE | DISABLE) STORAGE IN ROW
    | CHUNK integer
    | PCTVERSION integer
    | FREEPOOLS integer
    | lobRetentionClause
    | lobDeduplicateClause
    | lobCompressionClause
    | ENCRYPT encryptionSpec
    | DECRYPT
    | ( CACHE
      | NOCACHE
      | CACHE READS
      ) loggingClause?
    ;

lobRetentionClause
    : RETENTION ( MAX
                | MIN integer
                | AUTO
                | NONE
                )?
    ;

lobDeduplicateClause
    : DEDUPLICATE
    | KEEP_DUPLICATES
    ;

lobCompressionClause
    : COMPRESS (HIGH | MEDIUM | LOW)?
    | NOCOMPRESS
    ;

xmlTypeVirtualColumns
    : VIRTUAL COLUMNS '(' xmlTypeVirtualColumnsItem (',' xmlTypeVirtualColumnsItem)* ')'
    ;

xmlTypeVirtualColumnsItem
    : column AS '(' expr ')'
    ;

jsonStorageClause
    : JSON '(' jsonColumn=identifier ')'* STORE AS ('(' jsonParameters ')' | lobSegName? ('(' jsonParameters ')')?)
    ;

jsonParameters
    : jsonParametersItem (',' jsonParametersItem)*
    ;

jsonParametersItem
    : TABLESPACE tablespace
    | storageClause
    | (CHUNK | PCTVERSION | FREEPOOLS) integer
    | RETENTION
    ;

rowMovementClause
    : (ENABLE | DISABLE) ROW MOVEMENT
    ;

logicalReplicationClause
    : DISABLE LOGICAL REPLICATION
    | ENABLE LOGICAL REPLICATION (ALL KEYS | ALLOW NOVALIDATE KEYS)?
    ;

flashbackArchiveClause
    : FLASHBACK ARCHIVE flashbackArchive=identifier?
    | NO FLASHBACK ARCHIVE
    ;

privilegeAuditClause
    : PRIVILEGES systemPrivilege (',' systemPrivilege)*
    ;

actionAuditClause
    : (standardActions|componentActions)+
    ;

standardActions
    : ACTIONS standardAction (',' standardAction)*
    ;

standardAction
    : (objectAction | ALL) ON
       (
         DIRECTORY directoryName
       | MINING MODEL (schema '.')? identifier
       | (schema '.')? identifier
       )                                        #objectStandardAction
    | (systemAction | ALL)                        #systemStandardAction
    ;

componentActions
    : ACTIONS COMPONENT '='
      (
        (DATAPUMP | DIRECT_LOAD | OLS | XS) componentAction (',' componentAction)*
      | DV componentAction ON identifier (',' componentAction ON identifier)*
      | PROTOCOL (FTP | HTTP | AUTHENTICATION)
      )
    ;

roleAuditClause
    : ROLES role (',' role)*
    ;

objectPrivilege
    : ALTER
    | READ
    | SELECT
    | WRITE
    | EXECUTE
    | USE
    | FLASHBACK ARCHIVE
    | ON COMMIT REFRESH
    | QUERY REWRITE
    | DEBUG
    | UNDER
    | INSERT
    | DELETE
    | UPDATE
    | KEEP SEQUENCE
    | INDEX
    | REFERENCES
    | INHERIT PRIVILEGES
    | INHERIT REMOTE PRIVILEGES
    | TRANSLATE SQL
    | MERGE VIEW
    ;

systemPrivilege
    : systemAdministerPrivilege
    | systemAdvancedQueuingPrivilege
    | systemAdvisorFrameworkPrivilege
    | systemAlterAnyPrivilegesPrivilege
    | systemAlterPrivilegesPrivilege
    | systemAnalyticViewsPrivilege
    | systemAnalyzePrivilegesPrivilege
    | systemAssemblyPrivilegesPrivilege
    | systemAttributeDimensionsPrivilege
    | systemAuditPrivilegesPrivilege
    | systemBackupPrivilegesPrivilege
    | systemClustersPrivilege
    | systemCommentPrivilegesPrivilege
    | systemContainerDatabasePrivilege
    | systemContextsPrivilege
    | systemCreateAnyPrivilegesPrivilege
    | systemCreatePrivilegesPrivilege
    | systemDatabaseSystemPrivilege
    | systemDatabaseLinksPrivilege
    | systemDatastorePrivilege
    | systemDebugPrivilege
    | systemDeletePrivilege
    | systemDiagnosticsPrivilege
    | systemDimensionsPrivilege
    | systemDirectoriesPrivilege
    | systemDropAnyPrivilegesPrivilege
    | systemDropPrivilegesPrivilege
    | systemEditionsPrivilege
    | systemEnterpriseManagerPrivilege
    | systemEvaluationContextPrivilege
    | systemExecutePrivilegesPrivilege
    | systemExemptPrivilegesPrivilege
    | systemExportImportPrivilege
    | systemFineGrainedAccessControlPrivilege
    | systemFileGroupPrivilege
    | systemFlashbackPrivilege
    | systemForcePrivilege
    | systemGrantPrivilege
    | systemHierarchiesPrivilege
    | systemIndexesPrivilege
    | systemIndextypePrivilege
    | systemInheritPrivilege
    | systemInsertPrivilege
    | systemJobSchedulerPrivilege
    | systemLibrariesPrivilege
    | systemLocksPrivilege
    | systemLockdownProfilePrivilege
    | systemLogMiningPrivilege
    | systemLogicalPartitionTrackingPrivilege
    | systemMaterializedViewsPrivilege
    | systemMeasureFoldersPrivilege
    | systemMiningModelsPrivilege
    | systemMultiLingualEnginePrivilege
    | systemNotificationPrivilegePrivilege
    | systemOlapCubesPrivilege
    | systemOlapCubeBuildPrivilege
    | systemOlapCubeDimensionsPrivilege
    | systemOlapCubeMeasureFoldersPrivilege
    | systemOperatorPrivilege
    | systemOutlinesPrivilege
    | systemPlanManagementPrivilege
    | systemPoliciesPrivilege
    | systemProceduresPrivilege
    | systemProfilesPrivilege
    | systemQueryRewritePrivilege
    | systemReadAnyPrivilege
    | systemRealApplicationTestingPrivilege
    | systemRedactionPrivilege
    | systemResumablePrivilege
    | systemRolesPrivilege
    | systemRollbackSegmentPrivilege
    | systemSchedulerPrivilege
    | systemSelectPrivilege
    | systemSequencePrivilege
    | systemSessionPrivilege
    | systemSynonymPrivilege
    | systemSqlTranslationPrivilege
    | systemSystemPrivilegesPrivilege
    | systemTablesPrivilege
    | systemTablespacesPrivilege
    | systemTransactionsPrivilege
    | systemTriggersPrivilege
    | systemTypesPrivilege
    | systemUnderPrivilege
    | systemUpdatePrivilege
    | systemUserPrivilege
    | systemViewPrivilege
    | systemWritePrivilege
    ;

systemAdministerPrivilege
    : ADMINISTER ANY SQL TUNING SET
    | ADMINISTER DATABASE TRIGGER
    | ADMINISTER KEY MANAGEMENT
    | ADMINISTER RESOURCE MANAGER
    | ADMINISTER SQL MANAGEMENT OBJECT
    | ADMINISTER SQL TUNING SET
    | FLASHBACK ARCHIVE ADMINISTER
    | GRANT ANY OBJECT PRIVILEGE
    | GRANT ANY PRIVILEGE
    | GRANT ANY ROLE
    | MANAGE ANY FILE GROUP
    | MANAGE ANY QUEUE
    | MANAGE FILE GROUP
    | MANAGE SCHEDULER
    | MANAGE TABLESPACE
    ;

systemAdvancedQueuingPrivilege
    : DEQUEUE ANY QUEUE
    | ENQUEUE ANY QUEUE
    | MANAGE ANY QUEUE
    ;

systemAdvisorFrameworkPrivilege
    : ADMINISTER ANY SQL TUNING SET
    | ADMINISTER SQL MANAGEMENT OBJECT
    | ADMINISTER SQL TUNING SET
    | ADVISOR
    | ALTER ANY SQL PROFILE
    | CREATE ANY SQL PROFILE
    | DROP ANY SQL PROFILE
    ;

systemAlterAnyPrivilegesPrivilege
    : ALTER ANY ANALYTIC VIEW
    | ALTER ANY ASSEMBLY
    | ALTER ANY ATTRIBUTE DIMENSION
    | ALTER ANY CLUSTER
    | ALTER ANY CUBE
    | ALTER ANY CUBE BUILD PROCESS
    | ALTER ANY CUBE DIMENSION
    | ALTER ANY DIMENSION
    | ALTER ANY EDITION
    | ALTER ANY EVALUATION CONTEXT
    | ALTER ANY HIERARCHY
    | ALTER ANY INDEX
    | ALTER ANY INDEXTYPE
    | ALTER ANY LIBRARY
    | ALTER ANY MATERIALIZED VIEW
    | ALTER ANY MEASURE FOLDER
    | ALTER ANY MINING MODEL
    | ALTER ANY OPERATOR
    | ALTER ANY OUTLINE
    | ALTER ANY PROCEDURE
    | ALTER ANY ROLE
    | ALTER ANY RULE
    | ALTER ANY RULE SET
    | ALTER ANY SEQUENCE
    | ALTER ANY SQL PROFILE
    | ALTER ANY SQL TRANSLATION PROFILE
    | ALTER ANY TABLE
    | ALTER ANY TRIGGER
    | ALTER ANY TYPE
    ;

systemAlterPrivilegesPrivilege
    : ALTER DATABASE
    | ALTER DATABASE LINK
    | ALTER LOCKDOWN PROFILE
    | ALTER PROFILE
    | ALTER PUBLIC DATABASE LINK
    | ALTER RESOURCE COST
    | ALTER ROLLBACK SEGMENT
    | ALTER SESSION
    | ALTER SYSTEM
    | ALTER TABLESPACE
    | ALTER USER
    ;

systemAnalyticViewsPrivilege
    : ALTER ANY ANALYTIC VIEW
    | CREATE ANY ANALYTIC VIEW
    | CREATE ANALYTIC VIEW
    | DROP ANY ANALYTIC VIEW
    | READ ANY ANALYTIC VIEW CACHE
    | WRITE ANY ANALYTIC VIEW CACHE
    ;

systemAnalyzePrivilegesPrivilege
    : ANALYZE ANY
    | ANALYZE ANY DICTIONARY
    ;

systemAssemblyPrivilegesPrivilege
    : ALTER ANY ASSEMBLY
    | CREATE ANY ASSEMBLY
    | CREATE ASSEMBLY
    | DROP ANY ASSEMBLY
    | EXECUTE ANY ASSEMBLY
    | EXECUTE ASSEMBLY
    ;

systemAttributeDimensionsPrivilege
    : ALTER ANY ATTRIBUTE DIMENSION
    | CREATE ANY ATTRIBUTE DIMENSION
    | CREATE ATTRIBUTE DIMENSION
    | DROP ANY ATTRIBUTE DIMENSION
    ;

systemAuditPrivilegesPrivilege
    : AUDIT ANY
    | AUDIT SYSTEM
    ;

systemBackupPrivilegesPrivilege
    : BACKUP ANY TABLE
    ;

systemClustersPrivilege
    : ALTER ANY CLUSTER
    | CREATE ANY CLUSTER
    | CREATE CLUSTER
    | DROP ANY CLUSTER
    ;

systemCommentPrivilegesPrivilege
    : COMMENT ANY MINING MODEL
    | COMMENT ANY TABLE
    ;

systemContainerDatabasePrivilege
    : CREATE PLUGGABLE DATABASE
    | SET CONTAINER
    ;

systemContextsPrivilege
    : CREATE ANY CONTEXT
    | DROP ANY CONTEXT
    ;

systemCreateAnyPrivilegesPrivilege
    : CREATE ANY ANALYTIC VIEW
    | CREATE ANY ASSEMBLY
    | CREATE ANY ATTRIBUTE DIMENSION
    | CREATE ANY CLUSTER
    | CREATE ANY CONTEXT
    | CREATE ANY CREDENTIAL
    | CREATE ANY CUBE
    | CREATE ANY CUBE BUILD PROCESS
    | CREATE ANY CUBE DIMENSION
    | CREATE ANY DIMENSION
    | CREATE ANY DIRECTORY
    | CREATE ANY EDITION
    | CREATE ANY EVALUATION CONTEXT
    | CREATE ANY HIERARCHY
    | CREATE ANY INDEX
    | CREATE ANY INDEXTYPE
    | CREATE ANY JOB
    | CREATE ANY LIBRARY
    | CREATE ANY MATERIALIZED VIEW
    | CREATE ANY MEASURE FOLDER
    | CREATE ANY MINING MODEL
    | CREATE ANY OPERATOR
    | CREATE ANY OUTLINE
    | CREATE ANY PROCEDURE
    | CREATE ANY RULE
    | CREATE ANY RULE SET
    | CREATE ANY SEQUENCE
    | CREATE ANY SQL PROFILE
    | CREATE ANY SQL TRANSLATION PROFILE
    | CREATE ANY SYNONYM
    | CREATE ANY TABLE
    | CREATE ANY TRIGGER
    | CREATE ANY TYPE
    | CREATE ANY VIEW
    ;

systemCreatePrivilegesPrivilege
    : CREATE ANALYTIC VIEW
    | CREATE ASSEMBLY
    | CREATE ATTRIBUTE DIMENSION
    | CREATE CLUSTER
    | CREATE CREDENTIAL
    | CREATE CUBE
    | CREATE CUBE BUILD PROCESS
    | CREATE CUBE DIMENSION
    | CREATE DATABASE LINK
    | CREATE DIMENSION
    | CREATE EVALUATION CONTEXT
    | CREATE EXTERNAL JOB
    | CREATE HIERARCHY
    | CREATE INDEXTYPE
    | CREATE JOB
    | CREATE LIBRARY
    | CREATE LOCKDOWN PROFILE
    | CREATE LOGICAL PARTITION TRACKING
    | CREATE MATERIALIZED VIEW
    | CREATE MEASURE FOLDER
    | CREATE MINING MODEL
    | CREATE OPERATOR
    | CREATE PLUGGABLE DATABASE
    | CREATE PROCEDURE
    | CREATE PROFILE
    | CREATE PUBLIC DATABASE LINK
    | CREATE PUBLIC SYNONYM
    | CREATE ROLE
    | CREATE ROLLBACK SEGMENT
    | CREATE RULE
    | CREATE RULE SET
    | CREATE SEQUENCE
    | CREATE SESSION
    | CREATE SQL TRANSLATION PROFILE
    | CREATE SYNONYM
    | CREATE TABLE
    | CREATE TABLESPACE
    | CREATE TRIGGER
    | CREATE TYPE
    | CREATE USER
    | CREATE VIEW
    ;

systemDatabaseSystemPrivilege
    : ALTER DATABASE
    | ALTER SYSTEM
    | AUDIT SYSTEM
    | CREATE PLUGGABLE DATABASE
    ;

systemDatabaseLinksPrivilege
    : ALTER DATABASE LINK
    | ALTER PUBLIC DATABASE LINK
    | CREATE DATABASE LINK
    | CREATE PUBLIC DATABASE LINK
    | DROP PUBLIC DATABASE LINK
    ;

systemDatastorePrivilege
    : TEXT DATASTORE ACCESS
    ;

systemDebugPrivilege
    : DEBUG ANY PROCEDURE
    | DEBUG CONNECT ANY
    | DEBUG CONNECT SESSION
    ;

systemDeletePrivilege
    : DELETE ANY CUBE DIMENSION
    | DELETE ANY MEASURE FOLDER
    | DELETE ANY TABLE
    ;

systemDiagnosticsPrivilege
    : ENABLE DIAGNOSTICS
    ;

systemDimensionsPrivilege
    : ALTER ANY DIMENSION
    | CREATE ANY DIMENSION
    | CREATE DIMENSION
    | DROP ANY DIMENSION
    ;

systemDirectoriesPrivilege
    : CREATE ANY DIRECTORY
    | DROP ANY DIRECTORY
    ;

systemDropAnyPrivilegesPrivilege
    : DROP ANY ANALYTIC VIEW
    | DROP ANY ASSEMBLY
    | DROP ANY ATTRIBUTE DIMENSION
    | DROP ANY CLUSTER
    | DROP ANY CONTEXT
    | DROP ANY CUBE
    | DROP ANY CUBE BUILD PROCESS
    | DROP ANY CUBE DIMENSION
    | DROP ANY DIMENSION
    | DROP ANY DIRECTORY
    | DROP ANY EDITION
    | DROP ANY EVALUATION CONTEXT
    | DROP ANY HIERARCHY
    | DROP ANY INDEX
    | DROP ANY INDEXTYPE
    | DROP ANY LIBRARY
    | DROP ANY MATERIALIZED VIEW
    | DROP ANY MEASURE FOLDER
    | DROP ANY MINING MODEL
    | DROP ANY OPERATOR
    | DROP ANY OUTLINE
    | DROP ANY PROCEDURE
    | DROP ANY ROLE
    | DROP ANY RULE
    | DROP ANY RULE SET
    | DROP ANY SEQUENCE
    | DROP ANY SQL PROFILE
    | DROP ANY SQL TRANSLATION PROFILE
    | DROP ANY SYNONYM
    | DROP ANY TABLE
    | DROP ANY TRIGGER
    | DROP ANY TYPE
    | DROP ANY VIEW
    ;

systemDropPrivilegesPrivilege
    : DROP LOCKDOWN PROFILE
    | DROP LOGICAL PARTITION TRACKING
    | DROP PROFILE
    | DROP PUBLIC DATABASE LINK
    | DROP PUBLIC SYNONYM
    | DROP ROLLBACK SEGMENT
    | DROP TABLESPACE
    | DROP USER
    ;

systemEditionsPrivilege
    : ALTER ANY EDITION
    | CREATE ANY EDITION
    | DROP ANY EDITION
    ;

systemEnterpriseManagerPrivilege
    : EM EXPRESS CONNECT
    ;

systemEvaluationContextPrivilege
    : ALTER ANY EVALUATION CONTEXT
    | CREATE ANY EVALUATION CONTEXT
    | DROP ANY EVALUATION CONTEXT
    | EXECUTE ANY EVALUATION CONTEXT
    | CREATE EVALUATION CONTEXT
    ;

systemExecutePrivilegesPrivilege
    : EXECUTE ANY ASSEMBLY
    | EXECUTE ANY CLASS
    | EXECUTE ANY EVALUATION CONTEXT
    | EXECUTE ANY INDEXTYPE
    | EXECUTE ANY LIBRARY
    | EXECUTE ANY OPERATOR
    | EXECUTE ANY PROCEDURE
    | EXECUTE ANY PROGRAM
    | EXECUTE ANY RULE
    | EXECUTE ANY RULE SET
    | EXECUTE ANY TYPE
    | EXECUTE ASSEMBLY
    | EXECUTE DYNAMIC MLE
    ;

systemExemptPrivilegesPrivilege
    : EXEMPT ACCESS POLICY
    | EXEMPT IDENTITY POLICY
    | EXEMPT REDACTION POLICY
    ;

systemExportImportPrivilege
    : EXPORT FULL DATABASE
    | IMPORT FULL DATABASE
    ;

systemFineGrainedAccessControlPrivilege
    : EXEMPT ACCESS POLICY
    ;

systemFileGroupPrivilege
    : MANAGE ANY FILE GROUP
    | MANAGE FILE GROUP
    | READ ANY FILE GROUP
    ;

systemFlashbackPrivilege
    : FLASHBACK ANY TABLE
    | FLASHBACK ARCHIVE ADMINISTER
    | PURGE DBA_RECYCLEBIN
    ;

systemForcePrivilege
    : FORCE ANY TRANSACTION
    | FORCE TRANSACTION
    ;

systemGrantPrivilege
    : GRANT ANY OBJECT PRIVILEGE
    | GRANT ANY PRIVILEGE
    | GRANT ANY ROLE
    ;

systemHierarchiesPrivilege
    : ALTER ANY HIERARCHY
    | CREATE ANY HIERARCHY
    | CREATE HIERARCHY
    | DROP ANY HIERARCHY
    ;

systemIndexesPrivilege
    : ALTER ANY INDEX
    | CREATE ANY INDEX
    | DROP ANY INDEX
    ;

systemIndextypePrivilege
    : ALTER ANY INDEXTYPE
    | CREATE ANY INDEXTYPE
    | CREATE INDEXTYPE
    | DROP ANY INDEXTYPE
    | EXECUTE ANY INDEXTYPE
    ;

systemInheritPrivilege
    : INHERIT ANY PRIVILEGES
    | INHERIT ANY REMOTE PRIVILEGES
    ;

systemInsertPrivilege
    : INSERT ANY CUBE DIMENSION
    | INSERT ANY MEASURE FOLDER
    | INSERT ANY TABLE
    ;

systemJobSchedulerPrivilege
    : CREATE ANY JOB
    | CREATE EXTERNAL JOB
    | CREATE JOB
    | EXECUTE ANY CLASS
    | EXECUTE ANY PROGRAM
    | MANAGE SCHEDULER
    | USE ANY JOB RESOURCE
    ;

systemLibrariesPrivilege
    : ALTER ANY LIBRARY
    | CREATE ANY LIBRARY
    | CREATE LIBRARY
    | DROP ANY LIBRARY
    | EXECUTE ANY LIBRARY
    ;

systemLocksPrivilege
    : LOCK ANY TABLE
    ;

systemLockdownProfilePrivilege
    : ALTER LOCKDOWN PROFILE
    | CREATE LOCKDOWN PROFILE
    | DROP LOCKDOWN PROFILE
    ;

systemLogMiningPrivilege
    : LOGMINING
    ;

systemLogicalPartitionTrackingPrivilege
    : CREATE LOGICAL PARTITION TRACKING
    | DROP LOGICAL PARTITION TRACKING
    ;

systemMaterializedViewsPrivilege
    : ALTER ANY MATERIALIZED VIEW
    | CREATE ANY MATERIALIZED VIEW
    | CREATE MATERIALIZED VIEW
    | DROP ANY MATERIALIZED VIEW
    | FLASHBACK ANY TABLE
    | GLOBAL QUERY REWRITE
    | ON COMMIT REFRESH
    | QUERY REWRITE
    ;

systemMeasureFoldersPrivilege
    : ALTER ANY MEASURE FOLDER
    | CREATE ANY MEASURE FOLDER
    | CREATE MEASURE FOLDER
    | DELETE ANY MEASURE FOLDER
    | DROP ANY MEASURE FOLDER
    | INSERT ANY MEASURE FOLDER
    ;

systemMiningModelsPrivilege
    : ALTER ANY MINING MODEL
    | COMMENT ANY MINING MODEL
    | CREATE ANY MINING MODEL
    | CREATE MINING MODEL
    | DROP ANY MINING MODEL
    | SELECT ANY MINING MODEL
    ;

systemMultiLingualEnginePrivilege
    : EXECUTE DYNAMIC MLE
    ;

systemNotificationPrivilegePrivilege
    : CHANGE NOTIFICATION
    ;

systemOlapCubesPrivilege
    : ALTER ANY CUBE
    | CREATE ANY CUBE
    | CREATE CUBE
    | DROP ANY CUBE
    | SELECT ANY CUBE
    | UPDATE ANY CUBE
    ;

systemOlapCubeBuildPrivilege
    : ALTER ANY CUBE BUILD PROCESS
    | CREATE ANY CUBE BUILD PROCESS
    | CREATE CUBE BUILD PROCESS
    | DROP ANY CUBE BUILD PROCESS
    | UPDATE ANY CUBE BUILD PROCESS
    ;

systemOlapCubeDimensionsPrivilege
    : ALTER ANY CUBE DIMENSION
    | CREATE ANY CUBE DIMENSION
    | CREATE CUBE DIMENSION
    | DELETE ANY CUBE DIMENSION
    | DROP ANY CUBE DIMENSION
    | INSERT ANY CUBE DIMENSION
    | SELECT ANY CUBE DIMENSION
    | UPDATE ANY CUBE DIMENSION
    ;

systemOlapCubeMeasureFoldersPrivilege
    : CREATE ANY MEASURE FOLDER
    | CREATE MEASURE FOLDER
    | DELETE ANY MEASURE FOLDER
    | DROP ANY MEASURE FOLDER
    | INSERT ANY MEASURE FOLDER
    ;

systemOperatorPrivilege
    : ALTER ANY OPERATOR
    | CREATE ANY OPERATOR
    | CREATE OPERATOR
    | DROP ANY OPERATOR
    | EXECUTE ANY OPERATOR
    ;

systemOutlinesPrivilege
    : ALTER ANY OUTLINE
    | CREATE ANY OUTLINE
    | DROP ANY OUTLINE
    ;

systemPlanManagementPrivilege
    : ADMINISTER SQL MANAGEMENT OBJECT
    ;

systemPoliciesPrivilege
    : EXEMPT ACCESS POLICY
    | EXEMPT IDENTITY POLICY
    | EXEMPT REDACTION POLICY
    ;

systemProceduresPrivilege
    : ALTER ANY PROCEDURE
    | CREATE ANY PROCEDURE
    | CREATE PROCEDURE
    | DROP ANY PROCEDURE
    | EXECUTE ANY PROCEDURE
    | INHERIT ANY REMOTE PRIVILEGES
    ;

systemProfilesPrivilege
    : ALTER PROFILE
    | CREATE PROFILE
    | DROP PROFILE
    ;

systemQueryRewritePrivilege
    : GLOBAL QUERY REWRITE
    | QUERY REWRITE
    ;

systemReadAnyPrivilege
    : READ ANY ANALYTIC VIEW CACHE
    | READ ANY FILE GROUP
    | READ ANY TABLE
    ;

systemRealApplicationTestingPrivilege
    : KEEP DATE TIME
    | KEEP SYSGUID
    ;

systemRedactionPrivilege
    : EXEMPT REDACTION POLICY
    ;

systemResumablePrivilege
    : RESUMABLE
    ;

systemRolesPrivilege
    : ALTER ANY ROLE
    | CREATE ROLE
    | DROP ANY ROLE
    | GRANT ANY ROLE
    ;

systemRollbackSegmentPrivilege
    : ALTER ROLLBACK SEGMENT
    | CREATE ROLLBACK SEGMENT
    | DROP ROLLBACK SEGMENT
    ;

systemSchedulerPrivilege
    : MANAGE SCHEDULER
    ;

systemSelectPrivilege
    : SELECT ANY CUBE
    | SELECT ANY CUBE BUILD PROCESS
    | SELECT ANY CUBE DIMENSION
    | SELECT ANY DICTIONARY
    | SELECT ANY MEASURE FOLDER
    | SELECT ANY MINING MODEL
    | SELECT ANY SEQUENCE
    | SELECT ANY TABLE
    | SELECT ANY TRANSACTION
    ;

systemSequencePrivilege
    : ALTER ANY SEQUENCE
    | CREATE ANY SEQUENCE
    | CREATE SEQUENCE
    | DROP ANY SEQUENCE
    | SELECT ANY SEQUENCE
    ;

systemSessionPrivilege
    : ALTER RESOURCE COST
    | ALTER SESSION
    | CREATE SESSION
    | RESTRICTED SESSION
    ;

systemSynonymPrivilege
    : CREATE ANY SYNONYM
    | CREATE PUBLIC SYNONYM
    | CREATE SYNONYM
    | DROP ANY SYNONYM
    | DROP PUBLIC SYNONYM
    ;

systemSqlTranslationPrivilege
    : ALTER ANY SQL TRANSLATION PROFILE
    | CREATE ANY SQL TRANSLATION PROFILE
    | DROP ANY SQL TRANSLATION PROFILE
    | TRANSLATE ANY SQL
    | USE ANY SQL TRANSLATION PROFILE
    | CREATE SQL TRANSLATION PROFILE
    ;

systemSystemPrivilegesPrivilege
    : SYSBACKUP
    | SYSDBA
    | SYSDG
    | SYSKM
    | SYSOPER
    | SYSRAC
    ;

systemTablesPrivilege
    : ALTER ANY TABLE
    | BACKUP ANY TABLE
    | COMMENT ANY TABLE
    | CREATE ANY TABLE
    | CREATE TABLE
    | DELETE ANY TABLE
    | DROP ANY TABLE
    | FLASHBACK ANY TABLE
    | INSERT ANY TABLE
    | LOCK ANY TABLE
    | READ ANY TABLE
    | REDEFINE ANY TABLE
    | SELECT ANY TABLE
    | UNDER ANY TABLE
    | UPDATE ANY TABLE
    ;

systemTablespacesPrivilege
    : ALTER TABLESPACE
    | CREATE TABLESPACE
    | DROP TABLESPACE
    | MANAGE TABLESPACE
    | UNLIMITED TABLESPACE
    ;

systemTransactionsPrivilege
    : FORCE ANY TRANSACTION
    | FORCE TRANSACTION
    ;

systemTriggersPrivilege
    : ADMINISTER DATABASE TRIGGER
    | ALTER ANY TRIGGER
    | CREATE ANY TRIGGER
    | CREATE TRIGGER
    | DROP ANY TRIGGER
    ;

systemTypesPrivilege
    : ALTER ANY TYPE
    | CREATE ANY TYPE
    | CREATE TYPE
    | DROP ANY TYPE
    | EXECUTE ANY TYPE
    | UNDER ANY TYPE
    ;

systemUnderPrivilege
    : UNDER ANY TABLE
    | UNDER ANY TYPE
    | UNDER ANY VIEW
    ;

systemUpdatePrivilege
    : UPDATE ANY CUBE
    | UPDATE ANY CUBE BUILD PROCESS
    | UPDATE ANY CUBE DIMENSION
    | UPDATE ANY TABLE
    ;

systemUserPrivilege
    : ALTER USER
    | BECOME USER
    | CREATE USER
    | DROP USER
    ;

systemViewPrivilege
    : CREATE ANY VIEW
    | CREATE VIEW
    | DROP ANY VIEW
    | FLASHBACK ANY TABLE
    | MERGE ANY VIEW
    | UNDER ANY VIEW
    ;

systemWritePrivilege
    : WRITE ANY ANALYTIC VIEW CACHE
    ;

role
    : ACCHK_READ
    | identifier
    | CONNECT
    ;

objectAction
    : AUDIT
    | GRANT
    | READ
    | EXECUTE
    | ALTER
    | COMMENT
    | DELETE
    | INDEX
    | INSERT
    | LOCK
    | SELECT
    | UPDATE
    | RENAME
    | FLASHBACK
    ;

// TODO: impl
systemAction
    : CREATE TABLE
    | INSERT
    | SELECT
    | CREATE CLUSTER
    | ALTER CLUSTER
    | UPDATE
    | DELETE
    | DROP CLUSTER
    | CREATE INDEX
    | DROP INDEX
    | ALTER INDEX
    | DROP TABLE
    | CREATE SEQUENCE
    | ALTER SEQUENCE
    | ALTER TABLE
    | DROP SEQUENCE
    | CREATE SYNONYM
    | DROP SYNONYM
    | CREATE VIEW
    | DROP VIEW
    | CREATE PROCEDURE
    | ALTER PROCEDURE
    | LOCK TABLE
    | RENAME
    | COMMENT
    | CREATE DATABASE LINK
    | DROP DATABASE LINK
    | ALTER DATABASE
    | CREATE ROLLBACK SEGMENT
    | ALTER ROLLBACK SEGMENT
    | DROP ROLLBACK SEGMENT
    | CREATE TABLESPACE
    | ALTER TABLESPACE
    | DROP TABLESPACE
    | ALTER SESSION
    | ALTER USER
    | COMMIT
    | ROLLBACK
    | SET TRANSACTION
    | ALTER SYSTEM
    | CREATE USER
    | CREATE ROLE
    | DROP USER
    | DROP ROLE
    | SET ROLE
    | CREATE SCHEMA
    | ALTER TRACING
    | CREATE TRIGGER
    | ALTER TRIGGER
    | DROP TRIGGER
    | ANALYZE TABLE
    | ANALYZE INDEX
    | ANALYZE CLUSTER
    | CREATE PROFILE
    | DROP PROFILE
    | ALTER PROFILE
    | DROP PROCEDURE
    | ALTER RESOURCE COST
    | CREATE MATERIALIZED VIEW LOG
    | ALTER MATERIALIZED VIEW LOG
    | DROP MATERIALIZED VIEW  LOG
    | CREATE MATERIALIZED VIEW 
    | ALTER MATERIALIZED VIEW 
    | DROP MATERIALIZED VIEW 
    | CREATE TYPE
    | DROP TYPE
    | ALTER ROLE
    | ALTER TYPE
    | CREATE TYPE BODY
    | ALTER TYPE BODY
    | DROP TYPE BODY
    | DROP LIBRARY
    | TRUNCATE TABLE
    | TRUNCATE CLUSTER
    | ALTER VIEW
    | CREATE FUNCTION
    | ALTER FUNCTION
    | DROP FUNCTION
    | CREATE PACKAGE
    | ALTER PACKAGE
    | DROP PACKAGE
    | CREATE PACKAGE BODY
    | ALTER PACKAGE BODY
    | DROP PACKAGE BODY
    | ALTER MINING MODEL
    | CREATE MINING MODEL
    | CREATE DIRECTORY
    | DROP DIRECTORY
    | CREATE LIBRARY
    | CREATE JAVA
    | ALTER JAVA
    | DROP JAVA
    | CREATE OPERATOR
    | CREATE INDEXTYPE
    | DROP INDEXTYPE
    | ALTER INDEXTYPE
    | DROP OPERATOR
    | ASSOCIATE STATISTICS
    | DISASSOCIATE STATISTICS
    | CREATE DIMENSION
    | ALTER DIMENSION
    | DROP DIMENSION
    | CREATE CONTEXT
    | DROP CONTEXT
    | ALTER OUTLINE
    | CREATE OUTLINE
    | DROP OUTLINE
    | ALTER OPERATOR
    | CREATE SPFILE
    | CREATE PFILE
    | CHANGE PASSWORD
    | ALTER SYNONYM
    | ALTER DISK GROUP
    | CREATE DISK GROUP
    | DROP DISK GROUP
    | ALTER LIBRARY
    | PURGE RECYCLEBIN
    | PURGE TABLESPACE
    | PURGE TABLE
    | PURGE INDEX
    | FLASHBACK TABLE
    | CREATE RESTORE POINT
    | DROP RESTORE POINT
    | CREATE EDITION
    | DROP EDITION
    | DROP ASSEMBLY
    | CREATE ASSEMBLY
    | ALTER ASSEMBLY
    | CREATE FLASHBACK ARCHIVE
    | ALTER FLASHBACK ARCHIVE
    | DROP FLASHBACK ARCHIVE
    | CREATE SCHEMA SYNONYM
    | DROP SCHEMA SYNONYM
    | ALTER DATABASE LINK
    | CREATE PLUGGABLE DATABASE
    | ALTER PLUGGABLE DATABASE
    | DROP PLUGGABLE DATABASE
    | CREATE AUDIT POLICY
    | ALTER AUDIT POLICY
    | DROP AUDIT POLICY
    | CREATE LOCKDOWN PROFILE
    | DROP LOCKDOWN PROFILE
    | ALTER LOCKDOWN PROFILE
    | ADMINISTER KEY MANAGEMENT
    | CREATE MATERIALIZED ZONEMAP
    | ALTER MATERIALIZED ZONEMAP
    | DROP MATERIALIZED ZONEMAP
    | DROP MINING MODEL
    | CREATE ATTRIBUTE DIMENSION
    | ALTER ATTRIBUTE DIMENSION
    | DROP ATTRIBUTE DIMENSION
    | CREATE HIERARCHY
    | ALTER HIERARCHY
    | DROP HIERARCHY
    | CREATE ANALYTIC VIEW
    | ALTER ANALYTIC VIEW
    | DROP ANALYTIC VIEW
    | ALTER DATABASE DICTIONARY
    | CREATE INMEMORY JOIN GROUP
    | ALTER INMEMORY JOIN GROUP
    | DROP INMEMORY JOIN GROUP
    | GRANT
    | REVOKE
    | AUDIT
    | NOAUDIT
    | LOGON
    | LOGOFF
    | EXECUTE
    | EXPLAIN PLAN
    | CALL
    | PURGE DBA_RECYCLEBIN
    | ALL
    | READ DIRECTORY
    | WRITE DIRECTORY
    | EXECUTE DIRECTORY
    ;

componentAction
    : EXPORT
    | IMPORT
    | ALL
    ;

createDatabaseOption
    : USER SYS IDENTIFIED BY password                           #createDatabaseSysPasswordOption
    | USER SYSTEM IDENTIFIED BY password                        #createDatabaseSystemPasswordOption
    | CONTROLFILE REUSE                                         #createDatabaseControlFileReuseOption
    | MAXDATAFILES integer                                      #createDatabaseMaxDataFilesOption
    | MAXINSTANCES integer                                      #createDatabaseMaxInstantcesOption
    | CHARACTER SET charset=identifier                          #createDatabaseCharacterSetOption
    | NATIONAL CHARACTER SET charset=identifier                 #createDatabaseNationalCharacterSetOption
    | SET DEFAULT (BIGFILE | SMALLFILE) TABLESPACE              #createDatabaseSetDefaultTablespaceOption
    | databaseLoggingClauses                                    #createDatabaseDatabaseLoggingClausesOption
    | tablespaceClauses                                         #createDatabaseTablespaceClausesOption
    | setTimeZoneClause                                         #createDatabaseSetTimeZoneClauseOption
    | (BIGFILE | SMALLFILE)? USER_DATA TABLESPACE
      tablespace
      DATAFILE datafileTempfileSpec (',' datafileTempfileSpec)* #createDatabaseDataFileOption
    | enablePluggableDatabase                                   #createDatabaseEnablePluggableDatabaseOption
    ;

databaseLoggingClauses
    : LOGFILE databaseLoggingLogFileClause (',' databaseLoggingLogFileClause)*
    | MAXLOGFILES integer
    | MAXLOGMEMBERS integer
    | MAXLOGHISTORY integer
    | (ARCHIVELOG|NOARCHIVELOG)
    | FORCE LOGGING
    | SET STANDBY NOLOGGING FOR (DATA AVAILABILITY | LOAD PERFORMANCE)
    ;

tablespaceClauses
    : EXTENT MANAGEMENT LOCAL
    | DATAFILE fileSpecification (',' fileSpecification)*
    | SYSAUX DATAFILE fileSpecification (',' fileSpecification)*
    | defaultTablespace
    | defaultTempTablespace
    | undoTablespace
    ;

defaultTablespace
    : DEFAULT TABLESPACE tablespace (DATAFILE datafileTempfileSpec)? extentManagementClause?
    ;

extentManagementClause
    : EXTENT MANAGEMENT LOCAL
      (AUTOALLOCATE
      |UNIFORM (SIZE sizeClause)
      )?
    ;

defaultTempTablespace
    : ( BIGFILE | SMALLFILE )?
      DEFAULT
      ( TEMPORARY TABLESPACE
      | LOCAL TEMPORARY TABLESPACE FOR ( ALL | LEAF )
      ) tablespace
      ( TEMPFILE fileSpecification ( ',' fileSpecification )*)?
      extentManagementClause?
    ;

undoTablespace
    : (BIGFILE|SMALLFILE)?
      UNDO TABLESPACE tablespace (DATAFILE fileSpecification (',' fileSpecification)*)?
    ;

sizeClause
    : integer unit=(KW_K | KW_M | KW_G | KW_T | KW_P | KW_E)?
    ;

databaseLoggingLogFileClause
    : (GROUP integer)? fileSpecification
    ;

setTimeZoneClause
    : SET TIME_ZONE '=' stringLiteral
    ;

enablePluggableDatabase
    : ENABLE PLUGGABLE DATABASE (SEED fileNameConvert? (SYSTEM tablespaceDatafileClauses)? (SYSAUX tablespaceDatafileClauses)?)
    ;

fileNameConvert
    : FILE_NAME_CONVERT '='
      ( '(' TK_SINGLE_QUOTED_STRING
          ( ',' TK_SINGLE_QUOTED_STRING )*
        ')'
      | NONE
      )
    ;

fileNameConvertItem
    : filenamePattern ',' replacementFilenamePattern=stringLiteral
    ;

tablespaceDatafileClauses
    : DATAFILES (SIZE sizeClause|autoextendClause)+
    ;

autoextendClause
    : AUTOEXTEND ( OFF 
                 | ON ( NEXT sizeClause )? maxsizeClause?
                 )
    ;

maxsizeClause
    : MAXSIZE ( UNLIMITED | sizeClause )
    ;

fileSpecification
    : datafileTempfileSpec
    | redoLogFileSpec
    ;

datafileTempfileSpec
    : stringLiteral?
      ( SIZE sizeClause )?
      REUSE?
      autoextendClause?
    ;

redoLogFileSpec
    : ( stringLiteral
      | '(' stringLiteral ( ',' stringLiteral )* ')'
      )?
      ( SIZE sizeClause )?
      ( BLOCKSIZE sizeClause )?
      REUSE?
    ;

// TODO: The auditCondition can have a maximum length of 4000 characters. It can contain expressions, as well as the following functions and conditions:
// Numeric functions: BITAND, CEIL, FLOOR, POWER
// Character functions returning character values: CONCAT, LOWER, UPPER
// Character functions returning number values: INSTR, LENGTH
// Environment and identifier functions: SYS_CONTEXT, UID
// Comparison conditions: =, !=, <>, <, >, <=, >=
// Logical conditions: AND, OR
// Null conditions: IS [NOT] NULL
// [NOT] BETWEEN condition
// [NOT] IN condition

auditCondition
    : expr
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

calcMeasureClause
    : AS '(' calcMeasExpression ')'
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
    : (alias '.')? column '=' (alias '.')? column
    ;

attributesClause
    : ATTRIBUTES '(' attrDimAttributeClause (',' attrDimAttributeClause)* ')'
    ;

attrDimAttributeClause
    : (alias '.')? column (AS? attributeName=identifier)? classificationClause*
    ;

attrDimLevelClause
    : LEVEL level
      (NOT NULL | KW_SKIP WHEN NULL)?
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

grantSystemPrivileges
    : grantSystemPrivilegesRoleItem (',' grantSystemPrivilegesRoleItem)* TO (granteeClause | granteeIdentifiedBy)
      (WITH (ADMIN | DELEGATE) OPTION)?
    ;

grantObjectPrivileges
    : grantObjectPrivilegesItem (',' grantObjectPrivilegesItem)* onObjectClause
      TO granteeClause (WITH HIERARCHY OPTION)? (WITH GRANT OPTION)?
    ;

grantObjectPrivilegesItem
    : ( objectPrivilege | ALL PRIVILEGES?) ('(' column (',' column)* ')')?
    ;

onObjectClause
    : ON ( (schema '.')? object
         | USER user (',' user)*
         | DIRECTORY directoryName
         | EDITION editionName
         | MINING MODEL (schema '.')? miningModelName=identifier
         | JAVA (SOURCE | RESOURCE) (schema '.')? object
         | SQL TRANSLATION PROFILE (schema '.')? profile
         )
    ;

grantRolesToPrograms
    : role (',' role)* TO programUnit (',' programUnit)*
    ;

programUnit
    : FUNCTION (schema '.')? functionName
    | PROCEDURE (schema '.')? procedureName
    | PACKAGE (schema '.')? packageName
    ;

grantSystemPrivilegesRoleItem
    : systemPrivilege
    | role
    | ALL PRIVILEGES
    ;

granteeClause
    : granteeClauseItem (',' granteeClauseItem)*
    ;

granteeIdentifiedBy
    : user (',' user)* IDENTIFIED BY password (',' password)*
    ;

granteeClauseItem
    : user
    | role
    | PUBLIC
    ;

subquery
    : queryBlock orderByClause? rowOffset? rowFetchOption?
    | subquery ((UNION ALL? | INTERSECT | MINUS | EXCEPT) subquery)+ orderByClause? rowOffset? rowFetchOption?
    | '(' subquery ')' orderByClause? rowOffset? rowFetchOption?
    ;

orderByClause
    : ORDER SIBLINGS? BY items+=orderByItem (',' items+=orderByItem)*
    ;

orderByItem
    : (expr | position=integer) (ASC | DESC)? (NULLS FIRST | NULLS LAST)?
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
      ('(' errorLoggingSimpleExpression ')')?
      (REJECT LIMIT (integer | UNLIMITED))?
    ;


rowOffset
    : OFFSET offset=expr (ROW | ROWS)
    ;

rowFetchOption
    : FETCH (FIRST | NEXT) (rowcount=expr | percent=expr PERCENT)? (ROW | ROWS) (ONLY | WITH TIES)
    ;

forUpdateClause
    : FOR UPDATE (OF fullColumnPath (',' fullColumnPath)*)? (NOWAIT | WAIT TK_INTEGER_WITHOUT_SIGN | KW_SKIP LOCKED)?
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
      plsqlDeclarations?
      clauses+=factoringClause (',' clauses+=factoringClause)*
    ;

plsqlDeclarations
    : plsqlDeclaration+
    ;

plsqlDeclaration
    : functionDeclaration
    | functionDefinition
    | procedureDeclaration
    | procedureDefinition
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
      TO cycleValue=stringLiteral
      DEFAULT noCycleValue=stringLiteral
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
    | CONTAINERS '(' DEFAULT_PDB_HINT '=' stringLiteral ')'                         #containersHint
    | CURSOR_SHARING_EXACT                                                          #cursorSharingExactHint
    | DISABLE_PARALLEL_DML                                                          #disableParallelDmlHint
    | DRIVING_SITE '(' ( '@' queryBlock )? tablespec ')'                            #drivingSiteHint
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
    | PQ_DISTRIBUTE
      '(' hintQueryBlockName? tablespec
      (',' distribution | outerDistribution ',' innerDistribution) ')'              #pqDistributeHint
    | PQ_FILTER '(' (SERIAL | NONE | HASH | RANDOM) ')'                             #pqFilterHint
    | PQ_SKEW '(' hintQueryBlockName? tablespec ')'                                 #pqSkewHint
    | PUSH_PRED
      ('(' hintQueryBlockName ')' | '(' hintQueryBlockName? tablespec ')')?         #pushPredHint
    | PUSH_SUBQ ('(' hintQueryBlockName ')')?                                       #pushSubqHint
    | PX_JOIN_FILTER '(' tablespec ')'                                              #pxJoinFilterHint
    | QB_NAME '(' identifier ')'                                                    #qbNameHint
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

outerDistribution
    : NONE
    | BROADCAST
    | HASH
    | PARTITION
    ;

innerDistribution
    : NONE
    | BROADCAST
    | HASH
    | PARTITION
    ;

distribution
    : NONE
    | PARTITION
    | RANDOM
    | RANDOM_LOCAL
    ;

hintQueryBlockName
    : '@' identifier
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
    : tableReference ( innerCrossJoinClause | outerJoinClause | crossOuterApplyClause)*
    | '(' tableSource ')'
    | inlineAnalyticView
    ;

tableReference
    : (
       (
           ( queryTableExpression | ONLY '(' queryTableExpression ')' )
           flashbackQueryClause?
           (pivotClause | unpivotClause | rowPatternClause)?
       )
      | containersClause
      | shardsClause
      )
      tAlias?
    | tablePrimary (AS? tAlias)?
    ;

tablePrimary
    : jsonTableFunction
    | xmlTableFunction
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
    : ( schema '.' )? ( table ( modifiedExternalTable | partitionExtensionClause | '@' dblink )?
                      | ( view | materializedView ) ( '@' dblink )?
                      | hierarchyName
                      | analyticViewName=identifier ( HIERARCHIES '(' ( ( attributeDimension '.' )? hierarchyName ( ',' ( attributeDimension '.' )? hierarchyName )* )? ')' )?
                      | inlineExternalTable
                      )
                      sampleClause?
    | LATERAL? '(' subquery subqueryRestrictionClause? ')'
    | tableCollectionExpression
    // returning table function
    | functionExpression
    ;

modifiedExternalTable
    : EXTERNAL MODIFY '(' modifyExternalTableProperties ')'
    ;

modifyExternalTableProperties
    : ( DEFAULT DIRECTORY directoryName )?
      ( LOCATION ( '(' ( directoryName ':' )? TK_SINGLE_QUOTED_STRING ( ',' ( directoryName ':' )? TK_SINGLE_QUOTED_STRING )* ')'
                 | ( directoryName ':' )? TK_SINGLE_QUOTED_STRING
                 )
      )?
      ( ACCESS PARAMETERS ( BADFILE | LOGFILE | DISCARDFILE ) filename )?
      ( REJECT LIMIT ( integer | UNLIMITED ) )?
    ;

inlineExternalTable
    : EXTERNAL '(' '(' columnDefinition ( ',' columnDefinition )* ')' inlineExternalTableProperties ')'
    ;

inlineExternalTableProperties
    : ( TYPE accessDriverType )? externalTableDataProps ( REJECT LIMIT ( integer | UNLIMITED ))?
    ;

flashbackQueryClause
    : VERSIONS
        (BETWEEN (SCN | TIMESTAMP) (expr | MINVALUE) AND (expr | MAXVALUE)
        | PERIOD FOR validTimeColumn BETWEEN (expr | MINVALUE) AND (expr | MAXVALUE))
    | AS OF
        ((SCN | TIMESTAMP) expr
        | AS OF PERIOD FOR validTimeColumn expr)
    ;

addDiskClause
    : ADD ( ( SITE sitename=identifier )? (QUORUM | REGULAR)? (FAILGROUP failgroupName)? DISK qualifiedDiskClause (',' qualifiedDiskClause)*)+
    ;

qualifiedDiskClause
    : searchString=stringLiteral (NAME diskName=identifier)? (SIZE sizeClause)? (FORCE | NOFORCE)?
    ;

dropDiskClause
    : DROP ((QUORUM | REGULAR)? DISK diskName=identifier (FORCE | NOFORCE)? (',' diskName=identifier (FORCE | NOFORCE)?)* 
           | DISKS IN (QUORUM | REGULAR)? FAILGROUP failgroupName (FORCE | NOFORCE)? (',' failgroupName (FORCE | NOFORCE)?)*)
    ;

resizeDiskClause
    : RESIZE ALL (SIZE sizeClause)?
    ;

replaceDiskClause
    : REPLACE DISK diskName=identifier WITH stringLiteral (FORCE | NOFORCE)? (',' diskName=identifier WITH stringLiteral (FORCE | NOFORCE)?)* (POWER integer)? (WAIT | NOWAIT)?
    ;

renameDiskClause
    : RENAME ( DISK oldDiskName=identifier TO newDiskName=identifier (',' oldDiskName=identifier TO newDiskName=identifier)* 
             | DISKS ALL)
    ;

diskOnlineClause
    : ONLINE ( ( (QUORUM | REGULAR)? DISK diskName=identifier (',' diskName=identifier)* 
               | DISKS IN (QUORUM | REGULAR)? FAILGROUP failgroupName (',' failgroupName)*
               )*
             | ALL)
      (POWER integer)? (WAIT | NOWAIT)?
    ;

diskOfflineClause
    : OFFLINE ( (QUORUM | REGULAR)? DISK diskName=identifier (',' diskName=identifier)* 
              | DISKS IN (QUORUM | REGULAR)? FAILGROUP failgroupName (',' failgroupName)*)*
      timeoutClause?
    ;

timeoutClause
    : DROP AFTER integer (KW_M | KW_H)
    ;

rebalanceDiskgroupClause
    : REBALANCE ( ((WITH | WITHOUT) phase=identifier (',' phase=identifier)*)? (POWER integer)? (WAIT | NOWAIT)?
                | MODIFY POWER integer?
                )
    ;

checkDiskgroupClause
    : CHECK ALL? (REPAIR | NOREPAIR)?
    ;

diskgroupTemplateClauses
    : ((ADD | MODIFY) TEMPLATE templateName=identifier qualifiedTemplateClause (',' templateName=identifier qualifiedTemplateClause)*
    | DROP TEMPLATE templateName=identifier (',' templateName=identifier)*)
    ;

qualifiedTemplateClause
    : ATTRIBUTES '(' redundancyClause? stripingClause? ')'
    ;

redundancyClause
    : MIRROR 
    | HIGH 
    | UNPROTECTED 
    | PARITY 
    | DOUBLE
    ;

stripingClause
    : FINE 
    | COARSE
    ;

diskgroupDirectoryClauses
    : ADD DIRECTORY stringLiteral (',' stringLiteral)* 
    | DROP DIRECTORY stringLiteral (FORCE | NOFORCE)? (',' stringLiteral (FORCE | NOFORCE)?)*
    | RENAME DIRECTORY stringLiteral TO stringLiteral (',' stringLiteral TO stringLiteral)*
    ;

diskgroupAliasClauses
    : ADD ALIAS stringLiteral FOR stringLiteral (',' stringLiteral FOR stringLiteral)* 
    | DROP ALIAS stringLiteral (',' stringLiteral)* 
    | RENAME ALIAS stringLiteral TO stringLiteral (',' stringLiteral TO stringLiteral)*
    ;

diskgroupVolumeClauses
    : addVolumeClause
    | modifyVolumeClause 
    | RESIZE VOLUME asmVolume=identifier SIZE sizeClause 
    | DROP VOLUME asmVolume=identifier
    ;

addVolumeClause
    : ADD VOLUME asmVolume=identifier SIZE sizeClause redundancyClause?
      (STRIPE_WIDTH integer (KW_K  | KW_M))?
      (STRIPE_COLUMNS integer)?
    ;

modifyVolumeClause
    : MODIFY VOLUME asmVolume=identifier
      (MOUNTPATH stringLiteral)?
      (USAGE stringLiteral)?
    ;

diskgroupAttributes
    : SET ATTRIBUTE stringLiteral '=' stringLiteral
    ;

dropDiskgroupFileClause
    : DROP FILE stringLiteral (',' stringLiteral)*
    ;

convertRedundancyClause
    : CONVERT REDUNDANCY TO FLEX
    ;

usergroupClauses
    : ADD USERGROUP stringLiteral WITH MEMBER stringLiteral (',' stringLiteral)* 
    | MODIFY USERGROUP stringLiteral (ADD | DROP) MEMBER stringLiteral (',' stringLiteral)* 
    | DROP USERGROUP stringLiteral
    ;

userClauses
    : ADD USER stringLiteral (',' stringLiteral)* 
    | DROP USER stringLiteral (',' stringLiteral)* CASCADE? 
    | REPLACE USER stringLiteral WITH stringLiteral (',' stringLiteral WITH stringLiteral)*
    ;

filePermissionsClause
    : SET PERMISSION (OWNER | GROUP | OTHER) '=' (NONE | READ ONLY | READ WRITE)
                     (',' (OWNER | GROUP | OTHER | ALL) '=' (NONE | READ ONLY | READ WRITE))*
      FOR FILE stringLiteral (',' stringLiteral)*
    ;

fileOwnerClause
    : SET OWNERSHIP ( OWNER '=' stringLiteral 
                    | GROUP '=' stringLiteral
                    )
                    (',' ( OWNER '=' stringLiteral
                         | GROUP '=' stringLiteral)
                    )*
      FOR FILE stringLiteral (',' stringLiteral)*
    ;

scrubClause
    : SCRUB (FILE stringLiteral | DISK diskName=identifier)? (REPAIR | NOREPAIR)? (POWER (AUTO | LOW | HIGH | MAX))? (WAIT | NOWAIT)? (FORCE | NOFORCE)? STOP?
    ;

quotagroupClauses
    : ADD QUOTAGROUP quotagroupName=identifier (SET propertyName=identifier '=' propertyValue=identifier)? 
    | MODIFY QUOTAGROUP quotagroupName=identifier SET propertyName=identifier '=' propertyValue=identifier
    | MOVE FILEGROUP filegroupName=identifier TO quotagroupName=identifier
    | DROP QUOTAGROUP quotagroupName=identifier
    ;

filegroupClauses
    : addFilegroupClause 
    | modifyFilegroupClause 
    | moveToFilegroupClause 
    | dropFilegroupClause
    ;

addFilegroupClause
    : ADD FILEGROUP filegroupName=identifier ( DATABASE databaseName=identifier 
                                             | CLUSTER clusterName=identifier 
                                             | VOLUME asmVolume=identifier 
                                             | TEMPLATE (FROM TEMPLATE templateName=identifier)?)
      (SET stringLiteral '=' stringLiteral)?
    ;

modifyFilegroupClause
    : MODIFY FILEGROUP filegroupName=identifier SET stringLiteral '=' stringLiteral
    ;

moveToFilegroupClause
    : MOVE FILE stringLiteral TO FILEGROUP filegroupName=identifier
    ;

dropFilegroupClause
    : DROP FILEGROUP filegroupName=identifier CASCADE?
    ;

undropDiskClause
    : UNDROP DISKS
    ;

diskgroupAvailability
    : (MOUNT (RESTRICTED | NORMAL)? (FORCE | NOFORCE)? | DISMOUNT (FORCE | NOFORCE)?)
    ;

enableDisableVolume
    : (ENABLE | DISABLE) VOLUME (asmVolume=identifier (',' asmVolume=identifier)* | ALL)
    ;

keystoreManagementClauses
    : createKeystore
    | openKeystore
    | closeKeystore
    | backupKeystore
    | alterKeystorePassword
    | mergeIntoNewKeystore
    | mergeIntoExistingKeystore
    | isolateKeystore
    | uniteKeystore
    ;

createKeystore
    : CREATE ( KEYSTORE stringLiteral
             | LOCAL? AUTO_LOGIN KEYSTORE FROM KEYSTORE stringLiteral
             )
      IDENTIFIED BY keystorePassword
    ;

openKeystore
    : SET KEYSTORE OPEN (FORCE KEYSTORE)?
      IDENTIFIED BY (EXTERNAL STORE | keystorePassword)
      (CONTAINER '=' (ALL | CURRENT))?
    ;

closeKeystore
    : SET KEYSTORE CLOSE (IDENTIFIED BY (EXTERNAL STORE | keystorePassword))? (CONTAINER '=' (ALL | CURRENT))?
    ;

backupKeystore
    : BACKUP KEYSTORE (USING stringLiteral)? (FORCE KEYSTORE)? IDENTIFIED BY 
      (EXTERNAL STORE | keystorePassword)
      (TO stringLiteral)?
    ;

alterKeystorePassword
    : ALTER KEYSTORE PASSWORD (FORCE KEYSTORE)?
      IDENTIFIED BY oldKeystorePassword=identifier SET newKeystorePassword=identifier
      (WITH BACKUP (USING stringLiteral)?)?
    ;

mergeIntoNewKeystore
    : MERGE KEYSTORE stringLiteral (IDENTIFIED BY keystore1Password=identifier)?
      AND KEYSTORE stringLiteral (IDENTIFIED BY keystore2Password=identifier)?
      INTO NEW KEYSTORE stringLiteral IDENTIFIED BY keystore3Password=identifier
    ;

mergeIntoExistingKeystore
    : MERGE KEYSTORE stringLiteral (IDENTIFIED BY keystore1Password=identifier)?
      INTO EXISTING KEYSTORE stringLiteral IDENTIFIED BY keystore2Password=identifier
      (WITH BACKUP (USING stringLiteral)?)?
    ;

isolateKeystore
    : FORCE? ISOLATE KEYSTORE IDENTIFIED BY isolatedKeystorePassword=identifier FROM ROOT KEYSTORE
      (FORCE KEYSTORE)? IDENTIFIED BY (EXTERNAL STORE | unitedKeystorePassword=identifier)
      (WITH BACKUP (USING stringLiteral)?)?
    ;

uniteKeystore
    : UNITE KEYSTORE IDENTIFIED BY isolatedKeystorePassword=identifier WITH ROOT KEYSTORE
      (FORCE KEYSTORE)? IDENTIFIED BY (EXTERNAL STORE | unitedKeystorePassword=identifier)
      (WITH BACKUP (USING stringLiteral)?)? 
    ;

keyManagementClauses
    : setKey
    | createKey
    | useKey
    | setKeyTag
    | exportKeys
    | importKeys
    | migrateKey
    | reverseMigrateKey
    | moveKeys
    ;

setKey
    : SET ENCRYPTION? KEY stringLiteral? (USING TAG stringLiteral)?
      (USING ALGORITHM stringLiteral)? (FORCE KEYSTORE)?
      IDENTIFIED BY (EXTERNAL STORE | keyStorePassword=identifier)
      (WITH BACKUP (USING stringLiteral)?)?
      (CONTAINER '=' (ALL | CURRENT))?
    ;

createKey
    : CREATE ENCRYPTION? KEY (USING TAG stringLiteral)?
      (USING ALGORITHM stringLiteral)?
      (FORCE KEYSTORE)?
      IDENTIFIED BY (EXTERNAL STORE | keystorePassword)
      (WITH BACKUP (USING stringLiteral)?)?
      (CONTAINER '=' (ALL | CURRENT))?
    ;

useKey
    : USE ENCRYPTION? KEY stringLiteral (USING TAG stringLiteral)?
      (FORCE KEYSTORE)?
      IDENTIFIED BY (EXTERNAL STORE | keystorePassword)
      (WITH BACKUP (USING stringLiteral)?)?
    ;

setKeyTag
    : SET TAG stringLiteral FOR stringLiteral
      (FORCE KEYSTORE)?
      IDENTIFIED BY (EXTERNAL STORE | keystorePassword)
      (WITH BACKUP (USING stringLiteral)?)?
    ;

exportKeys
    : EXPORT ENCRYPTION? KEYS WITH SECRET secret=identifier TO stringLiteral
      (FORCE KEYSTORE)? IDENTIFIED BY keystorePassword
      (WITH IDENTIFIER IN (stringLiteral (',' stringLiteral)* | '(' subquery ')'))?
    ;

importKeys
    : IMPORT ENCRYPTION? KEYS WITH SECRET secret=identifier FROM stringLiteral
      (FORCE KEYSTORE)? IDENTIFIED BY keystorePassword
      (WITH BACKUP (USING stringLiteral)?)?
    ;

migrateKey
    : SET ENCRYPTION? KEY IDENTIFIED BY hSMAuthString=stringLiteral
      (FORCE KEYSTORE)? MIGRATE USING softwareKeystorePassword=identifier
      (WITH BACKUP (USING stringLiteral)?)?
    ;

reverseMigrateKey
    : SET ENCRYPTION? KEY IDENTIFIED BY softwareKeystorePassword=identifier
      (FORCE KEYSTORE)? REVERSE MIGRATE USING hSMAuthString=stringLiteral
    ;


moveKeys
    : MOVE ENCRYPTION? KEYS TO NEW KEYSTORE keyStoreLocation1=identifier
      IDENTIFIED BY keystore1Password=identifier FROM FORCE? KEYSTORE IDENTIFIED BY keystorePassword
      (WITH IDENTIFIER IN (stringLiteral (',' stringLiteral)* | '(' subquery ')'))?
      (WITH BACKUP (USING stringLiteral)?)?
    ;

secretManagementClauses
    : addUpdateSecret 
    | deleteSecret 
    | addUpdateSecretSeps 
    | deleteSecretSeps
    ;

addUpdateSecret
    : (ADD | UPDATE) SECRET stringLiteral FOR CLIENT stringLiteral 
      (USING TAG stringLiteral)?
      (FORCE KEYSTORE)?
      IDENTIFIED BY (EXTERNAL STORE | keystorePassword)
      (WITH BACKUP (USING stringLiteral)?)?
    ;

deleteSecret
    : DELETE SECRET FOR CLIENT stringLiteral
      (FORCE KEYSTORE)? IDENTIFIED BY (EXTERNAL STORE | keystorePassword)
      (WITH BACKUP (USING stringLiteral)?)?
    ;

addUpdateSecretSeps
    : (ADD | UPDATE) SECRET identifier FOR CLIENT stringLiteral
      (USING TAG stringLiteral)? TO LOCAL?
      AUTOLOGIN KEYSTORE directory=identifier
    ;

deleteSecretSeps
    : DELETE SECRET stringLiteral FOR CLIENT stringLiteral
      FROM LOCAL? AUTO_LOGIN KEYSTORE directory=identifier
    ;

zeroDowntimeSoftwarePatchingClauses
    : SWITCHOVER LIBRARY path=identifier FOR ALL CONTAINERS
    ;

pivotClause
    : PIVOT XML?
      '(' pivotItem (',' pivotItem)* pivotForClause pivotInClause ')'
    ;

pivotItem
    : expr (AS? alias)?
//    aggregateFunction
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
           KW_SKIP TO NEXT ROW
         | KW_SKIP PAST LAST ROW
         | KW_SKIP TO FIRST variableName
         | KW_SKIP TO LAST variableName
         | KW_SKIP TO variableName
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
    | '{' TK_INTEGER_WITHOUT_SIGN? ',' TK_INTEGER_WITHOUT_SIGN? '}' '?'?
    | '{' TK_INTEGER_WITHOUT_SIGN '}'
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
         ( ((expr | '(' expr (',' expr)* ')') (AS? alias)?) ( ',' ((expr | '(' expr (',' expr)* ')') (AS? alias)?) )*
         | subquery
         | ANY (',' ANY)*
         ) 
         ')'
    ;

partitionExtensionClause
    : PARTITION
          ( '(' partition ')'
          | FOR '(' partitionKeyValue (',' partitionKeyValue)* ')')
    | SUBPARTITION
          ('(' subpartition ')'
          | FOR '(' subpartitionKeyValue (',' subpartitionKeyValue)* ')')
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
    | expr
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
    : GROUP BY (groupByItems | '(' groupByItems ')') (HAVING condition)?
    ;

groupByItems
    : groupByItem (',' groupByItem)*
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
    : expr (',' expr )*
    | '(' expressionList ')'
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
    : windowName=identifier AS '(' existingWindowName=identifier? queryPartitionClause? orderByClause? windowingClause? ')'
    ;

windowingClause
    : ( ROWS | RANGE )
      ( BETWEEN
        ( UNBOUNDED PRECEDING
        | CURRENT ROW
        | valueExpr=expr ( PRECEDING | FOLLOWING )
        )
        AND
        ( UNBOUNDED FOLLOWING
        | CURRENT ROW
        | valueExpr=expr ( PRECEDING | FOLLOWING )
        )
      | ( UNBOUNDED PRECEDING
        | CURRENT ROW
        | valueExpr=expr PRECEDING
        )
      )
    ;

precision
    : TK_INTEGER_WITHOUT_SIGN
    ;

scale
    : TK_INTEGER_WITHOUT_SIGN
    ;

size
    : TK_INTEGER_WITHOUT_SIGN
    ;

collectionType
    : identifier
    ;

datatype
    : oracleBuiltInDatatypes
    | ansiSupportedDatatypes
    | userDefinedTypes
    | oracleSuppliedTypes
    ;

oracleBuiltInDatatypes
    : characterDatatypes
    | numberDatatypes
    | longAndRawDatatypes
    | datetimeDatatypes
    | largeObjectDatatypes
    | rowidDatatypes
    ;

characterDatatypes
    : CHAR ('(' size (BYTE | CHAR)? ')')?
    | VARCHAR2 ( '(' size (BYTE | CHAR)? ')' )?
    | NCHAR ('(' size ')')?
    | NVARCHAR2 ( '(' size ')' )?
    ;

numberDatatypes
    : NUMBER ('(' precision (',' scale)* ')')?
    | FLOAT ('(' precision ')')?
    | BINARY_FLOAT
    | BINARY_DOUBLE
    | BINARY_INTEGER
    ;

longAndRawDatatypes
    : LONG
    | LONG RAW
    | RAW '(' size ')'
    ;

datetimeDatatypes
    : DATE
    | TIMESTAMP ('(' fractionalSecondsPrecision ')')? (WITH LOCAL? TIME ZONE)?
    | INTERVAL YEAR ('(' yearPrecision=precision ')')? TO MONTH
    | INTERVAL DAY ('(' dayPrecision=precision ')')? TO SECOND ('(' fractionalSecondsPrecision ')')?
    ;

largeObjectDatatypes
    : BLOB
    | CLOB
    | NCLOB
    | BFILE
    ;

rowidDatatypes
    : ROWID
    | UROWID ('(' size ')')?
    ;

ansiSupportedDatatypes
    : CHARACTER VARYING? '(' size ')'
    | (CHAR | NCHAR) VARYING '(' size ')'
    | VARCHAR '(' size ')'
    | NATIONAL (CHARACTER | CHAR) VARYING? '(' size ')'
    | (NUMERIC | DECIMAL | DEC) ('(' precision ('.' scale)? ')')?
    | INTEGER
    | INT
    | SMALLINT
    | FLOAT ('(' size ')')?
    | DOUBLE PRECISION
    | REAL
    ;

userDefinedTypes
    : REF? ( identifier '.' )* identifier
    ;

oracleSuppliedTypes
    : anyTypes
    | xmlTypes
    | spatialTypes
    ;

anyTypes
    : SYS '.' ANYDATA
    | SYS '.' ANYTYPE
    | SYS '.' ANYDATASET
    ;

xmlTypes
    : XMLTYPE
    | URITYPE
    ;

spatialTypes
    : SDO_GEOMETRY
    | SDO_TOPO_GEOMETRY
    | SDO_GEORASTER
    ;

avExpression
    : avMeasExpression
    | avHierExpression
    ;

constraint
    : inlineConstraint
    | outOfLineConstraint
    | inlineRefConstraint
    | outOfLineRefConstraint
    ;

inlineConstraint
    : ( CONSTRAINT constraintName )?
      ( NOT? NULL
      | UNIQUE
      | PRIMARY KEY
      | referencesClause
      | CHECK '(' condition ')'
      )
      constraintState
    ;

outOfLineConstraint
    : ( CONSTRAINT constraintName )?
      ( UNIQUE '(' column (',' column)* ')'
      | PRIMARY KEY '(' column (',' column )* ')'
      | FOREIGN KEY '(' column (',' column )* ')' referencesClause
      | CHECK '(' condition ')'
      )
      constraintState
    ;

inlineRefConstraint
    : SCOPE IS (schema '.')? scopeTable=identifier
    | WITH ROWID
    | (CONSTRAINT constraintName)? referencesClause constraintState
    ;

outOfLineRefConstraint
    : SCOPE FOR '(' (refCol+=identifier | refAttr+=identifier) ')' IS (schema '.')? scopeTable=identifier
    | REF '(' (refCol+=identifier | refAttr+=identifier) ')' WITH ROWID
    | (CONSTRAINT constraintName)?
      FOREIGN KEY '(' (refCol+=identifier (',' refCol+=identifier)? | refAttr+=identifier (',' refAttr+=identifier)?) ')'
      referencesClause constraintState
    ;

condition
    : expr operator1 expr                                                               #simpleComparisonCondition1
    | '(' expr (',' expr)* ')' operator2 '(' (expressionList | subquery) ')'            #simpleComparisonCondition2
    | groupComparisonCondition                                                          #comparisonCondition
    | expr IS NOT? (NAN | INFINITE)                                                     #floatingPointCondition
    | expr IS NOT? DANGLING                                                             #danglingCondition
    | NOT condition                                                                     #logicalNotCondition
    | condition AND condition                                                           #logicalAndCondition
    | condition OR condition                                                            #logicalOrCondition
    | (dimensionColumn=identifier IS)? ANY                                              #modelIsAnyCondition
    | cellReference=cellAssignment IS PRESENT                                           #modelIsPresentCondition
    | nestedTable=identifier IS NOT? KW_A SET                                            #multisetIsASetCondition
    | nestedTable=identifier IS NOT? KW_EMPTY                                              #multisetIsEmptyCondition
    | expr NOT? MEMBER OF? nestedTable=identifier                                       #multisetMemberCondition
    | nestedTable1=identifier NOT? SUBMULTISET OF? nestedTable2=identifier              #multisetSubmultisetCondition
    | expr NOT? (LIKE | LIKEC | LIKE2 | LIKE4)
      expr (ESCAPE stringLiteral)?                                                      #patternMatchingLikeCondition
    | REGEXP_LIKE '(' (column|stringLiteral) ','
                      (column|stringLiteral)
                      (',' (column|stringLiteral))? ')'                                 #patternMatchingRegexpLikeCondition
//    | rangeCondition
    | expr IS NOT? NULL                                                                 #isNullCondition
    | EQUALS_PATH '(' expr ',' expr (',' expr)? ')'                                     #xmlEqualsPathCondition
    | UNDER_PATH '(' expr (',' expr) ',' expr (',' expr)? ')'                           #xmlUnderPathCondition
    | expr IS NOT? JSON (FORMAT JSON)? (STRICT | LAX)?
      ((ALLOW | DISALLOW) SCALARS)? ((WITH | WITHOUT) UNIQUE KEYS)?                     #jsonIsJsonCondition
    | JSON_EQUAL '(' expr ',' expr ')'                                                  #jsonEqualCondition
    | JSON_EXISTS '(' expr (FORMAT JSON)? ',' stringLiteral
      jsonPassingClause? jsonExistsOnErrorClause? jsonExistsOnEmptyClause? ')'          #jsonExistsCondition
    | JSON_TEXTCONTAINS '(' column ',' stringLiteral ',' stringLiteral ')'              #jsonTextContainsCondition
    | '(' condition ')'                                                                 #compoundParenthesisCondition
    | expr NOT? BETWEEN expr AND expr                                                   #betweenCondition
    | EXISTS '(' subquery ')'                                                           #existsCondition
    | expr NOT? IN '(' (expressionList|subquery) ')'                                    #inCondition1
    | '(' expr (',' expr)* ')' NOT?
      IN '(' (expressionList (',' expressionList)* | subquery) ')' #inCondition2
    | expr IS NOT? OF TYPE? '(' isOfTypeConditionItem (',' isOfTypeConditionItem)* ')'  #isOfTypeCondition
    ;

isOfTypeConditionItem
    : ONLY? (SCHEMA '.')? type
    ;

operator1
    : '='
    | '!='
    | '^='
    | '<>'
    | '>'
    | '<'
    | '>='
    | '<='
    ;

operator2
    : '='
    | '!='
    | '^='
    | '<>'
    ;

groupComparisonCondition
    : expr operator1 (ANY | SOME | ALL) '(' (expressionList | subquery) ')'
    | '(' expr (',' expr)* ')' operator2 (ANY | SOME | ALL) '(' (expressionList (',' expressionList)* | subquery) ')'
    | LNNVL '(' condition ')'
    ;

expr
    : '(' expr ')'                                                                          #parenthesisExpr
    | ( '+' | '-' | PRIOR ) expr                                                            #signExpr
    | TIMESTAMP expr                                                                        #timestampExpr
    | expr ( '*' | '/' | '+' | '-' | '||') expr                                             #binaryExpr
    | expr COLLATE collationName                                                            #collateExpr
    | functionExpression                                                                    #functionExpr
    | avMeasExpression                                                                      #calcMeasExpr
    | caseExpression                                                                        #caseExpr
    | CURSOR '(' subquery ')'                                                               #cursorExpr
    | intervalExpression                                                                    #intervalExpr
    | modelExpression                                                                       #modelExpr
    | objectAccessExpression                                                                #objectAccessExpr
    | placeholderExpression                                                                 #placeholderExpr
    | '(' subquery ')'                                                                      #scalarSubqueryExpr
    | typeConstructorExpression                                                             #typeConstructorExpr
    | expr AT ( LOCAL | TIME ZONE
        ( SINGLE_QUOTE_SYMBOL ('+'|'-')? hh=expr ':' mi=expr SINGLE_QUOTE_SYMBOL
        | DBTIMEZONE
        | SESSIONTIMEZONE
        | timeZoneName=stringLiteral
        | expr
        )
     )                                                                                      #datetimeExpr
    | simpleExpression                                                                      #simpleExpr
    | bindVariable                                                                          #bindVariableExpr
    | identifier MULTISET (EXCEPT | INTERSECT | UNION) (ALL | DISTINCT)? identifier         #multisetExceptExpr
    | identifier ('.' identifier)* '(' '+' ')'                                              #columnOuterJoinExpr
    ;

datetimeExpression
    : expr AT ( LOCAL | TIME ZONE
        ( SINGLE_QUOTE_SYMBOL ('+'|'-')? hh=expr ':' mi=expr SINGLE_QUOTE_SYMBOL
        | DBTIMEZONE
        | SESSIONTIMEZONE
        | timeZoneName=stringLiteral
        | expr
        )
     )
    ;

bindVariable
    : indexBindVariable
    | namedBindVariable
    ;

indexBindVariable
    : ':' TK_INTEGER_WITHOUT_SIGN
    ;

namedBindVariable
    : ':' identifier
    ;

errorLoggingSimpleExpression
    : simpleExpression
    | ((schema '.')? table '.')? identifier
    ;

simpleExpression
    : ((schema '.')? table '.')? ROWID
    | ROWNUM
    | literal
    | sequence '.' (CURRVAL | NEXTVAL)
    | NULL
    ;

//cursorExpression
//    : CURSOR '(' subquery ')'
//    ;

calcMeasExpression
    : avMeasExpression
    | avSimpleExpression
    | caseExpression
//    | compoundExpression
//    | intervalExpression
    ;

functionExpression
    : functionExpression '.' identifier
    | functionExpression '.' functionExpression
    | functionName '(' expressionList? ')'
    | functionName '(' expressionList? ')' OVER ( identifier | '(' analyticClause ')' )
    | castFunction
    | approxCountFunction
    | approxMedianFunction
    | approxPercentileFunction
    | approxPercentileDetailFunction
    | approxRankFunction
    | approxSumFunction
    | avgFunction
    | binToNumFunction
    | bitAndAggFunction
    | bitOrAggFunction
    | bitXorAggFunction
    | firstFunction
    | checksumFunction
    | chrFunction
    | clusterDetailsFunction
    | clusterDetailsAnalyticFunction
    | clusterDistanceFunction
    | clusterIdFunction
    | clusterIdAnalyticFunction
    | clusterProbabilityFunction
    | clusterProbAnalyticFunction
    | clusterSetFunction
    | clusterSetAnalyticFunction
    | collectFunction
    | connectByRootFunction
    | corrFunction
    | correlationFunction
    | countFunction
    | covarPopFunction
    | covarSampFunction
    | cubeTableFunction
    | cumeDistFunction
    | cumeDistAnalyticFunction
    | currentDateFunction
    | currentTimestampFunction
    | dbTimeZoneFunction
    | denseRankAggregateFunction
    | denseRankAnalyticFunction
    | extractDateTimeFunction
    | featureCompareFunction
    | featureDetailsFunction
    | featureIdFunction
    | featureIdAnalyticFunction
    | featureSetFunction
    | featureSetAnalyticFunction
    | featureValueFunction
    | featureValueAnalyticFunction
    | firstValueFunction
    | iterationNumberFunction
    | jsonArrayFunction
    | jsonArrayAggFunction
    | jsonMergePatchFunction
    | jsonObjectFunction
    | jsonObjectaggFunction
    | jsonQueryFunction
    | jsonScalarFunction
    | jsonSerializeFunction
    | jsonTableFunction
    | jsonTransformFunction
    | jsonValueFunction
    | kurtosisPopFunction
    | kurtosisSampFunction
    | lagFunction
    | lastFunction
    | lastValueFunction
    | leadFunction
    | listaggFunction
    | localtimestampFunction
    | maxFunction
    | medianFunction
    | minFunction
    | nthValueFunction
    | ntileFunction
    | oraDmPartitionNameFunction
    | oraInvokingUserFunction
    | oraInvokingUserIdFunction
    | percentRankAggregateFunction
    | percentRankAnalyticFunction
    | percentileContFunction
    | percentileDiscFunction
    | predictionFunction
    | predictionOrderedFunction
    | predictionAnalyticFunction
    | predictionBoundsFunction
    | predictionCostFunction
    | predictionCostAnalyticFunction
    | predictionDetailsFunction
    | predictionDetailsAnalyticFunction
    | predictionProbabilityFunction
    | predictionProbabilityOrderedFunction
    | predictionProbAnalyticFunction
    | predictionSetFunction
    | predictionSetOrderedFunction
    | predictionSetAnalyticFunction
    | rankAggregateFunction
    | rankAnalyticFunction
    | ratioToReportFunction
    | linearRegrFunction
    | sessiontimezoneFunction
    | rowNumberFunction
    | stddevFunction
    | stddevPopFunction
    | stddevSampFunction
    | sumFunction
    | sysDburigenFunction
    | sysdateFunction
    | systimestampFunction
    | toBinaryDoubleFunction
    | toBinaryFloatFunction
    | toDateFunction
    | toDsintervalFunction
    | toNumberFunction
    | toTimestampFunction
    | toTimestampTzFunction
    | toYmintervalFunction
    | translateUsingFunction
    | treatFunction
    | trimFunction
    | tzOffsetFunction
    | uidFunction
    | userFunction
    | validateConversionFunction
    | varPopFunction
    | varSampFunction
    | varianceFunction
    | xmlaggFunction
    | xmlcastFunction
    | xmlcorattvalFunction
    | xmlelementFunction
    | xmlCdataFunction
    | xmlexistsFunction
    | xmlforestFunction
    | xmlparseFunction
    | xmlpiFunction
    | xmlqueryFunction
    | xmlrootFunction
    | xmlsequenceFunction
    | xmlserializeFunction
    | xmlTableFunction
    ;

approxCountFunction
    : APPROX_COUNT '(' ('*'|expr) (',' stringLiteral)? ')'
    ;

approxMedianFunction
    : APPROX_MEDIAN '(' expr DETERMINISTIC? (',' stringLiteral)? ')'
    ;

approxPercentileFunction
    : APPROX_PERCENTILE '(' expr DETERMINISTIC? (',' stringLiteral)? ')' WITHIN GROUP '(' ORDER BY expr (DESC | ASC)? ')'
    ;

approxPercentileDetailFunction
    : APPROX_PERCENTILE_DETAIL '(' expr DETERMINISTIC? ')'
    ;

approxRankFunction
    : APPROX_RANK '(' expr? (PARTITION BY identifier)? (ORDER BY expr DESC)? ')'
    ;

approxSumFunction
    : APPROX_SUM '(' ('*' | expr) (',' SINGLE_QUOTE_SYMBOL MAX_ERROR SINGLE_QUOTE_SYMBOL) ')'?
    ;

avgFunction
    : AVG '(' (DISTINCT | ALL)? expr ')' (OVER '(' analyticClause ')')?
    ;

binToNumFunction
    : BIN_TO_NUM '(' expr (',' expr)* ')' insertIntoClause
    ;

bitAndAggFunction
    : BIT_AND_AGG '(' (DISTINCT | ALL | UNIQUE)? expr ')'
    ;

bitOrAggFunction
    : BIT_OR_AGG '(' (DISTINCT | ALL | UNIQUE)? expr ')'
    ;

bitXorAggFunction
    : BIT_XOR_AGG '(' (DISTINCT | ALL | UNIQUE)? expr ')'
    ;

castFunction
    : CAST'(' (expr | MULTISET '('subquery')' ) AS (datatype | TIMESTAMP WITH LOCAL? TIME ZONE)
        ( DEFAULT returnValue=expr ON CONVERSION ERROR )?
        (',' fmt=expr (',' nlsparam=expr )? )?')'
    ;

checksumFunction
    : CHECKSUM '(' (DISTINCT | ALL)? expr ')' (OVER '(' analyticClause ')')?
    ;

chrFunction
    : CHR '(' expr (USING NCHAR_CS)? ')'
    ;

clusterDetailsFunction
    : CLUSTER_DETAILS '(' (schema '.')? model (',' clusterId (',' topN=expr))? (DESC | ASC | ABS)? miningAttributeClause ')'
    ;

clusterDetailsAnalyticFunction
    : CLUSTER_DETAILS '(' INTO numberLiteral (',' clusterId (',' topN=expr))? (DESC | ASC | ABS)? miningAttributeClause ')' OVER '(' miningAnalyticClause ')'
    ;

clusterDistanceFunction
    : CLUSTER_DISTANCE '(' (schema '.')? model ('.' identifier)? miningAttributeClause ')'
    ;

clusterIdFunction
    : CLUSTER_ID '(' (schema '.')? model miningAttributeClause ')'
    ;

clusterIdAnalyticFunction
    : CLUSTER_ID '(' INTO expr miningAttributeClause ')' OVER '(' miningAnalyticClause ')'
    ;

clusterProbabilityFunction
    : CLUSTER_PROBABILITY '(' (schema '.')? model (',' identifier)? miningAttributeClause ')' 
    ;

clusterProbAnalyticFunction
    : CLUSTER_PROBABILITY '(' INTO expr (',' identifier)? miningAttributeClause ')' OVER '(' miningAnalyticClause ')'
    ;

clusterSetFunction
    : CLUSTER_SET '(' (schema '.')? model (',' topN=expr (',' cutoff=expr)?)? miningAttributeClause ')'
    ;

clusterSetAnalyticFunction
    : CLUSTER_SET '(' INTO expr (',' topN=expr (',' cutoff=expr)?)? miningAttributeClause ')' OVER '(' miningAttributeClause ')'
    ;

collectFunction
    : COLLECT '(' (DISTINCT | UNIQUE)? column (ORDER BY expr)? ')'
    ;

connectByRootFunction
    : CONNECT_BY_ROOT column
    ;

corrFunction
    : CORR '(' expr ',' expr ')' (OVER '(' analyticClause ')')?
    ;

correlationFunction
    : (CORR_K | CORR_S) '(' expr ',' expr 
      (',' (COEFFICIENT | ONE_SIDED_SIG | ONE_SIDED_SIG_POS | ONE_SIDED_SIG_NEG | TWO_SIDED_SIG))?
      ')'    
    ;

countFunction
    : COUNT '(' ('*' | (DISTINCT | ALL)? expr) ')' (OVER '(' analyticClause ')')?
    ;

covarPopFunction
    : COVAR_POP '(' expr ',' expr ')' (OVER '(' analyticClause ')')?
    ;

covarSampFunction
    : COVAR_SAMP '(' expr ',' expr ')' (OVER '(' analyticClause ')')?
    ;

cubeTableFunction
    : CUBE_TABLE '(' stringLiteral ')'
    ;

cumeDistFunction
    : CUME_DIST '(' expr (',' expr)* ')' WITHIN GROUP '(' ORDER BY cumeDistItem (',' cumeDistItem)* ')'
    ;

cumeDistAnalyticFunction
    : CUME_DIST '(' ')' OVER '(' queryPartitionClause? orderByClause ')'
    ;

currentDateFunction
    : CURRENT_DATE
    ;

currentTimestampFunction
    : CURRENT_TIMESTAMP ('(' precision ')')?
    ;

dbTimeZoneFunction
    : DBTIMEZONE
    ;

denseRankAggregateFunction
    : DENSE_RANK '(' expr (',' expr)* ')' WITHIN GROUP
      '(' ORDER BY denseRankAggregateItem (',' denseRankAggregateItem)* ')'
    ;

denseRankAnalyticFunction
    : DENSE_RANK '(' ')' OVER '(' queryPartitionClause? orderByClause ')'
    ;

extractDateTimeFunction
    : EXTRACT '(' 
      (YEAR | MONTH | DAY | HOUR | MINUTE | SECOND | TIMEZONE_HOUR | TIMEZONE_MINUTE | TIMEZONE_REGION | TIMEZONE_ABBR)
      FROM expressionList ')'
    ;

featureCompareFunction
    : FEATURE_COMPARE '(' (schema '.')? model miningAttributeClause AND miningAttributeClause ')'
    ;

featureDetailsFunction
    : FEATURE_DETAILS '(' (schema '.')? model
      (',' expr (',' expr)?)? (DESC | ASC | ABS)? miningAttributeClause ')'
    ;

featureIdFunction
    : FEATURE_ID '(' (schema '.')? model miningAttributeClause ')'
    ;

featureIdAnalyticFunction
    : FEATURE_ID '(' INTO identifier miningAttributeClause ')' OVER '(' miningAnalyticClause ')'
    ;

featureSetFunction
    : FEATURE_SET '(' (schema '.')? model (',' expr (',' expr)?)? miningAttributeClause ')'
    ;

featureSetAnalyticFunction
    : FEATURE_SET '(' INTO expr (',' expr (',' expr)?)? miningAttributeClause ')' OVER '(' miningAnalyticClause ')'
    ;

featureValueFunction
    : FEATURE_VALUE '(' (schema '.')? model (',' expr)? miningAttributeClause ')'
    ;

featureValueAnalyticFunction
    : FEATURE_VALUE '(' INTO expr (',' featureId)? miningAttributeClause ')' OVER '(' miningAnalyticClause ')'
    ;

firstFunction
    : functionCall KEEP '(' DENSE_RANK FIRST ORDER BY expr (DESC | ASC)? (NULLS (FIRST | LAST))? ')'
      (OVER '(' queryPartitionClause? ')')?
    ;

firstValueFunction
    : FIRST_VALUE ('(' ( expr | identifier ) ')' ((RESPECT | IGNORE) NULLS)?
                  |'(' ( expr | identifier ) ((RESPECT | IGNORE) NULLS)? ')'
                  )
      OVER ( '(' analyticClause ')' | analyticClause )
    ;

iterationNumberFunction
    : ITERATION_NUMBER
    ;

jsonArrayFunction
    : JSON_ARRAY '(' jsonArrayContent ')'
    | JSON '[' jsonArrayContent ']'
    ;

jsonArrayAggFunction
    : JSON_ARRAYAGG '(' expr (FORMAT JSON)? orderByClause? jsonOnNullClause?
      jsonReturningClause? STRICT? ')'
    ;

jsonMergePatchFunction
    : JSON_MERGEPATCH '(' expr ',' expr jsonReturningClause? PRETTY? ASCII? TRUNCATE? jsonOnErrorClause
    ;

jsonObjectFunction
    : JSON_OBJECT '(' jsonObjectContent ')'
    | JSON '(' jsonObjectContent ')'
    ;

jsonObjectaggFunction
    : JSON_OBJECTAGG '(' KEY? expr VALUE expr jsonOnNullClause? jsonReturningClause? 
      STRICT? (WITH UNIQUE KEYS)? ')'
    ;

jsonQueryFunction
    : JSON_QUERY '(' expr (FORMAT JSON)? ',' stringLiteral
      jsonQueryReturningClause jsonQueryWrapperClause?
      jsonQueryOnErrorClause? jsonQueryOnEmptyClause?
      ')'
    ;

jsonScalarFunction
    : JSON_SCALAR '(' expr (SQL | JSON)? (NULL ON NULL)? ')'
    ;

jsonSerializeFunction
    : JSON_SERIALIZE '(' expr jsonReturningClause?
      PRETTY? ASCII? TRUNCATE? ((NULL | ERROR) ON ERROR)? ')'
    ;

jsonTableFunction
    : JSON_TABLE '(' expr (FORMAT JSON)? (',' stringLiteral)? jsonTableOnErrorClause? ','? jsonColumnsClause ')'
    ;

jsonTransformFunction
    : JSON_TRANSFORM '(' expr ',' operation (',' operation) ')' jsonTransformReturningClause? jsonPassingClause?
    ;

jsonValueFunction
    : JSON_VALUE '(' expr (FORMAT JSON)? ',' stringLiteral? jsonValueReturningClause?
      jsonValueOnErrorClause? jsonValueOnEmptyClause? jsonValueOnMismatchClause?
      ')'
    ;

kurtosisPopFunction
    : KURTOSIS_POP '(' (DISTINCT | ALL | UNIQUE)? expr ')'
    ;

kurtosisSampFunction
    : KURTOSIS_SAMP '(' (DISTINCT | ALL | UNIQUE)? expr ')'
    ;

lagFunction
    : LAG ( '(' expr (',' expr (',' expr))? ')' ((RESPECT | IGNORE) NULLS)?
          | '(' expr ((RESPECT | IGNORE) NULLS)? (',' expr (',' expr)?)? ')'
          )
      OVER '(' queryPartitionClause? orderByClause ')'
    ;

lastFunction
    : functionCall KEEP '(' DENSE_RANK LAST ORDER BY lastFunctionItem (',' lastFunctionItem)* ')'
      (OVER '(' queryPartitionClause? ')')?
    ;

lastValueFunction
    : LAST_VALUE ('(' ( expr | identifier ) ')' ((RESPECT | IGNORE) NULLS)?
                  |'(' ( expr | identifier ) ((RESPECT | IGNORE) NULLS)? ')'
                  )
      OVER ( '(' analyticClause ')' | analyticClause )
    ;

leadFunction
    : LEAD ( '(' expr (',' expr (',' expr)?)? ')' ((RESPECT | IGNORE) NULLS)? 
           | '(' expr ((RESPECT | IGNORE) NULLS)? (',' expr (',' expr)?)? ')'
           )
           OVER '(' queryPartitionClause? orderByClause ')'
    ;

listaggFunction
    : LISTAGG '(' ALL? DISTINCT? expr (',' stringLiteral)? listaggOverflowClause? ')' (WITHIN GROUP)? '(' orderByClause ')' (OVER '(' queryPartitionClause ')')?
    ;

localtimestampFunction
    : LOCALTIMESTAMP ('(' expr ')')?
    ;

maxFunction
    : MAX '(' (DISTINCT | ALL)? expr ')' (OVER '(' analyticClause ')')?
    ;

medianFunction
    : MEDIAN '(' expr ')' (OVER '(' analyticClause ')')?
    ;

minFunction
    : MIN '(' (DISTINCT | ALL)? expr ')' (OVER '(' analyticClause ')')?
    ;

nthValueFunction
    : NTH_VALUE '(' ( expr | identifier ) ',' ( expr | identifier ) ')' (FROM (FIRST | LAST))? ((RESPECT | IGNORE) NULLS)?
      OVER ( '(' analyticClause ')' | analyticClause )
    ;

ntileFunction
    : NTILE '(' expr ')' OVER '(' queryPartitionClause? orderByClause ')'
    ;

oraDmPartitionNameFunction
    : ORA_DM_PARTITION_NAME '(' (schema '.')? model miningAttributeClause ')'
    ;

oraInvokingUserFunction
    : ORA_INVOKING_USER
    ;

oraInvokingUserIdFunction
    : ORA_INVOKING_USERID
    ;

percentRankAggregateFunction
    : PERCENT_RANK '(' expr (',' expr)* ')' WITHIN GROUP '(' ORDER BY 
      expr (DESC | ASC)? (NULLS (FIRST | LAST))? (',' expr (DESC | ASC)? (NULLS (FIRST | LAST))?)* 
      ')'
    ;

percentRankAnalyticFunction
    : PERCENT_RANK '(' ')' OVER '(' queryPartitionClause? orderByClause ')'
    ;

percentileContFunction
    : PERCENTILE_CONT '(' expr ')' WITHIN GROUP '(' ORDER BY expr (DESC | ASC)? ')'
      (OVER '(' queryPartitionClause ')')?
    ;

percentileDiscFunction
    : PERCENTILE_DISC '(' expr ')' WITHIN GROUP '(' ORDER BY expr (DESC | ASC)? ')'
      (OVER '(' queryPartitionClause ')')?
    ;

predictionFunction
    : PREDICTION '(' 
//      WARN: can't find definition 
//      groupingHint?
      (schema '.')? model costMatrixClause? miningAttributeClause ')'
    ;

predictionOrderedFunction
    : PREDICTION '('
//      WARN: can't find definition 
//      groupingHint?
      (schema '.')? model
      costMatrixClause? miningAttributeClause ')'
      OVER '(' orderByClause (',' orderByClause)* ')'
    ;

predictionAnalyticFunction
    : PREDICTION '(' (OF ANOMALY | FOR expr) costMatrixClause? miningAttributeClause ')'
      OVER '(' miningAnalyticClause ')'
    ;

predictionBoundsFunction
    : PREDICTION_BOUNDS '(' (schema '.')? model (',' expr (',' expr)?)? miningAttributeClause ')'
    ;

predictionCostFunction
    : PREDICTION_COST '(' (schema '.')? model (',' expr)? costMatrixClause miningAttributeClause ')'
      (OVER '(' orderByClause (',' orderByClause)* ')')?
    ;

predictionCostAnalyticFunction
    : PREDICTION_COST '(' ( OF ANOMALY | FOR expr ) (',' identifier)? costMatrixClause miningAttributeClause ')' OVER '(' miningAnalyticClause ')'
    ;

predictionDetailsFunction
    : PREDICTION_DETAILS '(' (schema '.')? model (',' expr (',' expr)?)? (DESC | ASC | ABS)? miningAttributeClause ')'
      (OVER '(' orderByClause (',' orderByClause)? ')')?
    ;

predictionDetailsAnalyticFunction
    : PREDICTION_DETAILS '(' ( OF ANOMALY | FOR expr ) (',' expr (',' expr)?)? (DESC | ASC | ABS)? miningAttributeClause ')' OVER '(' miningAnalyticClause ')'
    ;

predictionProbabilityFunction
    : PREDICTION_PROBABILITY '(' (schema '.')? model (',' expr)? miningAttributeClause ')'
    ;

predictionProbabilityOrderedFunction
    : PREDICTION_PROBABILITY '(' (schema '.')? model (',' expr)? miningAttributeClause ')' OVER '(' orderByClause (',' orderByClause)* ')'
    ;

predictionProbAnalyticFunction
    : PREDICTION_PROBABILITY '(' ( OF ANOMALY | FOR expr ) (',' expr)? miningAttributeClause ')' OVER '(' miningAnalyticClause ')'
    ;

predictionSetFunction
    : PREDICTION_SET '(' (schema '.')? model (',' identifier (',' identifier)?)? costMatrixClause? miningAttributeClause ')'
    ;

predictionSetOrderedFunction
    : PREDICTION_SET '(' (schema '.')? model (',' identifier (',' identifier)?)? costMatrixClause? miningAttributeClause ')' OVER '(' orderByClause (',' orderByClause)* ')'
    ;

predictionSetAnalyticFunction
    : PREDICTION_SET '(' '(' OF ANOMALY | FOR stringLiteral ')' (',' identifier (',' identifier)?)? costMatrixClause? miningAttributeClause ')' OVER '(' miningAnalyticClause ')'
    ;

rankAggregateFunction
    : RANK '(' expr (',' expr)* ')' WITHIN GROUP '(' ORDER BY expr (DESC | ASC)? (NULLS (FIRST | LAST))? (',' expr (DESC | ASC)? (NULLS (FIRST | LAST))?)* ')'
    ;

rankAnalyticFunction
    : RANK '(' ')' OVER '(' queryPartitionClause? orderByClause ')'
    ;

ratioToReportFunction
    : RATIO_TO_REPORT '(' expr ')' OVER '(' queryPartitionClause? ')'
    ;

linearRegrFunction
    : ( REGR_SLOPE 
      | REGR_INTERCEPT 
      | REGR_COUNT 
      | REGR_R2 
      | REGR_AVGX 
      | REGR_AVGY 
      | REGR_SXX 
      | REGR_SYY 
      | REGR_SXY
      ) '(' expr ',' expr ')'
      (OVER '(' analyticClause ')')?
    ;

sessiontimezoneFunction
    : SESSIONTIMEZONE
    ;

rowNumberFunction
    : ROW_NUMBER '(' ')' OVER '(' queryPartitionClause? orderByClause ')'
    ;

stddevFunction
    : STDDEV '(' (DISTINCT | ALL)? expr ')' (OVER '(' analyticClause ')')?
    ;

stddevPopFunction
    : STDDEV_POP '(' expr ')' (OVER '(' analyticClause ')')?
    ;

stddevSampFunction
    : STDDEV_SAMP '(' expr ')' (OVER '(' analyticClause ')')?
    ;

sumFunction
    : SUM '(' (DISTINCT | ALL)? ')' expr ')' (OVER '(' analyticClause ')')?
    ;

sysDburigenFunction
    : SYS_DBURIGEN '(' (identifier identifier?) (','identifier identifier?)* (',' stringLiteral '(' ')')? ')'
    ;

sysdateFunction
    : SYSDATE
    ;

systimestampFunction
    : SYSTIMESTAMP
    ;

toBinaryDoubleFunction
    : TO_BINARY_DOUBLE '(' expr (DEFAULT expr ON CONVERSION ERROR)? (',' expr (',' expr)?)? ')'
    ;

toBinaryFloatFunction
    : TO_BINARY_FLOAT '(' expr (DEFAULT expr ON CONVERSION ERROR)? (',' expr (',' expr)?)? ')'
    ;

toDateFunction
    : TO_DATE '(' expr (DEFAULT expr ON CONVERSION ERROR)? (',' expr (',' stringLiteral)?)? ')'
    ;

toDsintervalFunction
    : TO_DSINTERVAL '(' stringLiteral (DEFAULT expr ON CONVERSION ERROR)? ')'
    ;

toNumberFunction
    : TO_NUMBER '(' expr (DEFAULT expr ON CONVERSION ERROR)? (',' expr (',' expr)?)? ')'
    ;

toTimestampFunction
    : TO_TIMESTAMP '(' expr (DEFAULT expr ON CONVERSION ERROR)? (',' expr (',' expr)?)? ')'
    ;

toTimestampTzFunction
    : TO_TIMESTAMP_TZ '(' expr (DEFAULT expr ON CONVERSION ERROR)? (',' expr (',' expr)?)? ')'
    ;

toYmintervalFunction
    : TO_YMINTERVAL '(' stringLiteral (DEFAULT expr ON CONVERSION ERROR)? ')'
    ;

translateUsingFunction
    : TRANSLATE '(' expr USING (CHAR_CS | NCHAR_CS) ')'
    ;

treatFunction
    : TREAT '(' expr AS (REF? ( schema '.' )? type | JSON) ')' jsonNonfunctionSteps? jsonFunctionStep?
    ;

trimFunction
    : TRIM '(' (((LEADING | TRAILING | BOTH) expr? | expr) FROM)? expr ')'
    ;

tzOffsetFunction
    : TZ_OFFSET '(' (stringLiteral | SESSIONTIMEZONE | DBTIMEZONE) ')'
    ;

uidFunction
    : UID
    ;

userFunction
    : USER
    ;

validateConversionFunction
    : VALIDATE_CONVERSION '(' expr AS validateConversionTypeName (',' expr (',' stringLiteral)?)? ')'
    ;

validateConversionTypeName
    : BINARY_DOUBLE
    | BINARY_FLOAT
    | DATE
    | INTERVAL DAY TO SECOND
    | INTERVAL YEAR TO MONTH
    | NUMBER
    | TIMESTAMP
    | TIMESTAMP WITH TIME ZONE
    | TIMESTAMP WITH LOCAL TIME ZONE
    ;

varPopFunction
    : VAR_POP '(' expr ')' (OVER '(' analyticClause ')')?
    ;

varSampFunction
    : VAR_SAMP '(' expr ')' (OVER '(' analyticClause ')')?
    ;

varianceFunction
    : VARIANCE '(' (DISTINCT | ALL)? expr ')' (OVER '(' analyticClause ')')?
    ;

xmlaggFunction
    : XMLAGG '(' expr (',' expr)* orderByClause? ')'
    ;

xmlcastFunction
    : XMLCAST '(' expr AS datatype ')'
    ;

xmlcorattvalFunction
    : XMLCOLATTVAL '(' xmlcorattvalFunctionItem (',' xmlcorattvalFunctionItem)* ')'
    ;

xmlelementFunction
    : XMLELEMENT '(' (ENTITYESCAPING | NOENTITYESCAPING)? (NAME? identifier | EVALNAME expr)
      (',' xmlAttributesClause)? (',' expr (AS? cAlias)? )* ')'
    ;

xmlCdataFunction
    : XMLCDATA '(' stringLiteral ')'
    ;

xmlexistsFunction
    : XMLEXISTS '(' expr xmlPassingClause? ')'
    ;

xmlforestFunction
    : XMLFOREST '(' xmlForestItem (',' xmlForestItem)* ')'
    ;

xmlparseFunction
    : XMLPARSE '(' (DOCUMENT | CONTENT) expr WELLFORMED? ')'
    ;

xmlpiFunction
    : XMLPI '(' (NAME? identifier | EVALNAME expr) (',' expr)? ')'
    ;

xmlqueryFunction
    : XMLQUERY '(' stringLiteral xmlPassingClause? RETURNING CONTENT (NULL ON KW_EMPTY)? ')'
    ;

xmlrootFunction
    : XMLROOT '(' expr ',' VERSION (expr | NO VALUE) (',' STANDALONE (YES | NO | NO VALUE))? ')'
    ;

xmlsequenceFunction
    : XMLSEQUENCE '(' (expr | identifier ',' expr) ')'
    ;

xmlserializeFunction
    : XMLSERIALIZE '(' (DOCUMENT | CONTENT) expr (AS datatype)? (ENCODING stringLiteral)? (VERSION stringLiteral)?
      (NO INDENT | INDENT (SIZE '=' numberLiteral)?)?
      ((HIDE | SHOW) DEFAULTS)? ')'
    ;

xmlTableFunction
    : XMLTABLE '(' (xmlnamespacesClause ',')? expr xmltableOptions ')'
    ;

xmltableOptions
    : xmlPassingClause? (RETURNING SEQUENCE BY REF)? (COLUMNS xmlTableColumn (',' xmlTableColumn)*)?
    ;

xmlTableColumn  
    : column ( FOR ORDINALITY
             | ( datatype
               | XMLTYPE ('(' SEQUENCE ')' BY REF)?
               ) 
               (PATH stringLiteral)?
               (DEFAULT expr)?
             )
    ;

xmlnamespacesClause
    : XMLNAMESPACES '(' xmlnamespacesClauseItem (',' xmlnamespacesClauseItem)* ')' 
    ;

xmlnamespacesClauseItem
    : stringLiteral AS identifier
    | DEFAULT stringLiteral
    ;

xmlForestItem
    : expr (AS (cAlias | EVALNAME expr))?
    ;

xmlPassingClause
    : PASSING (BY VALUE)? xmlPassingClauseItem (',' xmlPassingClauseItem)*
    ;

xmlPassingClauseItem
    : expr (AS identifier)?
    ;

xmlAttributesClause
    : XMLATTRIBUTES
      '(' 
        (ENTITYESCAPING | NOENTITYESCAPING)? (SCHEMACHECK | NOSCHEMACHECK)?
        xmlAttributesClauseItem ( ',' xmlAttributesClauseItem )* 
      ')'
    ;

xmlAttributesClauseItem
    : expr (AS? cAlias | AS EVALNAME expr)?
    ;

xmlcorattvalFunctionItem
    : expr (AS cAlias | EVALNAME expr)?
    ;

costMatrixClause
    : COST ( MODEL AUTO? 
           | '(' expr (',' expr)* ')' VALUES '(' '(' expr (',' expr)* ')' (',' '(' expr (',' expr)* ')')* ')'
           )
    ;

listaggOverflowClause
    : ON OVERFLOW ERROR
    | ON OVERFLOW TRUNCATE stringLiteral? ((WITH | WITHOUT) COUNT)?
    ;

lastFunctionItem
    : expr (DESC | ASC)? (NULLS (FIRST | LAST))?
    ;

jsonValueReturningClause
    : RETURNING jsonValueReturnType ASCII?
    ;

jsonValueOnMismatchClause
    : ((IGNORE | ERROR | NULL) ON MISMATCH ('(' jsonValueOnMismatchClauseItem (',' jsonValueOnMismatchClauseItem)* ')')?)+
    ;

jsonValueOnMismatchClauseItem
    : MISSING DATA
    | EXTRA DATA
    | TYPE ERROR
    ;

operation
    : removeOp
    | insertOp
    | replaceOp
    | appendOp
    | setOp
    | renameOp
    | keepOp
    ;

removeOp
    : REMOVE expr ((IGNORE | ERROR) ON MISSING)?
    ;

insertOp
    : INSERT expr '=' rhsExpr ((REPLACE | IGNORE | ERROR) ON EXISTING)? ((NULL | IGNORE | ERROR | REMOVE) ON NULL)?
    ;

replaceOp
    : REPLACE expr '=' rhsExpr ((CREATE | IGNORE | ERROR) ON MISSING)? ((NULL | IGNORE | ERROR | REMOVE) ON NULL)?
    ;

appendOp
    : APPEND expr '=' rhsExpr ((CREATE | IGNORE | ERROR) ON MISSING) ((NULL | IGNORE | ERROR) ON NULL)?
    ;

setOp
    : SET expr '=' rhsExpr ((IGNORE | ERROR | REPLACE) ON EXISTING)? ((CREATE | IGNORE | ERROR) ON MISSING)? 
      ((NULL | IGNORE | ERROR) ON NULL)?
    ;

renameOp
    : RENAME expr WITH stringLiteral ((IGNORE | ERROR) ON MISSING)?
    ;

keepOp
    : KEEP keepOpItem (',' keepOpItem)*
    ;

keepOpItem
    : expr ((IGNORE | ERROR) ON MISSING)?
    ;

rhsExpr
    : sqlExpr=expr (FORMAT JSON)?
    ;

jsonTransformReturningClause
    : RETURNING ( VARCHAR2 ('(' size (BYTE | CHAR)?)?
                | CLOB
                | BLOB
                | JSON)
                (ALLOW | DISALLOW)?
    ;

jsonPassingClause
    : PASSING expr AS identifier (',' expr AS identifier)*
    ;

jsonTableOnErrorClause
    : (ERROR | NULL) ON ERROR
    ;

jsonColumnsClause
    : COLUMNS '(' jsonColumnDefinition (',' jsonColumnDefinition)* TRUNCATE? ')'
    ;

jsonColumnDefinition
    : jsonExistsColumn
    | jsonQueryColumn
    | jsonValueColumn
    | jsonNestedPath
    | ordinalityColumn
    ;

jsonExistsColumn
    : column jsonValueReturnType? EXISTS (PATH jsonPath)?
      jsonExistsOnErrorClause? jsonExistsOnEmptyClause?
    ;

jsonQueryColumn
    : column jsonQueryReturnType? (FORMAT JSON)?
      (ALLOW | DISALLOW SCALARS)? jsonQueryWrapperClause?
      (PATH jsonPath)? jsonQueryOnErrorClause?
    ;

jsonValueColumn
    : column jsonValueReturnType? TRUNCATE? (PATH jsonPath)?
      jsonValueOnErrorClause? jsonValueOnEmptyClause?
    ;

jsonValueOnErrorClause
    : (ERROR | NULL | DEFAULT literal) ON ERROR
    ;

jsonValueOnEmptyClause
    : (ERROR | NULL | DEFAULT literal) ON KW_EMPTY
    ;

jsonNestedPath
    : NESTED PATH? jsonPath jsonColumnsClause
    ;

jsonPath
    : dotnotation
    | stringLiteral
    ;

dotnotation
    : JSON_PATH_SYMBOL? dotnotationExpr ('.' dotnotationExpr)*
    ;

dotnotationExpr 
    : identifierWithQualifier
    | identifier
    ;

identifierWithQualifier 
    : identifier '[' ']'
    | identifier '[' integer ']'
    | identifier '[' queryExpr ']'
    | identifier '[' '?' '(' queryExpr ')' ']'
    ;

queryExpr : queryExpr ('&&' queryExpr )+
          | queryExpr ('||' queryExpr )+
          | '*'
          | '@' '.' identifier
          | '@' '.' identifier '>' integer
          | '@' '.' identifier '<' integer
          | '@' '.' LENGTH '-' integer
          | '@' '.' identifier '=' '=' integer
          | '@' '.' identifier '=' '=' stringLiteral
          ;

ordinalityColumn
    : column FOR ORDINALITY
    ;

jsonValueReturnType
    : VARCHAR2 ('(' size (BYTE | CHAR)? ')')? TRUNCATE?
    | CLOB
    | NUMBER ('(' precision (',' scale)? ')')?
    | DATE ((TRUNCATE | PRESERVE) TIME)?
    | TIMESTAMP (WITH TIMEZONE)?
    | SDO_GEOMETRY
    | jsonValueReturnObjectInstance
    ;

jsonValueReturnObjectInstance
    : identifier jsonValueMapperClause?
    ;

jsonValueMapperClause
    : USING CASE_SENSITIVE MAPPING
    ;

jsonArrayContent
    : jsonArrayElement (',' jsonArrayElement)* jsonOnNullClause?
      jsonReturningClause? STRICT?
    ;

jsonArrayElement
    : expr formatClause?
    ;

formatClause
    : FORMAT JSON
    ;

jsonOnNullClause
    : (NULL | ABSENT) ON NULL
    ;

jsonReturningClause
    : RETURNING ( VARCHAR2 ('(' size (BYTE | CHAR)? ')')? (WITH TYPENAME)?
                | CLOB
                | BLOB
                | JSON
                )
    ;

jsonQueryReturningClause
    : (RETURNING jsonQueryReturnType)? ((ALLOW | DISALLOW) SCALARS)? PRETTY? ASCII?
    ;

jsonQueryReturnType
    : VARCHAR2 ('(' size (BYTE | CHAR)? ')')?
    | CLOB
    | BLOB
    | JSON
    ;

jsonQueryWrapperClause
    : WITHOUT ARRAY? WRAPPER
    | WITH (UNCONDITIONAL | CONDITIONAL)? ARRAY? WRAPPER
    ;

jsonQueryOnErrorClause
    : (ERROR
      | NULL
      | KW_EMPTY
      | KW_EMPTY ARRAY
      | KW_EMPTY OBJECT
      ) ON ERROR
    ;

jsonQueryOnEmptyClause
    : (ERROR
      | NULL
      | KW_EMPTY
      | KW_EMPTY ARRAY
      | KW_EMPTY OBJECT
      ) ON KW_EMPTY
    ;

jsonObjectContent
    : (entry (',' entry)* | '*')? jsonOnNullClause? jsonReturningClause?
      STRICT? (WITH UNIQUE KEYS)?
    ;

entry
    : regularEntry formatClause?
    | wildcard
    ;

regularEntry
    : KEY? stringLiteral VALUE expr
    | expr (':' expr)?
    | column
    ;

wildcard
    : (identifier '.') identifier '.' '*'
    ;

jsonOnErrorClause
    : (ERROR | NULL) ON ERROR
    ;

denseRankAggregateItem
    : expr (DESC | ASC)? (NULLS (FIRST |LAST))?
    ;

cumeDistItem
    : expr (DESC | ASC)? (NULLS (FIRST |LAST))?
    ;

miningAnalyticClause
    : queryPartitionClause? orderByClause?
    ;

miningAttributeClause
    : USING ('*' | miningAttributeClauseItem (',' miningAttributeClauseItem)*)
    ;

miningAttributeClauseItem
    : (schema '.')? table '.' '*' | expr (AS alias)?
    ;

avMeasExpression
    : leadLagExpression
    | avWindowExpression
    | shareOfExpression
    | qdrExpression
    ;

leadLagExpression
    : leadLagFunctionName '(' calcMeasExpression ')' OVER '(' leadLagClause ')'
    ;

leadLagFunctionName
    : LAG
    | LAG_DIFF
    | LAG_DIFF_PERCENT
    | LEAD
    | LEAD_DIFF
    | LEAD_DIFF_PERCENT
    ;

leadLagClause
    : HIERARCHY hierarchyRef OFFSET offsetExpr=expr ( WITHIN ( LEVEL | PARENT ) | ACROSS ANCESTOR AT LEVEL levelRef=identifier POSITION FROM ( BEGINNING | END ))
    ;

hierarchyRef
    : ( attrDimAlias=identifier '.' )? hierAlias=identifier
    ;

avWindowExpression
    : functionExpression OVER ( avWindowClause )
    ;

avWindowClause
    : HIERARCHY hierarchyRef BETWEEN ( precedingBoundary | followingBoundary ) ( WITHIN ( LEVEL | PARENT | ANCESTOR AT LEVEL levelName=identifier ) )?
    ;

precedingBoundary
    : ( UNBOUNDED PRECEDING | offsetExpr=expr PRECEDING ) AND ( CURRENT MEMBER | offsetExpr=expr  ( PRECEDING | FOLLOWING ) | UNBOUNDED FOLLOWING )
    ;

followingBoundary
    : ( CURRENT MEMBER | offsetExpr=expr FOLLOWING ) AND ( offsetExpr=expr FOLLOWING | UNBOUNDED FOLLOWING )
    ;

shareOfExpression
    : SHARE_OF ( calcMeasExpression  shareClause )
    ;

shareClause
    : HIERARCHY hierarchyRef ( PARENT | LEVEL levelRef=identifier | MEMBER memberExpression )
    ;

levelMemberLiteral
    : levelRef=identifier ( posMemberKeys | namedMemberKeys )
    ;

posMemberKeys
    : '[' memberKeyExpr+=expr (',' memberKeyExpr+=expr)* ']'
    ;

namedMemberKeys
    : '[' attrName+=identifier '=' memberKeyExpr+=expr (',' attrName+=identifier '=' memberKeyExpr+=expr )* ']'
    ;

hierNavigationExpression
    : ( hierAncestorExpression | hierParentExpression | hierLeadLagExpression )
    ;

hierAncestorExpression
    : HIER_ANCESTOR '(' memberExpression AT ( LEVEL levelRef=identifier | DEPTH depthExpression=expr ) ')'
    ;

memberExpression
    : levelMemberLiteral
    | hierNavigationExpression
    | CURRENT MEMBER
    | NULL
    | ALL
    ;

hierParentExpression
    : HIER_PARENT '(' memberExpression ')'
    ;

hierLeadLagExpression
    : ( HIER_LEAD | HIER_LAG ) '(' hierLeadLagClause ')'
    ;

hierLeadLagClause
    : memberExpression  OFFSET offsetExpr=expr ( WITHIN ( ( LEVEL | PARENT ) | ACROSS ANCESTOR AT LEVEL levelRef=identifier ( POSITION FROM ( BEGINNING | END ) )? ) )?
    ;

qdrExpression
    : QUALIFY '(' calcMeasExpression',' qualifier ')'
    ;

qualifier
    : hierarchyRef '=' memberExpression
    ;

avSimpleExpression
    : stringLiteral
    | numberLiteral
    | NULL
    | measureRef
    ;

avHierExpression
    : hierFunctionName '(' memberExpression WITHIN HIERARCHY hierarchyRef ')'
    ;

hierFunctionName
    : HIER_CAPTION
    | HIER_DEPTH
    | HIER_DESCRIPTION
    | HIER_LEVEL
    | HIER_MEMBER_NAME
    | HIER_MEMBER_UNIQUE_NAME
    ;

measureRef
    : ( MEASURES '.' )? measName=identifier
    ;

//compoundExpression
//    :
//    ;

//datetimeExpressionession
//    :
//    ;

//intervalExpression
//    :
//    ;

caseExpression
    : CASE (simpleCaseExpression | searchedCaseExpression) elseClause? END
    ;

intervalExpression
    : '(' expr '-' expr ')'
      ( DAY ('(' leadingFieldPrecision=expr ')')? TO SECOND ('(' fractionalSecondPrecision=expr ')')?
      | YEAR ('(' leadingFieldPrecision=expr ')')? TO MONTH
      )
    ;

arrayStep
    : '[' (( integer | integer TO integer (',' (integer | integer TO integer) )* ) | '*') ']'
    ;

modelExpression
    : measureColumn=identifier '[' ( condition | expr ) (',' ( condition | expr ) )* ']'
    | functionExpression '['
              (
                ( condition | expr ) (',' ( condition | expr ) )*
                | singleColumnForLoop (',' singleColumnForLoop )*
                | multiColumnForLoop
              ) ']'
    ;

analyticClause
    : ( windowName=identifier | queryPartitionClause )? ( orderByClause windowingClause? )?
    ;

objectAccessExpression
    : ('(' expr ')' '.')? simpleIdentifier ('.' identifier arrayStep* )* ('(' ( argument (',' argument )* )? ')')?
    | pseudoColumn
    ;

placeholderExpression
    : namedBindVariable ( INDICATOR? ':' indicatorVariable=identifier )?
    ;

typeConstructorExpression
    : NEW ( schema '.' )? typeName '(' ( expr (',' expr )* )? ')'
    ;

simpleCaseExpression
    : expr (WHEN comparisonExpr=expr THEN returnExpr=expr)+
    ;

searchedCaseExpression
    : (WHEN condition THEN returnExpr=expr)+
    ;

elseClause
    : ELSE elseExpr=expr
    ;


jsonExistsOnErrorClause
    : ERROR ON ERROR
    | TRUE ON ERROR
    | FALSE ON ERROR
    ;

jsonExistsOnEmptyClause
    : NULL ON KW_EMPTY
    | ERROR ON KW_EMPTY
    | DEFAULT literal ON KW_EMPTY
    ;

jsonBasicPathExpression
    : jsonAbsolutePathExpr
    | jsonRelativePathExpr
    ;

jsonAbsolutePathExpr
    : '$' jsonNonfunctionSteps? jsonFunctionStep?
    ;

jsonNonfunctionSteps
    : (( jsonObjectStep
       | jsonArrayStep
//       | jsonDescendentStep
       ) jsonFilterExpr?)+
    ;

jsonObjectStep
    : '.' ('*' | jsonFieldName)
    ;

// TODO: Check
jsonFieldName
    : identifier
    ;

jsonArrayStep
    : '[' ('*'| jsonArrayStepItem (',' jsonArrayStepItem)*) ']'
    ;

jsonArrayStepItem
    : (jsonArrayIndex (TO jsonArrayIndex)?)
    ;

jsonArrayIndex
    : LAST (('-' | '+') integer)?
    | integer
    ;

jsonFunctionStep
    : '.' jsonItemMethod '(' ')'
    ;

jsonItemMethod
    : ABS
    | AVG
    | BINARY
    | BOOLEAN
    | BOOLEANONLY
    | CEILING
    | COUNT
    | DATE
    | DOUBLE
    | DSINTERVAL
    | FLOAT
    | FLOOR
    | LENGTH
    | LOWER
    | MAXNUMBER
    | MAXSTRING
    | MINNUMBER
    | MINSTRING
    | NUMBER
    | NUMBERONLY
    | SIZE
    | STRING
    | STRINGONLY
    | SUM
    | TIMESTAMP
    | TYPE
    | UPPER
    | YMINTERVAL
    ;

jsonFilterExpr
    : '?' '(' jsonCond ')'
    ;

jsonCond
    : jsonCond '&&' jsonCond                                        #jsonConjunction
    | '(' jsonCond ')'                                              #parenthesisJsonCond
    | jsonRelativePathExpr jsonComparePred (jsonVar | jsonScalar)   #jsonComparison1
    | (jsonVar | jsonScalar) jsonComparePred jsonRelativePathExpr   #jsonComparison2
    | jsonScalar jsonComparePred jsonScalar                         #jsonComparison3
//    : jsonDisjunction
//    | jsonNegation
//    | jsonExistsCond
//    | jsonInCond
//    | jsonLikeCond
//    | jsonLikeRegexCond
//    | jsonEqRegexCond
//    | jsonHasSubstringCond
//    | jsonStartsWithCond
    ;


jsonRelativePathExpr
    : '@' jsonNonfunctionSteps jsonFunctionStep
    ;

jsonComparePred
    : '=='
    | '!='
    | '<'
    | '<='
    | '>='
    | '>'
    ;

jsonVar
    : '$' identifier
    ;

jsonScalar
    : ('+' | '-') TK_INTEGER_WITHOUT_SIGN
    | TK_INTEGER_WITHOUT_SIGN
    | TRUE
    | FALSE
    | NULL
    | identifier
    ;

fullColumnPath
    : identifier ('.' identifier ('.' identifier)?)?
    ;

fullObjectPath
    : identifier ('.' identifier)?
    ;

varrayItem
    : identifier
    ;

lobSegName
    : identifier
    ;

variableName
    : identifier
    ;

hostVariableName
    : identifier
    ;

tablespace
    : identifier
    ;

tablespaceSet
    : identifier
    ;

dblink
    : ( identifier '.' )* identifier
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
    : ( identifier '.' )* identifier
    ;

user
    : identifier
    ;

object
    : identifier
    ;

directoryName
    : identifier
    ;

model
    : identifier
    ;

profile
    : identifier
    ;

policy
    : identifier
    ;

namespace
    : identifier
    ;

username
    : identifier
    ;

savepointName
    : identifier
    ;

password
    : identifier
    ;

oldPassword
    : identifier
    ;

rollbackSegment
    : identifier
    ;

cluster
    : identifier
    ;

restorePointName
    : identifier
    ;

database
    : identifier
    ;

trigger
    : identifier
    ;

sequenceName
    : identifier
    ;

dimensionName
    : identifier
    ;

diskgroupName
    : identifier
    ;

editionName
    : identifier
    ;

flashbackArchiveName
    : identifier
    ;

hierarchyName
    : identifier
    ;

indextype
    : fullObjectPath
    ;

joinGroup
    : identifier
    ;

objectName
    : identifier
    ;

libraryName
    : identifier
    ;

profileName
    : identifier
    ;

zonemapMame
    : identifier
    ;

operatorName
    : identifier
    ;

outlineName
    : identifier
    ;

packageName
    : identifier
    ;

pdbName
    : identifier
    ;

filestoreName
    : identifier
    ;

procedureName
    : ( identifier '.' )* identifier
    ;

partition
    : identifier
    ;

subpartition
    : identifier
    ;

parameterName
    : identifier
    ;

parameterValue
    : identifier
    | stringLiteral
    | numberLiteral
    ;

newHierName
    : identifier
    ;

implementationType
    : identifier
    ;

implementationPackage
    : identifier
    ;

parameterType
    : type
    ;

parameterTypes
    : parameterType ( ',' parameterType )*
    ;

type
    : datatype
    ;

varrayType
    : identifier
    ;

lobItem
    : identifier
    ;

stagingLogName
    : identifier
    ;

zonemapName
    : identifier
    ;

returnType
    : identifier
    ;

primaryOperator
    : identifier
    ;

outline
    : identifier
    ;

newOutlineName
    : identifier
    ;

newCategoryName
    : identifier
    ;

transportSecret
    : identifier
    ;

refColumn
    : identifier
    ;

refAttribute
    : identifier
    ;

scopeTableName
    : identifier
    ;

containerName
    : identifier
    ;

tablespaceGroupName
    : identifier
    ;

domain
    : identifier
    ;

appName
    : identifier
    ;

snapshotName
    : identifier
    ;

maxPdbSnapshots
    : identifier
    ;

clusterId
    : identifier '.' identifier
    ;

featureId
    : identifier '.' identifier
    ;

partitionKeyValue
    : expr
    ;

subpartitionKeyValue
    : expr
    ;

partitionExtendedName
    : PARTITION partition
    | PARTITION FOR '(' partitionKeyValue ( ',' partitionKeyValue )* ')'
    ;

subpartitionExtendedName
    : SUBPARTITION subpartition
    | SUBPARTITION FOR '(' subpartitionKeyValue ( ',' subpartitionKeyValue )* ')'
    ;

partitionExtendedNames
    : ( PARTITION | PARTITIONS )
        partition | ( FOR '(' partitionKeyValue ( ',' partitionKeyValue )* ')' )
        ( ',' partition | ( FOR '(' partitionKeyValue ( ',' partitionKeyValue )* ')' ) )*
    ;


subpartitionExtendedNames
    : ( SUBPARTITION | SUBPARTITIONS )
        subpartition | ( FOR '(' subpartitionKeyValue ( ',' subpartitionKeyValue )* ')' )
        ( ',' subpartition | ( FOR '(' subpartitionKeyValue ( ',' subpartitionKeyValue )* ')' ) )*
    ;

partitionName
    : identifier
    ;

partitionNumber
    : numberLiteral
    ;

partitionOrKeyValue
    : partition
    | FOR '(' partitionKeyValue ( ',' partitionKeyValue )* ')'
    ;

subpartitionOrKeyValue
    : subpartition
    | FOR '(' subpartitionKeyValue ( ',' subpartitionKeyValue )* ')'
    ;

newName
    : identifier
    ;

domainNameOfDirectoryGroup
    : identifier
    ;

serviceName
    : identifier
    ;

collationName
    : identifier
    ;

targetDbName
    : identifier
    ;

dispatcherName
    : identifier
    ;

categoryName
    : identifier
    ;

logGroup
    : identifier
    ;

newTableName
    : identifier
    ;

columnCollationName
    : identifier
    ;

constraintName
    : identifier
    ;

oldName
    : identifier
    ;

anydataColumn
    : column
    ;

validTimeColumn
    : column
    ;

startTimeColumn
    : column
    ;

endTimeColumn
    : column
    ;

collectionItem
    : identifier
    ;

clientId
    : identifier
    ;

newTablespaceName
    : identifier
    ;

triggerName
    : identifier
    ;

unitName
    : identifier
    ;

parameter
    : identifier
    ;

name
    : identifier
    ;

libName
    : identifier
    ;

argument
    : expr
    | expr '=' '>' expr
    ;

externalParameter
    : identifier
    ;

subprogram
    : identifier
    ;

method
    : identifier
    ;

objectType
    : identifier
    ;

dbUserProxy
    : identifier
    ;

containerDataObject
    : identifier
    ;

roleName
    : identifier
    ;

fileName
    : identifier
    ;

filePath
    : identifier
    ;

label
    : identifier
    ;

explicitCursorName
    : identifier
    ;

cursorVariableName
    : identifier
    ;

dbTableOrViewName
    : identifier
    ;

record1
    : identifier
    ;

record2
    : identifier
    ;

exceptionMessage
    : identifier
    ;

characterSet
    : identifier
    ;

connectString
    : stringLiteral
    ;

pathName
    : stringLiteral
    ;

siteName
    : identifier
    ;

failgroupName
    : identifier
    ;

parentEdition
    : identifier
    ;

recordType
    : identifier
    ;

field
    : identifier
    ;

dbTableOrView
    : identifier
    ;

cursor
    : identifier
    ;

cursorVariable
    : identifier
    ;

record
    : identifier
    ;

refCursorType
    : identifier
    ;

subtype
    : identifier
    ;

baseType
    : identifier
    ;

hierarchy
    : identifier
    ;

value
    : expr
    ;

newCollectionVar
    : identifier
    ;

assocArrayType
    : identifier
    ;

nestedTableType
    : identifier
    ;

collectionVar
    : identifier
    ;

lowerBound
    : identifier
    ;

upperBound
    : identifier
    ;

iterator
    : iterandDecl ( ',' iterandDecl )? IN iterationCtlSeq
    ;

iterandDecl
    : plsIdentifier ( MUTABLE | IMMUTABLE )? constrainedType?
    ;

plsIdentifier
    : identifier
    ;

constrainedType
    : identifier
    ;

iterationCtlSeq
    : ( qualIterationCtl (',' qualIterationCtl )? )+
    ;

qualIterationCtl
    : REVERSE? iterationControl predClauseSeq
    ;

iterationControl
    : steppedControl
    | singleExpressionControl
    | valuesOfControl
    | indiciesOfControl
    | pairsOfControl
    | cursorIterationControl
    ;

predClauseSeq
    : ( WHILE expr )? ( WHEN expr )?
    ;

steppedControl
    : lowerBound '.' '.' upperBound ( BY step )?
    ;

singleExpressionControl
    : REPEAT? expr
    ;

valuesOfControl
    : VALUES OF ( expr
                | cursorVariable
                | '(' ( cursorObject | dynamicSql | sqlStatement ) ')'
                )
    ;
dynamicSql
    : EXECUTE IMMEDIATE dynamicSqlStmt ( USING ( IN? ( bindVariable ','? )+ )? )?
    ;

step
    : numberLiteral
    ;

statement
    : ( '<' '<' label '>' '>' )* ( assignmentStatement
                                 | basicLoopStatement
                                 | caseStatement
                                 | closeStatement
                                 | continueStatement
                                 | cursorForLoopStatement
                                 | executeImmediateStatement
                                 | exitStatement
                                 | fetchStatement
                                 | forLoopStatement
                                 | forallStatement
                                 | gotoStatement
                                 | ifStatement
                                 | nullStatement
                                 | openStatement
                                 | openForStatement
                                 | pipeRowStatement
                                 | plsqlBlock
                                 | procedureCall
                                 | raiseStatement
                                 | returnStatement
                                 | selectIntoStatement
                                 | sqlStatement
                                 | whileLoopStatement
                                 )
    ;

procedureCall
    : procedureName ( '(' ( plsqlParameter ( ',' plsqlParameter )* )? ')' )?
    ;

assignmentStatement
    : assignmentStatementTarget ':' '=' plsqlExpression ';'
    ;

assignmentStatementTarget
    : ':'? variable ('.' identifier | '(' identifier ')')?
    | placeholder
    ;

placeholder
    : ':' variable (':' variable)?
    ;

basicLoopStatement
    : LOOP statement* END LOOP label? ';'
    ;

caseStatement
    : CASE caseExpr=plsqlExpression? (WHEN plsqlExpression THEN plsqlExpression ';')+
      (ELSE statement+ ';')? END CASE label? ';'
    ;

closeStatement
    : CLOSE plsqlNamedCursor ';'
    ;

continueStatement
    : CONTINUE label? (WHEN plsqlExpression)?
    ;

cursorForLoopStatement
    : FOR record IN ( cursor '(' plsqlExpression (',' plsqlExpression)* ')'
                    | '(' select ')'
                    )
      LOOP statement+ END LOOP label? ';'
    ;

executeImmediateStatement
    : EXECUTE IMMEDIATE dynamicSqlStmt ( ( plsqlIntoClause
                                         | plsqlBulkCollectIntoClause
                                         ) plsqlUsingClause?
                                       | plsqlUsingClause plsqlDynamicReturningClause?
                                       | plsqlDynamicReturningClause
                                       )?
      ';'
    ;

exitStatement
    : EXIT label? (WHEN plsqlExpression)? ';'
    ;

fetchStatement
    : FETCH ':'? identifier (plsqlIntoClause | plsqlBulkCollectIntoClause (LIMIT plsqlExpression)?) ';'
    ;

forLoopStatement
    : FOR plsqlIterator LOOP statement+ END LOOP label? ';'
    ;

forallStatement
    : FORALL index IN plsqlBoundsClause (SAVE EXCEPTIONS)? (insert | update | delete | merge) ';'
    ;

gotoStatement
    : GOTO label ';'
    ;

ifStatement
    : IF plsqlExpression THEN statement statement* (ELSIF plsqlExpression THEN statement+)* (ELSE statement+)? END IF ';'
    ;

nullStatement
    : NULL ';'
    ;

openStatement
    : OPEN cursor ('(' plsqlExpression (',' plsqlExpression)*)? ';'
    ;

openForStatement
    : OPEN ':'? variable FOR (select | plsqlExpression) plsqlUsingClause? ';'
    ;

pipeRowStatement
    : PIPE ROW '(' row=identifier ')' ';'
    ;

raiseStatement
    : RAISE exceptionMessage? ';'
    ;

returnStatement
    : RETURN plsqlExpression? ';'
    ;

selectIntoStatement
    : SELECT (DISTINCT | UNIQUE | ALL)? selectList (plsqlIntoClause | plsqlBulkCollectIntoClause) 
        FROM ( ( tableReference | THE? '(' subquery ')' ) alias? )*
        whereClause?
        hierarchicalQueryClause?
        groupByClause?
        modelClause?
        windowClause?
    ;

whileLoopStatement
    : WHILE plsqlExpression LOOP statement+ END LOOP label? ';'
    ;

sqlStatement
    : ( select
      | commit
      | collectionMethodCall
      | delete
      | insert
      | lockTable
      | merge
      | rollback
      | savepoint
      | setTransaction
      | update
      ) ';'
    ;

collectionMethodCall
    : identifier '.' ( COUNT 
                     | DELETE ('(' index (',' index)* ')')? 
                     | EXISTS ('(' index ')')? 
                     | EXTEND ('(' index (',' index)* ')')? 
                     | FIRST 
                     | LAST 
                     | LIMIT 
                     | NEXT '(' index ')' 
                     | PRIOR '(' index ')' 
                     | TRIM '(' numberLiteral ')'
                     )
    ;

dynamicSqlStmt
    : expr
    | select
    ;

plsqlUsingClause
    : USING (plsqlUsingClauseItem (',' plsqlUsingClauseItem)*)?
    ;

plsqlUsingClauseItem
    : (IN | OUT | IN OUT)? bindArgument=plsqlExpression
    ;

plsqlBoundsClause
    : lowerBound '.' '.' upperBound
    | INDICES OF collection=identifier (BETWEEN lowerBound AND upperBound)?
    | VALUES OF indexCollection=identifier
    ;

plsqlIterator
    : iterandDecl (',' iterandDecl) IN iterationCtlSeq
    ;

plsqlIntoClause
    : INTO identifier (',' identifier)*
    ;

plsqlBulkCollectIntoClause
    : BULK COLLECT INTO ':'? identifier (',' ':'? identifier)*
    ;

plsqlDynamicReturningClause
    : (RETURN | RETURNING) (plsqlIntoClause | plsqlBulkCollectIntoClause)
    ;

cursorObject
    : identifier
    ;

indiciesOfControl
    : INDICES OF ( expr
                 | cursorVariable
                 | '(' ( cursorObject | dynamicSql | sqlStatement ) ')'
                 )
    ;

pairsOfControl
    : PAIRS OF ( expr
               | cursorVariable
               | '(' ( cursorObject | dynamicSql | sqlStatement ) ')'
               )
    ;

cursorIterationControl
    : '(' ( cursorObject | cursorVariable | dynamicSql | sqlStatement ) ')'
    ;

constant
    : identifier
    ;

variable
    : identifier
    ;

attributeDimension
    : identifier
    ;

level
    : identifier
    ;

primaryName
    : identifier
    ;

matchString
    : identifier
    | stringLiteral
    | '*'
    ;

schemaName
    : identifier
    ;

directoryObjectName
    : identifier
    ;

serverFileName
    : identifier
    ;

sourceChar
    : identifier
    | stringLiteral
    ;

accessDriverType
    : identifier
    ;

directoryObject
    : identifier
    ;

credentialName
    : identifier
    ;

baseProfile
    : identifier
    ;

sourceOutline
    : identifier
    ;

filenamePattern
    : stringLiteral
    ;

cdbName
    : identifier
    ;

adminUserName
    : identifier
    ;

snapshotSCN
    : identifier
    ;

snapshotTimestamp
    : dateTimeLiteral
    ;

keystorePassword
    : identifier
    | stringLiteral
    ;

filename
    : stringLiteral
    ;

newPdbName
    : identifier
    ;

basePdbName
    : identifier
    ;

dblinkname
    : identifier
    ;

mirrorName
    : identifier
    ;

restorePoint
    : identifier
    ;

shardspaceName
    : identifier
    ;

parent
    : identifier
    ;

nestedTableColumn
    : identifier
    ;

noneditioningView
    : identifier
    ;

supertype
    : identifier
    ;

sizeLimit
    : integer
    ;

tablespec
    : identifier ('.' identifier)*
    ;

indexspec
    : identifier
    | '(' ((identifier '.')* identifier)+ ')'
    ;

integer
    : TK_INTEGER_WITHOUT_SIGN
    | ('+' | '-') TK_INTEGER_WITHOUT_SIGN
    ;

literal
    : intervalLiteral
    | numberLiteral
    | stringLiteral
    | dateTimeLiteral
    ;

numberLiteral
    : ('+' | '-') TK_INTEGER_WITHOUT_SIGN
    | ('+' | '-') TK_NUMBER_WITHOUT_SIGN
    | TK_INTEGER_WITHOUT_SIGN
    | TK_NUMBER_WITHOUT_SIGN
    ;

// '…'
// Q'…'
// N'…'
// NQ'…'
stringLiteral
    : TK_SINGLE_QUOTED_STRING
    | TK_QUOTED_STRING
    | TK_NATIONAL_STRING
    ;

dateTimeLiteral
    : DATE TK_SINGLE_QUOTED_STRING         #dateLiteral
    | TIMESTAMP TK_SINGLE_QUOTED_STRING    #timestampLiteral
    ;

intervalLiteral
    : INTERVAL TK_SINGLE_QUOTED_STRING ( YEAR | MONTH ) ('(' precision ')')? ( TO ( YEAR | MONTH ) )?
    | INTERVAL TK_SINGLE_QUOTED_STRING
        ( ( DAY | HOUR | MINUTE ) ( '(' precision ')' )?
        | SECOND ( '(' precision ( ',' fractionalSecondsPrecision )? ')' )?
        )
        ( TO
          ( DAY
          | HOUR
          | MINUTE
          | SECOND ( '(' fractionalSecondsPrecision ')' )?
          )
        )?
    ;

fractionalSecondsPrecision
    : precision
    ;

ddlEvent
    : ALTER
    | ANALYZE
    | ASSOCIATE STATISTICS
    | AUDIT
    | COMMENT
    | CREATE
    | DISASSOCIATE STATISTICS
    | DROP
    | GRANT
    | NOAUDIT
    | RENAME
    | REVOKE
    | TRUNCATE
    | DDL
    ;

databaseEvent
    : AFTER STARTUP
    | BEFORE SHUTDOWN
    | AFTER DB_ROLE_CHANGE
    | AFTER SERVERERROR
    | AFTER LOGON
    | BEFORE LOGOFF
    | AFTER SUSPEND
    | AFTER CLONE
    | BEFORE UNPLUG
    | ( BEFORE | AFTER )? SET CONTAINER
    ;

lockmode
    : ROW SHARE
    | ROW EXCLUSIVE
    | SHARE
    | SHARE UPDATE
    | SHARE ROW EXCLUSIVE
    | EXCLUSIVE
    | WAIT
    | NOWAIT
    ;

functionName
    : ( simpleIdentifier '.' )* simpleIdentifier
    ;

identifier
    : identifierFragment ( '@' identifierFragment )?
    ;

identifierFragment
    : simpleIdentifier
    | specialIdentifier
    | pseudoColumn
    ;

specialIdentifier
    : DBTIMEZONE
    | SESSIONTIMEZONE
    | SYSDATE
    | SYSTIMESTAMP
    | UID
    | USER
    ;

simpleIdentifier
    : TK_IDENTIFIER
    | TK_IDENTIFIER_OR_KEYWORD
    | nonReservedKeywordIdentifier
    ;

pseudoColumn
    : ROWNUM
    | ROWID
    | ORA_ROWSCN
    | COLUMN_VALUE
    | LEVEL
    | CONNECT_BY_ISLEAF
    | CONNECT_BY_ISCYCLE
    | VERSIONS_STARTTIME
    | VERSIONS_STARTSCN
    | VERSIONS_ENDTIME
    | VERSIONS_ENDSCN
    | VERSIONS_XID
    | VERSIONS_OPERATION
    ;

sqlOperation
    : ALTER
    | AUDIT
    | COMMENT
    | DELETE
    | FLASHBACK
    | GRANT
    | INDEX
    | INSERT
    | LOCK
    | RENAME
    | SELECT
    | UPDATE
    | ALTER
    | EXECUTE
    | READ
    ;

nonReservedKeywordIdentifier
    : ABORT
    | ABS
    | ABSENT
    | ACC
    | ACCEPT
    | ACCESSED
    | ACCESSIBLE
    | ACCHK_READ
    | ACCOUNT
    | ACROSS
    | ACTIONS
    | ACTIVATE
    | ACTIVE
    | ADMIN
    | ADMINISTER
    | ADVANCED
    | ADVISE
    | ADVISOR
    | AFFINITY
    | AFTER
    | AGENT
    | AGGREGATE
    | ALGORITHM
    | ALI
    | ALIAS
    | ALL_ROWS
    | ALLOCATE
    | ALLOW
    | ALTERNATE
    | ALWAYS
    | ANALYTIC
    | ANALYZE
    | ANCESTOR
    | ANCILLARY
    | ANOMALY
    | ANY_VALUE
    | ANYDATA
    | ANYDATASET
    | ANYSCHEMA
    | ANYTYPE
    | APP
    | APPEND
    | APPEND_VALUES
    | APPLICATION
    | APPLY
    | APPROX_COUNT
    | APPROX_COUNT_DISTINCT
    | APPROX_COUNT_DISTINCT_AGG
    | APPROX_COUNT_DISTINCT_DETAIL
    | APPROX_MEDIAN
    | APPROX_PERCENTILE
    | APPROX_PERCENTILE_AGG
    | APPROX_PERCENTILE_DETAIL
    | APPROX_RANK
    | APPROX_SUM
    | ARCHIVAL
    | ARCHIVE
    | ARCHIVED
    | ARCHIVELOG
    | ARE
    | ARRAY
    | ASCII
    | ASSEMBLY
    | ASSOCIATE
    | ASYNCHRONOUS
    | AT
    | ATTR
    | ATTRIBUTE
    | ATTRIBUTES
    | AUTHENTICATED
    | AUTHENTICATION
    | AUTHID
    | AUTHORIZATION
    | AUTO
    | AUTO_LOGIN
    | AUTOALLOCATE
    | AUTOEXTEND
    | AUTOLOGIN
    | AUTOMATIC
    | AVAILABILITY
    | AVG
    | BACKINGFILE
    | BACKUP
    | BADFILE
    | BASIC
    | BASICFILE
    | BATCH
    | BECOME
    | BEFORE
    | BEGIN
    | BEGINNING
    | BEQUEATH
    | BFILE
    | BIG
    | BIGFILE
    | BIN_TO_NUM
    | BINARY
    | BINARY_DOUBLE
    | BINARY_FLOAT
    | BINARY_FORMAT
    | BINARY_INTEGER
    | BINDING
    | BIT_AND_AGG
    | BIT_OR_AGG
    | BIT_XOR_AGG
    | BITMAP
    | BLOB
    | BLOCK
    | BLOCKCHAIN
    | BLOCKSIZE
    | BODY
    | BOLD
    | BOOLEAN
    | BOOLEANONLY
    | BOTH
    | BRE
    | BREADTH
    | BREAK
    | BREAKS
    | BROADCAST
    | BTI
    | BTITLE
    | BUFF
    | BUFFER
    | BUFFER_CACHE
    | BUFFER_POOL
    | BUILD
    | BULK
    | BULK_ROWCOUNT
    | BYTE
    | BYTEORDERMARK
    | BYTES
    | CACHE
    | CACHING
    | CALL
    | CANCEL
    | CAPACITY
    | CAPTION
    | CASCADE
    | CASE
    | CASE_SENSITIVE
    | CAST
    | CATEGORY
    | CDB
    | CE
    | CEILING
    | CELL_FLASH_CACHE
    | CENTER
    | CERTIFICATE
    | CHANGE
    | CHANGE_DUPKEY_ERROR_INDEX
    | CHAR_CS
    | CHARACTER
    | CHARACTERS
    | CHARACTERSET
    | CHECKPOINT
    | CHECKSUM
    | CHILD
    | CHR
    | CHUNK
    | CL
    | CLASS
    | CLASSIFICATION
    | CLASSIFIER
    | CLAUSE
    | CLE
    | CLEAN
    | CLEANUP
    | CLEAR
    | CLIENT
    | CLOB
    | CLONE
    | CLOSE
    | CLUSTER_DETAILS
    | CLUSTER_DISTANCE
    | CLUSTER_ID
    | CLUSTER_PROBABILITY
    | CLUSTER_SET
    | CLUSTERING
    | COALESCE
    | COARSE
    | CODE
    | COEFFICIENT
    | COL
    | COLLATION
    | COLLECT
    | COLUMN_VALUE
    | COLUMNS
    | COMMIT
    | COMMITTED
    | COMP
    | COMPACT
    | COMPATIBILITY
    | COMPILE
    | COMPLETE
    | COMPONENT
    | COMPOSITE_LIMIT
    | COMPOUND
    | COMPUTATION
    | COMPUTE
    | COMPUTES
    | CON_ID
    | CON_NAME
    | CONDITION
    | CONDITIONAL
    | CONFIRM
    | CONN
    | CONNECT_BY_ISCYCLE
    | CONNECT_BY_ISLEAF
    | CONNECT_BY_ROOT
    | CONNECT_TIME
    | CONSIDER
    | CONSISTENT
    | CONSTANT
    | CONSTRAINT
    | CONSTRAINTS
    | CONSTRUCTOR
    | CONTAINER
    | CONTAINER_DATA
    | CONTAINER_MAP
    | CONTAINERS
    | CONTAINERS_DEFAULT
    | CONTENT
    | CONTENTS
    | CONTEXT
    | CONTINUE
    | CONTROLFILE
    | CONVERSION
    | CONVERT
    | COPY
    | CORR
    | CORR_K
    | CORR_S
    | CORRUPTION
    | COST
    | COUNT
    | COUNTED
    | COVAR_POP
    | COVAR_SAMP
    | CPU_PER_CALL
    | CPU_PER_SESSION
    | CRE
    | CREATE_FILE_DEST
    | CREATION
    | CREDENTIAL
    | CREDENTIALS
    | CRITICAL
    | CROSS
    | CROSSEDITION
    | CSV
    | CUBE
    | CUBE_TABLE
    | CUME_DIST
    | CURRENT_DATE
    | CURRENT_TIMESTAMP
    | CURRENT_USER
    | CURRVAL
    | CURSOR
    | CURSOR_SHARING_EXACT
    | CYCLE
    | DANGLING
    | DATA
    | DATABASE
    | DATAFILE
    | DATAFILES
    | DATAPUMP
    | DATASTORE
    | DATE_CACHE
    | DATE_FORMAT
    | DAY
    | DAY_TO_SECOND
    | DAYS
    | DB_ROLE_CHANGE
    | DBA_RECYCLEBIN
    | DDL
    | DEALLOCATE
    | DEBUG
    | DEC
    | DECLARE
    | DECREMENT
    | DECRYPT
    | DEDUPLICATE
    | DEF
    | DEFAULT
    | DEFAULT_CREDENTIAL
    | DEFAULT_PDB_HINT
    | DEFAULTIF
    | DEFAULTS
    | DEFERRABLE
    | DEFERRED
    | DEFINE
    | DEFINER
    | DEFINITION
    | DEL
    | DELAY
    | DELEGATE
    | DELETE_ALL
    | DELETING
    | DELIMITED
    | DEMAND
    | DENSE_RANK
    | DEPENDENT
    | DEPTH
    | DEQUEUE
    | DESCRIBE
    | DESCRIPTION
    | DETECTED
    | DETERMINES
    | DETERMINISTIC
    | DIAGNOSTICS
    | DICTIONARY
    | DIGEST
    | DIMENSION
    | DIRECT_LOAD
    | DIRECT_PATH
    | DIRECTIO
    | DIRECTORY
    | DISABLE
    | DISABLE_ALL
    | DISABLE_DIRECTORY_LINK_CHECK
    | DISABLE_PARALLEL_DML
    | DISALLOW
    | DISASSOCIATE
    | DISC
    | DISCARD
    | DISCARDFILE
    | DISCONNECT
    | DISK
    | DISKGROUP
    | DISKS
    | DISMOUNT
    | DISTINGUISHED
    | DISTRIBUTE
    | DISTRIBUTED
    | DML
    | DNFS_DISABLE
    | DNFS_ENABLE
    | DNFS_READBUFFERS
    | DOCUMENT
    | DOUBLE
    | DOWNGRADE
    | DRIVING_SITE
    | DSINTERVAL
    | DUPLICATE
    | DUPLICATED
    | DV
    | DYNAMIC
    | DYNAMIC_SAMPLING
    | EACH
    | ED
    | EDIT
    | EDITION
    | EDITIONABLE
    | EDITIONING
    | EDITIONS
    | ELEMENT
    | ELSIF
    | EM
    | EMBEDDED
    | ENABLE
    | ENABLE_ALL
    | ENABLE_PARALLEL_DML
    | ENCLOSED
    | ENCODING
    | ENCRYPT
    | ENCRYPTION
    | END
    | ENDIAN
    | ENFORCED
    | ENQUEUE
    | ENTERPRISE
    | ENTITYESCAPING
    | EQUALS_PATH
    | ERR
    | ERROR
    | ERRORS
    | ESCAPE
    | EVALNAME
    | EVALUATE
    | EVALUATION
    | EVERY
    | EXCEPT
    | EXCEPTION
    | EXCEPTIONS
    | EXCHANGE
    | EXCLUDE
    | EXCLUDING
    | EXEC
    | EXECUTE
    | EXEMPT
    | EXISTING
    | EXIT
    | EXPIRE
    | EXPLAIN
    | EXPORT
    | EXPRESS
    | EXTEND
    | EXTENDED
    | EXTENT
    | EXTERNAL
    | EXTERNALLY
    | EXTRA
    | EXTRACT
    | FACT
    | FAILED
    | FAILED_LOGIN_ATTEMPTS
    | FAILGROUP
    | FAILOVER
    | FAILURE
    | FALSE
    | FAR
    | FAST
    | FEATURE
    | FEATURE_COMPARE
    | FEATURE_DETAILS
    | FEATURE_ID
    | FEATURE_SET
    | FEATURE_VALUE
    | FETCH
    | FIELD
    | FIELD_NAMES
    | FIELDS
    | FIEST
    | FILE_NAME_CONVERT
    | FILEGROUP
    | FILES
    | FILESTORE
    | FILESYSTEM_LIKE_LOGGING
    | FILTER
    | FINAL
    | FINE
    | FINISH
    | FIRST
    | FIRST_ROWS
    | FIRST_VALUE
    | FIXED
    | FLASH_CACHE
    | FLASHBACK
    | FLEX
    | FLOOR
    | FLUSH
    | FOLDER
    | FOLLOWING
    | FOLLOWS
    | FORALL
    | FORCE
    | FOREIGN
    | FORMAT
    | FORWARD
    | FOUND
    | FREELIST
    | FREELISTS
    | FREEPOOLS
    | FRESH
    | FRESH_MV
    | FTP
    | FULL
    | FUNCTION
    | GATHER_OPTIMIZER_STATISTICS
    | GENERATED
    | GET
    | GLOBAL
    | GLOBAL_NAME
    | GLOBAL_TOPIC_ENABLED
    | GLOBALLY
    | GOTO
    | GRANTED
    | GROUP_ID
    | GROUPING
    | GROUPING_ID
    | GROUPS
    | GUARANTEE
    | GUARD
    | HALF_YEARS
    | HASH
    | HASHING
    | HASHKEYS
    | HEAP
    | HELP
    | HIDE
    | HIER_ANCESTOR
    | HIER_CAPTION
    | HIER_DEPTH
    | HIER_DESCRIPTION
    | HIER_LAG
    | HIER_LEAD
    | HIER_LEVEL
    | HIER_MEMBER_NAME
    | HIER_MEMBER_UNIQUE_NAME
    | HIER_ORDER
    | HIER_PARENT
    | HIERARCHY
    | HIGH
    | HINT
    | HIST
    | HISTORY
    | HO
    | HOST
    | HOUR
    | HOURS
    | HTTP
    | ID
    | IDENTIFIER
    | IDENTITY
    | IDLE
    | IDLE_TIME
    | IF
    | IGNORE
    | IGNORE_CHARS_AFTER_EOR
    | IGNORE_ROW_ON_DUPKEY_INDEX
    | ILM
    | IMMUTABLE
    | IMPORT
    | INACTIVE_ACCOUNT_TIME
    | INCLUDE
    | INCLUDING
    | INDENT
    | INDEX_ASC
    | INDEX_COMBINE
    | INDEX_DESC
    | INDEX_FFS
    | INDEX_JOIN
    | INDEX_SS
    | INDEX_SS_ASC
    | INDEX_SS_DESC
    | INDEXES
    | INDEXING
    | INDEXTYPE
    | INDICATOR
    | INDICES
    | INFINITE
    | INHERIT
    | INITIALIZED
    | INITIALLY
    | INITRANS
    | INMEMORY
    | INMEMORY_PRUNING
    | INNER
    | INPUT
    | INSERTING
    | INSTALL
    | INSTANCE
    | INSTANCES
    | INSTANTIABLE
    | INSTEAD
    | INT
    | INTERLEAVED
    | INTERNAL
    | INTERVAL
    | INVALIDATE
    | INVALIDATION
    | INVISIBLE
    | IO_OPTIONS
    | IS_LEAF
    | ISOLATE
    | ISOLATION
    | ISOPEN
    | ITERATE
    | ITERATION_NUMBER
    | JAVA
    | JOB
    | JOIN
    | JSON
    | JSON_ARRAY
    | JSON_ARRAYAGG
    | JSON_EQUAL
    | JSON_EXISTS
    | JSON_MERGEPATCH
    | JSON_OBJECT
    | JSON_OBJECTAGG
    | JSON_QUERY
    | JSON_SCALAR
    | JSON_SERIALIZE
    | JSON_TABLE
    | JSON_TEXTCONTAINS
    | JSON_TRANSFORM
    | JSON_VALUE
    | KEEP
    | KEEP_DUPLICATES
    | KEY
    | KEYS
    | KEYSTORE
    | KILL
    | KURTOSIS_POP
    | KURTOSIS_SAMP
    | KW_A
    | KW_C
    | KW_D
    | KW_E
    | KW_EMPTY
    | KW_G
    | KW_H
    | KW_I
    | KW_K
    | KW_L
    | KW_M
    | KW_P
    | KW_R
    | KW_S
    | KW_SKIP
    | KW_T
    | KW_X
    | LAB
    | LABEL
    | LAG
    | LAG_DIFF
    | LAG_DIFF_PERCENT
    | LANGUAGE
    | LAST
    | LAST_VALUE
    | LATERAL
    | LAX
    | LDRTRIM
    | LE
    | LEAD
    | LEAD_CDB
    | LEAD_CDB_URI
    | LEAD_DIFF
    | LEAD_DIFF_PERCENT
    | LEADING
    | LEAF
    | LEFT
    | LENGTH
    | LESS
    | LEVEL_NAME
    | LEVELS
    | LIBRARY
    | LIKE2
    | LIKE4
    | LIKEC
    | LIMIT
    | LINEAR
    | LINK
    | LIST
    | LISTAGG
    | LITTLE
    | LLS
    | LNNVL
    | LOAD
    | LOB
    | LOBS
    | LOCAL
    | LOCALTIMESTAMP
    | LOCATION
    | LOCATOR
    | LOCKDOWN
    | LOCKED
    | LOCKING
    | LOG
    | LOGFILE
    | LOGFILES
    | LOGGING
    | LOGICAL
    | LOGICAL_READS_PER_CALL
    | LOGICAL_READS_PER_SESSION
    | LOGMINING
    | LOGOFF
    | LOGON
    | LOOP
    | LOST
    | LOW
    | LOW_COST_TBS
    | LOWER
    | LRTRIM
    | LTRIM
    | MAIN
    | MANAGE
    | MANAGED
    | MANAGEMENT
    | MANAGER
    | MANDATORY
    | MANUAL
    | MAP
    | MAPPING
    | MASK
    | MASTER
    | MATCH
    | MATCH_NUMBER
    | MATCH_RECOGNIZE
    | MATCHED
    | MATERIALIZE
    | MATERIALIZED
    | MAX
    | MAX_AUDIT_SIZE
    | MAX_DIAG_SIZE
    | MAX_ERROR
    | MAXDATAFILES
    | MAXIMIZE
    | MAXINSTANCES
    | MAXLOGFILES
    | MAXLOGHISTORY
    | MAXLOGMEMBERS
    | MAXNUMBER
    | MAXSIZE
    | MAXSTRING
    | MAXVALUE
    | MEASURE
    | MEASURES
    | MEDIAN
    | MEDIUM
    | MEMBER
    | MEMBER_CAPTION
    | MEMBER_DESCRIPTION
    | MEMBER_NAME
    | MEMBER_UNIQUE_NAME
    | MEMCOMPRESS
    | MEMOPTIMIZE
    | MEMORY
    | MERGE
    | METADATA
    | MIGRATE
    | MIGRATION
    | MIN
    | MINALUE
    | MINEXTENTS
    | MINIMIZE
    | MINIMUM
    | MINING
    | MINNUMBER
    | MINSTRING
    | MINUTE
    | MINUTES
    | MINVALUE
    | MIRROR
    | MISMATCH
    | MISSING
    | MLE
    | MODEL
    | MODEL_MIN_ANALYSIS
    | MODIFICATION
    | MONITOR
    | MONITORING
    | MONTH
    | MONTHS
    | MOUNT
    | MOUNTPATH
    | MOUNTPOINT
    | MOVE
    | MOVEMENT
    | MULTISET
    | MULTIVALUE
    | MUTABLE
    | NAME
    | NAMED
    | NAMESPACE
    | NAN
    | NATIONAL
    | NATIVE_FULL_OUTER_JOIN
    | NATURAL
    | NAV
    | NCHAR
    | NCHAR_CS
    | NCLOB
    | NESTED
    | NETWORK
    | NEVER
    | NEW
    | NEWLINE
    | NEXT
    | NEXTVAL
    | NO
    | NO_CLUSTERING
    | NO_EXPAND
    | NO_FACT
    | NO_GATHER_OPTIMIZER_STATISTICS
    | NO_INDEX
    | NO_INDEX_FFS
    | NO_INDEX_SS
    | NO_INMEMORY
    | NO_INMEMORY_PRUNING
    | NO_MERGE
    | NO_MONITOR
    | NO_NATIVE_FULL_OUTER_JOIN
    | NO_PARALLEL
    | NO_PARALLEL_INDEX
    | NO_PQ_CONCURRENT_UNION
    | NO_PQ_SKEW
    | NO_PUSH_PRED
    | NO_PUSH_SUBQ
    | NO_PX_JOIN_FILTER
    | NO_QUERY_TRANSFORMATION
    | NO_RESULT_CACHE
    | NO_REWRITE
    | NO_STAR_TRANSFORMATION
    | NO_STATEMENT_QUEUING
    | NO_UNNEST
    | NO_USE_BAND
    | NO_USE_CUBE
    | NO_USE_HASH
    | NO_USE_MERGE
    | NO_USE_NL
    | NO_XML_QUERY_REWRITE
    | NO_XMLINDEX_REWRITE
    | NO_ZONEMAP
    | NOAPPEND
    | NOARCHIVELOG
    | NOBADFILE
    | NOCACHE
    | NOCHECK
    | NOCOPY
    | NOCYCLE
    | NODELAY
    | NODIRECTIO
    | NODISCARDFILE
    | NOEDITIONABLE
    | NOENTITYESCAPING
    | NOEXTEND
    | NOFORCE
    | NOGUARANTEE
    | NOKEEP
    | NOLIST
    | NOLOGFILE
    | NOLOGGING
    | NOMAPPING
    | NOMAXVALUE
    | NOMINIMIZE
    | NOMINVALUE
    | NOMONITORING
    | NOMOUNT
    | NON
    | NONE
    | NONEDITIONABLE
    | NONSCHEMA
    | NONULLIF
    | NOORDER
    | NOPARALLEL
    | NOPR
    | NOPROMPT
    | NORELOCATE
    | NORELY
    | NOREPAIR
    | NOREPLAY
    | NORESETLOGS
    | NOREVERSE
    | NORMAL
    | NOROWDEPENDENCIES
    | NOSCALE
    | NOSCHEMACHECK
    | NOSHARD
    | NOSORT
    | NOSWITCH
    | NOTFOUND
    | NOTHING
    | NOTIFICATION
    | NOTIMEOUT
    | NOTRIM
    | NOVALIDATE
    | NTH_VALUE
    | NTILE
    | NULLIF
    | NULLS
    | NUMBER
    | NUM
    | NUMBERONLY
    | NUMERIC
    | NVARCHAR2
    | OBJECT
    | OFF
    | OFFSET
    | OID
    | OIDINDEX
    | OLD
    | OLS
    | OLTP
    | ONE
    | ONE_SIDED_SIG
    | ONE_SIDED_SIG_NEG
    | ONE_SIDED_SIG_POS
    | ONLY
    | OPAQUE
    | OPEN
    | OPERATOR
    | OPT_PARAM
    | OPTIMAL
    | OPTIMIZE
    | OPTIONALLY
    | ORA_DM_PARTITION_NAME
    | ORA_INVOKING_USER
    | ORA_INVOKING_USERID
    | ORA_ROWSCN
    | ORACLE_DATE
    | ORACLE_NUMBER
    | ORDERED
    | ORDINALITY
    | ORGANIZATION
    | OSERROR
    | OTHER
    | OTHERS
    | OUT
    | OUTER
    | OUTLINE
    | OVER
    | OVERFLOW
    | OVERRIDE
    | OVERRIDING
    | OWNER
    | OWNERSHIP
    | PACKAGE
    | PAGE
    | PAIRS
    | PARALLEL
    | PARALLEL_ENABLE
    | PARALLEL_INDEX
    | PARAMETERS
    | PARENT
    | PARENT_LEVEL_NAME
    | PARENT_UNIQUE_NAME
    | PARITY
    | PARTIAL
    | PARTITION
    | PARTITIONING
    | PARTITIONS
    | PARTITIONSET
    | PASSING
    | PASSW
    | PASSWORD
    | PASSWORD_GRACE_TIME
    | PASSWORD_LIFE_TIME
    | PASSWORD_LOCK_TIME
    | PASSWORD_REUSE_MAX
    | PASSWORD_REUSE_TIME
    | PASSWORD_ROLLOVER_TIME
    | PASSWORD_VERIFY_FUNCTION
    | PASSWORDFILE_METADATA_CACHE
    | PAST
    | PATCH
    | PATH
    | PATH_PREFIX
    | PATTERN
    | PAU
    | PAUSE
    | PCTINCREASE
    | PCTTHRESHOLD
    | PCTUSED
    | PCTVERSION
    | PER
    | PERCENT
    | PERCENT_RANK
    | PERCENTILE_CONT
    | PERCENTILE_DISC
    | PERFORMANCE
    | PERIOD
    | PERMANENT
    | PERMISSION
    | PERMUTE
    | PERSISTABLE
    | PFILE
    | PHYSICAL
    | PIPE
    | PIPELINED
    | PIVOT
    | PLAN
    | PLS_INTEGER
    | PLUGGABLE
    | PMEM
    | POINT
    | POLICY
    | POLYMORPHIC
    | PORT
    | POSITION
    | POST_TRANSACTION
    | POWER
    | PQ_CONCURRENT_UNION
    | PQ_DISTRIBUTE
    | PQ_FILTER
    | PQ_SKEW
    | PRAGMA
    | PREBUILT
    | PRECEDES
    | PRECEDING
    | PRECISION
    | PREDICTION
    | PREDICTION_BOUNDS
    | PREDICTION_COST
    | PREDICTION_DETAILS
    | PREDICTION_PROBABILITY
    | PREDICTION_SET
    | PREPARE
    | PREPROCESSOR
    | PREPROCESSOR_TIMEOUT
    | PRESENT
    | PRESERVE
    | PRETTY
    | PREV
    | PRIMARY
    | PRINT
    | PRIORITY
    | PRIVATE
    | PRIVATE_SGA
    | PRIVILEGE
    | PRIVILEGES
    | PRO
    | PROCEDURAL
    | PROCEDURE
    | PROCESS
    | PROFILE
    | PROGRAM
    | PROJECT
    | PROMPT
    | PROPERTY
    | PROTECTION
    | PROTOCOL
    | PROXY
    | PRUNING
    | PUBLIC
    | PURGE
    | PUSH_PRED
    | PUSH_SUBQ
    | PX_JOIN_FILTER
    | QB_NAME
    | QUALIFY
    | QUARTERS
    | QUERY
    | QUEUE
    | QUIESCE
    | QUIET
    | QUIT
    | QUORUM
    | QUOTA
    | QUOTAGROUP
    | RAISE
    | RANDOM
    | RANDOM_LOCAL
    | RANGE
    | RANK
    | RATIO_TO_REPORT
    | READ
    | READS
    | READSIZE
    | REAL
    | REBALANCE
    | REBUILD
    | RECORD
    | RECORDS
    | RECORDS_PER_BLOCK
    | RECOVER
    | RECOVERY
    | RECYCLE
    | RECYCLEBIN
    | REDACTION
    | REDEFINE
    | REDO
    | REDUCED
    | REDUNDANCY
    | REF
    | REFERENCE
    | REFERENCED
    | REFERENCES
    | REFERENCING
    | REFRESH
    | REGEXP_LIKE
    | REGISTER
    | REGR_AVGX
    | REGR_AVGY
    | REGR_COUNT
    | REGR_INTERCEPT
    | REGR_R2
    | REGR_SLOPE
    | REGR_SXX
    | REGR_SXY
    | REGR_SYY
    | REGULAR
    | REJECT
    | REKEY
    | RELATIONAL
    | RELOCATE
    | RELY
    | REMOTE
    | REMOVE
    | REP
    | REPAIR
    | REPEAT
    | REPF
    | REPFOOTER
    | REPH
    | REPHEADER
    | REPLACE
    | REPLICATION
    | REQUIRED
    | RESET
    | RESETLOGS
    | RESIZE
    | RESOLVE
    | RESOLVER
    | RESPECT
    | RESTART
    | RESTORE
    | RESTRICT
    | RESTRICT_REFERENCES
    | RESTRICTED
    | RESULT
    | RESULT_CACHE
    | RESUMABLE
    | RESUME
    | RETENTION
    | RETRY_ON_ROW_CHANGE
    | RETURN
    | RETURNING
    | REUSE
    | REVERSE
    | REWRITE
    | RIGHT
    | RNDS
    | RNPS
    | ROLE
    | ROLES
    | ROLLBACK
    | ROLLING
    | ROLLUP
    | ROOT
    | ROW_NUMBER
    | ROWCOUNT
    | ROWDEPENDENCIES
    | ROWTYPE
    | RTRIM
    | RULE
    | RULES
    | RUN
    | RUNNING
    | SALT
    | SAMPLE
    | SAV
    | SAVE
    | SAVEPOINT
    | SCALAR
    | SCALARS
    | SCALE
    | SCAN
    | SCHEDULER
    | SCHEMA
    | SCHEMACHECK
    | SCN
    | SCOPE
    | SCR
    | SCREEN
    | SCRUB
    | SDO_GEOMETRY
    | SDO_GEORASTER
    | SDO_TOPO_GEOMETRY
    | SEARCH
    | SECOND
    | SECONDS
    | SECRET
    | SECUREFILE
    | SEED
    | SEGMENT
    | SELF
    | SEQUENCE
    | SEQUENTIAL
    | SERIAL
    | SERIALIZABLE
    | SERVERERROR
    | SERVICE
    | SERVICE_NAME_CONVERT
    | SESSIONS_PER_USER
    | SETS
    | SETTINGS
    | SHA2_512
    | SHARD
    | SHARDED
    | SHARDS
    | SHARDSPACE
    | SHARE_OF
    | SHARED
    | SHARED_POOL
    | SHARING
    | SHO
    | SHOW
    | SHRINK
    | SHUTDOWN
    | SIBLINGS
    | SID
    | SINGLE
    | SITE
    | SIZES
    | SKEWNESS_POP
    | SKEWNESS_SAMP
    | SMALLFILE
    | SNAPSHOT
    | SOME
    | SORT
    | SOURCE
    | SOURCE_FILE_DIRECTORY
    | SOURCE_FILE_NAME_CONVERT
    | SPACE
    | SPATIAL
    | SPFILE
    | SPLIT
    | SPO
    | SPOOL
    | SQL
    | SQL_MACRO
    | SQLERROR
    | STA
    | STANDALONE
    | STANDARD
    | STANDBY
    | STANDBYS
    | STAR_TRANSFORMATION
    | STARTUP
    | STATE
    | STATEMENT
    | STATEMENT_ID
    | STATEMENT_QUEUING
    | STATEMENTS
    | STATIC
    | STATISTICS
    | STATS_BINOMIAL_TEST
    | STATS_CROSSTAB
    | STATS_F_TEST
    | STATS_KS_TEST
    | STATS_MODE
    | STATS_MW_TEST
    | STATS_ONE_WAY_ANOVA
    | STATS_T_TEST_INDEP
    | STATS_T_TEST_INDEPU
    | STATS_T_TEST_ONE
    | STATS_T_TEST_PAIRED
    | STATS_WSR_TEST
    | STDDEV
    | STDDEV_POP
    | STDDEV_SAMP
    | STOP
    | STORAGE
    | STORE
    | STRICT
    | STRING
    | STRINGONLY
    | STRIPE_COLUMNS
    | STRIPE_WIDTH
    | SUBMULTISET
    | SUBPARTITION
    | SUBPARTITIONS
    | SUBSET
    | SUBSTITUTABLE
    | SUBTYPE
    | SUCCESS
    | SUM
    | SUPPLEMENTAL
    | SUSPEND
    | SWITCH
    | SWITCHOVER
    | SYNC
    | SYNCHRONOUS
    | SYS
    | SYS_DBURIGEN
    | SYS_OP_ZONE_ID
    | SYS_XMLAGG
    | SYSAUX
    | SYSBACKUP
    | SYSDBA
    | SYSDG
    | SYSGUID
    | SYSKM
    | SYSOPER
    | SYSRAC
    | SYSTEM
    | TAB
    | TABLES
    | TABLESPACE
    | TAG
    | TARGET
    | TEMP
    | TEMPFILE
    | TEMPLATE
    | TEMPORARY
    | TERMINATED
    | TERRITORY
    | TEST
    | TEXT
    | THAN
    | THE
    | THESE
    | THREAD
    | THROUGH
    | TIER
    | TIES
    | TIME
    | TIME_ZONE
    | TIMEOUT
    | TIMESTAMP
    | TIMEZONE
    | TIMEZONE_ABBR
    | TIMEZONE_HOUR
    | TIMEZONE_MINUTE
    | TIMEZONE_REGION
    | TIMI
    | TIMING
    | TO_APPROX_COUNT_DISTINCT
    | TO_APPROX_PERCENTILE
    | TO_BINARY_DOUBLE
    | TO_BINARY_FLOAT
    | TO_DATE
    | TO_DSINTERVAL
    | TO_NUMBER
    | TO_TIMESTAMP
    | TO_TIMESTAMP_TZ
    | TO_YMINTERVAL
    | TOPLEVEL
    | TRACE
    | TRACING
    | TRACKING
    | TRAILING
    | TRANSACTION
    | TRANSACTIONAL
    | TRANSFORM
    | TRANSLATE
    | TRANSLATION
    | TREAT
    | TRIGGERS
    | TRIM
    | TRUE
    | TRUNCATE
    | TRUST
    | TRUSTED
    | TTI
    | TTITLE
    | TUNING
    | TWO_SIDED_SIG
    | TYPE
    | TYPENAME
    | TZ_OFFSET
    | UNARCHIVED
    | UNBOUNDED
    | UNCONDITIONAL
    | UNDEF
    | UNDEFINE
    | UNDER
    | UNDER_PATH
    | UNDO
    | UNDROP
    | UNIFORM
    | UNINSTALL
    | UNITE
    | UNLIMITED
    | UNLOCK
    | UNNEST
    | UNPACKED
    | UNPIVOT
    | UNPLUG
    | UNPROTECTED
    | UNQUIESCE
    | UNRECOVERABLE
    | UNSIGNED
    | UNTIL
    | UNUSABLE
    | UNUSED
    | UPDATED
    | UPDATING
    | UPGRADE
    | UPPER
    | UPSERT
    | URITYPE
    | UROWID
    | USABLE
    | USAGE
    | USE
    | USE_BAND
    | USE_CONCAT
    | USE_CUBE
    | USE_HASH
    | USE_MERGE
    | USE_NL
    | USE_NL_WITH_INDEX
    | USE_STORED_OUTLINES
    | USER_DATA
    | USER_TABLESPACES
    | USERGROUP
    | USERS
    | USING
    | USING_NLS_COMP
    | V1
    | VALIDATE_CONVERSION
    | VALIDATION
    | VALUE
    | VAR
    | VAR_POP
    | VAR_SAMP
    | VARCHARC
    | VARIABLE
    | VARIANCE
    | VARRAW
    | VARRAWC
    | VARRAY
    | VARRAYS
    | VARYING
    | VERIFY
    | VERSION
    | VERSIONS
    | VERSIONS_ENDSCN
    | VERSIONS_ENDTIME
    | VERSIONS_OPERATION
    | VERSIONS_STARTSCN
    | VERSIONS_STARTTIME
    | VERSIONS_XID
    | VIRTUAL
    | VISIBILITY
    | VISIBLE
    | VOLUME
    | WAIT
    | WALLET
    | WARN
    | WARNING
    | WEEKS
    | WELLFORMED
    | WHEN
    | WHILE
    | WHITESPACE
    | WINDOW
    | WITHIN
    | WITHOUT
    | WNDS
    | WNPS
    | WORK
    | WRAPPER
    | WRITE
    | XDB
    | XML
    | XMLAGG
    | XMLATTRIBUTES
    | XMLCAST
    | XMLCDATA
    | XMLCOLATTVAL
    | XMLELEMENT
    | XMLEXISTS
    | XMLFOREST
    | XMLINDEX
    | XMLNAMESPACES
    | XMLPARSE
    | XMLPI
    | XMLQUERY
    | XMLROOT
    | XMLSCHEMA
    | XMLSEQUENCE
    | XMLSERIALIZE
    | XMLTABLE
    | XMLTAG
    | XMLTYPE
    | XQUERY
    | XS
    | YEAR
    | YEAR_TO_MONTH
    | YEARS
    | YES
    | YMINTERVAL
    | ZONE
    | ZONED
    | ZONEMAP
    ;