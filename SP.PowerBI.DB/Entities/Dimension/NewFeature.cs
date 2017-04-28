using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.PowerBI.DB.Entities.Dimension
{
    public class NewFeature
    {
        [ColumnName("New features")]
        public string FeatureName { get; set; }

        [ColumnName("Handoff date")]
        public DateTime HandoffDate { get; set; }

        [ColumnName("Supplier resources ramp up a new feature testing task within x days")]
        public float RampupDays { get; set; }

        [ColumnName("Test pass")]
        public int TestPassNum { get; set; }

        public string Comments { get; set; }
    }
}
