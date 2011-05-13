using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EyeInTheSky
{
    public class RegexStalk : Stalk
    {
        private Regex r;
        public new void init()
        {
            this.regex = true;
            r = new Regex(this.SearchTerm);
        }
    }
}
