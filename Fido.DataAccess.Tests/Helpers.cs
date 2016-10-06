using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fido.Entities;
using Fido.Entities.UserDetails;

namespace Fido.DataAccess.Tests
{
    public static class Helpers
    {
        public static User GetUser(Guid Id)
        {
            User UserEntity;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                UserEntity = Repository.Get(Id);
            }

            return UserEntity;
        }

        public static Role GetRole(Guid Id)
        {
            Role RoleEntity;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                var Repository = DataAccessFactory.CreateRepository<IRoleRepository>(UnitOfWork);

                RoleEntity = Repository.Get(Id);
            }

            return RoleEntity;
        }

        public static Guid InsertCitizen()
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

        public static int CountCitizens()
        {
            int Count;

            using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
            {
                IUserRepository Repository = DataAccessFactory.CreateRepository<IUserRepository>(UnitOfWork);

                Count = Repository.GetAsIEnumerable(e => e.Fullname.Surname == "Citizen").ToList().Count;
            }

            return Count;
        }
    }
}
