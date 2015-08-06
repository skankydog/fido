using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core.Bootstrapper;
using Fido.Core.Exceptions;
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
            User UserEntity = new User
            {
                Id = Guid.NewGuid(),
                //CreatedUtc = DateTime.UtcNow,
                Password = "Jjdjsj^^77d8sJHJHDjjh",
                EmailAddress = "john.citizen@skankydog.com",
                Fullname = new Fullname { Firstname = "John", Surname = "Citizen" }
            };

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Repository.Insert(UserEntity);
                UnitOfWork.Rollback();
            }

            Assert.IsFalse(Exists(UserEntity.Id));
        }

        [TestMethod]
        public void CanCommit()
        {
            User UserEntity = new User
            {
                Id = Guid.NewGuid(),
                CreatedUtc = DateTime.UtcNow,
                Password = "Jjdjsj^^77d8sJHJHDjjh",
                EmailAddress = "john.citizen@skankydog.com",
                Fullname = new Fullname { Firstname = "John", Surname = "Citizen" }
            };

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Repository.Insert(UserEntity);
                UnitOfWork.Commit();
            }

            Assert.IsTrue(Exists(UserEntity.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(ConcurrencyException))]
        public void ChecksForOptimisticConcurrency()
        {
            using (IUnitOfWork OuterUnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var OuterRepository = DataAccessFactory.CreateRepository<IUserRepository>(OuterUnitOfWork);
                User OuterEntity = OuterRepository.Get(e => e.EmailAddress == "homer.simpson@skankydog.com");

                using (IUnitOfWork InnerUnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var InnerRepository = DataAccessFactory.CreateRepository<IUserRepository>(InnerUnitOfWork);
                    User InnerEntity = InnerRepository.Get(e => e.EmailAddress == "homer.simpson@skankydog.com");

                    InnerEntity.Fullname.Firstname = "John";
                    InnerRepository.Update(InnerEntity);
                    InnerUnitOfWork.Commit();
                }

                OuterRepository.Update(OuterEntity);
                OuterUnitOfWork.Commit();
            }
        }

        private bool Exists(Guid Id)
        {
            User UserEntity;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                UserEntity = Repository.Get(Id);
            }

            return UserEntity != null;
        }

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
            DataAccess.DataAccessFactory.CreateDataPrimer().Delete();
        }
    }
}
