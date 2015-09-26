using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using LibNode;
using WpfUtils.Utils;

namespace WpfUtils.Views.Graphics
{
    public static class PlotPointEx
    {
        public static IReadOnlyList<D2Val<Color>> TestSequence()
        {
            var x = 0;

            return ColorSequence.TriPolar(128)
                .Colors
                .Select(
                    c => new D2Val<Color>
                        (
                        x: x % 128,
                        y: (x++) / 128,
                        val: c
                        )).ToList();
        }
    }
}
