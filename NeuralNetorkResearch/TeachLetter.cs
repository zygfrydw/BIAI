using System.Windows.Media.Imaging;
using NuralNetwork;

namespace NeuralNetorkResearch
{
    public class TeachLetter : ITeachLetter
    {
        public BitmapImage Image { get; set; }
        public string Name { get; set; }
    }
}