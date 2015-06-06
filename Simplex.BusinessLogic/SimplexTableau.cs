using Newtonsoft.Json;
using Simplex.Common;
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
        public Dictionary<string, float> Bi { get; set; }
        public Dictionary<string, float> Zj { get; set; }
        public Dictionary<string, float> Cj_Zj { get; set; }
        public Dictionary<string, float> Bi_Aik { get; set; }

        LinearProgramCanonical _linearProgramCanonical;


        public SimplexTableau(SimplexTableau previousTableau)
        {
            XjCj = new Dictionary<string, float>();
            XiCi = new Dictionary<string, float>();

            XjCj = ObjectCopier.CloneJson(previousTableau.XjCj);
            XiCi = ObjectCopier.CloneJson(previousTableau.XiCi);

            XiCi.Remove(previousTableau.LeavingVariable);
            XiCi.Add(previousTableau.EnteringVariable, previousTableau.XjCj[previousTableau.EnteringVariable]);

            var previousEnteringLeavinCoordinates = new Tuple<string, string>(previousTableau.LeavingVariable, previousTableau.EnteringVariable);
            var previousEnteringLeavingValue = previousTableau.A[previousEnteringLeavinCoordinates];
            Bi = new Dictionary<string, float>();
            A = new Dictionary<Tuple<string, string>, float>();
            foreach (var xj in XjCj.Keys)
            {
                var currentACoordinates = new Tuple<string, string>(previousTableau.EnteringVariable, xj);
                var previousACoordinates = new Tuple<string, string>(previousTableau.LeavingVariable, xj);

                var key = currentACoordinates;
                var value = previousTableau.A[previousACoordinates] / previousEnteringLeavingValue;

                A.Add(key, value);
            }
            Bi.Add(previousTableau.EnteringVariable, previousTableau.Bi[previousTableau.LeavingVariable] / previousEnteringLeavingValue);
            
            var remainingXi = previousTableau.XiCi.Where(xici => xici.Key != previousTableau.LeavingVariable).Select(xici => xici.Key);
            foreach (var xi in remainingXi)
            {
                foreach (var xj in XjCj.Keys)
                {
                    var previousAijCoordinates = new Tuple<string, string>(xi, xj);
                    var previousAij = previousTableau.A[previousAijCoordinates];

                    var previousLeavingAiCoordinates = new Tuple<string, string>(previousTableau.LeavingVariable, xj);
                    var previousLeavingAiValue = previousTableau.A[previousLeavingAiCoordinates];

                    var currentEnteredAijCoordinates = new Tuple<string, string>(xi, previousTableau.EnteringVariable);
                    var currentEnteredAijValue = previousTableau.A[currentEnteredAijCoordinates];  

                    var currentAijCoordinates = new Tuple<string, string>(xi, xj);
                    var currentAijValue = previousAij - (previousLeavingAiValue * currentEnteredAijValue);
                    A.Add(currentAijCoordinates, currentAijValue);


                }
            }

        }


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
            FillBi();
            FillZj();
            FillCj_Zj();
            FillBi_Aik(EnteringVariable);
        }

        public string EnteringVariable
        {
            get
            {
                if (_linearProgramCanonical.Type == LinearProgramType.Minimalization)
                {
                    var lowestCj_Zj = Cj_Zj.OrderBy(cj_xj => cj_xj.Value).First().Key;

                    return lowestCj_Zj;
                }

                if (_linearProgramCanonical.Type == LinearProgramType.Maximization)
                {
                    var highestCj_Zj = Cj_Zj.OrderBy(cj_xj => cj_xj.Value).Last().Key;

                    return highestCj_Zj;
                }

                throw new NotImplementedException();
            }
        }

        private string LeavingVariable
        {
            get
            {
                var lowestBi_aik = Bi_Aik.OrderBy(bi_aik => bi_aik.Value).First();

                return lowestBi_aik.Key;
            }
        }

        private void FillBi_Aik(string enteringVariable)
        {
            Bi_Aik = new Dictionary<string,float>();

            foreach (var xi in XiCi.Keys)
	        {
                var bi = Bi[xi];
                var currentCoordinates = new Tuple<string, string>(xi, enteringVariable);
                var aik = A[currentCoordinates];

                if(aik != 0)
                {
                    var key = xi;
                    var value = bi/aik;

                    var isValuePositive = value >= 0;
                    if (isValuePositive)
                        Bi_Aik.Add(key, value);
                }
	        }
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

        private void FillBi()
        {
            Bi = new Dictionary<string, float>();
            foreach (var xi in XiCi.Keys)
            {
                var currentLimitationFunction = _linearProgramCanonical.Limitations.Single(l => l.LeftExpression.Symbols[xi] == 1);

                var key = xi;
                var value = currentLimitationFunction.RightExpression.Symbols[String.Empty];

                Bi.Add(key, value);
            }
        }

        private bool IsOptimal()
        {
            if(_linearProgramCanonical.Type == LinearProgramType.Minimalization)
            {
                var isAllCj_ZjPositive = Cj_Zj.All(cj_zj => cj_zj.Value >= 0);

                return isAllCj_ZjPositive;
            }
            else
            {
                var isAllCj_ZjNegative = Cj_Zj.All(cj_zj => cj_zj.Value <= 0);

                return isAllCj_ZjNegative;
            }
        }

    }




}
