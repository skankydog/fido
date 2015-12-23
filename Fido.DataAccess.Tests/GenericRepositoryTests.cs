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
    public class GenericRepositoryTests
    {
        [TestMethod]
        public void CanInsert()
        {
            User UserEntity = new User
            {
                Id = Guid.NewGuid(),
                //CreatedUtc = DateTime.UtcNow,
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

            Assert.IsTrue(Exists(UserEntity.Id));
        }

        [TestMethod]
        public void CanGetById()
        {
            Guid Id = Insert();
            User UserEntity;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                UserEntity = Repository.Get(Id);
            }

            Assert.IsNotNull(UserEntity);
        }

        [TestMethod]
        public void CanGetByPredicate()
        {
            Guid Id = Insert();
            User UserEntity;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                UserEntity = Repository.Get(e => e.Fullname.Surname == "Citizen");
            }

            Assert.IsNotNull(UserEntity);
        }

        [TestMethod]
        public void CanUpdate()
        {
            Guid Id = Insert();
            User UserEntity = Get(Id);

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                UserEntity.Fullname.Surname = "Changed";
                Repository.Update(UserEntity);
                UnitOfWork.Commit();
            }

            UserEntity = Get(Id);
            Assert.AreEqual("Changed", UserEntity.Fullname.Surname);
        }

        [TestMethod]
        public void CanDeleteByPredicate()
        {
            Assert.AreEqual(0, Count());

            Insert();
            Insert();
            Insert();

            Assert.AreEqual(3, Count());

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Repository.Delete(e => e.Fullname.Surname == "Citizen");
                UnitOfWork.Commit();
            }

            Assert.AreEqual(0, Count());
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

        private Guid Insert()
        {
            User UserEntity = new User
            {
                Id = Guid.NewGuid(),
                EmailAddress = string.Concat(Guid.NewGuid(), "@skankydog.com"),
                Password = "WEwew66&&3jhjsD",
                Fullname = new Fullname { Firstname = "John", Surname = "Citizen" }
            };

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Repository.CascadeInsert(UserEntity);
                UnitOfWork.Commit();
            }

            return UserEntity.Id;
        }

        private User Get(Guid Id)
        {
            User UserEntity;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                UserEntity = Repository.Get(Id);
            }

            return UserEntity;
        }

        private int Count()
        {
            int Count;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Count = Repository.GetAsIEnumerable(e => e.Fullname.Surname == "Citizen").ToList().Count;
            }

            return Count;
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
