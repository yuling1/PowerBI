using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.PowerBI.DB.Entities.Dimension
{
    public class Resource
    {
        public string Name { set; get; }

        public string Alias { set; get; }

        [ColumnName("Onboard Date")]
        public DateTime OnboardDate { set; get; }

        public string Title { set; get; }

        public string Skillset { set; get; }
        
        [ColumnName("Ramp up time")]
        public string RampUpTime { set; get; }

        public bool Attrition { set; get; }

        public string Comments { set; get; }
    }
}
