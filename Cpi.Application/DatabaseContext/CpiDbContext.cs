using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Migrations;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Data.Entity.Core.Metadata.Edm;
using System;
using System.Data.Entity.Core.Objects;
using Cpi.Application.Helpers;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Cpi.Application.DatabaseContext
{
    public class CpiDbContext : DbContext
    {
        private static Dictionary<Type, EntitySetBase> _mappingCache = new Dictionary<Type, EntitySetBase>();

        public CpiDbContext()
            : base("name=DefaultConnection")
        {
            Database.SetInitializer<CpiDbContext>(null);
            //Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // remove pluralization (ex. prevent client to clients)
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // no cascade deleting: all entries should be soft deleted or many to many references should be manually removed
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Configurations.Add(new CallMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new CallCommodityMap());
            modelBuilder.Configurations.Add(new InvoiceMap());
            modelBuilder.Configurations.Add(new InvoiceCommodityMap());
            modelBuilder.Configurations.Add(new ExpenseMap());

            modelBuilder.Configurations.Add(new LookUpUserRoleMap());
            modelBuilder.Configurations.Add(new LookUpUserOccupationMap());
            modelBuilder.Configurations.Add(new LookUpCallStatusMap());
            modelBuilder.Configurations.Add(new LookUpInvoiceStatusMap());
            modelBuilder.Configurations.Add(new LookUpCommodityMap());

        }


        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries()
                      .Where(p => p.State == EntityState.Deleted))
                SoftDelete(entry);

            return base.SaveChanges();
        }

        private void SoftDelete(DbEntityEntry entry)
        {
            Type entryEntityType = entry.Entity.GetType();

            string tableName = GetTableName(entryEntityType);
            string primaryKeyName = GetPrimaryKeyName(entryEntityType);

            string sql =
                string.Format(
                    "UPDATE {0} SET Deleted = 1, ModifiedById = {1}, ModifiedDate = GETDATE() WHERE {2} = @id",
                        tableName, UserHelper.GetUserId(), primaryKeyName);

            Database.ExecuteSqlCommand(
                sql,
                new SqlParameter("@id", entry.OriginalValues[primaryKeyName]));

            // prevent hard delete            
            entry.State = EntityState.Detached;
        }

        private string GetTableName(Type type)
        {
            EntitySetBase es = GetEntitySet(type);

            return string.Format("[{0}].[{1}]",
                es.MetadataProperties["Schema"].Value,
                es.MetadataProperties["Table"].Value);
        }

        private string GetPrimaryKeyName(Type type)
        {
            EntitySetBase es = GetEntitySet(type);

            return es.ElementType.KeyMembers[0].Name;
        }

        private EntitySetBase GetEntitySet(Type type)
        {
            if (!_mappingCache.ContainsKey(type))
            {
                ObjectContext octx = ((IObjectContextAdapter)this).ObjectContext;

                string typeName = ObjectContext.GetObjectType(type).Name;

                var es = octx.MetadataWorkspace
                                .GetItemCollection(DataSpace.SSpace)
                                .GetItems<EntityContainer>()
                                .SelectMany(c => c.BaseEntitySets
                                                .Where(e => e.Name == typeName))
                                .FirstOrDefault();

                if (es == null)
                    throw new ArgumentException("Entity type not found in GetTableName", typeName);

                _mappingCache.Add(type, es);
            }

            return _mappingCache[type];
        }

        internal sealed class MigrationConfiguration : DbMigrationsConfiguration<CpiDbContext>
        {
            public MigrationConfiguration()
            {
                AutomaticMigrationsEnabled = true;
                AutomaticMigrationDataLossAllowed = true;
            }
        }
    }
}
