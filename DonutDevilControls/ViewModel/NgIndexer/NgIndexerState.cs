using System;

namespace DonutDevilControls.ViewModel.NgIndexer
{
    public enum NgIndexerState
    {
        RingSelected,
        RingUnselected,
        TorusX,
        TorusY,
        TorusUnselected,
        Disabled
    }

    public static class NdfasExt
    {
        public static NgIndexerState Convert(this NgIndexerState ngIndexerState, NgisDisplayMode ngisDisplayMode)
        {
            switch (ngisDisplayMode)
            {
                case NgisDisplayMode.Ring:
                    switch (ngIndexerState)
                        {
                            case NgIndexerState.RingSelected:
                                return NgIndexerState.RingSelected;
                            case NgIndexerState.RingUnselected:
                                return NgIndexerState.RingSelected;
                            case NgIndexerState.TorusX:
                                return NgIndexerState.RingSelected;
                            case NgIndexerState.TorusY:
                                return NgIndexerState.RingUnselected;
                            case NgIndexerState.TorusUnselected:
                                return NgIndexerState.RingUnselected;
                            case NgIndexerState.Disabled:
                                return NgIndexerState.Disabled;
                            default:
                                throw new Exception("NgIndexerState not handled");
                        }
                case NgisDisplayMode.Torus:
                    switch (ngIndexerState)
                    {
                        case NgIndexerState.RingSelected:
                            return NgIndexerState.TorusX;
                        case NgIndexerState.RingUnselected:
                            return NgIndexerState.TorusUnselected;
                        case NgIndexerState.TorusX:
                            return NgIndexerState.TorusX;
                        case NgIndexerState.TorusY:
                            return NgIndexerState.TorusY;
                        case NgIndexerState.TorusUnselected:
                            return NgIndexerState.TorusUnselected;
                        case NgIndexerState.Disabled:
                            return NgIndexerState.Disabled;
                        default:
                            throw new Exception("NgIndexerState not handled");
                    }
                default:
                    throw new Exception("NgIndexerSetState not handled");
            }

        }

    }
}
