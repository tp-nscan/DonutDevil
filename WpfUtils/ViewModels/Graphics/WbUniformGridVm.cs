﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using MathLib.NumericTypes;
using WpfUtils.Views.Graphics;

namespace WpfUtils.ViewModels.Graphics
{
    public class WbUniformGridVm : WbImageVm
    {
        public WbUniformGridVm(int cellDimX, int cellDimY)
            : base(CalcPixelResolution(cellDimX, cellDimY) * cellDimX, CalcPixelResolution(cellDimX, cellDimY) * cellDimY)
        {
            if ((cellDimX > MaxCellDim) || (cellDimY > MaxCellDim))
            {
                throw new Exception("Cell dimension too large");
            }

            _cellDimX = cellDimX;
            _cellDimY = cellDimY;
            PixelResolution = CalcPixelResolution(cellDimX, cellDimY);
        }

        private readonly int _cellDimX;
        public int CellDimX
        {
            get { return _cellDimX; }
        }

        private readonly int _cellDimY;
        public int CellDimY
        {
            get { return _cellDimY; }
        }

        public int PixelResolution { get; set; }

        public void AddValues(IEnumerable<D2Val<Color>> cellColors)
        {
            PlotRectangleList.Clear();

            PlotRectangleList = cellColors.Select(

                gv =>
                    new PlotRectangle
                        (
                            x: PixelResolution * gv.X,
                            y: PixelResolution * gv.Y,
                            width: PixelResolution,
                            height: PixelResolution,
                            color: gv.Value
                        )
                    ).ToList();

            OnPropertyChanged("PlotRectangles");

        }

        public static int CalcPixelResolution(int cellDimX, int cellDimY)
        {
            return Math.Min(MaxCellDim / Math.Max(cellDimX, cellDimY), 20);
        }

        public const int MaxCellDim = 1024;
    }
}
