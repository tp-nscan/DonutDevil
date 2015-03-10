using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using MathLib.NumericTypes;
using WpfUtils.Views.Graphics;

namespace WpfUtils.ViewModels.Graphics
{
    public class WbVerticalStripesVm : WbImageVm
    {
        public WbVerticalStripesVm(int cellDimX, double heightOverWidth, Func<float, Color> colorMap)
            : base(CalcPixelResolution(cellDimX) * cellDimX, (int) ((CalcPixelResolution(cellDimX) * cellDimX) * heightOverWidth))
        {
            if (cellDimX > MaxCellDim)
            {
                throw new Exception("Cell dimension too large");
            }

            _cellDimX = cellDimX;
            _colorMap = colorMap;
            PixelResolution = CalcPixelResolution(cellDimX);
        }

        private List<D2Val<float>> _gridVals = new List<D2Val<float>>();

        private List<D2Val<float>> GridVals
        {
            get { return _gridVals; }
            set { _gridVals = value; }
        }

        private readonly int _cellDimX;
        public int CellDimX
        {
            get { return _cellDimX; }
        }

        public int PixelResolution { get; set; }

        public void AddValues(IEnumerable<D2Val<float>> gridVals)
        {
            GridVals = gridVals.ToList();
            RefreshPlotRectangles();
        }

        private Func<float, Color> _colorMap;
        public Func<float, Color> ColorMap
        {
            get { return _colorMap; }
            set
            {
                _colorMap = value;
                RefreshPlotRectangles();
            }
        }

        void RefreshPlotRectangles()
        {
            PlotRectangleList.Clear();

            PlotRectangleList = GridVals.Select(
                
                gv =>
                    new PlotRectangle
                       (
                            x: PixelResolution * gv.X,
                            y: PixelResolution * gv.Y,
                            width: PixelResolution,
                            height: ImageHeight,
                            color: ColorMap.Invoke(gv.Value)                    
                       )
                    ).ToList();

            OnPropertyChanged("PlotRectangles");

        }

        public static int CalcPixelResolution(int cellDimX)
        {
            return Math.Min(MaxCellDim / cellDimX, 20);
        }

        public const int MaxCellDim = 2000;
    }
}
