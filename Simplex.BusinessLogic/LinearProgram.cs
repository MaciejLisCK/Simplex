using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex.BusinessLogic
{
    public class LinearProgram
    {
        public LinearProgramType Type { get; set; }
        public Expression Objective { get; set; }
        public List<Relation> Limitations { get; set; }

        public LinearProgram()
        {
            Objective = new Expression();
            Limitations = new List<Relation>();
        }
    }
}
