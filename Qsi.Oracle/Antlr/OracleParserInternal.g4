parser grammar OracleParserInternal;

options { 
    tokenVocab=OracleLexerInternal;
}

root
    : EOF
    | (oracleStatement (SEMICOLON_SYMBOL EOF? | EOF))+
    ;

oracleStatement
    : select
    | delete
    | create
    | alter
    | drop
    | grant
    | insert
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

grant
    : GRANT ( (grantSystemPrivileges | grantObjectPrivileges) (CONTAINER '=' (CURRENT | ALL))?
            | grantRolesToPrograms
            )
    ;

create
    : createAnalyticView
    | createAttributeDimension
    | createAuditPolicy
//    | createCluster
//    | createContext
//    | createControlfile
    | createDatabase
//    | createDatabaseLink
//    | createDimension
//    | createDirectory
//    | createDiskgroup
//    | createEdition
//    | createFlashbackArchive
//    | createFunction
//    | createHierarchy
    | createIndex
//    | createIndextype
//    | createInmemoryJoinGroup
//    | createJava
//    | createLibrary
//    | createLockdownProfile
//    | createMaterializedView
//    | createMaterializedViewLog
//    | createMaterializedZonemap
//    | createOperator
//    | createOutLine
//    | createPackage
//    | createPackageBody
//    | createPfile
//    | createPluggableDatabase
//    | createPmemFileStore
//    | createProcedure
//    | createProfile
//    | createRestorePoint
//    | createRole
//    | createRollbackSegment
    | createSchema
//    | createSequence
//    | createSpfile
    | createSynonym
    | createTable
//    | createTablespace
//    | createTablespaceSet
//    | createTrigger
//    | createType
//    | createTypeBody
//    | createUser
    | createView
    ;

alter
    : alterAnalyticView
    | alterAttributeDimension
    | alterAuditPolicy
//    | alterCluster
    | alterDatabase
//    | alterDatabaseDictionary
//    | alterDatabaseLink
//    | alterDimension
//    | alterDiskgroup
//    | alterFlashbackArchive
//    | alterFunction
//    | alterHierarchy
    | alterIndex
//    | alterIndextype
//    | alterInmemoryJoinGroup
//    | alterJava
//    | alterLibrary
//    | alterLockdownProfile
//    | alterMaterializedView
//    | alterMaterializedViewLog
//    | alterMaterializedZonemap
//    | alterOperator
//    | alterOutline
//    | alterPackage
//    | alterPluggableDatabase
//    | alterPmemFilestore
//    | alterProcedure
//    | alterProfile
//    | alterResourceCost
//    | alterRole
//    | alterRollbackSegment
//    | alterSequence
//    | alterSession
    | alterSynonym
//    | alterSystem

// TODO: Impl

//    | alterTable
//    | alterTablespace
//    | alterTablespaceSet
//    | alterTrigger
//    | alterType
//    | alterUser
    | alterView
    ;

drop
    : dropAnalyticView
    | dropAttributeDimension
    | dropAuditPolicy
//    | dropCluster
//    | dropContext
    | dropDatabase
//    | dropDatabaseLink
//    | dropDimension
//    | dropDirectory
//    | dropDiskgroup
//    | dropEdition
//    | dropFlashbackArchive
//    | dropFunction
//    | dropHierarchy
    | dropIndex
//    | dropIndextype
//    | dropInmemoryJoinGroup
//    | dropJava
//    | dropLibrary
//    | dropLockdownProfile
//    | dropMaterializedView
//    | dropMaterializedViewLog
//    | dropMaterializedZonemap
//    | dropOperator
//    | dropOutline
//    | dropPackage
//    | dropPluggableDatabase
//    | dropPmemFilestore
//    | dropProcedure
//    | dropProfile
//    | dropRestorePoint
//    | dropRole
//    | dropRollbackSegment
//    | dropSequence
    | dropSynonym
    | dropTable
//    | dropTablespace
//    | dropTablespaceSet
//    | dropTrigger
//    | dropType
//    | dropTypeBody
//    | dropUser
    | dropView
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
    : CREATE AUDIT POLICY policy=identifier
      privilegeAuditClause?
      actionAuditClause?
      roleAuditClause?
      (WHEN S_SINGLE_QUOTE auditCondition S_SINGLE_QUOTE
       EVALUATE PER (STATEMENT | SESSION | INSTANCE)
      )?
      (ONLY TOPLEVEL)?
      (CONTAINER '=' (ALL | CURRENT))?
    ;

alterAuditPolicy
    : ALTER AUDIT POLICY policy=identifier 
      ADD?
      ( (privilegeAuditClause? actionAuditClause? roleAuditClause?)
      | (ONLY TOPLEVEL)?
      )
      DROP?
      ( (privilegeAuditClause? actionAuditClause? roleAuditClause?)
      | (ONLY TOPLEVEL)?
      )
      (CONDITION ( DROP
                 | S_SINGLE_QUOTE auditCondition S_SINGLE_QUOTE EVALUATE PER (STATEMENT | SESSION | INSTANCE)
                 )
      )?
    ;

dropAuditPolicy
    : DROP AUDIT POLICY policy=identifier
    ;

createDatabase
    : CREATE DATABASE database=identifier?
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

createSchema
    : CREATE SCHEMA AUTHORIZATION schema 
      (createTable | createView | grant)+
    ;

createTable
    : CREATE 
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
    : CREATE (UNIQUE | BITMAP | MULTIVALUE)? INDEX (schema '.')? indexName=identifier
      indexIlmClause? ON (clusterIndexClause | tableIndexClause | bitmapJoinIndexClause)
      (USABLE | UNUSABLE)? ((DEFERRED | IMMEDIATE) INVALIDATION)?
    ;

