using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky.StalkNodes
{
    abstract class SingleChildLogicalNode : LogicalNode
    {
        public StalkNode ChildNode { get; set; }
    }
}
