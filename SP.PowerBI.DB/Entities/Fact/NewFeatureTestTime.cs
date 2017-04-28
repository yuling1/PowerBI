using SP.PowerBI.DB.Entities.Dimension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.PowerBI.DB.Entities.Fact
{
    public class NewFeatureTestTime
    {
        [ColumnName("One Test pass time")]
        public string TestTime { get; set; }

        public NewFeature NewFeatureInfo { set; get; }
    }
}
