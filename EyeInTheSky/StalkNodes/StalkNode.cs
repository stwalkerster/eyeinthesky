using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky.StalkNodes
{
    abstract class StalkNode
    {
        abstract public bool match(RecentChange rc);
    }
}
