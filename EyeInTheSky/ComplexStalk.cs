using System;
using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky
{
    public class ComplexStalk : Stalk
    {
        public ComplexStalk(string flag)
            : base(flag)
        {
        }

        private StalkNode baseNode;

        #region Overrides of Stalk

        public override bool match(RecentChange rc)
        {
            return baseNode != null && baseNode.match(rc);
        }

        public override void ToXmlFragment(XmlTextWriter xtw)
        {
            throw new NotImplementedException();
        }

        public static new Stalk newFromXmlElement(XmlElement element)
        {
            ComplexStalk s = new ComplexStalk(element.Attributes["flag"].Value);
            
            if(element.HasChildNodes)
            {
                StalkNode n = StalkNode.newFromXmlFragment(element.FirstChild);
                s.baseNode = n;
            }

            return s;
        }

        #endregion

        public void setSearchTree(StalkNode node)
        {
            baseNode = node;
        }
    }
}