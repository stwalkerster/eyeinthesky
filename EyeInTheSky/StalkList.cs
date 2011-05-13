using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace EyeInTheSky
{
    public class StalkList
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

            if(EyeInTheSkyBot.config["ignoreregex", "false"] == "false")
            {
                throw new NotImplementedException();
            }

            return null;
        }
   
        public void add(Stalk s)
        {
            if(s.IsRegularExpression)
            {
                regex.Add(s);
            }
            else
            {
                nonregex.Add(s.SearchTerm, s);
            }
        }

        public void remove(string flag)
        {
            nonregex.Remove(flag);
            for (int i = 0; i < regex.Count; i++)
            {
                if(regex[i].Flag == flag)
                {
                    regex.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
