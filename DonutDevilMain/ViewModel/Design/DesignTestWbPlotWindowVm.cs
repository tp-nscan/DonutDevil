using DonutDevilControls.ViewModel.Design.Common;
using WpfUtils.Utils;

namespace DonutDevilMain.ViewModel.Design
{
    public class DesignTestWbPlotWindowVm : TestWbPlotWindowVm
    {
        public DesignTestWbPlotWindowVm()
        {

            var n1ColorSequence = ColorSequence.TriPolar(2);

            var x = 0;
        }

        private DesignPlot1DVm _plotRectangles = new DesignPlot1DVm();
        public DesignPlot1DVm DesignPlot1DVm
        {
            get { return _plotRectangles; }
            set
            {
                _plotRectangles = value;
                OnPropertyChanged("DesignPlot1DVm");
            }
        }

    }
}
