parser grammar ImpalaParserInternal;

options { 
    superClass = ImpalaBaseParser;
    tokenVocab = ImpalaLexerInternal;
}

@header {
    using Qsi.Data;
    using Qsi.Tree;
    using Qsi.Utilities;
}

root
    : stmt (SEMICOLON+ | EOF)
    ;

stmt
    : query_stmt
    | use_stmt
    | upsert_stmt
    | update_stmt
    | truncate_stmt
    | show_tables_stmt
    | show_stats_stmt
    | show_roles_stmt
    | show_range_partitions_stmt
    | show_partitions_stmt
    | show_grant_principal_stmt
    | show_functions_stmt
    | show_files_stmt
    | show_dbs_stmt
    | show_data_srcs_stmt
    | show_create_tbl_stmt
    | show_create_function_stmt
    | set_stmt
    | revoke_role_stmt
    | revoke_privilege_stmt
    | reset_metadata_stmt
    | load_stmt
    | insert_stmt
    | grant_role_stmt
    | grant_privilege_stmt
    | explain_stmt
    | drop_tbl_or_view_stmt
    | drop_stats_stmt
    | drop_function_stmt
    | drop_db_stmt
    | drop_data_src_stmt
    | describe_stmt
    | delete_stmt
    | create_view_stmt
    | create_udf_stmt
    | create_uda_stmt
    | create_tbl_stmt
    | create_tbl_like_stmt
    | create_tbl_as_select_stmt
    | create_drop_role_stmt
    | create_db_stmt
    | create_data_src_stmt
    | copy_testcase_stmt
    | compute_stats_stmt
    | comment_on_stmt
    | alter_view_stmt
    | alter_tbl_stmt
    | alter_db_stmt
    | admin_fn_stmt
    ;

load_stmt
    : KW_LOAD KW_DATA KW_INPATH STRING_LITERAL KW_OVERWRITE? KW_INTO KW_TABLE table_name partition_spec?
    ;

truncate_stmt
    : KW_TRUNCATE KW_TABLE? if_exists_val? table_name
    ;

reset_metadata_stmt
    : KW_REFRESH table_name partition_spec?
    | KW_REFRESH KW_FUNCTIONS ident_or_default
    | KW_REFRESH KW_AUTHORIZATION
    | KW_INVALIDATE KW_METADATA table_name?
    ;

explain_stmt
    : KW_EXPLAIN upsert_stmt
    | KW_EXPLAIN update_stmt
    | KW_EXPLAIN query_stmt
    | KW_EXPLAIN insert_stmt
    | KW_EXPLAIN delete_stmt
    | KW_EXPLAIN create_tbl_as_select_stmt
    ;

copy_testcase_stmt
    : KW_COPY testcase_ident KW_TO STRING_LITERAL query_stmt
    | KW_COPY testcase_ident KW_FROM STRING_LITERAL
    ;

insert_stmt
    : opt_with_clause? KW_INSERT plan_hints? (KW_OVERWRITE | KW_INTO) KW_TABLE? table_name partition_clause? query_stmt
    | opt_with_clause? KW_INSERT plan_hints? (KW_OVERWRITE | KW_INTO) KW_TABLE? table_name LPAREN ident_list? RPAREN partition_clause? query_stmt?
    | opt_with_clause? KW_INSERT (KW_OVERWRITE | KW_INTO) KW_TABLE? table_name partition_clause? plan_hints? query_stmt
    | opt_with_clause? KW_INSERT (KW_OVERWRITE | KW_INTO) KW_TABLE? table_name LPAREN ident_list? RPAREN partition_clause? plan_hints? query_stmt?
    ;

update_stmt
    : KW_UPDATE dotted_path KW_SET update_set_expr_list from_clause? where_clause?
    ;

update_set_expr_list
    : slot_ref EQUAL expr (COMMA slot_ref EQUAL expr)*
    ;

upsert_stmt
    : opt_with_clause? KW_UPSERT plan_hints? KW_INTO KW_TABLE? table_name query_stmt
    | opt_with_clause? KW_UPSERT plan_hints? KW_INTO KW_TABLE? table_name LPAREN ident_list? RPAREN query_stmt?
    | opt_with_clause? KW_UPSERT KW_INTO KW_TABLE? table_name plan_hints? query_stmt
    | opt_with_clause? KW_UPSERT KW_INTO KW_TABLE? table_name LPAREN ident_list? RPAREN plan_hints? query_stmt?
    ;

delete_stmt
    : KW_DELETE KW_FROM? dotted_path where_clause?
    | KW_DELETE dotted_path from_clause where_clause?
    ;

show_roles_stmt
    : KW_SHOW KW_ROLE KW_GRANT KW_GROUP ident_or_default
    | KW_SHOW KW_CURRENT? KW_ROLES
    ;

show_grant_principal_stmt
    : KW_SHOW KW_GRANT principal_type ident_or_default (KW_ON uri_ident STRING_LITERAL)?
    | KW_SHOW KW_GRANT principal_type ident_or_default KW_ON server_ident
    | KW_SHOW KW_GRANT principal_type ident_or_default KW_ON KW_TABLE table_name
    | KW_SHOW KW_GRANT principal_type ident_or_default KW_ON KW_DATABASE ident_or_default
    | KW_SHOW KW_GRANT principal_type ident_or_default KW_ON KW_COLUMN column_name
    ;

create_drop_role_stmt
    : KW_DROP KW_ROLE ident_or_default
    | KW_CREATE KW_ROLE ident_or_default
    ;

grant_role_stmt
    : KW_GRANT KW_ROLE ident_or_default KW_TO KW_GROUP ident_or_default
    ;

revoke_role_stmt
    : KW_REVOKE KW_ROLE ident_or_default KW_FROM KW_GROUP ident_or_default
    ;

grant_privilege_stmt
    : KW_GRANT privilege_spec KW_TO (KW_ROLE | KW_GROUP | IDENT)? ident_or_default opt_with_grantopt?
    ;

