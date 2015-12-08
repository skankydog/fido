using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.DataAccess;
using Fido.Entities;
using Fido.Entities.UserDetails;

namespace Fido.DataAccess.Tests
{
    [TestClass]
    public class UserRepositoryTests
    {
        [TestMethod]
        public void CanGetByExternalCredentials()
        {
            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Assert.IsNotNull(Repository.GetByExternalCredentials("Facebook", "HomerFacebook1"));
                Assert.IsNull(Repository.GetByExternalCredentials("Facebook", "MissingKey"));
            }
        }

        [TestMethod]
        public void CanGetByExternalEmailAddress()
        {
            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Assert.IsNotNull(Repository.GetByExternalEmailAddress("homer.simpson@skankydog.com"));
                Assert.IsNotNull(Repository.GetByExternalEmailAddress("homer@gmail.com"));
                Assert.IsNotNull(Repository.GetByExternalEmailAddress("homie@hotmail.com"));
                Assert.IsNull(Repository.GetByExternalEmailAddress("simpson@skankydog.com"));
                Assert.IsNull(Repository.GetByExternalEmailAddress("non-existant@skankydog.com"));
            }
        }

        [TestMethod]
        public void CanInsertWithRoles()
        {
            Activity NewActivity01 = new Activity { Id = Guid.NewGuid(), Name = "NewActivity01" };
            Activity NewActivity02 = new Activity { Id = Guid.NewGuid(), Name = "NewActivity02" };
            Activity NewActivity03 = new Activity { Id = Guid.NewGuid(), Name = "NewActivity03" };

            Role NewRole1 = new Role { Id = Guid.NewGuid(), Name = "NewRole1" };
            NewRole1.Activities.Add(NewActivity01);
            NewRole1.Activities.Add(NewActivity02);

            Role NewRole2 = new Role { Id = Guid.NewGuid(), Name = "NewRole2" };
            NewRole2.Activities.Add(NewActivity01);
            NewRole2.Activities.Add(NewActivity03);

            User NewUser = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "some password",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "new.user@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "John", Surname = "Citizen" },
                About = "about text"
            };
            NewUser.Roles.Add(NewRole1);
            NewUser.Roles.Add(NewRole2);
            NewUser.ExternalCredentialState = "Enabled";
   //         NewUser.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "HomerFacebook1", EmailAddress = "homer@gmail.com" });
   //         NewUser.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "HomerFacebook2", EmailAddress = "homie@hotmail.com" });

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                UserRepository.Insert(NewUser);

                UnitOfWork.Commit();
            }

            User SavedUser;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                SavedUser = UserRepository.Get(NewUser.Id, "Roles, Roles.Activities");
            }

            Assert.AreEqual(2, SavedUser.Roles.Count());
            Assert.AreEqual(2, SavedUser.Roles.First().Activities.Count());
        }

        [TestMethod]
        public void CanUpdateWithRoles()
        {
            User ReadUser;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                ReadUser = UserRepository.GetByExternalEmailAddress("homer.simpson@skankydog.com");
            }

            Assert.IsNotNull(ReadUser);
            Assert.AreEqual(3, ReadUser.Roles.Count());

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var RoleRepository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                var NumberedRoles = RoleRepository.GetAsIEnumerable(r => r.Name.Contains("Role0"));
                Assert.AreEqual(2, NumberedRoles.Count());

                ReadUser.Roles = (ICollection<Role>)NumberedRoles;
                UserRepository.Update(ReadUser);

                UnitOfWork.Commit();
            }

            User SavedUser;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                SavedUser = UserRepository.Get(ReadUser.Id);
            }

            Assert.AreEqual(2, SavedUser.Roles.Count());
        }

        [TestMethod]
        public void CanInsertWithExternalCredentials()
        {
            User NewUser = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "some password",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "new.user@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "John", Surname = "Citizen" },
                About = "about text"
            };
            NewUser.ExternalCredentialState = "Enabled";
            NewUser.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "HomerFacebook1", EmailAddress = "homer@gmail.com" });
            NewUser.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "HomerFacebook2", EmailAddress = "homie@hotmail.com" });

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                UserRepository.Insert(NewUser);

                UnitOfWork.Commit();
            }

            User SavedUser;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                SavedUser = UserRepository.Get(NewUser.Id);
            }

            Assert.AreEqual(2, SavedUser.ExternalCredentials.Count());
        }

        [TestMethod]
        public void CanUpdateWithExternalCredentials()
        {
            User ReadUser;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                ReadUser = UserRepository.GetByExternalEmailAddress("homer.simpson@skankydog.com");
            }

            Assert.IsNotNull(ReadUser);
            Assert.AreEqual(6, ReadUser.ExternalCredentials.Count());

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                var Marge = UserRepository.GetByExternalEmailAddress("marge.simpson@skankydog.com");
                Assert.AreEqual(3, Marge.ExternalCredentials.Count());

                ReadUser.ExternalCredentials = Marge.ExternalCredentials;
                UserRepository.Update(ReadUser);

                UnitOfWork.Commit();
            }

            User SavedUser;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                SavedUser = UserRepository.Get(ReadUser.Id);
            }

            Assert.AreEqual(3, SavedUser.ExternalCredentials.Count());
        }

        [ClassInitialize]
        public static void Initialise(TestContext Context)
        {
            DataAccess.DataAccessFactory.Boot();
        }

        [TestInitialize]
        public void TestInitialise()
        {
            DataAccess.DataAccessFactory.CreateDataPrimer().Refresh();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DataAccess.DataAccessFactory.CreateDataPrimer().Refresh();
        }
    }
}
