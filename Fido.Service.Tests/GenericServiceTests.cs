using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.Core.Exceptions;
//using Fido.DataAccess.Exceptions;
using Fido.Dtos;
using Fido.Service;
using Fido.Service.Exceptions;

namespace Fido.Service.Tests
{
    [TestClass]
    public class GenericServiceTests
    {
        [TestMethod]
        public void CanGetAll()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();
            var AllDTOs = Service.GetAll();

            Assert.AreNotEqual(0, AllDTOs.Count);
        }

        [TestMethod]
        public void CanGetEntityById()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();
            User UserDTO = Service.GetByEmailAddress("bart.simpson@skankydog.com");

            Assert.IsNotNull(Service.Get(UserDTO.Id));
        }

        [TestMethod]
        public void CanInsertEntity()
        {
            Activity ActivityDTO = new Activity
            {
                Name = "BrandNewEntity"
            };

            IActivityService Service = ServiceFactory.CreateService<IActivityService>();
            Service.Save(ActivityDTO);

            Assert.IsNotNull(Service.Get(ActivityDTO.Id));
        }

        [TestMethod]
        public void CanUpdateEntity()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();

            User UserDTO = Service.GetByEmailAddress("homer.simpson@skankydog.com");
            Assert.AreEqual("Homer", UserDTO.Fullname.Firstname);

            UserDTO.Fullname.Firstname = "Test";
            Service.Save(UserDTO);

            UserDTO = Service.GetByEmailAddress("homer.simpson@skankydog.com");
            Assert.AreEqual("Test", UserDTO.Fullname.Firstname);
        }

        [TestMethod]
        public void CanDeleteEntity()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();

            User UserDTO = Service.GetByEmailAddress("homer.simpson@skankydog.com");
            Service.Delete(UserDTO.Id);

            Assert.IsNull(Service.GetByEmailAddress("homer.simpson@skankydog.com"));
        }

        [TestMethod]
        [ExpectedException(typeof(ConcurrencyException))]
        public void ChecksForOptimisticConcurrency()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();
            
            // Something reads the entity
            var OuterUserDTO = Service.GetByEmailAddress("homer.simpson@skankydog.com");

            // Something else reads the entity and edits a field and saves the record. The
            // database now contains a different RowVersion value than OuterUserDTO
            var InnerUserDTO = Service.GetByEmailAddress("homer.simpson@skankydog.com");
            InnerUserDTO.Fullname.Firstname = "New";
            Service.Save(InnerUserDTO);

            // OuterUserDTO is now saved. The framework should see that the RowVersion field
            // is different and throw an exception
            Service.Save(OuterUserDTO);
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

        [ClassInitialize]
        public static void Initialise(TestContext Context)
        {
            //DataAccess.DataAccessFactory.CreateBootstrapperEngine().Bootstrap();
            Service.ServiceFactory.Boot();
        }
    }
}
