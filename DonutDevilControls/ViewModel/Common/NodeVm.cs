using System.Windows;
using System.Windows.Media;
using MathLib.NumericTypes;
using WpfUtils;

namespace DonutDevilControls.ViewModel.Common
{
    public interface INodeVm
    {
        Brush ForegroundBrush { get; set; }
        Brush BackgroundBrush { get; set; }
        Brush BorderBrush { get; set; }
        Thickness BorderThickness { get; set; }
        double Size { get; set; }
        PointFlt UpLeft { get; set; }
        int GroupIndex { get; }
    }

    public static class NodeVm
    {
        public static INodeVm MakeSimple(
                int groupIndex
            )
        {
            return new NodeVmImpl(
                    groupIndex: groupIndex
                )
            {
                ForegroundBrush = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            };
        }

        public static double LowRightX(this INodeVm nodeVm)
        {
            return nodeVm.UpLeft.X + nodeVm.Size;
        }

        public static double LowRightY(this INodeVm nodeVm)
        {
            return nodeVm.UpLeft.Y + nodeVm.Size;
        }

    }


    public class NodeVmImpl : NotifyPropertyChanged, INodeVm
    {
        public NodeVmImpl(int groupIndex)
        {
            _groupIndex = groupIndex;
        }

        private Brush _foregroundBrush;
        public Brush ForegroundBrush
        {
            get { return _foregroundBrush; }
            set
            {
                _foregroundBrush = value;
                OnPropertyChanged("ForegroundBrush");
            }
        }


        private Brush _backgroundBrush;
        public Brush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set
            {
                _backgroundBrush = value;
                OnPropertyChanged("BackgroundBrush");
            }
        }


        private Brush _borderBrush;
        public Brush BorderBrush
        {
            get { return _borderBrush; }
            set
            {
                _borderBrush = value;
                OnPropertyChanged("BorderBrush");
            }
        }

        private Thickness _borderThickness;
        public Thickness BorderThickness
        {
            get { return _borderThickness; }
            set
            {
                _borderThickness = value;
                OnPropertyChanged("BorderThickness");
            }
        }

        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private PointFlt _upLeft;
        public PointFlt UpLeft
        {
            get { return _upLeft; }
            set
            {
                _upLeft = value;
                OnPropertyChanged("UpLeft");
            }
        }


        private readonly int _groupIndex;
        public int GroupIndex
        {
            get { return _groupIndex; }
        }

        private double _size;
        public double Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged("Size");
            }
        }
    }
}
