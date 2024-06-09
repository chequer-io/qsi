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
        "SELECT k, v, v <*> @query_vec AS score FROM vect ORDER BY score DESC LIMIT 1;",
        "SELECT k, v, v <*> '[9, 0]' AS score FROM vect ORDER BY score SEARCH_OPTIONS '{\"k\" : 30 }' DESC LIMIT 3;",
        "SELECT id, v, v <-> @qv AS score FROM ann_test ORDER BY score LIMIT 5;",
        "SELECT k, v <*> ('[9, 0]' :> vector(2)) AS score FROM vect ORDER BY score USE KEY (v) DESC LIMIT 2;",
        "SELECT k, v <*> ('[9, 0]' :> vector(2)) AS score FROM vect ORDER BY score USE KEY () DESC LIMIT 2;",

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
        "SELECT \"POLYGON((1 1,2 1,2 2, 1 2, 1 1))\" :> GEOGRAPHY;",
        "SELECT \"POINT(3.5 3.5)\" :> GEOGRAPHYPOINT;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/character-encoding/character-set-and-collation-override/"/>
        "SELECT lastname :> text COLLATE utf8_bin FROM grades GROUP BY 1;",
        "SELECT lastname :> text COLLATE utf8_general_ci FROM grades GROUP BY 1;",
        "SELECT * FROM sets WHERE sets.json_field::$x :> text COLLATE utf8_bin = \"string1\" AND sets.json_field::$y :> text COLLATE utf8_general_ci = \"string2\";",
        "SELECT \"My string\" COLLATE utf8mb4_unicode_ci;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/character-encoding/special-cases/"/>
        "SELECT character_length(\"敥\"), character_length(\"\ud83d\ude00\");\n",
        "SELECT \"My string literal\" COLLATE utf8mb4_unicode_ci;",
        "SELECT character_length(\"\ud83d\ude00\" :> CHAR(20) COLLATE utf8mb4_unicode_ci) as result;",
        "SELECT a, character_length(a) AS len FROM t;",

        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/user-defined-variables/select-into-user-defined-variable/"/>
        "SELECT 3.14 INTO @pi;",
        "SELECT Radius, Radius*POW(@pi,2) AS \"Area\" FROM circle;",
        "SELECT number_students FROM udv_courses WHERE course_code = 'CS-301' and section_number = 1 INTO @ns;",

        // <see href="https://docs.singlestore.com/db/v8.5/query-data/basic-query-examples/"/>
        "SELECT COUNT(*) from employees;",
        "SELECT id, name FROM employees ORDER BY id;",
        "SELECT id, name FROM employees WHERE state = 'TX' ORDER BY id;",
        "SELECT id, name, hireDate FROM employees WHERE hireDate < '2002-01-01' ORDER BY id;",
        "SELECT state, COUNT(*) from employees group by state ORDER BY state;",
        "SELECT e.name, s.salary FROM employees e, salaries s WHERE e.id = s.employeeId and s.salary = (SELECT MAX(salary) FROM salaries);",
        "SELECT e.state, AVG(salary) FROM employees e JOIN salaries s on e.id = s.employeeId GROUP BY e.state ORDER BY e.state;",
        "SELECT name FROM employees WHERE id IN (SELECT managerId FROM employees) ORDER BY name;",
        "SELECT m.name, COUNT(*) count FROM employees m JOIN employees e ON m.id = e.managerId GROUP BY m.id ORDER BY count DESC;",
        "SELECT m.name, COUNT(e.id) count FROM employees m LEFT JOIN employees e ON m.id = e.managerId GROUP BY m.id ORDER BY count DESC;",
        "SELECT e.name employee_name, m.name manager_name FROM employees e LEFT JOIN employees m ON e.managerId = m.id ORDER BY manager_name;",
        "SELECT m.name, SUM(salary) FROM employees m JOIN employees e ON m.id = e.managerId JOIN salaries s ON s.employeeId = e.id GROUP BY m.id ORDER BY SUM(salary) DESC;",
        "SELECT e.name employee_name, se.salary employee_salary, m.name manager_name, sm.salary manager_salary FROM employees e JOIN salaries se ON e.id = se.employeeId JOIN employees m ON m.id = e.managerId JOIN salaries sm ON sm.employeeId = m.id JOIN departments d ON d.id = e.deptId WHERE d.name = 'Finance' AND sm.salary < se.salary ORDER BY employee_salary, manager_salary;",

        // <see href="https://docs.singlestore.com/db/v8.5/query-data/advanced-query-topics/with-common-table-expressions/"/>
        "WITH myCTE AS (SELECT col FROM myTable) SELECT col FROM myCTE;",
        "WITH myCTE (colAlias) AS (SELECT col FROM myTable) SELECT colAlias FROM myCTE;",
        "WITH orderCTE AS (select o_orderkey from orders), lineitemCTE AS (select l_orderkey from lineitem) select count(*) from orderCTE join lineitemCTE on o_orderkey = l_orderkey;",
        "WITH foo AS (WITH bar AS (SELECT * FROM t) SELECT * FROM bar) SELECT * FROM foo;",
        "WITH EmpSal(averageSal) AS (SELECT AVG(Salary) FROM Employee) SELECT EmpID, Name, Salary FROM Employee, EmpSal WHERE Employee.Salary > EmpSal.averageSal;",
        "WITH ObjectCTE (Name, Id, Date) AS (SELECT objname, objectid, invoicedate FROM Inventory) INSERT INTO Itemlist(Name, objectid, createdDate) SELECT Name, Id, Date FROM ObjectCTE;",
        "WITH RECURSIVE org_chart (emp_id, mgr_id, name, level) AS (SELECT cte_emp.id, cte_emp.mgr_id, cte_emp.name, 0 AS level FROM cte_emp WHERE cte_emp.mgr_id IS NULL UNION ALL SELECT cte_emp.id, cte_emp.mgr_id, cte_emp.name, level + 1 FROM cte_emp INNER JOIN org_chart ON cte_emp.mgr_id = org_chart.emp_id) SELECT * FROM org_chart;",
        "WITH RECURSIVE depths AS (SELECT 0 depth, * FROM cte_g WHERE id = 0 UNION ALL SELECT depth + 1, cte_g.id, cte_g.pr FROM cte_g, depths WHERE cte_g = depths.id) SELECT depth, id FROM depths;",
        "WITH RECURSIVE depths AS (SELECT 0 depth, id, pr FROM cte_g WHERE pr IS NULL UNION ALL SELECT CAST((depth + 1) AS DECIMAL(10, 2)), cte_g.id, cte_g.pr FROM cte_g, depths WHERE cte_g.pr = depths.id) SELECT depth, id FROM depths;",
        "WITH RECURSIVE routes (id, path) AS (SELECT id, CAST(1 AS CHAR(30)) FROM cte_g WHERE pr IS NULL UNION ALL SELECT t.id, CONCAT(routes.path, '-->', t.id) FROM cte_g JOIN routes ON routes.id = cte_g.pr) SELECT * FROM routes;",
        "SELECT * FROM foo OPTION(materialize_ctes=\"OFF\");",
        "WITH foo AS (SELECT WITH (materialize = off) * FROM titanic), bar AS (SELECT * FROM titanic) SELECT * FROM foo, bar;",
        "WITH foo AS (SELECT * FROM t) SELECT * FROM foo, foo AS bar WHERE foo.a = 1 AND bar.b = 2;",
        "WITH foo AS (SELECT * FROM t WHERE t.a = 1 OR t.b = 2) SELECT * FROM foo, foo AS bar WHERE foo.a = 1 AND bar.b = 2;",

        // <see href="https://docs.singlestore.com/db/v8.5/query-data/advanced-query-topics/read-query-results-in-parallel/"/>
        "SELECT * FROM :: t1_result_table WHERE partition_id() = 1 and partition_row_id() < 4;",

        // <see href="https://docs.singlestore.com/db/v8.5/manage-data/local-and-unlimited-database-storage-concepts/"/>
        "SELECT SUM(mv_columnstore_files.size) / (1024 * 1024 * 1024) AS dataSizeGB FROM mv_columnstore_files WHERE mv_columnstore_files.database_name = 'database_name';",
        "SELECT COUNT(*) AS numSegments, SUM(mv_cached_blobs.size) / (1024 * 1024 * 1024) AS blobdataSizeGB FROM mv_cached_blobs WHERE mv_cached_blobs.database_name = 's2_dataset_tpch' AND type = 'primary';",

        // <see href="https://docs.singlestore.com/db/v8.5/manage-data/moving-data/moving-data-between-databases/"/>
        "SELECT * FROM table_name_1 INTO OUTFILE '/home/username/file_name.csv' FIELDS TERMINATED BY ',' LINES TERMINATED BY '\\n';",
        "SELECT * FROM table_name_1 INTO S3 'bucket/target' CONFIG 'configuration_json' CREDENTIALS 'credentials_json';",

        // <see href="https://docs.singlestore.com/db/v8.5/create-a-database/using-json/"/>
        "SELECT * FROM assets WHERE properties::$license_plate = \"VGB116\";",
        "SELECT * FROM assets ORDER BY properties::%weight;",
        "SELECT * FROM json_empty_values_table ORDER BY a;",
        "SELECT data::first, data::$first FROM TestJSON;",
        "SELECT CONCAT(data::$first, ' ', data::$second) FROM TestJSON;",
        "SELECT json, json::a::`2` FROM (SELECT '{\"a\":[1,2,3,4]}' AS json) sub;",
        "SELECT json, json_extract_json(json, 'a', 1+1) FROM (SELECT '{\"a\":[1,2,3,4]}' AS json) sub;",
        "WITH t AS(SELECT id, jsondata::city city , table_col AS sports_clubs FROM json_tab JOIN TABLE(JSON_TO_ARRAY(jsondata::sports_teams))), t1 AS(SELECT t.id, t.city, t.sports_clubs::sport_name sport, table_col AS clubs FROM t JOIN TABLE(JSON_TO_ARRAY(t.sports_clubs::teams))) SELECT t1.id, t1.city,t1.sport,t1.clubs::club_name club_name FROM t1;",
        "WITH t AS (SELECT id, jsondata::city city , table_col AS sports_clubs FROM json_tab JOIN TABLE(JSON_TO_ARRAY(jsondata::sports_teams))), t1 AS (SELECT t.id, t.city, t.sports_clubs::sport_name sport, table_col AS clubs FROM t JOIN TABLE(JSON_TO_ARRAY(t.sports_clubs::teams))) SELECT t1.id, t1.city,t1.sport,t1.clubs::club_name club_name FROM t1 WHERE t1.clubs::club_name = 'Yankees';",
        "SELECT ticker_symbol FROM stocks WHERE statistics::%`P/E` > 1.5;",
        "SELECT ticker_symbol FROM stocks WHERE JSON_EXTRACT_DOUBLE(statistics, 'P/E') > 1.5;",
        "SELECT t.table_col::$l_shipmode, sum(t.table_col::%l_quantity) as quantity FROM orders JOIN TABLE(JSON_TO_ARRAY(lineitems_json)) t GROUP BY t.table_col::$l_shipmode;",
        "SELECT o_orderpriority as priority, sum(t.table_col::$l_quantity) as quantity FROM orders JOIN TABLE(JSON_TO_ARRAY(lineitems_json)) t GROUP BY o_orderpriority",
        "SELECT t.table_col::$l_returnflag as r, t.table_col::$l_linestatus as s, sum(t.table_col::%l_quantity) as sum_qty, sum(t.table_col::%l_extendedprice) as sum_base_price, sum(t.table_col::%l_extendedprice * (1 - t.table_col::%l_discount)) as sum_disc_price, avg(t.table_col::%l_quantity) as avg_qty FROM orders JOIN TABLE(JSON_TO_ARRAY(lineitems_json)) t GROUP by r, s",
        "SELECT '{\"a\":\"\\\\u00F9\"}' :> JSON;",
        "SELECT * FROM sets WHERE sets.json_field::$x :> text collate utf8_bin = 'string1' AND sets.json_field::$y :> text collate utf8_general_ci = 'string2'",

        // <see href="https://docs.singlestore.com/db/v8.5/create-a-database/columnstore/encoded-data-in-columnstores/"/>
        "SELECT table_name, COUNT(*) FROM r WHERE table_name LIKE '%COLUMN%' GROUP BY table_name;",
        "SELECT d.category, COUNT(*) FROM r, d WHERE r.n = d.n AND d.category LIKE 'cat1%' GROUP BY d.category;",
        "SELECT SUM(a) FROM t WITH (disable_ordered_scan=true) GROUP BY b;",
        "SELECT SUM(t2.c) FROM (SELECT a, COUNT(*) AS c FROM t GROUP BY a) AS t2;",

        // <see href="https://docs.singlestore.com/db/v8.5/create-a-database/understanding-shard-key-selection/"/>
        "SELECT DATABASE_NAME, TABLE_NAME, ORDINAL AS PARTITION_ID, ROWS, MEMORY_USE FROM INFORMATION_SCHEMA.TABLE_STATISTICS WHERE TABLE_NAME = 'people_1';"
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

        // <see href="https://docs.singlestore.com/db/v8.0/reference/sql-reference/data-types/geospatial-types/"/>
        "INSERT INTO departments (id, name) VALUES (1, 'Marketing'), (2, 'Finance'), (3, 'Sales'), (4, 'Customer Service');",

        // <see href="https://docs.singlestore.com/db/v8.5/query-data/advanced-query-topics/performing-upserts/"/>
        "INSERT INTO cust (name, id, orders) VALUES (\"Chris\",7214,2), (\"Adam\",3412,5), (\"Elen\",8301,4);",
        "INSERT INTO cust (ID, ORDERS) VALUES (7214, 3) ON DUPLICATE KEY UPDATE ORDERS=3;",
        "INSERT INTO cust (ID, ORDERS) VALUES (7214, 4) ON DUPLICATE KEY UPDATE ORDERS = VALUES(ORDERS) + ORDERS;",
        "INSERT INTO cust (NAME, ID, ORDERS) SELECT * FROM cust_new ON DUPLICATE KEY UPDATE NAME = VALUES(NAME), ORDERS =  VALUES (ORDERS);",
        "INSERT IGNORE INTO cust SELECT * FROM cust_new;",
        "INSERT INTO product(name,id_1,id_2,quantity) VALUES ('red pen',2792,5,325) ON DUPLICATE KEY UPDATE quantity = 325;",
        "INSERT INTO product(name,id_1,id_2,quantity) VALUES ('yellow paper',4624,7,125) ON DUPLICATE KEY UPDATE quantity = 125;",

        // <see href="https://docs.singlestore.com/db/v8.5/query-data/advanced-query-topics/read-query-results-in-parallel/"/>
        "INSERT INTO t1 (colint, colchar, colst) SELECT * FROM :: t1_result_table;",

        // <see href="https://docs.singlestore.com/db/v8.5/query-data/advanced-query-topics/with-common-table-expressions/"/>
        // Queries below are self-made
        "INSERT INTO actor VALUES (default, 'Mason', 'Oh', now()) OPTION(materialize_ctes=\"OFF\")",
        "INSERT INTO actor VALUES (9, 'mason', 'oh', now()) ON DUPLICATE KEY UPDATE actor_id = actor_id + 1 OPTION(materialize_ctes='off');",

        // <see href="https://docs.singlestore.com/db/v8.5/create-a-database/using-json/"/>
        "INSERT INTO test_table(col_a,col_b) VALUES ('hello','{\"x\":\"goodbye\",\"y\":\"goodnight\"}');",
        "INSERT INTO json_empty_values_table VALUES (1, '{\"v\":null}');",
        "INSERT INTO json_empty_values_table VALUES (2, '{\"w\":[]}');",
        "INSERT INTO json_empty_values_table VALUES (3, '{\"x\":\"foo\",\"y\":null,\"z\":[]}');",
        "INSERT INTO json_empty_values_table VALUES (4, 'null');",
        "INSERT INTO json_empty_values_table VALUES (5, '[]');",
        "INSERT INTO TestJSON VALUES ('{\"first\":\"hello\"}');",
        "INSERT INTO TestJSON VALUES ('{\"first\":\"hello\", \"second\":\"world\"}');",
        "INSERT INTO json_tab VALUES ( 8765 ,' {\"city\":\"SFO\",\"sports_teams\":[{\"sport_name\":\"football\",\"teams\":  [{\"club_name\":\"Raiders\"},{\"club_name\":\"49ers\"}]},{\"sport_name\":\"baseball\",\"teams\" : [{\"club_name\":\"As\"},{\"club_name\":\"SF Giants\"}]}]}') ;",
        "INSERT INTO json_tab VALUES ( 9876,'{\"city\":\"NY\",\"sports_teams\" : [{ \"sport_name\":\"football\",\"teams\" : [{ \"club_name\":\"Jets\"},{\"club_name\":\"Giants\"}]},{\"sport_name\":\"baseball\",\"teams\" : [ {\"club_name\":\"Mets\"},{\"club_name\":\"Yankees\"}]},{\"sport_name\":\"basketball\",\"teams\" : [{\"club_name\":\"Nets\"},{\"club_name\":\"Knicks\"}]}]}');",
        "INSERT INTO test_json VALUES ('{\"addParams\": \"{\\\"Emp_Id\\\":\\\"1487\\\", \\\"Emp_LastName\\\":\\\"Stephens\\\",\\\"Emp_FirstName\\\":\\\"Mark\\\",\\\"Dept\\\":\\\"Support\\\"}\"}');",
        "INSERT INTO orders2 SELECT * FROM orders;",
        "INSERT INTO new_table SELECT l_orderkey, JSON_AGG( JSON_BUILD_OBJECT( 'l_partkey', l_partkey, 'l_suppkey', l_suppkey, 'l_linenumber', l_linenumber, 'l_quantity', l_quantity, 'l_extendedprice', l_extendedprice, 'l_discount', l_discount, 'l_tax', l_tax, 'l_returnflag', l_returnflag, 'l_linestatus', l_linestatus, 'l_shipdate', l_shipdate, 'l_commitdate', l_commitdate, 'l_receiptdate', l_receiptdate, 'l_shipinstruct', l_shipinstruct, 'l_shipmode', l_shipmode, 'l_comment', l_comment ) ) as lineitems FROM lineitem GROUP BY l_orderkey;",

        // <see href="https://docs.singlestore.com/db/v8.5/create-a-database/understanding-shard-key-selection/"/>
        "INSERT INTO people_1 (id, user, first, last) SELECT WITH(force_random_reshuffle=1) * FROM people;",

        // <see href="https://docs.singlestore.com/db/v8.5/create-a-database/understanding-how-datatype-can-affect-performance/"/>
        "INSERT INTO tmp_dates (date_str) SELECT * FROM dates WHERE date_int <> CONVERT(CONVERT(date_str, DATE) + 1, SIGNED INT);"
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

        // <see href="https://docs.singlestore.com/db/v8.5/query-data/advanced-query-topics/performing-upserts/"/>
        "UPDATE cust JOIN cust_new ON (cust_new.ID = cust.ID) SET cust.NAME = cust_new.NAME, cust.ORDERS = cust_new.ORDERS;",

        // <see href="https://docs.singlestore.com/db/v8.5/query-data/advanced-query-topics/row-locking/"/>
        "UPDATE stock JOIN product ON stock.qty = 10 AND stock.id = product.id SET stock.qty = stock.qty + 1",

        // <see href="https://docs.singlestore.com/db/v8.5/query-data/advanced-query-topics/with-common-table-expressions/"/>
        // Queries below are self-made
        "UPDATE actor SET first_name = 'Mason' WHERE actor_id < 30 OPTION(materialize_ctes=\"AUTO\")",

        // <see href="https://docs.singlestore.com/db/v8.5/create-a-database/using-json/"/>
        "UPDATE users SET userdata::name::first = 'Alex';",
        "UPDATE users SET userdata = JSON_SET_STRING(userdata, 'name', 'first', 'Alex');",
        "UPDATE orders o JOIN new_table t ON o.o_orderkey = t.l_orderkey SET o.lineitems_json = t.lineitems;",

        // <see href="https://docs.singlestore.com/db/v8.5/create-a-database/columnstore/locking-in-columnstores/"/>
        "UPDATE app_errors SET error_code = 'ERR-2000' WHERE app_name = 'App1' OPTION (columnstore_table_lock_threshold = 4000);"
    };

    public static readonly string[] ValidQuery_Delete =
    {
        // <see href="https://docs.singlestore.com/db/v8.0/reference/sql-reference/data-manipulation-language-dml/delete/"/>
        "DELETE FROM mytbl WHERE seq = 1;",
        "DELETE FROM mytable LIMIT 100000;",
        "DELETE FROM mytbl WHERE id IN (SELECT id FROM myother) LIMIT 10;",
        "DELETE t_rec FROM t_rec JOIN t_invalid WHERE t_rec.id = t_invalid.id;",
        "DELETE t_rec FROM t_rec JOIN (SELECT id FROM t_rec ORDER BY score LIMIT 10) temp WHERE t_rec.id = temp.id;",
        "DELETE b FROM a, b, c WHERE a.name = b.name OR b.name = c.name;",
        "DELETE x FROM looooooooooongName as x, y WHERE x.id = y.id;",

        // <see href="https://docs.singlestore.com/db/v8.5/query-data/advanced-query-topics/with-common-table-expressions/"/>
        // Queries below are self-made
        "DELETE FROM actor WHERE actor_id < 3 OPTION(materialize_ctes=\"ON\")",

        // <see href="https://docs.singlestore.com/db/v8.5/create-a-database/understanding-how-datatype-can-affect-performance/"/>
        "DELETE FROM dates WHERE date_int <> CONVERT(CONVERT(date_str, DATE) + 1, SIGNED INT);"
    };

    public static readonly string[] ValidQuery_Set =
    {
        "SET @query_vec = ('[9,0]'):> VECTOR(2) :> BLOB;",
        // "SET CLUSTER character_set_server = 'utf8';",
        "SET GLOBAL use_seekable_json = OFF;",
        "SET GLOBAL use_seekable_json = ON"
    };

    public static readonly string[] ValidQuery_CreateView =
    {
        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/data-definition-language-ddl/create-view/"/>
        "CREATE VIEW person_view AS SELECT first_name, last_name FROM table_name WHERE user_id = 'real_person';",
        "CREATE VIEW active_items_view AS SELECT name FROM items WHERE status = 'active';",
        "CREATE VIEW discounted_items_view AS SELECT name FROM active_items_view WHERE discount = 1;",
        "CREATE VIEW customer_orders AS SELECT o.id, c.last_name, c.first_name FROM orders_db.orders o, customers_db.customers c WHERE o.customer_id = c.id;",
        // Queries below are self-made
        "CREATE DEFINER=mason SCHEMA_BINDING=ON VIEW mason_view AS select * from mason_table;"
    };
    #endregion

    public static readonly string[] ValidQuery_LoadData =
    {
        // <see href="https://docs.singlestore.com/db/v8.5/reference/sql-reference/user-defined-variables/select-into-user-defined-variable/"/>
        "LOAD DATA INFILE '/tmp/data.csv' INTO TABLE allviews FIELDS TERMINATED BY ',' (State,Product,@Views) SET Views = @Views + 10;",

        // <see href="https://docs.singlestore.com/db/v8.5/manage-data/moving-data/moving-data-between-databases/"/>
        "LOAD DATA INFILE '/home/username/file_name.csv' INTO TABLE table_name_2;",

        // <see href="https://docs.singlestore.com/db/v8.5/load-data/load-data-from-files/load-data-from-local-files/"/>
        "LOAD DATA INFILE '/tmp/emp_data.csv' INTO TABLE employees FIELDS TERMINATED BY ',' ENCLOSED BY '\"';",

        // <see href="https://docs.singlestore.com/db/v8.5/load-data/load-data-from-files/load-data-from-avro-files/"/>
        "LOAD DATA FS \"/path/to/files/data.avro\" INTO TABLE t FORMAT AVRO SCHEMA REGISTRY \"your_schema_registry_host_name_or_ip:your_schema_registry_port\" (id <- %::id, color <- %::color, input_record <- %);",
        "LOAD DATA FS \"/path/to/files/data.avro\" INTO TABLE t FORMAT AVRO SCHEMA REGISTRY \"your_schema_registry_host_name_or_ip:your_schema_registry_port\" (id <- %::id, color <- %::color, price <- %::price DEFAULT NULL, input_record <- %);",
        "LOAD DATA FS \"/path/to/files/data.avro\" INTO TABLE t FORMAT AVRO SCHEMA REGISTRY \"your_schema_registry_host_name_or_ip:your_schema_registry_port\" (id <- %::id, color <- %::color, price <- %::price, input_record <- %);",
        "LOAD DATA FS \"/path/to/files/data.avro\" INTO TABLE t FORMAT AVRO SCHEMA REGISTRY \"\" (id <- %::id, color <- %::color, input_record <- %);",
        "LOAD DATA FS \"/path/to/files/data.avro\" INTO TABLE t FORMAT AVRO SCHEMA REGISTRY \"\" (id <- %::id, color <- %::color, input_record <- %) CONFIG '{\"schema.registry.ssl.certificate.location\": \"/var/private/ssl/client_memsql_client.pem\", \"schema.registry.ssl.key.location\": \"/var/private/ssl/client_memsql_client.key\", \"schema.registry.ssl.ca.location\": \"/var/private/ssl/ca-cert.pem\"}' CREDENTIALS '{\"schema.registry.ssl.key.password\": \"abcdefgh\"}';",

        // <see href="https://docs.singlestore.com/db/v8.5/load-data/load-data-from-files/load-data-in-csv-format-from-amazon-s-3-using-a-pipeline/"/>
        "LOAD DATA S3 's3://test-bucket/nautical_books.csv' CONFIG '{\"region\":\"us-west-2\"}' CREDENTIALS '{\"aws_access_key_id\": \"XXXXXXXXXXXXXXX\", \"aws_secret_access_key\": \"XXXXXXXXXXXXXXX\"}' INTO TABLE nautical_books;",
        "LOAD DATA S3 's3://test-bucket/nautical_books.csv' CONFIG '{\"region\":\"us-west-2\"}' CREDENTIALS '{\"aws_access_key_id\": \"XXXXXXXXXX\", \"aws_secret_access_key\": \"XXXXXXXXXX\"}' INTO TABLE nautical_books FIELDS TERMINATED BY ',' ENCLOSED BY '\"' ESCAPED BY '\\\\' LINES TERMINATED BY '\\r\\n' STARTING BY '';",
        "LOAD DATA S3 's3://test-bucket/nautical_books.csv' CONFIG '{\"region\":\"us-west-2\"}' CREDENTIALS '{\"aws_access_key_id\": \"XXXXXXXXXX\", \"aws_secret_access_key\": \"XXXXXXXXXX\"}' INTO TABLE nautical_books FIELDS TERMINATED BY ',' ENCLOSED BY '\"' ESCAPED BY '\\\\' LINES TERMINATED BY '\\r\\n' STARTING BY '' IGNORE 1 LINES;",

        // <see href="https://docs.singlestore.com/db/v8.5/load-data/load-data-from-files/load-data-in-json-format-from-amazon-s-3-using-a-wildcard/"/>
        "LOAD DATA S3 '<bucket_name>/<folder_name>/*.json' CONFIG '{\"region\":\"us-west-2\"}' CREDENTIALS '{\"aws_access_key_id\": \"<xxxxxxxxxxxxxxx>\", \"aws_secret_access_key\": \"<xxxxxxxxxxxxxxx>\"}' BATCH_INTERVAL 2500 MAX_PARTITIONS_PER_BATCH 1 DISABLE OUT_OF_ORDER OPTIMIZATION DISABLE OFFSETS METADATA GC SKIP DUPLICATE KEY ERRORS INTO TABLE employees FORMAT JSON (`lastname` <- `lastname` default '', `firstname` <- `firstname` default '', `age` <- `age` default -1, `DOB` <- `DOB` default -1, `partner` <- `partner` default '', `hasChildren` <- `hasChildren` default '', `children` <- `children` default '');",

        // <see href="https://docs.singlestore.com/db/v8.5/load-data/load-data-from-files/load-data-from-parquet-files/"/>
        "LOAD DATA INFILE '/tmp/assets.parquet' INTO TABLE assets (Product_ID <- Product_ID, Category <- Category, Model <- Model, Price <- Price, Employee_ID <- Employee_ID) FORMAT PARQUET;",
        "LOAD DATA S3 '<bucket name>' CONFIG '{\"region\" : \"<region_name>\"}' CREDENTIALS '{\"aws_access_key_id\" : \"<key_id> \", \"aws_secret_access_key\": \"<access_key>\"}' INTO TABLE <table_name> (`<col_a>` <- %, `<col_b>` <- % DEFAULT NULL) FORMAT PARQUET;",
        "LOAD DATA S3 's3://<path to file>/employee_data.parquet' CONFIG '{\"region\":\"us-west-2\"}' CREDENTIALS '{\"aws_access_key_id\": \"XXXXXXXXXX\", \"aws_secret_access_key\": \"XXXXXXXXXX\"}' INTO TABLE employees (`ID` <- ID, `Last_Name` <- Last_Name, `First_Name` <- First_Name, `Job_Title` <- Job_Title, `Department` <- Department, `City` <- City, `State` <- State, `Email` <- Email) FORMAT PARQUET;",
    };
}
