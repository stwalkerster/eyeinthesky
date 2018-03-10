using System.Text.RegularExpressions;

namespace EyeInTheSky.StalkNodes
{
    abstract class LeafNode : StalkNode
    {
        protected Regex Expression;

        public void SetMatchExpression(string regex)
        {
            this.Expression = new Regex(regex);
        }
    }
}
