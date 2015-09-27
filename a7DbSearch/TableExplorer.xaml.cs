using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace a7DbSearch
{
    /// <summary>
    /// Interaction logic for DBExplorer.xaml
    /// </summary>
    public partial class TableExplorer : UserControl
    {
        private Action _close;
        public a7TableExplorer ViewModel
        {
            get
            {
                if (this.DataContext is a7TableExplorer)
                    return this.DataContext as a7TableExplorer;
                else
                    return null;
            }
        }

        public TableExplorer(Action close)
        {
            InitializeComponent();
            _close = close;
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            if (_close != null)
                _close();
        }

        private void bFilter_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.Refresh();
        }

        private void tbValuesToSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void tbValuesToSearch_KeyUp(object sender, KeyEventArgs e)
        {
        //    if(ViewModel!=null)
        //        this.ViewModel.ClearFieldFilters();
        }

        private void bCommitChanges_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CommitChanges();
        }
    }
}
