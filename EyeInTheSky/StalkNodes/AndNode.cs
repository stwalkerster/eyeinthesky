using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky.StalkNodes
{
    class AndNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return (LeftChildNode.match(rc) && RightChildNode.match(rc));
        }

        #endregion
    }
}
