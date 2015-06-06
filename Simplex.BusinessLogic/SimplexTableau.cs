using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex.BusinessLogic
{
    public class SimplexTableau
    {
        public Dictionary<string, float> XjCj { get; set; }
        public Dictionary<string, float> XiCi { get; set; }
        public float[,] A { get; set; }
        public float[] Zj { get; set; }

        LinearProgramCanonical _linearProgramCanonical;

        public SimplexTableau(LinearProgramCanonical linearProgramCanonical)
        {
            XjCj = new Dictionary<string, float>();
            XiCi = new Dictionary<string, float>();

            _linearProgramCanonical = linearProgramCanonical;

            foreach (var objectiveSymbol in _linearProgramCanonical.Objective.Symbols)
            {
                XjCj.Add(objectiveSymbol.Key, objectiveSymbol.Value);
            }

            var baseVariables = _linearProgramCanonical.Limitations.Select(l => l.LeftExpression.Symbols.Last(s => s.Value == 1).Key);

            foreach (var baseVariable in baseVariables)
            {
                var baseValue = _linearProgramCanonical.Objective.Symbols[baseVariable];

                XiCi.Add(baseVariable, baseValue);
            }

            A = new float[XiCi.Count, XjCj.Count];

            int xi = 0;
            int xj = 0;
            foreach (var xiCi in XiCi)
            {
                foreach (var xjCj in XjCj)
                {
                    A[xi, xj] = _linearProgramCanonical.Limitations[xi].LeftExpression.Symbols[xjCj.Key];

                    xj++;
                }
                xj = 0;
                xi++;
            }

        }

        public SimplexTableau(SimplexTableau previousTableau)
        {
            XjCj = new Dictionary<string, float>();
            XiCi = new Dictionary<string, float>();

        }
    }
}
