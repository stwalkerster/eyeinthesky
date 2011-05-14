using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EyeInTheSky.StalkNodes
{
    abstract class LeafNode : StalkNode
    {
        protected Regex expression;

        public void setMatchExpression(string regex)
        {
            expression = new Regex(regex);
        }
    }
}
