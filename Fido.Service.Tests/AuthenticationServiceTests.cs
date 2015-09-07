using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fido.Core;
using Fido.Dtos;
using Fido.Service;
using Fido.Service.Implementation;
using Fido.Service.Exceptions;
using Fido.DataAccess.Exceptions;

namespace Fido.Service.Tests
{
    [TestClass]
    public class AuthenticationServiceTests
    {
        #region Has Local/External Credentials Tests
        [TestMethod]
        public void CanCheckForLocalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var UserService = ServiceFactory.CreateService<IUserService>();

            var IdWithLocalCredentals = UserService.GetByEmailAddress("homer.simpson@skankydog.com").Id;
            var IdWithoutLocalCredentials = AuthenticationService.CreateByExternalCredentials("Facebook", "<unique>", "santas.little.helper@skankydog.com", "Santa Simpson").Id;

            Assert.IsTrue(AuthenticationService.HasLocalCredentials(IdWithLocalCredentals));
            Assert.IsFalse(AuthenticationService.HasLocalCredentials(IdWithoutLocalCredentials));
        }

        [TestMethod]
        public void CanCheckForExternalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            var IdWithExternalCredentials = AuthenticationService.CreateByExternalCredentials("Facebook", "<unique>", "santas.little.helper@skankydog.com", "Santa Simpson").Id;
            var ConfirmationId = AuthenticationService.InitiateRegistration("charles.burns@skankydog.com", "sdkjKJsdI887487834ewd", "Charles", "Burns");
            var IdWithoutExternalCredentials = AuthenticationService.CompleteRegistration(ConfirmationId).Id;

