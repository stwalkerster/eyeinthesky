using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace EyeInTheSky
{
    public abstract class Stalk
    {
        /// <summary>
        /// the name of the stalkworkd
        /// </summary>
        protected string flag;

        protected Stalk(string flag)
        {
            if (flag == "")
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

        public abstract bool match(RecentChange rc);


        public abstract void ToXmlFragment(XmlTextWriter xtw);
    }

    public class SimpleStalk : Stalk
    {
        public SimpleStalk(string flag) : base(flag)
        {

        }

        public bool HasUserSearch
        {
            get { return hasusercheck; }
        }

        public bool HasPageSearch
        {
            get { return haspagecheck; }
        }

        public bool HasSummarySearch
        {
            get { return hassummarycheck; }
        }

        private bool hasusercheck;
        private bool haspagecheck;
        private bool hassummarycheck;

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

        public override bool match(RecentChange rc)
        {
            if (!(HasUserSearch || HasPageSearch || hassummarycheck))
                return false;

            if(HasUserSearch && ! user.Match(rc.User).Success)
            {
                return false;
            }

            if(HasPageSearch && ! page.Match(rc.Page).Success)
            {
                return false;
            }

            if (hassummarycheck && !summary.Match(rc.EditSummary).Success)
            {
                return false;
            }

            return true;
        }
    
        public override void ToXmlFragment(XmlTextWriter xtw)
        {
            xtw.WriteStartElement("stalk");
            {
                xtw.WriteAttributeString("flag", flag);

                if(HasUserSearch)
                {
                    xtw.WriteStartElement("user");
                    {
                        xtw.WriteAttributeString("value", user.ToString());   
                    }
                    xtw.WriteEndElement();
                }

                if (HasPageSearch)
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

    public class ComplexStalk:Stalk
    {
        public ComplexStalk(string flag) : base(flag)
        {
        }

        #region Overrides of Stalk

        public override bool match(RecentChange rc)
        {
            throw new NotImplementedException();
        }

        public override void ToXmlFragment(XmlTextWriter xtw)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