revoke_privilege_stmt
    : KW_REVOKE opt_grantopt_for? privilege_spec KW_FROM (KW_ROLE | KW_GROUP | IDENT)? ident_or_default
    ;

privilege_spec
    : privilege (LPAREN ident_list? RPAREN)? KW_ON KW_TABLE table_name
    | privilege KW_ON uri_ident STRING_LITERAL
    | privilege KW_ON server_ident ident_or_default?
    | privilege KW_ON KW_DATABASE ident_or_default
    ;

privilege
    : KW_SELECT
    | KW_REFRESH
    | KW_INSERT
    | KW_DROP
    | KW_CREATE
    | KW_ALTER
    | KW_ALL
    ;

principal_type
    : KW_ROLE
    | KW_GROUP
    | IDENT
    ;

opt_grantopt_for
    : KW_GRANT option_ident KW_FOR
    ;

opt_with_grantopt
    : KW_WITH KW_GRANT option_ident
    ;

partition_def
    : partition_spec location_val? cache_op_val?
    ;

partition_def_list
    : partition_def+
    ;

comment_on_stmt
    : KW_COMMENT KW_ON (KW_VIEW | KW_TABLE) table_name KW_IS nullable_comment_val
    | KW_COMMENT KW_ON KW_DATABASE ident_or_default KW_IS nullable_comment_val
    | KW_COMMENT KW_ON KW_COLUMN column_name KW_IS nullable_comment_val
    ;

alter_db_stmt
    : KW_ALTER KW_DATABASE ident_or_default KW_SET IDENT (KW_ROLE | IDENT) ident_or_default
    ;

alter_tbl_stmt
    : KW_ALTER KW_TABLE table_name partition_set? KW_UNSET table_property_type if_exists_val? LPAREN properties_key_list RPAREN
    | KW_ALTER KW_TABLE table_name partition_set? KW_SET table_property_type LPAREN properties_map RPAREN
    | KW_ALTER KW_TABLE table_name partition_set? KW_SET KW_LOCATION STRING_LITERAL
    | KW_ALTER KW_TABLE table_name partition_set? KW_SET KW_FILEFORMAT file_format_val
    | KW_ALTER KW_TABLE table_name partition_set? KW_SET IDENT (KW_ROLE | IDENT) ident_or_default
    | KW_ALTER KW_TABLE table_name partition_set? KW_SET cache_op_val
    | KW_ALTER KW_TABLE table_name partition_set? KW_SET row_format_val
    | KW_ALTER KW_TABLE table_name partition_set? KW_SET KW_COLUMN KW_STATS ident_or_default LPAREN properties_map RPAREN
    | KW_ALTER KW_TABLE table_name KW_SORT KW_BY (KW_ZORDER | KW_LEXICAL)? LPAREN ident_list? RPAREN
    | KW_ALTER KW_TABLE table_name KW_REPLACE KW_COLUMNS LPAREN column_def_list RPAREN
    | KW_ALTER KW_TABLE table_name KW_RENAME KW_TO table_name
    | KW_ALTER KW_TABLE table_name KW_RECOVER KW_PARTITIONS
    | KW_ALTER KW_TABLE table_name KW_DROP KW_COLUMN? ident_or_default
    | KW_ALTER KW_TABLE table_name KW_DROP if_exists_val? partition_set KW_PURGE?
    | KW_ALTER KW_TABLE table_name KW_DROP if_exists_val? KW_RANGE range_param
    | KW_ALTER KW_TABLE table_name KW_CHANGE KW_COLUMN? ident_or_default column_def
    | KW_ALTER KW_TABLE table_name KW_ALTER KW_COLUMN? ident_or_default KW_SET column_options_map
    | KW_ALTER KW_TABLE table_name KW_ALTER KW_COLUMN? ident_or_default KW_DROP KW_DEFAULT
    | KW_ALTER KW_TABLE table_name KW_ADD KW_COLUMN if_not_exists_val? column_def
    | KW_ALTER KW_TABLE table_name KW_ADD if_not_exists_val? partition_def_list
    | KW_ALTER KW_TABLE table_name KW_ADD if_not_exists_val? KW_RANGE range_param
    | KW_ALTER KW_TABLE table_name KW_ADD if_not_exists_val? KW_COLUMNS LPAREN column_def_list RPAREN
    ;

table_property_type
    : KW_TBLPROPERTIES
    | KW_SERDEPROPERTIES
    ;

create_db_stmt
    : KW_CREATE db_or_schema_kw if_not_exists_val? ident_or_default opt_comment_val? location_val? managed_location_val?
    ;

create_tbl_as_select_stmt
    : KW_CREATE plan_hints? create_tbl_as_select_params
    ;

create_tbl_as_select_params
    : tbl_def_without_col_defs (primary_keys partitioned_data_layout? | iceberg_partition_spec_list)? tbl_options KW_AS query_stmt
    | tbl_def_without_col_defs KW_PARTITIONED KW_BY LPAREN ident_list RPAREN tbl_options KW_AS query_stmt
    ;

create_tbl_stmt
    : KW_CREATE tbl_def_without_col_defs partition_column_defs? tbl_options
    | KW_CREATE tbl_def_without_col_defs KW_LIKE file_format_val STRING_LITERAL opt_tbl_data_layout tbl_options
    | KW_CREATE tbl_def_with_col_defs opt_tbl_data_layout tbl_options
    | KW_CREATE tbl_def_with_col_defs KW_PRODUCED KW_BY KW_DATA source_ident ident_or_default opt_init_string_val? opt_comment_val?
    ;

create_tbl_like_stmt
    : KW_CREATE tbl_def_without_col_defs opt_sort_cols?? KW_LIKE table_name opt_comment_val? file_format_create_table_val? location_val?
    ;

tbl_def_without_col_defs
    : KW_EXTERNAL? KW_TABLE if_not_exists_val? table_name
    ;

