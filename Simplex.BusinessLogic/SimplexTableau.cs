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
        public Dictionary<Tuple<string,string>, float> A { get; set; }
        public Dictionary<string, float> Zj { get; set; }
        public Dictionary<string, float> Cj_Zj { get; set; }

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

            A = new Dictionary<Tuple<string, string>, float>();

            foreach (var Xi in XiCi.Keys)
            {
                foreach (var Xj in XjCj.Keys)
                {
                    var key = new Tuple<string, string>(Xi,Xj);
                    var currentLimitation = _linearProgramCanonical.Limitations.Single(l => l.LeftExpression.Symbols[Xi] == 1);
                    var value = currentLimitation.LeftExpression.Symbols[Xj];

                    A.Add(key, value); 
                }
            }

            FillZj();
            FillCj_Zj();
        }

        public SimplexTableau(SimplexTableau previousTableau)
        {
            XjCj = new Dictionary<string, float>();
            XiCi = new Dictionary<string, float>();

        }
        private void FillZj()
        {
            Zj = new Dictionary<string, float>();

            foreach (var xj in XjCj.Keys)
            {
                float zj = 0;

                foreach (var xici in XiCi)
	            {
                    var xi = xici.Key;
                    var ci = xici.Value;

                    var cellCordinates = new Tuple<string, string>(xi,xj);

                    zj += ci * A[cellCordinates];
	            }

                Zj.Add(xj, zj);
            }
        }

        private void FillCj_Zj()
        {
            Cj_Zj = new Dictionary<string, float>();

            foreach (var xj in XjCj.Keys)
            {
                var cj = XjCj[xj];
                var zj = Zj[xj];

                var key = xj;
                var value = cj - zj;

                Cj_Zj.Add(key, value);
            }
        }
    }
}
