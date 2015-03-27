using System;
using System.Reactive.Subjects;
using System.Windows.Input;
using WpfUtils;

namespace DonutDevilMain.ViewModel
{
    public class SandboxVm : NotifyPropertyChanged, IMainWindowVm
    {
        #region Navigation

        public MainWindowType MainWindowType
        {
            get { return MainWindowType.Sandbox; }
        }

        private readonly Subject<IMainWindowVm> _mainWindowTypehanged
            = new Subject<IMainWindowVm>();
        public IObservable<IMainWindowVm> OnMainWindowTypeChanged
        {
            get { return _mainWindowTypehanged; }
        }

        #endregion // Navigation

        #region GoToMenuCommand

        RelayCommand _goToMenuCommand;
        public ICommand GoToMenuCommand
        {
            get
            {
                return _goToMenuCommand ?? (_goToMenuCommand = new RelayCommand(
                    param => DoGoToMenu(),
                    param => CanGoToMenu()
                    ));
            }
        }

        private void DoGoToMenu()
        {
            _mainWindowTypehanged.OnNext(new MenuVm());
        }

        bool CanGoToMenu()
        {
            return true;
        }

        #endregion // GoToMenuCommand
    }
}
