using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Qsi.Tests.SingleStore;

public sealed partial class SingleStoreParserTest
{
    private static TestCaseData[] GetAllValidQueryTestCaseDatas
    {
        get
        {
            return Enumerable.Empty<string>()
                .Concat(ValidQuery_Select)
                .Concat(ValidQuery_Insert)
                .Concat(ValidQuery_Update)
                .Concat(ValidQuery_Delete)
                .Concat(ValidQuery_Set)
                .Concat(ValidQuery_CreateView)
                .Select(q => new TestCaseData(q))
                .ToArray();
        }
    }

    private static IEnumerable<TestCaseData> GetTestCaseData(string[] query)
        => query.Select(q => new TestCaseData(q));

    #region Test Queries
    private static readonly string[] ValidQuery_Select =
    {
        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/data-manipulation-language-dml/select/"/>
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
        "SELECT c.name, o.order_id, o.order_total FROM customer c WITH (SAMPLE_RATIO = 0.4) JOIN order_tbl o ON c.customer_id = o.customer_id;",
        "SELECT c.name, o.order_id, o.order_total FROM customer c JOIN order_tbl o WITH (SAMPLE_RATIO = 0.4) ON c.customer_id = o.customer_id;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/operational-commands/select-global/"/>
        "SELECT @@GLOBAL.redundancy_level;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/data-manipulation-language-dml/table/"/>
        "SELECT * FROM TABLE([1,2,3]);",
        "SELECT * FROM TABLE([\"hello\", \"world\"]);",
        "SELECT * FROM TABLE(JSON_TO_ARRAY('[1,2,3]'));",
        "SELECT * FROM num, TABLE([1,2]);",
        "SELECT num, table_col AS \"SQUARE\" FROM square INNER JOIN TABLE(to_array(6)) ON table_col = num*num ORDER BY num;",
        "SELECT Name, table_col AS \"Title\" FROM empRole JOIN TABLE(JSON_TO_ARRAY(Role));",

        // <see href="https://docs.singlestore.com/db/v8.5/developer-resources/functional-extensions/working-with-vector-data/"/>
        "SELECT id, comment, category, comment_embedding <*> @query_vec AS score FROM comments ORDER BY score DESC LIMIT 2;",
        "SELECT id, comment, category, comment_embedding <*> @query_vec AS score FROM comments WHERE category = \"Food\" ORDER BY score DESC LIMIT 3;",
        "SELECT id, comment, category, comment_embedding <*> @query_vec AS score FROM comments ORDER BY score DESC LIMIT 2;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/vector-functions/vector-indexing/"/>
        // Vector indexing feature is available after v8.5
        // "SELECT k, v, v <*> @query_vec AS score FROM vect ORDER BY score DESC LIMIT 1;",
        // "SELECT k, v, v <*> '[9, 0]' AS score FROM vect ORDER BY score SEARCH_OPTIONS '{\"k\" : 30 }' DESC LIMIT 3;",
        // "SELECT id, v, v <-> @qv AS score FROM ann_test ORDER BY score LIMIT 5;",
        // "SELECT k, v <*> ('[9, 0]' :> vector(2)) AS score FROM vect ORDER BY score USE KEY (v) DESC LIMIT 2;",
        // "SELECT k, v <*> ('[9, 0]' :> vector(2)) AS score FROM vect ORDER BY score USE KEY () DESC LIMIT 2;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/vector-functions/dot-product/"/>
        "SELECT vec, vec <*> @query_vec AS score FROM vectors ORDER BY score DESC;",
        "SELECT vec FROM vectors WHERE vec <*> @query_vec > 0.7;",
        "SELECT vec, vec <*> @query_vec AS score FROM vectors WHERE score > 0.7 ORDER BY score DESC;",
        "SELECT v1.id, v2.id_2, v1.vec <*> v2.vec_2 AS score FROM vectors v1, vectors_2 v2 WHERE v1.vec <*> v2.vec_2 > 0.7 ORDER BY score DESC;",
        "SELECT vec, vec <*> '[0.44, 0.554, 0.34, 0.62]' AS score FROM vectors ORDER BY score DESC;",
        "SELECT id, '[3, 2, 1]' <*> vectors_i16.vec AS score FROM vectors_i16 ORDER BY score DESC;",
        "SELECT id, @query_vec <*> vectors_b.vec AS score FROM vectors_b ORDER BY score DESC;",
        "SELECT @query_vec <*> @query_vec AS DotProduct;",
        "SELECT JSON_ARRAY_UNPACK(vec) FROM vectors_b; ",
        "SELECT DOT_PRODUCT(vec, @query_vec) AS score FROM vectors_b ORDER BY score DESC;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/vector-functions/euclidean-distance/"/>
        "SELECT vec <-> @query_vec AS score FROM vectors ORDER BY score ASC;",
        "SELECT v1.id, v2.id_2, v1.vec <-> v2.vec_2 AS score FROM vectors v1, vectors_2 v2 WHERE v1.vec <-> v2.vec_2 < 0.7 ORDER BY score ASC;",
        "SELECT vec, vec <-> '[0.44, 0.554, 0.34, 0.62]' AS score FROM vectors ORDER BY score ASC;",
        "SELECT id, '[3, 2, 1]' <-> vectors_i16.vec AS score FROM vectors_i16 ORDER BY score ASC;",
        "SELECT id, @query_vec <*> vectors_b.vec AS score FROM vectors_b ORDER BY score DESC;",
        "SELECT EUCLIDEAN_DISTANCE(JSON_ARRAY_PACK('[0.44, 0.554, 0.34, 0.62]'), vec) AS score FROM vectors_b ORDER BY score ASC;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/comparison-operators-and-functions/between-not-between/"/>
        "SELECT * FROM bet_r WHERE t BETWEEN \"blue\" AND \"pink\";",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/comparison-operators-and-functions/null-handling/"/>
        "SELECT * FROM n_s WHERE t IS NULL;",
        "SELECT 'x' IS NULL, NULL IS NULL, NULL IS NOT NULL;",
        "SELECT * FROM n_s AS s1, n_s AS s2 WHERE s1.t <=> s2.t;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/comparison-operators-and-functions/like-not-like/"/>
        "SELECT * FROM like_s WHERE t LIKE 'r_d';",
        "SELECT * FROM like_s WHERE t NOT LIKE 'r%';",
        "SELECT \"foo_\" LIKE \"foo\\_\";",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/comparison-operators-and-functions/in-not-in/"/>
        "SELECT * FROM hrRec WHERE City IN('DC','New York');",
        "SELECT * FROM hrRec WHERE City NOT IN('DC','Brooklyn');",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/comparison-operators-and-functions/strcmp/"/>
        "SELECT r1.t, r2.t, strcmp(r1.t, r2.t) FROM sc_r AS r1, sc_r AS r2;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/comparison-operators-and-functions/where-operators-can-be-used/"/>
        "SELECT t, t LIKE 'r%', t != 'red' FROM s WHERE t = 'red';",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/comparison-operators-and-functions/using-comparison-operators-with-date-and-time-functions/"/>
        "SELECT * FROM Emp WHERE MONTH(DOJ) = 01;",
        "SELECT * FROM Emp WHERE DOJ < CURRENT_DATE;",
        "SELECT * FROM Emp WHERE DOJ BETWEEN DATE('2019-01-01') AND DATE('2020-01-30');",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/database-object-case-sensitivity/"/>
        "SELECT @@table_name_case_sensitivity;",

        // <see href="https://docs.singlestore.com/db/v8.0/developer-resources/functional-extensions/working-with-geospatial-features/"/>
        // Queries below are self-made
        "select \"POLYGON((1 1,2 1,2 2, 1 2, 1 1))\" :> GEOGRAPHY;",
        "select \"POINT(3.5 3.5)\" :> GEOGRAPHYPOINT;"
    };

