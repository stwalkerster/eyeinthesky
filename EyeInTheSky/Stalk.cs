using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EyeInTheSky
{
    class Stalk
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

        protected Stalk()
        {
            this.regex = false;
        }

        static Stalk create()
        {
            throw new NotImplementedException();
        }
    }
}
