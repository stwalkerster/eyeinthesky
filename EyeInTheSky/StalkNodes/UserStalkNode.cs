using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky.StalkNodes
{
    class UserStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