    public static readonly string[] ValidQuery_Insert =
    {
        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/data-manipulation-language-dml/insert/"/>
        "INSERT INTO t1 (col1, col2) VALUES ((SELECT col1 FROM t2 LIMIT 1), 1);",
        "INSERT INTO mytbl (v) VALUES (\"hello\"), (\"goodbye\");",
        "INSERT IGNORE mytbl2 VALUES(null);",
        "INSERT INTO mytbl (column1, column2, column3) SELECT WITH(force_random_reshuffle=1) * FROM mytbl_new ON DUPLICATE KEY UPDATE column1 = VALUES(column1), column2 = VALUES(column2), column3 = VALUES(column3);",

        // ON DUPLICATE KEY DELETE ~ is available after v8.5
        // "INSERT INTO viewing_stats VALUES(_program_id, _view_count) ON DUPLICATE KEY DELETE WHEN view_count + values(view_count) <= 0",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/data-manipulation-language-dml/replace/"/>
        "REPLACE INTO Emp(ID,Name,City) VALUES(10,\"Bill\",\"San Jose\");",
        "REPLACE INTO Emp SET ID = 10, Name = \"Bill\", City = \"San Jose\";",
        "REPLACE INTO Emp(ID,City) VALUES(10,\"San Jose\");",
        "REPLACE INTO EmpTable(ID,Name,City) SELECT ID,EmpName,CityName FROM EmpCity WHERE ID = 20;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/data-manipulation-language-dml/update/"/>
        "INSERT INTO t1 SELECT t1.* FROM t1 JOIN t2 ON t1.b = t2.b ON DUPLICATE KEY UPDATE t1.b = t1.b + 1",
        "INSERT INTO t1 VALUES (1, 1, 0),(2, 2, 0),(3, 3, 0);",
        "INSERT INTO t2 VALUES (1, 11, 0),(2, 12, 0),(3, 13, 0);",
        "INSERT INTO lmt_exp VALUES(1, 'widget'), (2, 'lgr widget'), (3, 'xl widget');",
    };

