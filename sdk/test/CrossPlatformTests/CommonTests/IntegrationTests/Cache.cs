using System;
using System.Linq;
using System.Threading;

using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Util;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DocumentModel;
using System.Text;
using System.IO;
using NUnit.Framework;
using CommonTests.Framework;
using CommonTests.IntegrationTests.DynamoDB;

namespace CommonTests.IntegrationTests
{
    [TestFixture]
    public class CacheTests
    {
        private const string tableName = "cache-test-table";
        private AmazonDynamoDBClient client = null;
        private bool lastUseSdkCacheValue;

        [SetUp]
        public void Init()
        {
            lastUseSdkCacheValue = AWSConfigs.UseSdkCache;
            AWSConfigs.UseSdkCache = true;
            SdkCache.Clear();

            client = TestBase.CreateClient<AmazonDynamoDBClient>();
        }

        [TearDown]
        public void Cleanup()
        {
            var tableExists = true;

            //var status = AWSSDK_DotNet.IntegrationTests.Tests.DynamoDB.DynamoDBTests.GetStatus(tableName);
            //tableExists = (status != null);

            var allTables = client.ListTablesAsync().Result.TableNames;
            tableExists = allTables.Contains(tableName);

            if (tableExists)
                DeleteTable();

            AWSConfigs.UseSdkCache = lastUseSdkCacheValue;
            SdkCache.Clear();
        }


        [Test]
        public void TestCache()
        {
            Func<string, TableDescription> creator = tn => client.DescribeTableAsync(tn).Result.Table;

            var tableName = GetTableName();
            var tableCache = SdkCache.GetCache<string, TableDescription>(client, DynamoDBTests.TableCacheIdentifier, StringComparer.Ordinal);
            Assert.AreEqual(0, tableCache.ItemCount);

            using (var counter = new ServiceResponseCounter(client))
            {
                var table = tableCache.GetValue(tableName, creator);
                Assert.AreEqual(1, counter.ResponseCount);
                Assert.AreEqual(1, tableCache.ItemCount);

                // verify the item is still there
                table = tableCache.GetValue(tableName, creator);
                Assert.AreEqual(1, counter.ResponseCount);

                // verify item was reloaded
                tableCache.Clear(tableName);
                table = tableCache.GetValue(tableName, creator);
                Assert.AreEqual(2, counter.ResponseCount);
                Assert.AreEqual(1, tableCache.ItemCount);

                // test item expiration
                tableCache.MaximumItemLifespan = TimeSpan.FromSeconds(1);
                UtilityMethods.Sleep(tableCache.MaximumItemLifespan);
                table = tableCache.GetValue(tableName, creator);
                Assert.AreEqual(3, counter.ResponseCount);
                Assert.AreEqual(1, tableCache.ItemCount);
            }
        }

        [Test]
        public void MultipleClientsTest()
        {
            Table.ClearTableCache();
            var tableName = GetTableName();

            Table table;
            using (var nc = TestBase.CreateClient<AmazonDynamoDBClient>())
            {
                table = Table.LoadTable(nc, tableName);
            }

            Table.ClearTableCache();
            table = Table.LoadTable(client, tableName);
        }

        [Test]
        public void ChangingTableTest()
        {
            var item = new Document(new Dictionary<string, DynamoDBEntry>
            {
                { "Id", 42 },
                { "Name", "Floyd" },
                { "Coffee", "Yes" }
            });
            var tableCache = SdkCache.GetCache<string, TableDescription>(client, DynamoDBTests.TableCacheIdentifier, StringComparer.Ordinal);

            CreateTable(defaultKeys: true);
            var table = Table.LoadTable(client, tableName);
            table.PutItemAsync(item).Wait();

            using (var counter = new ServiceResponseCounter(client))
            {
                var doc = table.GetItemAsync(42, "Floyd").Result;
                Assert.IsNotNull(doc);
                Assert.AreNotEqual(0, doc.Count);
                Assert.AreEqual(1, counter.ResponseCount);
                AssertExtensions.ExpectExceptionAsync(table.GetItemAsync("Floyd", 42)).Wait();

                var oldTableDescription = tableCache.GetValue(tableName, null);

                DeleteTable();
                CreateTable(defaultKeys: false);

                table.PutItemAsync(item).Wait();
                AssertExtensions.ExpectExceptionAsync(table.GetItemAsync(42, "Yes")).Wait();

                counter.Reset();
                Table.ClearTableCache();
                table = Table.LoadTable(client, tableName);
                doc = table.GetItemAsync(42, "Yes").Result;
                Assert.IsNotNull(doc);
                Assert.AreNotEqual(0, doc.Count);
                Assert.AreEqual(2, counter.ResponseCount);

                counter.Reset();
                Table.ClearTableCache();
                PutItem(tableCache, tableName, oldTableDescription);
                table = Table.LoadTable(client, tableName);
                doc = tableCache.UseCache(tableName,
                    () => table.GetItemAsync(42, "Yes").Result,
                    () => table = Table.LoadTable(client, tableName),
                    shouldRetryForException: null);

                Assert.IsNotNull(doc);
                Assert.AreNotEqual(0, doc.Count);
                Assert.AreEqual(2, counter.ResponseCount);
            }
        }

        private void CreateTable(bool defaultKeys)
        {
            var keySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "Id",
                    KeyType = KeyType.HASH
                },
                new KeySchemaElement
                {
                    AttributeName = defaultKeys ? "Name" : "Coffee",
                    KeyType = KeyType.RANGE
                }
            };

            var attributes = new List<AttributeDefinition>
            {
                new AttributeDefinition
                {
                    AttributeName = "Id",
                    AttributeType = ScalarAttributeType.N
                },
                new AttributeDefinition
                {
                    AttributeName = defaultKeys ? "Name" : "Coffee",
                    AttributeType = ScalarAttributeType.S
                }
            };

            client.CreateTableAsync(new CreateTableRequest
            {
                TableName = tableName,
                KeySchema = keySchema,
                AttributeDefinitions = attributes,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            }).Wait();

            DynamoDBTests.WaitForTableStatus(client, tableName, TableStatus.ACTIVE);
        }
        private void DeleteTable()
        {
            client.DeleteTableAsync(tableName).Wait();
            DynamoDBTests.WaitForTableStatus(client, tableName, null);
        }
        private string GetTableName()
        {
            var tables = client.ListTablesAsync().Result.TableNames;
            Assert.AreNotEqual(0, tables.Count);
            var tableName = tables.First();

            return tableName;
        }
        private void PutItem<TKey,TValue>(ICache<TKey,TValue> cache, TKey key, TValue value)
        {
            cache.Clear(key);
            cache.GetValue(key, k => value);
        }
    }
}
