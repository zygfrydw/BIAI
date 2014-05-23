using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using BIAI.Annotations;
using NuralNetwork;

namespace BIAI
{
    public class NeuralNetworkWraper : INotifyPropertyChanged
    {
        private Dictionary<string, int> lettersMap;
        private NeuralNetwork nuralNetwork;
        private double[] answere;
        private string ansereLetter;
        private ConversionType conversionType;
        private int lettersCount;

        public void TeachNetwork(NetworkParameters parameters)
        {
            lettersMap = new Dictionary<string, int>();
            var image = parameters.LearningSets[0].Letters[0].Image;
            InputWidth = image.PixelWidth;
            InputHeight = image.PixelHeight;
            conversionType = parameters.ConversionType;
            TeachNeuralNetwork(parameters);
        }

        public int InputHeight { get; private set; }

        public int InputWidth { get; private set; }

        private void TeachNeuralNetwork(NetworkParameters parameters)
        {
            var vectors = GetTeachingVectors(parameters);
            nuralNetwork = new NeuralNetwork(3, inputsCount, lettersCount, parameters.Eta, parameters.Alpha, parameters.Beta);
            parameters.Iterations = 0;
            nuralNetwork.TeachNetwork(vectors, parameters.NetworkError, (iteration, error) =>
            {
                if (iteration % 10 == 0)
                {
                    parameters.Iterations = iteration;
                    parameters.ActualError = error;
                }
            },
            parameters.MaxIterations
            );
        }

        class DistinctComparer : IEqualityComparer<LearningSet>
        {
            public bool Equals(LearningSet x, LearningSet y)
            {
                return x.Name.Equals(y.Name);
            }

            public int GetHashCode(LearningSet obj)
            {
                return obj.Name.GetHashCode();
            }
        }
        private IEnumerable<TeachingVector> GetTeachingVectors(NetworkParameters parameters)
        {
            var vector = new List<TeachingVector>();
            var learningSets = parameters.LearningSets.SelectMany(x => x.Letters).ToList();
            var i = 0;
            lettersMap = learningSets.Select(x => x.Name).Distinct().ToDictionary( x => x, y => i++);
            lettersCount = lettersMap.Count();

            vector.AddRange(from t in learningSets 
                                let input = ConvertImageToVector(t.Image) 
                                let name = t.Name 
                                let index = lettersMap[name] 
                                let output = EncodeOneToN(index) 
                                select new TeachingVector {Inputs = input, Outputs = output});
            
            return vector;
        }

        private double[] ConvertImageToVector(BitmapImage teachLetter)
        {
            var size = teachLetter.PixelHeight * teachLetter.PixelWidth;
            var table = new double[size];
            var pixels = new int[size];
            teachLetter.CopyPixels(pixels, teachLetter.PixelWidth * 4, 0);

            for (int i = 0; i < size; i++)
            {
                table[i] = GetPixelRepresentation(pixels[i]);
            }
            return table;
        }
        private double GetPixelRepresentation(int color)
        {
            Color tmp = Color.FromArgb(color);
            switch (conversionType)
            {
                case ConversionType.Hue:
                    return tmp.GetBrightness();
                case ConversionType.Saturation:
                    return tmp.GetBrightness();
                case ConversionType.Brightness:
                    return tmp.GetBrightness();
                case ConversionType.Averange:
                    return (tmp.B + tmp.R + tmp.G ) / 765.0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string CalculateOutput(BitmapImage image)
        {
            var input = ConvertImageToVector(image);
            Answere = nuralNetwork.CalculateResponse(input);
            var maxIndex = 0;
            for (var i = 1; i < Answere.Length; i++)
            {
                if (Answere[maxIndex] < Answere[i])
                    maxIndex = i;
            }
            AnsereLetter = lettersMap.First(x=> x.Value == maxIndex).Key;
            return AnsereLetter;
        }

        public string AnsereLetter
        {
            get { return ansereLetter; }
            set
            {
                if (value == ansereLetter) return;
                ansereLetter = value;
                OnPropertyChanged();
            }
        }

        public double[] Answere
        {
            get { return answere; }
            set
            {
                if (Equals(value, answere)) return;
                answere = value;
                OnPropertyChanged();
            }
        }

        private double[] EncodeOneToN(int i)
        {
            var table = new double[lettersCount];
            table[i] = 1.0;
            return table;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public int inputsCount { get { return InputHeight*InputWidth; }}
    }
}

public static class DictionaryUtility
{
    public static int GetKeyOrAdd<K>(this Dictionary<K, int> dictionary, K key)
    {
        if (dictionary.ContainsKey(key))
            return dictionary[key];
        var count = dictionary.Count;
            dictionary.Add(key, count);
        return count;
    }
}