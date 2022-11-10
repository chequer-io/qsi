using System.Collections.Generic;
using System.Data.Common;
using System.Transactions;
using NUnit.Framework;
using Qsi.Data;
using Qsi.Engines;
using Qsi.Services;

namespace Qsi.Tests;

[NonParallelizable]
public abstract class VendorTestBase
{
    protected IList<QsiScript> ScriptHistories => ((RepositoryProviderDriverBase)Engine.RepositoryProvider).ScriptHistories;

    protected DbConnection Connection { get; private set; }

    protected QsiEngine Engine { get; private set; }

    private readonly string _connectionString;

    protected VendorTestBase(string connectionString)
    {
        _connectionString = connectionString;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Connection = OpenConnection(_connectionString);
        Connection.Open();
        
        PrepareConnection(Connection);

        Engine = new QsiEngine(CreateLanguageService(Connection));
    }

    [SetUp]
    public void SetUp()
    {
        ScriptHistories.Clear();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Connection.Close();
        Connection.Dispose();
    }

    protected abstract DbConnection OpenConnection(string connectionString);

    protected abstract void PrepareConnection(DbConnection connection);

    protected abstract IQsiLanguageService CreateLanguageService(DbConnection connection);
}
