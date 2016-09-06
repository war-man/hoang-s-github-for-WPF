using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ReadingStudy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Fields and Properties

        private readonly DispatcherTimer _timer;
        private int _index;

        private string _document;
        private bool _isLoop;
        private bool _isSeparateByNumber;
        private string _readingText;
        private string _separateData;
        private int _speed;
        private int _textFontSize;

        public string ReadingText
        {
            get { return _readingText; }
            set
            {
                _readingText = value;
                OnPropertyChanged("ReadingText");
            }
        }

        public string Document
        {
            get { return _document; }
            set
            {
                _document = value;
                OnPropertyChanged("Document");
            }
        }

        public string SeparateData
        {
            get { return _separateData; }
            set
            {
                _separateData = value;
                OnPropertyChanged("SeparateData");
            }
        }

        public bool IsSeparateByNumber
        {
            get { return _isSeparateByNumber; }
            set
            {
                _isSeparateByNumber = value;
                SeparateData = value ? "5" : SEPARATE;

                OnPropertyChanged("IsSeparateByNumber");
            }
        }

        public bool IsLoop
        {
            get { return _isLoop; }
            set
            {
                _isLoop = value;

                OnPropertyChanged("IsLoop");
            }
        }

        public int Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                OnPropertyChanged("Speed");
            }
        }

        public int TextFontSize
        {
            get { return _textFontSize; }
            set
            {
                _textFontSize = value;
                OnPropertyChanged("TextFontSize");
            }
        }

        #endregion

        private const string SEPARATE = @"~!@#$%^&*()_+|\=-`,./<>?';:";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            SeparateData = SEPARATE;
            Document = "This is demo data. You can click on 'Start to Read', then you can see something happen. Enjoy it!";
            TextFontSize = 16;
            Speed = 1;

            _timer = new DispatcherTimer();
            _timer.Tick += (o, s) =>
                               {
                                   if (_index == Lines.Count)
                                   {
                                       if (IsLoop)
                                           _index = 0;
                                       else
                                           _timer.Stop();
                                   }
                                   else
                                   {
                                       ReadingText = Lines[_index];
                                       _index++;
                                   }
                               };
        }

        private List<string> Lines { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private static List<string> SeparateDocument(string document, string separateDate, bool isByNumberOfWord)
        {
            if (string.IsNullOrEmpty(document))
                return new List<string>();

            if (isByNumberOfWord)
            {
                int number;
                if (int.TryParse(separateDate, out number))
                {
                    var words = document.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    var lines = new List<string>();
                    int index = 0;
                    while (index < words.Count)
                    {
                        string line = string.Empty;
                        for (int i = 0; i < number; i++, index++)
                        {
                            if (index >= words.Count) break;
                            line += words[index] + " ";
                        }
                        lines.Add(line);
                    }

                    return lines;
                }
                return new List<string>();
            }
            return document.Split(separateDate.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void StartToReadClick(object sender, RoutedEventArgs e)
        {
            _index = 0;
            Lines = SeparateDocument(Document, SeparateData, IsSeparateByNumber);
            int value = 1000/Speed;
            _timer.Interval = TimeSpan.FromMilliseconds(value);
            _timer.Start();
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }
    }
}