    public static readonly string[] ValidQuery_Update =
    {
        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/data-manipulation-language-dml/update/"/>
        "UPDATE t1 JOIN t2 ON t1.b = t2.b SET t1.b = t1.b+1;",
        "UPDATE a SET c1 = 0;",
        "UPDATE a SET c1 = 0;",
        "UPDATE a SET c1 = 0 WHERE c2 = 100;",
        "UPDATE a, b SET a.v = b.v WHERE a.name = b.name;",
        "UPDATE a LEFT JOIN b ON a.name = b.name SET a.v = b.v;",
        "UPDATE looooooooong as a, b SET a.v = b.v WHERE a.name = b.name;",
        "UPDATE a, b, c SET a.v = 0 WHERE a.x = b.x and b.y = c.y;",
        "UPDATE a, b, c SET a.v = c.v WHERE a.x = b.x and b.y = c.y;",
        "UPDATE b, a SET a.v = b.v WHERE a.name = b.name;",
        "UPDATE dataset SET valid = false WHERE v = (SELECT MAX(v) FROM dataset);",
        "UPDATE dataset SET valid = false WHERE name IN (SELECT * FROM invalid_names);",
        "UPDATE dataset SET v = v - (SELECT MIN(v) FROM dataset);",
        "UPDATE records a JOIN (SELECT name, COUNT(*) as count FROM samples GROUP BY name) b SET a.count = a.count + b.count WHERE a.name = b.name;",
        "UPDATE t1 SET b = (SELECT b FROM t2 WHERE t1.a = t2.a), c = (SELECT c FROM t2 WHERE t1.a = t2.a) WHERE t1.a IN (SELECT t2.a FROM t2);",
        "UPDATE stock s INNER JOIN product p ON s.ID = p.ID SET s.P_ID = p.ID;",
        "UPDATE lmt_exp SET item_id=2 LIMIT 1;", // NOTE: LIMIT is used with an UPDATE query to limit the number of rows that will be updated. However, for UPDATE to work, it must run on a single partition; otherwise, it will result in an error.
        "UPDATE lmt_exp SET item_name='med widget' WHERE item_id = 2 LIMIT 1;",
    };

    public static readonly string[] ValidQuery_Delete =
    {
        "DELETE FROM mytbl WHERE seq = 1;",
        "DELETE FROM mytable LIMIT 100000;",
        "DELETE FROM mytbl WHERE id IN (SELECT id FROM myother) LIMIT 10;",
        "DELETE t_rec FROM t_rec JOIN t_invalid WHERE t_rec.id = t_invalid.id;",
        "DELETE t_rec FROM t_rec JOIN (SELECT id FROM t_rec ORDER BY score LIMIT 10) temp WHERE t_rec.id = temp.id;",
        "DELETE b FROM a, b, c WHERE a.name = b.name OR b.name = c.name;",
        "DELETE x FROM looooooooooongName as x, y WHERE x.id = y.id;"
    };

    public static readonly string[] ValidQuery_Set =
    {
        "SET @query_vec = ('[9,0]'):> VECTOR(2) :> BLOB;"
    };

    public static readonly string[] ValidQuery_CreateView =
    {
        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/data-definition-language-ddl/create-view/"/>
        "CREATE VIEW person_view AS SELECT first_name, last_name FROM table_name WHERE user_id = 'real_person';",
        "CREATE VIEW active_items_view AS SELECT name FROM items WHERE status = 'active';",
        "CREATE VIEW discounted_items_view AS SELECT name FROM active_items_view WHERE discount = 1;",
        "CREATE VIEW customer_orders AS SELECT o.id, c.last_name, c.first_name\nFROM orders_db.orders o, customers_db.customers c WHERE o.customer_id = c.id;",
        // Queries below are self-made
        "CREATE DEFINER=mason SCHEMA_BINDING=ON VIEW mason_view AS select * from mason_table;"
    };
    #endregion
}
