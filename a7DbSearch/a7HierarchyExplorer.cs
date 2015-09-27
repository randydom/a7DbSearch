using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using a7DbSearch.Configuration;
using System.Data;
using System.ComponentModel;

namespace a7DbSearch
{
    public class a7HierarchyExplorer : INotifyPropertyChanged
    {
        public DataTable Data { get; set; }
        public string UsedMacrosList { get; set; }
        public string NotUsedMacrosList { get; set; }
        public string[] HierarchyNames { 
            get
            {
                return ExportConfiguration.HierarchyExplorerConfigs.Keys.ToArray<string>();
            }
        }

        public a7HierarchyExplorer()
        {
           
        }

        public void Explore(string classList, string hierarchyName)
        {
            HierarchyExplorerConfiguration currentConfig = ExportConfiguration.HierarchyExplorerConfigs[hierarchyName];
            this.UsedMacrosList = "";
            currentConfig.ClassList = classList;
            string sql = currentConfig.Hierarchy.AddRootWhereToSql(currentConfig.SQL);
            SqlCommand sqlSelectCommand = new SqlCommand(sql, ExportConfiguration.SqlConnection);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlSelectCommand);
            DataTable RootData = new DataTable();
            Data = new DataTable();
            sqlAdapter.Fill(RootData);
            sqlAdapter.FillSchema(Data, SchemaType.Source);
            Data.PrimaryKey = null;
            foreach (DataColumn dc in Data.Columns)
            {
                if (dc.DataType == typeof(string))
                    dc.MaxLength = int.MaxValue;
                if (dc.Unique)
                    dc.Unique = false;
            }
            AddSubRows(Data, RootData, 0, "", currentConfig, new List<string>());
            if (currentConfig.MacroFieldProps != null)
            {
                string notUsedSql = "SELECT " + currentConfig.MacroFieldProps.IdField + " FROM " + currentConfig.MacroFieldProps.TableName +
                    " WHERE " + currentConfig.MacroFieldProps.IdField + " NOT IN (" + UsedMacrosList + ")";
                SqlCommand notUsedCommand = new SqlCommand(notUsedSql, ExportConfiguration.SqlConnection);
                SqlDataAdapter notUsedAdapter = new SqlDataAdapter(notUsedCommand);
                DataTable notUsedDt = new DataTable();
                notUsedAdapter.Fill(notUsedDt);
                NotUsedMacrosList = "";
                bool first = true;
                foreach (DataRow dr in notUsedDt.Rows)
                {
                    if (first)
                        first = false;
                    else
                        NotUsedMacrosList += ",";
                    NotUsedMacrosList += "'" + dr[0].ToString() + "'";
                }
            }
            OnPropertyChanged("Data");
            OnPropertyChanged("UsedMacrosList");
            OnPropertyChanged("NotUsedMacrosList");
        }

        int debugCount = 0;
        private void AddSubRows(DataTable mainData, DataTable fetchedData, int tabsToAdd, string ValueForFieldEqualToParent, HierarchyExplorerConfiguration currentConfig, List<string> parentFields)
        {
            if (fetchedData != null && tabsToAdd < currentConfig.Hierarchy.MaxDepth)
            {
                foreach (DataRow dr in fetchedData.Rows)
                {
                    if (currentConfig.Hierarchy.NoLoops)
                    {
                        bool indexContainsInParentIds = false;
                        for (int i = 0; i < currentConfig.Hierarchy.IdFields.Length; i++)
                        {
                            if (parentFields.Contains(dr[currentConfig.Hierarchy.IdFields[i]].ToString()))
                            {
                                indexContainsInParentIds = true;
                                break;
                            }
                        }
                        if (indexContainsInParentIds)
                            break;
                    }
                    debugCount++;
                    string subsql = currentConfig.Hierarchy.AddParentWhereToSql(currentConfig.SQL, dr);
                    SqlCommand sqlSubSelectCommand = new SqlCommand(subsql, ExportConfiguration.SqlConnection);
                    SqlDataAdapter sqlSubDataAdapter = new SqlDataAdapter(sqlSubSelectCommand);
                    DataTable subDt = new DataTable();
                    sqlSubDataAdapter.Fill(subDt);
                    string hierarchyPrefix = "";
                    for (int i = 0; i < tabsToAdd; i++)
                    {
                        if (i == 0)
                            hierarchyPrefix += "";
                        hierarchyPrefix += "     ";
                        if (i == tabsToAdd - 1)
                            hierarchyPrefix += "↳";
                    }
                    dr[currentConfig.Hierarchy.CaptionField] = hierarchyPrefix + dr[currentConfig.Hierarchy.CaptionField];
                    if (currentConfig.MacroFieldProps != null)
                    {
                        foreach (MacroField macroField in currentConfig.MacroFields)
                        {
                            if (dr[macroField.Name] != null && !string.IsNullOrEmpty(dr[macroField.Name].ToString()))
                            {
                                dr[macroField.Name] = AppendNextMacros(dr[macroField.Name].ToString(), currentConfig);
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(ValueForFieldEqualToParent) || dr[currentConfig.Hierarchy.FieldEqualToParent].ToString() == ValueForFieldEqualToParent)
                        Data.ImportRow(dr);
                    string tmpValueForFieldEqualToParent = (Data.Columns.Contains(currentConfig.Hierarchy.FieldEqualToParent)) ?  dr[currentConfig.Hierarchy.FieldEqualToParent].ToString() : "";
                    List<string> newParentFields = new List<string>();
                    newParentFields.AddRange(parentFields);
                    for (int i = 0; i < currentConfig.Hierarchy.IdFields.Length; i++)
                    {
                        newParentFields.Add(dr[currentConfig.Hierarchy.IdFields[i]].ToString());
                        //addWhere += ParentIdFields[i] + "='" + parentRow[IdFields[i]] + "' and ";
                    }
                    AddSubRows(mainData, subDt, tabsToAdd + 1, tmpValueForFieldEqualToParent, currentConfig, newParentFields);
                    //foreach (DataRow subdr in subDt.Rows)
                    //{
                    //    Data.ImportRow(subdr);
                    //}
                }
            }
        }

        private string AppendNextMacros(string macroName, HierarchyExplorerConfiguration currentConfig)
        {
            return AppendNextMacros(macroName, macroName, currentConfig);
        }

        private string AppendNextMacros(string macroName, string lastMacroString, HierarchyExplorerConfiguration currentConfig)
        {
            string ret = lastMacroString;
            string sql = "Select " + currentConfig.MacroFieldProps.NextField + " FROM " + currentConfig.MacroFieldProps.TableName +
                " WHERE " + currentConfig.MacroFieldProps.IdField + " = '" + macroName + "'";// ExportConfiguration.NextMacroSql.Replace("%ID%", macroName);
            SqlCommand sqlSubSelectCommand = new SqlCommand(sql, ExportConfiguration.SqlConnection);
            SqlDataAdapter sqlSubDataAdapter = new SqlDataAdapter(sqlSubSelectCommand);
            DataTable subDt = new DataTable();
            sqlSubDataAdapter.Fill(subDt);
            if (subDt.Rows.Count > 0)
            {
                string nextMacroId = subDt.Rows[0][0].ToString();
                if (!string.IsNullOrEmpty(nextMacroId))
                {
                    ret = ret + " → " + nextMacroId;
                    ret = ret + AppendNextMacros(nextMacroId, ret, currentConfig);
                }
            }
            if (!string.IsNullOrEmpty(UsedMacrosList))
                UsedMacrosList += ",";
            UsedMacrosList += "'" + macroName + "'";
            return ret;
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
