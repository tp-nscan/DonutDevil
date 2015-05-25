using System;

namespace La.ViewModel
{
    public interface IMainWindowVm
    {
        MainWindowType MainWindowType { get; }
        IObservable<IMainWindowVm> OnMainWindowTypeChanged { get; }
    }

    public enum MainWindowType
    {
        Menu,
        Network,
        Sandbox
    }
}
