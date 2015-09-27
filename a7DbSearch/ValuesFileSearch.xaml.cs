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
using System.IO;

namespace a7DbSearch
{
    /// <summary>
    /// Interaction logic for ValuesFileSearch.xaml
    /// </summary>
    public partial class ValuesFileSearch : UserControl
    {
        public a7FileSearch FileSearch
        {
            get
            {
                if (DataContext is a7FileSearch)
                    return DataContext as a7FileSearch;
                else return null;
            }
        }

        public ValuesFileSearch()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(UserControl1_DataContextChanged);
        }

        void UserControl1_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (FileSearch != null)
            {
                FileSearch.ActualizedWork += new EventHandler<a7FileSearch.FileSearchEventArgs>(fileSearch_AcutalizedWork);
                FileSearch.Finished += new EventHandler(fileSearch_finished);
            }
        }


        void fileSearch_finished(object sender, EventArgs e)
        {
            tiFileSearchResult.Dispatcher.Invoke(new Action(() => { tiFileSearchResult.IsSelected = true; }));
        }

        void fileSearch_AcutalizedWork(object sender, a7FileSearch.FileSearchEventArgs e)
        {
            this.tbProgress.Dispatcher.Invoke(
                        new Action(
                            () =>
                            {
                                this.tbProgress.Text = e.Message + ": " + "Value:" + e.ActualSearchedValue + " (" + e.SearchedValuesFinished + "/" + e.SearchedValuesCount +
                                    ") File:" + e.ActualSearchedFile;
                            }
                            )
                        );
            this.pbProgress.Dispatcher.Invoke(
                new Action(
                    () =>
                    {
                        this.pbProgress.Value = ((double)e.SearchedValuesFinished / ((double)e.SearchedValuesCount)) * 100;
                    }
                    )
                );
        }

        private void lbValuesSearchedInFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string selectedValue = ((KeyValuePair<string, int>)e.AddedItems[0]).Key;
                FileSearch.SelectValueFileList(selectedValue);
                tbSelectedValueForFile.Text = (selectedValue);
                ucSqler.ParseQuery(selectedValue);
                ucSqler2.ParseQuery(selectedValue);
            }
        }

        private void bSearchFiles_Click(object sender, RoutedEventArgs e)
        {
            this.pbProgress.Value = 0;
            FileSearch.BeginSearchFiles(this.tbFolder.Text, this.tbValuesToSearch.Text, this.tbExtensions.Text);
        }

        private void bSelectFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.SelectedPath = this.tbFolder.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.tbFolder.Text = dlg.SelectedPath;
            }
        }


        private void lbFilesWithValueFound_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb != null && lb.SelectedItem is string)
            {
                FileInfo fi = new FileInfo(lb.SelectedItem as string);
                string windir = Environment.GetEnvironmentVariable("WINDIR");
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                if (rbExplorer.IsChecked == true)
                {
                    prc.StartInfo.FileName = windir + @"\explorer.exe";
                    prc.StartInfo.Arguments = fi.Directory.FullName;
                }
                else if (rbNotepad.IsChecked == true)
                {
                    prc.StartInfo.FileName = windir + @"\notepad.exe";
                    prc.StartInfo.Arguments = fi.FullName;
                }
                prc.Start();


            }
        }


        private void bAbortSearch_Click(object sender, RoutedEventArgs e)
        {
            this.FileSearch.AbortSearch();
        }


        private void cbShowSqler_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
                cdSqler.Width = new GridLength(400);
            else
                cdSqler.Width = new GridLength(0);
        }

        private void showEditor_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                if (cb.IsChecked == true)
                    cdEditor.Height = new GridLength(1000.0,GridUnitType.Star);
                else
                    cdEditor.Height = new GridLength(0);
            }
        }

        int found = 0;
        int actualFound = 0;
        string selectedValueToSearch = "";
        string selectedFile = "";
        private void lbFilesWithValueFound_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb != null && lb.SelectedItem is string)
            {
                selectedFile = lb.SelectedItem as string;
                using (StreamReader sr = new StreamReader(selectedFile))
                {
                    string fileContent = sr.ReadToEnd();
                    FlowDocument fd = new FlowDocument();
                    Paragraph p = new Paragraph(new Run(fileContent));
                    fd.Blocks.Add(p);
                    rtbEditFile.Document = fd;
                    actualFound = 0; 
                    selectedValueToSearch =((KeyValuePair<string, int>)this.lbValuesSearchedInFile.SelectedItem).Key;
                    found = HighlightSearched(selectedValueToSearch,actualFound);
                    tbFoundCount.Text = string.Format("{0}/{1}", actualFound + 1, found);
                }
            }
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            FileStream fStream;
            TextRange range;
            range = new TextRange(rtbEditFile.Document.ContentStart, rtbEditFile.Document.ContentEnd);
            
            try
            {
                fStream = new FileStream(selectedFile, FileMode.Create);
                range.Save(fStream, DataFormats.Text);
                fStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bPrev_Click(object sender, RoutedEventArgs e)
        {
            if (actualFound > 0)
            {
                actualFound--;
                found = HighlightSearched(selectedValueToSearch, actualFound);
                tbFoundCount.Text = string.Format("{0}/{1}", actualFound + 1, found);
            }

        }

        private void bNext_Click(object sender, RoutedEventArgs e)
        {
            if (actualFound < found-1)
            {
                actualFound++;
                found = HighlightSearched(selectedValueToSearch, actualFound);
                tbFoundCount.Text = string.Format("{0}/{1}", actualFound + 1, found);
            }
        }


        private int HighlightSearched(string text, int superHighlightFound)
        {
            var paragraphArray = rtbEditFile.Document.Blocks.Where(check => check is Paragraph).ToArray();
            int iRet = 0;
            int superHighlightPos = -1;
            var pText = "";
            for (var p = 0; p < paragraphArray.Length; p++)
            {
                var paragraph = paragraphArray[p] as Paragraph;
                pText = GetTextFromParagraph(paragraph);
                paragraph.Inlines.Clear();
                if (string.IsNullOrEmpty(text))
                {
                    paragraph.Inlines.Add(new Run { Text = pText, Foreground = new SolidColorBrush(Colors.Black) });
                    continue;
                }
                if (pText.IndexOf(text,0,StringComparison.InvariantCultureIgnoreCase)!=-1)
                {
                    var textLeft = pText;
                    do
                    {
                        int index;
                        var parts = GetTextParts(textLeft, text, out index);
                        if (!string.IsNullOrEmpty(parts[0]))
                            paragraph.Inlines.Add(new Run { Text = parts[0] });
                        if (iRet == superHighlightFound)
                        {
                            var r = new Run { Text = text, Foreground = new SolidColorBrush(Colors.Yellow), Background = new SolidColorBrush(Colors.Blue) };
                            paragraph.Inlines.Add(r);
                            superHighlightPos = index;
                            var start = r.ContentStart;
                            rtbEditFile.Selection.Select(GetPoint(start, superHighlightPos), GetPoint(start, superHighlightPos + text.Length));
                            rtbEditFile.Focus();
                        }
                        else
                            paragraph.Inlines.Add(new Run { Text = text, Foreground = new SolidColorBrush(Colors.Blue), Background = new SolidColorBrush(Colors.Yellow) });
                        textLeft = parts[1];
                        iRet++;
                    } while (!string.IsNullOrEmpty(textLeft) && textLeft.Contains(text));
                    if (!string.IsNullOrEmpty(textLeft))
                        paragraph.Inlines.Add(new Run { Text = textLeft, Foreground = new SolidColorBrush(Colors.Black) });
                }
                else
                    paragraph.Inlines.Add(new Run { Text = pText, Foreground = new SolidColorBrush(Colors.Black) });
             }
            
            return iRet;
        }

        private static TextPointer GetPoint(TextPointer start, int x)
        {
            var ret = start;
            var i = 0;
            while (i < x && ret != null)
            {
                if (ret.GetPointerContext(LogicalDirection.Backward) ==
                    TextPointerContext.Text ||
                    ret.GetPointerContext(LogicalDirection.Backward) ==
                        TextPointerContext.None)
                    i++;
                if (ret.GetPositionAtOffset(1,
                        LogicalDirection.Forward) == null)
                    return ret;
                ret = ret.GetPositionAtOffset(1,
                     LogicalDirection.Forward);
            }
            return ret;
        }


        private string GetTextFromParagraph(Paragraph paragraph)
        {
            var sb = new StringBuilder();
            foreach (Run run in paragraph.Inlines.Where(check => check is Run))
                sb.Append(run.Text);
            return sb.ToString();
        }

        private string[] GetTextParts(string original, string textToFind, out int index)
        {
            var result = new string[2];
            index = original.IndexOf(textToFind,0,StringComparison.InvariantCultureIgnoreCase);
            result[0] = original.Substring(0, index);
            result[1] = original.Substring(index + textToFind.Length);
            return result;
        }

        private void tbEnterSeperator_Click(object sender, RoutedEventArgs e)
        {
            this.tbSeperator.SetCurrentValue(TextBox.TextProperty, "[enter]");
            BindingExpression be = BindingOperations.GetBindingExpression(tbSeperator, TextBox.TextProperty);
            if (be != null) be.UpdateSource();
        }

    }
}
