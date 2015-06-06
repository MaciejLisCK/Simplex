using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex.BusinessLogic
{
    public class Expression
    {
        public Dictionary<string, float> Symbols { get; set; }

        public Expression()
        {
            Symbols = new Dictionary<string, float>();
        }

        public Expression Clone()
        {
            var newExpression = new Expression();
            
            newExpression.Symbols = new Dictionary<string, float>(Symbols);

            return newExpression;
        }
    }
}
