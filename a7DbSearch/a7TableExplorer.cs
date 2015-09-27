using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using a7DbSearch.Configuration;
using System.ComponentModel;
using System.Windows.Forms;

namespace a7DbSearch
{
    public class a7TableExplorer : INotifyPropertyChanged
    {
        public string TableName { get; private set; }
        public DataTable Data { get; private set; }
        private SqlDataAdapter DataAdapter;
        private string _combinedSql;
        public string SQL { get; set; }
        private string _columnWhere;
        public Dictionary<string, string> FilterFields;


        public a7TableExplorer(string name)
        {
            Data = new DataTable();
            this.TableName = name;
            this.SQL = "Select top 100 * from " + TableName;
            this._combinedSql = this.SQL;
            this._columnWhere = "";
            Refresh();
        }

        public void Refresh()
        {
            Filter2Sql();
            Data = new DataTable();
            SqlCommand columnsSelect = new SqlCommand(this._combinedSql, ExportConfiguration.SqlConnection);
            
            DataAdapter = new SqlDataAdapter(columnsSelect);
            SqlCommandBuilder sqlBuilder = new SqlCommandBuilder(DataAdapter);
            DataAdapter.UpdateCommand = sqlBuilder.GetUpdateCommand();
            DataAdapter.InsertCommand = sqlBuilder.GetInsertCommand();
            DataAdapter.DeleteCommand = sqlBuilder.GetDeleteCommand();
            DataAdapter.Fill(Data);
            OnPropertyChanged("Data");
        }


        public void ClearFieldFilters()
        {
            FilterFields.Clear();
        }

        public void Filter2Sql()
        {

            if (FilterFields != null)
            {
                string where = "";
                bool isFirst = true;
                foreach (KeyValuePair<string, string> kv in FilterFields)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        where += " AND ";
                    where += kv.Key + " LIKE ('%" + kv.Value + "%') ";
                }

                if (string.IsNullOrWhiteSpace(where))
                {
                    this._combinedSql = this.SQL;
                }
                else if (this.SQL.IndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    this._combinedSql = this.SQL.Replace(this.TableName, this.TableName + " WHERE " + where);
                }
                else
                {
                    int pos1 = this.SQL.IndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase);
                    int pos2 = pos1 + "WHERE".Length;
                    string beforeWhere = this.SQL.Substring(0, pos2 + 1);
                    string afterWhere = this.SQL.Substring(pos2 + 1);
                    this._combinedSql = beforeWhere + " ( " + where + " ) AND ( " + afterWhere + " ) ";
                }
            }
            else
            {
                _combinedSql = this.SQL;
            }
            OnPropertyChanged("SQL");
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion


        public void CommitChanges()
        {
            try
            {
                int ret = DataAdapter.Update(Data);
            //    DataAdapter.Fill(Data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
