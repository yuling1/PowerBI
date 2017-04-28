using SP.PowerBI.DB.Entities.Dimension;
using SP.PowerBI.DB.Entities.Fact;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.PowerBI.DB.DBContext
{
    public class Basic_DbContext : DbContext
    {
        public DbSet<Bug> Bug { set; get; }
        public DbSet<NewFeature> NewFeature { set; get; }
        public DbSet<Resource> Resource { get; set; }
        public DbSet<Communication> Communication { get; set; }
        public DbSet<EfficiencyImprovement> EfficiencyImprovement { get; set; }
        public DbSet<HandoffTesting> HandoffTesting { get; set; }
        public DbSet<NewFeatureTestTime> NewFeatureTestTime { get; set; }

        public class Basic_DbContextInitializer : CreateDatabaseIfNotExists<Basic_DbContext>
        {
            protected override void Seed(Basic_DbContext context)
            {
                context.Resource.AddRange(new List<Resource>
                {
                    new Resource { Name ="Pengmeng Wu", Alias = "v-pengwu", OnboardDate = DateTime.Now.Date,Title = "SDET1",Skillset = "CSharp",RampUpTime = "1 day", Attrition = false, Comments = "wupengmeng@beyondsoft.com"},
                    new Resource { Name ="Yuling Jiao", Alias = "v-yuljia", OnboardDate = DateTime.Now.Date,Title = "SDET1",Skillset = "CSharp",RampUpTime = "1 day", Attrition = false, Comments = "jiaoyuling@beyondsoft.com"}
                });
                base.Seed(context);
            }
        }
    }
}
