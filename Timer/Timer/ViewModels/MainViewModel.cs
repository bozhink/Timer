using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Timer.Resources;

namespace Timer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        public ObservableCollection<ItemViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData()
        {
            //// Sample data; replace with real data
            //this.Items.Add(new ItemViewModel()
            //{
            //    ID = "0",
            //    LineOne = "runtime one",
            //    LineTwo = "Maecenas praesent accumsan bibendum",
            //    LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu"
            //});
            //this.Items.Add(new ItemViewModel()
            //{
            //    ID = "1",
            //    LineOne = "runtime two",
            //    LineTwo = "Dictumst eleifend facilisi faucibus",
            //    LineThree = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus"
            //});
            this.Items.Clear();

            if (App.textContent.Length > 0)
            {
                string[] lines = App.textContent.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] data = lines[i].Split('\t');
                    if (data.Length == 3)
                    {
                        this.Items.Add(new ItemViewModel()
                        {
                            ID = i.ToString(),
                            LineOne = data[0],
                            LineTwo = data[1],
                            LineThree = data[2]
                        });
                    }
                }
            }

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
