using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using MathLib.Intervals;

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
        public static Color IntToColor(this int colorVal)
        {
            return Color.FromArgb((byte)(255), (byte)(colorVal >> 0), (byte)(colorVal >> 8), (byte)(colorVal >> 16));
        }

        public static IEnumerable<Color> UniformSpread(Color lowColor, Color hiColor, int stepCount)
        {
            return Enumerable.Range(0, stepCount).Select
                (
                    i => new Color
                    {
                        ScA = RealIntervalExt.TicAtIndex(lowColor.ScA, hiColor.ScA, i, stepCount + 1),
                        ScR = RealIntervalExt.TicAtIndex(lowColor.ScR, hiColor.ScR, i, stepCount + 1),
                        ScG = RealIntervalExt.TicAtIndex(lowColor.ScG, hiColor.ScG, i, stepCount + 1),
                        ScB = RealIntervalExt.TicAtIndex(lowColor.ScB, hiColor.ScB, i, stepCount + 1)
                    }
                );
        }

        public static IEnumerable<Color> D2(int width)
        {
            float w2 = width * width;
            return Enumerable.Range(0, width * width).Select
                (

                    i => new Color
                    {
                        ScA = (float)1.0,
                        ScR = (float)((1.0 + Math.Sin(i * Math.PI * 2.0 / width)) / 2.0),
                        ScG = (float)((1.0 + Math.Sin(i * Math.PI * 2.0 / w2)) / 2.0),
                        ScB = (float)((2.0 + Math.Cos(i * Math.PI * 2.0 / w2) + Math.Cos(i * Math.PI * 2.0 / width)) / 4.0)
                    }

                );
        }

        public static IEnumerable<Color> D3(int width)
        {
            float w2 = width * width;

            return Enumerable.Range(0, width * width).Select
                (

                    i => new Color
                    {
                        ScA = (float)1.0,
                        ScR = (float)( i % 2),
                        ScG = (float)((i / width) % 2),
                        ScB = (float)((2.0 + Math.Cos(i * Math.PI * 2.0 / w2) + Math.Cos(i * Math.PI * 2.0 / width)) / 4.0)
                    }

                );
        }

        public static IEnumerable<Color> FadingSpread(this Color color, int stepCount)
        {
            return Enumerable.Range(0, stepCount).Select
                (
                    i => new Color
                    {
                        ScA = RealIntervalExt.TicAtIndex((float) 0.0, color.ScA, i, stepCount + 1),
                        ScR = color.ScR,
                        ScG = color.ScG,
                        ScB = color.ScB
                    }
                );
        }

        public static IN1ColorSequence Dipolar(Color negativeColor, Color positiveColor, int halfStepCount)
        {
            return new Ir1ColorSequence(
                    new ColorSequenceImpl(positiveColor.FadingSpread(halfStepCount)),
                    new ColorSequenceImpl(negativeColor.FadingSpread(halfStepCount))
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
                    UniformSpread(color1, color2, quarterStepCount)
                        .Concat(UniformSpread(color2, color3, quarterStepCount))
                        .Concat(UniformSpread(color3, color4, quarterStepCount))
                        .Concat(UniformSpread(color4, color1, quarterStepCount))
                );
        }

        public static IColorSequence2D TriPolar(int width)
        {
            return new ColorSequenceImpl2D
                (
                   colors: D2(width),
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
            return colorSequence.Colors[(int)((value + 0.5) * 0.999 * colorSequence.Colors.Count)];
        }

        public static Color ToUnitColor2D(this IColorSequence colorSequence, double valueX, double valueY, int colorWidth)
        {
            var xoff = (int)((valueX + 0.5) * 0.999 * colorWidth) * colorWidth;
            var yoff = (int)((valueY + 0.5) * 0.999 * colorWidth);

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
        }

        private readonly IColorSequence _positiveColors;
        public IColorSequence PositiveColors
        {
            get { return _positiveColors; }
        }

        private readonly IColorSequence _negativeColors;
        public IColorSequence NegativeColors
        {
            get { return _negativeColors; }
        }

        public IReadOnlyList<Color> Colors
        {
            get
            {
                return NegativeColors
                            .Colors
                            .Reverse()
                            .Concat
                            (
                                PositiveColors.Colors
                            ).ToList();
            }
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
