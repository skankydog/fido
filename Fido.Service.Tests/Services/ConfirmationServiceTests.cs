using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.Dtos;
using Fido.Service;
using Fido.Service.Implementation;
using Fido.Service.Exceptions;

namespace Fido.Service.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ConfirmationServiceTests
    {
        [TestMethod]
        public void get_confirmations_by_user_id()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();
            var BartId = UserService.GetByEmailAddress("bart.simpson@skankydog.com").Id;

            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
            var All = ConfirmationService.GetAll(BartId).Count();
            
            Assert.AreEqual(7, All);
        }

        [TestMethod]
        public void get_queued_confirmations_by_user_id()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();
            var BartId = UserService.GetByEmailAddress("bart.simpson@skankydog.com").Id;

            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
            var Queued = ConfirmationService.GetQueued(BartId).Count();

            Assert.AreEqual(2, Queued);
        }

        [TestMethod]
        public void get_sent_confirmations_by_user_id()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();
            var BartId = UserService.GetByEmailAddress("bart.simpson@skankydog.com").Id;

            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
            var Sent = ConfirmationService.GetSent(BartId).Count();

            Assert.AreEqual(1, Sent);
        }

        [TestMethod]
        public void get_received_confirmations_by_user_id()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();
            var BartId = UserService.GetByEmailAddress("bart.simpson@skankydog.com").Id;

            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
            var Received = ConfirmationService.GetReceived(BartId).Count();

            Assert.AreEqual(4, Received);
        }

        [TestMethod]
        public void get_queued_confirmations()
        {
            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
            var Queued = ConfirmationService.GetQueued().Count();

            Assert.AreEqual(2, Queued);
        }

        [TestMethod]
        public void mark_confirmation_as_sent()
        {
            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();

            var QueuedBefore = ConfirmationService.GetQueued();
            Assert.AreEqual(2, QueuedBefore.Count);

            ConfirmationService.MarkAsSent(QueuedBefore.FirstOrDefault().Id);

            var QueuedAfter = ConfirmationService.GetQueued();
            Assert.AreEqual(1, QueuedAfter.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void confirmation_deletion_throws_when_confirmation_already_sent()
        {
            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
            var Created = new Confirmation { Id = Guid.NewGuid(), ConfirmType = "Example", QueuedUTC = DateTime.UtcNow, SentUTC = DateTime.UtcNow, ReceivedUTC = DateTime.UtcNow };
            var CreatedConfirmation = ConfirmationService.Save(Created);

            var Deleted = ConfirmationService.Delete(Created.Id);
        }

        #region Initialisation
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
            Service.ServiceFactory.Boot();
        }
        #endregion
    }
}
