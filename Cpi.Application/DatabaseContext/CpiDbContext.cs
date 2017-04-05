using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Migrations;
using Cpi.Application.DataModels;
using Cpi.Application.DataModels.LookUp;

namespace Cpi.Application.DatabaseContext
{
    public class CpiDbContext : DbContext
    {
        public CpiDbContext()
            : base("name=DefaultConnection")
        {
            Database.SetInitializer<CpiDbContext>(null);
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
            modelBuilder.Configurations.Add(new AddressMap());

            modelBuilder.Configurations.Add(new LookUpCommodityMap());
            modelBuilder.Configurations.Add(new LookUpUserRoleMap());
            modelBuilder.Configurations.Add(new LookUpUserOccupationMap());
            modelBuilder.Configurations.Add(new LookUpCallStatusMap());
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
