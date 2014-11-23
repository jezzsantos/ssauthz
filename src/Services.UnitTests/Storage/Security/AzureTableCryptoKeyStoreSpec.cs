using System;
using System.Linq;
using Common.Storage;
using Common.Storage.Security;
using DotNetOpenAuth.Messaging.Bindings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testing.Common;

namespace Services.UnitTests.Storage.Security
{
    public class AzureTableCryptoKeyStoreSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private CryptoKeyStore store;
            private Mock<IStorageProvider<IOAuthCryptoKey>> storage;

            [TestInitialize]
            public void Initialize()
            {
                this.storage = new Mock<IStorageProvider<IOAuthCryptoKey>>();
                this.store = new CryptoKeyStore
                {
                    Storage = this.storage.Object,
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetKeyWithNullBucket_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () => this.store.GetKey(null, "foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetKeyWithNullHandle_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () => this.store.GetKey("foo", null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetKeyWithUnknownKey_ThenReturnsNull()
            {
                this.storage.Setup(s => s.Get(It.IsAny<string>()))
                    .Returns((IOAuthCryptoKey)null);

                var result = this.store.GetKey("foo", "bar");

                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetKeyWithKnownKey_ThenReturnsKey()
            {
                var key = new OAuthCryptoKey
                {
                    Bucket = "foo",
                    Handle = "bar",
                    Key = new byte[]
                    {
                        0x01,
                        0x01
                    },
                    ExpiresUtc = DateTime.UtcNow,
                };
                this.storage.Setup(s => s.Find(It.IsAny<string>()))
                    .Returns(new[]
                    {
                        key
                    });

                var result = this.store.GetKey("foo", "bar");

                Assert.Equal(key.Key, result.Key);
                Assert.Equal(key.ExpiresUtc, result.ExpiresUtc);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetKeysWithNullBucket_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    this.store.GetKeys(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetKeys_ThenReturnsKeys()
            {
                var key = new OAuthCryptoKey
                {
                    Bucket = "foo",
                    Handle = "bar",
                    Key = new byte[]
                    {
                        0x01,
                        0x01
                    },
                    ExpiresUtc = DateTime.UtcNow,
                };

                this.storage.Setup(s => s.Find(It.IsAny<string>()))
                    .Returns(new[]
                    {
                        key
                    });

                var keys = this.store.GetKeys("foo");

                Assert.Equal(1, keys.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenRemoveKeyWithNullBucket_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    this.store.RemoveKey(null, "foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenRemoveKeyWithNullHandle_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    this.store.RemoveKey("foo", null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenRemoveKeyWithUnknownHandle_ThenRemovesKey()
            {
                var key = new OAuthCryptoKey
                {
                    Id = "fooid",
                    Bucket = "foo",
                    Handle = "bar",
                    Key = new byte[]
                    {
                        0x01,
                        0x01
                    },
                    ExpiresUtc = DateTime.UtcNow,
                };

                this.storage.Setup(s => s.Find(It.IsAny<string>()))
                    .Returns(new[]
                    {
                        key
                    });

                this.store.RemoveKey("foo", "bar2");

                this.storage.Verify(s => s.Delete("fooid"), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenRemoveKeyWithKnownHandle_ThenReturnsKeys()
            {
                var key = new OAuthCryptoKey
                {
                    Id = "fooid",
                    Bucket = "foo",
                    Handle = "bar",
                    Key = new byte[]
                    {
                        0x01,
                        0x01
                    },
                    ExpiresUtc = DateTime.UtcNow,
                };

                this.storage.Setup(s => s.Find(It.IsAny<string>()))
                    .Returns(new[]
                    {
                        key
                    });

                this.store.RemoveKey("foo", "bar");

                this.storage.Verify(s => s.Delete("fooid"), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenStoreKey_ThenAddsKey()
            {
                var fakeKey = new FakeCryptoKey();

                this.store.StoreKey("foo", "bar", fakeKey);

                this.storage.Verify(s => s.Add(It.Is<OAuthCryptoKey>(ack =>
                    ack.Bucket == "foo"
                    && ack.Handle == "bar"
                    && ack.ExpiresUtc == DateTime.Today.ToUniversalTime())), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenStoreExistingKey_ThenReplacesKey()
            {
                var fakeKey = new FakeCryptoKey();
                var key = new OAuthCryptoKey
                {
                    Id = "fooid",
                    Bucket = "foo",
                    Handle = "bar",
                    Key = new byte[]
                    {
                        0x01,
                        0x01
                    },
                    ExpiresUtc = DateTime.UtcNow,
                };

                this.storage.Setup(s => s.Find(It.IsAny<string>()))
                    .Returns(new[]
                    {
                        key
                    });

                this.store.StoreKey("foo", "bar", fakeKey);

                this.storage.Verify(s => s.Delete("fooid"), Times.Once());
                this.storage.Verify(s => s.Add(It.Is<OAuthCryptoKey>(ack =>
                    ack.Bucket == "foo"
                    && ack.Handle == "bar"
                    && ack.ExpiresUtc == DateTime.Today.ToUniversalTime())), Times.Once());
            }
        }

        internal class FakeCryptoKey : CryptoKey
        {
            private static byte[] key =
            {
                0x01,
                0x01
            };

            /// <summary>
            /// Creates a new instance of the <see cref="FakeCryptoKey"/> class.
            /// </summary>
            public FakeCryptoKey()
                : base(key, DateTime.Today.ToUniversalTime())
            {
            }
        }
    }
}
