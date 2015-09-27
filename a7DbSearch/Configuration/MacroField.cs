using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Linq;

namespace a7DbSearch.Configuration
{
    class MacroField : XConfigElementBase
    {
        public string Name { get { return xelement.Attribute("name").Value; } }

        public MacroField(XElement x) : base(x)
        {

        }
    }
}
