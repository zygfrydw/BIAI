using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace NuralNetwork
{
    public interface ILearningSet
    {
        string Name { get; }
        IEnumerable<ITeachLetter> Letters { get; }
    }

    public interface ITeachLetter
    {
        BitmapImage Image { get; }
        string Name { get;  }
    }
}