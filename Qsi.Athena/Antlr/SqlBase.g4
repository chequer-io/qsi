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


/***
 * Athena SQL statements follows Hive and Presto SQL statements.
 
 * 1. View DDL statements follow Presto.
 * 2. Other DDL statements follow Hive.
 * 3. DML statements follow Presto.
 */

statement
    /***
     * Athena DDL
     *
     * @athena (Amazon Athena DDL Reference)[https://docs.aws.amazon.com/athena/latest/ug/language-reference.html]
     * @hive (HiveQL DDL Reference)[https://cwiki.apache.org/confluence/display/Hive/LanguageManual+DDL]
     */
     
    /**********************
     *                    *
     *                    *
     *    Database DDL    *
     *                    *
     *                    *
     **********************/
     
    /**
     * Create Database
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/create-database.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-CreateDatabaseCreateDatabase
     */
    : CREATE
        (REMOTE)?
        (DATABASE | SCHEMA)
        (IF NOT EXISTS)?
        databaseName=identifier[null]
        (COMMENT comment=string)?
        (LOCATION location=string)?
        (MANAGEDLOCATION managedLocation=string)?
        (WITH DBPROPERTIES dbProperties=stringProperties)?                                                              #createDatabase

    /**
     * Drop Database
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-DropDatabase
     */
    | DROP
        (DATABASE | SCHEMA)
        (IF EXISTS)?
        databaseName=qualifiedName
        (RESTRICT | CASCADE)?                                                                                           #dropDatabase
    
    /**
     * Database Set DB Properties 
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/alter-database-set-dbproperties.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AlterDatabase
     */
    | ALTER
        (DATABASE | SCHEMA)
        databaseName=qualifiedName
        SET DBPROPERTIES dbProperties=stringProperties                                                                  #setDbProperties

    /**
     * Database Set Owner
     *
     * @notsupport Tested at 2021. 12. 14.
     */
    // Not Implemented

    /**
     * Database Set Location
     *
     * @notsupport Tested at 2021. 12. 14.
     */
    // Not Implemented
    
    /**
     * Database Set Managed Location
     *
     * @notsupport Tested at 2021. 12. 14.
     */
    // Not Implemented
    
    /**
     * Use Database
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-UseDatabase
     * @notsupport Tested at 2021. 12. 20.
     */
