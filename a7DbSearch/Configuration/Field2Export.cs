using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Linq;

namespace a7DbSearch.Configuration
{
    class Field2Export : XConfigElementBase
    {
        public string Name { get { return xelement.Attribute("name").Value;} }
        public string Caption { get { return xelement.Attribute("caption").Value; } }

        public Field2Export(XElement x):base(x)
        {

        }
    }
}
