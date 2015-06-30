using LibNode;

namespace La.ViewModel.Design
{
    public class DesignEntityRepoVm : EntityRepoVm
    {
        public DesignEntityRepoVm() : base(DesignEntityRepo)
        {
        }

        public static IEntityRepo DesignEntityRepo { get { return null;} }
    }
}
