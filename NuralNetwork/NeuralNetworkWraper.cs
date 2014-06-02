using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;

namespace NuralNetwork
{
    public class NeuralNetworkWraper
    {
        private Dictionary<string, int> lettersMap;
        private NeuralNetwork nuralNetwork;
        private ConversionType conversionType;
        private int lettersCount;

        public void TeachNetwork(INetworkParameters parameters, Action<uint, double> notifyChanges)
        {
            lettersMap = new Dictionary<string, int>();
            var image = parameters.LearningSets.First().Letters.First().Image;
            InputWidth = image.PixelWidth;
            InputHeight = image.PixelHeight;
            conversionType = parameters.ConversionType;
            TeachNeuralNetwork(parameters, notifyChanges);
        }

        public int InputHeight { get; private set; }

        public int InputWidth { get; private set; }

        private void TeachNeuralNetwork(INetworkParameters parameters, Action<uint, double> notifyChanges)
        {
            var learningSets = GetTeachingVectors(parameters.LearningSets);
            var testSets = GetTeachingVectors(parameters.TestSet);

            var network = new SigmoidalNeuralNetwork(3, inputsCount, lettersCount, parameters.Eta, parameters.Alpha, parameters.Beta);
            network.NeuronActivateFunction = GetSigmoidFunction(parameters);
            nuralNetwork = network;
            nuralNetwork.TeachNetwork(learningSets, testSets, parameters.NetworkError, parameters.MaxIterations, notifyChanges);
        }

        private SigmoidFunction GetSigmoidFunction(INetworkParameters parameters)
        {
            switch (parameters.NeuronFunction)
            {
                case NeuronFunction.Sigmoid:
                    return NeuronFunctions.Sigmoid;
                case NeuronFunction.HyperbolicTangens:
                    return NeuronFunctions.HyperbolicTangens;
                case NeuronFunction.Sinusoida:
                    return NeuronFunctions.Sinusoidal;
                case NeuronFunction.Cosinusoidal:
                    return NeuronFunctions.Cosinusoidal;
                case NeuronFunction.Function001:
                    return NeuronFunctions.Function001;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerable<TeachingVector> GetTeachingVectors(IEnumerable<ILearningSet> collection)
        {
            var vector = new List<TeachingVector>();
            var learningSets = collection.SelectMany(x => x.Letters).ToList();
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

        public string AnsereLetter { get; set; }

        public double[] Answere { get; set; }

        private double[] EncodeOneToN(int i)
        {
            var table = new double[lettersCount];
            table[i] = 1.0;
            return table;
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
