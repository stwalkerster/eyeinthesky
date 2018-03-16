namespace EyeInTheSky.StalkNodes
{
    using System;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("true")]
    class TrueNode : LogicalNode
    {
        public override bool Match(IRecentChange rc)
        {
            if (rc == null)
            {
                throw new ArgumentNullException("rc");
            }
            
            return true;
        }

        public override string ToString()
        {
            return "(true)";
        }
    }
}
