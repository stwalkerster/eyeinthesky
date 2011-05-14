using System;

namespace EyeInTheSky.StalkNodes
{
    class NotNode : SingleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return !this.ChildNode.match(rc);
        }

        #endregion
    }
}