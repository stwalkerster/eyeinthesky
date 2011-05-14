using System;
using System.Xml;

namespace EyeInTheSky
{
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