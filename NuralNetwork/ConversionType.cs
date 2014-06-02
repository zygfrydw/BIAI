using System.ComponentModel;

namespace NuralNetwork
{
    public enum ConversionType
    {
        [Description("Jasność")]
        Brightness,
        [Description("Nasycenie")]
        Saturation,
        [Description("Barwa")]
        Hue,
        [Description("Średnia")]
        Averange
    }
}