//    | USE
//        databaseName=identifier[null]                                                                     #useDatabase
        
        
        
        
        
    /**********************
     *                    *
     *                    *
     * Data Connector DDL *
     *                    *
     *                    *
     **********************/
    /**
     * Create Connector
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-CreateDataConnectorCreateConnector
     * @notsupport
     */
    // Not Implemented
    
    /**
     * Drop Connector
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-DropConnector
     * @notsupport
     */
    // Not Implemented
    
    /**
     * Connector Set DC Properties
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AlterConnector
     * @notsupport
     */
    // Not Implemented
    
    /**
     * Connector Set URL
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AlterConnector
     * @notsupport
     */
    // Not Implemented
    
    /**
     * Connector Set Owner
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AlterConnector
     * @notsupport
     */
    // Not Implemented
    
    
    
    
    
    /**********************
     *                    *
     *                    *
     *     Table DDL      *
     *                    *
     *                    *
     **********************/
    /**
      * Create Table
      *
      * @athena https://docs.aws.amazon.com/athena/latest/ug/create-table.html
      * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-CreateTable
      */
    | CREATE
        (TEMPORARY)?
        (EXTERNAL)?
        TABLE
        (IF NOT EXISTS)?
        tableName=qualifiedName
        ( '(' columnDefinition (',' columnDefinition)* ')' )?
        (COMMENT tableComment=string)?
        (PARTITIONED BY '(' columnDefinition (',' columnDefinition)* ')' )?
        (
            CLUSTERED BY '(' identifier[null] (',' identifier[null])* ')'
            (SORTED BY (columnName=identifier[null] (ASC | DESC)? (',' columnName=identifier[null] (ASC | DESC)? )* ) )?
            INTO numBuckets=number BUCKETS
        )?
        (
            SKEWED BY '(' columnName=identifier[null] (',' columnName=identifier[null])* ')'
            ON '(' '(' columnValue=string (',' columnValue=string)* ')' ( ',' '(' columnValue=string (',' columnValue=string)* ')' )*  ')'
            (STORED AS DIRECTORIES)?
        )?
        (
            ROW FORMAT rowFormat
            | STORED AS fileFormat
            | ROW FORMAT rowFormat STORED AS fileFormat
            | STORED BY storageHandlerClassName=string
                (WITH SERDEPROPERTIES serDeProperties=stringProperties)? 
        )?
        (LOCATION location=string)?
        (TBLPROPERTIES tblProperties=stringProperties)?                                                                 #createTable
        
    /**
      * Create Table As
      *
      * @athena https://docs.aws.amazon.com/athena/latest/ug/create-table.html
      * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-CreateTable
      */
    | CREATE
        (TEMPORARY)?
        (EXTERNAL)?
        TABLE
        (IF NOT EXISTS)?
        tableName=qualifiedName
        ( '(' columnDefinition (',' columnDefinition)* ')' )?
        (COMMENT tableComment=string)?
        (PARTITIONED BY '(' columnDefinition (',' columnDefinition)* ')' )?
        (
            CLUSTERED BY '(' identifier[null] (',' identifier[null])* ')'
            (SORTED BY (columnName=identifier[null] (ASC | DESC)? (',' columnName=identifier[null] (ASC | DESC)? )* ) )?
            INTO numBuckets=number BUCKETS
        )?
        (
            SKEWED BY '(' columnName=identifier[null] (',' columnName=identifier[null])* ')'
            ON '(' '(' columnValue=string (',' columnValue=string)* ')' ( ',' '(' columnValue=string (',' columnValue=string)* ')' )*  ')'
            (STORED AS DIRECTORIES)?
        )?
        (
            ROW FORMAT rowFormat
            | STORED AS fileFormat
            | ROW FORMAT rowFormat STORED AS fileFormat
            | STORED BY storageHandlerClassName=string
                (WITH SERDEPROPERTIES serDeProperties=stringProperties)? 
        )?
        (LOCATION location=string)?
        (TBLPROPERTIES tblProperties=stringProperties)?
        AS query
        (WITH NO? DATA)?                                                                                                #createTableAs
        
       
    /**
     * Create TABLE LIKE
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-CreateTable
     */
    | CREATE
        (TEMPORARY)?
        (EXTERNAL)?
        TABLE
        (IF NOT EXISTS)?
        tableName=qualifiedName
        LIKE likeTableName=qualifiedName
        (LOCATION location=string)                                                                                      #createTableLike

    /**
     * Drop Table
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/drop-table.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-DropTable
     */
    | DROP
        TABLE
        (IF NOT EXISTS)?
        tableName=qualifiedName
        (PURGE)?                                                                                                        #dropTable

    /**
     * Truncate Table
     *
     * @notsupport Tested at 2021. 12. 16.
     */
    // Not Implemented
     
    /**
     * Table Rename to
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
    
    /**
     * Table Set Tbl Properties
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/alter-table-set-tblproperties.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AlterTableProperties
     */
    | ALTER
        TABLE tableName=qualifiedName
        SET TBLPROPERTIES tblProperties=stringProperties                                                                #setTblProperties
     
    /**
     * Table Set Serde
     *
     * @notsupport Tested at 2021. 12. 14.
     */
    // Not Implemented
   
    /**
     * Table Set Serde Properties
     *
     * @notsupport Tested at 2021. 12. 14.
     */
    // Not Implemented
    
    /**
     * Table Unset Serde Properties
     *
     * @notsupport Tested at 2021. 12. 14.
     */
    // Not Implemented
     
    /**
     * Table Add Constraint
     *
     * @notsupport Tested at 2021. 12. 14.
     */
    // Not Implemented
     
    /**
     * Table Clustered by
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Table Not Clustered
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Table Skewed by
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Table Set Skewed Location
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Table Not Skewed
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Table Not Stored
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Table Not Stored as Directories
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
    
    /**
     * Msck Repair Table
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/msck-repair-table.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-RecoverPartitions(MSCKREPAIRTABLE)
     */
    | MSCK (REPAIR)? TABLE tableName=qualifiedName
        (ADD | DROP | SYNC PARTITIONS)                                                                                  #repairTable
    
    
    
    
    
    /***************************
     *                         *
     *                         *
     *   Table/Partition DDL   *
     *                         *
     *                         *
     ***************************/
    /**
     * Table/Partition Set File Format
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Table/Partition Touch
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Table/Partition Compact
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Table/Partition Concatenate
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
    
    /**
     * Table/Partition Enable/Disable No Drop
     *
     * @notsupport Tested at 2021. 12. 14.
     */
    // Not Implemented
    
    /**
     * Table/Partition Enable/Disable Offline
     *
     * @notsupport Tested at 2021. 12. 14.
     */
    // Not Implemented
    
    /**
     * Table/Partition Update Columns
     *
     * @notsupport Tested at 2021. 12. 14.
     */
    // Not Implemented
    
    /**
     * Table/Partition Set Location
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/alter-table-set-location.html
     * @hive https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=82706445#LanguageManualDDL-AlterTable/PartitionLocation
     */
    | ALTER TABLE tableName=qualifiedName
        (PARTITION partitionSpec=properties)?
        SET LOCATION location=string                                                                                    #setLocation





    /**********************
     *                    *
     *                    *
     *   Partition DDL    *
     *                    *
     *                    *
     **********************/
    /**
     * Archive Partition
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
    
    /**
     * Unarchive Partition
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
    
    /**
     * Add Partitions
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/alter-table-add-partition.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AddPartitions
     */
    | ALTER TABLE tableName=qualifiedName ADD (IF NOT EXISTS)?
        (
            PARTITION partitionSpec=properties
            (LOCATION location=string)?
        )+                                                                                                              #addPartitions

    /**
     * Rename Partition
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/alter-table-rename-partition.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-RenamePartition
     */
    | ALTER TABLE tableName=qualifiedName
        PARTITION from=properties
        RENAME TO PARTITION to=properties                                                                               #renamePartition

    /**
     * Exchange Partition
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented

    /**
     * Drop Partition
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/alter-table-drop-partition.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-DropPartitions
     */
    | ALTER TABLE tableName=qualifiedName DROP (IF EXISTS)?
        PARTITION partitionSpec=properties (',' PARTITION partitionSpec=properties)*
        (IGNORE PROTECTION)?
        (PURGE)?                                                                                                        #dropPartition





    /**********************
     *                    *
     *                    *
     *     Column DDL     *
     *                    *
     *                    *
     **********************/
    /**
     * Change Column
     * @comment According to Reference 1(AWS), CHANGE COLUMN is not supported in Athena, but works in test
     *          test query: "ALTER TABLE table_name CHANGE original_column changed_column int"
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ChangeColumnName/Type/Position/Comment
     */
    | ALTER TABLE tableName=qualifiedName
        (PARTITION properties)?
        CHANGE (COLUMN)? identifier[null] columnDefinition
        (FIRST | AFTER identifier[null])?
        (CASCADE | RESTRICT)?                                                                                           #changeColumn

    /**
     * Add Columns
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/alter-table-add-columns.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-Add/ReplaceColumns
     */
    | ALTER TABLE tableName=qualifiedName
        (PARTITION partitionSpec=properties)?
        ADD COLUMNS
        '(' columnDefinition (',' columnDefinition)* ')'
        (CASCADE|RESTRICT)?                                                                                             #addColumns

    /**
     * Replace Columns
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/alter-table-replace-columns.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-Add/ReplaceColumns
     */
    | ALTER TABLE tableName=qualifiedName
        (PARTITION partitionSpec=properties)?
        REPLACE COLUMNS
        '(' columnDefinition (',' columnDefinition)* ')'
        (CASCADE|RESTRICT)?                                                                                             #replaceColumns




    
    /**********************
     *                    *
     *                    *
     *      View DDL      *
     *                    *
     *                    *
     **********************/
    /**
     * Create View
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/create-view.html
     * @presto https://prestodb.io/docs/current/sql/create-view.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-CreateView
     */
    | CREATE
        (OR REPLACE)?
        VIEW
        (IF NOT EXISTS)?
        viewName=qualifiedName
        (
            columnName=identifier[null] (COMMENT columnComment=string)?
            (',' columnName=identifier[null] (COMMENT columnComment=string)? )*
        )?
        viewColumnAliases?
        (SECURITY (DEFINER | INVOKER))?
        (COMMENT viewComment=string)?
        (TBLPROPERTIES tblProperties=stringProperties)?
        AS query                                                                                                        #createView
    
    /**
     * Drop View
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/drop-view.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-DropView
     */
    | DROP VIEW (IF EXISTS)? viewName=qualifiedName                                                                     #dropView
     
    /**
     * Set View Tbl Properties
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AlterViewProperties
     */
    | ALTER VIEW viewName=qualifiedName
        SET TBLPROPERTIES tblProperties=stringProperties                                                                #setViewTblProperties
    
    /**
     * Alter View As Select
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AlterViewAsSelect
     */
    | ALTER VIEW viewName=qualifiedName AS query                                                                        #alterViewAsSelect





    /*************************
     *                       *
     *                       *
     * Materialized View DDL *
     *                       *
     *                       *
     *************************/
    /**
     * Create Materialized View
     *
     * @notsupport Not tested
     */
    // Not Implemented
     
    /**
     * Drop Materialized View
     *
     * @notsupport Not tested
     */
    // Not Implemented
     
    /**
     * Alter Materialized View
     *
     * @notsupport Not tested
     */
    // Not Implemented





    /*************************
     *                       *
     *                       *
     *       Index DDL       *
     *                       *
     *                       *
     *************************/
    /**
     * Create Index
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented

    /**
     * Drop Index
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented

    /**
     * Alter Index
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented





    /*************************
     *                       *
     *                       *
     *       Macro DDL       *
     *                       *
     *                       *
     *************************/
    /**
     * Create Temporary Macro
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented

    /**
     * Drop Temporary Macro
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented





    /*************************
     *                       *
     *                       *
     *     Function DDL      *
     *                       *
     *                       *
     *************************/
    /**
     * Create Function
     *
     * @notsupport Tested at 2021. 12. 16.
     */
    // Not Implemented

    /**
     * Drop Function
     *
     * @notsupport Tested at 2021. 12. 16.
     */
    // Not Implemented

    /**
     * Reload Function
     *
     * @notsupport Tested at 2021. 12. 16.
     */
    // Not Implemented





    /*************************
     *                       *
     *                       *
     *       Role DDL        *
     *                       *
     *                       *
     *************************/
    /**
     * Create Role
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Grant Role
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Revoke Role
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Drop Role
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
    
    /**
     * Show Roles
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Show Role Grant
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Show Current Roles
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Set Role
     *
     * @notsupport Not tested
     */
    // Not Implemented
     
    /**
     * Show Principals
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
     
    /**
     * Grant privilege_type
     *
     * @notsupport Not tested
     */
    // Not Implemented
     
    /**
     * Revoke privilege_type
     *
     * @notsupport Not tested
     */
    // Not Implemented
     
    /**
     * Show Grant
     *
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented





    /*************************
     *                       *
     *                       *
     *       Show DDL        *
     *                       *
     *                       *
     *************************/
    /**
     * Show Databases
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/show-databases.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowDatabases
     */
    | SHOW (DATABASES | SCHEMAS)
        (LIKE string)?                                                                                                  #showDatabases

    /**
     * Show Connectors
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowConnectors
     * @notsupport Not tested
     */
    // Not Implemented
    
    /**
     * Show Tables
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/show-tables.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowTables
     */
    | SHOW TABLES
        (IN databaseName=identifier[null])?
        (string)?                                                                                                       #showTables
    
    /**
     * Show Views
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/show-views.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowViews
     */
    | SHOW VIEWS
        ( (IN | FROM) databaseName=identifier[null])?
        (LIKE string)                                                                                                   #showViews
        
    /**
     * Show Materialized Views
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowMaterializedViews
     * @notsupport Not tested
     */
    // Not Implemented
    
    /**
     * Show Partitions
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/show-partitions.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowPartitions
     */
    | SHOW PARTITIONS
        tableName=qualifiedName
        (PARTITION properties)?
        (WHERE where=booleanExpression)?
        (ORDER BY sortItem (',' sortItem)*)?
        (LIMIT limit=INTEGER_VALUE)?                                                                                    #showPartitions
    
    /**
     * Show Table/Partition Extended
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowTable/PartitionExtended
     */
    | SHOW TABLE EXTENDED
        ((IN | FROM) databaseName=qualifiedName)?
        LIKE string
        (PARTITION properties)?                                                                                         #showTableExtended
    
    /**
     * Show Table Properties
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/show-tblproperties.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowTableProperties
     */
    | SHOW TBLPROPERTIES
        tableName=qualifiedName
        ( '(' string ')' )?                                                                                             #showTableProperties
        
    /**
     * Show Create Table
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/show-create-table.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowCreateTable
     */
    | SHOW CREATE TABLE
        tableName=qualifiedName                                                                                         #showCreateTable
    
    /**
     * Show Indexes
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowIndexes
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
    
    /**
     * Show Columns
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/show-columns.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowColumns
     */
    | SHOW COLUMNS
        (FROM | IN)
        tableName=qualifiedName
        ( (FROM | IN) databaseName=qualifiedName?)
        (LIKE string)?                                                                                                  #showColumns
    
    /**
     * Show Functions
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowFunctions
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
    
    /**
     * Show Locks
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowLocks
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
    
    /**
     * Show Conf
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowConf
     * @notsupport Tested at 2021. 12. 16.
     */
    // Not Implemented
    
    /**
     * Show Transactions
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowTransactions
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
    
    /**
     * Show Compactions
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-ShowCompactions
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented





    /*************************
     *                       *
     *                       *
     *     Describe DDL      *
     *                       *
     *                       *
     *************************/
    /**
     * Describe Database
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-DescribeDatabase
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented
    
    /**
     * Describe Data Connector
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-DescribeDataconnector
     * @notsupport Not tested
     */
    // Not Implemented
    
    /**
     * Describe Table / View / MaterializedView / Column
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/describe-table.html
     * @athena https://docs.aws.amazon.com/athena/latest/ug/describe-view.html
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-DescribeTable/View/MaterializedView/Column
     */
    | (DESCRIBE | DESC)
        (EXTENDED | FORMATTED)?
        tableName=qualifiedName
        (PARTITION partitionSpec=properties)?
        (
            columnName=identifier[null]
            ( '.' (fieldName=identifier[null] | '$elem$' | '$key$' | '$value$') )*
        )?                                                                                                              #describeTable





    /*************************
     *                       *
     *                       *
     *       Abort DDL       *
     *                       *
     *                       *
     *************************/
    /**
     * Abort Transactions
     *
     * @hive https://cwiki.apache.org/confluence/display/hive/languagemanual+ddl#LanguageManualDDL-AbortTransactions
     * @notsupport Not tested
     */



    /******************************************************************************************************************/


    /**
     * Athena DML
     *
     * @athena (Amazon Athena DML Reference)[https://docs.aws.amazon.com/athena/latest/ug/functions-operators-reference-section.html]
     * @presto (Presto SQL Syntax Refrence)[https://prestodb.io/docs/current/sql.html]
     */
     
    /**
     * ALTER FUNCTION
     *
     * @presto https://prestodb.io/docs/current/sql/alter-function.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * ALTER SCHEMA
     *
     * @presto https://prestodb.io/docs/current/sql/alter-schema.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * ALTER TABLE
     *
     * @presto https://prestodb.io/docs/current/sql/alter-table.html
     * @notsupport Use hive syntax
     */
    // Not Implemented
     
    /**
     * Analyze
     *
     * @presto https://prestodb.io/docs/current/sql/analyze.html
     * @notsupport Tested at 2021. 12. 20.
     */
    // Not Implemented
    
    /**
     * Call
     *
     * @presto https://prestodb.io/docs/current/sql/call.html
     * @notsupport Tested at 2021. 12. 20.
     */
    // Not Implemented

    /**
     * COMMIT
     *
     * @presto https://prestodb.io/docs/current/sql/commit.html
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemeneted

    /**
     * CREATE FUNCTION
     *
     * @presto https://prestodb.io/docs/current/sql/create-function.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * CREATE ROLE
     *
     * @presto https://prestodb.io/docs/current/sql/create-role.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * CREATE SCHEMA
     *
     * @presto https://prestodb.io/docs/current/sql/create-schema.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * CREATE TABLE
     *
     * @presto https://prestodb.io/docs/current/sql/create-table.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * CREATE TABLE AS
     *
     * @presto https://prestodb.io/docs/current/sql/create-table-as.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * CREATE VIEW
     *
     * @presto https://prestodb.io/docs/current/sql/create-view.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * DEALLOCATE PREPARE
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/querying-with-prepared-statements.html
     * @presto https://prestodb.io/docs/current/sql/deallocate-prepare.html
     */
    | DEALLOCATE PREPARE identifier[null]                                                                               #deallocate

    /**
     * DELETE
     *
     * @presto https://prestodb.io/docs/current/sql/delete.html
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented

    /**
     * DESCRIBE
     *
     * @presto https://prestodb.io/docs/current/sql/describe.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * DESCRIBE INPUT
     *
     * @presto https://prestodb.io/docs/current/sql/describe-input.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * DESCRIBE OUTPUT
     *
     * @presto https://prestodb.io/docs/current/sql/describe-output.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * DROP FUNCTION
     *
     * @presto https://prestodb.io/docs/current/sql/drop-function.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * DROP ROLE
     *
     * @presto https://prestodb.io/docs/current/sql/drop-role.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * DROP SCHEMA
     *
     * @presto https://prestodb.io/docs/current/sql/drop-schema.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * DROP TABLE
     *
     * @presto https://prestodb.io/docs/current/sql/drop-table.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * DROP VIEW
     *
     * @presto https://prestodb.io/docs/current/sql/drop-view.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * EXECUTE
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/querying-with-prepared-statements.html
     * @presto https://prestodb.io/docs/current/sql/execute.html
     */
    | EXECUTE identifier[null] (USING expression (',' expression)*)?                                                    #execute

    /**
     * EXPLAIN
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/athena-explain-statement.html
     * @presto https://prestodb.io/docs/current/sql/explain.html
     */
    | EXPLAIN VERBOSE?
        ('(' explainOption (',' explainOption)* ')')? statement                                                         #explain

    /**
     * EXPLAIN ANALYZE
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/athena-explain-statement.html
     * @presto https://prestodb.io/docs/current/sql/explain-analyze.html
     */
    | EXPLAIN ANALYZE VERBOSE?
        ('(' explainOption (',' explainOption)* ')')? statement                                                         #explainAnalyze

    /**
     * GRANT
     *
     * @presto https://prestodb.io/docs/current/sql/grant.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * GRANT ROLES
     *
     * @presto https://prestodb.io/docs/current/sql/grant-roles.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * INSERT
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/insert-into.html
     * @presto https://prestodb.io/docs/current/sql/insert.html
     */
    | INSERT INTO qualifiedName columnAliases? query                                                                    #insertInto

    /**
     * PREPARE
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/querying-with-prepared-statements.html
     * @presto 
     */
    | PREPARE identifier[null] FROM statement                                                                           #prepare

    /**
     * RESET SESSION
     *
     * @presto https://prestodb.io/docs/current/sql/reset-session.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * REVOKE
     *
     * @presto https://prestodb.io/docs/current/sql/revoke.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * REVOKE ROLES
     *
     * @presto https://prestodb.io/docs/current/sql/revoke-roles.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * ROLLBACK
     *
     * @presto https://prestodb.io/docs/current/sql/rollback.html
     * @notsupport https://docs.aws.amazon.com/athena/latest/ug/unsupported-ddl.html
     */
    // Not Implemented

    /**
     * SELECT
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/select.html
     * @presto https://prestodb.io/docs/current/sql/select.html
     */
    | query                                                                                                             #statementDefault

    /**
     * SET ROLE
     *
     * @presto https://prestodb.io/docs/current/sql/set-role.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SET SESSION
     *
     * @presto https://prestodb.io/docs/current/sql/set-session.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW CATALOGS
     *
     * @presto https://prestodb.io/docs/current/sql/show-catalogs.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW COLUMNS
     *
     * @presto https://prestodb.io/docs/current/sql/show-columns.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW CREATE FUNCTION
     *
     * @presto https://prestodb.io/docs/current/sql/show-create-function.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW CREATE TABLE
     *
     * @presto https://prestodb.io/docs/current/sql/show-create-table.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW CREATE VIEW
     *
     * @presto https://prestodb.io/docs/current/sql/show-create-view.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW FUNCTIONS
     *
     * @presto https://prestodb.io/docs/current/sql/show-functions.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW GRANTS
     *
     * @presto https://prestodb.io/docs/current/sql/show-grants.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW ROLE GRANTS
     *
     * @presto https://prestodb.io/docs/current/sql/show-role-grants.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW ROLES
     *
     * @presto https://prestodb.io/docs/current/sql/show-roles.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW SCHEMAS
     *
     * @presto https://prestodb.io/docs/current/sql/show-schemas.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW SESSION
     *
     * @presto https://prestodb.io/docs/current/sql/show-session.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW STATS
     *
     * @presto https://prestodb.io/docs/current/sql/show-stats.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * SHOW TABLES
     *
     * @presto https://prestodb.io/docs/current/sql/show-tables.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * START TRANSACTION
     *
     * @presto https://prestodb.io/docs/current/sql/start-transaction.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * USE
     *
     * @presto https://prestodb.io/docs/current/sql/use.html
     * @notsupport Use hive syntax
     */
    // Not Implemented

    /**
     * VALUES
     *
     * @presto https://prestodb.io/docs/current/sql/values.html
     */
    // Implemented in "query"
    
    /******************************************************************************************************************/
    
    /**
     * Athena Only Statements
     */
     
    /**
     * UNLOAD
     *
     * @athena https://docs.aws.amazon.com/athena/latest/ug/unload.html
     */
    | UNLOAD '(' query ')' 
        TO location=string WITH properties                                                                              #unload
    ;

