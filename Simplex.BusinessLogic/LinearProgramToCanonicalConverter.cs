using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex.BusinessLogic
{
    public class LinearProgramToCanonicalConverter
    {
        public LinearProgramToCanonicalConverter()
        {

        }

        public LinearProgramCanonical Convert(LinearProgram linearProgram)
        {
            var linearProgramCanonical = new LinearProgramCanonical();

            var clonedLimitations = linearProgram.Limitations.Select(l => l.Clone()).ToList();
            var clonedObjective = linearProgram.Objective.Clone();

            for (int i = 0; i < clonedLimitations.Count; i++)
			{
                var clonedLimitation = clonedLimitations[i];
                var newSymbolName = "_x"+i;
                var newIdentitySymbolName = "_s" + i;

                if(clonedLimitation.Type == RelationType.LowerOrEqual)
                {
                    foreach (var l in clonedLimitations)
	                {
                        if(l == clonedLimitation)
                        {
                            l.LeftExpression.Symbols.Add(newSymbolName, 1);
                        }
                        else
                        {
                            l.LeftExpression.Symbols.Add(newSymbolName, 0);
                        }
	                }
                    clonedObjective.Symbols.Add(newSymbolName, 0);
                }
                else if(clonedLimitation.Type == RelationType.GreaterOrEqual)
                {
                    foreach (var l in clonedLimitations)
	                {
                        if(l == clonedLimitation)
                        {
                            l.LeftExpression.Symbols.Add(newSymbolName, -1);
                            l.LeftExpression.Symbols.Add(newIdentitySymbolName, 1);
                        }
                        else
                        {
                            l.LeftExpression.Symbols.Add(newSymbolName, 0);
                            l.LeftExpression.Symbols.Add(newIdentitySymbolName, 0);
                        }
	                }

                    clonedObjective.Symbols.Add(newSymbolName, 0);

                    if (linearProgram.Type == LinearProgramType.Minimalization)
                        clonedObjective.Symbols.Add(newIdentitySymbolName, (Single.MaxValue / 1000));
                    if (linearProgram.Type == LinearProgramType.Maximization)
                        clonedObjective.Symbols.Add(newIdentitySymbolName, -(Single.MaxValue / 1000));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            clonedLimitations.ForEach(l => l.Type = RelationType.Equality);

            linearProgramCanonical.Limitations = clonedLimitations;
            linearProgramCanonical.Type = linearProgram.Type;
            linearProgramCanonical.Objective = clonedObjective;

            return linearProgramCanonical;
        }

    }
}
