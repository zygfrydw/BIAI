using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NuralNetwork;
using Color = System.Drawing.Color;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace BIAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static readonly DependencyProperty LearningSetsProperty = DependencyProperty.Register(
            "LearningSets", typeof (ObservableCollection<LearningSet>), typeof (MainWindow), new PropertyMetadata(default(ObservableCollection<LearningSet>)));

        public ObservableCollection<LearningSet> LearningSets
        {
            get { return (ObservableCollection<LearningSet>) GetValue(LearningSetsProperty); }
            set { SetValue(LearningSetsProperty, value); }
        }

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

        private BackgroundWorker worker;

        public NetworkParameters NetworkParameters
        {
            get { return (NetworkParameters) GetValue(NetworkParametersProperty); }
            set { SetValue(NetworkParametersProperty, value); }
        }
        public MainWindow()
        {
            InitializeComponent();
            LearningSetPath = @"C:\Users\Zygfryd\Grafika\Biai — kopia";
            NetworkParameters = new NetworkParameters();
            NetworkParameters.NetworkError = 0.1;
            NetworkParameters.Eta = 0.2;
            NetworkParameters.Alpha = 0.6;
            NetworkParameters.Beta = 1;
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LearningSetPath = dialog.SelectedPath;
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var mainDirectory = new DirectoryInfo(LearningSetPath);
            LearningSets = new ObservableCollection<LearningSet>();
            foreach (var set in mainDirectory.GetDirectories().Select(directory => new LearningSet
            {
                
                Name = directory.Name,
                Letters = directory.GetFiles("*.bmp")
                    .Select(x => new TeachLetter() { Name = Path.GetFileNameWithoutExtension(x.Name), Image = new BitmapImage(new Uri(x.FullName)) })
                    .ToList()
            }))
            {
                LearningSets.Add(set);
            }
        }

        private void CreateNetork_Click(object sender, RoutedEventArgs e)
        {
            worker = new BackgroundWorker();
            worker.DoWork += WorkerOnDoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();
        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            CreateNetwork();
        }

        private void CreateNetwork()
        {
            letters = LearningSets.Min(x => x.Letters.Count);
            lettersArray = new string[letters];
            var image = LearningSets[0].Letters[0].Image;
            var size = image.PixelHeight*image.PixelWidth;
            nuralNetwork = new NeuralNetwork(3, size, letters, NetworkParameters.Eta, NetworkParameters.Alpha,
                NetworkParameters.Beta);
            try
            {
                TeachNeuralNetwork();
                MessageBox.Show("Done", "Done", MessageBoxButton.OK);
            }
            catch (Exception)
            {
                MessageBox.Show("Error", "Error", MessageBoxButton.OK);
            }
        }

        private void TeachNeuralNetwork()
        {
            var vector = new List<TeachingVector>();
            foreach (var learningSet in LearningSets)
            {
                for (int i = 0; i < letters; i++)
                {
                    var input = GetPixelVaues(learningSet.Letters[i].Image);
                    var output = EncodeToOneToN(i);
                    lettersArray[i] = learningSet.Letters[i].Name;
                    var teach = new TeachingVector();
                    teach.Inputs = input;
                    teach.Outputs = output;
                    vector.Add(teach);
                }
            }
            nuralNetwork.TeachNetwork(vector, NetworkParameters.NetworkError);
        }

        private double[] EncodeToOneToN(int i)
        {
            var table = new double[letters];
            table[i] = 1.0;
            return table;
        }

        private double[] GetPixelVaues(BitmapImage teachLetter)
        {
            var size = teachLetter.PixelHeight*teachLetter.PixelWidth;
            var table = new double[size];
            var pixels = new int[size];
            teachLetter.CopyPixels(pixels, teachLetter.PixelWidth * 4, 0);

            for (int i = 0; i < size; i++)
            {
                table[i] = ParseColor(pixels[i]);
            }
            return table;
        }

        private double ParseColor(int color)
        {
            var bytes = BitConverter.GetBytes(color);
            return 1- (bytes[0] / 255.0 + bytes[1] / 255.0 + bytes[2] / 255.0) / 3.0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var input = GetPixelVaues(InputImage);
            var tmp = nuralNetwork.CalculateResponse(input);
            Answere = tmp;
            var maxIndex = 0;
            for (var i = 1; i < Answere.Length; i++)
            {
                if (tmp[maxIndex] < tmp[i])
                    maxIndex = i;
            }
            AnsereLetter = lettersArray[maxIndex];
        }

        private void InputImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = @"Bitmapa (*.bmp)|*.bmp|All files (*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                InputImage = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(worker != null)
                worker.CancelAsync();
            MessageBox.Show("Canceled", "Canceled", MessageBoxButton.OK);
        }
    }
}
