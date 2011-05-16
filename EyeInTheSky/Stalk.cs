using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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

        protected Stalk(string flag, string lastUpdateTime, string lastTriggerTime)
        {
            if (flag == "")
                throw new ArgumentOutOfRangeException();
            this.flag = flag;

            this.lastUpdateTime = DateTime.Parse(lastUpdateTime);

            this.lastTriggerTime = DateTime.Parse(lastTriggerTime);

        }

        private DateTime lastUpdateTime = DateTime.Now;
        private DateTime lastTriggerTime = DateTime.Parse("1/1/1970 00:00:00");

        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime; }
            protected set { lastUpdateTime = value;}
        }

        public DateTime LastTriggerTime
        {
            get { return lastTriggerTime; }
            protected set { lastTriggerTime = value; }
        }

        /// <summary>
        /// the name of the stalkworkd
        /// </summary>
        public string Flag
        {
            get { return flag; }
        }

        public abstract bool match(RecentChange rc);

        public abstract XmlElement ToXmlFragment(XmlDocument doc, string xmlns);

        public static Stalk newFromXmlElement(XmlElement element)
        {
            switch (element.Name)
            {
                case "stalk":
                    return SimpleStalk.newFromXmlElement(element);
                case "complexstalk":
                    return ComplexStalk.newFromXmlElement(element);
                default:
                    throw new XmlException();
            }
        }
    }
}
