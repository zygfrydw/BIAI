using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BIAI.Annotations;

namespace BIAI
{
    public class NetworkParameters : INotifyPropertyChanged
    {
        private double actualError;
        private double alpha;
        private double beta;
        private ConversionType conversionType;
        private double eta;
        private uint iterations;
        private ObservableCollection<LearningSet> learningSets;
        private uint maxIterations;
        private double networkError;
        private ObservableCollection<LearningSet> testSet;

        public NetworkParameters()
        {
            NetworkError = 0.1;
            Eta = 0.2;
            Alpha = 0.6;
            Beta = 1;
            MaxIterations = 20000;
        }

        public double NetworkError
        {
            get { return networkError; }
            set
            {
                if (value.Equals(networkError)) return;
                networkError = value;
                OnPropertyChanged();
            }
        }

        public double Alpha
        {
            get { return alpha; }
            set
            {
                if (value.Equals(alpha)) return;
                alpha = value;
                OnPropertyChanged();
            }
        }

        public double Eta
        {
            get { return eta; }
            set
            {
                if (value.Equals(eta)) return;
                eta = value;
                OnPropertyChanged();
            }
        }

        public double Beta
        {
            get { return beta; }
            set
            {
                if (value.Equals(beta)) return;
                beta = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LearningSet> LearningSets
        {
            get { return learningSets; }
            set
            {
                if (Equals(value, learningSets)) return;
                learningSets = value;
                OnPropertyChanged();
            }
        }

        public uint Iterations
        {
            get { return iterations; }
            set
            {
                if (value == iterations) return;
                iterations = value;
                OnPropertyChanged();
            }
        }

        public uint MaxIterations
        {
            get { return maxIterations; }
            set
            {
                if (value == maxIterations) return;
                maxIterations = value;
                OnPropertyChanged();
            }
        }

        public double ActualError
        {
            get { return actualError; }
            set
            {
                if (value.Equals(actualError)) return;
                actualError = value;
                OnPropertyChanged();
            }
        }

        public ConversionType ConversionType
        {
            get { return conversionType; }
            set
            {
                if (value == conversionType) return;
                conversionType = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LearningSet> TestSet
        {
            get { return testSet; }
            set
            {
                if (Equals(value, testSet)) return;
                testSet = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}