query
    : with? queryNoWith
    ;

with
    : WITH namedQuery (',' namedQuery)*
    ;

//tableElement
//    : columnDefinition
//    | likeClause
//    ;

columnDefinition
    : identifier[null] dataType (COMMENT comment=string)?
    ;

//likeClause
//    : LIKE qualifiedName (optionType=(INCLUDING | EXCLUDING) PROPERTIES)?
//    ;

rowFormat
    : DELIMITED
        (FIELDS TERMINATED BY string (ESCAPED BY string)?)?
        (COLLECTION ITEMS TERMINATED BY string)?
        (MAP KEYS TERMINATED BY string)?
        (LINES TERMINATED BY string)?
        (NULL DEFINED AS string)?
    | SERDE string
        (WITH SERDEPROPERTIES stringProperties)?
    ;

fileFormat
    : SEQUENCEFILE
    | TEXTFILE
    | RCFILE
    | ORC
    | PARQUET
    | AVRO
    | JSONFILE
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

//sqlParameterDeclaration
//    : identifier[null] type
//    ;

//routineCharacteristics
//    : routineCharacteristic*
//    ;

//routineCharacteristic
//    : LANGUAGE language
//    | determinism
//    | nullCallClause
//    ;

//alterRoutineCharacteristics
//    : alterRoutineCharacteristic*
//    ;

