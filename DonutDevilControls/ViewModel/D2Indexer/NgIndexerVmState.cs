using System;
using DonutDevilControls.ViewModel.Legend;

namespace DonutDevilControls.ViewModel.D2Indexer
{
    public enum NgIndexerVmState
    {
        OneDSelected,
        OneDUnselected,
        TwoDx,
        TwoDy,
        TwoDUnselected,
        Disabled
    }

    public static class NdfasExt
    {
        public static NgIndexerVmState Convert(this NgIndexerVmState ngIndexerState, LegendMode ngisDisplayMode)
        {
            switch (ngisDisplayMode)
            {
                case LegendMode.OneLayer:
                    switch (ngIndexerState)
                        {
                            case NgIndexerVmState.OneDSelected:
                                return NgIndexerVmState.OneDSelected;
                            case NgIndexerVmState.OneDUnselected:
                                return NgIndexerVmState.OneDSelected;
                            case NgIndexerVmState.TwoDx:
                                return NgIndexerVmState.OneDSelected;
                            case NgIndexerVmState.TwoDy:
                                return NgIndexerVmState.OneDUnselected;
                            case NgIndexerVmState.TwoDUnselected:
                                return NgIndexerVmState.OneDUnselected;
                            case NgIndexerVmState.Disabled:
                                return NgIndexerVmState.Disabled;
                            default:
                                throw new Exception("NgIndexerState not handled");
                        }
                case LegendMode.TwoLayers:
                    switch (ngIndexerState)
                    {
                        case NgIndexerVmState.OneDSelected:
                            return NgIndexerVmState.TwoDx;
                        case NgIndexerVmState.OneDUnselected:
                            return NgIndexerVmState.TwoDUnselected;
                        case NgIndexerVmState.TwoDx:
                            return NgIndexerVmState.TwoDx;
                        case NgIndexerVmState.TwoDy:
                            return NgIndexerVmState.TwoDy;
                        case NgIndexerVmState.TwoDUnselected:
                            return NgIndexerVmState.TwoDUnselected;
                        case NgIndexerVmState.Disabled:
                            return NgIndexerVmState.Disabled;
                        default:
                            throw new Exception("NgIndexerState not handled");
                    }
                default:
                    throw new Exception("NgIndexerSetState not handled");
            }

        }

    }
}
