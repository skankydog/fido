using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core.Bootstrapper;
using Fido.DataAccess.Exceptions;
using Fido.Entities;
using Fido.Entities.UserDetails;

namespace Fido.DataAccess.Tests
{
    [TestClass]
    public class UnitOfWorkTests
    {
        [TestMethod]
        public void CanRollback()
        {
            // Make sure an insert of a new entity call be rolled back...
            User UserEntity = new User
            {
                Id = Guid.NewGuid(),
                Password = "Jjdjsj^^77d8sJHJHDjjh",
                EmailAddress = "john.citizen@skankydog.com",
                Fullname = new Fullname { Firstname = "John", Surname = "Citizen" }
            };

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Repository.CascadeInsert(UserEntity);
                UnitOfWork.Rollback();
            }

            Assert.IsNull(Helpers.GetUser(UserEntity.Id));

            // Make sure a change to an existing entity can be rolled back...
            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                var User = Repository.GetByExternalEmailAddress("homer.simpson@skankydog.com");

                User.Fullname.Firstname = "John";

                Repository.Update(User);
                UnitOfWork.Rollback();
            }

            User RetrievedUser;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                RetrievedUser = Repository.GetByExternalEmailAddress("homer.simpson@skankydog.com");
            }

            Assert.AreEqual("Homer", RetrievedUser.Fullname.Firstname);
        }

        [TestMethod]
        public void CanCommit()
        {
            User UserEntity = new User {
                Id = Guid.NewGuid(),
                CreatedUtc = DateTime.UtcNow,
                Password = "Jjdjsj^^77d8sJHJHDjjh",
                EmailAddress = "john.citizen@skankydog.com",
                Fullname = new Fullname { Firstname = "John", Surname = "Citizen" },
                Roles = new List<Role>() {
                    new Role {
                        Id = Guid.NewGuid(),
                        Name = "SomeRole",
                        Activities = new List<Activity>() {
                            new Activity {
                                Id = Guid.NewGuid(),
                                Name = "SomeActivity",
                                ReadWrite ="Action 2",
                                Area = "SomeArea"
                            }
                        }
                    }
                }
            };

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Repository.CascadeInsert(UserEntity);
                UnitOfWork.Commit();
            }

            User RetrievedUser;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                RetrievedUser = Repository.Get(UserEntity.Id, "Roles, Roles.Activities");
            }

            Assert.IsNotNull(RetrievedUser);
            Assert.AreEqual(1, RetrievedUser.Roles.Count());
            Assert.AreEqual(1, RetrievedUser.Roles.First().Activities.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public void ChecksForOptimisticConcurrency()
        {
            using (IUnitOfWork OuterUnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var OuterRepository = DataAccessFactory.CreateRepository<IActivityRepository>(OuterUnitOfWork);
                var OuterEntity = OuterRepository.Get(e => e.Name == "Controller/Model 1");

                using (IUnitOfWork InnerUnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var InnerRepository = DataAccessFactory.CreateRepository<IActivityRepository>(InnerUnitOfWork);
                    var InnerEntity = InnerRepository.Get(e => e.Name == "Controller/Model 1");

                    InnerEntity.Name = "New Name";
                    InnerRepository.Update(InnerEntity);
                    InnerUnitOfWork.Commit();
                }

                OuterRepository.Update(OuterEntity);
                OuterUnitOfWork.Commit();
            }
        }

        #region Initialisation
        [ClassInitialize]
        public static void ClassInitialise(TestContext Context)
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