tbl_def_with_col_defs
    : tbl_def_without_col_defs LPAREN column_def_list (COMMA primary_keys)? RPAREN
    | tbl_def_without_col_defs LPAREN column_def_list COMMA primary_keys enable_spec? validate_spec? rely_spec? (COMMA foreign_keys_list)? RPAREN
    | tbl_def_without_col_defs LPAREN column_def_list COMMA foreign_keys_list (COMMA primary_keys enable_spec? validate_spec? rely_spec?)? RPAREN
    ;

foreign_keys_list
    : KW_FOREIGN key_ident LPAREN ident_list RPAREN KW_REFERENCES table_name LPAREN ident_list RPAREN enable_spec? validate_spec? rely_spec? (COMMA KW_FOREIGN key_ident LPAREN ident_list RPAREN KW_REFERENCES table_name LPAREN ident_list RPAREN enable_spec? validate_spec? rely_spec?)*
    ;

primary_keys
    : KW_PRIMARY key_ident LPAREN ident_list RPAREN
    ;

rely_spec
    : KW_RELY
    | KW_NORELY
    ;

validate_spec
    : KW_VALIDATE
    | KW_NOVALIDATE
    ;

enable_spec
    : KW_ENABLE
    | KW_DISABLE
    ;

tbl_options
    : opt_sort_cols? opt_comment_val? row_format_val? serde_properties? file_format_create_table_val? location_val? cache_op_val? tbl_properties?
    ;

opt_sort_cols
    : KW_SORT KW_BY (KW_ZORDER | KW_LEXICAL)? LPAREN ident_list? RPAREN
    ;

opt_tbl_data_layout
    : partitioned_data_layout?
    | partition_column_defs
    ;

partitioned_data_layout
    : partition_param_list
    | iceberg_partition_spec_list
    ;

partition_column_defs
    : KW_PARTITIONED KW_BY LPAREN column_def_list RPAREN
    ;

partition_param_list
    : KW_PARTITION KW_BY range_partition_param (COMMA hash_partition_param_list)?
    | KW_PARTITION KW_BY hash_partition_param_list (COMMA range_partition_param)?
    ;

hash_partition_param_list
    : hash_partition_param (COMMA hash_partition_param)*
    ;

hash_partition_param
    : KW_HASH (LPAREN ident_list RPAREN)? KW_PARTITIONS INTEGER_LITERAL
    ;

range_partition_param
    : KW_RANGE LPAREN (ident_list RPAREN LPAREN)? range_params_list RPAREN
    ;

range_params_list
    : range_param (COMMA range_param)*
    ;

range_param
    : KW_PARTITION opt_lower_range_val? KW_VALUES opt_upper_range_val?
    | KW_PARTITION dotted_path EQUAL LPAREN expr_list RPAREN
    | KW_PARTITION dotted_path EQUAL expr
    ;

opt_lower_range_val
    : LPAREN expr_list RPAREN LESSTHAN EQUAL?
    | expr LESSTHAN EQUAL?
    ;

opt_upper_range_val
    : LESSTHAN EQUAL? LPAREN expr_list RPAREN
    | LESSTHAN EQUAL? expr
    ;

iceberg_partition_spec_list
    : iceberg_partition_spec_defs
    ;

iceberg_partition_spec_defs
    : iceberg_partition_spec_def (COMMA iceberg_partition_spec_def)*
    ;

iceberg_partition_spec_def
    : iceberg_partition_field_list
    ;

iceberg_partition_field_list
    : KW_PARTITION KW_BY KW_SPEC LPAREN iceberg_partition_field_defs RPAREN
    ;

iceberg_partition_field_defs
    : iceberg_partition_field_def (COMMA iceberg_partition_field_def)*
    ;

iceberg_partition_field_def
    : IDENT iceberg_partition_transform
    ;

iceberg_partition_transform
    : IDENT
    | iceberg_partition_transform_type INTEGER_LITERAL
    ;

iceberg_partition_transform_type
    : KW_TRUNCATE
    | IDENT
    ;

create_udf_stmt
    : KW_CREATE KW_FUNCTION if_not_exists_val? function_name (function_def_args KW_RETURNS type_def)? KW_LOCATION STRING_LITERAL function_def_args_map?
    ;

create_uda_stmt
    : KW_CREATE KW_AGGREGATE KW_FUNCTION if_not_exists_val? function_name function_def_args KW_RETURNS type_def opt_aggregate_fn_intermediate_type_def? KW_LOCATION STRING_LITERAL function_def_args_map?
    ;

cache_op_val
    : KW_UNCACHED
    | KW_CACHED KW_IN STRING_LITERAL opt_cache_op_replication?
    ;

opt_cache_op_replication
    : KW_WITH KW_REPLICATION EQUAL INTEGER_LITERAL
    ;

comment_val
    : KW_COMMENT STRING_LITERAL
    ;

opt_comment_val
    : KW_COMMENT STRING_LITERAL
    ;

nullable_comment_val
    : STRING_LITERAL
    | KW_NULL
    ;

location_val
    : KW_LOCATION STRING_LITERAL
    ;

managed_location_val
    : KW_MANAGED_LOCATION STRING_LITERAL
    ;

opt_init_string_val
    : LPAREN STRING_LITERAL RPAREN
    ;

if_not_exists_val
    : KW_IF KW_NOT KW_EXISTS
    ;

row_format_val
    : KW_ROW KW_FORMAT KW_DELIMITED field_terminator_val? escaped_by_val? line_terminator_val?
    ;

escaped_by_val
    : KW_ESCAPED KW_BY STRING_LITERAL
    ;

line_terminator_val
    : KW_LINES terminator_val
    ;

field_terminator_val
    : KW_FIELDS terminator_val
    ;

terminator_val
    : KW_TERMINATED KW_BY STRING_LITERAL
    ;

