using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace a7DbSearch.Configuration
{
    class HierarchyExplorerConfiguration :  XConfigElementBase
    {
        string _sql;
        string _inClassList;
        string _classList;

        public string Name { get; private set; }

        public string SQL { get { return _sql.Replace("%CLASSLIST%", _inClassList); } }

        public string ClassList
        {
            get { return _classList; }
            set
            {
                _classList = value;
                string[] ars = _classList.Split(';');
                _inClassList = "";
                for (int i = 0; i < ars.Length; i++)
                {
                    if (i > 0)
                        _inClassList += ",";
                    _inClassList += "'" + ars[i] + "'";
                }
            }
        }
        public Hierarchy Hierarchy { get; private set; }
        public MacroFieldProps MacroFieldProps { get; private set; }
        public List<MacroField> MacroFields { get; private set; }

        public HierarchyExplorerConfiguration(XElement x)
            : base(x)
        {
            this.Name = x.Attribute("name").Value;
            _sql = xelement.Element("SQL").Value.Trim();
            Hierarchy = new Configuration.Hierarchy(xelement.Element("hierarchy"));
            if (xelement.Element("macroFieldProperties")!=null)
                MacroFieldProps = new Configuration.MacroFieldProps(xelement.Element("macroFieldProperties"));
            if (xelement.Element("macroFields") != null)
            {
                MacroFields = new List<MacroField>();
                foreach (XElement xe in xelement.Element("macroFields").Elements("macroField"))
                {
                    MacroFields.Add(new MacroField(xe));
                }
            }
        }
    }
}
