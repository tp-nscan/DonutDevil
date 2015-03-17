namespace DonutDevilControls.ViewModel.Legend
{
    public enum LegendVmType
    {
        Image,
        Seq
    }

    public interface ILegendVm
    {
        LegendVmType LegendVmType { get; }
    }
}
