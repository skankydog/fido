using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fido.Core;
using Fido.DataAccess;
using Fido.Entities;
using Fido.Entities.UserDetails;

namespace Fido.DataAccess.Implementation
{
    internal class Context : DbContext, IDisposable
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Confirmation> Confirmations { get; set; }

        public Context(string DbName)
            : base(DbName)
        {
            Log.Debug("Creating Context");
        }

        public new void Dispose()
        {
            Log.Debug("Disposing Context");
            base.Dispose();
        }

        public void FixEfProviderServicesProblem()
        {
            // The Entity Framework provider type 'System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer'
            // for the 'System.Data.SqlClient' ADO.NET provider could not be loaded. Make sure the provider assembly is available
            // to the running application. See http://go.microsoft.com/fwlink/?LinkId=260882 for more information.
            var Instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        protected override void OnModelCreating(DbModelBuilder DbModelBuilder)
        {
            // http://stackoverflow.com/questions/10614575/entity-framework-code-first-unique-column/23055838#23055838

            using (new FunctionLogger(Log))
            {
                // NOTE: I want To set the first three properties for ALL entities - a generic/reflection-based function perhaps?
                DbModelBuilder.Entity<Configuration>().HasKey(e => e.Id);
                DbModelBuilder.Entity<Configuration>().Property(e => e.CreatedUtc).IsRequired();
                DbModelBuilder.Entity<Configuration>().Property(e => e.RowVersion).IsRowVersion();

                DbModelBuilder.Entity<User>().HasKey(e => e.Id);
                DbModelBuilder.Entity<User>().Property(e => e.CreatedUtc).IsRequired();
                DbModelBuilder.Entity<User>().Property(e => e.RowVersion).IsRowVersion();
                DbModelBuilder.Entity<User>().Property(e => e.EmailAddress).HasMaxLength(150);
                DbModelBuilder.Entity<User>()
                    .HasOptional(e => e.UserImage).WithRequired(e => e.User).WillCascadeOnDelete(true);

                DbModelBuilder.Entity<UserImage>().HasKey(e => e.Id);

                DbModelBuilder.Entity<Role>().HasKey(e => e.Id);
                DbModelBuilder.Entity<Role>().Property(e => e.CreatedUtc).IsRequired();
                DbModelBuilder.Entity<Role>().Property(e => e.RowVersion).IsRowVersion();
                DbModelBuilder.Entity<Role>().Property(e => e.Name).HasMaxLength(150).IsRequired();

                DbModelBuilder.Entity<Activity>().HasKey(e => e.Id);
                DbModelBuilder.Entity<Activity>().Property(e => e.CreatedUtc).IsRequired();
                DbModelBuilder.Entity<Activity>().Property(e => e.RowVersion).IsRowVersion();
                DbModelBuilder.Entity<Activity>().Property(e => e.Area).HasMaxLength(150).IsRequired();
                DbModelBuilder.Entity<Activity>().Property(e => e.Name).HasMaxLength(150).IsRequired();
                DbModelBuilder.Entity<Activity>().Property(e => e.ReadWrite).HasMaxLength(50).IsRequired();

                DbModelBuilder.Entity<ExternalCredential>().HasKey(e => new { e.Id, e.UserId }); // Composite primary key
                DbModelBuilder.Entity<ExternalCredential>().Property(e => e.CreatedUtc).IsRequired();
                DbModelBuilder.Entity<ExternalCredential>().Property(e => e.RowVersion).IsRowVersion();
                DbModelBuilder.Entity<ExternalCredential>().Property(e => e.LoginProvider).HasMaxLength(150).IsRequired();
                DbModelBuilder.Entity<ExternalCredential>().Property(e => e.ProviderKey).HasMaxLength(150).IsRequired();
                DbModelBuilder.Entity<ExternalCredential>().Property(e => e.EmailAddress).HasMaxLength(150);
                DbModelBuilder.Entity<ExternalCredential>().HasRequired(e => e.User).WithMany(e => e.ExternalCredentials).HasForeignKey(e => e.UserId).WillCascadeOnDelete(true);

                DbModelBuilder.Entity<Confirmation>().HasKey(e => e.Id);
                DbModelBuilder.Entity<Confirmation>().Property(e => e.CreatedUtc).IsRequired();
                DbModelBuilder.Entity<Confirmation>().Property(e => e.RowVersion).IsRowVersion();
                DbModelBuilder.Entity<Confirmation>().Property(e => e.ConfirmType).HasMaxLength(50).IsRequired();

                base.OnModelCreating(DbModelBuilder);
            }
        }

        public string GetTableName<TENTITY>()
            where TENTITY : Entity
        {
            ObjectContext ObjectContext = ((IObjectContextAdapter)this).ObjectContext;

            string SQL = ObjectContext.CreateObjectSet<TENTITY>().ToTraceString();
            Regex Regex = new Regex("FROM (?<table>.*) AS");
            Match Match = Regex.Match(SQL);

            return Match.Groups["table"].Value;
        }
    }
}
