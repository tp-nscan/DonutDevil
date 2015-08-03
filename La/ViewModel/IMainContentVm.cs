using System;
using LibNode;

namespace La.ViewModel
{
    public interface IMainContentVm
    {
        IEntityRepo EntityRepo { get; }
        MainContentType MainContentType { get; }
        IObservable<IMainContentVm> OnMainWindowTypeChanged { get; }
    }

    public enum MainContentType
    {
        Menu,
        Network,
        Sandbox,
        Zeus,
        What
    }
}
