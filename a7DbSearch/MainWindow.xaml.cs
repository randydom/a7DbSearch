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

using System.Reflection;
using System.Configuration;
using a7DbSearch.Configuration;
using System.Data;
using System.ComponentModel;

namespace a7DbSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppViewModel ViewModel { get; set; }


        public a7DbSearchEngine DBSearch
        {
            get
            {
                if (ViewModel != null)
                    return ViewModel.DBSearch;
                else return null;
            }
        }

        public a7FileSearch FileSearch
        {
            get
            {
                if (ViewModel != null)
                    return ViewModel.FileSearch;
                else return null;
            }
        }

        public MainWindow()
        {
            ViewModel = new AppViewModel();
            this.DataContext = ViewModel;
            InitializeComponent();
            
            
        }



    }
}
