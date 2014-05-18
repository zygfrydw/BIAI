using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using BIAI.Annotations;

namespace BIAI
{
    public class LearningSet : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TeachLetter : INotifyPropertyChanged
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