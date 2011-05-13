using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EyeInTheSky
{
    public class Stalk
    {
        /// <summary>
        /// is the search term a regular expression?
        /// </summary>
        protected bool regex;
        /// <summary>
        /// what to search for
        /// </summary>
        protected string search;
        /// <summary>
        /// the name of the stalkworkd
        /// </summary>
        protected string flag;
        
        /// <summary>
        /// is the search term a regular expression?
        /// </summary>
        public bool IsRegularExpression
        {
            get { return regex; }
        }

        /// <summary>
        /// what to search for
        /// </summary>
        public string SearchTerm
        {
            get { return search; }
        }

        /// <summary>
        /// the name of the stalkworkd
        /// </summary>
        public string Flag
        {
            get { return flag; }
        }

        public void init()
        {
            this.regex = false;
        }

        public static Stalk create(string flag, string search, bool isregex)
        {
            Stalk s = isregex ? new RegexStalk() : new Stalk();
            s.flag = flag;
            s.search = search;
            s.init();

            return s;
        }
    }
}
