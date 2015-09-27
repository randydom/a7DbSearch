using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace a7DbSearch
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public a7HierarchyExplorer HierarchyExplorer { get; set; }
        public a7DbSearchEngine DBSearch { get; set; }
        public a7FileSearch FileSearch { get; set; }

        public AppViewModel()
        {
            DBSearch = new a7DbSearchEngine();
            HierarchyExplorer = new a7HierarchyExplorer();
            FileSearch = new a7FileSearch();
            FileSearch.DBSearch = DBSearch;
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
    }
}
