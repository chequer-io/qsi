using System;
using NUnit.Framework;

namespace Qsi.Tests.PrimarSql;

public partial class PrimarSqlTest
{
    private static readonly TestCaseData[] Select_TestDatas =
    {
        new("SELECT * FROM tbl_hash"),
        new("SELECT * FROM tbl_hash_sort")
    };

    private static readonly TestCaseData[] Execute_TestDatas =
    {
        new("INSERT INTO tbl_hash VALUES (1), (2)", Array.Empty<string>(), 1),
        new("INSERT INTO tbl_hash (hash) VALUES (1), (2)", Array.Empty<string>(), 1),
        new("INSERT INTO tbl_hash (x, hash) VALUES ('x', 1), ('x', 2)", Array.Empty<string>(), 1),
        new("INSERT INTO tbl_hash (hash, y) VALUES (1, 'y'), (2, 'y')", Array.Empty<string>(), 1),

        new("DELETE FROM tbl_hash", new[] { "SELECT * FROM tbl_hash" }, 1),
        new("DELETE FROM tbl_hash WHERE hash = 2", new[] { "SELECT * FROM tbl_hash WHERE hash = 2" }, 1),
    };

    private static readonly TestCaseData[] Print_TestDatas =
    {
        new("INSERT INTO tbl_hash VALUES (1), (2)"),
        new("INSERT INTO tbl_hash (hash) VALUES (1), (2)"),
        new("INSERT INTO tbl_hash (x, hash) VALUES ('x', 1), ('x', 2)"),
        new("INSERT INTO tbl_hash (hash, y) VALUES (1, 'y'), (2, 'y')"),

        new("INSERT INTO tbl_hash VALUES {'hash': 1}, {'hash': 2}"),
        new("INSERT INTO tbl_hash VALUES {'x': 'x', 'hash': 1}, {'x': 'x', 'hash': 2}"),
        new("INSERT INTO tbl_hash VALUES {'hash': 1, 'y': 'y'}, {'hash': 2, 'y': 'y'}"),
        new("INSERT INTO tbl_hash VALUES {'a': 'a', 'hash': 1}, {'b': 'b', 'hash': 2}"),
        new("INSERT INTO tbl_hash VALUES {'a': 'a', 'b': 'b', 'hash': 1}, {'b': 'c', 'hash': 2}"),
        new("INSERT INTO tbl_hash VALUES {'hash': 1, 'a': 'a'}, {'hash': 2, 'b': 'b'}"),
        new("INSERT INTO tbl_hash VALUES {'hash': 1, 'a': 'a'}, {'hash': 2}"),
        new("INSERT INTO tbl_hash VALUES {'hash': 1}, {'hash': 2, 'b': 'b'}, {'hash': 3, 'c': 'c'}"),

        new("INSERT INTO tbl_hash_sort VALUES (1, 2), (3, 4)"),
        new("INSERT INTO tbl_hash_sort (hash, sort) VALUES (1, 2), (3, 4)"),
        new("INSERT INTO tbl_hash_sort (sort, hash) VALUES (2, 1), (4, 3)"),
        new("INSERT INTO tbl_hash_sort (x, hash, sort) VALUES ('x', 1, 2), ('x', 3, 4)"),
        new("INSERT INTO tbl_hash_sort (sort, hash, y) VALUES (2, 1, 'y'), (4, 3, 'y')"),

        new("INSERT INTO tbl_hash VALUES {'hash': 1, 'sort': 2}, {'hash': 3, 'sort': 4}"),
        new("INSERT INTO tbl_hash VALUES {'hash': 1, 'sort': 2}, {'sort': 4, 'hash': 3}"),
        new("INSERT INTO tbl_hash VALUES {'x':'x', 'hash': 1, 'sort': 2}, {'sort': 4, 'hash': 3, 'y': 'y'}"),
        new("INSERT INTO tbl_hash VALUES {'hash': 1, 'x':'x', 'sort': 2}, {'sort': 4, 'y': 'y', 'hash': 3}"),
    };

    private static readonly TestCaseData[] Throw_TestDatas =
    {
        new("INSERT INTO tbl_hash (a) VALUES (1)") { ExpectedResult = "QSI-0005: hash column not specified" },
        new("INSERT INTO tbl_hash_sort (a) VALUES (1)") { ExpectedResult = "QSI-0005: hash, sort columns not specified" },
        new("INSERT INTO tbl_hash_sort (sort) VALUES (1)") { ExpectedResult = "QSI-0005: hash column not specified" },
        new("INSERT INTO tbl_hash_sort (hash) VALUES (1)") { ExpectedResult = "QSI-0005: sort column not specified" },

        new("INSERT INTO tbl_hash VALUES {'a':1}") { ExpectedResult = "QSI-0005: hash column not specified" },
        new("INSERT INTO tbl_hash_sort VALUES {'a':1}") { ExpectedResult = "QSI-0005: hash, sort columns not specified" },
        new("INSERT INTO tbl_hash_sort VALUES {'sort':1}") { ExpectedResult = "QSI-0005: hash column not specified" },
        new("INSERT INTO tbl_hash_sort VALUES {'hash':1}") { ExpectedResult = "QSI-0005: sort column not specified" },
    };
}