            Assert.IsTrue(AuthenticationService.HasExternalCredentials(IdWithExternalCredentials));
            Assert.IsFalse(AuthenticationService.HasExternalCredentials(IdWithoutExternalCredentials));
        }

        [TestMethod]
        public void CanLoginByLocalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            User UserDTO = AuthenticationService.LoginByLocalCredentials("marge.simpson@skankydog.com", "Jjdjsj^^77d8sJHJHDjjh");
            Assert.IsNotNull(UserDTO);
        }
        #endregion

        #region Login Tests
        [TestMethod]
        public void CanLoginByExternalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            Assert.IsNull(AuthenticationService.LoginByExternalCredentials("<non-existent provider>", "<non-existent key>"));

            var LinkedUser = AuthenticationService.LoginByExternalCredentials("Facebook", "HomerFacebook1");
            Assert.IsNotNull(LinkedUser);
            Assert.AreEqual("Homer", LinkedUser.Fullname.Firstname);
            Assert.AreEqual("Simpson", LinkedUser.Fullname.Surname);

            Assert.IsNotNull(AuthenticationService.LoginByExternalCredentials("Facebook", "HomerFacebook1"));
        }

        [TestMethod]
        public void CanLoginByExternalEmailAddress()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            // Find User.ExternalCredentials.EmailAddress & link...
            Assert.IsNull(AuthenticationService.LoginByExternalCredentials("<new provider 1>", "<new key 1>"));

            var LinkedUser = AuthenticationService.LoginByExternalEmailAddress("<new provider 1>", "<new key 1>", "homer@gmail.com");
            Assert.IsNotNull(LinkedUser);
            Assert.AreEqual("Homer", LinkedUser.Fullname.Firstname);
            Assert.AreEqual("Simpson", LinkedUser.Fullname.Surname);
            Assert.IsNotNull(AuthenticationService.LoginByExternalCredentials("<new provider 1>", "<new key 1>"));

            // Find User.EmailAddress & link...
            Assert.IsNull(AuthenticationService.LoginByExternalCredentials("<new provider 2>", "<new key 2>"));

            LinkedUser = AuthenticationService.LoginByExternalEmailAddress("<new provider 2>", "<new key 2>", "marge.simpson@skankydog.com");
            Assert.IsNotNull(LinkedUser);
            Assert.AreEqual("Marge", LinkedUser.Fullname.Firstname);
            Assert.AreEqual("Simpson", LinkedUser.Fullname.Surname);
            Assert.IsNotNull(AuthenticationService.LoginByExternalCredentials("<new provider 2>", "<new key 2>"));
        }

        [TestMethod]
        public void CanCreateByExternalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            var LinkedUser = AuthenticationService.CreateByExternalCredentials("<non-existent provder>", "<non-existent key>", "ralph.wiggum@skankydog.com", "Ralph Wiggum");

            Assert.IsNotNull(LinkedUser);
            Assert.AreEqual("Ralph", LinkedUser.Fullname.Firstname);
            Assert.AreEqual("Wiggum", LinkedUser.Fullname.Surname);
            Assert.AreEqual("ralph.wiggum@skankydog.com", LinkedUser.EmailAddress);
            Assert.IsNotNull(AuthenticationService.LoginByExternalCredentials("<non-existent provder>", "<non-existent key>"));
        }
        #endregion

        #region Set Local Credentials Tests
        [TestMethod]
        public void CanSetLocalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var UserService = ServiceFactory.CreateService<IUserService>();

            var UserDTO = AuthenticationService.LoginByExternalCredentials("Facebook", "WaylanFacebook1");
            Assert.AreEqual("None", UserDTO.LocalCredentialState);

            var ConfirmationId = AuthenticationService.InitiateSetLocalCredentials(UserDTO.Id, "waylan.smithers@skankydog.com", "28*8sdjhhjdjssd");
            UserDTO = UserService.Get(UserDTO.Id);
            Assert.AreEqual("Registered", UserDTO.LocalCredentialState);

            AuthenticationService.CompleteSetLocalCredentials(ConfirmationId);
            UserDTO = UserService.Get(UserDTO.Id);
            Assert.AreEqual("Enabled", UserDTO.LocalCredentialState);

            Assert.IsNotNull(AuthenticationService.LoginByLocalCredentials("waylan.smithers@skankydog.com", "28*8sdjhhjdjssd"));
        }

        [TestMethod]
        [ExpectedException(typeof(EmailAddressDuplicationException))]
        public void DuplicateEmailAddressThrowsOnSetLocalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var UserDTO = AuthenticationService.LoginByExternalCredentials("Facebook", "WaylanFacebook1");

            AuthenticationService.InitiateSetLocalCredentials(UserDTO.Id, "bart.simpson@skankydog.com", "skjk(8sdkjkjsd");
        }

        [TestMethod]
        [ExpectedException(typeof(EmailAddressValidationException))]
        public void InvalidEmailAddressThrowsOnSetLocalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var UserDTO = AuthenticationService.LoginByExternalCredentials("Facebook", "WaylanFacebook1");

            AuthenticationService.InitiateSetLocalCredentials(UserDTO.Id, "not.a.valid.email.address", "skjk(8sdkjkjsd");
        }

        [TestMethod]
        [ExpectedException(typeof(PasswordValidationException))]
        public void InvalidPasswordThrowsOnSetLocalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var UserDTO = AuthenticationService.LoginByExternalCredentials("Facebook", "WaylanFacebook1");

            AuthenticationService.InitiateSetLocalCredentials(UserDTO.Id, "waylan.smithers@skankydog.com", "weak");
        }
        #endregion

        #region Registration Tests
        [TestMethod]
        public void CanRegisterForLocalAccount()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var UserService = ServiceFactory.CreateService<IUserService>();

            var ConfirmationId = AuthenticationService.InitiateRegistration("santas.little.helper@skankydog.com", "28*8sdjhhjdjssd", "John", "Citizen");
            var UserDTO = UserService.GetByEmailAddress("santas.little.helper@skankydog.com");
            Assert.AreEqual("Registered", UserDTO.LocalCredentialState);

            AuthenticationService.CompleteRegistration(ConfirmationId);
            UserDTO = UserService.GetByEmailAddress("santas.little.helper@skankydog.com");
            Assert.AreEqual("Enabled", UserDTO.LocalCredentialState);
        }

        [TestMethod]
        [ExpectedException(typeof(EmailAddressDuplicationException))]
        public void DuplicateEmailAddressThrowsOnRegistration()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            AuthenticationService.InitiateRegistration("jamie.danckert@skankydog.com", "skjk(8sdkjkjsd", "Jamie", "Danckert");
            AuthenticationService.InitiateRegistration("jamie.danckert@skankydog.com", "skjk(8sdkjkjsd", "Jamie", "Danckert");
        }

        [TestMethod]
        [ExpectedException(typeof(EmailAddressValidationException))]
        public void InvalidEmailAddressThrowsOnRegistration()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            AuthenticationService.InitiateRegistration("not.a.valid.email.address", "skjk(8sdkjkjsd", "Jamie", "Danckert");
        }

        [TestMethod]
        [ExpectedException(typeof(PasswordValidationException))]
        public void InvalidPasswordThrowsOnRegistration()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            AuthenticationService.InitiateRegistration("jamie.danckert@skankydog.com", "weak", "Jamie", "Danckert");
        }
        #endregion

        #region Forgotten Password Tests
        [TestMethod]
        public void CanPerformForgottenPassword()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var ConfirmationId = AuthenticationService.InitiateForgottenPassword("homer.simpson@skankydog.com");
            AuthenticationService.CompleteForgottenPassword(ConfirmationId, "SDssdsdeEEWe*&*74##");

            var UserService = ServiceFactory.CreateService<IUserService>();
            var UserDTO = UserService.GetByEmailAddress("homer.simpson@skankydog.com");
            Assert.AreEqual("Enabled", UserDTO.LocalCredentialState);
        }

        [TestMethod]
        [ExpectedException(typeof(PasswordValidationException))]
        public void InvalidPasswordThrowsOnForgottenPassword()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var ConfirmationId = AuthenticationService.InitiateForgottenPassword("homer.simpson@skankydog.com");
            AuthenticationService.CompleteForgottenPassword(ConfirmationId, "weak");
        }
        #endregion

        #region Email Address Logic Tests
        [TestMethod]
        public void CanCheckEmailAddressIsFree()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            Assert.IsFalse(AuthenticationService.EmailAddressIsFree("homer.simpson@skankydog.com"));
            Assert.IsTrue(AuthenticationService.EmailAddressIsFree("unknown.person@unknown.com"));
        }

        [TestMethod]
        public void CanValidateEmailAddress()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            Assert.IsFalse(AuthenticationService.EmailAddressPassesValidation(""));
            Assert.IsFalse(AuthenticationService.EmailAddressPassesValidation("leftonly"));
            Assert.IsFalse(AuthenticationService.EmailAddressPassesValidation("rightonly.com"));
            Assert.IsFalse(AuthenticationService.EmailAddressPassesValidation("@rightonly.com"));
            Assert.IsFalse(AuthenticationService.EmailAddressPassesValidation("too@many@domain.com"));
            Assert.IsTrue(AuthenticationService.EmailAddressPassesValidation("ok@domain.com"));
            Assert.IsTrue(AuthenticationService.EmailAddressPassesValidation("ok@domain.com.au"));
            Assert.IsTrue(AuthenticationService.EmailAddressPassesValidation("ok@domain.co.uk"));
            Assert.IsTrue(AuthenticationService.EmailAddressPassesValidation("ok_name@domain.com"));
        }
        #endregion

        #region Password Logic Tests
        [TestMethod]
        public void CanValidatePassword()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            Assert.IsFalse(AuthenticationService.PasswordPassesValidation(""));
            Assert.IsFalse(AuthenticationService.PasswordPassesValidation(" "));
            Assert.IsFalse(AuthenticationService.PasswordPassesValidation("12 45678"));
            Assert.IsFalse(AuthenticationService.PasswordPassesValidation("1"));
            Assert.IsFalse(AuthenticationService.PasswordPassesValidation("123"));
            Assert.IsFalse(AuthenticationService.PasswordPassesValidation("1234567"));
            Assert.IsFalse(AuthenticationService.PasswordPassesValidation("abcdefg"));
            Assert.IsTrue(AuthenticationService.PasswordPassesValidation("d$$%872GHSDj$^&232jash"));
        }

        [TestMethod]
        public void CanReturnPasswordScore()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            Assert.AreEqual(PasswordScore.Invalid, AuthenticationService.GetPasswordScore(""));
            Assert.AreEqual(PasswordScore.Invalid, AuthenticationService.GetPasswordScore(" "));
            Assert.AreEqual(PasswordScore.Invalid, AuthenticationService.GetPasswordScore("12 45678"));

            Assert.AreEqual(PasswordScore.Invalid, AuthenticationService.GetPasswordScore("1"));
            Assert.AreEqual(PasswordScore.Invalid, AuthenticationService.GetPasswordScore("123"));

            Assert.AreEqual(PasswordScore.Invalid, AuthenticationService.GetPasswordScore("1234567"));
            Assert.AreEqual(PasswordScore.Invalid, AuthenticationService.GetPasswordScore("abcdefg"));
            Assert.AreEqual(PasswordScore.VeryWeak, AuthenticationService.GetPasswordScore("aBcDeFg"));
            Assert.AreEqual(PasswordScore.VeryWeak, AuthenticationService.GetPasswordScore("abcd123"));
            Assert.AreEqual(PasswordScore.VeryWeak, AuthenticationService.GetPasswordScore("$%#!123"));
            Assert.AreEqual(PasswordScore.Weak, AuthenticationService.GetPasswordScore("AbCd123"));
            Assert.AreEqual(PasswordScore.Weak, AuthenticationService.GetPasswordScore("AbCd+$#"));
            Assert.AreEqual(PasswordScore.Medium, AuthenticationService.GetPasswordScore("AbCd+$3"));

            Assert.AreEqual(PasswordScore.VeryWeak, AuthenticationService.GetPasswordScore("12345678"));
            Assert.AreEqual(PasswordScore.VeryWeak, AuthenticationService.GetPasswordScore("abcdefgh"));
            Assert.AreEqual(PasswordScore.Weak, AuthenticationService.GetPasswordScore("aBcDeFgH"));
            Assert.AreEqual(PasswordScore.Weak, AuthenticationService.GetPasswordScore("abcd1234"));
            Assert.AreEqual(PasswordScore.Weak, AuthenticationService.GetPasswordScore("$%#!1234"));
            Assert.AreEqual(PasswordScore.Medium, AuthenticationService.GetPasswordScore("AbCd1234"));
            Assert.AreEqual(PasswordScore.Medium, AuthenticationService.GetPasswordScore("AbCd+$#$"));
            Assert.AreEqual(PasswordScore.Strong, AuthenticationService.GetPasswordScore("AbCd+$34"));

            Assert.AreEqual(PasswordScore.Weak, AuthenticationService.GetPasswordScore("123456789012"));
            Assert.AreEqual(PasswordScore.Weak, AuthenticationService.GetPasswordScore("abcdefghijkl"));
            Assert.AreEqual(PasswordScore.Medium, AuthenticationService.GetPasswordScore("aBcDeFgHiJkL"));
            Assert.AreEqual(PasswordScore.Medium, AuthenticationService.GetPasswordScore("abcd12345678"));
            Assert.AreEqual(PasswordScore.Medium, AuthenticationService.GetPasswordScore("$%#!12345678"));
            Assert.AreEqual(PasswordScore.Strong, AuthenticationService.GetPasswordScore("AbCd12345678"));
            Assert.AreEqual(PasswordScore.Strong, AuthenticationService.GetPasswordScore("AbCd+$#$@#%&"));
            Assert.AreEqual(PasswordScore.VeryStrong, AuthenticationService.GetPasswordScore("AbCd+$3456$#"));
        }
        #endregion

        #region Manage External Credentials Tests
        [TestMethod]
        public void CanGetExternalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();
            var UserService = ServiceFactory.CreateService<IUserService>();

            User UserDTO = UserService.GetByEmailAddress("homer.simpson@skankydog.com");
            Assert.AreEqual(6, AuthenticationService.GetExternalCredentials(UserDTO.Id).Count);
        }

        [TestMethod]
        public void CanLinkExternalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            var UserDto = AuthenticationService.CreateByExternalCredentials("<provider1>", "<key1>", "ralph.wiggum@skankydog.com", "Ralph Wiggum");
            AuthenticationService.LinkExternalCredentials(UserDto.Id, "<provider1>", "<key2>", "ralph.wiggum@skankydog.com");

            Assert.AreEqual(2, AuthenticationService.GetExternalCredentials(UserDto.Id).Count);
            Assert.IsNotNull(AuthenticationService.LoginByExternalCredentials("<provider1>", "<key1>"));
            Assert.IsNotNull(AuthenticationService.LoginByExternalCredentials("<provider1>", "<key2>"));
        }

        [TestMethod]
        public void CanUnlinkedExternalCredentials()
        {
            var AuthenticationService = ServiceFactory.CreateService<IAuthenticationService>();

            var UserDto = AuthenticationService.CreateByExternalCredentials("<provider1>", "<key1>", "ralph.wiggum@skankydog.com", "Ralph Wiggum");
            var Credential = AuthenticationService.LinkExternalCredentials(UserDto.Id, "<provider1>", "<key2>", "ralph.wiggum@skankydog.com");
            Assert.AreEqual(2, AuthenticationService.GetExternalCredentials(UserDto.Id).Count);

            AuthenticationService.UnlinkExternalCredentials(UserDto.Id, Credential.Id);
            Assert.AreEqual(1, AuthenticationService.GetExternalCredentials(UserDto.Id).Count);

            Assert.IsNull(AuthenticationService.LoginByExternalCredentials("<provider1>", "<key1>"));
            Assert.IsNotNull(AuthenticationService.LoginByExternalCredentials("<provider1>", "<key2>"));
        }
        #endregion

        #region Initialisation
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
            Service.ServiceFactory.Boot();
        }
        #endregion
    }
}
