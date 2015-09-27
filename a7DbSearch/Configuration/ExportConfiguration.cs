using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace a7DbSearch.Configuration
{
    class ExportConfiguration
    {

        static SqlConnection _sqlConn;
        public static SqlConnection SqlConnection
        { 
            get {
                if (_sqlConn == null)
                    _sqlConn = new SqlConnection(DB);
                if (_sqlConn.State == System.Data.ConnectionState.Closed)
                    _sqlConn.Open();
                return _sqlConn;
            } 
        }
        public static string DB { get; private set; }
        public static Dictionary<string,HierarchyExplorerConfiguration> HierarchyExplorerConfigs;

        static ExportConfiguration()
        {
            XDocument xconf = XDocument.Load("Config.xml");
            XElement xroot = xconf.Root;
            DB = xroot.Element("DB").Value.Trim();
            HierarchyExplorerConfigs = new Dictionary<string, HierarchyExplorerConfiguration>();
            foreach (XElement xe in xroot.Elements("HierarchyExplorer"))
            {
                HierarchyExplorerConfigs.Add(xe.Attribute("name").Value, new HierarchyExplorerConfiguration(xe));
            }
        }
    }
}
