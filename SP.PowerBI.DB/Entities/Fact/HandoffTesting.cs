using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.PowerBI.DB.Entities.Fact
{
    public class HandoffTesting
    {
        [ColumnName("Test pass x")]
        public string TestPass { set; get; }

        [ColumnName("Total test cases")]
        public int TotalCaseNum { set; get; }

        [ColumnName("Excution test cases account")]
        public string ExcutionAccount { set; get; }

        [ColumnName("Required QFE bugs Number")]
        public int RequiredQFEBugNum { set; get; }

        [ColumnName("Validated QFE bugs Number")]
        public int ValidatedQFEBugNum { set; get; }

        [ColumnName("Test pass time")]
        public string TestPassTime { set; get; }

    }
}