file_format_create_table_val
    : KW_STORED KW_AS file_format_val
    ;

file_format_val
    : KW_TEXTFILE
    | KW_SEQUENCEFILE
    | KW_RCFILE
    | KW_PARQUETFILE
    | KW_PARQUET
    | KW_ORC
    | KW_KUDU
    | KW_ICEBERG
    | KW_HUDIPARQUET
    | KW_AVRO
    ;

tbl_properties
    : KW_TBLPROPERTIES LPAREN properties_map RPAREN
    ;

serde_properties
    : KW_WITH KW_SERDEPROPERTIES LPAREN properties_map RPAREN
    ;

properties_map
    : STRING_LITERAL EQUAL STRING_LITERAL (COMMA STRING_LITERAL EQUAL STRING_LITERAL)*
    ;

properties_key_list
    : STRING_LITERAL (COMMA STRING_LITERAL)*
    ;

column_def_list
    : column_def (COMMA column_def)*
    ;

column_def
    : ident_or_default type_def column_options_map?
    ;

column_options_map
    : column_option+
    ;

column_option
    : nullability_val
    | is_primary_key_val
    | encoding_val
    | default_val
    | compression_val
    | comment_val
    | block_size_val
    ;

is_primary_key_val
    : KW_PRIMARY key_ident
    ;

nullability_val
    : KW_NOT? KW_NULL
    ;

encoding_val
    : KW_ENCODING word
    ;

compression_val
    : KW_COMPRESSION word
    ;

default_val
    : KW_DEFAULT expr
    ;

block_size_val
    : KW_BLOCKSIZE literal
    ;

create_view_stmt
    : KW_CREATE KW_VIEW if_not_exists_val? table_name view_column_defs? opt_comment_val? KW_AS query_stmt
    ;

create_data_src_stmt
    : KW_CREATE KW_DATA source_ident if_not_exists_val? ident_or_default KW_LOCATION STRING_LITERAL KW_CLASS STRING_LITERAL KW_API_VERSION STRING_LITERAL
    ;

key_ident
    : i=IDENT { VerifyTokenIgnoreCase($i, "KEY"); }
    ;

system_ident
    : i=IDENT { VerifyTokenIgnoreCase($i, "SYSTEM"); }
    ;

source_ident
    : i=IDENT { VerifyTokenIgnoreCase($i, "SOURCE"); }
    ;

sources_ident
    : i=IDENT { VerifyTokenIgnoreCase($i, "SOURCES"); }
    ;

uri_ident
    : i=IDENT { VerifyTokenIgnoreCase($i, "URI"); }
    ;

server_ident
    : i=IDENT { VerifyTokenIgnoreCase($i, "SERVER"); }
    ;

testcase_ident
    : i=IDENT { VerifyTokenIgnoreCase($i, "TESTCASE"); }
    ;

option_ident
    : i=IDENT { VerifyTokenIgnoreCase($i, "OPTION"); }
    ;

view_column_defs
    : LPAREN view_column_def_list RPAREN
    ;

view_column_def_list
    : view_column_def (COMMA view_column_def)*
    ;

view_column_def
    : ident_or_default opt_comment_val?
    ;

alter_view_stmt
    : KW_ALTER KW_VIEW table_name view_column_defs? KW_AS query_stmt
    | KW_ALTER KW_VIEW table_name KW_SET IDENT (KW_ROLE | IDENT) ident_or_default
    | KW_ALTER KW_VIEW table_name KW_RENAME KW_TO table_name
    ;

cascade_val
    : KW_RESTRICT
    | KW_CASCADE
    ;

compute_stats_stmt
    : KW_COMPUTE KW_STATS table_name (LPAREN ident_list? RPAREN)? opt_tablesample?
    | KW_COMPUTE KW_INCREMENTAL KW_STATS table_name partition_set? (LPAREN ident_list? RPAREN)?
    ;

drop_stats_stmt
    : KW_DROP KW_STATS table_name
    | KW_DROP KW_INCREMENTAL KW_STATS table_name partition_set
    ;

drop_db_stmt
    : KW_DROP db_or_schema_kw if_exists_val? ident_or_default cascade_val?
    ;

drop_tbl_or_view_stmt
    : KW_DROP KW_VIEW if_exists_val? table_name
    | KW_DROP KW_TABLE if_exists_val? table_name KW_PURGE?
    ;

drop_function_stmt
    : KW_DROP KW_AGGREGATE? KW_FUNCTION if_exists_val? function_name function_def_args?
    ;

drop_data_src_stmt
    : KW_DROP KW_DATA source_ident if_exists_val? ident_or_default
    ;

db_or_schema_kw
    : KW_SCHEMA
    | KW_DATABASE
    ;

dbs_or_schemas_kw
    : KW_SCHEMAS
    | KW_DATABASES
    ;

if_exists_val
    : KW_IF KW_EXISTS
    ;

partition_clause
    : KW_PARTITION LPAREN partition_key_value_list RPAREN
    ;

partition_key_value_list
    : partition_key_value (COMMA partition_key_value)*
    ;

partition_set
    : KW_PARTITION LPAREN expr_list RPAREN
    ;

partition_spec
    : KW_PARTITION LPAREN static_partition_key_value_list RPAREN
    ;

static_partition_key_value_list
    : static_partition_key_value (COMMA static_partition_key_value)*
    ;

partition_key_value
    : static_partition_key_value
    | ident_or_default
    ;

static_partition_key_value
    : ident_or_default EQUAL expr
    ;

function_def_args
    : LPAREN (function_def_arg_list DOTDOTDOT?)? RPAREN
    ;

function_def_arg_list
    : type_def (COMMA type_def)*
    ;

opt_aggregate_fn_intermediate_type_def
    : KW_INTERMEDIATE type_def
    ;

function_def_args_map
    : (function_def_arg_key EQUAL STRING_LITERAL)+
    ;

