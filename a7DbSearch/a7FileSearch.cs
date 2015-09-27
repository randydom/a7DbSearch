using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace a7DbSearch
{
    public class a7FileSearch : INotifyPropertyChanged
    {
        public EventHandler<FileSearchEventArgs> ActualizedWork;
        public EventHandler Finished;
        public string Seperator { get; set; }
        public Dictionary<string, string> CachedFiles;
        public Dictionary<string, int> Values { get; set; }
        public Dictionary<string, List<string>> ValueFileLists { get; set; }
        public List<string> SelectedValueFileList { get; set; }
        public a7DbSearchEngine DBSearch { get; set; }
        private string[] _values;
        private string[] _extensions;
        private bool abortSearch = false;

        public class FileSearchEventArgs : EventArgs
        {
            public string Message { get; set; }
            public string ActualSearchedFile { get; set; }
            public string ActualSearchedValue { get; set; }
            public int SearchedValuesFinished { get; set; }
            public int SearchedValuesCount { get; set; }
        }

        public a7FileSearch()
        {
            Seperator = ",";
        }

        public void BeginSearchFiles(string path, string values, string ext)
        {
            abortSearch = false;

            if (string.IsNullOrEmpty(Seperator))
                _values = new string[] { values.ToString() };
            else
            {
                string seperator;
                if (Seperator.ToLower() == "[enter]")
                    seperator = "\r\n";
                else
                    seperator = Seperator;
                _values = values.ToString().Split(new string[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
            }

            _extensions = ext.Split(';');
            Values = new Dictionary<string, int>();
            ValueFileLists = new Dictionary<string, List<string>>();
            CachedFiles = new Dictionary<string, string>();
            ParameterizedThreadStart pt = new ParameterizedThreadStart(search);
            Thread t = new Thread(pt);
            t.Start(path);
        }

        public void SelectValueFileList(string value)
        {
            SelectedValueFileList = ValueFileLists[value];
            OnPropertyChanged("SelectedValueFileList");
        }

        public void AbortSearch()
        {
            abortSearch = true;
        }

        private void search(object path)
        {
            cacheFiles(new DirectoryInfo(path as string));
            foreach (string v in _values)
            {
                if (abortSearch)
                    return;
                string trimmed = v.Trim();
                Values[trimmed] = 0;
                ValueFileLists[trimmed] = new List<string>();
                searchDirectoryForValue(trimmed);
                GC.Collect();
            }
            OnPropertyChanged("Values");
            OnPropertyChanged("ValueFileLists");
            CachedFiles.Clear();
            GC.Collect();
            if (Finished != null)
                Finished(this, null);
        }

        private void cacheFiles(DirectoryInfo di)
        {
            foreach (string ext in _extensions)
            {
                foreach (FileInfo fi in di.GetFiles(ext))
                {
                    if (abortSearch)
                        return;
                    OnActualizedWork("caching", fi.FullName, "");
                    try
                    {
                        using (StreamReader sr = fi.OpenText())
                        {
                            CachedFiles[fi.FullName] = sr.ReadToEnd();
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                cacheFiles(subDi);
            }
        }

        private void searchDirectoryForValue(string value)
        {

            string escapedValue = value.Replace(".", @"\.");

            Regex rx = new Regex(escapedValue, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);           
            foreach (KeyValuePair<string,string> kv in CachedFiles)
            {
                OnActualizedWork("searching", kv.Key, value);
                try
                {
                    if (rx.IsMatch(kv.Value))
                    {
                        Values[value]++;
                        ValueFileLists[value].Add(kv.Key);
                    }
                }
                catch (Exception e)
                {

                }
            }
            //GC.Collect();
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

        public void OnActualizedWork(string message, string file, string value)
        {
            if (this.ActualizedWork != null)
                ActualizedWork(this, new FileSearchEventArgs() { 
                    ActualSearchedValue = value, 
                    ActualSearchedFile = file, 
                    SearchedValuesCount = _values.Length,  
                    SearchedValuesFinished = Values.Count,
                    Message = message
                });
        }
    }
}
