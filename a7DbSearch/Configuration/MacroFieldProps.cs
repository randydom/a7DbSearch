using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace a7DbSearch.Configuration
{
    class MacroFieldProps : XConfigElementBase
    {
        public string TableName { get { return xelement.Attribute("tableName").Value; } }
        public string IdField { get { return xelement.Attribute("idField").Value; } }
        public string NextField { get { return xelement.Attribute("nextMacroField").Value; } }

        public MacroFieldProps(XElement e): base(e)
        {

        }
    }
}
