﻿using System;
using System.Collections.Generic;
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
    [TestClass]
    public class ConfirmationServiceTests
    {
        [TestMethod]
        public void CanGetAllConfirmationsForUser()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();
            var BartId = UserService.GetByEmailAddress("bart.simpson@skankydog.com").Id;

            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
            var All = ConfirmationService.GetAll(BartId).Count();
            
            Assert.AreEqual(7, All);
        }

        [TestMethod]
        public void CanGetQueuedConfirmationsForUser()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();
            var BartId = UserService.GetByEmailAddress("bart.simpson@skankydog.com").Id;

            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
            var Queued = ConfirmationService.GetQueued(BartId).Count();

            Assert.AreEqual(2, Queued);
        }

        [TestMethod]
        public void CanGetSentConfirmationsForUser()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();
            var BartId = UserService.GetByEmailAddress("bart.simpson@skankydog.com").Id;

            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
            var Sent = ConfirmationService.GetSent(BartId).Count();

            Assert.AreEqual(1, Sent);
        }

        [TestMethod]
        public void CanGetReceivedConfirmationsForUser()
        {
            var UserService = ServiceFactory.CreateService<IUserService>();
            var BartId = UserService.GetByEmailAddress("bart.simpson@skankydog.com").Id;

            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
            var Received = ConfirmationService.GetReceived(BartId).Count();

            Assert.AreEqual(4, Received);
        }

        [TestMethod]
        public void CanGetAllQueuedConfirmations()
        {
            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();
            var Queued = ConfirmationService.GetQueued().Count();

            Assert.AreEqual(2, Queued);
        }

        [TestMethod]
        public void CanMarkConfirmationAsSent()
        {
            var ConfirmationService = ServiceFactory.CreateService<IConfirmationService>();

            var QueuedBefore = ConfirmationService.GetQueued();
            Assert.AreEqual(2, QueuedBefore.Count);

            ConfirmationService.MarkAsSent(QueuedBefore.FirstOrDefault().Id);

            var QueuedAfter = ConfirmationService.GetQueued();
            Assert.AreEqual(1, QueuedAfter.Count);
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