//alterRoutineCharacteristic
//    : nullCallClause
//    ;

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

//language
//    : SQL
//    | identifier[null]
//    ;
//
//determinism
//    : DETERMINISTIC
//    | NOT DETERMINISTIC;
//
//nullCallClause
//    : RETURNS NULL ON NULL INPUT
//    | CALLED ON NULL INPUT
//    ;

externalRoutineName
    : identifier[null]
    ;

queryNoWith
    : queryTerm
        orderBy?
        limitOffsetTerm?
    ;

limitOffsetTerm
    : offsetTerm limitTerm
    | offsetTerm 
    | limitTerm
    ;

offsetTerm
    : OFFSET rowCountTerm ROW?
    | OFFSET rowCountTerm ROWS
    ;
    
limitTerm
    : LIMIT rowCountTerm
    | LIMIT ALL
    ;

rowCountTerm
    : INTEGER_VALUE
    | parameterExpression
    ;

queryTerm
    : queryPrimary                                                                                                      #queryTermDefault
    | left=queryTerm operator=(UNION | EXCEPT | INTERSECT) setQuantifier? right=queryTerm                               #setOperation
    ;

queryPrimary
    : querySpecification                                                                                                #queryPrimaryDefault
    | TABLE qualifiedName                                                                                               #table
    | VALUES expression (',' expression)*                                                                               #inlineTable
    | '(' queryNoWith  ')'                                                                                              #subquery
    ;

