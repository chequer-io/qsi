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

    protected string ConnectionString { get; }

    private TransactionScope _transactionScope;

    protected VendorTestBase(string connectionString)
    {
        ConnectionString = connectionString;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        PrepareConnectionPreview();

        _transactionScope = new TransactionScope();
        Connection = OpenConnection(ConnectionString);
        Connection.Open();
        Connection.EnlistTransaction(Transaction.Current);

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
        _transactionScope.Dispose();
        Connection.Close();
        Connection.Dispose();
    }

    protected virtual void PrepareConnectionPreview()
    {
    }

    protected abstract DbConnection OpenConnection(string connectionString);

    protected virtual void PrepareConnection(DbConnection connection)
    {
    }

    protected abstract IQsiLanguageService CreateLanguageService(DbConnection connection);
}
