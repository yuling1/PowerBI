using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.PowerBI.DB.Entities.Fact
{
    public class Communication
    {
        [ColumnName("Average Microsoft Fulltime employee request response within x hours")]
        public string AverageResponseTime { set; get; }

        [ColumnName("Communication to Microsoft with test Plan, test owner, test schedule")]
        public bool WithPlan { set; get; }

        [ColumnName("Technical issues were resolved by date agreed upon by Microsoft and supplier")]
        public bool IssuesResolvedByDate { set; get; }
    }
}
