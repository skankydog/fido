using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fido.Service.Exceptions;
using Fido.Core;
using Fido.Dtos;
using Fido.DataAccess;

namespace Fido.Service.Implementation
{
    internal class ConfigurationService : IConfigurationService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Configuration Get()
        {
            using (new FunctionLogger(Log))
            {
                using (IUnitOfWork UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<IConfigurationRepository>(UnitOfWork);
                    var ConfigurationEntity = Repository.Get(e => e.Id != null);

                    var ConfigurationDTO = Mapper.Map<Entities.Configuration, Configuration>(ConfigurationEntity);

                    return ConfigurationDTO;
                }
            }
        }

        public void Set(Configuration Configuration)
        {
            using (new FunctionLogger(Log))
            {
                using (var UnitOfWork = DataAccessFactory.CreateUnitOfWork())
                {
                    var Repository = DataAccessFactory.CreateRepository<DataAccess.IConfigurationRepository>(UnitOfWork);
                    var ConfigurationEntity = Repository.Get(e => e.Id != null);

                    ConfigurationEntity = Mapper.Map<Configuration, Entities.Configuration>(Configuration, ConfigurationEntity);
                    Repository.Update(ConfigurationEntity);

                    UnitOfWork.Commit();
                }
            }
        }
    }
}
