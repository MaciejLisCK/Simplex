using Simplex.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Simplex.Web.Controllers
{
    public class SimplexController : Controller
    {
        // GET: Simplex
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show()
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
            linearProgram.Limitations.AddRange(new Relation[] { limitation1, limitation2, limitation3 });

            var linearProgramToCanonicalConverter = new LinearProgramToCanonicalConverter();

            var canonical = linearProgramToCanonicalConverter.Convert(linearProgram);


            var firstSimplexTableau = new SimplexTableau(canonical);
            var secondSimplexTableau = new SimplexTableau(firstSimplexTableau);
            var thirdSimplexTableau = new SimplexTableau(secondSimplexTableau);

            var tables = new List<SimplexTableau>();
            tables.Add(firstSimplexTableau);
            tables.Add(secondSimplexTableau);
            tables.Add(thirdSimplexTableau);

            return View(tables);
        }
    }
}