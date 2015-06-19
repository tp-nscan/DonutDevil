using LibNode;
using WpfUtils;

namespace La.ViewModel
{
    public class EntityVm : NotifyPropertyChanged
    {
        public EntityVm(Entity entity)
        {
            Entity = entity;
        }

        Entity Entity { get; }

        public string Name => Entvert.EntityNameToString(Entity.Name);

        public string GeneratorId => Entvert.GeneratorIdToString(Entity.GeneratorId);

        public int Iteration => Entity.Iteration;
    }
}
