using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace EyeInTheSky
{
    public class Stalk
    {
        /// <summary>
        /// the name of the stalkworkd
        /// </summary>
        protected string flag;

        public Stalk(string flag)
        {
            if(flag == "")
                throw new ArgumentOutOfRangeException();
            this.flag = flag;
        }

        /// <summary>
        /// the name of the stalkworkd
        /// </summary>
        public string Flag
        {
            get { return flag; }
        }

        private bool hasusercheck, haspagecheck, hassummarycheck;

        private Regex user, page, summary;

        public void setUserSearch(string regex)
        {
            hasusercheck = true;
            user = new Regex(regex);
        }
        public void setPageSearch(string regex)
        {
            haspagecheck = true;
            page = new Regex(regex);
        }
        public void setSummarySearch(string regex)
        {
            hassummarycheck = true;
            summary = new Regex(regex);
        }
    
        public void ToXmlFragment(XmlTextWriter xtw)
        {
            xtw.WriteStartElement("stalk");
            {
                xtw.WriteAttributeString("flag", flag);

                if(hasusercheck)
                {
                    xtw.WriteStartElement("user");
                    {
                        xtw.WriteAttributeString("value", user.ToString());   
                    }
                    xtw.WriteEndElement();
                }

                if (haspagecheck)
                {
                    xtw.WriteStartElement("page");
                    {
                        xtw.WriteAttributeString("value", page.ToString());
                    }
                    xtw.WriteEndElement();
                }

                if (hassummarycheck)
                {
                    xtw.WriteStartElement("summary");
                    {
                        xtw.WriteAttributeString("value", summary.ToString());
                    }
                    xtw.WriteEndElement();
                }
            }
            xtw.WriteEndElement();
        }
    }
}
