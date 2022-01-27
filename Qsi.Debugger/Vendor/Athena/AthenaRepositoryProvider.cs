using System;
using Qsi.Data;
using Qsi.Data.Object;

namespace Qsi.Debugger.Vendor.Athena
{
   internal class AthenaRepositoryProvider : VendorRepositoryProvider
    {
        protected override QsiQualifiedIdentifier ResolveQualifiedIdentifier(QsiQualifiedIdentifier identifier)
        {
            identifier = identifier.Level switch
            {
                1 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("AwsDataCatalog", false),
                    new QsiIdentifier("default", false),
                    identifier[0]
                ),
                2 => new QsiQualifiedIdentifier(
                    new QsiIdentifier("AwsDataCatalog", false),
                    identifier[0],
                    identifier[1]
                ),
                _ => identifier
            };

            if (identifier.Level != 3)
                throw new InvalidOperationException();

            return identifier;
        }

        protected override QsiTableStructure LookupTable(QsiQualifiedIdentifier identifier)
        {
            if (identifier.Level < 1) return null;

            if (identifier[0].Compare("AwsDataCatalog")) return LookUpTableInAwsDataCatalog(identifier);

            return null;
        }

        private QsiTableStructure LookUpTableInAwsDataCatalog(QsiQualifiedIdentifier identifier)
        {
            if (identifier.Level < 2) return null;
            
            if (identifier[1].Compare("default")) return LookUpTableInAwsDataCatalogDefault(identifier);

            return null;
        }

        private QsiTableStructure LookUpTableInAwsDataCatalogDefault(QsiQualifiedIdentifier identifier)
        {
            if (identifier.Level < 3) return null;

            if (identifier[2].Compare("elb_logs"))
            {
                var table = CreateTable("AwsDataCatalog", "default", "elb_logs");
                AddColumns(table, "request_timestamp", "elb_name", "request_ip", "request_port", "backend_ip", "backend_port", "request_processing_time", "backend_processing_time", "client_response_time", "elb_response_code", "backend_response_code", "received_bytes", "sent_bytes", "request_verb", "url", "protocol", "user_agent", "ssl_cipher", "ssl_protocol");

                return table;
            }

            if (identifier[2].Compare("elb_logs_from_chrome"))
            {
                var view = CreateTable("AwsDataCatalog", "default", "elb_logs_from_chrome");
                view.Type = QsiTableType.View;
                AddColumns(view, "request_timestamp", "elb_name", "request_ip", "request_port", "backend_ip", "backend_port", "request_processing_time", "backend_processing_time", "client_response_time", "elb_response_code", "backend_response_code", "received_bytes", "sent_bytes", "request_verb", "url", "protocol", "user_agent", "ssl_cipher", "ssl_protocol");
                
                return view;
            }

            if (identifier[3].Compare("elb_logs_from_edge")) {
                var view = CreateTable("AwsDataCatalog", "default", "elb_logs_from_chrome");
                view.Type = QsiTableType.View;
                AddColumns(view, "request_timestamp", "elb_name", "request_ip", "request_port", "backend_ip", "backend_port", "request_processing_time", "backend_processing_time", "client_response_time", "elb_response_code", "backend_response_code", "received_bytes", "sent_bytes", "request_verb", "url", "protocol", "user_agent", "ssl_cipher", "ssl_protocol");
                
                return view;
            }

            return null;
        }

        protected override QsiScript LookupDefinition(QsiQualifiedIdentifier identifier, QsiTableType type)
        {
            Console.WriteLine($"{type.ToString()} {identifier}");
            if (identifier.Compare("AwsDataCatalog", "default", "elb_logs"))
            {
                const string script = @"CREATE EXTERNAL TABLE `elb_logs`(
  `request_timestamp` string COMMENT '', 
  `elb_name` string COMMENT '', 
  `request_ip` string COMMENT '', 
  `request_port` int COMMENT '', 
  `backend_ip` string COMMENT '', 
  `backend_port` int COMMENT '', 
  `request_processing_time` double COMMENT '', 
  `backend_processing_time` double COMMENT '', 
  `client_response_time` double COMMENT '', 
  `elb_response_code` string COMMENT '', 
  `backend_response_code` string COMMENT '', 
  `received_bytes` bigint COMMENT '', 
  `sent_bytes` bigint COMMENT '', 
  `request_verb` string COMMENT '', 
  `url` string COMMENT '', 
  `protocol` string COMMENT '', 
  `user_agent` string COMMENT '', 
  `ssl_cipher` string COMMENT '', 
  `ssl_protocol` string COMMENT '')
ROW FORMAT SERDE 
  'org.apache.hadoop.hive.serde2.RegexSerDe' 
WITH SERDEPROPERTIES ( 
  'input.regex'='([^ ]*) ([^ ]*) ([^ ]*):([0-9]*) ([^ ]*):([0-9]*) ([.0-9]*) ([.0-9]*) ([.0-9]*) (-|[0-9]*) (-|[0-9]*) ([-0-9]*) ([-0-9]*) \\\""([^ ]*) ([^ ]*) (- |[^ ]*)\\\"" (\""[^\""]*\"") ([A-Z0-9-]+) ([A-Za-z0-9.-]*)$') 
STORED AS INPUTFORMAT 
  'org.apache.hadoop.mapred.TextInputFormat' 
OUTPUTFORMAT 
  'org.apache.hadoop.hive.ql.io.HiveIgnoreKeyTextOutputFormat'
LOCATION
  's3://athena-examples-ap-northeast-2/elb/plaintext'
TBLPROPERTIES (
  'last_modified_by'='hadoop', 
  'last_modified_time'='1635146167', 
  'numFiles'='42', 
  'numRows'='-1', 
  'rawDataSize'='-1', 
  'totalSize'='406582288', 
  'transient_lastDdlTime'='1635216089')
";

                return new QsiScript(script, QsiScriptType.Create);
            }

            if (identifier.Compare("AwsDataCatalog", "default", "elb_logs_from_chrome"))
            {
                const string script = @"CREATE VIEW sampledb.elb_logs_from_chrome AS
SELECT *
FROM
  sampledb.elb_logs
WHERE (user_agent LIKE '%Chrome/%')
";

                return new QsiScript(script, QsiScriptType.Create);
            }
            
            if (identifier.Compare("AwsDataCatalog", "default", "elb_logs_from_edge"))
            {
                const string script = @"CREATE VIEW sampledb.elb_logs_from_edge AS
SELECT *
FROM
  sampledb.elb_logs
WHERE (user_agent LIKE '%Edge/%')
";
                return new QsiScript(script, QsiScriptType.Create);
            }

            if (identifier.Compare("prepared_stmt_select_1") && type == QsiTableType.Prepared)
            {
                const string script = @"SELECT * FROM elb_logs WHERE user_agent LIKE ?";
                
                return new QsiScript(script, QsiScriptType.Select);
            }

            return null;
        }

        protected override QsiVariable LookupVariable(QsiQualifiedIdentifier identifier)
        {
            if (identifier.Compare("AwsDataCatalog", "default", "prepared_stmt_select_1")) {
                return new QsiVariable
                {
                    Identifier = CreateIdentifier("prepared_stmt_select_1"),
                    Type = QsiDataType.String,
                    Value = @"SELECT * FROM elb_logs WHERE user_agent LIKE ?"
                };
            }

            return null;
        }

        protected override QsiObject LookupObject(QsiQualifiedIdentifier identifier, QsiObjectType type)
        {
            return null;
        }
    }
}
