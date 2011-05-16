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
            baseNode = new FalseNode();
        }

        public ComplexStalk(string flag, string time)
            : base(flag, time)
        {
            baseNode = new FalseNode();

        }

        private StalkNode baseNode;
        
        public override bool match(RecentChange rc)
        {
            return baseNode.match(rc);
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("complexstalk", xmlns);
            e.SetAttribute("flag", this.flag);
            e.SetAttribute("lastupdate", LastUpdateTime.ToString());

            e.AppendChild(baseNode.toXmlFragment(doc, xmlns));
            return e;
        }

        public override string ToString()
        {
            return "Flag: " + flag + ", Last modified: "+this.LastUpdateTime+", Type: Complex " + baseNode.ToString();
        }

        public static new Stalk newFromXmlElement(XmlElement element)
        {
            XmlAttribute time = element.Attributes["lastupdate"];
            string timeval = time == null ? DateTime.Now.ToString() : time.Value;
            ComplexStalk s = new ComplexStalk(element.Attributes["flag"].Value, timeval);
            
            if(element.HasChildNodes)
            {
                StalkNode n = StalkNode.newFromXmlFragment(element.FirstChild);
                s.baseNode = n;
            }

            return s;
        }

        public void setSearchTree(StalkNode node, bool isupdate)
        {
            if (isupdate)
                this.LastUpdateTime = DateTime.Now;

            baseNode = node;
        }
    }
}