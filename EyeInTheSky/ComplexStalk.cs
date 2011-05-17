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

        public ComplexStalk(string flag, string time, string time2)
            : base(flag, time, time2)
        {
            baseNode = new FalseNode();

        }

        private StalkNode baseNode;


        public override bool match(RecentChange rc)
        {
            bool success = baseNode.match(rc);
            if(success)
            {
                this.LastTriggerTime = DateTime.Now;
                EyeInTheSkyBot.config.LogStalkTrigger(flag, rc);
                return true;
            }
            return false;
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("complexstalk", xmlns);
            e.SetAttribute("flag", this.flag);
            e.SetAttribute("lastupdate", LastUpdateTime.ToString());
            e.SetAttribute("lasttrigger", LastTriggerTime.ToString());

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
            time = element.Attributes["lasttrigger"];
            string timeval2 = time == null ? DateTime.Parse("1/1/1970 00:00:00").ToString() : time.Value;


            ComplexStalk s = new ComplexStalk(element.Attributes["flag"].Value, timeval, timeval2);
            
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