using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky
{
    class StalkLogItem
    {
        private Stalk stalk;
        private RecentChange rc;

        public StalkLogItem(Stalk s, RecentChange rcitem)
        {
            stalk = s;
            rc = rcitem;
        }

        public Stalk Stalk
        {
            get { return stalk; }
        }

        public RecentChange RecentChangeItem
        {
            get { return rc; }
        }
    
        public XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            
        }

        public override string ToString()
        {
            
        }
    }
}