sortItem
    : expression ordering=(ASC | DESC)? (NULLS nullOrdering=(FIRST | LAST))?
    ;

querySpecification
    : SELECT
        setQuantifier?
        selectItem (',' selectItem)*
        fromTerm?
        whereTerm?
        groupByHavingTerm?
    ;
    
fromTerm
    : FROM relation (',' relation)*
    ;
    
whereTerm
    : WHERE where=booleanExpression
    ;

groupByHavingTerm
    : GROUP BY setQuantifier? groupingElement (',' groupingElement)*
        HAVING having=booleanExpression
    | GROUP BY setQuantifier? groupingElement (',' groupingElement)*
    | HAVING having=booleanExpression
    ;

//groupBy
//    : setQuantifier? groupingElement (',' groupingElement)*
//    ;

groupingElement
    : groupingSet                                            #singleGroupingSet
    | ROLLUP '(' (expression (',' expression)*)? ')'         #rollup
    | CUBE '(' (expression (',' expression)*)? ')'           #cube
    | GROUPING SETS '(' groupingSet (',' groupingSet)* ')'   #multipleGroupingSets
    ;

groupingSet
    : '(' (expression (',' expression)*)? ')'                #multipleExpressionGroupingSet
    | expression                                             #singleExpressionGroupingSet
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
    | qualifiedName '.' ASTERISK          #selectAll
    | ASTERISK                            #selectAll
    ;

