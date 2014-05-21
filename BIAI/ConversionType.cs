using System.ComponentModel;

namespace BIAI
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