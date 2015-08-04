using LibNode;
using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace La.ViewModel
{
    public class ZeusSnapVm : NotifyPropertyChanged
    {
        public ZeusSnapVm(AthenaTr athenaTr, string caption)
        {
            AthenaTr = athenaTr;
            Caption = caption;
            if (AthenaTr == null) return;

            Iteration = AthenaTr.Athena.Iteration;
            aM = new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, heightOverWidth: 2.0);
            bM = new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, heightOverWidth: 2.0);
            sM = new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, heightOverWidth: 2.0);
            dAdR = new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, heightOverWidth: 2.0);
            dBdR = new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, heightOverWidth: 2.0);
            dAdA = new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, heightOverWidth: 2.0);
            dAdB = new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, heightOverWidth: 2.0);
            dBdA = new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, heightOverWidth: 2.0);
            dBdB = new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, heightOverWidth: 2.0);
            dSdS = new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, heightOverWidth: 2.0);
            dSdP = new WbVerticalStripesVm(stripeCount: AthenaTr.Athena.GroupCount, heightOverWidth: 2.0);
        }

        public AthenaTr AthenaTr { get;}

        public string Caption { get; }
        public int Iteration { get; }

        public WbVerticalStripesVm aM { get; }
        public WbVerticalStripesVm bM { get; }
        public WbVerticalStripesVm sM { get; }
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
