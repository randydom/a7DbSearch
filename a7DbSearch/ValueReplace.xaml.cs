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
    /// Interaction logic for ValueReplace.xaml
    /// </summary>
    public partial class ValueReplace : UserControl
    {
        public ValueReplace()
        {
            InitializeComponent();
        }

        private void bReplace_Click(object sender, RoutedEventArgs e)
        {
            string repl = tbReplace.Text;
            string with = tbWith.Text;
            if (repl.ToLower() == "[enter]")
                repl = "\r\n";
            if (with.ToLower() == "[enter]")
                with = "\r\n";
            tbOut.Text = tbIn.Text.Replace(repl, with);
        }
    }
}
