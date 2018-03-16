namespace EyeInTheSky.StalkNodes
{
    using System;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("false")]
    class FalseNode : LogicalNode
    {
        public override bool Match(IRecentChange rc)
        {
            if (rc == null)
            {
                throw new ArgumentNullException("rc");
            }
            
            return false;
        }

        public override string ToString()
        {
            return "(false)";
        }
    }
}
