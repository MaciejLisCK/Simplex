using Simplex.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex
{
    class Program
    {
        static void Main(string[] args)
        {
            var linearProgram = new LinearProgram();

            linearProgram.Type = LinearProgramType.Maximization;

            linearProgram.Objective.Symbols.Add("x1", 50);
            linearProgram.Objective.Symbols.Add("x2", 75);

            var limitation1 = new Relation();
            limitation1.LeftExpression.Symbols.Add("x1", 2);
            limitation1.LeftExpression.Symbols.Add("x2", 1);
            limitation1.Type = RelationType.LowerOrEqual;
            limitation1.RightExpression.Symbols.Add("", 12);
            var limitation2 = new Relation();
            limitation2.LeftExpression.Symbols.Add("x1", 2);
            limitation2.LeftExpression.Symbols.Add("x2", 2);
            limitation2.Type = RelationType.LowerOrEqual;
            limitation2.RightExpression.Symbols.Add("", 20);
            var limitation3 = new Relation();
            limitation3.LeftExpression.Symbols.Add("x1", 1);
            limitation3.LeftExpression.Symbols.Add("x2", -2.5f);
            limitation3.Type = RelationType.GreaterOrEqual;
            limitation3.RightExpression.Symbols.Add("", 0);
            var limitation4 = new Relation();
            limitation4.LeftExpression.Symbols.Add("x1", 1);
            limitation4.Type = RelationType.GreaterOrEqual;
            limitation4.RightExpression.Symbols.Add("", 0);
            var limitation5 = new Relation();
            limitation5.LeftExpression.Symbols.Add("x2", 1);
            limitation5.Type = RelationType.GreaterOrEqual;
            limitation5.RightExpression.Symbols.Add("", 0);
            linearProgram.Limitations.AddRange(new Relation[] { limitation1, limitation2, limitation3, limitation4, limitation5 });

            



        }
    }
}
