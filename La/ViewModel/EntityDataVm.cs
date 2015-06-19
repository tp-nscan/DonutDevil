using LibNode;

namespace La.ViewModel
{
    public class EntityDataVm
    {
        public EntityDataVm(EntityData entityData)
        {
            EntityData = entityData;
        }

        EntityData EntityData { get; }

        public string Epn => Entvert.EpnToString(EntityData.Epn);

        public string Description => EntityOps.ArrayDescrString(EntityData.ArrayDescr);
    }
}
