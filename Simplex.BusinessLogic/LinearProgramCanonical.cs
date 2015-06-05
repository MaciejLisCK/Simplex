using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex.BusinessLogic
{
    public class LinearProgramCanonical
    {
        public LinearProgramType Type { get; set; }
        public Expression Objective { get; set; }
        public List<Relation> Limitations { get; set; }

        public LinearProgramCanonical()
        {
            Objective = new Expression();
            Limitations = new List<Relation>();
        }
    }
}
