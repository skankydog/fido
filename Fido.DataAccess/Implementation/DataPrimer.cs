using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Entities;
using Fido.Entities.UserDetails;

namespace Fido.DataAccess.Implementation
{
    public class DataPrimer : IDataPrimer
    {
        public void Refresh()
        {
            Delete();

            Activity Activity01 = new Activity { Id = Guid.NewGuid(), Action = "Action 1", Name = "Controller/Model 1", Area = "Namespace 1" };
            Activity Activity02 = new Activity { Id = Guid.NewGuid(), Action = "Action 2", Name = "Controller/Model 2", Area = "Namespace 1" };
            Activity Activity03 = new Activity { Id = Guid.NewGuid(), Action = "Action 2", Name = "Controller/Model 3", Area = "Namespace 1" };
            Activity Activity04 = new Activity { Id = Guid.NewGuid(), Action = "Action 1", Name = "Controller/Model 4", Area = "Namespace 2" };
            Activity Activity05 = new Activity { Id = Guid.NewGuid(), Action = "Action 1", Name = "Controller/Model 5", Area = "Namespace 3" };
            Activity Activity06 = new Activity { Id = Guid.NewGuid(), Action = "Action 1", Name = "Controller/Model 6", Area = "Namespace 4" };

            Role AllActivitiesRole = new Role { Id = Guid.NewGuid(), Name = "All Activities" };
            AllActivitiesRole.Activities.Add(Activity01);
            AllActivitiesRole.Activities.Add(Activity02);
            AllActivitiesRole.Activities.Add(Activity03);
            AllActivitiesRole.Activities.Add(Activity04);
            AllActivitiesRole.Activities.Add(Activity05);
            AllActivitiesRole.Activities.Add(Activity06);

            Role Role2 = new Role { Id = Guid.NewGuid(), Name = "Role 2" };
            Role2.Activities.Add(Activity01);
            Role2.Activities.Add(Activity02);
            Role2.Activities.Add(Activity03);
            Role2.Activities.Add(Activity04);

            Role Role3 = new Role { Id = Guid.NewGuid(), Name = "Role 3" };
            Role3.Activities.Add(Activity03);
            Role3.Activities.Add(Activity04);
            Role3.Activities.Add(Activity05);
            Role3.Activities.Add(Activity06);

            Role FidoAdministrator = new Role { Id = Guid.NewGuid(), Name = "Fido Administrator" };

            User HomerSimpson = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "hello",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "homer.simpson@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "Homer", Surname = "Simpson" },
                About = string.Concat("This is really just a sentence or two about the user that this account applies ",
                          "to - in this case it is Homer Simpson. I would assume Homer would enter this ",
                          "information into his own User Profile to explain a little about himself.\n\n",
                          "At this stage, there is not really any information about Homer.")
            };
            HomerSimpson.Roles.Add(AllActivitiesRole);
            HomerSimpson.Roles.Add(Role2);
            HomerSimpson.Roles.Add(Role3);
            HomerSimpson.Roles.Add(FidoAdministrator);
            HomerSimpson.ExternalCredentialState = "Enabled";
            HomerSimpson.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "HomerFacebook1", EmailAddress = "homer@gmail.com" });
            HomerSimpson.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "HomerFacebook2", EmailAddress = "homie@hotmail.com" });
            HomerSimpson.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "HomerFacebook3", EmailAddress = "hjs@hotmail.com" });
            HomerSimpson.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "HomerFacebook3", EmailAddress = "simpsons@skankydog.com" });
            HomerSimpson.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "LinkedIn", ProviderKey = "HomerLinkedIn1", EmailAddress = "homer.simpson@skankydog.com" });
            HomerSimpson.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Google", ProviderKey = "HomerGoogle1", EmailAddress = "homer.simpson@skankydog.com" });

            User MargeSimpson = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "hello",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "marge.simpson@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "Marge", Surname = "Simpson" },
                About = "This is something about Marge"
            };
            MargeSimpson.Roles.Add(Role2);
            MargeSimpson.Roles.Add(Role3);
            MargeSimpson.ExternalCredentialState = "Enabled";
            MargeSimpson.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "MargeFacebook1", EmailAddress = "marge.simpson@skankydog.com" });
            MargeSimpson.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "MargeFacebook3", EmailAddress = "simpsons@skankydog.com" });
            MargeSimpson.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Google", ProviderKey = "MargeGoogle1", EmailAddress = "marge@gmail.com" });

            User BartSimpson = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "hello",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "bart.simpson@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "Bart", Surname = "Simpson" },
                About = "This is something about Bart"
            };
            BartSimpson.Roles.Add(Role2);
            BartSimpson.ExternalCredentialState = "Enabled";
            BartSimpson.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "BartFacebook1", EmailAddress = "bart.simpson@skankydog.com" });

            User WaylanSmithers = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "None",
                Fullname = new Fullname { Firstname = "Waylan", Surname = "Smithers" },
                About = "This is something about Waylan"
            };
            // Waylan Smithers has no roles
            WaylanSmithers.ExternalCredentialState = "Enabled";
            WaylanSmithers.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "WaylanFacebook1", EmailAddress = "waylan@hotmail.com" });

            User MontyBurns = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "Jjdjsj^^77d8sJHJHDjjh",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "bernie@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "Monty", Surname = "Burns" },
                About = "Rich$$$"
            };
            // Monty Burns has no roles
            MontyBurns.ExternalCredentialState = "Enabled";
            MontyBurns.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "MontyFacebook1", EmailAddress = "monty@skankydog.com" });

            User Blinky = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "Jjdjsj^^77d8sJHJHDjjh",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "blinky@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "Blinky", Surname = "Fish" },
                About = "A three-eyed fish"
            };
            // Blinky has no roles
            Blinky.ExternalCredentialState = "Enabled";
            Blinky.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "BlinkyFacebook1", EmailAddress = "blinky@skankydog.com" });

            User KentBrockman = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "Jjdjsj^^77d8sJHJHDjjh",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "kentbrockman@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "Kent", Surname = "Brockman" },
                About = "New reader extrodinaire"
            };
            // Kent has no roles
            KentBrockman.ExternalCredentialState = "Enabled";
            KentBrockman.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "BrockyFacebook1", EmailAddress = "kent@skankydog.com" });

            User BumblebeeMan = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "Jjdjsj^^77d8sJHJHDjjh",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "bbm@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "Bumblebee", Surname = "Man" },
                About = "Hasta La Vista!"
            };
            // Bumbebee Man has no roles
            BumblebeeMan.ExternalCredentialState = "Enabled";
            BumblebeeMan.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "BumbleBeeManFacebook1", EmailAddress = "bbm@skankydog.com" });

            User CrazyCatLady = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "Jjdjsj^^77d8sJHJHDjjh",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "ccl@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "Crazy", Surname = "Cat Lady" },
                About = "Meow!!"
            };
            // Crazy Cat Lady has no roles
            CrazyCatLady.ExternalCredentialState = "Enabled";
            CrazyCatLady.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "CrazyCatLadyFacebook1", EmailAddress = "ccl@skankydog.com" });

            User DiscoStu = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "Jjdjsj^^77d8sJHJHDjjh",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "stu@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "Disco", Surname = "Stu" },
                About = "Stu is cool"
            };
            // Disco Stu has no roles
            DiscoStu.ExternalCredentialState = "Enabled";
            DiscoStu.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "DiscoStuFacebook1", EmailAddress = "stu@skankydog.com" });

            User FatTony = new User
            {
                Id = Guid.NewGuid(),
                LocalCredentialState = "Enabled",
                Password = "Jjdjsj^^77d8sJHJHDjjh",
                PasswordLastChangeUtc = DateTime.UtcNow,
                EmailAddress = "tony@skankydog.com",
                EmailAddressLastChangeUtc = DateTime.UtcNow,
                Fullname = new Fullname { Firstname = "Fat", Surname = "Tony" },
                About = "I can make you an offer you can't refuse"
            };
            // Fat Tony has no roles
            FatTony.ExternalCredentialState = "Enabled";
            FatTony.ExternalCredentials.Add(new ExternalCredential { Id = Guid.NewGuid(), LoginProvider = "Facebook", ProviderKey = "TonyFacebook1", EmailAddress = "fatso@skankydog.com" });

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var ConfigurationRepository = DataAccessFactory.CreateRepository<IConfigurationRepository>(UnitOfWork);
                ConfigurationRepository.CascadeInsert(
                    new Configuration { PasswordChangePolicyDays = 30 });

                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                UserRepository.CascadeInsert(HomerSimpson);
                UserRepository.CascadeInsert(MargeSimpson);
                UserRepository.CascadeInsert(BartSimpson);
                UserRepository.CascadeInsert(WaylanSmithers);
                UserRepository.CascadeInsert(MontyBurns);
                UserRepository.CascadeInsert(Blinky);
                UserRepository.CascadeInsert(KentBrockman);
                UserRepository.CascadeInsert(BumblebeeMan);
                UserRepository.CascadeInsert(CrazyCatLady);
                UserRepository.CascadeInsert(DiscoStu);
                UserRepository.CascadeInsert(FatTony);

                UnitOfWork.Commit();
            }
        }

        public void Delete()
        {
            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var UserRepository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);
                UserRepository.Delete(e => e.Id != null);

                var RoleRepository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);
                RoleRepository.Delete(e => e.Id != null);

                var ActivityRepository = DataAccessFactory.CreateRepository<IActivityRepository>(UnitOfWork);
                ActivityRepository.Delete(e => e.Id != null);

                var ConfirmationRepository = DataAccessFactory.CreateRepository<IConfirmationRepository>(UnitOfWork);
                ConfirmationRepository.Delete(e => e.Id != null);

                var ExternalCredentialRepository = DataAccessFactory.CreateRepository<IExternalCredentialRepository>(UnitOfWork);
                ExternalCredentialRepository.Delete(e => e.Id != null);

                var ConfigurationRepository = DataAccessFactory.CreateRepository<IConfigurationRepository>(UnitOfWork);
                ConfigurationRepository.Delete(e => e.Id != null);

                UnitOfWork.Commit();
            }
        }
    }
}
