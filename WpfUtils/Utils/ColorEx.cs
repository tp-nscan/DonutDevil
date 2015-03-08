using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using MathLib.Intervals;

namespace WpfUtils.Utils
{
    public static class ColorEx
    {
        public static Color IntToColor(this int colorVal)
        {
            return Color.FromArgb(255, (byte) (colorVal >> 0), (byte) (colorVal >> 8), (byte) (colorVal >> 16));
        }

        public static uint ToUint(this Color c)
        {
            return (uint) c.A << 24 | ((uint) c.R << 16) | ((uint) c.G << 8) | c.B;
        }

        public static int ToInt(this Color c)
        {
            return c.A << 24 | (c.R << 16) | (c.G << 8) | c.B;
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
            float w2 = width*width;
            return Enumerable.Range(0, width*width).Select
                (
                    i => new Color
                    {
                        ScA = (float) 1.0,
                        ScR = (float) ((1.0 + Math.Sin(i*Math.PI*2.0/width))/2.0),
                        ScG = (float) ((1.0 + Math.Sin(i*Math.PI*2.0/w2))/2.0),
                        ScB = (float) ((2.0 + Math.Cos(i*Math.PI*2.0/w2) + Math.Cos(i*Math.PI*2.0/width))/4.0)
                    }

                );
        }

        public static IEnumerable<Color> D3(int width)
        {
            float w2 = width*width;

            return Enumerable.Range(0, width*width).Select
                (

                    i => new Color
                    {
                        ScA = (float) 1.0,
                        ScR = (float) (i%2),
                        ScG = (float) ((i/width)%2),
                        ScB = (float) ((2.0 + Math.Cos(i*Math.PI*2.0/w2) + Math.Cos(i*Math.PI*2.0/width))/4.0)
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
    }
}