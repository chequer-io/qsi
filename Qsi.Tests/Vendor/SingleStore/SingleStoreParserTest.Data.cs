using System.Linq;
using NUnit.Framework;

namespace Qsi.Tests.SingleStore;

public sealed partial class SingleStoreParserTest
{
    public static TestCaseData[] ValidQuery_Select_TestCaseData
        => _validQuery_Select_TestCaseData ??= GetTestCaseData(ValidQuery_Select);

    private static TestCaseData[] _validQuery_Select_TestCaseData;

    private static TestCaseData[] GetTestCaseData(string[] queries)
        => queries.Select(q => new TestCaseData(q)).ToArray();

    #region Test Queries
    /// <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/data-manipulation-language-dml/select/"/>
    /// <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/operational-commands/select-global/"/>
    /// <see cref="https://docs.singlestore.com/db/v8.5/reference/sql-reference/data-manipulation-language-dml/table/"/>
    public static readonly string[] ValidQuery_Select =
    {
        "SELECT * FROM hrRec;",
        "SELECT * FROM hrRec LIMIT 2;",
        "SELECT * FROM hrRec LIMIT 1,2;",
        "SELECT * FROM hrRec LIMIT 2 OFFSET 1;",
        "SELECT * FROM Orders;",
        "SELECT * FROM Orders WHERE OrderNumber = 3 FOR UPDATE;",
        "SELECT test1.t1.*, test2.t2.* FROM test1.t1 JOIN test2.t2 ON t1.id = t2.id;",
        "SELECT * FROM my_MemSQL_table WHERE col = 1;",
        "SELECT COUNT(*), user_name, page_url from clicks, users, pages WHERE clicks.user_id = users.user_id AND pages.page_id = clicks.page_id GROUP BY users.user_id, pages.page_id ORDER BY COUNT(*) DESC;",
        "SELECT t1.*, t2.* FROM t1 FULL OUTER JOIN t2 ON t1.a = t2.a;",
        "SELECT cust_id FROM customers WHERE EXISTS ( SELECT order_id FROM orders WHERE order_id IN (SELECT id FROM transaction WHERE count > 5));",
        "DELETE FROM records WHERE id = ( SELECT order_id FROM orders WHERE order_date > ( SELECT CURRENT_DATE() + 30) );",
        "SELECT employees.emp_name AS employees, asset_desc as assets, assets.asset_type FROM assets JOIN employees USING (emp_id);",
        "SELECT FirstName, LastName, City, Tenure INTO fname, lname, city, ten FROM hrRec WHERE Tenure > 40;",
        "SELECT COUNT(*), SUM(Tenure) INTO row_c, sum_t FROM hrRec;",
        "SELECT City INTO cnum FROM hrRec WHERE Tenure > 50;",
        "SELECT * FROM Product;",
        "SELECT ALL Brand_name FROM Product;",
        "SELECT DISTINCT Brand_name FROM Product;",
        "SELECT * FROM table_name INTO OUTFILE '/home/username/file_name.csv' FIELDS TERMINATED BY ',' LINES TERMINATED BY '\\n'",
        "SELECT * FROM mason_test INTO AZURE 'container/blob-prefix' CREDENTIALS '{\"json\": \"testing\"}' WITH COMPRESSION GZIP",
        "SELECT * FROM stockTo INTO HDFS 'hdfs://hadoop-namenode:8020/stock_dir/records_file.csv'",
        "SELECT * FROM t1 INTO LINK S3con 'testing/output';",
        "SELECT * FROM table_name INTO FS '/tmp/a' FIELDS TERMINATED BY ',' LINES TERMINATED BY '\\n'",
        "SELECT * FROM t1 INTO S3 'testing/output' CONFIG '{\"region\":\"us-east-1\"}' CREDENTIALS '{\"aws_access_key_id\":\"your_access_key_id\",\"aws_secret_access_key\":\"your_secret_access_key\"}';",
        "SELECT t1.a, t2.a FROM t1, t2 WHERE t1.a = t2.a GROUP BY t1.a INTO S3 'bucket_name/file_name' CONFIG '{\"region\":\"us-east-1\", \"multipart_chunk_size_mb\":100}' CREDENTIALS '{\"role_arn\": \"arn:aws:iam::<AccountID>:role/EC2AmazonS3FullAccess\"}'",
        "SELECT * FROM t INTO S3 'tmp/a' CONFIG '{\"region\":\"us-east-1\"}' CREDENTIALS '{\"aws_access_key_id\":\"your_access_key_id\",\"aws_secret_access_key\":\"your_secret_access_key\"}' FIELDS TERMINATED BY ',' LINES TERMINATED BY '\\n'",
        "SELECT * FROM table_name INTO GCS 'bucket/path' CREDENTIALS '{\"access_id\": \"replace_with_your_google_access_key\", \"secret_key\": \"replace_with_your_google_secret_key\"}' FIELDS TERMINATED BY ',' LINES TERMINATED BY '\\n';",
        "SELECT col1, col2, col3 FROM t ORDER BY col1 INTO KAFKA 'host.example.com:9092/test-topic' FIELDS TERMINATED BY ',' ENCLOSED BY '\"' ESCAPED BY \"\\t\" LINES TERMINATED BY '}' STARTING BY '{';",
        "SELECT col1, col2, col3 FROM t ORDER BY col1 INTO KAFKA 'host.example.com:9092/test-topic' FIELDS TERMINATED BY '\\t' ENCLOSED BY '' ESCAPED BY '\\\\' LINES TERMINATED BY '\\n' STARTING BY '';",
        "SELECT text FROM t INTO KAFKA 'host.example.com:9092/test-topic' CONFIG '{\"security.protocol\": \"ssl\", \"ssl.certificate.location\": \"/var/private/ssl/client_memsql_client.pem\", \"ssl.key.location\": \"/var/private/ssl/client_memsql_client.key\", \"ssl.ca.location\": \"/var/private/ssl/ca-cert.pem\"}' CREDENTIALS '{\"ssl.key.password\": \"abcdefgh\"}'",
        "SELECT text FROM t INTO KAFKA 'host.example.com:9092/test-topic'",
        "SELECT text FROM t INTO KAFKA 'host1.example.com:9092,host2.example.com:9092,host3.example.com:9092/test-topic'",
        "SELECT column_b, column_a, column_b, column_c FROM kktest INTO KAFKA 'localhost:1234/kafkaWithKey' KAFKA KEY \"column_b\";",
        "SELECT * FROM table1 into fs '/tmp/parquet_files' FORMAT PARQUET;",
        "SELECT * FROM table1 INTO OUTFILE '/tmp/parquet_files3' FORMAT PARQUET;",
        "SELECT num FROM example_table WITH (SAMPLE_RATIO = 0.7) ORDER BY num;",
        "SELECT AVG(num) FROM example_table WITH (SAMPLE_RATIO = 0.8);",
        "SELECT c.name, o.order_id, o.order_total FROM customer WITH (SAMPLE_RATIO = 0.4) c JOIN order o ON c.customer_id = o.customer_id;",
        "SELECT c.name, o.order_id, o.order_total FROM customer c JOIN order o WITH (SAMPLE_RATIO = 0.4) ON c.customer_id = o.customer_id;",

        "SELECT @@GLOBAL.redundancy_level;",

        "SELECT * FROM TABLE([1,2,3]);",
        "SELECT * FROM TABLE([\"hello\", \"world\"]);",
        "SELECT * FROM TABLE(JSON_TO_ARRAY('[1,2,3]'));",
        "SELECT * FROM num, TABLE([1,2]);",
        "SELECT num, table_col AS \"SQUARE\" FROM square INNER JOIN TABLE(to_array(6)) ON table_col = num*num ORDER BY num;",
        "SELECT Name, table_col AS \"Title\" FROM empRole JOIN TABLE(JSON_TO_ARRAY(Role));"
    };
    #endregion
}
