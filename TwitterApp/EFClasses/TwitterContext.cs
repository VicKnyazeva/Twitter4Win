using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterApp.EFClasses
{
    public class TwitterContext : DbContext
    {
        public TwitterContext()
            : base("Data Source=DELL-VIC\\SQL2017EXP;Initial Catalog=TwitterAppDb;Integrated Security=True")
        {
        }

        public DbSet<TwitterPost> Posts { get; set; }

        public DbSet<TwitterUser> Users { get; set; }

        public DbSet<TwitterTrend> Trends { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
