
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/chequer-io/qsi/main/LICENSE)
[![Nuget](https://img.shields.io/nuget/dt/Qsi?label=Nuget.org%20Downloads&style=flat-square&color=blue)](https://www.nuget.org/packages/Qsi)
[![QSI Unit Tests](https://github.com/chequer-io/qsi/actions/workflows/qsi-unit-tests.yml/badge.svg)](https://github.com/chequer-io/qsi/actions/workflows/qsi-unit-tests.yml)

![Logo](https://github.com/chequer-io/qsi/blob/main/Resources/logo-256.png?raw=true)


The QSI is the pure C# Query Structure Interface.

## Languages

|Language|Parser|Repos|
|--|--|--|
|MySql|MySQL Workbench source code(Antlr4)|[mysql-fworkbench](https://github.com/mysql/mysql-workbench)|
|SingleStore|based on Qsi.MySql||
|PostgreSql|PostgreSQL server source code(yacc)|[libpg_query](https://github.com/lfittl/libpg_query)|
|Redshift|based on Qsi.PostgreSql||
|Oracle|Antlr4||
|SqlServer|Microsoft.SqlServer.TransactSql.ScriptDom, Microsoft.SqlServer.Management.SqlParser|[MSDN](https://docs.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.transactsql.scriptdom?view=sql-dacfx-150), [NuGet (ScriptDom)](https://www.nuget.org/packages/Microsoft.SqlServer.TransactSql.ScriptDom),[NuGet (Management)](https://www.nuget.org/packages/Microsoft.SqlServer.Management.SqlParser/)|
|Cassandra|Antlr||
|Athena|Antlr||
|SAP Hana|Antlr||
|Impala|Antlr||
|Trino|Trino source code(Antlr4)|[trino](https://github.com/trinodb/trino/)|
|PrimarSql|PrimarSql|[PrimarSql](https://github.com/chequer-io/PrimarSql)|

<!-- |PhoenixSql|Phoenix server source code(Antlr3)|[PhoenixSql](https://github.com/chequer-io/PhoenixSql)| -->

## Structure Compiler
It compiles the result structure and relation 

based on semantic tree transformed by parser's  for each language.

## Status

<!-- 
&#x2714; ✔
&#x274C; ❌
&#x26A0; ⚠
-->

<table>
    <tr>
        <th>Features</th>
        <th>
            <a href="https://www.mysql.com/">
                <img
                    src="https://github.com/user-attachments/assets/efc5b372-03e5-4e48-995c-10588e8645b5" 
                    alt="MySql"
                    title="MySql"
                    style="width:20px;height:auto;" />
            </a>
        </th>
        <th>
            <a href="https://www.singlestore.com/">
                <img
                    src="https://github.com/user-attachments/assets/32bc33c1-a3c6-4d45-8143-d39e95e4ccd8" 
                    alt="SingleStore"
                    title="SingleStore"
                    style="width:20px;height:auto;" />
            </a>
        </th>
        <th>
            <a href="https://www.postgresql.org/">
                <img
                    src="https://github.com/user-attachments/assets/b691ab32-4f53-418a-8639-e960265d34ae"
                    alt="PostgreSql"
                    title="PostgreSql"
                    style="width:20px;height:auto;" />
            </a>
        </th>
        <th>
            <a href="https://aws.amazon.com/redshift/">
                <img
                    src="https://github.com/user-attachments/assets/b35a983f-2cfc-4892-93f9-7108d24ff14d" 
                    alt="Amazon Redshift"
                    title="Amazon Redshift"
                    style="width:20px;height:auto;" />
            </a>
        </th>
        <th>
            <a href="https://www.oracle.com/database/">
                <img
                    src="https://github.com/user-attachments/assets/c93aaae3-91d3-4463-a993-f191f1ad130a" 
                    alt="Oracle"
                    title="Oracle"
                    style="width:20px;height:auto;" />
            </a>
        </th>
        <th>
            <a href="https://www.microsoft.com/en-us/sql-server">
                <img
                    src="https://github.com/user-attachments/assets/2212f7a0-8f53-4198-b5e0-d9bbe6451a7a" 
                    alt="SQLServer"
                    title="SQLServer"
                    style="width:20px;height:auto;" />
            </a>
        </th>
        <th>
            <a href="https://cassandra.apache.org/_/index.html">
                <img
                    src="https://github.com/user-attachments/assets/23516570-37c3-48ff-9247-0d4eaa581968" 
                    alt="Cassandra(Cql)"
                    title="Cassandra(Cql)"
                    style="width:20px;height:auto;" />
            </a>
        </th>
        <th>
            <a href="https://aws.amazon.com/athena/">
                <img
                    src="https://github.com/user-attachments/assets/abcc7f7a-fd3c-4116-ba88-31bcbaf47485" 
                    alt="Amazon Athena"
                    title="Amazon Athena"
                    style="width:20px;height:auto;" />
            </a>
        </th>
        <th>
            <a href="https://www.sap.com/products/technology-platform/hana/what-is-sap-hana.html">
                <img
                    src="https://github.com/user-attachments/assets/2f436dda-90cb-43c9-b324-48b039d6b614" 
                    alt="SAP Hana"
                    title="SAP Hana"
                    style="width:20px;height:auto;" />
            </a>
        </th>
<!--         <th>
            <a href="https://phoenix.apache.org/">
                <img
                    src="https://github.com/user-attachments/assets/c0a7fc23-d692-4048-b620-317e2eafc1d6" 
                    alt="Apache Phoenix"
                    title="Apache Phoenix"
                    style="width:20px;height:auto;" />
            </a>
        </th> -->
        <th>
            <a href="https://impala.apache.org/">
                <img
                    src="https://github.com/user-attachments/assets/7b566a03-e7eb-4902-aecc-c9499925a9c5" 
                    alt="Apache Impala"
                    title="Apache Impala"
                    style="width:20px;height:auto;" />
            </a>
        </th>
        <th>
            <a href="https://trino.io/">
                <img
                    src="https://github.com/user-attachments/assets/bc11487e-ad05-4c44-ae20-4194dac12243" 
                    alt="Trino"
                    title="Trino"
                    style="width:20px;height:auto;" />
            </a>
        </th>
        <th>
            <a href="https://github.com/chequer-io/PrimarSql">
                <img
                    src="https://raw.githubusercontent.com/chequer-io/primarsql/main/Logo.png" 
                    alt="Trino"
                    title="Trino"
                    style="width:20px;height:auto;" />
            </a>
        </th>
    </tr>
    <tr>
        <td>No table</td>
        <td>&check;</td> <!-- MySql -->
        <td>&check;</td> <!-- SingleStore -->
        <td>&check;</td> <!-- PostgreSql -->
        <td>&check;</td> <!-- Amazon Redshift -->
        <td>&#10005;</td> <!-- Oracle -->
        <td>&check;</td> <!-- SqlServer -->
        <td>&#10005;</td> <!-- Cassandra -->
        <td>&check;</td> <!-- Amazon Athena -->
        <td>&#10005;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&check;</td> <!-- Apache Impala -->
        <td>&check;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Table access</td>
        <td>&check;</td> <!-- MySql -->
        <td>&check;</td> <!-- SingleStore -->
        <td>&check;</td> <!-- PostgreSql -->
        <td>&check;</td> <!-- Amazon Redshift -->
        <td>&check;</td> <!-- Oracle -->
        <td>&check;</td> <!-- SqlServer -->
        <td>&check;</td> <!-- Cassandra -->
        <td>&check;</td> <!-- Amazon Athena -->
        <td>&check;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&check;</td> <!-- Apache Impala -->
        <td>&check;</td> <!-- Trino -->
        <td>&check;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Derived table</td>
        <td>&check;</td> <!-- MySql -->
        <td>&check;</td> <!-- SingleStore -->
        <td>&check;</td> <!-- PostgreSql -->
        <td>&check;</td> <!-- Amazon Redshift -->
        <td>&check;</td> <!-- Oracle -->
        <td>&check;</td> <!-- SqlServer -->
        <td>&#10005;</td> <!-- Cassandra -->
        <td>&check;</td> <!-- Amazon Athena -->
        <td>&check;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&check;</td> <!-- Apache Impala -->
        <td>&check;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Derived table (Non-Alias)</td>
        <td>&check;</td> <!-- MySql -->
        <td>&check;</td> <!-- SingleStore -->
        <td>&#10005;</td> <!-- PostgreSql -->
        <td>&#10005;</td> <!-- Amazon Redshift -->
        <td>&check;</td> <!-- Oracle -->
        <td>&#10005;</td> <!-- SqlServer -->
        <td>&#10005;</td> <!-- Cassandra -->
        <td>&check;</td> <!-- Amazon Athena -->
        <td>&check;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&#10005;</td> <!-- Apache Impala -->
        <td>&check;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Specify columns to table alias</td>
        <td>&#10005;</td> <!-- MySql -->
        <td>&#10005;</td> <!-- SingleStore -->
        <td>&check;</td> <!-- PostgreSql -->
        <td>&check;</td> <!-- Amazon Redshift -->
        <td>&#10005;</td> <!-- Oracle -->
        <td>&#10005;</td> <!-- SqlServer -->
        <td>&#10005;</td> <!-- Cassandra -->
        <td>&check;</td> <!-- Amazon Athena -->
        <td>&#10005;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&#10005;</td> <!-- Apache Impala -->
        <td>&check;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Inline derived table</td>
        <td>&#10005;</td> <!-- MySql -->
        <td>&#10005;</td> <!-- SingleStore -->
        <td>&check;</td> <!-- PostgreSql -->
        <td>&check;</td> <!-- Amazon Redshift -->
        <td>&#10005;</td> <!-- Oracle -->
        <td>&check;</td> <!-- SqlServer -->
        <td>&#10005;</td> <!-- Cassandra -->
        <td>&check;</td> <!-- Amazon Athena -->
        <td>&#10005;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&#10005;</td> <!-- Apache Impala -->
        <td>&check;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Table function</td>
        <td>&#9888;</td> <!-- MySql -->
        <td>&#9888;</td> <!-- SingleStore -->
        <td>&check;</td> <!-- PostgreSql -->
        <td>&check;</td> <!-- Amazon Redshift -->
        <td>&#9888;</td> <!-- Oracle -->
        <td>&check;</td> <!-- SqlServer -->
        <td>&#10005;</td> <!-- Cassandra -->
        <td>&#10005;</td> <!-- Amazon Athena -->
        <td>&#9888;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&#10005;</td> <!-- Apache Impala -->
        <td>&#10005;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Table variable</td>
        <td>&#9888;</td> <!-- MySql -->
        <td>&#9888;</td> <!-- SingleStore -->
        <td>&#9888;</td> <!-- PostgreSql -->
        <td>&#9888;</td> <!-- Amazon Redshift -->
        <td>&#9888;</td> <!-- Oracle -->
        <td>&#9888;</td> <!-- SqlServer -->
        <td>&#9888;</td> <!-- Cassandra -->
        <td>&#9888;</td> <!-- Amazon Athena -->
        <td>&#9888;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&#9888;</td> <!-- Apache Impala -->
        <td>&#9888;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Common table expression</td>
        <td>&check;</td> <!-- MySql -->
        <td>&check;</td> <!-- SingleStore -->
        <td>&check;</td> <!-- PostgreSql -->
        <td>&check;</td> <!-- Amazon Redshift -->
        <td>&check;</td> <!-- Oracle -->
        <td>&check;</td> <!-- SqlServer -->
        <td>&#10005;</td> <!-- Cassandra -->
        <td>&check;</td> <!-- Amazon Athena -->
        <td>&check;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&check;</td> <!-- Apache Impala -->
        <td>&check;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Join tables</td>
        <td>&check;</td> <!-- MySql -->
        <td>&check;</td> <!-- SingleStore -->
        <td>&check;</td> <!-- PostgreSql -->
        <td>&check;</td> <!-- Amazon Redshift -->
        <td>&check;</td> <!-- Oracle -->
        <td>&check;</td> <!-- SqlServer -->
        <td>&#10005;</td> <!-- Cassandra -->
        <td>&check;</td> <!-- Amazon Athena -->
        <td>&check;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&check;</td> <!-- Apache Impala -->
        <td>&check;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Union many tables</td>
        <td>&check;</td> <!-- MySql -->
        <td>&check;</td> <!-- SingleStore -->
        <td>&check;</td> <!-- PostgreSql -->
        <td>&check;</td> <!-- Amazon Redshift -->
        <td>&check;</td> <!-- Oracle -->
        <td>&check;</td> <!-- SqlServer -->
        <td>&#10005;</td> <!-- Cassandra -->
        <td>&check;</td> <!-- Amazon Athena -->
        <td>&check;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&check;</td> <!-- Apache Impala -->
        <td>&check;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Table pivot</td>
        <td>&#9888;</td> <!-- MySql -->
        <td>&#9888;</td> <!-- SingleStore -->
        <td>&#9888;</td> <!-- PostgreSql -->
        <td>&#9888;</td> <!-- Amazon Redshift -->
        <td>&#9888;</td> <!-- Oracle -->
        <td>&#9888;</td> <!-- SqlServer -->
        <td>&#9888;</td> <!-- Cassandra -->
        <td>&#9888;</td> <!-- Amazon Athena -->
        <td>&#9888;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&#9888;</td> <!-- Apache Impala -->
        <td>&#9888;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Table unpivot</td>
        <td>&#9888;</td> <!-- MySql -->
        <td>&#9888;</td> <!-- SingleStore -->
        <td>&#9888;</td> <!-- PostgreSql -->
        <td>&#9888;</td> <!-- Amazon Redshift -->
        <td>&#9888;</td> <!-- Oracle -->
        <td>&#9888;</td> <!-- SqlServer -->
        <td>&#9888;</td> <!-- Cassandra -->
        <td>&#9888;</td> <!-- Amazon Athena -->
        <td>&#9888;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&#9888;</td> <!-- Apache Impala -->
        <td>&#9888;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Trace view definition</td>
        <td>&check;</td> <!-- MySql -->
        <td>&check;</td> <!-- SingleStore -->
        <td>&check;</td> <!-- PostgreSql -->
        <td>&check;</td> <!-- Amazon Redshift -->
        <td>&check;</td> <!-- Oracle -->
        <td>&check;</td> <!-- SqlServer -->
        <td>&check;</td> <!-- Cassandra -->
        <td>&check;</td> <!-- Amazon Athena -->
        <td>&check;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&check;</td> <!-- Apache Impala -->
        <td>&check;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Trace variable definition</td>
        <td>&#9888;</td> <!-- MySql -->
        <td>&#9888;</td> <!-- SingleStore -->
        <td>&#9888;</td> <!-- PostgreSql -->
        <td>&#9888;</td> <!-- Amazon Redshift -->
        <td>&#9888;</td> <!-- Oracle -->
        <td>&#9888;</td> <!-- SqlServer -->
        <td>&#9888;</td> <!-- Cassandra -->
        <td>&#9888;</td> <!-- Amazon Athena -->
        <td>&#9888;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&#9888;</td> <!-- Apache Impala -->
        <td>&#9888;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Execute prepared table query</td>
        <td>&#9888;</td> <!-- MySql -->
        <td>&#9888;</td> <!-- SingleStore -->
        <td>&#9888;</td> <!-- PostgreSql -->
        <td>&#9888;</td> <!-- Amazon Redshift -->
        <td>&#9888;</td> <!-- Oracle -->
        <td>&#9888;</td> <!-- SqlServer -->
        <td>&#9888;</td> <!-- Cassandra -->
        <td>&#9888;</td> <!-- Amazon Athena -->
        <td>&#9888;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&#9888;</td> <!-- Apache Impala -->
        <td>&#9888;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
    <tr>
        <td>Call table procedure</td>
        <td>&#9888;</td> <!-- MySql -->
        <td>&#9888;</td> <!-- SingleStore -->
        <td>&#9888;</td> <!-- PostgreSql -->
        <td>&#9888;</td> <!-- Amazon Redshift -->
        <td>&#9888;</td> <!-- Oracle -->
        <td>&#9888;</td> <!-- SqlServer -->
        <td>&#9888;</td> <!-- Cassandra -->
        <td>&#9888;</td> <!-- Amazon Athena -->
        <td>&#9888;</td> <!-- SAP Hana -->
        <!-- <td>&#10005;</td> <!-- Apache Phoenix -->
        <td>&#9888;</td> <!-- Apache Impala -->
        <td>&#9888;</td> <!-- Trino -->
        <td>&#10005;</td> <!-- PrimarSql -->
    </tr>
</table>

## Table Features


<table>

<tr>
<th>Done?</th>
<th>Feature</th>
<th colspan="2">Example</th>
</tr>

<!-- No table -->
<tr>
<td align="center">&#9745;</td>
<td>No table</td>
<td>

```sql
SELECT 1 AS a, '2' AS b
```
</td>
<td>

|Column|References            |
|:----:|----------------------|
|`a`   |1 <em>(literal)</em>  |
|`b`   |'2' <em>(literal)</em>|
</td>
</tr>

<!-- Table access -->
<tr>
<td align="center">&#9745;</td>
<td>Table access</td>
<td>

```sql
-- table : id, name
SELECT * FROM table
```
</td>
<td>

|Column|References  |
|:----:|------------|
|`id`  |table.`id`  |
|`name`|table.`name`|
</td>
</tr>

<!-- Derived table -->
<tr>
<td align="center">&#9745;</td>
<td>Derived table</td>
<td>

```sql
-- table : id, name
SELECT * FROM
    (SELECT * FROM table) AS alias
```
</td>
<td>

|Column|References                   |
|:----:|-----------------------------|
|`id`  |alias.`id`</br>table.`id`    |
|`name`|alias.`name`</br>table.`name`|
</td>
</tr>

<!-- Derived table (Non-Alias) -->
<tr>
<td align="center">&#9745;</td>
<td>Derived table (Non-Alias)</td>
<td>

```sql
-- table : id, name
SELECT * FROM
    (SELECT * FROM table)
```
</td>
<td>

|Column|References                              |
|:----:|----------------------------------------|
|`id`  |<em>derived</em>.`id`</br>table.`id`    |
|`name`|<em>derived</em>.`name`</br>table.`name`|
</td>
</tr>

<!-- Specify columns to table alias -->
<tr>
<td align="center">&#9745;</td>
<td>Specify columns to table alias</td>
<td>

```sql
-- table : id, name
SELECT * FROM table AS alias(a, b)
```
</td>
<td>

|Column|References                |
|:----:|--------------------------|
|`a`   |alias.`a`</br>table.`id`  |
|`b`   |alias.`b`</br>table.`name`|
</td>
</tr>

<!-- Inline derived table -->
<tr>
<td align="center">&#9745;</td>
<td>Inline derived table</td>
<td>

```sql
SELECT * FROM
    (
        VALUES (1, 2), (3, 4)
    ) AS inline_table(a, b)
```
</td>
<td>

|Column|References          |
|:----:|--------------------|
|`a`   |inline_table.`a`</br>1 <em>(literal)</em></br>3 <em>(literal)</em>|
|`b`   |inline_table.`b`</br>2 <em>(literal)</em></br>4 <em>(literal)</em>|
</td>
</tr>

<!-- Table function -->
<tr>
<td align="center">&#9744;</td>
<td>Table function</td>
<td>

```sql
-- function : id, name
SELECT * FROM tbl_func('table function')
```
</td>
<td>
    
|Column|References                |
|:----:|--------------------------|
|`id`   | no reference |
|`name` | no reference |
</td>
</tr>

<!-- Table variable -->
<tr>
<td align="center">&#9744;</td>
<td>Table variable</td>
<td>

```sql
-- TODO
```
</td>
<td>

<!--
|Column|References|
|:-:|-|
|||
-->
</td>
</tr>

<!-- Common table expression -->
<tr>
<td align="center">&#9745;</td>
<td>Common table expression</td>
<td>

```sql
WITH cte AS (SELECT 1 AS n)
SELECT * FROM cte
```
</td>
<td>

|Column|References          |
|:----:|--------------------|
|`n`   |1 <em>(literal)</em>|
</td>
</tr>

<!-- Common table expression (Aliases) -->
<tr>
<td align="center">&#9745;</td>
<td>Common table expression (Aliases)</td>
<td>

```sql
WITH cte (n) AS (SELECT 1)
SELECT * FROM cte
```
</td>
<td>

|Column |References          |
|:-----:|--------------------|
|`n`    |1 <em>(literal)</em>|
</td>
</tr>

<!-- Common table expression (Recursive) -->
<tr>
<td align="center">&#9745;</td>
<td>Common table expression (Recursive)</td>
<td>

```sql
WITH RECURSIVE cte AS (
    SELECT 1 AS n
    UNION ALL
    SELECT n + 1 FROM cte WHERE n < 10
)
SELECT * FROM cte
```
</td>
<td>

|Column|References                  |
|:----:|----------------------------|
|`n`   |1 <em>(literal)</em>        |
|      |cte.`n` <em>(recursive)</em>|
</td>
</tr>

<!-- Join tables -->
<tr>
<td align="center">&#9745;</td>
<td>Join tables</td>
<td>

```sql
-- left_table : name, uid
-- right_table : age, uid
SELECT * FROM
    left_table l
    JOIN right_table r ON l.uid = r.uid
```
</td>
<td>

|Column|References|
|:-:|-|
|`name`|left_table.`name`|
|`uid` |left_table.`uid` |
|`age` |right_table.`age`|
|`uid` |right_table.`uid`|
</td>
</tr>

<!-- Join tables (Pivot columns) -->
<tr>
<td align="center">&#9745;</td>
<td>Join tables (Pivot columns)</td>
<td>

```sql
-- left_table : name, uid
-- right_table : age, uid
SELECT * FROM
    left_table
    JOIN right_table USING (uid)
```
</td>
<td>

|Column|References                            |
|:----:|--------------------------------------|
|`uid` |left_table.`uid`</br>right_table.`uid`|
|`name`|left_table.`name`                     |
|`age` |right_table.`age`                     |
</td>
</tr>

<!-- Union many tables -->
<tr>
<td align="center">&#9745;</td>
<td>Union many tables</td>
<td>

```sql
SELECT a FROM first_table
UNION ALL
SELECT b FROM second_table
```
</td>
<td>

|Column   |References                          |
|:-------:|------------------------------------|
|`a`      |first_table.`a`</br>second_table.`b`|
</td>
</tr>

<!-- Table pivot -->
<tr>
<td align="center">&#9744;</td>
<td>Table pivot</td>
<td>

```sql
-- TODO
```
</td>
<td>

<!--
|Column|References|
|:-:|-|
|||
-->
</td>
</tr>

<!-- Table unpivot -->
<tr>
<td align="center">&#9744;</td>
<td>Table unpivot</td>
<td>

```sql
-- TODO
```
</td>
<td>

<!--
|Column|References|
|:-:|-|
|||
-->
</td>
</tr>

<!-- Trace view definition -->
<tr>
<td align="center">&#9745;</td>
<td>Trace view definition</td>
<td>

```sql
-- table_view : a, b
-- table : id, name
SELECT * FROM table_view
```
</td>
<td>

|Column|References                     |
|:----:|-------------------------------|
|`a`   |table_view.`a`</br>table.`id`  |
|`b`   |table_view.`b`</br>table.`name`|
</td>
</tr>

<!--
|Column|References|
|:-:|-|
|||
-->
</td>
</tr>

<!-- Trace variable definition -->
<tr>
<td align="center">&#9744;</td>
<td>Trace variable definition</td>
<td>

```sql
-- TODO
```
</td>
<td>

<!--
|Column|References|
|:-:|-|
|||
-->
</td>
</tr>

<!--
|Column|References|
|:-:|-|
|||
-->
</td>
</tr>

<!-- Execute prepared table query -->
<tr>
<td align="center">&#9744;</td>
<td>Execute prepared table query</td>
<td>

```sql
-- TODO
```
</td>
<td>

<!--
|Column|References|
|:-:|-|
|||
-->
</td>
</tr>

<!-- Call table procedure -->
<tr>
<td align="center">&#9744;</td>
<td>Call table procedure</td>
<td>

```sql
-- TODO
```
</td>
<td>

<!--
|Column|References|
|:-:|-|
|||
-->
</td>
</tr>

</table>

## Development

#### Requirements

- PowerShell
- .NET Core 3.1
- Java >= 1.8

### Command

#### Setup

```ps1
PS> cd ./qsi
PS> ./Setup.ps1
```

#### Publish

```ps1
PS> cd ./qsi
PS> ./Publish.ps1 <VERSION> [-Mode <PUBLISH_MODE>]
```

- `<VERSION>`

  Specifies the package version.
  
  Version must be greater than the latest version tag on git.

- `-Mode <PUBLISH_MODE>`

  Specifies the publish mode.
  
  |PUBLISH_MODE|Action|
  |--|--|
  |Publish(Default)|Publish packages to NuGet, GitHub repositories|
  |Local|Publish packages to local repository|
  |Archive|Build packages to `./qsi/Publish`|
  
  

### Debugger

It supports abstract syntax trees and semantic trees, and a debugger that can debug compilation results.

![Preview](https://github.com/chequer-io/qsi/blob/main/Qsi.Debugger/Screenshot.png?raw=true)

### Run
```sh
$ cd qsi/Qsi.Debugger
$ dotnet run
```