function_def_arg_key
    : KW_UPDATE_FN
    | KW_SYMBOL
    | KW_SERIALIZE_FN
    | KW_PREPARE_FN
    | KW_MERGE_FN
    | KW_INIT_FN
    | KW_FINALIZE_FN
    | KW_COMMENT
    | KW_CLOSE_FN
    ;

query_stmt
    : opt_with_clause? set_operand_list
    ;

opt_with_clause
    : KW_WITH with_view_def_list
    ;

with_view_def
    : STRING_LITERAL (LPAREN ident_list RPAREN)? KW_AS LPAREN query_stmt RPAREN
    | ident_or_default (LPAREN ident_list RPAREN)? KW_AS LPAREN query_stmt RPAREN
    ;

with_view_def_list
    : with_view_def (COMMA with_view_def)*
    ;

set_operand
    : values_stmt
    | select_stmt
    | LPAREN query_stmt RPAREN
    ;

set_operand_list
    : sets+=set_operand (ops+=set_op sets+=set_operand)*
      opt_order_by_clause?
      opt_limit_offset_clause
    ;

set_op
    : (KW_MINUS | KW_INTERSECT | KW_EXCEPT) KW_DISTINCT?
    | KW_UNION (KW_DISTINCT | KW_ALL)
    ;

values_stmt
    : KW_VALUES values_operand_list opt_order_by_clause? opt_limit_offset_clause
    | KW_VALUES LPAREN values_operand_list RPAREN opt_order_by_clause? opt_limit_offset_clause
    ;

values_operand_list
    : values_operand+
    ;

values_operand
    : LPAREN select_list RPAREN
    ;

use_stmt
    : KW_USE ident_or_default
    ;

show_tables_stmt
    : KW_SHOW KW_TABLES (KW_IN ident_or_default)? show_pattern?
    ;

show_dbs_stmt
    : KW_SHOW dbs_or_schemas_kw show_pattern?
    ;

show_stats_stmt
    : KW_SHOW (KW_TABLE | KW_COLUMN) KW_STATS table_name
    ;

show_partitions_stmt
    : KW_SHOW KW_PARTITIONS table_name
    ;

show_range_partitions_stmt
    : KW_SHOW KW_RANGE KW_PARTITIONS table_name
    ;

show_functions_stmt
    : KW_SHOW opt_function_category? KW_FUNCTIONS (KW_IN ident_or_default)? show_pattern?
    ;

opt_function_category
    : KW_ANALYTIC
    | KW_AGGREGATE
    ;

show_data_srcs_stmt
    : KW_SHOW KW_DATA sources_ident show_pattern?
    ;

show_pattern
    : KW_LIKE? STRING_LITERAL
    ;

show_create_tbl_stmt
    : KW_SHOW KW_CREATE show_create_tbl_object_type table_name
    ;

show_create_tbl_object_type
    : KW_VIEW
    | KW_TABLE
    ;

show_create_function_stmt
    : KW_SHOW KW_CREATE KW_AGGREGATE? KW_FUNCTION function_name
    ;

show_files_stmt
    : KW_SHOW KW_FILES KW_IN table_name partition_set?
    ;

describe_stmt
    : KW_DESCRIBE (KW_FORMATTED | KW_EXTENDED)? dotted_path
    | KW_DESCRIBE IDENT table_name
    | KW_DESCRIBE db_or_schema_kw describe_output_style? ident_or_default
    ;

describe_output_style
    : KW_FORMATTED
    | KW_EXTENDED
    ;

select_stmt
    : select      = select_clause
      (
      from        = from_clause
      where       = where_clause?
      groupBy     = group_by_clause?
      having      = having_clause?
      orderBy     = opt_order_by_clause?
      limitOffset = opt_limit_offset_clause
      )?
    ;

select_clause
    : KW_SELECT (KW_DISTINCT | KW_ALL)? plan_hints? select_list
    ;

set_stmt
    : KW_SET KW_ALL?
    | KW_SET ident_or_default EQUAL word
    | KW_SET ident_or_default EQUAL SUBTRACT? numeric_literal
    | KW_SET ident_or_default EQUAL STRING_LITERAL
    ;

admin_fn_stmt
    : COLON ident_or_default LPAREN expr_list? RPAREN
    ;

select_list
    : items+=select_list_item (COMMA items+=select_list_item)*
    ;

select_list_item
    : star_expr
    | expr alias_clause?
    ;

alias_clause
    : KW_AS? STRING_LITERAL
    | KW_AS? ident_or_default
    ;

star_expr
    : (dotted_path DOT)? STAR
    ;

table_name
    : (ident_or_default DOT)? ident_or_default
    ;

column_name
    : (ident_or_default DOT)? ident_or_default DOT ident_or_default
    ;

function_name
    : KW_GROUPING
    | dotted_path
    ;

from_clause
    : KW_FROM table_ref_list
    ;

table_ref_list
    : table_ref                                                                            #singleTableRef
    | <assoc=right> left=table_ref_list KW_CROSS KW_JOIN hint=plan_hints? right=table_ref  #crossJoin
    | <assoc=right> left=table_ref_list join_operator hint=plan_hints? right=table_ref
      (
        KW_ON onClause=expr
        | KW_USING LPAREN usingClause=ident_list RPAREN
      )?                                                                                   #join
    | <assoc=right> left=table_ref_list COMMA right=table_ref                              #commaJoin
    ;

table_ref
    : LPAREN query=query_stmt RPAREN alias=alias_clause  sample=opt_tablesample? hint=plan_hints?
    | reference=dotted_path          alias=alias_clause? sample=opt_tablesample? hint=plan_hints? 
    ;

join_operator
    : KW_INNER? KW_JOIN
    | KW_LEFT KW_OUTER? KW_JOIN
    | KW_LEFT KW_SEMI KW_JOIN
    | KW_LEFT KW_ANTI KW_JOIN
    | KW_RIGHT KW_OUTER? KW_JOIN
    | KW_RIGHT KW_SEMI KW_JOIN
    | KW_RIGHT KW_ANTI KW_JOIN
    | KW_FULL KW_OUTER? KW_JOIN
    ;

