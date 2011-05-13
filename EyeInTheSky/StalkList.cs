﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace EyeInTheSky
{
    public class StalkList : SortedList<string,Stalk>
    {
        public Stalk search(string value)
        {
            throw new NotImplementedException();
        }

        internal static StalkList fetch(XPathNodeIterator xpni)
        {
            StalkList list = new StalkList();

            while(xpni.MoveNext())
            {
                Stalk s = new Stalk(xpni.Current.GetAttribute("flag", ""));
                XPathDocument xpd = new XPathDocument(xpni.Current.ReadSubtree());
                var xpn = xpd.CreateNavigator();
                XmlNameTable xnt = xpn.NameTable;
                XmlNamespaceManager xnm = new XmlNamespaceManager(xnt);
                xnm.AddNamespace("e",
                                 "https://github.com/stwalkerster/eyeinthesky/raw/master/EyeInTheSky/DataFileSchema.xsd");
                var it = xpn.Select("//e:user", xnm);
                if(it.Count == 1)
                {
                    it.MoveNext();
                    s.setUserSearch(it.Current.GetAttribute("value", ""));
                }
                
                it = xpn.Select("//e:page", xnm);
                if(it.Count == 1)
                {
                    it.MoveNext();
                    s.setPageSearch(it.Current.GetAttribute("value",""));
                }

                it = xpn.Select("//e:summary", xnm);
                if(it.Count == 1)
                {
                    it.MoveNext();
                    s.setSummarySearch(it.Current.GetAttribute("value",""));
                }

                list.Add(s.Flag,s);
            }

            return list;
        }
    }
}