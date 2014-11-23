using System;
using System.Linq;
using System.Net;
using Common.Security;
using Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Properties;
using Services.DataContracts;
using Services.MessageContracts;
using Testing.Common;

namespace Services.IntTests
{
    partial class UserAccountsSpec
    {
        partial class GivenTheUserAccountsService
        {
            private static int counter = 1;

            protected override CreateUserAccount MakeCreateUserAccount()
            {
                counter++;
                return new CreateUserAccount
                {
                    Forenames = "aforename",
                    Surname = "asurname",
                    Username = "ausername" + counter,
                    MobilePhone = string.Empty,
                    Address = new Address
                    {
                        Town = "atown",
                    },
                    PasswordHash = PasswordHasher.CreateHash("apasswordhash"),
                    Email = counter + "foo@foo.com",
                };
            }

            [TestMethod, TestCategory("Integration")]
            public override void WhenGetListUserAccountsWithMultipleUserAccounts_ThenReturnsAllResources()
            {
                var account1 = MakeCreateUserAccount();
                account1.Username = "ausername1";
                var account2 = MakeCreateUserAccount();
                account2.Username = "ausername2";
                var account3 = MakeCreateUserAccount();
                account3.Username = "ausername3";

                var created1 = Client.Post(account1).UserAccount.Id;
                var created2 = Client.Post(account2).UserAccount.Id;
                var created3 = Client.Post(account3).UserAccount.Id;

                var result1 = Client.Get(new ListUserAccounts { Username = "ausername1" });
                var result2 = Client.Get(new ListUserAccounts { Username = "ausername2" });
                var result3 = Client.Get(new ListUserAccounts { Username = "ausername3" });

                Assert.Equal(1, result1.UserAccounts.Count());
                Assert.Equal(1, result2.UserAccounts.Count());
                Assert.Equal(1, result3.UserAccounts.Count());
                Assert.NotNull(result1.UserAccounts.FirstOrDefault(x => x.Id == created1));
                Assert.NotNull(result2.UserAccounts.FirstOrDefault(x => x.Id == created2));
                Assert.NotNull(result3.UserAccounts.FirstOrDefault(x => x.Id == created3));

                var httpResponse = Client.Get<HttpWebResponse>(this.MakeListUserAccounts());

                Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            }

            [TestMethod, TestCategory("Integration")]
            public override void WhenGetGetUserAccount_ThenReturnsResource()
            {
                var created = this.CreateNewUserAccount();
                var fetched = Client.Get(new GetUserAccount { Id = created });

                Assert.Equal(created, fetched.UserAccount.Id);

                var httpResponse = Client.Get<HttpWebResponse>(new GetUserAccount { Id = created });

                Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenCreateUserAccountWithExistingEmail_ThenThrows()
            {
                var newAccount = MakeCreateUserAccount();
                newAccount.Email = "foo@foo.com";

                Client.Post(newAccount);

                Assert.Throws<ResourceConflictException>(HttpErrorCode.FromHttpStatusCode(HttpStatusCode.Conflict),
                    () => Client.Post(newAccount));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenCreateUserAccountWithExistingUsername_ThenThrows()
            {
                var newAccount = MakeCreateUserAccount();

                Client.Post(newAccount);

                Assert.Throws<ResourceConflictException>(HttpErrorCode.FromHttpStatusCode(HttpStatusCode.Conflict),
                    () => Client.Post(newAccount));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenCreateUserAccountWithoutEmail_ThenThrows()
            {
                var newAccount = MakeCreateUserAccount();
                newAccount.Email = string.Empty;

                Assert.Throws<InvalidOperationException>(Resources.CreateUserAccountValidator_InvalidEmail,
                    () => Client.Post(newAccount));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenCreateUserAccountWithoutTown_ThenThrows()
            {
                var newAccount = MakeCreateUserAccount();
                newAccount.Address.Town = string.Empty;

                Assert.Throws<InvalidOperationException>(Resources.AddressValidator_InvalidAddressTown,
                    () => Client.Post(newAccount));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenCreateUserAccountWithCredentials_ThenCreatesNormalUserAccount()
            {
                var newAccount = MakeCreateUserAccount();
                newAccount.Email = "foo@foo.com";

                var result = Client.Post(newAccount);

                Assert.NotNull(result.UserAccount.Id);
                Assert.Equal(newAccount.Forenames, result.UserAccount.Forenames);
                Assert.Equal(newAccount.Surname, result.UserAccount.Surname);
                Assert.Equal(newAccount.Email, result.UserAccount.Email);
                Assert.Equal(newAccount.MobilePhone, result.UserAccount.MobilePhone);
                Assert.Equal(newAccount.Address.Street1, result.UserAccount.Address.Street1);
                Assert.Equal(newAccount.Address.Street2, result.UserAccount.Address.Street2);
                Assert.Equal(newAccount.Address.Town, result.UserAccount.Address.Town);
                Assert.Equal(newAccount.Username, result.UserAccount.Username);
                Assert.Equal(newAccount.PasswordHash, result.UserAccount.PasswordHash);
                Assert.Equal(AuthorizationRoles.NormalUser, result.UserAccount.Roles);
                Assert.True(result.UserAccount.IsRegistered);
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenCreateUserAccountWithoutCredentials_ThenCreatesParticipantUserAccount()
            {
                var newAccount = MakeCreateUserAccount();
                newAccount.Email = "foo@foo.com";
                newAccount.Username = null;
                newAccount.PasswordHash = null;

                var result = Client.Post(newAccount);

                Assert.NotNull(result.UserAccount.Id);
                Assert.Equal(newAccount.Forenames, result.UserAccount.Forenames);
                Assert.Equal(newAccount.Surname, result.UserAccount.Surname);
                Assert.Equal(newAccount.Email, result.UserAccount.Email);
                Assert.Equal(newAccount.MobilePhone, result.UserAccount.MobilePhone);
                Assert.Equal(newAccount.Address.Street1, result.UserAccount.Address.Street1);
                Assert.Equal(newAccount.Address.Street2, result.UserAccount.Address.Street2);
                Assert.Equal(newAccount.Address.Town, result.UserAccount.Address.Town);
                Assert.Equal(newAccount.Email, result.UserAccount.Username);
                Assert.Equal(null, result.UserAccount.PasswordHash);
                Assert.Equal(AuthorizationRoles.ParticipantUser, result.UserAccount.Roles);
                Assert.False(result.UserAccount.IsRegistered);
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenUpdateUserAccountWithNewPasswordAndWrongOldPassword_ThenThrows()
            {
                var created = this.CreateNewUserAccount();

                var newAccount = new UpdateUserAccount
                {
                    Id = created,
                    OldPasswordHash = PasswordHasher.CreateHash("wrongpassword"),
                    NewPasswordHash = PasswordHasher.CreateHash("anewpassword"),
                };

                Assert.Throws<InvalidOperationException>(HttpErrorCode.FromHttpStatusCode(HttpStatusCode.BadRequest),
                    () => Client.Put(newAccount));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenUpdateUserAccountWithNewPassword_ThenUpdates()
            {
                var created = this.CreateNewUserAccount();

                var account = Client.Get(new GetUserAccount { Id = created });

                var newPasswordHash = PasswordHasher.CreateHash("anewpassword");
                var newAccount = new UpdateUserAccount
                {
                    Id = created,
                    OldPasswordHash = account.UserAccount.PasswordHash,
                    NewPasswordHash = newPasswordHash,
                };

                var result = Client.Put(newAccount);

                Assert.Equal(newPasswordHash, result.UserAccount.PasswordHash);
            }
        }
    }
}
