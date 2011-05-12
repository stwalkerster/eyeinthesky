using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace EyeInTheSky
{
    class StalkList
    {
        private SortedList<string, Stalk> nonregex;
        private List<Stalk> regex;

        private StalkList()
        {
            nonregex = new SortedList<string, Stalk>();
            regex = new List<Stalk>();
        }

        public static StalkList fetch(XPathNodeIterator xpni)
        {
            StalkList list = new StalkList();
            
            while( xpni.MoveNext())
            {
                Stalk s = Stalk.create(xpni.Current.GetAttribute("flag", ""), xpni.Current.GetAttribute("search", ""),
                             xpni.Current.GetAttribute("regex", "") == "true");
                if(s.IsRegularExpression)
                {
                    list.regex.Add(s);
                }
                else
                {
                    list.nonregex.Add(s.SearchTerm, s);
                }
            }

            return list;
        }
 
        public Stalk search(string value)
        {
            // try to retrieve
            Stalk s = nonregex[value];
            if(s!= null)
            {
                return s;
            }

            return null;
        }
    }
}
