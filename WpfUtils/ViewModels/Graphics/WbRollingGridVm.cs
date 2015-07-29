using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using LibNode;
using WpfUtils.Views.Graphics;

namespace WpfUtils.ViewModels.Graphics
{
    public class WbRollingGridVm : WbImageVm
    {
        public WbRollingGridVm(
            int imageWidth, int imageHeight,
            int cellDimX, int cellDimY) 
            : base(imageWidth, imageHeight)
        {
            CellDimX = cellDimX;
            CellDimY = cellDimY;
        }

        public void AddValues(IEnumerable<D2Val<Color>> cellColors)
        {
            PlotRectangleList.Clear();

            var cellColorList = cellColors.ToList();

            //var squareStride = Math.Min
            //    (
            //        ImageWidth / CellDimX,
            //        ImageHeight / CellDimY
            //    );
            var squareWidth = ImageWidth/CellDimX;
            var squareHeight = ImageHeight/CellDimY;
;

            PlotRectangleList = cellColorList.Select(

                gv =>
                    new PlotRectangle
                        (
                            x: squareWidth * gv.X,
                            y: squareHeight * gv.Y,
                            width: squareWidth,
                            height: squareHeight,
                            color: gv.Val
                        )
                    ).ToList();

            OnPropertyChanged("PlotRectangles");
        }

        public int CellDimX { get; private set; }

        public int CellDimY { get; private set; }

    }
}
