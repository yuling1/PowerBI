using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.PowerBI.DB.Entities.Dimension
{
    public class Bug
    {
        [ColumnName("Bug ID")]
        public int BugID { get; set; }

        public string Area { get; set; }

        public string Feature { get; set; }

        public int Severity { get; set; }

        [ColumnName("Open date")]
        public DateTime OpenDate { set; get; }

        [ColumnName("Change date")]
        public DateTime ChangeDate { set; get; }

        public string Status { set; get; }

        public string Resolution { set; get; }

        [ColumnName("Open by")]
        public string OpenBy { set; get; }

        [ColumnName("Found from which work item")]
        public string WorkItemFrom { set; get; }

        [ColumnName("Test pass X")]
        public string TestPassX { set; get; }

        [ColumnName("From test cases")]
        public bool FromCases { set; get; }

        [ColumnName("Valid or not")]
        public bool Valid { set; get; }

        [ColumnName("Bug validation time after build ready")]
        public string BugValidationTime { set; get; }

        [ColumnName("Mis-opened bug")]
        public bool MisOpendBug { set; get; }

        [ColumnName("Miss bug from Test cases execution")]
        public bool MissBugFromCases { set; get; }

        public string Comments { set; get; }
    }
}
