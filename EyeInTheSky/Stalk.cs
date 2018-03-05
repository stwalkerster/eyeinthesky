﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky
{
    using EyeInTheSky.Model;

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

        public abstract XmlElement ToXmlFragment(XmlDocument doc, string xmlns);

        public static Stalk newFromXmlElement(XmlElement element)
        {
            switch (element.Name)
            {
                case "stalk":
                    return ComplexStalk.NewFromXmlElement(element);
                case "complexstalk":
                    return ComplexStalk.NewFromXmlElement(element);
                default:
                    throw new XmlException();
            }
        }


    }
}
