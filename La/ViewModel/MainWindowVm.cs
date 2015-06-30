using System;
using WpfUtils;

namespace La.ViewModel
{
    public class MainWindowVm : NotifyPropertyChanged
    {
        public MainWindowVm()
        {
            ContentVm = new MenuVm();
        }

        private IMainContentVm _contentVm;

        public IMainContentVm ContentVm
        {
            get { return _contentVm; }
            set
            {
                _subscripton?.Dispose();
                _contentVm = value;
                _subscripton = _contentVm.OnMainWindowTypeChanged.Subscribe(vm => ContentVm = vm);
                OnPropertyChanged("ContentVm");
            }
        }

        private IDisposable _subscripton;
    }
}
