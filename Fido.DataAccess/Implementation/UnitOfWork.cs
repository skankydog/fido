using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Fido.DataAccess;
using Fido.DataAccess.Exceptions;
using Fido.Core;
using Fido.Core.Exceptions;

namespace Fido.DataAccess.Implementation
{
    internal class UnitOfWork : IUnitOfWork
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Context Context { get; private set; }

        public UnitOfWork()
        {
            using (new FunctionLogger(Log))
            {
                Log.Debug("Creating UnitOfWork");
                string DbName = Assembly.GetExecutingAssembly().GetName().Name.Split('.').First();
                this.Context = new Context(DbName);
            }
        }

        public void Dispose()
        {
            using (new FunctionLogger(Log))
            {
                Log.Debug("Disposing UnitOfWork");
                this.Context.Dispose();
            }
        }

        public void Commit()
        {
            using (new FunctionLogger(Log))
            {
                try
                {
                    this.Context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException Ex)
                {
                    Log.WarnFormat("Exception: {0}", Ex);
                    throw new ConcurrencyException("At least one of the entities was modified between when it was read and when this write was attempted. Please reprocess.", Ex);
                }
                catch (DbUpdateException Ex)
                {
                    Log.WarnFormat("Exception: {0}", Ex);

                    // This could be caught because the write to the database is violating a unique field
                    // contraint - even if we check before we write, another user could insert a record to
                    // the database before our write. As this is an issue a user can handle, we need to raise
                    // as a unique exception so it can be caught and acted upon (tell the user and ask them
                    // to change the value and try again).
                    if (Ex.InnerException == null ||
                        Ex.InnerException.InnerException == null ||
                        Ex.InnerException.InnerException.GetType() != typeof(SqlException))
                    {
                        throw;
                    }

                    SqlException SqlEx = Ex.InnerException.InnerException as SqlException;

                    if (SqlEx.Number != 2601)
                    {
                        throw;
                    }

                    throw new UniqueFieldException(SqlEx.Message, Ex);
                }
            }
        }

        public void Rollback()
        {
            using (new FunctionLogger(Log))
            {
                Dispose();
            }
        }
    }
}
