using System;

namespace EyeInTheSky.StalkNodes
{
    class SummaryStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}