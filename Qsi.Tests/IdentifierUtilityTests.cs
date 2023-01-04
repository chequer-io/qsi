using System;
using NUnit.Framework;
using Qsi.Utilities;

namespace Qsi.Tests;

public class IdentifierUtilityTests
{
    // '..'
    [TestCase("'abcd'", ExpectedResult = "abcd")]
    [TestCase("'ab\"`[]$$cd'", ExpectedResult = "ab\"`[]$$cd")]
    [TestCase("'ab''cd'", ExpectedResult = "ab'cd")]
    [TestCase("'ab''''cd'", ExpectedResult = "ab''cd")]
    [TestCase("'''abcd'''", ExpectedResult = "'abcd'")]
    [TestCase(@"'ab\'cd'", ExpectedResult = "ab'cd")]
    [TestCase(@"'ab\'\'cd'", ExpectedResult = "ab''cd")]
    [TestCase(@"'\'abcd\''", ExpectedResult = "'abcd'")]
    // ".."
    [TestCase("\"abcd\"", ExpectedResult = "abcd")]
    [TestCase("\"ab'`[]$$cd\"", ExpectedResult = "ab'`[]$$cd")]
    [TestCase("\"ab\"\"cd\"", ExpectedResult = "ab\"cd")]
    [TestCase("\"ab\"\"\"\"cd\"", ExpectedResult = "ab\"\"cd")]
    [TestCase("\"\"\"abcd\"\"\"", ExpectedResult = "\"abcd\"")]
    [TestCase(@"""ab\""cd""", ExpectedResult = "ab\"cd")]
    [TestCase(@"""ab\""\""cd""", ExpectedResult = "ab\"\"cd")]
    [TestCase(@"""\""abcd\""""", ExpectedResult = "\"abcd\"")]
    // `..`
    [TestCase("`abcd`", ExpectedResult = "abcd")]
    [TestCase("`ab'\"[]$$cd`", ExpectedResult = "ab'\"[]$$cd")]
    [TestCase("`ab``cd`", ExpectedResult = "ab`cd")]
    [TestCase("`ab````cd`", ExpectedResult = "ab``cd")]
    [TestCase("```abcd```", ExpectedResult = "`abcd`")]
    [TestCase(@"`ab\`cd`", ExpectedResult = "ab`cd")]
    [TestCase(@"`ab\`\`cd`", ExpectedResult = "ab``cd")]
    [TestCase(@"`\`abcd\``", ExpectedResult = "`abcd`")]
    // [..]
    [TestCase("[abcd]", ExpectedResult = "abcd")]
    [TestCase("[ab'\"`$$cd]", ExpectedResult = "ab'\"`$$cd")]
    [TestCase("[ab]]cd]", ExpectedResult = "ab]cd")]
    [TestCase("[ab[]]cd]", ExpectedResult = "ab[]cd")]
    [TestCase("[ab]]]]cd]", ExpectedResult = "ab]]cd")]
    [TestCase("[ab[[]]]]cd]", ExpectedResult = "ab[[]]cd")]
    [TestCase("[[abcd]]]", ExpectedResult = "[abcd]")]
    [TestCase(@"[ab\]cd]", ExpectedResult = "ab]cd")]
    [TestCase(@"[ab\[\]cd]", ExpectedResult = "ab[]cd")]
    [TestCase(@"[ab\]\]cd]", ExpectedResult = "ab]]cd")]
    [TestCase(@"[ab\[\[\]\]cd]", ExpectedResult = "ab[[]]cd")]
    [TestCase(@"[\]abcd\]]", ExpectedResult = "]abcd]")]
    // $$..$$
    [TestCase("$$abcd$$", ExpectedResult = "abcd")]
    [TestCase("$$ab'\"`[]cd$$", ExpectedResult = "ab'\"`[]cd")]
    [TestCase("$$ab$$$$cd$$", ExpectedResult = "ab$$cd")]
    [TestCase("$$ab$$$$$$$$cd$$", ExpectedResult = "ab$$$$cd")]
    [TestCase("$$$$$$abcd$$$$$$", ExpectedResult = "$$abcd$$")]
    // misc
    [TestCase(@"'\'\""\`\[\]\\\n\r\t\b\0'", ExpectedResult = "'\"`[]\\\n\r\t\b\0")]
    public string Test_Unescape(string value)
    {
        return IdentifierUtility.Unescape(value);
    }

    [TestCase(@"'''")]
    public void Test_Unescape_Fail(string value)
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            IdentifierUtility.Unescape(value);
        });
    }
}
