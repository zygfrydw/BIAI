using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using NuralNetwork;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace BIAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public static readonly DependencyProperty LearningSetPathProperty = DependencyProperty.Register(
            "LearningSetPath", typeof (string), typeof (MainWindow), new PropertyMetadata(default(string)));

        private NeuralNetwork nuralNetwork;
        private int letters;
        private string[] lettersArray;

        public static readonly DependencyProperty AnswereProperty = DependencyProperty.Register(
            "Answere", typeof (double[]), typeof (MainWindow), new PropertyMetadata(default(double[])));

        public double[] Answere
        {
            get { return (double[]) GetValue(AnswereProperty); }
            set { SetValue(AnswereProperty, value); }
        }
        public string LearningSetPath
        {
            get { return (string) GetValue(LearningSetPathProperty); }
            set { SetValue(LearningSetPathProperty, value); }
        }

        public static readonly DependencyProperty AnsereLetterProperty = DependencyProperty.Register(
            "AnsereLetter", typeof (string), typeof (MainWindow), new PropertyMetadata(default(string)));

        public string AnsereLetter
        {
            get { return (string) GetValue(AnsereLetterProperty); }
            set { SetValue(AnsereLetterProperty, value); }
        }

        public static readonly DependencyProperty InputImageProperty = DependencyProperty.Register(
            "InputImage", typeof(BitmapImage), typeof(MainWindow), new PropertyMetadata(default(BitmapImage)));

        public BitmapImage InputImage
        {
            get { return (BitmapImage)GetValue(InputImageProperty); }
            set { SetValue(InputImageProperty, value); }
        }

        public static readonly DependencyProperty NetworkParametersProperty = DependencyProperty.Register(
            "NetworkParameters", typeof (NetworkParameters), typeof (MainWindow), new PropertyMetadata(default(NetworkParameters)));

        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
            "IsBusy", typeof (bool), typeof (MainWindow), new PropertyMetadata(default(bool)));

        public bool IsBusy
        {
            get { return (bool) GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
        private AbortableWorker worker;

        public static readonly DependencyProperty NeuralNetworkWraperProperty = DependencyProperty.Register(
            "NeuralNetworkWraper", typeof (NeuralNetworkWraper), typeof (MainWindow), new PropertyMetadata(default(NeuralNetworkWraper)));

        public NeuralNetworkWraper NeuralNetworkWraper
        {
            get { return (NeuralNetworkWraper) GetValue(NeuralNetworkWraperProperty); }
            set { SetValue(NeuralNetworkWraperProperty, value); }
        }
        public NetworkParameters NetworkParameters
        {
            get { return (NetworkParameters) GetValue(NetworkParametersProperty); }
            set { SetValue(NetworkParametersProperty, value); }
        }

    
        public MainWindow()
        {
            InitializeComponent();
            var path = Path.GetFullPath(@"..\..\TeachingSets\\Simple");
            LearningSetPath = path;
            NetworkParameters = new NetworkParameters();
            IterationLabel.DataContext = NetworkParameters;
        }

        private void CreateWorker()
        {
            worker = new AbortableWorker();
            worker.DoWork += CreateNetworkAsync;
            worker.RunWorkerCompleted += CreateNetworkCompleted;
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var path = Path.GetFullPath(@"..\..\TeachingSets");
            dialog.SelectedPath = path;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LearningSetPath = dialog.SelectedPath;
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var mainDirectory = new DirectoryInfo(LearningSetPath);
            var images = mainDirectory.GetDirectories().Select(SelectLearningSet);
            
            NetworkParameters.LearningSets = new ObservableCollection<LearningSet>(images);
            
            if(NetworkParameters.LearningSets.Count == 0)
                NetworkParameters.LearningSets.Add(SelectLearningSet(mainDirectory));
            
            LettersTabControl.SelectedIndex = 0;
            NeuralNetworkWraper = null;
        }

        private LearningSet SelectLearningSet(DirectoryInfo directory)
        {
            return new LearningSet
            {
                
                Name = directory.Name,
                Letters = directory.GetFiles("*.bmp").Select(x => new TeachLetter()
                {
                    Name = Path.GetFileNameWithoutExtension(x.Name),
                    Image = new BitmapImage(new Uri(x.FullName))

                }).ToList()
            };
        }

        private void CreateNetork_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkParameters.LearningSets != null)
            {
                FreezeImages();
                IsBusy = true;
                CreateWorker();
                worker.RunWorkerAsync(NetworkParameters);
            }
            else
            {
                MessageBox.Show("Nie załadowałeś liter którymi chcesz nauczyć sieć", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FreezeImages()
        {
            foreach (var item in NetworkParameters.LearningSets)
            {
                item.Letters.ForEach(x => x.Image.Freeze());
            }
        }


        private void CreateNetworkAsync(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            try
            {
                var networkWraper = new NeuralNetworkWraper();
                var parameters = doWorkEventArgs.Argument as NetworkParameters;
                networkWraper.TeachNetwork(parameters);
                doWorkEventArgs.Result = networkWraper;
            }
            catch (Exception e)
            {
                doWorkEventArgs.Cancel = true;
            }
        }

        private void CreateNetworkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
            if (!e.Cancelled)
            {
                NeuralNetworkWraper = e.Result as NeuralNetworkWraper;
                MessageBox.Show("Sieć została utworzona", "Zrobione", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Sieć nie została utworzona ponieważ została przekroczona maksymalna liczba prób uczenia sieci", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            BindingOperations.DisableCollectionSynchronization(NetworkParameters.LearningSets);
        }

        private void CalculateAnswereClick(object sender, RoutedEventArgs e)
        {
            if (NeuralNetworkWraper != null)
            {
                try
                {
                    AnsereLetter = NeuralNetworkWraper.CalculateOutput(InputImage);
                    Answere = NeuralNetworkWraper.Answere;
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Obraz powinien mieć taki sam rozmiar jak dane uczące. Czyli " + NeuralNetworkWraper.InputHeight + " X " + NeuralNetworkWraper.InputWidth, "Błąd", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Nie utworzyłeś sieci. Najpiew naucz sieć neuronową.", "Błąd", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void InputImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void LoadQueryImage(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = @"Bitmapa (*.bmp)|*.bmp|All files (*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                InputImage = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            if(worker != null)
                worker.Abort();
            IsBusy = false;
            //MessageBox.Show("Sieć nie została utworzona ponieważ operacja została anulowana", "Przewane", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