relation
    : left=relation
      (
        CROSS JOIN right=sampledRelation
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
    
viewColumnAliases
    : '('
        columnName+=identifier[null] (COMMENT string)?
        (',' columnName+=identifier[null] (COMMENT string)? )*
      ')'
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
    | parameterExpression                                                                 #parameter
    | POSITION '(' valueExpression IN valueExpression ')'                                 #position
    | '(' expression (',' expression)+ ')'                                                #rowConstructor
    | ROW '(' expression (',' expression)* ')'                                            #rowConstructor
    | qualifiedName '(' ASTERISK ')' filter? over?                                        #functionCall
    | qualifiedName '(' (setQuantifier? expression (',' expression)*)?
        orderBy? ')' filter? (nullTreatment? over)?                                       #functionCall
    | identifier[null] '->' expression                                                    #lambda
    | '(' (identifier[null] (',' identifier[null])*)? ')' '->' expression                 #lambda
    | '(' query ')'                                                                       #subqueryExpression
    // This is an extension to ANSI SQL, which considers EXISTS to be a <boolean expression>
    | EXISTS '(' query ')'                                                                #exists
    | CASE valueExpression whenClause+ (ELSE elseExpression=expression)? END              #simpleCase
    | CASE whenClause+ (ELSE elseExpression=expression)? END                              #searchedCase
    | CAST '(' expression AS dataType ')'                                                 #cast
    | TRY_CAST '(' expression AS dataType ')'                                             #cast
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

timeZoneSpecifier
    : TIME ZONE interval  #timeZoneInterval
    | TIME ZONE string    #timeZoneString
    ;

interval
    : INTERVAL sign=(PLUS | MINUS)? string from=intervalField (TO to=intervalField)?
    ;
    
orderBy
    : ORDER BY sortItem (',' sortItem)*
    ;
    
string
    : STRING                                #basicStringLiteral
    | UNICODE_STRING (UESCAPE STRING)?      #unicodeStringLiteral
    ;

number
    : DECIMAL_VALUE  #decimalLiteral
    | DOUBLE_VALUE   #doubleLiteral
    | INTEGER_VALUE  #integerLiteral
    ;
    
nullTreatment
    : IGNORE NULLS
    | RESPECT NULLS
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

intervalField
    : YEAR | MONTH | DAY | HOUR | MINUTE | SECOND
    ;

normalForm
    : NFD | NFC | NFKD | NFKC
    ;

//dataTypes
//    : '(' (dataType (',' dataType)*)? ')'
//    ;

dataType
    : primitiveType
    | arrayType
    | mapType
    | structType
    | unionType
    ;

primitiveType
    : TINYINT
    | SMALLINT
    | INT
    | BIGINT
    | BOOLEAN
    | FLOAT
    | DOUBLE
    | DOUBLE PRECISION
    | STRING
    | BINARY
    | TIMESTAMP
    | DECIMAL
    | DECIMAL '(' INTEGER_VALUE ',' INTEGER_VALUE ')'
    | DATE
    | VARCHAR
    | CHAR
    ;


arrayType
    :  ARRAY '<' dataType '>'
    ;
    
mapType
    : MAP < primitiveType, dataType >
    ;
    
structType
    : STRUCT
        '<'
            columnName=identifier[null] ':' dataType (COMMENT comment=string)?
            (',' columnName=identifier[null] ':' dataType (COMMENT comment=string)? )*
        '>'
    ;
    
unionType
    : UNIONTYPE
        '<' dataType (',' dataType)* '>'
    ;
    
    
parameterExpression returns [int index]
    : '?' { $index = NextBindParameterIndex(); }
    ;

//type
//    : type ARRAY
//    | ARRAY '<' type '>'
//    | MAP '<' type ',' type '>'
//    | ROW '(' identifier[null] type (',' identifier[null] type)* ')'
//    | baseType ('(' typeParameter (',' typeParameter)* ')')?
//    | INTERVAL from=intervalField TO to=intervalField
//    ;

//typeParameter
//    : INTEGER_VALUE | type
//    ;
//
//baseType
//    : TIME_WITH_TIME_ZONE
//    | TIMESTAMP_WITH_TIME_ZONE
//    | DOUBLE_PRECISION
//    | qualifiedName
//    ;

whenClause
    : WHEN condition=expression THEN result=expression
    ;

filter
    : FILTER '(' WHERE booleanExpression ')'
    ;

over
    : OVER '(' windowSpecification ')'
    ;

windowSpecification:
    partitionBy?
    orderBy?
    windowFrame?
    ;

partitionBy:
    PARTITION BY partition+=expression (',' partition+=expression)*
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

//transactionMode
//    : ISOLATION LEVEL levelOfIsolation    #isolationLevel
//    | READ accessMode=(ONLY | WRITE)      #transactionAccessMode
//    ;

//levelOfIsolation
//    : READ UNCOMMITTED                    #readUncommitted
//    | READ COMMITTED                      #readCommitted
//    | REPEATABLE READ                     #repeatableRead
//    | SERIALIZABLE                        #serializable
//    ;

//callArgument
//    : expression                          #positionalArgument
//    | identifier[null] '=>' expression    #namedArgument
//    ;

//privilege
//    : SELECT | DELETE | INSERT | identifier[null]
//    ;

qualifiedName returns [QsiQualifiedIdentifier qqi] locals [List<QsiIdentifier> buffer]
    @init { $buffer = new List<QsiIdentifier>(); }
    @after { $qqi = new QsiQualifiedIdentifier($buffer); }
    : identifier[$buffer] ('.' identifier[$buffer] )*
    ;

//grantor
//    : CURRENT_USER          #currentUserGrantor
//    | CURRENT_ROLE          #currentRoleGrantor
//    | principal             #specifiedPrincipal
//    ;

//principal
//    : USER identifier[null]       #userPrincipal
//    | ROLE identifier[null]       #rolePrincipal
//    | identifier[null]            #unspecifiedPrincipal
//    ;

//roles
//    : identifier[null] (',' identifier[null])*
//    ;

identifier[List<QsiIdentifier> buffer] returns [QsiIdentifier qi]
    @after { $buffer?.Add($qi); }
    : i=IDENTIFIER { $qi = new QsiIdentifier($i.text, false); }             #unquotedIdentifier
    | i=QUOTED_IDENTIFIER { $qi = new QsiIdentifier($i.text, true); }                 #quotedIdentifier
    | ki=nonReserved { $qi = new QsiIdentifier($ki.text, false); }          #unquotedIdentifier
    | i=BACKQUOTED_IDENTIFIER { $qi = new QsiIdentifier($i.text, true); }             #backQuotedIdentifier
    | i=DIGIT_IDENTIFIER { $qi = new QsiIdentifier($i.text, false); }       #digitIdentifier
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
    | SYNC | PROTECTION | PURGE | SKEWED | DIRECTORIES | SORTED | REMOTE | MANAGEDLOCATION;

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
SKEWED: 'SKEWED';
DIRECTORIES: 'DIRECTORIES';
SORTED: 'SORTED';
JSONFILE: 'JSONFILE';
REMOTE: 'REMOTE';
MANAGEDLOCATION: 'MANAGEDLOCATION';

TINYINT: 'TINYINT';
BIGINT: 'BIGINT';
INT: 'INT';
SMALLINT: 'SMALLINT';
BOOLEAN: 'BOOLEAN';
FLOAT: 'FLOAT';
DOUBLE: 'DOUBLE';
PRECISION: 'PRECISION';
BINARY: 'BINARY';
DECIMAL: 'DECIMAL';
VARCHAR: 'VARCHAR';
CHAR: 'CHAR';
STRUCT: 'STRUCT';
UNIONTYPE: 'UNIONTYPE';

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
