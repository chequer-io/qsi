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
    | createAuditPolicy
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

createAuditPolicy
    : CREATE AUDIT POLICY policy=identifier
      privilegeAuditClause?
      actionAuditClause?
      roleAuditClause?
      (WHEN 
       S_SINGLE_QUOTE auditCondition S_SINGLE_QUOTE
       EVALUATE PER (STATEMENT|SESSION|INSTANCE)
      )?
      (ONLY TOPLEVEL)?
      (CONTAINER '=' (ALL|CURRENT))?
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
    : (objectAction|ALL) ON 
       (
         DIRECTORY directoryName=identifier
       | MINING MODEL (schema '.')? identifier
       | (schema '.')? identifier
       )                                        #objectStandardAction
    | (systemAction|ALL)                        #systemStandardAction
    ;

componentActions
    : ACTIONS COMPONENT '=' 
      (
        (DATAPUMP|DIRECT_LOAD|OLS|XS) componentAction (',' componentAction)*
      | DV componentAction ON identifier (',' componentAction ON identifier)*
      | PROTOCOL (FTP|HTTP|AUTHENTICATION)
      )
    ;

roleAuditClause
    : ROLES role (',' role)*
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
    : AS '(' calcMeasExpression=avExpression ')'
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

avExpression
    :
//    : avMeasExpression
//    | avHierExpression
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