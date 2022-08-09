using NUnit.Framework;
using Qsi.Tree.Data;

namespace Qsi.Tests;

public class UserDataHolderTest
{
    [TestCase]
    public void Test_PutData()
    {
        var holder = new UserDataHolder();
        var key1 = new Key<int>("A");
        var key2 = new Key<int>("B");
        var key3 = new Key<int>("C");

        holder.PutData(key1, 1);
        holder.PutData(key2, 2);
        holder.PutData(key3, 3);

        var v1 = holder.GetData(key1);
        var v2 = holder.GetData(key2);
        var v3 = holder.GetData(key3);

        Assert.AreEqual(1, v1);
        Assert.AreEqual(2, v2);
        Assert.AreEqual(3, v3);
    }

    [TestCase]
    public void Test_RemoveFirst()
    {
        var holder = new UserDataHolder();
        var key1 = new Key<int>("A");
        var key2 = new Key<int>("B");
        var key3 = new Key<int>("C");

        holder.PutData(key1, 1);
        holder.PutData(key2, 2);
        holder.PutData(key3, 3);

        // Remove
        holder.PutData(key1, default);

        var v1 = holder.GetData(key1);
        var v2 = holder.GetData(key2);
        var v3 = holder.GetData(key3);

        Assert.AreEqual(0, v1);
        Assert.AreEqual(2, v2);
        Assert.AreEqual(3, v3);
    }

    [TestCase]
    public void Test_RemoveCenter()
    {
        var holder = new UserDataHolder();
        var key1 = new Key<int>("A");
        var key2 = new Key<int>("B");
        var key3 = new Key<int>("C");

        holder.PutData(key1, 1);
        holder.PutData(key2, 2);
        holder.PutData(key3, 3);

        // Remove
        holder.PutData(key2, default);

        var v1 = holder.GetData(key1);
        var v2 = holder.GetData(key2);
        var v3 = holder.GetData(key3);

        Assert.AreEqual(1, v1);
        Assert.AreEqual(0, v2);
        Assert.AreEqual(3, v3);
    }

    [TestCase]
    public void Test_RemoveLast()
    {
        var holder = new UserDataHolder();
        var key1 = new Key<int>("A");
        var key2 = new Key<int>("B");
        var key3 = new Key<int>("C");

        holder.PutData(key1, 1);
        holder.PutData(key2, 2);
        holder.PutData(key3, 3);

        // Remove
        holder.PutData(key3, default);

        var v1 = holder.GetData(key1);
        var v2 = holder.GetData(key2);
        var v3 = holder.GetData(key3);

        Assert.AreEqual(1, v1);
        Assert.AreEqual(2, v2);
        Assert.AreEqual(0, v3);
    }
}
