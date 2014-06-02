using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using BIAI.Annotations;

namespace NuralNetwork
{
    public class LearningSet : INotifyPropertyChanged, ILearningSet
    {
        private string name;
        private List<TeachLetter> letters;

        public string Name
        {
            get { return name; }
            set
            {
                if (value == name) return;
                name = value;
                OnPropertyChanged();
            }
        }

        public List<TeachLetter> Letters
        {
            get { return letters; }
            set
            {
                if (Equals(value, letters)) return;
                letters = value;
                OnPropertyChanged();
            }
        }

        IEnumerable<ITeachLetter> ILearningSet.Letters
        {
            get { return Letters; }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    public class TeachLetter : INotifyPropertyChanged, ITeachLetter
    {
        private BitmapImage image;
        private string name;

        public BitmapImage Image
        {
            get { return image; }
            set
            {
                if (Equals(value, image)) return;
                image = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (value == name) return;
                name = value;
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