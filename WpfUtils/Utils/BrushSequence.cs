using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace WpfUtils.Utils
{
    public interface IBrushSequence
    {
        IReadOnlyList<Brush> Brushes { get; }
    }

    public interface IZ1BrushSequence
    {
        IReadOnlyList<Brush> PositiveBrushes { get; }
        IReadOnlyList<Brush> NegativeBrushes { get; }
    }

    public static class BrushSequence
    {
        public static IBrushSequence ToBrushSequence(this IEnumerable<Color> colors)
        {
            return new BrushSequenceImpl(colors.ToBrushes());
        }

        public static IEnumerable<Brush> ToBrushes(this IEnumerable<Color> colors)
        {
           return colors.Select(c => new SolidColorBrush(c));
        }

        public static IZ1BrushSequence DualBrushes(Color positiveColor, Color negativeColor, int steps)
        {
            return new Z1BrushSequence(
                positiveBrushes: positiveColor.FadingSpread(steps + 1).ToBrushes(),
                negativeBrushes: negativeColor.FadingSpread(steps + 1).ToBrushes()
            );
        }

        public static IZ1BrushSequence RedBlueBrushSequence(int steps)
        {
            return DualBrushes(positiveColor: Color.FromRgb(64, 128, 255), negativeColor: Color.FromRgb(255, 0, 0), steps: steps);
        }

        public static Brush ToBrush(this IZ1BrushSequence iz1BrushSequence, float value)
        {
            return (value > 0)
                ? iz1BrushSequence.PositiveBrushes[(int) (value * 0.999 * iz1BrushSequence.PositiveBrushes.Count)]
                : iz1BrushSequence.NegativeBrushes[(int)(-value * 0.999 * iz1BrushSequence.NegativeBrushes.Count)];
        }
    }

    internal class BrushSequenceImpl : IBrushSequence
    {
        public BrushSequenceImpl(IEnumerable<Brush> brushes)
        {
            _brushes = brushes.ToList();
        }

        private readonly List<Brush> _brushes;
        public IReadOnlyList<Brush> Brushes
        {
            get { return _brushes; }
        }
    }


    internal class Z1BrushSequence : IZ1BrushSequence
    {
        public Z1BrushSequence(IEnumerable<Brush> positiveBrushes, IEnumerable<Brush> negativeBrushes)
        {
            _positiveBrushes = positiveBrushes.ToList();
            _negativeBrushes = negativeBrushes.ToList();
        }

        private readonly List<Brush> _positiveBrushes;
        public IReadOnlyList<Brush> PositiveBrushes
        {
            get { return _positiveBrushes; }
        }

        private readonly List<Brush> _negativeBrushes;
        public IReadOnlyList<Brush> NegativeBrushes
        {
            get { return _negativeBrushes; }
        }

    }
}
