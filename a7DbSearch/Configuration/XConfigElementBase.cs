using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace a7DbSearch.Configuration
{
    abstract class XConfigElementBase 
    {
        protected XElement xelement;

        public XConfigElementBase(XElement x)
        {
            xelement = x;
        }
    }
}
