using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EyeInTheSky
{
    class RegexStalk : Stalk
    {
        private Regex r;
        protected RegexStalk() : base()
        {
            this.regex = true;
            r = new Regex(this.search);
        }
    }
}
