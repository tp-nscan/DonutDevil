using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using LibNode;
using WpfUtils.Views.Graphics;

namespace WpfUtils.ViewModels.Graphics
{
    public class WbFullUniformGridVm : WbImageVm
    {
        public WbFullUniformGridVm(int cellDimX, int cellDimY) : base(cellDimX, cellDimY)
        {
        }

        public void AddValues(IEnumerable<D2Val<Color>> cellColors)
        {
            PlotRectangleList.Clear();

            var cellColorList = cellColors.ToList();
            var squareStride = Math.Min
                (
                    ImageWidth / (cellColorList.Max(c => c.X) + 1),
                    ImageHeight / (cellColorList.Max(c => c.Y) + 1)
                );

            PlotRectangleList = cellColorList.Select(

                gv =>
                    new PlotRectangle
                        (
                            x: squareStride * gv.X,
                            y: squareStride * gv.Y,
                            width: squareStride,
                            height: squareStride,
                            color: gv.Val
                        )
                    ).ToList();

            OnPropertyChanged("PlotRectangles");
        }

    }
}