alterIndex
    : ALTER INDEX (schema '.')? indexName=identifier indexIlmClause? 
      ( ( deallocateUnusedClause
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
     | RENAME TO newName=identifier 
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
    | (schema '.')? view (SHARING '=' (METADATA | DATA | EXTENDED DATA | NONE))?
      ( '(' createViewConstraintItem (',' createViewConstraintItem)* ')'
      | objectViewClause
      | xmlTypeViewClause
      )?
      (DEFAULT COLLATION collationName=identifier)?
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

dropView
    : DROP VIEW (schema '.')? view (CASCADE CONSTRAINTS)?
    ;

createSynonym
    : CREATE (OR REPLACE)? (EDITIONABLE | NONEDITIONABLE)? (PUBLIC)? SYNONYM (schema '.')? synonym=identifier
      (SHARING '=' (METADATA | NONE))? FOR (schema '.')? object=identifier ('@' dblink)?
    ;

alterSynonym
    : ALTER PUBLIC? SYNONYM (schema '.')? synonym=identifier (EDITIONABLE | NONEDITIONABLE | COMPILE)
    ;

dropSynonym
    : DROP PUBLIC? SYNONYM (schema '.')? synonym=identifier FORCE?
    ;

deallocateUnusedClause
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
    : REBUILD ( PARTITION partition=identifier 
              | SUBPARTITION subpartition=identifier 
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
    : MODIFY DEFAULT ATTRIBUTES (FOR PARTITION partition=identifier)? (physicalAttributesClause | TABLESPACE (tablespace | DEFAULT) | loggingClause)*
    ;

addHashIndexPartition
    : ADD PARTITION partitionName=identifier? (TABLESPACE tablespaceName=identifier)? indexCompression? parallelClause?
    ;

modifyIndexPartition
    : MODIFY PARTITION partition=identifier 
      ( (deallocateUnusedClause | allocateExtentClause | physicalAttributesClause | loggingClause | indexCompression)* 
      | PARAMETERS '(' stringLiteral ')' 
      | COALESCE CLEANUP? parallelClause? 
      | UPDATE BLOCK REFERENCES 
      | UNUSABLE
      )
    ;

renameIndexPartition
    : RENAME (PARTITION partition=identifier | SUBPARTITION subpartition=identifier) TO newName=identifier
    ;

dropIndexPartition
    : DROP PARTITION partitionName=identifier
    ;

splitIndexPartition
    : SPLIT PARTITION partitionNameOld=identifier AT '(' literal (',' literal)* ')' 
      (INTO '(' indexPartitionDescription ',' indexPartitionDescription ')')? 
      parallelClause?
    ;

indexPartitionDescription
    : PARTITION ( partition=identifier ( (segmentAttributesClause | indexCompression)+ 
                                       | PARAMETERS '(' stringLiteral ')'
                                       )? 
                                       (USABLE | UNUSABLE)?
                )?
    ;

coalesceIndexPartition
    : COALESCE PARTITION parallelClause?
    ;

modifyIndexSubpartition
    : MODIFY SUBPARTITION subpartition=identifier (UNUSABLE | allocateExtentClause | deallocateUnusedClause)
    ;

databaseClause
    : DATEBASE dbName=identifier
    | PLUGGABLE DATABASE pdbName=identifier
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
    : RECOVER AUTOMATIC? (FROM SINGLE_QUOTED_STRING)?
      ( ( fullDatabaseRecovery
        | partialDatabaseRecovery
        | LOGFILE SINGLE_QUOTED_STRING
        )
      | )
    ;

fullDatabaseRecovery
    :  STANDBY? DATABASE 
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
    | DATAFILE (filename=SINGLE_QUOTED_STRING | filenumber=integer)
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
    : CREATE DATAFILE (stringLiteral | filenumber=integer) (',' (stringLiteral | filenumber=integer)*)
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
    : ADD STANDBY? LOGFILE ( (INSTANCE stringLiteral | THREAD integer)? (GROUP integer)? redoLogFileSpec (',' (GROUP integer)? redoLogFileSpec)* 
                           | MEMBER stringLiteral REUSE? (',' stringLiteral REUSE?)* TO logfileDescriptor (',' logfileDescriptor)*)
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
      LOGFILE (fileSpecification (',' fileSpecification)*)?
      (FOR logminerSessionName=identifier)?
    ;

switchoverClause
    : SWITCHOVER TO targetDbName=identifier (VERIFY | FORCE)?
    ;

failoverClause
    : FAILOVER TO targetDbName=identifier FORCE?
    ;

commitSwitchoverClause
    : (PREPARE | COMMIT) TO SWITCHOVER 
      ( TO (((PHYSICAL | LOGICAL)? PRIMARY | PHYSICAL? STANDBY) ((WITH | WITHOUT) SESSION SHUTDOWN (WAIT | NOWAIT))? | LOGICAL STANDBY) 
      | CANCEL)?
    ;

// TODO: scnValue
startStandbyClause
    : START LOGICAL STANDBY APPLY IMMEDIATE? NODELAY? (NEW PRIMARY dblink | INITIAL scnValue=literal? | (K_SKIP FAILED TRANSACTION | FINISH))?
    ;

stopStandbyClause
    : (STOP | ABORT) LOGICAL STANDBY APPLY
    ;

convertDatabaseClause
    : CONVERT TO (PHYSICAL | SNAPSHOT) STANDBY
    ;

defaultSettingsClauses
    : DEFAULT EDITION '=' editionName=identifier
    | SET DEFAULT (BIGFILE | SMALLFILE) TABLESPACE 
    | DEFAULT TABLESPACE tablespace 
    | DEFAULT LOCAL? TEMPORARY TABLESPACE (tablespace | tablespaceGroupName=identifier) 
    | RENAME GLOBAL_NAME TO database=identifier '.' domain=identifier ('.' domain=identifier)* 
    | ENABLE BLOCK CHANGE TRACKING (USING FILE stringLiteral REUSE?)? 
    | DISABLE BLOCK CHANGE TRACKING 
    | NO? FORCE FULL DATABASE CACHING 
    | CONTAINERS DEFAULT TARGET '=' ('(' containerName=identifier ')' | NONE) 
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
    : DROP MIRROR COPY mirrorName=identifier
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
    : ADD CACHE MEASURE GROUP (ALL | measName=identifier (',' measName=identifier)*)?
      LEVELS alterCacheClauseItem (',' alterCacheClauseItem)*
    ;

alterDropCacheClause
    : DROP CACHE MEASURE GROUP (ALL | measName=identifier (',' measName=identifier)*)?
      LEVELS alterCacheClauseItem (',' alterCacheClauseItem)*
    ;

alterCacheClauseItem
    : ((dimName=identifier '.')? hierName=identifier '.')? levelName=identifier
    ;

createViewConstraintItem
    : alias (VISIBLE | INVISIBLE)? inlineConstraint*
    | outOfLineConstraint
    ;

objectViewClause
    : OF (schema '.')? typeName=identifier ( WITH OBJECT (IDENTIFIER | ID) (DEFAULT | '(' attribute (',' attribute)* ')')
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
    : CLUSTER (schema '.')? cluster=identifier indexAttributes
    ;

tableIndexClause
    : (schema '.')? table tAlias? tableIndexClauseItem (',' tableIndexClauseItem)* indexProperties
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
    : OF (schema '.')? objectType=identifier objectTableSubstitution?
      ('(' objectProperties ')') (ON COMMIT (DELETE | PRESERVE) ROWS)?
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
    : column (datatype (COLLATE columnCollateName=identifier)?) SORT? (VISIBLE|INVISIBLE)?
      (DEFAULT (ON NULL)? expr | identityClause)?
      (ENCRYPT encryptionSpec)?
      (inlineConstraint+|inlineRefConstraint)?
    ;

virtualColumnDefinition
    : column (datatype (COLLATE columnCollactionName=identifier)) (VISIBLE | INVISIBLE)?
      (GENERATED | ALWAYS)? AS '(' columnExpression=expr ')' VIRTUAL? evaluationEditionClause?
      unusableEditionsClause inlineConstraint*
    ;

evaluationEditionClause
    : EVALUATE USING ( CURRENT EDITION
                     | EDITION edition=identifier
                     | NULL EDITION
                     )
    ;

unusableEditionsClause
    : (UNUSABLE BEFORE (CURRENT EDITION | EDITION edition=identifier))?
      (UNUSABLE BEGINNING WITH (CURRENT EDITION | EDITION edition=identifier | NULL EDITION))?
    ;

periodDefinition
    : PERIOD FOR validTimeColumn=identifier ('(' startTimeColumn=identifier ',' endTimeColumn=identifier ')')?
    ;

supplementalLoggingProps
    : SUPPLEMENTAL LOG ( supplementalLogGrpClause
                       | supplementalIdKeyClause
                       )
    ;

supplementalLogGrpClause
    : GROUP logGroup=identifier '(' supplementalLogGrpClauseItem (',' supplementalLogGrpClauseItem)* ')' ALWAYS?
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
                  | indexProperties
                  )
    ;

indexProperties
    : ( (globalPartitionedIndex | localPartitionedIndex | indexAttributes)+
      | INDEXTYPE IS (domainIndexClause | xmlIndexClause)
      )?
    ;

domainIndexClause
    : indextype=fullObjectPath localDomainIndexClause? parallelClause? (PARAMETERS '(' stringLiteral ')')?
    ;

xmlIndexClause
    : (XDB '.')? XMLINDEX localXmlIndexClause? parallelClause? xmlIndexParametersClause?
    ;

localXmlIndexClause
    : LOCAL ('(' localXmlIndexClauseItem (',' localXmlIndexClauseItem) ')')?
    ;

localXmlIndexClauseItem
    : PARTITION partition=identifier xmlIndexParametersClause?
    ;

// cannot find definition copy from domainIndexClause
xmlIndexParametersClause
    : PARAMETERS '(' stringLiteral ')'
    ;

localDomainIndexClause
    : LOCAL ('(' localDomainIndexClauseItem (',' localDomainIndexClauseItem)* ')')?
    ;

localDomainIndexClauseItem
    : PARTITION partition=identifier (PARAMETERS '(' stringLiteral ')')?
    ;

globalPartitionedIndex
    : GLOBAL PARTITION BY ( RANGE '(' columnList ')' '(' indexPartitioningClause ')'
                          | HASH '(' columnList ')' (individualHashPartitions | hashPartitionsByQuantity)
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
    : PARTITION partition=identifier? (segmentAttributesClause|indexCompression)* (USABLE|UNUSABLE)?
    ;

onListPartitionedTable
    : '(' onListPartitionedTableItem (',' onListPartitionedTableItem)* ')'
    ;

onListPartitionedTableItem
    : PARTITION partition=identifier? (segmentAttributesClause|indexCompression)* (USABLE|UNUSABLE)?
    ;

onHashPartitionedTable
    : STORE IN '(' tablespace (',' tablespace)* ')'
    | '(' onHashPartitionedTableItem (',' onHashPartitionedTableItem)* ')'
    ;

onHashPartitionedTableItem
    : PARTITION partition=identifier? (TABLESPACE tablespace)? indexCompression? (USABLE|UNUSABLE)?
    ;

onCompPartitionedTable
    : (STORE IN '(' tablespace (',' tablespace) ')')
    | '(' onCompPartitionedTableItem (',' onCompPartitionedTableItem)* ')'
    ;

onCompPartitionedTableItem
    : PARTITION partition=identifier? (segmentAttributesClause|indexCompression)* (USABLE|UNUSABLE)? indexSubpartitionClause?
    ;

indexAttributes
    : indexAttributesItem+
    ;

indexAttributesItem
    : physicalAttributesClause
    | loggingClause
    | ONLINE
    | TABLESAPCE (tablespace | DEFAULT)
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
    | PARALLEL integer
    ;

indexSubpartitionClause
    : STORE IN '(' tablespace (',' tablespace) ')'
    | '(' indexSubpartitionClauseItem (',' indexSubpartitionClauseItem)* ')'
    ;

indexSubpartitionClauseItem
    : SUBPARTITION subpartition=identifier? (TABLESPACE tablespace)? indexCompression? (USABLE|UNUSABLE)?
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
    : PARTITION partition=identifier? VALUES LESS THAN '(' literal (',' literal)* ')' segmentAttributesClause?
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
    : (USING encryptAlgorithm=stringLiteral)? (IDENTIFIED BY password=identifier) (integrityAlgorithm=stringLiteral) (NO? SALT)?
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
    : deferredSegmentCreation? segmentAttributesClause tableCompression? inmemoryTableClause ilmClause?
    | deferredSegmentCreation? ( ORGANIZATION ( HEAP segmentAttributesClause? heapOrgTableClause
                                              | INDEX segmentAttributesClause? indexOrgTableClause
                                              | EXTERNAL externalTableClause
                                              )
                               | EXTERNAL PARTITION ATTRIBUTES externalTableClause (REJECT LIMIT)?
                               )
    | CLUSTER cluster=identifier '(' column (',' column)* ')'
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
    : indexOrgTableClauseItem* indexOrgOverflowClause?
    ;

indexOrgTableClauseItem
    : mappingTableClause
    | PCTTHRESHOLD integer
    | prefixCompression
    ;

externalTableClause
    : '(' (TYPE accessDriverType=identifier)? externalTableDataProps ')' (REJECT LIMIT (integer | UNLIMITED))?
    ;

externalTableDataProps
    : (DEFAULT DIRECTORY directory=identifier)?
      (ACCESS PARAMETERS ( '(' opaqueFormatSpec=etRecordSpec ')'
                         | USING CLOB subquery
                         )
      )?
      (LOCATION '(' externalTableDataPropsLocation (',' externalTableDataPropsLocation)* ')')?
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
    | (DATE_CACHE | K_SKIP) integer
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
    ( (DATE | TIMESTAMP (WITH LOCAL? TIME ZONE)?)+ MASK QUOTED_OBJECT_NAME
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
    : (HEXA1 | HEXA2)? (SINGLE_QUOTED_STRING | QUOTED_OBJECT_NAME)
    ;

mappingTableClause
    : MAPPING TABLE
    | NOMAPPING
    ;

indexOrgOverflowClause
    : prefixCompression
    | advancedIndexCompression
    ;

tableCompression
    : COMPRESS
    | ROW STORE COMPRESS (BASIC | ADVANCED)?
    | COLUMN STORE COMPRESS (FOR (QUERY | ARCHIVE) (LOW | HIGH)? )? (NO? ROW LEVEL LOCKING)?
    | NOCOMPRESS
    ;

inmemoryTableClause
    : (INMEMORY inmemoryAttributes | NO INMEMORY)? inmemoryColumnClause?
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
    : DISTRIBUTE (AUTO | BY (ROWID RANGE | PARTITION | SUBPARTITION))? (FOR SERVICE (DEFAULT | ALL | serviceName=identifier | NONE))?
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
    : columnProperties? readOnlyClause?
      indexingClause? tablePartitioningClauses?
      attributeClusteringClause? (CACHE | NOCACHE)?
      resultCacheClause? parallelClause?
      (ROWDEPENDENCIES | NOROWDEPENDENCIES)?
      enableDisableClause*
      rowMovementClause?
      logicalReplicationClause?
      flashbackArchiveClause?
      (ROW ARCHIVAL)?
      (AS subquery | (FOR EXCHANGE WITH TABLE (schema '.')? table))?
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
    : (PARTITION partition=identifier rangeValuesClause tablePartitionDescription externalPartSubpartDataProps?)
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
    : PARTITION partition=identifier? readOnlyClause? indexingClause? partitioningStorageClause?
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
    : PARTITION partition=identifier? listValuesClause tablePartitionDescription externalPartSubpartDataProps?
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
    : PARTITION partition=identifier? tablePartitionDescription ')'
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
    : PARTITIONSET partitionSet=identifier listValuesClause (TABLESPACE SET tablespaceSet)? lobStorageClause?
      (SUBPARTITIONS STORE IN '(' tablespaceSet (',' tablespaceSet) ')')?
    ;

rangePartitionDesc
    : PARTITION partition=identifier? rangeValuesClause tablePartitionDescription
      ( '(' ( rangeSubpartitionDesc (',' rangeSubpartitionDesc)* 
            | listSubpartitionDesc (',' listSubpartitionDesc)* 
            | individualHashSubparts (',' individualHashSubparts)*
            )
        ')'
      | hashSubpartsByQuantity
      )?
    ;

listPartitionDesc
    : PARTITION partition=identifier? listValuesClause tablePartitionDescription
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
    : SUBPARTITION subpartition=identifier? rangeValuesClause readOnlyClause? indexingClause? partitioningStorageClause? externalPartSubpartDataProps?
    ;

listSubpartitionDesc
    : SUBPARTITION subpartition=identifier? listValuesClause readOnlyClause? indexingClause? partitioningStorageClause? externalPartSubpartDataProps?
    ;

individualHashSubparts
    : SUBPARTITION subpartition=identifier? readOnlyClause? indexingClause? partitioningStorageClause?
    ;

hashSubpartsByQuantity
    : SUBPARTITIONS integer (STORE IN '(' tablespace (',' tablespace)* ')')?
    ;

rangeValuesClause
    : VALUES LESS THAN '(' (literal | MAXVALUE) (',' (literal | MAXVALUE))* ')'
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
    : WITH MATERIALIZED ZONEMAP ('(' zonemapName=identifier ')')? 
    | WITHOUT MATERIALIZED ZONEMAP
    ;

enableDisableClause
    : (ENABLE | DISABLE) (VALIDATE | NOVALIDATE)?
      (UNIQUE '(' column (',' column)* ')' | PRIMARY KEY | CONSTRAINT constraintName=identifier)
      usingIndexClause? exceptionsClause? CASCADE? ((KEEP | DROP) INDEX)?
    ;

objectTypeColProperties
    : COLUMN column substitutableColumnClause
    ;

substitutableColumnClause
    : ELEMENT? IS OF TYPE? '(' ONLY type=identifier ')'
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
    : LOB ( '(' lobItem=identifier (',' lobItem=identifier)* ')' STORE AS (SECUREFILE | BASICFILE | '(' lobStorageParameters ')')+
          | '(' lobItem=identifier ')' STORE AS (SECUREFILE | BASICFILE | lobSegName | '(' lobStorageParameters ')')+
          )
    ;

lobPartitionStorage
    : PARTITION partition=identifier (lobStorageClause | varrayColProperties)+
      ('(' SUBPARTITION subpartition=identifier (lobStorageClause | varrayColProperties)+ ')')?
    ;

lobStorageParameters
    : lobStorageParameter+
    ;

lobStorageParameter
    : ( TABLESPACE tablespace
      | TABLESPACE SET tablespaceSet
      ) storageClause
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
         DIRECTORY directoryName=identifier
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

// TODO: impl
role
    : ACCHK_READ
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
    | RENAME
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
    ;

componentAction
    : EXPORT
    | IMPORT
    | ALL
    ;

createDatabaseOption
    : USER SYS IDENTIFIED BY password=identifier                #createDatabaseSysPasswordOption
    | USER SYSTEM IDENTIFIED BY password=identifier             #createDatabaseSystemPasswordOption
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
      tablespaceName=identifier
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
    : (BIGFILE|SMALLFILE)?
      DEFAULT
      (TEMPORARY TABLESPACE
      |LOCAL TEMPORARY TABLESPACE FOR (ALL|LEAF)
      ) tablespace
    ;

undoTablespace
    : (BIGFILE|SMALLFILE)?
      UNDO TABLESPACE tablespace (DATAFILE fileSpecification (',' fileSpecification)*)?
    ;

sizeClause
    : integer unit=(U_KILOBYTE|U_MEGABYTE|U_GIGABYTE|U_TERABYTE|U_PETABYTE|U_EXABYTE)?
    ;

databaseLoggingLogFileClause
    : (GROUP integer)? fileSpecification
    ;

setTimeZoneClause
    : SET TIMEZONE '=' stringLiteral
    ;

enablePluggableDatabase
    : ENABLE PLUGGABLE DATABASE (SEED fileNameConvert? (SYSTEM tablespaceDatafileClauses)? (SYSAUX tablespaceDatafileClauses)?)
    ;

fileNameConvert
    : FILE_NAME_CONVERT '=' 
      ( '(' (fileNameConvertItem) (',' fileNameConvertItem)* ')'
      | NONE
      )
    ;

fileNameConvertItem
    : filenamePattern=stringLiteral ',' replacementFilenamePattern=stringLiteral
    ;

tablespaceDatafileClauses
    : DATAFILES (SIZE sizeClause|autoextendClause)+
    ;

autoextendClause
    : AUTOEXTEND (OFF|ON (NEXT sizeClause)? maxsizeClause?)
    ;

maxsizeClause
    : MAXSIZE (UNLIMITED|sizeClause)
    ;

fileSpecification
    : datafileTempfileSpec
    | redoLogFileSpec
    ;

datafileTempfileSpec
    : stringLiteral
    | (SIZE sizeClause)
    | REUSE
    | autoextendClause
    ;

redoLogFileSpec
    : stringLiteral
    |'(' stringLiteral ')'
    | SIZE sizeClause
    | BLOCKSIZE sizeClause
    | REUSE
    ;

// TODO: The audit_condition can have a maximum length of 4000 characters. It can contain expressions, as well as the following functions and conditions:
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
    : ON ( (schema '.')? object=identifier
         | USER user (',' user)*
         | DIRECTORY directoryName=identifier
         | EDITION editionName=identifier
         | MINING MODEL (schema '.')? miningModelName=identifier
         | JAVA (SOURCE | RESOURCE) (schema '.')? object=identifier
         | SQL TRANSLATION PROFILE (schema '.')? profile=identifier
         )
    ;

grantRolesToPrograms
    : role (',' role)* TO programUnit (',' programUnit)*
    ;

programUnit
    : FUNCTION (schema '.')? functionName
    | PROCEDURE (schema '.')? procedureName=identifier
    | PACKAGE (schema '.')? packageName=identifier
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
    : user (',' user)* IDENTIFIED BY (password=identifier) (',' password=identifier)*
    ;

granteeClauseItem
    : user
    | role
    | PUBLIC
    ;

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
    : FETCH (FIRST | NEXT) (rowcount=expr | percent=expr PERCENT)? (ROW | ROWS) (ONLY | WITH TIES)
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
    : expr (',' expr )* 
    | '(' (expr (',' expr )*)? ')'
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

precision
    : S_INTEGER_WITHOUT_SIGN
    ;

scale
    : S_INTEGER_WITHOUT_SIGN
    ;

size
    : S_INTEGER_WITHOUT_SIGN
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
    | VARCHAR2 '(' size (BYTE | CHAR)? ')'
    | NCHAR ('(' size ')')?
    | NVARCHAR2 '(' size ')'
    ;

numberDatatypes
    : NUMBER ('(' precision (',' scale)* ')')?
    | FLOAT ('(' precision ')')?
    | BINARY_FLOAT
    | BINARY_DOUBLE
    ;

longAndRawDatatypes
    : LONG
    | LONG RAW
    | RAW '(' size ')'
    ;

datetimeDatatypes
    : DATE
    | TIMESTAMP ('(' fractionalSecondsPrecision=precision ')')? (WITH LOCAL? TIME ZONE)?
    | INTERVAL YEAR ('(' yearPrecision=precision ')')? TO MONTH
    | INTERVAL DAY ('(' dayPrecision=precision ')')? TO SECOND ('(' factionalSecondsPrecision=precision ')')?
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
    : identifier
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
    : SDO_GEOMERTY
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
    : (CONSTRAINT constraintName=identifier)?
      ( NOT? NULL
      | UNIQUE
      | PRIMARY KEY
      | referencesClause
      | CHECK '(' condition ')'
      )
      constraintState
    ;

outOfLineConstraint 
    : (CONSTRAINT constraintName=identifier)?
      ( UNIQUE '(' fullObjectPath (',' fullObjectPath)* ')'
      | PRIMARY KEY '(' fullObjectPath (',' fullObjectPath)* ')'
      | FOREIGN KEY '(' fullObjectPath (',' fullObjectPath)* ')' referencesClause
      | CHECK '(' condition ')'
      )
      constraintState
    ;

inlineRefConstraint
    : SCOPE IS (schema '.')? scopeTable=identifier
    | WITH ROWID
    | (CONSTRAINT constraintName=identifier)? referencesClause constraintState
    ;

outOfLineRefConstraint
    :  SCOPE FOR '(' (refCol+=identifier | refAttr+=identifier) ')' IS (schema '.')? scopeTable=identifier
    | REF '(' (refCol+=identifier | refAttr+=identifier) ')' WITH ROWID 
    | (CONSTRAINT constraintName=identifier)? 
      FOREIGN KEY '(' (refCol+=identifier (',' refCol+=identifier)? | refAttr+=identifier (',' refAttr+=identifier)?) ')' 
      referencesClause constraintState
    ;

condition
    : simpleComparisonCondition                                                         #comparisonCondition1
    | groupComparisonCondition                                                          #comparisonCondition2
    | expr IS NOT? (NAN | INFINITE)                                                     #floatingPointCondition
    | NOT '(' condition ')'                                                             #logicalNotCondition
    | condition AND condition                                                           #logicalAndCondition
    | condition OR condition                                                            #logicalOrCondition
    | (dimensionColumn=identifier IS)? ANY                                              #modelIsAnyCondition
    | cellReference=cellAssignment IS PRESENT                                           #modelIsPresentCondition
    | nestedTable=identifier IS NOT? K_A SET                                            #multisetIsASetCondition
    | nestedTable=identifier IS NOT? EMPTY                                              #multisetIsEmptyCondition
    | expr NOT? MEMBER OF? nestedTable=identifier                                       #multisetMemberCondition
    | nestedTable1=identifier NOT? SUBMULTISET OF? nestedTable2=identifier              #multisetSubmultisetCondition
    | (column|stringLiteral) NOT? (LIKE | LIKEC | LIKE2 | LIKE4) 
      (column|stringLiteral) (ESCAPE stringLiteral)?                                    #patternMatchingLikeCondition
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
//    | JSON_EXISTS '(' expr (FORMAT JSON)? ',' jsonBasicPathExpression
//      jsonPassingClause=expr? jsonExistsOnErrorClause? jsonExistsOnEmptyClause?  #jsonExistsCondition
//    | JSON_TEXTCONTAINS '(' 
//        column ',' 
//        jsonBasicPathExpression ','
//        stringLiteral ')'                                                   #jsonTextContainsCondition
    | '(' condition ')'                                                                 #compoundParenthesisCondition
    | NOT condition                                                                     #compoundNotCondition
    | condition (AND | OR) condition                                                    #compoundAndOrCondition
    | expr NOT? BETWEEN expr AND expr                                                   #betweenCondition
    | EXISTS '(' subquery ')'                                                           #existsCondition
    | expr NOT? IN '(' (expressionList|subquery) ')'                                    #inCondition1
    | '(' expr (',' expr)* ')' NOT? 
      IN '(' (expressionList (',' expressionList)* | subquery) ')' #inCondition2
    | expr IS NOT? OF TYPE? '(' isOfTypeConditionItem (',' isOfTypeConditionItem) ')'   #isOfTypeCondition
    ;

isOfTypeConditionItem
    : (ONLY? (SCHEMA '.')? type=identifier)
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

simpleComparisonCondition
    : expr operator1 expr
    | '(' expr (',' expr)* ')' operator2 '(' (expressionList | subquery) ')'
    ;

groupComparisonCondition
    : expr operator1 (ANY | SOME | ALL) '(' (expressionList | subquery) ')'
    | '(' expr (',' expr)* ')' operator2 (ANY | SOME | ALL) '(' (expressionList (',' expressionList)* | subquery) ')'
    ;

expr
    : '(' expr ')'                              #parenthesisExpr
    | ('+' | '-'| PRIOR) expr                   #signExpr
    | TIMESTAMP expr                            #timestampExpr
    | expr ( '*' | '/' | '+' | '-' | '||') expr #binaryExpr
    | expr COLLATE collationName=identifier     #collateExpr
    | functionExpression                        #functionExpr
    | calcMeasExpression                        #calcMeasExpr 
    | caseExpression                            #caseExpr
    | CURSOR '('subquery')'                     #cursorExpr 
    | intervalExpression                        #intervalExpr
    | jsonObjectAccessExpression                #jsonObjectAccessExpr
    | modelExpression                           #modelExpr
    | objectAccessExpression                    #objectAccessExpr
    | placeholderExpression                     #placeholderExpr
//    | scalarSubqueryExpression                  #scalarSubqueryExpr
    | typeConstructorExpression                 #typeConstructorExpr
    | expr AT ( LOCAL | TIME ZONE
        ( S_SINGLE_QUOTE ('+'|'-')? hh=expr ':' mi=expr S_SINGLE_QUOTE
        | DBTIMEZONE
        | SESSIONTIMEZONE
        | timeZoneName=SINGLE_QUOTED_STRING
        | expr
        )
     )                                          #datetimeExpr
    | simpleExpression                          #simpleExpr
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

calcMeasExpression
    : avMeasExpression
    | avSimpleExpression
    | caseExpression
//    | compoundExpression
//    | intervalExpression
    ;

functionExpression
    : functionName '(' expressionList? ')'
    | analyticFunction
    | castFunction
    | treatFunction
    ;

castFunction
    : CAST'('(expr | MULTISET '('subquery')' ) AS (identifier | TIMESTAMP WITH LOCAL? TIME ZONE)
        ( DEFAULT returnValue=expr ON CONVERSION ERROR )?
        (',' fmt=expr (',' nlsparam=expr )? )?')'
    ;

treatFunction
    : TREAT '(' expr AS (REF? ( schema '.' )? type=identifier | JSON) ')' jsonNonfunctionSteps? jsonFunctionStep?
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

calcMeasOrderByClause
    : calcMeasExpression ( ASC | DESC )?  ( NULLS ( FIRST | LAST ) )?
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

//datetimeExpression
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

jsonObjectAccessExpression
    : tableAlias=identifier '.' jsonColumn=identifier ('.' jsonObjectKey=identifier arrayStep*)+? 
    ;

arrayStep
    : '[' (( integer | integer TO integer (',' (integer | integer TO integer) )* ) | '*') ']'
    ;

modelExpression
    : measureColumn=identifier '[' ( condition | expr ) (',' ( condition | expr ) )* ']'
    | aggregateFunction '['
              (
                ( condition | expr ) (',' ( condition | expr ) )*
                | singleColumnForLoop (',' singleColumnForLoop )*
                | multiColumnForLoop
              ) ']'
    | analyticFunction
    ;

analyticFunction
    : analyticFunctionName '(' expr? ( ',' expr )? ( ',' expr )? ')' OVER ( windowName=identifier | '(' analyticClause ')' )
    ;

analyticClause
    : ( windowName=identifier | queryPartitionClause )? ( orderByClause windowingClause? )?
    ;

aggregateFunction
    : aggregateFunctionName '(' expressionList? ')'
    ;

objectAccessExpression
    : ( tableAlias=identifier '.' column '.'
      | objectTableAlias=identifier '.'
      | '(' expr ')' '.'
      )
      ( attribute ('.'attribute )* ('.' method=identifier '(' ( argument=expr (',' argument=expr )* ) ')' )?
      | method=identifier '(' ( argument=expr (',' argument=expr )* ) ')'
      )
    ;

placeholderExpression
    : ':' hostVariable=identifier ( INDICATOR? ':' indicatorVariable=identifier )?
    ;

typeConstructorExpression
    : NEW? ( schema '.' )? typeName=identifier '(' ( expr (',' expr )* )? ')'
    ;

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
    : NULL ON EMPTY
    | ERROR ON EMPTY
    | DEFAULT literal ON EMPTY
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
    : ('+' | '-') S_INTEGER_WITHOUT_SIGN
    | S_INTEGER_WITHOUT_SIGN
    | TRUE
    | FALSE
    | NULL
    | QUOTED_OBJECT_NAME
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

user
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
    : S_INTEGER_WITHOUT_SIGN
    | ('+' | '-') S_INTEGER_WITHOUT_SIGN
    ;

literal
    : intervalLiteral
    | numberLiteral
    | stringLiteral
    | dateTimeLiteral
    ;

numberLiteral
    : ('+' | '-') S_INTEGER_WITHOUT_SIGN
    | ('+' | '-') S_NUMBER_WITHOUT_SIGN
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
    : INTERVAL SINGLE_QUOTED_STRING (YEAR|MONTH) ('(' precision ')')? (TO (YEAR|MONTH))?
    ;

aggregateFunctionName
    : ANY_VALUE
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
    | AVG
    | BIT_AND_AGG
    | BIT_OR_AGG
    | BIT_XOR_AGG
    | CHECKSUM
    | COLLECT
    | CORR
    | CORR_S
    | CORR_K
    | COUNT
    | COVAR_POP
    | COVAR_SAMP
    | CUME_DIST
    | DENSE_RANK
    | FIRST
    | GROUP_ID
    | GROUPING
    | GROUPING_ID
    | JSON_ARRAYAGG
    | JSON_OBJECTAGG
    | KURTOSIS_POP
    | KURTOSIS_SAMP
    | LAST
    | LISTAGG
    | MAX
    | MEDIAN
    | MIN
    | PERCENT_RANK
    | PERCENTILE_CONT
    | PERCENTILE_DISC
    | RANK
    | REGR_SLOPE
    | REGR_INTERCEPT
    | REGR_COUNT
    | REGR_R2
    | REGR_AVGX
    | REGR_AVGY
    | REGR_SXX
    | REGR_SYY
    | REGR_SXY
    | SKEWNESS_POP
    | SKEWNESS_SAMP
    | STATS_BINOMIAL_TEST
    | STATS_CROSSTAB
    | STATS_F_TEST
    | STATS_KS_TEST
    | STATS_MODE
    | STATS_MW_TEST
    | STATS_ONE_WAY_ANOVA
    | STATS_T_TEST_ONE
    | STATS_T_TEST_PAIRED
    | STATS_T_TEST_INDEP
    | STATS_T_TEST_INDEPU
    | STATS_WSR_TEST
    | STDDEV
    | STDDEV_POP
    | STDDEV_SAMP
    | SUM
    | SYS_OP_ZONE_ID
    | SYS_XMLAGG
    | TO_APPROX_COUNT_DISTINCT
    | TO_APPROX_PERCENTILE
    | VAR_POP
    | VAR_SAMP
    | VARIANCE
    | XMLAGG
    ;

analyticFunctionName
    : ANY_VALUE
    | AVG
    | BIT_AND_AGG
    | BIT_OR_AGG
    | BIT_XOR_AGG
    | CHECKSUM
    | CLUSTER_DETAILS
    | CLUSTER_DISTANCE
    | CLUSTER_ID
    | CLUSTER_PROBABILITY
    | CLUSTER_SET
    | CORR
    | COUNT
    | COVAR_POP
    | COVAR_SAMP
    | CUME_DIST
    | DENSE_RANK
    | FEATURE_DETAILS
    | FEATURE_ID
    | FEATURE_SET
    | FEATURE_VALUE
    | FIRST
    | FIRST_VALUE
    | KURTOSIS_POP
    | KURTOSIS_SAMP
    | LAG
    | LAST
    | LAST_VALUE
    | LEAD
    | LISTAGG
    | MAX
    | MIN
    | NTH_VALUE
    | NTILE
    | PERCENT_RANK
    | PERCENTILE_CONT
    | PERCENTILE_DISC
    | PREDICTION
    | PREDICTION_COST
    | PREDICTION_DETAILS
    | PREDICTION_PROBABILITY
    | PREDICTION_SET
    | RANK
    | RATIO_TO_REPORT
    | REGR_SLOPE
    | REGR_INTERCEPT
    | REGR_COUNT
    | REGR_R2
    | REGR_AVGX
    | REGR_AVGY
    | REGR_SXX
    | REGR_SYY
    | REGR_SXY
    | ROW_NUMBER
    | STDDEV
    | STDDEV_POP
    | SKEWNESS_POP
    | SKEWNESS_SAMP
    | STDDEV_SAMP
    | SUM
    | VAR_POP
    | VAR_SAMP
    | VARIANCE
    ;
    
singleRowFunctionName
    : ADD_MONTHS
    | CURRENT_DATE
    | CURRENT_TIMESTAMP
    | EXTRACT
    | FROM_TZ
    | LAST_DAY
    | LOCALTIMESTAMP
    | MONTHS_BETWEEN
    | NEW_TIME
    | NEXT_DAY
    | NUMTODSINTERVAL
    | NUMTOYMINTERVAL
    | ORA_DST_AFFECTED
    | ORA_DST_CONVERT
    | ORA_DST_ERROR
    | ROUND
    | SYS_EXTRACT_UTC
    | SYSDATE
    | SYSTIMESTAMP
    | TO_CHAR
    | TO_DSINTERVAL
    | TO_TIMESTAMP
    | TO_TIMESTAMP_TZ
    | TO_YMINTERVAL
    | TRUNC
    | TZ_OFFSET
    | ASCIISTR
    | BIN_TO_NUM
    | CAST
    | CHARTOROWID
    | COMPOSE
    | CONVERT
    | DECOMPOSE
    | HEXTORAW
    | RAWTOHEX
    | RAWTONHEX
    | ROWIDTOCHAR
    | ROWIDTONCHAR
    | SCN_TO_TIMESTAMP
    | TIMESTAMP_TO_SCN
    | TO_BINARY_DOUBLE
    | TO_BINARY_FLOAT
    | TO_BLOB
    | TO_CLOB
    | TO_DATE
    | TO_LOB
    | TO_MULTI_BYTE
    | TO_NCHAR
    | TO_NCLOB
    | TO_NUMBER
    | TO_SINGLE_BYTE
    | TREAT
    | UNISTR
    | VALIDATE_CONVERSION
    ;

functionName
    : identifier
    ;

identifier
    : UNQUOTED_OBJECT_NAME
    | QUOTED_OBJECT_NAME
    | nonReservedKeywordIdentifier
    | singleRowFunctionName
    | aggregateFunctionName
    | analyticFunctionName
    | pseudoColumn
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

nonReservedKeywordIdentifier
    : ABS
    | ACCHK_READ
    | ACROSS
    | ACTIONS
    | ADMIN
    | ADMINISTER
    | ADVANCED
    | ADVISOR
    | AFTER
    | AGGREGATE
    | ALL_ROWS
    | ALLOW
    | ALTERNATE
    | ALWAYS
    | ANALYTIC
    | ANALYZE
    | ANCESTOR
    | ANY_VALUE
    | ANYDATA
    | ANYDATASET
    | ANYSCHEMA
    | ANYTYPE
    | APPEND
    | APPEND_VALUES
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
    | ARCHIVELOG
    | ARE
    | ASSEMBLY
    | ASSOCIATE
    | AT
    | ATTRIBUTE
    | ATTRIBUTES
    | AUTHENTICATION
    | AUTHORIZATION
    | AUTO
    | AUTOALLOCATE
    | AUTOEXTEND
    | AUTOMATIC
    | AVAILABILITY
    | AVG
    | BACKUP
    | BADFILE
    | BASIC
    | BASICFILE
    | BECOME
    | BEFORE
    | BEGINNING
    | BEQUEATH
    | BFILE
    | BIG
    | BIGFILE
    | BINARY
    | BINARY_DOUBLE
    | BINARY_FLOAT
    | BIT_AND_AGG
    | BIT_OR_AGG
    | BIT_XOR_AGG
    | BITMAP
    | BLOB
    | BLOCK
    | BLOCKCHAIN
    | BLOCKSIZE
    | BODY
    | BOOLEAN
    | BOOLEANONLY
    | BREADTH
    | BUFFER_POOL
    | BUILD
    | BYTE
    | BYTEORDERMARK
    | BYTES
    | CACHE
    | CALL
    | CAPACITY
    | CAPTION
    | CASCADE
    | CASE
    | CEILING
    | CELL_FLASH_CACHE
    | CHANGE
    | CHANGE_DUPKEY_ERROR_INDEX
    | CHARACTER
    | CHARACTERS
    | CHARACTERSET
    | CHECKSUM
    | CHUNK
    | CLASS
    | CLASSIFICATION
    | CLASSIFIER
    | CLOB
    | CLUSTER_DETAILS
    | CLUSTER_DISTANCE
    | CLUSTER_ID
    | CLUSTER_PROBABILITY
    | CLUSTER_SET
    | CLUSTERING
    | CODE
    | COLLATION
    | COLLECT
    | COLUMN_VALUE
    | COLUMNS
    | COMMIT
    | COMPONENT
    | CONSISTENT
    | CONSTRAINT
    | CONSTRAINTS
    | CONTAINER
    | CONTAINER_MAP
    | CONTAINERS
    | CONTAINERS_DEFAULT
    | CONTEXT
    | CONTROLFILE
    | CORR
    | CORR_K
    | CORR_S
    | COST
    | COUNT
    | COUNTED
    | COVAR_POP
    | COVAR_SAMP
    | CREATION
    | CREDENTIAL
    | CRITICAL
    | CROSS
    | CSV
    | CUBE
    | CUME_DIST
    | CURRENT_USER
    | CURRVAL
    | CURSOR
    | CURSOR_SHARING_EXACT
    | CYCLE
    | DATA
    | DATABASE
    | DATAFILE
    | DATAFILES
    | DATAPUMP
    | DATASTORE
    | DATE
    | DATE_CACHE
    | DATE_FORMAT
    | DAY
    | DAY_TO_SECOND
    | DAYS
    | DBA_RECYCLEBIN
    | DBTIMEZONE
    | DEBUG
    | DEC
    | DECREMENT
    | DECRYPT
    | DEDUPLICATE
    | DEFAULT_PDB_HINT
    | DEFAULTIF
    | DEFERRABLE
    | DEFERRED
    | DEFINE
    | DEFINER
    | DEFINITION
    | DELEGATE
    | DELETE_ALL
    | DELIMITED
    | DENSE_RANK
    | DEPTH
    | DEQUEUE
    | DESCRIPTION
    | DETECTED
    | DETERMINES
    | DIAGNOSTICS
    | DICTIONARY
    | DIMENSION
    | DIRECT_LOAD
    | DIRECTIO
    | DIRECTORY
    | DISABLE
    | DISABLE_ALL
    | DISABLE_DIRECTORY_LINK_CHECK
    | DISABLE_PARALLEL_DML
    | DISALLOW
    | DISASSOCIATE
    | DISCARDFILE
    | DISK
    | DISTRIBUTE
    | DML
    | DNFS_DISABLE
    | DNFS_ENABLE
    | DNFS_READBUFFERS
    | DOUBLE
    | DRIVING_SITE
    | DSINTERVAL
    | DUPLICATE
    | DUPLICATED
    | DV
    | DYNAMIC
    | DYNAMIC_SAMPLING
    | EDITION
    | EDITIONABLE
    | EDITIONING
    | ELEMENT
    | EM
    | EMBEDDED
    | EMPTY
    | ENABLE
    | ENABLE_ALL
    | ENABLE_PARALLEL_DML
    | ENCLOSED
    | ENCRYPT
    | END
    | ENDIAN
    | ENQUEUE
    | EQUALS_PATH
    | ERROR
    | ERRORS
    | ESCAPE
    | EVALUATE
    | EVALUATION
    | EXCEPTIONS
    | EXCHANGE
    | EXCLUDE
    | EXECUTE
    | EXEMPT
    | EXPLAIN
    | EXPORT
    | EXPRESS
    | EXTENDED
    | EXTENT
    | EXTERNAL
    | FACT
    | FALSE
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
    | FILES
    | FILESYSTEM_LIKE_LOGGING
    | FILTER
    | FINAL
    | FIRST
    | FIRST_ROWS
    | FIRST_VALUE
    | FIXED
    | FLASH_CACHE
    | FLASHBACK
    | FLOOR
    | FOLDER
    | FOLLOWING
    | FORCE
    | FOREIGN
    | FORMAT
    | FREELIST
    | FREELISTS
    | FREEPOOLS
    | FRESH_MV
    | FTP
    | FULL
    | FUNCTION
    | GATHER_OPTIMIZER_STATISTICS
    | GENERATED
    | GLOBAL
    | GROUP_ID
    | GROUPING
    | GROUPING_ID
    | GROUPS
    | HALF_YEARS
    | HASH
    | HASHING
    | HEAP
    | HIER_ANCESTOR
    | HIER_CAPTION
    | HIER_DEPTH
    | HIER_DESCRIPTION
    | HIER_LAG
    | HIER_LEAD
    | HIER_LEVEL
    | HIER_MEMBER_NAME
    | HIER_MEMBER_UNIQUE_NAME
    | HIER_PARENT
    | HIERARCHY
    | HIGH
    | HOURS
    | HTTP
    | ID
    | IDENTIFIER
    | IDENTITY
    | IDLE
    | IGNORE
    | IGNORE_CHARS_AFTER_EOR
    | IGNORE_ROW_ON_DUPKEY_INDEX
    | ILM
    | IMPORT
    | INCLUDE
    | INDEX_ASC
    | INDEX_COMBINE
    | INDEX_DESC
    | INDEX_FFS
    | INDEX_JOIN
    | INDEX_SS
    | INDEX_SS_ASC
    | INDEX_SS_DESC
    | INDEXING
    | INDEXTYPE
    | INDICATOR
    | INFINITE
    | INHERIT
    | INITIALLY
    | INITRANS
    | INMEMORY
    | INMEMORY_PRUNING
    | INNER
    | INSTANCE
    | INT
    | INTERLEAVED
    | INTERNAL
    | INTERVAL
    | INVALIDATION
    | INVISIBLE
    | IO_OPTIONS
    | ITERATE
    | JAVA
    | JOB
    | JOIN
    | JSON
    | JSON_ARRAYAGG
    | JSON_EQUAL
    | JSON_OBJECTAGG
    | K_SKIP
    | KEEP
    | KEEP_DUPLICATES
    | KEY
    | KEYS
    | KURTOSIS_POP
    | KURTOSIS_SAMP
    | LAG
    | LAG_DIFF
    | LAG_DIFF_PERCENT
    | LANGUAGE
    | LAST
    | LAST_VALUE
    | LATERAL
    | LAX
    | LDRTRIM
    | LEAD
    | LEAD_DIFF
    | LEAD_DIFF_PERCENT
    | LEADING
    | LEAF
    | LEFT
    | LENGTH
    | LESS
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
    | LOAD
    | LOB
    | LOBS
    | LOCAL
    | LOCATION
    | LOCATOR
    | LOCKDOWN
    | LOCKED
    | LOCKING
    | LOG
    | LOGFILE
    | LOGGING
    | LOGICAL
    | LOGMINING
    | LOGOFF
    | LOGON
    | LOW
    | LOW_COST_TBS
    | LOWER
    | LRTRIM
    | LTRIM
    | MAIN
    | MANAGE
    | MANAGEMENT
    | MANAGER
    | MAPPING
    | MASK
    | MATCH
    | MATCH_NUMBER
    | MATCH_RECOGNIZE
    | MATERIALIZED
    | MAX
    | MAX_ERROR
    | MAXDATAFILES
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
    | MEMCOMPRESS
    | MEMOPTIMIZE
    | MERGE
    | METADATA
    | MIN
    | MINALUE
    | MINEXTENTS
    | MINING
    | MINNUMBER
    | MINSTRING
    | MINUTES
    | MINVALUE
    | MISSING
    | MLE
    | MODEL
    | MODEL_MIN_ANALYSIS
    | MODIFICATION
    | MONITOR
    | MONTH
    | MONTHS
    | MOVEMENT
    | MULTIVALUE
    | NAME
    | NAN
    | NATIONAL
    | NATIVE_FULL_OUTER_JOIN
    | NATURAL
    | NAV
    | NCHAR
    | NCLOB
    | NESTED
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
    | NOCYCLE
    | NODIRECTIO
    | NODISCARDFILE
    | NOFORCE
    | NOLOGFILE
    | NOLOGGING
    | NOMAPPING
    | NOMAXVALUE
    | NOMINVALUE
    | NONE
    | NONEDITIONABLE
    | NONSCHEMA
    | NONULLIF
    | NOORDER
    | NOPARALLEL
    | NORELY
    | NOROWDEPENDENCIES
    | NOSORT
    | NOTIFICATION
    | NOTRIM
    | NOVALIDATE
    | NTH_VALUE
    | NTILE
    | NULLIF
    | NULLS
    | NUMBERONLY
    | NUMERIC
    | NVARCHAR2
    | OBJECT
    | OFF
    | OFFSET
    | OIDINDEX
    | OLS
    | ONE
    | ONLY
    | OPERATOR
    | OPT_PARAM
    | OPTIMAL
    | OPTIMIZE
    | OPTIONALLY
    | ORACLE_DATE
    | ORACLE_NUMBER
    | ORDERED
    | ORGANIZATION
    | OUTER
    | OUTLINE
    | OVER
    | OVERFLOW
    | OVERRIDE
    | PACKAGE
    | PARALLEL
    | PARALLEL_INDEX
    | PARAMETERS
    | PARENT
    | PARTIAL
    | PARTITION
    | PARTITIONS
    | PARTITIONSET
    | PASSWORD
    | PAST
    | PATH
    | PATTERN
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
    | PERMUTE
    | PFILE
    | PIVOT
    | PLAN
    | PLUGGABLE
    | POINT
    | POLICY
    | POSITION
    | PQ_CONCURRENT_UNION
    | PQ_FILTER
    | PQ_SKEW
    | PRECEDING
    | PRECISION
    | PREDICTION
    | PREDICTION_COST
    | PREDICTION_DETAILS
    | PREDICTION_PROBABILITY
    | PREDICTION_SET
    | PREPROCESSOR
    | PREPROCESSOR_TIMEOUT
    | PRESENT
    | PRESERVE
    | PREV
    | PRIMARY
    | PRIORITY
    | PRIVATE
    | PRIVILEGE
    | PRIVILEGES
    | PROCEDURE
    | PROCESS
    | PROFILE
    | PROGRAM
    | PROTOCOL
    | PURGE
    | PUSH_PRED
    | PUSH_SUBQ
    | PX_JOIN_FILTER
    | QB_NAME
    | QUALIFY
    | QUARTERS
    | QUERY
    | QUEUE
    | RANDOM
    | RANGE
    | RANK
    | RATIO_TO_REPORT
    | READ
    | READS
    | READSIZE
    | REAL
    | RECORDS
    | RECYCLE
    | RECYCLEBIN
    | REDACTION
    | REDEFINE
    | REF
    | REFERENCE
    | REFERENCES
    | REFRESH
    | REGEXP_LIKE
    | REGR_AVGX
    | REGR_AVGY
    | REGR_COUNT
    | REGR_INTERCEPT
    | REGR_R2
    | REGR_SLOPE
    | REGR_SXX
    | REGR_SXY
    | REGR_SYY
    | REJECT
    | RELATIONAL
    | RELY
    | REMOTE
    | REPLACE
    | REPLICATION
    | RESTORE
    | RESTRICTED
    | RESULT_CACHE
    | RESUMABLE
    | RETENTION
    | RETRY_ON_ROW_CHANGE
    | RETURN
    | RETURNING
    | REUSE
    | REVERSE
    | REWRITE
    | RIGHT
    | ROLE
    | ROLES
    | ROLLBACK
    | ROLLUP
    | ROW_NUMBER
    | ROWDEPENDENCIES
    | RTRIM
    | RULE
    | RULES
    | RUNNING
    | SALT
    | SAMPLE
    | SAVEPOINT
    | SCALARS
    | SCAN
    | SCHEDULER
    | SCHEMA
    | SCN
    | SCOPE
    | SDO_GEOMERTY
    | SDO_GEORASTER
    | SDO_TOPO_GEOMETRY
    | SEARCH
    | SECOND
    | SECONDS
    | SECUREFILE
    | SEED
    | SEGMENT
    | SEQUENCE
    | SEQUENTIAL
    | SERIAL
    | SERVICE
    | SESSIONTIMEZONE
    | SETS
    | SHA2_512
    | SHARDED
    | SHARDS
    | SHARE_OF
    | SHARING
    | SIBLINGS
    | SINGLE
    | SIZES
    | SKEWNESS_POP
    | SKEWNESS_SAMP
    | SMALLFILE
    | SOME
    | SORT
    | SOURCE
    | SPATIAL
    | SPFILE
    | SQL
    | STANDARD
    | STANDBY
    | STAR_TRANSFORMATION
    | STATEMENT
    | STATEMENT_QUEUING
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
    | STORAGE
    | STORE
    | STRICT
    | STRING
    | STRINGONLY
    | SUBMULTISET
    | SUBPARTITION
    | SUBPARTITIONS
    | SUBSET
    | SUBSTITUTABLE
    | SUM
    | SUPPLEMENTAL
    | SYS
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
    | TABLES
    | TABLESAPCE
    | TABLESPACE
    | TEMP
    | TEMPLATE
    | TEMPORARY
    | TERMINATED
    | TERRITORY
    | TEXT
    | THAN
    | THESE
    | TIER
    | TIES
    | TIME
    | TIMESTAMP
    | TIMEZONE
    | TO_APPROX_COUNT_DISTINCT
    | TO_APPROX_PERCENTILE
    | TOPLEVEL
    | TRACING
    | TRACKING
    | TRANSACTION
    | TRANSFORM
    | TRANSLATE
    | TRANSLATION
    | TRUE
    | TRUNCATE
    | TUNING
    | TYPE
    | UNBOUNDED
    | UNDER
    | UNDER_PATH
    | UNDO
    | UNIFORM
    | UNLIMITED
    | UNNEST
    | UNPIVOT
    | UNSIGNED
    | UNTIL
    | UNUSABLE
    | UPDATED
    | UPPER
    | UPSERT
    | URITYPE
    | UROWID
    | USABLE
    | USE
    | USE_BAND
    | USE_CONCAT
    | USE_CUBE
    | USE_HASH
    | USE_MERGE
    | USE_NL
    | USE_NL_WITH_INDEX
    | USER_DATA
    | USING
    | V1
    | VALUE
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
    | VERSION
    | VERSIONS
    | VIRTUAL
    | VISIBLE
    | WAIT
    | WEEKS
    | WHEN
    | WHITESPACE
    | WINDOW
    | WITHIN
    | WITHOUT
    | WORK
    | WRITE
    | XDB
    | XML
    | XMLAGG
    | XMLINDEX
    | XMLSCHEMA
    | XMLTAG
    | XMLTYPE
    | XS
    | YEAR
    | YEAR_TO_MONTH
    | YEARS
    | YES
    | YMINTERVAL
    | ZONE
    | ZONED
    | ZONEMAP

    | K_A
    | U_KILOBYTE
    | U_MEGABYTE
    | U_GIGABYTE
    | U_TERABYTE
    | U_PETABYTE
    | U_EXABYTE
    | HEXA1
    ;