plan_hints
    : LBRACKET plan_hint_list RBRACKET
    | KW_STRAIGHT_JOIN
    | COMMENTED_PLAN_HINT_START plan_hint_list COMMENTED_PLAN_HINT_END
    ;

plan_hint
    : KW_STRAIGHT_JOIN
    | IDENT (LPAREN (INTEGER_LITERAL | ident_list) RPAREN)?
    ;

plan_hint_list
    : plan_hint? (COMMA plan_hint?)*
    ;

opt_tablesample
    : KW_TABLESAMPLE system_ident LPAREN INTEGER_LITERAL RPAREN
      (KW_REPEATABLE LPAREN INTEGER_LITERAL RPAREN)?
    ;

ident_list
    : ident_or_default (COMMA ident_or_default)*
    ;

expr_list
    : expr (COMMA expr)*
    ;

where_clause
    : KW_WHERE hint=plan_hints? expr
    ;

group_by_clause
    : KW_GROUP KW_BY (KW_ROLLUP | KW_CUBE) LPAREN expr_list RPAREN   #groupBy1
    | KW_GROUP KW_BY KW_GROUPING KW_SETS LPAREN grouping_sets RPAREN #groupBy2
    | KW_GROUP KW_BY expr_list (KW_WITH (KW_ROLLUP | KW_CUBE))?      #groupBy3
    ;

grouping_set
    : LPAREN expr_list? RPAREN
    | expr
    ;

grouping_sets
    : grouping_set (COMMA grouping_set)*
    ;

having_clause
    : KW_HAVING expr
    ;

opt_order_by_clause
    : KW_ORDER KW_BY order_by_elements
    ;

order_by_elements
    : order_by_element (COMMA order_by_element)*
    ;

order_by_element
    : expr opt_order_param? opt_nulls_order_param?
    ;

opt_order_param
    : KW_DESC
    | KW_ASC
    ;

opt_nulls_order_param
    : KW_NULLS KW_LAST
    | KW_NULLS KW_FIRST
    ;

opt_limit_offset_clause
    : limit=opt_limit_clause? offset=opt_offset_clause?
    ;

opt_limit_clause
    : KW_LIMIT expr
    ;

opt_offset_clause
    : KW_OFFSET expr
    ;

cast_format_val
    : KW_FORMAT STRING_LITERAL
    ;

cast_expr
    : KW_CAST LPAREN expr KW_AS type_def cast_format_val? RPAREN
    ;

case_expr
    : KW_CASE expr? case_when_clause_list case_else_clause? KW_END
    ;

case_when_clause_list
    : (KW_WHEN expr KW_THEN expr)+
    ;

case_else_clause
    : KW_ELSE expr
    ;

//sign_chain_expr
//    : SUBTRACT expr
//    | ADD expr
//    ;

expr returns [bool p]
    : LPAREN expr RPAREN                                                                        {$p=true;} #expr_parens

    // predicate
    | <assoc=right> expr KW_LOGICAL_OR expr                                                     {$p=true;} #predicate1
    | <assoc=right> expr KW_IS KW_NOT? KW_NULL                                                  {$p=true;} #predicate2

    // predicate > like_predicate
    | <assoc=right> expr KW_NOT? (KW_RLIKE | KW_REGEXP | KW_LIKE | KW_IREGEXP | KW_ILIKE) expr  {$p=true;} #like_predicate

    // predicate > in_predicate
    | <assoc=right> expr KW_NOT? KW_IN subquery                                                 {$p=true;} #in_predicate_subquery
    | <assoc=right> expr KW_NOT? KW_IN LPAREN expr_list RPAREN                                  {$p=true;} #in_predicate

    // predicate > exists_predicate
    | KW_EXISTS subquery                                                                        {$p=true;} #exists_predicate

    // predicate > compound_predicate
    | (NOT | KW_NOT) expr                                                                       {$p=true;} #compound_predicate1
    | <assoc=right> expr (KW_OR | KW_AND) expr                                                  {$p=true;} #compound_predicate2

    // predicate > comparison_predicate
    | <assoc=right> expr (NOTEQUAL | LESSTHAN | EQUAL) expr                                     {$p=true;} #comparison_predicate1
    | <assoc=right> expr (NOT | LESSTHAN) EQUAL expr                                            {$p=true;} #comparison_predicate2
    | <assoc=right> expr LESSTHAN EQUAL? GREATERTHAN expr                                       {$p=true;} #comparison_predicate3
    | <assoc=right> expr KW_IS KW_NOT? KW_DISTINCT KW_FROM expr                                 {$p=true;} #comparison_predicate4
    | <assoc=right> expr GREATERTHAN EQUAL? expr                                                {$p=true;} #comparison_predicate5

    // predicate > bool_test_expr
    | <assoc=right> expr KW_IS KW_NOT? (KW_UNKNOWN | KW_TRUE | KW_FALSE)                        {$p=true;} #bool_test_expr

    // predicate > between_predicate
    | <assoc=right> expr KW_NOT? KW_BETWEEN expr KW_AND expr                                    {$p=true;} #between_predicate

    // non_pred_expr
    | slot_ref                                                                                             #slot_ref_
    | (SUBTRACT | ADD) expr                                                                                #sign_chain_expr
    | literal                                                                                              #literal_
    | function_call_expr                                                                                   #function_call_expr_
    | cast_expr                                                                                            #cast_expr_
    | case_expr                                                                                            #case_expr_ 
    | analytic_expr                                                                                        #analytic_expr_

    // non_pred_expr > timestamp_arithmetic_expr
    | KW_INTERVAL expr IDENT ADD expr                                                                      #timestamp_arithmetic_expr1
    | function_name LPAREN expr_list COMMA KW_INTERVAL expr IDENT RPAREN                                   #timestamp_arithmetic_expr2
    | expr (SUBTRACT | ADD) KW_INTERVAL expr IDENT                                                         #timestamp_arithmetic_expr3

    // non_pred_expr > arithmetic_expr
    | expr (SUBTRACT | STAR | MOD | KW_DIV | DIVIDE | BITXOR | BITOR | BITAND | ADD) expr                  #arithmetic_expr
    | expr NOT                                                                                             #arithmetic_expr_factorial
    | BITNOT expr                                                                                          #arithmetic_expr_bitnot

    | subquery                                                                                             #subquery_
    ;

