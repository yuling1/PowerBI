using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.PowerBI.DB.Entities.Fact
{
    public class EfficiencyImprovement
    {
        [ColumnName("Save Test hours per test pass")]
        public string SavedTestHours { set; get; }

        [ColumnName("Extra work doing")]
        public string ExtraWork { set; get; }
    }
}
