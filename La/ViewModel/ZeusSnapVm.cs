using System;
using System.Linq;
using System.Windows.Media;
using LibNode;
using MathLib.NumericTypes;
using MathNet.Numerics.LinearAlgebra;
using WpfUtils;
using WpfUtils.Utils;
using WpfUtils.ViewModels.Graphics;

namespace La.ViewModel
{
    public class ZeusSnapVm : NotifyPropertyChanged
    {
        WbVerticalStripesVm Standard => 
            new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, 
                                    heightOverWidth: 0.02, 
                                    crispness:8);

        protected const int Colorsteps = 256;
        public ZeusSnapVm(AthenaTr athenaTr, string caption)
        {
            AthenaTr = athenaTr;
            Caption = caption;
            if (AthenaTr == null) return;

            Iteration = AthenaTr.Athena.Iteration;
            rM = Standard;
            vM = Standard;
            aM = Standard;
            bM = Standard;
            sM = Standard;
            dA = Standard;
            dB = Standard;
            dS = Standard;
            dAdR = Standard;
            dBdR = Standard;
            dAdA = Standard;
            dAdB = Standard;
            dBdA = Standard;
            dBdB = Standard;
            dSdS = Standard;
            dSdP = Standard;

            var legendColorSequence = ColorSequence.Dipolar(Colors.Red, Colors.Green, Colorsteps / 2);
            DrawLegend(i => legendColorSequence.Colors[(int)(i * Colorsteps)]);
        }

        public void DrawLegend(Func<float, Color> colorFunc)
        {
            DrawLegendM1(colorFunc, AthenaTr.mR, rM);
            DrawLegendM1(colorFunc, AthenaTr.mV, vM);
            DrawLegendM1(colorFunc, AthenaTr.Athena.mA, aM);
            DrawLegendM1(colorFunc, AthenaTr.dA, dA);
            DrawLegendM1(colorFunc, AthenaTr.dAdR, dAdR);
            DrawLegendM1(colorFunc, AthenaTr.dAdA, dAdA);
            DrawLegendM1(colorFunc, AthenaTr.dAdB, dAdB);

            DrawLegendM1(colorFunc, AthenaTr.Athena.mB, bM);
            DrawLegendM1(colorFunc, AthenaTr.dB, dB);
            DrawLegendM1(colorFunc, AthenaTr.dBdR, dBdR);
            DrawLegendM1(colorFunc, AthenaTr.dBdA, dBdA);
            DrawLegendM1(colorFunc, AthenaTr.dBdB, dBdB);

            DrawLegendM1(colorFunc, AthenaTr.Athena.mS, sM);
            DrawLegendM1(colorFunc, AthenaTr.dS, dS);
            DrawLegendM1(colorFunc, AthenaTr.dSdS, dSdS);
            DrawLegendM1(colorFunc, AthenaTr.dSdP, dSdP);
        }

        public void DrawLegendM1(Func<float, Color> colorFunc, Matrix<float> m, WbVerticalStripesVm vsvm)
        {
            var absMax = (float)(Math.Max(Enumerable.Range(0, m.ColumnCount).Max(i => Math.Abs(m[0, i])), 
                                          1.0));
            vsvm.AddValues
            (
                Enumerable.Range(0, aM.StripeCount)
                    .Select(i =>
                        new D1Val<Color>(
                            i,
                            colorFunc((m[0, i] / absMax + 1.0f) /2.05f)
                        ))
            );
        }

        public AthenaTr AthenaTr { get;}

        public string Caption { get; }
        public int Iteration { get; }

        public WbVerticalStripesVm rM { get; }
        public WbVerticalStripesVm vM { get; }
        public WbVerticalStripesVm aM { get; }
        public WbVerticalStripesVm bM { get; }
        public WbVerticalStripesVm sM { get; }

        public WbVerticalStripesVm dA { get; }
        public WbVerticalStripesVm dB { get; }
        public WbVerticalStripesVm dS { get; }

        public WbVerticalStripesVm dAdR { get; }
        public WbVerticalStripesVm dBdR { get; }
        public WbVerticalStripesVm dAdA { get; }
        public WbVerticalStripesVm dAdB { get; }
        public WbVerticalStripesVm dBdA { get; }
        public WbVerticalStripesVm dBdB { get; }
        public WbVerticalStripesVm dSdS { get; }
        public WbVerticalStripesVm dSdP { get; }
    }
}
