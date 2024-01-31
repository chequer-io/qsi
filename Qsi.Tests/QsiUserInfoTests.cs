using NUnit.Framework;
using Qsi.Data;

namespace Qsi.Tests;

public class QsiUserInfoTests
{
    [TestCase("TestKey1", "TestValue")]
    [TestCase("TestKey2", true)]
    [TestCase("TestKey3", false)]
    [TestCase("TestKey4", null)]
    [TestCase("TestKey5", 123)]
    [TestCase("TestKey6", 456)]
    // MySql
    [TestCase(MySql.Tree.MySqlProperties.User.IsRandomPassword, true)]
    [TestCase(MySql.Tree.MySqlProperties.User.IsRandomPassword, false)]
    public void Test_Properties(string key, object value)
    {
        var user = new QsiUserInfo
        {
            Properties = { [key] = value }
        };

        Assert.AreEqual(user.Properties[key], value);
    }
}