//exists_predicate
//    : KW_EXISTS subquery
//    ;

function_call_expr
    : (KW_TRUNCATE | KW_RIGHT | KW_REPLACE | KW_LEFT | KW_IF) LPAREN expr_list RPAREN
    | function_name LPAREN (ident_or_default KW_FROM expr | function_params)? RPAREN
    ;

analytic_expr
    : function_call_expr KW_OVER LPAREN opt_partition_by_clause? opt_order_by_clause? opt_window_clause? RPAREN
    ;

opt_partition_by_clause
    : KW_PARTITION KW_BY expr_list
    ;

opt_window_clause
    : window_type (KW_BETWEEN window_boundary KW_AND)? window_boundary
    ;

window_type
    : KW_ROWS
    | KW_RANGE
    ;

window_boundary
    : KW_UNBOUNDED KW_PRECEDING
    | KW_UNBOUNDED KW_FOLLOWING
    | KW_CURRENT KW_ROW
    | expr KW_PRECEDING
    | expr KW_FOLLOWING
    ;

//arithmetic_expr
//    : expr (SUBTRACT | STAR | MOD | KW_DIV | DIVIDE | BITXOR | BITOR | BITAND | ADD) expr
//    | expr NOT
//    | BITNOT expr
//    ;

//timestamp_arithmetic_expr
//    : KW_INTERVAL expr IDENT ADD expr
//    | function_name LPAREN expr_list COMMA KW_INTERVAL expr IDENT RPAREN
//    | expr (SUBTRACT | ADD) KW_INTERVAL expr IDENT
//    ;

numeric_literal
    : INTEGER_LITERAL
    | DECIMAL_LITERAL
    ;

literal
    : l=UNMATCHED_STRING_LITERAL //expr
      {
        // we have an unmatched string literal.
        // to correctly report the root cause of this syntax error
        // we must force parsing to fail at this point,
        // and generate an unmatched string literal symbol
        // to be passed as the last seen token in the
        // error handling routine (otherwise some other token could be reported)
        ParseError($l, null);
      }
    | l=NUMERIC_OVERFLOW
      {
        // similar to the unmatched string literal case
        // we must terminate parsing at this point
        // and generate a corresponding symbol to be reported
        ParseError($l, null);
      }
    | numeric_literal
    | KW_NULL
    | KW_TRUE
    | KW_FALSE
    | STRING_LITERAL
    | KW_DATE STRING_LITERAL
    ;

function_params
    : KW_DISTINCT expr_list
    | KW_ALL? STAR
    | KW_ALL? expr_list
    | expr_list KW_IGNORE KW_NULLS
    ;

//comparison_predicate
//    : expr (NOTEQUAL | LESSTHAN | EQUAL) expr
//    | expr (NOT | LESSTHAN) EQUAL expr
//    | expr LESSTHAN EQUAL? GREATERTHAN expr
//    | expr KW_IS KW_NOT? KW_DISTINCT KW_FROM expr
//    | expr GREATERTHAN EQUAL? expr
//    ;

//like_predicate
//    : expr KW_NOT? (KW_RLIKE | KW_REGEXP | KW_LIKE | KW_IREGEXP | KW_ILIKE) expr
//    ;

//between_predicate
//    : expr KW_NOT? KW_BETWEEN (predicate | non_pred_expr) KW_AND expr
//    ;

//in_predicate
//    : expr KW_NOT? KW_IN subquery
//    | expr KW_NOT? KW_IN LPAREN expr_list RPAREN
//    ;

//bool_test_expr
//    : expr KW_IS KW_NOT? KW_UNKNOWN
//    | expr KW_IS KW_NOT? KW_TRUE
//    | expr KW_IS KW_NOT? KW_FALSE
//    ;

subquery
    : LPAREN (nest=subquery | query_stmt) RPAREN
    ;

//compound_predicate
//    : NOT expr
//    | KW_NOT expr
//    | expr (KW_OR | KW_AND) expr
//    ;

slot_ref
    : dotted_path
    ;

dotted_path
    : ident_or_default (DOT ident_or_default)*
    ;

type_def
    : type
    ;

type
    : KW_VARCHAR (LPAREN INTEGER_LITERAL RPAREN)?
    | KW_TINYINT
    | KW_TIMESTAMP
    | KW_STRUCT LESSTHAN struct_field_def_list GREATERTHAN
    | KW_STRING
    | KW_SMALLINT
    | KW_MAP LESSTHAN type COMMA type GREATERTHAN
    | KW_INT
    | KW_FLOAT
    | KW_DOUBLE
    | KW_DECIMAL (LPAREN INTEGER_LITERAL (COMMA INTEGER_LITERAL)? RPAREN)?
    | KW_DATETIME
    | KW_DATE
    | KW_CHAR LPAREN INTEGER_LITERAL RPAREN
    | KW_BOOLEAN
    | KW_BINARY
    | KW_BIGINT
    | KW_ARRAY LESSTHAN type GREATERTHAN
    ;

struct_field_def
    : word COLON type opt_comment_val?
    ;

struct_field_def_list
    : struct_field_def (COMMA struct_field_def)*
    ;

ident_or_default
    : KW_DEFAULT
    | IDENT
    ;

