namespace DonutDevilControls.ViewModel.Params
{
    public enum ParamType
    {
        Bool,
        Int,
        Float,
        Enum
    }

    public interface IParamVm
    {
        ParamType ParamType { get; }
        string Name { get; }
    }
}
