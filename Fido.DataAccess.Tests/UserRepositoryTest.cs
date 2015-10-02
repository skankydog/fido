using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.DataAccess;
using Fido.Entities;

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