word
    : UNUSED_RESERVED_WORD
    | KW_ZORDER
    | KW_WITH
    | KW_WHERE
    | KW_WHEN
    | KW_VIEW
    | KW_VARCHAR
    | KW_VALUES
    | KW_VALIDATE
    | KW_USING
    | KW_USE
    | KW_UPSERT
    | KW_UPDATE
    | KW_UPDATE_FN
    | KW_UNSET
    | KW_UNKNOWN
    | KW_UNION
    | KW_UNCACHED
    | KW_UNBOUNDED
    | KW_TRUNCATE
    | KW_TRUE
    | KW_TO
    | KW_TINYINT
    | KW_TIMESTAMP
    | KW_THEN
    | KW_TEXTFILE
    | KW_TERMINATED
    | KW_TBLPROPERTIES
    | KW_TABLESAMPLE
    | KW_TABLES
    | KW_TABLE
    | KW_SYMBOL
    | KW_STRUCT
    | KW_STRING
    | KW_STRAIGHT_JOIN
    | KW_STORED
    | KW_STATS
    | KW_SPEC
    | KW_SORT
    | KW_SMALLINT
    | KW_SHOW
    | KW_SETS
    | KW_SET
    | KW_SERIALIZE_FN
    | KW_SERDEPROPERTIES
    | KW_SEQUENCEFILE
    | KW_SEMI
    | KW_SELECT
    | KW_SCHEMAS
    | KW_SCHEMA
    | KW_ROWS
    | KW_ROW
    | KW_ROLLUP
    | KW_ROLES
    | KW_ROLE
    | KW_RLIKE
    | KW_RIGHT
    | KW_REVOKE
    | KW_RETURNS
    | KW_RESTRICT
    | KW_REPLICATION
    | KW_REPLACE
    | KW_REPEATABLE
    | KW_RENAME
    | KW_RELY
    | KW_REGEXP
    | KW_REFRESH
    | KW_REFERENCES
    | KW_RECOVER
    | KW_RCFILE
    | KW_RANGE
    | KW_PURGE
    | KW_PRODUCED
    | KW_PRIMARY
    | KW_PREPARE_FN
    | KW_PRECEDING
    | KW_PARTITIONS
    | KW_PARTITIONED
    | KW_PARTITION
    | KW_PARQUETFILE
    | KW_PARQUET
    | KW_OVERWRITE
    | KW_OVER
    | KW_OUTER
    | KW_ORDER
    | KW_ORC
    | KW_OR
    | KW_ON
    | KW_OFFSET
    | KW_NULLS
    | KW_NULL
    | KW_NOVALIDATE
    | KW_NOT
    | KW_NORELY
    | KW_MINUS
    | KW_METADATA
    | KW_MERGE_FN
    | KW_MAP
    | KW_MANAGED_LOCATION
    | KW_LOGICAL_OR
    | KW_LOCATION
    | KW_LOAD
    | KW_LINES
    | KW_LIMIT
    | KW_LIKE
    | KW_LEXICAL
    | KW_LEFT
    | KW_LAST
    | KW_KUDU
    | KW_JOIN
    | KW_IS
    | KW_IREGEXP
    | KW_INVALIDATE
    | KW_INTO
    | KW_INTERVAL
    | KW_INTERSECT
    | KW_INTERMEDIATE
    | KW_INT
    | KW_INSERT
    | KW_INPATH
    | KW_INNER
    | KW_INIT_FN
    | KW_INCREMENTAL
    | KW_IN
    | KW_ILIKE
    | KW_IGNORE
    | KW_IF
    | KW_ICEBERG
    | KW_HUDIPARQUET
    | KW_HAVING
    | KW_HASH
    | KW_GROUPING
    | KW_GROUP
    | KW_GRANT
    | KW_FUNCTIONS
    | KW_FUNCTION
    | KW_FULL
    | KW_FROM
    | KW_FORMATTED
    | KW_FORMAT
    | KW_FOREIGN
    | KW_FOR
    | KW_FOLLOWING
    | KW_FLOAT
    | KW_FIRST
    | KW_FINALIZE_FN
    | KW_FILES
    | KW_FILEFORMAT
    | KW_FIELDS
    | KW_FALSE
    | KW_EXTERNAL
    | KW_EXTENDED
    | KW_EXPLAIN
    | KW_EXISTS
    | KW_EXCEPT
    | KW_ESCAPED
    | KW_END
    | KW_ENCODING
    | KW_ENABLE
    | KW_ELSE
    | KW_DROP
    | KW_DOUBLE
    | KW_DIV
    | KW_DISTINCT
    | KW_DISABLE
    | KW_DESCRIBE
    | KW_DESC
    | KW_DELIMITED
    | KW_DELETE
    | KW_DEFAULT
    | KW_DECIMAL
    | KW_DATETIME
    | KW_DATE
    | KW_DATABASES
    | KW_DATABASE
    | KW_DATA
    | KW_CURRENT
    | KW_CUBE
    | KW_CROSS
    | KW_CREATE
    | KW_COPY
    | KW_CONSTRAINT
    | KW_COMPUTE
    | KW_COMPRESSION
    | KW_COMMENT
    | KW_COLUMNS
    | KW_COLUMN
    | KW_CLOSE_FN
    | KW_CLASS
    | KW_CHAR
    | KW_CHANGE
    | KW_CAST
    | KW_CASE
    | KW_CASCADE
    | KW_CACHED
    | KW_BY
    | KW_BOOLEAN
    | KW_BLOCKSIZE
    | KW_BINARY
    | KW_BIGINT
    | KW_BETWEEN
    | KW_AVRO
    | KW_AUTHORIZATION
    | KW_ASC
    | KW_AS
    | KW_ARRAY
    | KW_API_VERSION
    | KW_ANTI
    | KW_AND
    | KW_ANALYTIC
    | KW_ALTER
    | KW_ALL
    | KW_AGGREGATE
    | KW_ADD
    | IDENT
    ;
