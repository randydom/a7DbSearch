using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Linq;
using System.Data;

namespace a7DbSearch.Configuration
{
    class Hierarchy : XConfigElementBase
    {
        public string CaptionField { get { return xelement.Attribute("captionField").Value; } }
        public string[] IdFields { get { return xelement.Attribute("idFields").Value.Split(','); } }
        public string[] ParentIdFields { get { return xelement.Attribute("parentIdFields").Value.Split(','); } }
        public string RootParentId { get { return xelement.Attribute("rootParent").Value; } }
        public string FieldEqualToParent 
        { 
            get {
                if (xelement.Attribute("fieldEqualToParent") != null)
                    return xelement.Attribute("fieldEqualToParent").Value;
                else
                    return "";
            } 
        }
        public int MaxDepth 
        { 
            get
            {
                if (xelement.Attribute("maxDepth") == null)
                    return int.MaxValue;
                else
                    return int.Parse(xelement.Attribute("maxDepth").Value);
            }
        }
        public bool NoLoops
        {
            get
            {
                if (xelement.Attribute("noLoops") == null)
                    return false;
                else
                    return bool.Parse(xelement.Attribute("noLoops").Value);
            }
        }

        public Hierarchy(XElement x) :base(x)
        {
        }

        private string RootWhere
        {
            get
            {
                if (RootParentId.ToUpper() == "NULL")
                    return ParentIdFields[0] + " IS NULL and ";
                else
                    return ParentIdFields[0] + "='"+RootParentId+"' and ";
            }
        }

        public string AddRootWhereToSql(string sql)
        {
            string ret = sql.Replace("where", "where " + RootWhere);
            return ret;
        }

        public string AddParentWhereToSql(string sql, DataRow parentRow)
        {
            string addWhere = " where ";
            for(int i = 0; i<IdFields.Length; i++)
            {
                addWhere += ParentIdFields[i] + "='" + parentRow[IdFields[i]] + "' and ";
            }
            string ret = sql.Replace("where", addWhere);
            return ret;
        }
    }
}
