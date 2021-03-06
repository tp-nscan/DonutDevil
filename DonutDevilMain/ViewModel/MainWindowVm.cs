﻿using System;
using WpfUtils;

namespace DonutDevilMain.ViewModel
{
    public class MainWindowVm : NotifyPropertyChanged
    {

        public MainWindowVm()
        {
            ContentVm = new MenuVm();
        }

        private IMainWindowVm _contentVm;
        public IMainWindowVm ContentVm
        {
            get { return _contentVm; }
            set
            {
                if (_subscripton != null)
                {
                    _subscripton.Dispose();
                }

                _contentVm = value;
                _subscripton =_contentVm.OnMainWindowTypeChanged.Subscribe(vm => ContentVm = vm);
                OnPropertyChanged("ContentVm");
            }
        }

        private IDisposable _subscripton;
    }
}
