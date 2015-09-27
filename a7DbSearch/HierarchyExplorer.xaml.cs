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
    /// Interaction logic for MenuExplorer.xaml
    /// </summary>
    public partial class HierarchyExplorer : UserControl
    {
        public a7HierarchyExplorer ViewModel
        {
            get { return DataContext as a7HierarchyExplorer; }
        }

        public HierarchyExplorer()
        {
            InitializeComponent();
            cbHierarchies.SelectedIndex = 0;
            DataContextChanged += new DependencyPropertyChangedEventHandler(UserControl1_DataContextChanged);
        }

        void UserControl1_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void bExport_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.Explore(this.tbClassList.Text, this.cbHierarchies.SelectedValue.ToString());
            }
        }

        private void cbHierarchies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
