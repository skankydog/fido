using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.Dtos;
using Fido.Service;
using Fido.Service.Exceptions;

namespace Fido.Service.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CRUDServiceTests
    {
        [TestMethod]
        public void get_all_dtos()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();
            var AllDtos = Service.GetAll();

            Assert.AreNotEqual(0, AllDtos.Count);
        }

        [TestMethod]
        public void count_all_dtos()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();
            var AllDtoCount = Service.CountAll();

            Assert.AreEqual(11, AllDtoCount);
        }

        [TestMethod]
        public void get_dto_by_id()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();
            User UserDto = Service.GetByEmailAddress("bart.simpson@skankydog.com");

            Assert.IsNotNull(Service.Get(UserDto.Id));
        }

        [TestMethod]
        public void insert_dto()
        {
            Activity ActivityDto = new Activity
            {
                ReadWrite = "Action 1",
                Name = "BrandNewEntity",
                Area = "Nowhere"
            };

            IActivityService Service = ServiceFactory.CreateService<IActivityService>();
            Service.Save(ActivityDto);

            Assert.IsNotNull(Service.Get(ActivityDto.Id));
        }

        [TestMethod]
        public void update_dto()
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
        public void delete_dto_by_id()
        {
            IUserService Service = ServiceFactory.CreateService<IUserService>();

            User UserDto = Service.GetByEmailAddress("homer.simpson@skankydog.com");
            Service.Delete(UserDto.Id);

            Assert.IsNull(Service.GetByEmailAddress("homer.simpson@skankydog.com"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void update_dto_throws_on_concurrency_issues()
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
