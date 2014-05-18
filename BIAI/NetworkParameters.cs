using System.ComponentModel;
using System.Runtime.CompilerServices;
using BIAI.Annotations;

namespace BIAI
{

    public class NetworkParameters : INotifyPropertyChanged
    {
        private double networkError;
        private double alpha;
        private double eta;
        private double beta;

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}