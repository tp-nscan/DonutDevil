using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using DonutDevilControls.ViewModel.Common;
using NodeLib;
using NodeLib.NodeLib;
using WpfUtils;

namespace DonutDevilMain.ViewModel
{
    public class MenuVm : NotifyPropertyChanged, IMainWindowVm
    {
        public MenuVm()
        {
            _networkBuilderVms = new ObservableCollection<NetworkBuilderVm>
                (
                    NetworkBuilder.CurrentBuilders.Select(cb=> new NetworkBuilderVm(cb))
                );
        }

        #region Navigation
        public MainWindowType MainWindowType
        {
            get { return MainWindowType.Menu; }
        }

        private readonly Subject<IMainWindowVm> _mainWindowTypehanged
            = new Subject<IMainWindowVm>();
        public IObservable<IMainWindowVm> OnMainWindowTypeChanged
        {
            get { return _mainWindowTypehanged; }
        }

        #endregion // Navigation

        #region GoToNetworkCommand

        RelayCommand _resetNetworkCommand;
        public ICommand GoToNetworkCommand
        {
            get
            {
                return _resetNetworkCommand ?? (_resetNetworkCommand = new RelayCommand(
                    param => DoGoToNetwork(),
                    param => CanGoToNetwork()
                    ));
            }
        }

        private void DoGoToNetwork()
        {
            _mainWindowTypehanged.OnNext(new NetworkVm(Network.DoubleRing(128, 1234)));
        }

        bool CanGoToNetwork()
        {
            return (NetworkBuilderVm != null);
        }

        #endregion // GoToNetworkCommand

        #region GoToSandboxCommand

        RelayCommand _goToSandboxCommand;
        public ICommand GoToSandboxCommand
        {
            get
            {
                return _goToSandboxCommand ?? (_goToSandboxCommand = new RelayCommand(
                    param => DoGoToSandbox(),
                    param => CanGoToSandbox()
                    ));
            }
        }

        private void DoGoToSandbox()
        {
            _mainWindowTypehanged.OnNext(new SandboxVm());
        }

        bool CanGoToSandbox()
        {
            return true;
        }

        #endregion // GoToSandboxCommand


        private ObservableCollection<NetworkBuilderVm> _networkBuilderVms;
        public ObservableCollection<NetworkBuilderVm> NetworkBuilderVms
        {
            get { return _networkBuilderVms; }
            set { _networkBuilderVms = value; }
        }

        private NetworkBuilderVm _networkBuilderVm;
        public NetworkBuilderVm NetworkBuilderVm
        {
            get { return _networkBuilderVm; }
            set
            {
                _networkBuilderVm = value;
                OnPropertyChanged("NetworkBuilderVm");
            }
        }
    }
}
