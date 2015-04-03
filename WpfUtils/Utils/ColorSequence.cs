using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace WpfUtils.Utils
{
    public interface IColorSequence
    {
        IReadOnlyList<Color> Colors { get; }
    }

    public interface IN1ColorSequence : IColorSequence
    {
        IColorSequence PositiveColors { get; }
        IColorSequence NegativeColors { get; } 
    }

    public interface IColorSequence2D : IColorSequence
    {
        int Width { get; }
    }

    public static class ColorSequence
    {
        public static IColorSequence Dipolar(Color negativeColor, Color positiveColor, int halfStepCount)
        {
            return new Ir1ColorSequence(
                    new ColorSequenceImpl(positiveColor.LessFadingSpread(halfStepCount)),
                    new ColorSequenceImpl(negativeColor.LessFadingSpread(halfStepCount))
                );
        }

        public static IColorSequence Quadrupolar(
            Color color1,
            Color color2,
            Color color3,
            Color color4,
            int quarterStepCount)
        {
            return new ColorSequenceImpl
                (
                    ColorEx.UniformSpread(color1, color2, quarterStepCount)
                        .Concat(ColorEx.UniformSpread(color2, color3, quarterStepCount))
                        .Concat(ColorEx.UniformSpread(color3, color4, quarterStepCount))
                        .Concat(ColorEx.UniformSpread(color4, color1, quarterStepCount))
                );
        }

        public static IColorSequence2D TriPolar(int width)
        {
            return new ColorSequenceImpl2D
                (
                   colors: ColorEx.D2(width),
                   width: width
                );
        }

        public static Color ToUnitColor(this IN1ColorSequence in1ColorSequence, double value)
        {
            return (value > 0)
                ? in1ColorSequence.PositiveColors.Colors[(int)( value * 0.999 * in1ColorSequence.PositiveColors.Colors.Count)]
                : in1ColorSequence.NegativeColors.Colors[(int)(-value * 0.999 * in1ColorSequence.NegativeColors.Colors.Count)];
        }

        public static Color ToUnitColor(this IColorSequence colorSequence, double value)
        {
            return colorSequence.Colors[(int)(value * 0.999 * colorSequence.Colors.Count)];
        }

        public static Color ToUnitColor2D(this IColorSequence colorSequence, double valueX, double valueY, int colorWidth)
        {
            return colorSequence.Colors[((int)((valueX + 0.5) * 0.999 * colorWidth) * colorWidth) + (int)((valueY + 0.5) * 0.999 * colorWidth)];
        }

        public static Color ToUnitColor2D(
            this IColorSequence colorSequence, 
            int valueX, 
            int valueY, 
            int colorBits)
        {
            var xoff = (valueX) << (colorBits);
            var yoff = (valueY);

            return colorSequence.Colors[xoff + yoff];
        }


        public static IColorSequence ToUniformColorSequence(this Color maxColor, int steps)
        {
            return new ColorSequenceImpl(maxColor.FadingSpread(steps));
        }

    }


    public class ColorSequenceImpl : IColorSequence
    {
        public ColorSequenceImpl(IEnumerable<Color> colors)
        {
            _colors = colors.ToList();
        }

        private readonly List<Color> _colors;
        public IReadOnlyList<Color> Colors
        {
            get { return _colors; }
        }

    }


    public class Ir1ColorSequence : IN1ColorSequence
    {
        public Ir1ColorSequence
            (
                IColorSequence positiveColors, 
                IColorSequence negativeColors
            )
        {
            _positiveColors = positiveColors;
            _negativeColors = negativeColors;
            _colors = NegativeColors
                            .Colors
                            .Reverse()
                            .Concat
                            (
                                PositiveColors.Colors
                            ).ToList();
        }

        private readonly IColorSequence _positiveColors;
        public IColorSequence PositiveColors
        {
            get { return _positiveColors; }
        }

        private IReadOnlyList<Color> _colors;

        private readonly IColorSequence _negativeColors;
        public IColorSequence NegativeColors
        {
            get { return _negativeColors; }
        }

        public IReadOnlyList<Color> Colors
        {
            get { return _colors; }
        }
    }

    public class ColorSequenceImpl2D : IColorSequence2D
    {
        public ColorSequenceImpl2D(IEnumerable<Color> colors, int width)
        {
            _width = width;
            _colors = colors.ToList();
        }

        private readonly List<Color> _colors;
        public IReadOnlyList<Color> Colors
        {
            get { return _colors; }
        }

        private readonly int _width;
        public int Width
        {
            get { return _width; }
        }
    }
}
