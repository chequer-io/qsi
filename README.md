![Logo](https://github.com/chequer-io/qsi/blob/master/Resources/logo-256.png?raw=true)

The QSI is the pure C# Query Structure Interface.

## Languages

|Language|Parser|Repos|
|--|--|--|
|MySql|Antlr4|[grammars-v4](https://github.com/antlr/grammars-v4)|
|PostgreSql10|PostgreSQL server source code|[libpg_query](https://github.com/lfittl/libpg_query), [ChakraCore](https://github.com/microsoft/ChakraCore)|
|JSql|JavaCC|[JSqlParser](https://github.com/JSQLParser/JSqlParser), [IKVM](https://github.com/ikvm-revived/ikvm)|
|Oracle|Qsi.JSql||
|SqlServer|Microsoft.SqlServer.TransactSql.ScriptDom|[MSDN](https://docs.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.transactsql.scriptdom?view=sql-dacfx-150), [NuGet](https://www.nuget.org/packages/Microsoft.SqlServer.TransactSql.ScriptDom)|


## Structure Compiler
It compiles the result structure and relation 

based on semantic tree transformed by parser's  for each language.

## Status

<!-- 
&#x2714; ✔
&#x274C; ❌
&#x26A0; ⚠
-->

|Features                           |MySql   |PostgreSql|JSql    |Oracle  |SqlServer|
|-----------------------------------|:------:|:--------:|:------:|:------:|:-------:|
|No table                           |&#x2714;|&#x2714;  |&#x2714;|&#x274C;|&#x2714; |
|Table access                       |&#x2714;|&#x2714;  |&#x2714;|&#x2714;|&#x2714; |
|Derived table                      |&#x2714;|&#x2714;  |&#x2714;|&#x2714;|&#x2714; |
|Derived table (Non-Alias)          |&#x274C;|&#x274C;  |&#x2714;|&#x2714;|&#x274C; |
|Specify columns to table alias     |&#x274C;|&#x2714;  |&#x2714;|&#x274C;|&#x274C; |
|Inline derived table               |&#x274C;|&#x2714;  |&#x2714;|&#x2714;|&#x2714; |
|Table function                     |&#x26A0;|&#x26A0;  |&#x26A0;|&#x26A0;|&#x26A0; |
|Common table expression            |&#x2714;|&#x2714;  |&#x2714;|&#x2714;|&#x2714; |
|Common table expression (Aliases)  |&#x2714;|&#x2714;  |&#x2714;|&#x2714;|&#x2714; |
|Common table expression (Recursive)|&#x2714;|&#x2714;  |&#x2714;|&#x2714;|&#x2714; |
|Join tables                        |&#x2714;|&#x2714;  |&#x2714;|&#x2714;|&#x2714; |
|Join tables (Pivot columns)        |&#x2714;|&#x2714;  |&#x2714;|&#x2714;|&#x274C; |
|Union many tables                  |&#x2714;|&#x2714;  |&#x2714;|&#x2714;|&#x2714; |
|Table pivot                        |&#x26A0;|&#x26A0;  |&#x26A0;|&#x26A0;|&#x26A0; |
|Table unpivot                      |&#x26A0;|&#x26A0;  |&#x26A0;|&#x26A0;|&#x26A0; |
|Trace view definition              |&#x2714;|&#x2714;  |&#x2714;|&#x2714;|&#x2714; |
|Execute prepared table query       |&#x26A0;|&#x26A0;  |&#x26A0;|&#x26A0;|&#x26A0; |
|Call table procedure               |&#x26A0;|&#x26A0;  |&#x26A0;|&#x26A0;|&#x26A0; |

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

|OS     |Support |
|-------|:------:|
|Windows|&#x2714;|
|OSX    |&#x2714;|
|Linux  |&#x274C;|

![Preview](https://github.com/chequer-io/qsi/blob/master/Qsi.Debugger/Screenshot.png?raw=true)

### Run
```sh
$ cd qsi/Qsi.Debugger
$ dotnet run
```
