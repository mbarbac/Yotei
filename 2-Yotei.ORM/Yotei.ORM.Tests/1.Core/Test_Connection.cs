﻿#pragma warning disable IDE0079
#pragma warning disable CA1859

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_Connection
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        Assert.Equal(Connection.RETRIES, connection.Retries);
        Assert.Equal(Connection.RETRYINTERVAL, connection.RetryInterval.Milliseconds);
        //Assert.NotNull(connection.Transaction);
        Assert.NotEmpty(connection.ToDatabase);

        Assert.Equal(2, connection.ToDatabase.Count);
        Locale locale = new();

        DateOnly dateonly = new(2001, 12, 31);
        DateTime datetime = (DateTime)connection.ToDatabase.TryConvert(dateonly, locale)!;
        Assert.Equal(2001, datetime.Year);
        Assert.Equal(12, datetime.Month);
        Assert.Equal(31, datetime.Day);

        datetime = new(2020, 11, 29);
        dateonly = (DateOnly)connection.ToDatabase.TryConvert(datetime, locale)!;
        Assert.Equal(2020, dateonly.Year);
        Assert.Equal(11, dateonly.Month);
        Assert.Equal(29, dateonly.Day);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new FakeConnection(engine);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Retries, target.Retries);
        Assert.Equal(source.RetryInterval, target.RetryInterval);
        //Assert.NotSame(source.Transaction, target.Transaction);
        Assert.NotSame(source.ToDatabase, target.ToDatabase);

        Assert.Equal(2, target.ToDatabase.Count);
        Assert.NotNull(target.ToDatabase.Find<DateTime>());
        Assert.NotNull(target.ToDatabase.Find<DateOnly>());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Open_Close()
    {
        IConnection connection;

        using (connection = new FakeConnection(new FakeEngine()))
        {
            Assert.False(connection.IsOpen);

            connection.Open();
            Assert.True(connection.IsOpen);
            //Assert.False(connection.Transaction.IsActive);
            //Assert.False(connection.Transaction.IsDisposed);

            connection.Close();
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
            //Assert.False(connection.Transaction.IsActive);
            //Assert.False(connection.Transaction.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
        //Assert.True(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Close_From_Transaction()
    {
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(new FakeEngine()))
        {
            Assert.False(connection.IsOpen);

            using (transaction = connection.CreateTransaction())
            {
                transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(transaction.IsActive);

                transaction.Commit();
                Assert.False(connection.IsOpen);
                Assert.False(connection.IsDisposed);
                Assert.False(transaction.IsActive);
            }   
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
        Assert.True(transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Close_From_Transaction_Implicit_Abort()
    {
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(new FakeEngine()))
        {
            Assert.False(connection.IsOpen);

            using (transaction = connection.CreateTransaction())
            {
                transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(transaction.IsActive);
            }

            Assert.False(transaction.IsActive);
            Assert.True(transaction.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }
}