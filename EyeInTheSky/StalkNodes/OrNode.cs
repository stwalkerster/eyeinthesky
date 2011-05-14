using System;

namespace EyeInTheSky.StalkNodes
{
    class OrNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return (LeftChildNode.match(rc) || RightChildNode.match(rc));
        }

        #endregion
    }
}