using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex.BusinessLogic
{
    public class Relation
    {
        public Expression LeftExpression { get; set; }
        public Expression RightExpression { get; set; }
        public RelationType Type { get; set; }

        public Relation()
        {
            LeftExpression = new Expression();
            RightExpression = new Expression();
        }

        public Relation Clone()
        {
            var newRelation = new Relation();

            newRelation.LeftExpression = this.LeftExpression.Clone();
            newRelation.RightExpression = this.RightExpression.Clone();
            newRelation.Type = this.Type;

            return newRelation;
        }
    }
}
