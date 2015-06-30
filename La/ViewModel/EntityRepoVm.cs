using System.Collections.ObjectModel;
using System.Linq;
using LibNode;
using WpfUtils;

namespace La.ViewModel
{
    public class EntityRepoVm : NotifyPropertyChanged
    {
        public EntityRepoVm(IEntityRepo entityRepo)
        {
            EntityRepo = entityRepo;
            EntityVms = new ObservableCollection<EntityVm>();
            var res = EntityRepo.GetEntities();
            if (res.IsSuccess)
            {
                var ents = Rop.ExtractResult(res);
                EntityVms.AddMany(ents.Value.Select(ent => new EntityVm(ent)));
            }
            else
            {
                ErrorMessage = Rop.ExtractErrorMessage(res).Value.ToReport("\n");
            }
        }

        IEntityRepo EntityRepo { get; }

        public ObservableCollection<EntityVm> EntityVms { get; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }
    }
}
