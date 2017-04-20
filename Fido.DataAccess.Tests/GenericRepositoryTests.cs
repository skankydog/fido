using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.DataAccess;
using Fido.Entities;
using Fido.Entities.UserDetails;

namespace Fido.DataAccess.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class GenericRepositoryTests
    {
        [TestMethod]
        public void insert_entity_flat()
        {
            User UserEntity = new User
            {
                Id = Guid.NewGuid(),
                Password = "WEwew66&&3jhjsD",
                EmailAddress = "john.citizen@skankydog.com",
                Fullname = new Fullname { Firstname = "John", Surname = "Citizen" }
            };

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Repository.CascadeInsert(UserEntity);
                UnitOfWork.Commit();
            }

            Assert.IsNotNull(Helpers.GetUser(UserEntity.Id));
        }

        [TestMethod]
        public void insert_entity_cascade()
        {
            Activity NewActivity01 = new Activity { Id = Guid.NewGuid(), Name = "NewActivity01", Area = "", ReadWrite = "" };
            Activity NewActivity02 = new Activity { Id = Guid.NewGuid(), Name = "NewActivity02", Area = "", ReadWrite = "" };
            Activity NewActivity03 = new Activity { Id = Guid.NewGuid(), Name = "NewActivity03", Area = "", ReadWrite = "" };

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

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                UserRepository.CascadeInsert(NewUser);

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
        public void get_entity_by_id()
        {
            Guid Id = Helpers.InsertCitizen();
            User UserEntity;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                UserEntity = Repository.Get(Id);
            }

            Assert.IsNotNull(UserEntity);
        }

        [TestMethod]
        public void get_entity_by_predicate()
        {
            Guid Id = Helpers.InsertCitizen();
            User UserEntity;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                UserEntity = Repository.Get(e => e.Fullname.Surname == "Citizen");
            }

            Assert.IsNotNull(UserEntity);
        }

        //[TestMethod]
        //public void CanUpdate()
        //{
        //    Guid Id = Helpers.InsertCitizen();
        //    User UserEntity = Helpers.GetUser(Id);

        //    using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
        //    {
        //        IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

        //        UserEntity.Fullname.Surname = "Changed";
        //        Repository.Update(UserEntity);
        //        UnitOfWork.Commit();
        //    }

        //    UserEntity = Helpers.GetUser(Id);
        //    Assert.AreEqual("Changed", UserEntity.Fullname.Surname);
        //}

        [TestMethod]
        public void update_entity()
        {
            User ReadUser;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                ReadUser = UserRepository.GetByExternalEmailAddress("homer.simpson@skankydog.com");
            }

            Assert.IsNotNull(ReadUser);
            Assert.AreEqual(4, ReadUser.Roles.Count());

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var RoleRepository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                var NumberedRoles = RoleRepository.GetAsIEnumerable(r => r.Name.StartsWith("Role"));
                Assert.AreEqual(2, NumberedRoles.Count());

                ReadUser.Roles = (ICollection<Role>)NumberedRoles;
                ReadUser.Fullname.Surname = "Changed";
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
            Assert.AreEqual("Changed", SavedUser.Fullname.Surname);
        }

        [TestMethod]
        public void delete_entity_by_predicate()
        {
            Assert.AreEqual(0, Helpers.CountCitizens());

            Helpers.InsertCitizen();
            Helpers.InsertCitizen();
            Helpers.InsertCitizen();

            Assert.AreEqual(3, Helpers.CountCitizens());

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Repository.Delete(e => e.Fullname.Surname == "Citizen");
                UnitOfWork.Commit();
            }

            Assert.AreEqual(0, Helpers.CountCitizens());
        }

        #region Initialisation
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
        #endregion
    }
}
