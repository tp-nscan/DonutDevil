﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Windows.Input;
using DonutDevilControls.ViewModel.Common;
using La.Model;
using LibNode;
using WpfUtils;

namespace La.ViewModel
{
    public class MenuVm : NotifyPropertyChanged, IMainContentVm
    {
        public MenuVm()
        {
            _networkBuilderVms = new List<NetworkBuilderVm>
                (
                    //NetworkBuilder.CurrentBuilders.Select(cb=> new NetworkBuilderVm(cb))
                );
        }

        #region Navigation

        public IEntityRepo EntityRepo
        {
            get { return _entityRepo; }
        }

        public MainContentType MainContentType => MainContentType.Menu;

        private readonly Subject<IMainContentVm> _mainWindowTypehanged
            = new Subject<IMainContentVm>();

        public IObservable<IMainContentVm> OnMainWindowTypeChanged => _mainWindowTypehanged;

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
            _mainWindowTypehanged.OnNext(
                new NetworkVm(DesignNetwork)
                );
        }

        bool CanGoToNetwork()
        {
            return true; // (NetworkBuilderVm != null);
        }

        public static ISym DesignNetwork
        {
            get
            {
                return null;// Network.MakeLinearNetwork(memLength:256, memCount:25);
            }
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


        private readonly List<NetworkBuilderVm> _networkBuilderVms;
        public IList NetworkBuilderVms => _networkBuilderVms;

        private NetworkBuilderVm _networkBuilderVm;
        private IEntityRepo _entityRepo;

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
