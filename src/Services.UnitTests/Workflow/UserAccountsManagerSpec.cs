using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Security;
using Common.Services;
using Common.Storage;
using Common.Storage.DataEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services.AuthZ.Properties;
using Services.AuthZ.Workflow;
using Services.DataContracts;
using Testing.Common;

namespace Services.UnitTests.Workflow
{
    public class UserAccountsManagerSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private UserAccountsManager manager;
            private Mock<IStorageProvider<IUserAccount>> storageProvider;

            [TestInitialize]
            public void Initialize()
            {
                storageProvider = new Mock<IStorageProvider<IUserAccount>>();
                manager = new UserAccountsManager
                {
                    Storage = storageProvider.Object,
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetUserAccountWithNullId_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () => manager.GetUserAccount("auserid", null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetUserAccountWithUnknownId_ThenThrows()
            {
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>())).Returns((IUserAccount) null);

                Assert.Throws<ResourceNotFoundException>(
                    () => manager.GetUserAccount("auserid", "foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetUserAccountWithKnownId_ThenReturnsAccount()
            {
                var account = new UserAccount();
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>())).Returns(account);

                IUserAccount result = manager.GetUserAccount("auserid", "foo");

                Assert.Equal(account, result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenListUserAccountsWithUserName_ThenReturnsAccountsByUsername()
            {
                var account = new UserAccount();
                storageProvider.Setup(sp => sp.Find(It.IsAny<string>()))
                    .Returns(new[]
                    {
                        account
                    });

                IEnumerable<IUserAccount> result = manager.ListUserAccounts("auserid", "foo", null);

                storageProvider.Verify(sp => sp.Find(It.IsAny<string>()), Times.Once());
                storageProvider.Verify(sp => sp.BuildQuery("Username", QueryOperator.EQ, "foo"), Times.Once());
                Assert.Equal(1, result.Count());
                Assert.Equal(account, result.First());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenListUserAccountsWithEmail_ThenReturnsAccountsByEmail()
            {
                var account = new UserAccount();
                storageProvider.Setup(sp => sp.Find(It.IsAny<string>()))
                    .Returns(new[]
                    {
                        account
                    });

                IEnumerable<IUserAccount> result = manager.ListUserAccounts("auserid", null, "foo");

                storageProvider.Verify(sp => sp.Find(It.IsAny<string>()), Times.Once());
                storageProvider.Verify(sp => sp.BuildQuery("Email", QueryOperator.EQ, "foo"), Times.Once());
                Assert.Equal(1, result.Count());
                Assert.Equal(account, result.First());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenListUserAccountsWithUsernameAndEmail_ThenReturnsAccountsByUsername()
            {
                var account = new UserAccount();
                storageProvider.Setup(sp => sp.Find(It.IsAny<string>()))
                    .Returns(new[]
                    {
                        account
                    });

                IEnumerable<IUserAccount> result = manager.ListUserAccounts("auserid", "foo", "bar");

                storageProvider.Verify(sp => sp.Find(It.IsAny<string>()), Times.Once());
                storageProvider.Verify(sp => sp.BuildQuery("Username", QueryOperator.EQ, "foo"), Times.Once());
                Assert.Equal(1, result.Count());
                Assert.Equal(account, result.First());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenListUserAccountsWithEmptyUserNameAndEmptyEmail_ThenReturnsNoAccounts()
            {
                var account = new UserAccount();
                storageProvider.Setup(sp => sp.Find(It.IsAny<string>()))
                    .Returns(new[]
                    {
                        account
                    });

                IEnumerable<IUserAccount> result = manager.ListUserAccounts("auserid", string.Empty, string.Empty);

                storageProvider.Verify(sp => sp.Find(It.IsAny<string>()), Times.Never());
                storageProvider.Verify(
                    sp => sp.BuildQuery(It.IsAny<string>(), It.IsAny<QueryOperator>(), It.IsAny<string>()),
                    Times.Never());
                Assert.Equal(0, result.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateUserAccountWithNullForenames_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    manager.CreateUserAccount("ausername", "ausername", "apasswordhash", null, "asurname", "anemail",
                        "amobilephone", new Address(),
                        "someroles"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateUserAccountWithNullSurname_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    manager.CreateUserAccount("ausername", "ausername", "apasswordhash", "aforename", null, "anemail",
                        "amobilephone", new Address(), "someroles"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateUserAccountWithNullEmail_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    manager.CreateUserAccount("ausername", "ausername", "apasswordhash", "aforename", "asurname", null,
                        "amobilephone", new Address(), "someroles"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateUserAccountWithNullAddress_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    manager.CreateUserAccount("ausername", "ausername", "apasswordhash", "aforename", "asurname",
                        "anemail", "amobilephone", null, "someroles"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateUserAccountWithExistingEmail_ThenThrows()
            {
                var account = new UserAccount();
                storageProvider.Setup(sp => sp.BuildQuery("Email", It.IsAny<QueryOperator>(), It.IsAny<string>()))
                    .Returns("aquery");
                storageProvider.Setup(sp => sp.Find("aquery"))
                    .Returns(new[]
                    {
                        account
                    });

                Assert.Throws<ResourceConflictException>(
                    Resources.UserAccountsManager_UserAccountExistsByEmail.FormatWith("anemail"), () =>
                        manager.CreateUserAccount("ausername", "ausername", "apasswordhash", "aforename", "asurname",
                            "anemail", "amobilephone", new Address(), "someroles"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateUserAccountWithExistingUsername_ThenThrows()
            {
                var account = new UserAccount();
                storageProvider.Setup(
                    sp => sp.BuildQuery("Username", It.IsAny<QueryOperator>(), It.IsAny<string>()))
                    .Returns("aquery");
                storageProvider.Setup(sp => sp.Find("aquery"))
                    .Returns(new[]
                    {
                        account
                    });

                Assert.Throws<ResourceConflictException>(
                    Resources.UserAccountsManager_UserAccountExistsByUsername.FormatWith("ausername"), () =>
                        manager.CreateUserAccount("ausername", "ausername", "apasswordhash", "aforename", "asurname",
                            "anemail", "amobilephone", new Address(), "someroles"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateUserAccountWithNewUsernameAndPassword_ThenCreatesRegisteredAccount()
            {
                storageProvider.Setup(sp => sp.Find(It.IsAny<string>()))
                    .Returns(Enumerable.Empty<IUserAccount>());
                storageProvider.Setup(sp => sp.Add(It.IsAny<IUserAccount>()))
                    .Returns("bar");

                IUserAccount result = manager.CreateUserAccount("auserid", "ausername", "apasswordhash", "aforename",
                    "asurname", "anemail", "amobilephone",
                    new Address
                    {
                        Street1 = "astreet1",
                        Street2 = "astreet2",
                        Town = "atown",
                    }, "someroles");

                storageProvider.Verify(sp => sp.Add(It.Is<IUserAccount>(
                    ua => ua.Username == "ausername"
                          && ua.PasswordHash.HasValue()
                          && ua.Forenames == "aforename"
                          && ua.Surname == "asurname"
                          && ua.Email == "anemail"
                          && ua.MobilePhone == "amobilephone"
                          && ua.Address.Street1 == "astreet1"
                          && ua.Address.Street2 == "astreet2"
                          && ua.Address.Town == "atown"
                          && ua.Roles == "someroles"
                          && ua.IsRegistered)), Times.Once());

                Assert.Equal("bar", result.Id);
                Assert.Equal("ausername", result.Username);
                Assert.Equal("apasswordhash", result.PasswordHash);
                Assert.Equal("aforename", result.Forenames);
                Assert.Equal("asurname", result.Surname);
                Assert.Equal("anemail", result.Email);
                Assert.Equal("amobilephone", result.MobilePhone);
                Assert.Equal("astreet1", result.Address.Street1);
                Assert.Equal("astreet2", result.Address.Street2);
                Assert.Equal("atown", result.Address.Town);
                Assert.Equal("someroles", result.Roles);
                Assert.Equal(true, result.IsRegistered);
            }

            [TestMethod, TestCategory("Unit")]
            public void
                WhenCreateUserAccountWithUsernameAndPasswordAndNullRoles_ThenCreatesRegisteredAccountForNormalUser()
            {
                storageProvider.Setup(sp => sp.Find(It.IsAny<string>()))
                    .Returns(Enumerable.Empty<IUserAccount>());
                storageProvider.Setup(sp => sp.Add(It.IsAny<IUserAccount>()))
                    .Returns("bar");

                IUserAccount result = manager.CreateUserAccount("auserid", "ausername", "apasswordhash",
                    "aforename",
                    "asurname", "anemail", "amobilephone",
                    new Address
                    {
                        Street1 = "astreet1",
                        Street2 = "astreet2",
                        Town = "atown",
                    }, null);

                storageProvider.Verify(sp => sp.Add(It.Is<IUserAccount>(
                    ua => ua.Username == "ausername"
                          && ua.PasswordHash.HasValue()
                          && ua.Forenames == "aforename"
                          && ua.Surname == "asurname"
                          && ua.Email == "anemail"
                          && ua.MobilePhone == "amobilephone"
                          && ua.Address.Street1 == "astreet1"
                          && ua.Address.Street2 == "astreet2"
                          && ua.Address.Town == "atown"
                          && ua.Roles == AuthorizationRoles.NormalUser
                          && ua.IsRegistered)), Times.Once());

                Assert.Equal("bar", result.Id);
                Assert.Equal("ausername", result.Username);
                Assert.Equal("apasswordhash", result.PasswordHash);
                Assert.Equal("aforename", result.Forenames);
                Assert.Equal("asurname", result.Surname);
                Assert.Equal("anemail", result.Email);
                Assert.Equal("amobilephone", result.MobilePhone);
                Assert.Equal("astreet1", result.Address.Street1);
                Assert.Equal("astreet2", result.Address.Street2);
                Assert.Equal("atown", result.Address.Town);
                Assert.Equal(AuthorizationRoles.NormalUser, result.Roles);
                Assert.Equal(true, result.IsRegistered);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateUserAccountWithNullUsernameAndNullPasswordHashAndRoles_ThenThrows()
            {
                storageProvider.Setup(sp => sp.Find(It.IsAny<string>()))
                    .Returns(Enumerable.Empty<IUserAccount>());

                Assert.Throws<RuleViolationException>(Resources.UserAccountsManager_NoRolesForParticipant, () =>
                    manager.CreateUserAccount("auserid", null, null, "aforename",
                        "asurname", "anemail", "amobilephone",
                        new Address
                        {
                            Street1 = "astreet1",
                            Street2 = "astreet2",
                            Town = "atown",
                        }, "roles"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUpdateUserAccountWithNullId_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () =>
                        manager.UpdateUserAccount("auserid", null, "wrongpasswordhash", "apasswordhash", "newforname",
                            "newsurname", "newemail", "newmobilephone", new Address()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUpdateUserAccountWithUnknownId_ThenThrows()
            {
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>())).Returns((IUserAccount) null);

                Assert.Throws<ResourceNotFoundException>(
                    () =>
                        manager.UpdateUserAccount("auserid", "auserid", "wrongpasswordhash", "apasswordhash",
                            "newforname",
                            "newsurname", "newemail", "newmobilephone", new Address()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUpdateUserAccountWithWrongOldPassword_ThenThrows()
            {
                var account = new UserAccount
                {
                    Id = "foo",
                    Username = "ausername",
                    PasswordHash = "apasswordhash",
                    Forenames = "aforename",
                    Surname = "asurname",
                    Email = "anemail",
                };
                var newAccount = new UserAccount();
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>()))
                    .Returns(account);
                storageProvider.Setup(sp => sp.Update(It.IsAny<string>(), It.IsAny<IUserAccount>()))
                    .Returns(newAccount);

                Assert.Throws<RuleViolationException>(() =>
                    manager.UpdateUserAccount("auserid", "bar1", "wrongpasswordhash", "apasswordhash", "newforname",
                        "newsurname", "newemail", "newmobilephone", new Address()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUpdateUserAccountWithNoNewPassword_ThenUpdatesAccount()
            {
                var account = new UserAccount
                {
                    Id = "foo",
                    Username = "ausername",
                    PasswordHash = "apasswordhash",
                    Forenames = "aforename",
                    Surname = "asurname",
                    Email = "anemail",
                    Address = new Address(),
                };
                var newAccount = new UserAccount();
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>()))
                    .Returns(account);
                storageProvider.Setup(sp => sp.Update(It.IsAny<string>(), It.IsAny<IUserAccount>()))
                    .Returns(newAccount);

                IUserAccount result = manager.UpdateUserAccount("auserid", "bar1", "apasswordhash", string.Empty,
                    "newforename",
                    "newsurname", "newemail", "newmobilephone",
                    new Address
                    {
                        Street1 = "newstreet1",
                        Street2 = "newstreet2",
                        Town = "newtown",
                    });

                storageProvider.Verify(sp => sp.Update("bar1", It.Is<IUserAccount>(
                    ua => ua.Username == "ausername"
                          && ua.PasswordHash.HasValue()
                          && ua.Forenames == "newforename"
                          && ua.Surname == "newsurname"
                          && ua.Email == "newemail"
                          && ua.MobilePhone == "newmobilephone"
                          && ua.Address.Street1 == "newstreet1"
                          && ua.Address.Street2 == "newstreet2"
                          && ua.Address.Town == "newtown"
                    )), Times.Once());

                Assert.Equal(newAccount, result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUpdateUserAccountWithKnownId_ThenUpdatesAccount()
            {
                var account = new UserAccount
                {
                    Id = "foo",
                    Username = "ausername",
                    PasswordHash = "apasswordhash",
                    Forenames = "aforename",
                    Surname = "asurname",
                    Email = "anemail",
                    MobilePhone = "amobilephone",
                    Address = new Address(),
                    Roles = "roles"
                };
                var newAccount = new UserAccount();
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>()))
                    .Returns(account);
                storageProvider.Setup(sp => sp.Update(It.IsAny<string>(), It.IsAny<IUserAccount>()))
                    .Returns(newAccount);

                IUserAccount result = manager.UpdateUserAccount("auserid", "bar1", "apasswordhash", "newpasswordhash",
                    "newforename",
                    "newsurname", "newemail", "newmobilephone",
                    new Address
                    {
                        Street1 = "newstreet1",
                        Street2 = "newstreet2",
                        Town = "newtown",
                    });

                storageProvider.Verify(sp => sp.Update("bar1", It.Is<IUserAccount>(
                    ua => ua.Username == "ausername"
                          && ua.PasswordHash == "newpasswordhash"
                          && ua.Forenames == "newforename"
                          && ua.Surname == "newsurname"
                          && ua.Email == "newemail"
                          && ua.MobilePhone == "newmobilephone"
                          && ua.Address.Street1 == "newstreet1"
                          && ua.Address.Street2 == "newstreet2"
                          && ua.Address.Town == "newtown"
                    )), Times.Once());

                Assert.Equal(newAccount, result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUpdateUserAccountWithNullAddress_ThenUpdatesAccount()
            {
                var account = new UserAccount
                {
                    Id = "foo",
                    Username = "ausername",
                    PasswordHash = "apasswordhash",
                    Forenames = "aforename",
                    Surname = "asurname",
                    Email = "anemail",
                    MobilePhone = "amobilephone",
                    Address = new Address
                    {
                        Street1 = "astreet1",
                        Street2 = "astreet2",
                        Town = "atown",
                    },
                    Roles = "roles"
                };
                var newAccount = new UserAccount();
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>()))
                    .Returns(account);
                storageProvider.Setup(sp => sp.Update(It.IsAny<string>(), It.IsAny<IUserAccount>()))
                    .Returns(newAccount);

                IUserAccount result = manager.UpdateUserAccount("auserid", "bar1", "apasswordhash", "newpasswordhash",
                    "newforename",
                    "newsurname", "newemail", "newmobilephone", null);

                storageProvider.Verify(sp => sp.Update("bar1", It.Is<IUserAccount>(
                    ua => ua.Username == "ausername"
                          && ua.PasswordHash == "newpasswordhash"
                          && ua.Forenames == "newforename"
                          && ua.Surname == "newsurname"
                          && ua.Email == "newemail"
                          && ua.MobilePhone == "newmobilephone"
                          && ua.Address.Street1 == "astreet1"
                          && ua.Address.Street2 == "astreet2"
                          && ua.Address.Town == "atown"
                    )), Times.Once());

                Assert.Equal(newAccount, result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUpdateUserAccountWithPartialAddress_ThenUpdatesAccount()
            {
                var account = new UserAccount
                {
                    Id = "foo",
                    Username = "ausername",
                    PasswordHash = "apasswordhash",
                    Forenames = "aforename",
                    Surname = "asurname",
                    Email = "anemail",
                    MobilePhone = "amobilephone",
                    Address = new Address
                    {
                        Street1 = "astreet1",
                        Street2 = "",
                        Town = "atown",
                    },
                    Roles = "roles"
                };
                var newAccount = new UserAccount();
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>()))
                    .Returns(account);
                storageProvider.Setup(sp => sp.Update(It.IsAny<string>(), It.IsAny<IUserAccount>()))
                    .Returns(newAccount);

                IUserAccount result = manager.UpdateUserAccount("auserid", "bar1", "apasswordhash", "newpasswordhash",
                    "newforename",
                    "newsurname", "newemail", "newmobilephone",
                    new Address {Street2 = "newstreet2",});

                storageProvider.Verify(sp => sp.Update("bar1", It.Is<IUserAccount>(
                    ua => ua.Username == "ausername"
                          && ua.PasswordHash == "newpasswordhash"
                          && ua.Forenames == "newforename"
                          && ua.Surname == "newsurname"
                          && ua.Email == "newemail"
                          && ua.MobilePhone == "newmobilephone"
                          && ua.Address.Street1 == "astreet1"
                          && ua.Address.Street2 == "newstreet2"
                          && ua.Address.Town == "atown"
                    )), Times.Once());

                Assert.Equal(newAccount, result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeleteUserAccountWithNullId_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () => manager.DeleteUserAccount("auserid", null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeleteUserAccountWithUnknownId_ThenThrows()
            {
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>())).Returns((IUserAccount) null);

                Assert.Throws<ResourceNotFoundException>(
                    () => manager.DeleteUserAccount("auserid", "foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeleteUserAccountWithKnownId_ThenDeletesAccount()
            {
                var account = new UserAccount();
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>()))
                    .Returns(account);

                manager.DeleteUserAccount("auserid", "foo");

                storageProvider.Verify(sp => sp.Delete("foo"), Times.Once());
            }
        }
    }
}