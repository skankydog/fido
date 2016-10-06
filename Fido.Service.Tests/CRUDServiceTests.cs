using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.Dtos;
using Fido.Service;
using Fido.Service.Exceptions;

namespace Fido.Service.Tests
{
    [TestClass]
    public class CRUDServiceTests
    {
        [TestMethod]
        public void CanGetAll()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();
            var AllDtos = Service.GetAll();

            Assert.AreNotEqual(0, AllDtos.Count);
        }

        [TestMethod]
        public void CanCountAll()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();
            var AllDtoCount = Service.CountAll();

            Assert.AreEqual(11, AllDtoCount);
        }

        [TestMethod]
        public void CanGetEntityById()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();
            User UserDto = Service.GetByEmailAddress("bart.simpson@skankydog.com");

            Assert.IsNotNull(Service.Get(UserDto.Id));
        }

        [TestMethod]
        public void CanInsertEntity()
        {
            Activity ActivityDto = new Activity
            {
                Action = "Action 1",
                Name = "BrandNewEntity",
                Area = "Nowhere"
            };

            IActivityService Service = ServiceFactory.CreateService<IActivityService>();
            Service.Save(ActivityDto);

            Assert.IsNotNull(Service.Get(ActivityDto.Id));
        }

        [TestMethod]
        public void CanUpdateEntity()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();

            User UserDto = Service.GetByEmailAddress("homer.simpson@skankydog.com");
            Assert.AreEqual("Homer", UserDto.Fullname.Firstname);

            UserDto.Fullname.Firstname = "Test";
            Service.Save(UserDto);

            UserDto = Service.GetByEmailAddress("homer.simpson@skankydog.com");
            Assert.AreEqual("Test", UserDto.Fullname.Firstname);
        }

        [TestMethod]
        public void CanDeleteEntity()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();

            User UserDto = Service.GetByEmailAddress("homer.simpson@skankydog.com");
            Service.Delete(UserDto.Id);

            Assert.IsNull(Service.GetByEmailAddress("homer.simpson@skankydog.com"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ChecksForOptimisticConcurrency()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();
            
            // someone reads the entity
            var OuterUserDto = Service.GetByEmailAddress("homer.simpson@skankydog.com");

            // someone else reads the entity, edits a field and saves the record - the
            // database now contains a different RowVersion value than OuterUserDTO
            var InnerUserDto = Service.GetByEmailAddress("homer.simpson@skankydog.com");
            InnerUserDto.Fullname.Firstname = "New";
            Service.Save(InnerUserDto);

            // OuterUserDTO is now saved - the framework should see that the RowVersion
            // field is different and throw an exception
            Service.Save(OuterUserDto);
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

        [ClassInitialize]
        public static void Initialise(TestContext Context)
        {
            //DataAccess.DataAccessFactory.CreateBootstrapperEngine().Bootstrap();
            Service.ServiceFactory.Boot();
        }
    }
}
